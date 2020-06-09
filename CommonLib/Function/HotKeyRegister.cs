using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLib.Enums;

namespace CommonLib.Function
{
    /// <summary>
    /// 热键注册类
    /// </summary>
    public static class HotKeyRegister
    {
        /// <summary>
        /// 注册热键
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="id">待注册热键的识别ID</param>
        /// <param name="control">组合键代码：Alt - 1，Ctrl - 2，Alt + Ctrl - 3，Shift - 4，Alt + Shift - 5，Ctrl + Shift - 6，Alt + Ctrl + Shift - 7，Win - 8
        /// </param>
        /// <param name="vk">键值</param>
        /// <returns>假如注册成功，返回true，否则返回false</returns>
        [DllImport("user32")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint control, int vk);

        /// <summary>
        /// 热键取消注册
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="id">已注册热键的识别ID</param>
        /// <returns>假如取消注册成功，返回true，否则返回false</returns>
        [DllImport("user32")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// 注册热键
        /// </summary>
        /// <param name="winHandle">窗口句柄</param>
        /// <param name="id">待注册热键的识别ID</param>
        /// <param name="hotKeys">热键中的组合键：Ctrl, Alt, Shift或它们的组合</param>
        /// <param name="key">热键中的配合按键</param>
        /// <returns>假如注册成功，返回true，否则返回false</returns>
        public static bool Register(IntPtr winHandle, int id, HotKeys hotKeys, Keys key)
        {
            return RegisterHotKey(winHandle, id, (uint)hotKeys, (int)key);
        }

        /// <summary>
        /// 热键取消注册
        /// </summary>
        /// <param name="winHandle">窗口句柄</param>
        /// <param name="id">已注册热键的识别ID</param>
        /// <returns>假如取消注册成功，返回true，否则返回false</returns>
        public static bool Unregister(IntPtr winHandle, int id)
        {
            return UnregisterHotKey(winHandle, id);
        }
    }
}
