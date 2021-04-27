using System.Collections.Generic;
using Dreamland.Core.Simulate.Interfaces;

namespace Dreamland.Core.Simulate
{
    /// <summary>
    ///     模拟执行的结果
    /// </summary>
    public class SimulateResult
    {
        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="success">全部任务执行成功</param>
        public SimulateResult(bool success) : this(success, null, null)
        {

        }

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="success">全部任务执行成功</param>
        /// <param name="failedTasks">执行失败的任务</param>
        /// <param name="unenforcedTasks">未执行的任务</param>
        public SimulateResult(bool success, IList<ISimulateTask> failedTasks, IList<ISimulateTask> unenforcedTasks)
        {
            Success = success;
            FailedTasks = failedTasks ?? new List<ISimulateTask>();
            UnenforcedTasks = unenforcedTasks ?? new List<ISimulateTask>();
        }

        /// <summary>
        ///     全部任务执行成功
        /// </summary>
        public bool Success { get; }

        /// <summary>
        ///     执行失败的任务
        /// </summary>
        public IList<ISimulateTask> FailedTasks { get; }

        /// <summary>
        ///     未执行的任务
        /// </summary>
        public IList<ISimulateTask> UnenforcedTasks { get; }
    }
}
