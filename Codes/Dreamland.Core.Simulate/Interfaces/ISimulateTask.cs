using System;
using System.Threading.Tasks;

namespace Dreamland.Core.Simulate.Interfaces
{
    /// <summary>
    ///     模拟操作的任务
    /// </summary>
    public interface ISimulateTask : IDisposable
    {
        /// <summary>
        ///     任务名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     运行该任务
        /// </summary>
        /// <returns></returns>
        Task<SimulateResult> RunAsync();

        /// <summary>
        ///     运行该任务
        /// </summary>
        /// <param name="beginAction">开始任务时的委托</param>
        /// <param name="endAction">结束任务时的委托</param>
        /// <returns></returns>
        Task<SimulateResult> RunAsync(Action<ISimulateTask> beginAction, Action<ISimulateTask, SimulateResult> endAction);

        /// <summary>
        ///     获取最后一次执行失败的信息
        /// </summary>
        public string GetLastError();
    }
}
