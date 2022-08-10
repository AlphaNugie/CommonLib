using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Diagnostics;
using CommonLib.Clients;

namespace CommonLib.UIControlUtil.WinForms
{
    /// <summary>
    /// 窗体功能类
    /// </summary>
    public static class WindowUtil
    {
        /// <summary>
        /// 根据主窗体句柄显示或隐藏窗体
        /// </summary>
        /// <param name="hWnd">主窗体句柄</param>
        /// <param name="nCmdShow">0 隐藏（不关闭），1 显示</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        /// <summary>
        /// 显示或隐藏进程的窗体
        /// </summary>
        /// <param name="process">待操作的进程</param>
        /// <param name="nCmdShow">0 隐藏（不关闭），1 显示</param>
        /// <returns></returns>
        public static bool ShowWindow(this Process process, uint nCmdShow)
        {
            return process != null && ShowWindow(process.MainWindowHandle, nCmdShow);
            //return process != null && ShowWindow(process.Handle, nCmdShow);
        }

        ///// <summary>
        ///// 显示或隐藏进程的窗体
        ///// </summary>
        ///// <param name="process">待操作的进程</param>
        ///// <param name="hWnd"></param>
        ///// <param name="nCmdShow">0 隐藏（不关闭），1 显示</param>
        ///// <returns></returns>
        //public static bool ShowWindow(this Process process, IntPtr hWnd, uint nCmdShow)
        //{
        //    return process != null && ShowWindow(hWnd, nCmdShow);
        //    //return process != null && ShowWindow(process.Handle, nCmdShow);
        //}
    }
}
