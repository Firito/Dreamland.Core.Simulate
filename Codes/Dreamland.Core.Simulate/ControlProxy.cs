using System;
using System.Collections.Generic;
using Dreamland.Core.Simulate.Interfaces;
using Dreamland.Core.Vision.Capture;
using Dreamland.Core.Vision.Match;
using OpenCvSharp;
using PInvoke;

namespace Dreamland.Core.Simulate
{
    /// <summary>
    ///     控件代理
    /// <para>基于SIFT特征点匹配方式搜索匹配的控件</para>
    /// </summary>
    public class ControlProxy : IControlProxy
    {
        /// <summary>
        ///     该控件所在窗口的句柄
        /// <para>在设置后则只在指定窗口进行搜索</para>
        /// </summary>
        private readonly IntPtr _windowHandle;

        /// <summary>
        ///     搜索
        /// </summary>
        private readonly List<Mat> _searchMats = new List<Mat>();

        #region 构造函数

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="searchImage">控件描述图像，将用该图片进行搜索</param>
        /// <param name="hWnd"> 该控件所在窗口的句柄，在设置后则只在指定窗口进行搜索</param>
        public ControlProxy(Mat searchImage, IntPtr hWnd)
        {
            _windowHandle = hWnd;
            _searchMats.Add(searchImage);
        }

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="searchImage">控件描述图像，将用该图片进行搜索</param>
        public ControlProxy(Mat searchImage) : this(searchImage, IntPtr.Zero)
        {
        }

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="searchImagePath">控件描述图像，将用该图片进行搜索</param>
        /// <param name="hWnd"> 该控件所在窗口的句柄，在设置后则只在指定窗口进行搜索</param>
        public ControlProxy(string searchImagePath, IntPtr hWnd) : this(new Mat(searchImagePath), hWnd)
        {
        }

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="searchImagePath">控件描述图像，将用该图片进行搜索</param>
        public ControlProxy(string searchImagePath) : this(new Mat(searchImagePath), IntPtr.Zero)
        {
        }

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="searchImages">控件描述图像列表，将用这些图片进行搜索</param>
        public ControlProxy(IReadOnlyList<Mat> searchImages) : this(searchImages, IntPtr.Zero)
        {
        }

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="searchImages">控件描述图像列表，将用这些图片进行搜索</param>
        /// <param name="hWnd"> 该控件所在窗口的句柄，在设置后则只在指定窗口进行搜索</param>
        public ControlProxy(IReadOnlyList<Mat> searchImages, IntPtr hWnd)
        {
            _searchMats.AddRange(searchImages);
            _windowHandle = hWnd;
        }

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="searchImagePaths">控件描述图像列表，将用这些图片进行搜索</param>
        public ControlProxy(IReadOnlyList<string> searchImagePaths) : this(searchImagePaths, IntPtr.Zero)
        {
        }

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="searchImagePaths">控件描述图像列表，将用这些图片进行搜索</param>
        /// <param name="hWnd"> 该控件所在窗口的句柄，在设置后则只在指定窗口进行搜索</param>
        public ControlProxy(IReadOnlyList<string> searchImagePaths, IntPtr hWnd)
        {
            foreach (var path in searchImagePaths)
            {
                var mat = new Mat(path);
                _searchMats.Add(mat);
            }

            _windowHandle = hWnd;
        }

        #endregion

        /// <summary>
        ///     控件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     搜索参数
        /// </summary>
        public FeatureMatchArgument MatchArgument { get; } = new FeatureMatchArgument()
        {
            Ratio = 0.2,
            RansacThreshold = 2
        };

        /// <summary>
        ///     是否是在全屏中搜索
        /// </summary>
        public bool IsSearchScreen => _windowHandle == IntPtr.Zero;

        /// <summary>
        ///     搜索控件
        /// </summary>
        /// <param name="matchResult"></param>
        /// <returns></returns>
        public bool Search(out FeatureMatchResult matchResult)
        {
            matchResult = null;
            var captor = CaptorFactory.GetCaptor();
            using var captureWindow = IsSearchScreen ? captor.CaptureScreen() : captor.CaptureWindow(_windowHandle);

            foreach (var searchMat in _searchMats)
            {
                matchResult = CvMatch.FeatureMatch(captureWindow, searchMat, FeatureMatchType.Sift, MatchArgument);
                if (matchResult.Success)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     执行点击操作
        /// </summary>
        /// <returns></returns>
        public bool Click(MouseButtons buttons = MouseButtons.Left)
        {
            if (!Search(out var matchResult))
            {
                return false;
            }

            var point = IsSearchScreen
                ? matchResult.MatchItems[0].Point
                : ToAbsolutePoint(_windowHandle, matchResult.MatchItems[0].Point);
            Mouse.Click(buttons, point);
            return true;
        }

        /// <summary>
        ///     执行双击操作
        /// </summary>
        /// <returns></returns>
        public bool DoubleClick(MouseButtons buttons = MouseButtons.Left)
        {
            if (!Search(out var matchResult))
            {
                return false;
            }

            var point = IsSearchScreen
                ? matchResult.MatchItems[0].Point
                : ToAbsolutePoint(_windowHandle, matchResult.MatchItems[0].Point);
            Mouse.DoubleClick(buttons, point);
            return true;
        }

        /// <summary>
        ///     从窗口坐标转为相对屏幕的绝对坐标
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private static System.Drawing.Point ToAbsolutePoint(IntPtr hWnd, System.Drawing.Point point)
        {
            User32.GetWindowRect(hWnd, out var windowPoint);
            point.Offset(windowPoint.left, windowPoint.top);
            return point;
        }

        public void Dispose()
        {
            _searchMats.ForEach(x => x.Dispose());
            _searchMats.Clear();
        }
    }
}
