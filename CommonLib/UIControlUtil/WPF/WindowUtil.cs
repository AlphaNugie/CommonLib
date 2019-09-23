using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommonLib.UIControlUtil.WPF
{
    /// <summary>
    /// WPF窗口功能类
    /// </summary>
    public static class WindowUtil
    {
        /// <summary>
        /// 保存窗口信息
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static WindowStateInfo SaveWindowStateInfo(this Window window)
        {
            if (window == null)
                throw new NullReferenceException("预备载入窗口信息的WPF窗口为空");

            WindowStateInfo info = new WindowStateInfo("FullScreenInfo")
            {
                WindowState = window.WindowState,
                WindowStyle = window.WindowStyle,
                ResizeMode = window.ResizeMode,
                Left = window.Left,
                Top = window.Top,
                Width = window.Width,
                Height = window.Height
            };
            return info;
        }

        /// <summary>
        /// 为WPF窗口载入窗口信息（如状态、边框格式、宽高等）
        /// </summary>
        /// <param name="window">WPF窗口对象</param>
        /// <param name="info">待载入窗口库信息</param>
        public static void LoadWindowStateInfo(this Window window, WindowStateInfo info)
        {
            if (window == null)
                throw new NullReferenceException("预备载入窗口信息的WPF窗口为空");
            if (info == null)
                throw new ArgumentNullException("info", "预备载入的窗口信息为空");

            window.WindowState = info.WindowState;
            window.WindowStyle = info.WindowStyle;
            window.ResizeMode = info.ResizeMode;
            window.Left = info.Left;
            window.Top = info.Top;
            window.Width = info.Width;
            window.Height = info.Height;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="window"></param>
        public static void SetFullScreen(this Window window)
        {
            window.LoadWindowStateInfo(Base.FullScreenInfo);
        }
    }
}
