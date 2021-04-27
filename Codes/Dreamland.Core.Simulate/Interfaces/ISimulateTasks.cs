using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamland.Core.Simulate.Interfaces
{
    /// <summary>
    ///     由多个<see cref="ISimulateTask"/>组合的任务组
    /// </summary>
    public interface ISimulateTasks
    {
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
        Task<SimulateResult> RunAsync(uint millisecondsDelay, bool useRandomOrder = false);

        /// <summary>
        ///     运行该任务组，忽略执行过程中出现的失败直到所有任务执行完毕
        /// </summary>
        /// <param name="millisecondsDelay">每个任务之间间隔的时间</param>
        /// <param name="useRandomOrder">是否使用随机顺序运行<see cref="Tasks"/>中的<see cref="ISimulateTask"/></param>
        /// <returns></returns>
        Task<SimulateResult> RunIgnoreFailureAsync(uint millisecondsDelay, bool useRandomOrder = false);

        /// <summary>
        ///     运行该任务
        /// </summary>
        /// <param name="millisecondsDelay">每个任务之间间隔的时间</param>
        /// <param name="beginAction">开始任务时的委托</param>
        /// <param name="endAction">结束任务时的委托</param>
        /// <param name="useRandomOrder">是否使用随机顺序运行<see cref="Tasks"/>中的<see cref="ISimulateTask"/></param>
        /// <returns></returns>
        Task<SimulateResult> RunAsync(uint millisecondsDelay, Action<ISimulateTask> beginAction,
            Action<ISimulateTask, SimulateResult> endAction, bool useRandomOrder = false);

        /// <summary>
        ///     运行该任务组，忽略执行过程中出现的失败直到所有任务执行完毕
        /// </summary>
        /// <param name="millisecondsDelay">每个任务之间间隔的时间</param>
        /// <param name="beginAction">开始任务时的委托</param>
        /// <param name="endAction">结束任务时的委托</param>
        /// <param name="useRandomOrder">是否使用随机顺序运行<see cref="Tasks"/>中的<see cref="ISimulateTask"/></param>
        /// <returns></returns>
        Task<SimulateResult> RunIgnoreFailureAsync(uint millisecondsDelay, Action<ISimulateTask> beginAction,
            Action<ISimulateTask, SimulateResult> endAction, bool useRandomOrder = false);
    }
}
