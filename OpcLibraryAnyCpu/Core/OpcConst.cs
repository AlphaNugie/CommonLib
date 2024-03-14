using CommonLib.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpcLibrary.Core
{
    /// <summary>
    /// OPC连接、读取或写入的基础参数
    /// </summary>
    public static class OpcConst
    {
        //private static readonly LogClient _log = new LogClient("logs", "intercomm", "executable.log", false, true);
        /// <summary>
        /// 日志
        /// </summary>
        public static LogClient Log { get; } = new LogClient("logs", "OpcTaskBase", "executable.log", false, true);
        //public static LogClient Log { get { return _log; } }

        /// <summary>
        /// Sqlite文件路径，可为相对路径
        /// </summary>
        public static string SqliteFileDir { get; set; }

        /// <summary>
        /// Sqlite文件名称，包括后缀
        /// </summary>
        public static string SqliteFileName { get; set; }

        #region OPC
        /// <summary>
        /// 是否使用OPC
        /// </summary>
        public static bool OpcEnabled { get; set; }

        /// <summary>
        /// OPC SERVER IP地址
        /// </summary>
        public static string OpcServerIp { get; set; }

        /// <summary>
        /// OPC SERVER 名称
        /// </summary>
        public static string OpcServerName { get; set; }

        /// <summary>
        /// 是否写入PLC
        /// </summary>
        public static bool Write2Plc { get; set; }

        /// <summary>
        /// OPC读取与写入间隔（毫秒）
        /// </summary>
        public static int OpcLoopInterval { get; set; }
        #endregion

        /// <summary>
        /// 写入日志同时在控制台输出
        /// </summary>
        /// <param name="info"></param>
        public static void WriteConsoleLog(string info)
        {
            Log.WriteLogsToFile(info);
            Console.WriteLine(info);
        }
    }
}
