using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dreamland.Core.Simulate.Interfaces;

namespace Dreamland.Core.Simulate
{
    /// <summary>
    ///     控件任务
    /// </summary>
    public abstract class SimulateTask : ISimulateTask
    {
        /// <summary>
        ///     最后一次执行失败的信息
        /// </summary>
        protected string LastError { get; set; }

        /// <summary>
        ///     所属该任务的控件代理
        /// </summary>
        protected Dictionary<string, ControlProxy> ControlProxies { get; } = new Dictionary<string, ControlProxy>();

        /// <summary>
        ///     任务名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     运行该任务
        /// </summary>
        /// <returns></returns>
        public abstract Task<SimulateResult> RunAsync();

        /// <summary>
        ///     运行该任务
        /// </summary>
        /// <param name="beginAction">开始任务时的委托</param>
        /// <param name="endAction">结束任务时的委托</param>
        /// <returns></returns>
        public async Task<SimulateResult> RunAsync(Action<ISimulateTask> beginAction, Action<ISimulateTask, SimulateResult> endAction)
        {
            beginAction?.Invoke(this);
            var result = await RunAsync();
            endAction?.Invoke(this, result);
            return result;
        }

        /// <summary>
        ///     获取最后一次执行失败的信息
        /// </summary>
        public string GetLastError() => LastError;
            
        /// <summary>
        ///     注销
        /// </summary>
        public void Dispose()
        {
            foreach (var keyValuePair in ControlProxies)
            {
                keyValuePair.Value?.Dispose();
            }
            ControlProxies.Clear();
        }
    }
}
