using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommonLib.UIControlUtil.WPF
{
    /// <summary>
    /// 窗口信息
    /// </summary>
    public class WindowStateInfo
    {
        /// <summary>
        /// 代表这套信息的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 窗口状态
        /// </summary>
        public WindowState WindowState { get; set; }

        /// <summary>
        /// 窗口边框类型
        /// </summary>
        public WindowStyle WindowStyle { get; set; }

        /// <summary>
        /// 缩放方式
        /// </summary>
        public ResizeMode ResizeMode { get; set; }

        /// <summary>
        /// 窗口左边缘相对于桌面的位置（像素）
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// 窗口上边缘相对于桌面的位置
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// 窗口宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 窗口高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="name">窗口信息名称</param>
        public WindowStateInfo(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 获取全屏所需
        /// </summary>
        /// <returns></returns>
        public static WindowStateInfo GetFullScreenInfo()
        {
            WindowStateInfo info = new WindowStateInfo("FullScreenInfo")
            {
                WindowState = WindowState.Normal,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Left = 0,
                Top = 0,
                Width = SystemParameters.PrimaryScreenWidth,
                Height = SystemParameters.PrimaryScreenHeight
            };
            return info;
        }
    }
}
