using System;
using Dreamland.Core.Vision.Match;

namespace Dreamland.Core.Simulate.Interfaces
{
    /// <summary>
    ///     控件代理
    /// <para>基于SIFT特征点匹配方式搜索匹配的控件</para>
    /// </summary>
    public interface IControlProxy : IDisposable
    {
        /// <summary>
        ///     搜索参数
        /// </summary>
        FeatureMatchArgument MatchArgument { get; }

        /// <summary>
        ///     是否是在全屏中搜索
        /// </summary>
        bool IsSearchScreen { get; }

        /// <summary>
        ///     搜索控件
        /// </summary>
        /// <param name="matchResult"></param>
        /// <returns></returns>
        bool Search(out FeatureMatchResult matchResult);

        /// <summary>
        ///     执行点击操作
        /// </summary>
        /// <returns></returns>
        bool Click(MouseButtons buttons = MouseButtons.Left);

        /// <summary>
        ///     执行双击操作
        /// </summary>
        /// <returns></returns>
        bool DoubleClick(MouseButtons buttons = MouseButtons.Left);
    }
}
