using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dreamland.Core.Simulate.Interfaces;
using Dreamland.Core.Simulate.Extensions;

namespace Dreamland.Core.Simulate
{
    /// <summary>
    ///     控件任务组
    /// </summary>
    public class SimulateTasks : ISimulateTasks
    {
        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tasks"></param>
        public SimulateTasks(string name, IList<ISimulateTask> tasks)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
        }

        /// <summary>
        ///     任务组名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     任务组包含的任务项列表
        /// </summary>
        public IList<ISimulateTask> Tasks { get; } 

        /// <summary>
        ///     运行该任务组，执行过程中出现失败则直接结束
        /// </summary>
        /// <param name="millisecondsDelay">每个任务之间间隔的时间</param>
        /// <param name="useRandomOrder">是否使用随机顺序运行<see cref="Tasks"/>中的<see cref="ISimulateTask"/></param>
        /// <returns></returns>
        public Task<SimulateResult> RunAsync(uint millisecondsDelay, bool useRandomOrder = false)
        {
            return RunAsync(millisecondsDelay, null, null, useRandomOrder);
        }

        /// <summary>
        ///     运行该任务
        /// </summary>
        /// <param name="millisecondsDelay">每个任务之间间隔的时间</param>
        /// <param name="beginAction">开始任务时的委托</param>
        /// <param name="endAction">结束任务时的委托</param>
        /// <param name="useRandomOrder">是否使用随机顺序运行<see cref="Tasks"/>中的<see cref="ISimulateTask"/></param>
        /// <returns></returns>
        public async Task<SimulateResult> RunAsync(uint millisecondsDelay, Action<ISimulateTask> beginAction,
            Action<ISimulateTask, SimulateResult> endAction, bool useRandomOrder = false)
        {
            var sortedTasks = GetSortedSimulateTasks(useRandomOrder);
            var taskCount = sortedTasks.Count;
            for (var i = 0; i < taskCount; i++)
            {
                var result = await sortedTasks[i].RunAsync(beginAction, endAction);
                if (result.Success)
                {
                    if (i < taskCount -1 && millisecondsDelay > 0)
                    {
                        await Task.Delay((int)millisecondsDelay);
                    }
                    continue;
                }

                //如果当前任务执行失败，则创建失败结果
                var simulateResult = new SimulateResult(false);
                simulateResult.FailedTasks.Add(sortedTasks[i]);
                for (var j = i + 1; j < taskCount; j++)
                {
                    simulateResult.UnenforcedTasks.Add(sortedTasks[j]);
                }
                return simulateResult;
            }

            return new SimulateResult(true);
        }

        /// <summary>
        ///     运行该任务组，忽略执行过程中出现的失败直到所有任务执行完毕
        /// </summary>
        /// <param name="millisecondsDelay">每个任务之间间隔的时间</param>
        /// <param name="useRandomOrder">是否使用随机顺序运行<see cref="Tasks"/>中的<see cref="ISimulateTask"/></param>
        /// <returns></returns>
        public Task<SimulateResult> RunIgnoreFailureAsync(uint millisecondsDelay, bool useRandomOrder = false)
        {
            return RunIgnoreFailureAsync(millisecondsDelay, null, null, useRandomOrder);
        }

        /// <summary>
        ///     运行该任务组，忽略执行过程中出现的失败直到所有任务执行完毕
        /// </summary>
        /// <param name="millisecondsDelay">每个任务之间间隔的时间</param>
        /// <param name="beginAction">开始任务时的委托</param>
        /// <param name="endAction">结束任务时的委托</param>
        /// <param name="useRandomOrder">是否使用随机顺序运行<see cref="Tasks"/>中的<see cref="ISimulateTask"/></param>
        /// <returns></returns>
        public async Task<SimulateResult> RunIgnoreFailureAsync(uint millisecondsDelay, Action<ISimulateTask> beginAction,
            Action<ISimulateTask, SimulateResult> endAction, bool useRandomOrder = false)
        {
            var failedTasks = new List<ISimulateTask>();
            var sortedTasks = GetSortedSimulateTasks(useRandomOrder);
            var taskCount = sortedTasks.Count;

            for (var i = 0; i < taskCount; i++)
            {
                var result = await sortedTasks[i].RunAsync(beginAction, endAction);
                if (i < taskCount -1 && millisecondsDelay > 0)
                {
                    await Task.Delay((int)millisecondsDelay);
                }
                if (!result.Success)
                {
                    failedTasks.Add(sortedTasks[i]);
                }
            }

            return failedTasks.Any() 
                ? new SimulateResult(false, failedTasks, null) 
                : new SimulateResult(true);
        }

        /// <summary>
        ///     获取排好执行顺序的任务
        /// </summary>
        /// <param name="useRandomOrder"></param>
        /// <returns></returns>
        private IList<ISimulateTask> GetSortedSimulateTasks(bool useRandomOrder)
        {
            var sortedTasks = new List<ISimulateTask>();
            if (useRandomOrder)
            {
                var indexOrder = this.GetRandomIndex();
                sortedTasks.AddRange(indexOrder.Select(index => Tasks[index]));
            }
            else
            {
                sortedTasks.AddRange(Tasks);
            }

            return sortedTasks;
        }
    }
}
