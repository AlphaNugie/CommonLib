using CommonLib.Clients;
using CommonLib.Helpers;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcLibrary.SiemensPlcNet452.Core
{
    /// <summary>
    /// OPC连接、读取或写入的基础参数
    /// </summary>
    public static class SiemensConst
    {
        //private static readonly LogClient _log = new LogClient("logs", "intercomm", "executable.log", false, true);
        /// <summary>
        /// 日志
        /// </summary>
        public static LogClient Log { get; } = new LogClient("logs", "SiemensPlc", "executable.log", false, true);
        //public static LogClient Log { get { return _log; } }

        /// <summary>
        /// Sqlite文件路径，可为相对路径
        /// </summary>
        public static string SqliteFileDir { get; set; }

        /// <summary>
        /// Sqlite文件名称，包括后缀
        /// </summary>
        public static string SqliteFileName { get; set; }

        #region SIEMENS
        /// <summary>
        /// 是否启用与PLC的通信
        /// </summary>
        public static bool Enabled { get; set; }

        /// <summary>
        /// 西门子PLC的类型
        /// </summary>
        public static CpuType CpuType { get; set; }

        /// <summary>
        /// 西门子PLC的IP地址
        /// </summary>
        public static string PlcIp { get; set; }

        /// <summary>
        /// 机架号
        /// </summary>
        public static int Rack { get; set; }

        /// <summary>
        /// 插槽号
        /// </summary>
        public static int Slot { get; set; }

        /// <summary>
        /// 是否写入PLC
        /// </summary>
        public static bool Write2Plc { get; set; }

        /// <summary>
        /// 读取与写入间隔（毫秒）
        /// </summary>
        public static int LoopInterval { get; set; }

        private static string _schemaFile;
        /// <summary>
        /// 数据源内各变量值的描述文件的完整路径（文件内容以JSON格式提供，为避免中文乱码请使用UTF-8编码）
        /// </summary>
        public static string SchemaFile
        {
            get { return _schemaFile; }
            set
            {
                //假如给出的路径为空则不赋值
                if (string.IsNullOrWhiteSpace(value))
                    return;
                //假如为不包含盘符分隔符则添加启动路径
                else if (!value.Contains(FileSystemHelper.VolumeSeparator))
                    value = FileSystemHelper.StartupPath + value;
                _schemaFile = value;
            }
        }
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
