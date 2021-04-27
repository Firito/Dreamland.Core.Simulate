using System;
using System.Drawing;
using Dreamland.Core.Simulate.Interfaces;
using PInvoke;

namespace Dreamland.Core.Simulate.Extensions
{
    /// <summary>
    ///     <see cref="ISimulateTask"/>和<see cref="ISimulateTasks"/>的静态拓展方法
    /// </summary>
    public static class SimulateTaskExtension
    {
        /// <summary>
        ///     随机数升级器
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        ///     在指定<paramref name="rect"/>区域内获取随机点
        /// </summary>
        /// <param name="rect">随机点区间</param>
        /// <returns></returns>
        public static Point GetRandomPoint(this RECT rect)
        {
            var x = Random.Next(rect.left, rect.right);
            var y = Random.Next(rect.top, rect.bottom);
            return new Point(x, y);
        }

        /// <summary>
        ///     获取一个当前<see cref="ISimulateTasks"/>的随机执行顺序
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static int[] GetRandomIndex(this ISimulateTasks tasks)
        {
            var taskCount = tasks.Tasks.Count;
            var indexArray = new int[taskCount];

            //对顺序数组元素赋值
            for (var i = 0; i < taskCount; i++)
            {
                indexArray[i] = i;
            }

            //随机分布
            for (var i = 0; i < taskCount; i++)
            {
                var index = Random.Next(taskCount);
                var value = indexArray[i];
                indexArray[i] = indexArray[index];
                indexArray[index] = value;
            }

            return indexArray;
        }
    }
}
