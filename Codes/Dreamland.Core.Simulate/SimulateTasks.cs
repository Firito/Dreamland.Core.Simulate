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
        /// <param name="useRandomOrder">是否使用随机顺序运行<see cref="Tasks"/>中的<see cref="ISimulateTask"/></param>
        /// <returns></returns>
        public async Task<SimulateResult> RunAsync(bool useRandomOrder = false)
        {
            var taskCount = Tasks.Count;
            for (var i = 0; i < taskCount; i++)
            {
                var result = await Tasks[i].RunAsync();
                if (result.Success)
                {
                    continue;
                }

                //如果当前任务执行失败，则创建失败结果
                var simulateResult = new SimulateResult(false);
                simulateResult.FailedTasks.Add(Tasks[i]);
                for (var j = i + 1; j < taskCount; j++)
                {
                    simulateResult.UnenforcedTasks.Add(Tasks[i]);
                }
                return simulateResult;
            }

            return new SimulateResult(true);
        }

        /// <summary>
        ///     运行该任务组，忽略执行过程中出现的失败直到所有任务执行完毕
        /// </summary>
        /// <param name="useRandomOrder"></param>
        /// <returns></returns>
        public async Task<SimulateResult> RunIgnoreFailureAsync(bool useRandomOrder = false)
        {
            var failedTasks = new List<ISimulateTask>();

            if (useRandomOrder)
            {
                var indexOrder = this.GetRandomIndex();
                foreach (var index in indexOrder)
                {
                    var task = Tasks[index];
                    var result = await task.RunAsync();
                    if (!result.Success)
                    {
                        failedTasks.Add(task);
                    }
                }
            }
            else
            {
                foreach (var task in Tasks)
                {
                    var result = await task.RunAsync();
                    if (!result.Success)
                    {
                        failedTasks.Add(task);
                    }
                }
            }

            return failedTasks.Any() 
                ? new SimulateResult(false, failedTasks, null) 
                : new SimulateResult(true);
        }
    }
}
