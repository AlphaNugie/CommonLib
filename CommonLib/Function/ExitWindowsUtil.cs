using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function
{
    /// <summary>
    /// Windows操作系统关闭、重启、注销操作类
    /// </summary>
    public static class ExitWindowsUtils
    {
        #region win32 api
        [StructLayout(LayoutKind.Sequential, Pack = 1)]

        private struct TokPriv1Luid
        {
            public int Count;

            public long Luid;

            public int Attr;
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
            ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool ExitWindowsEx(int flg, int rea);
        #endregion

        private const int SE_PRIVILEGE_ENABLED = 0x00000002;

        private const int TOKEN_QUERY = 0x00000008;

        private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        #region 操作系统操作标识（Exit Windows Flags）
        /// <summary>
        /// 结束所有进程，注销当前用户，显示登录对话框来切换用户
        /// </summary>
        private const int EWX_LOGOFF = 0x00000000;

        /// <summary>
        /// 只关闭系统，不重启，不关电源
        /// </summary>
        private const int EWX_SHUTDOWN = 0x00000001;

        /// <summary>
        /// 关闭系统，重新启动
        /// </summary>
        private const int EWX_REBOOT = 0x00000002;

        /// <summary>
        /// 关闭系统，关闭电源（只对支持此性质的系统有效）
        /// </summary>
        private const int EWX_POWEROFF = 0x00000008;

        /// <summary>
        /// 立即强制终止所有应用程序。使用此选项可能会是正在运行的程序丢失数据
        /// </summary>
        private const int EWX_FORCE = 0x00000004;

        /// <summary>
        /// 终止无响应的应用程序
        /// </summary>
        private const int EWX_FORCEIFHUNG = 0x00000010;
        #endregion

        public static void DoExitWin(int flg)
        {
            #region 给予当前进程操作权限
            //give current process SeShutdownPrivilege
            TokPriv1Luid tp;
            IntPtr hproc = GetCurrentProcess(), htok = IntPtr.Zero;
            if (!OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok))
                throw new Exception("Open Process Token fail");

            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;

            if (!LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid))
                throw new Exception("Lookup Privilege Value fail");
            if (!AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
                throw new Exception("Adjust Token Privileges fail");
            #endregion

            //Exit windows
            if (!ExitWindowsEx(flg, 0))
                throw new Exception("Exit Windows fail");
        }

        /// <summary>
        /// 重新启动操作系统
        /// </summary>
        /// <param name="force">是否强制终止所有应用程序</param>
        public static void Reboot(bool force)
        {
            DoExitWin(EWX_REBOOT | (force ? EWX_FORCE : EWX_FORCEIFHUNG));
            //if (force)
            //    DoExitWin(EWX_REBOOT | EWX_FORCE);
            //else
            //    DoExitWin(EWX_REBOOT | EWX_FORCEIFHUNG);
        }

        /// <summary>
        /// 重新启动操作系统，终止无响应的应用程序
        /// </summary>
        public static void Reboot()
        {
            Reboot(false);
        }

        /// <summary>
        /// 关闭操作系统
        /// </summary>
        /// <param name="force">是否强制终止所有应用程序</param>
        public static void Shutdown(bool force)
        {
            DoExitWin(EWX_SHUTDOWN | (force ? EWX_FORCE : EWX_FORCEIFHUNG));
            //if (force)
            //    DoExitWin(EWX_SHUTDOWN | EWX_FORCE);
            //else
            //    DoExitWin(EWX_SHUTDOWN | EWX_FORCEIFHUNG);
        }

        /// <summary>
        /// 关闭操作系统，终止无响应的应用程序
        /// </summary>
        public static void Shutdown()
        {
            Shutdown(false);
        }

        /// <summary>
        /// 注销当前用户
        /// </summary>
        /// <param name="force">是否强制终止所有应用程序</param>
        public static void Logoff(bool force)
        {
            DoExitWin(EWX_LOGOFF | (force ? EWX_FORCE : EWX_FORCEIFHUNG));
            //if (force)
            //    DoExitWin(EWX_LOGOFF | EWX_FORCE);
            //else
            //    DoExitWin(EWX_LOGOFF | EWX_FORCEIFHUNG);
        }

        /// <summary>
        /// 注销当前用户，终止无响应的应用程序
        /// </summary>
        public static void Logoff()
        {
            Logoff(false);
        }
    }
}
