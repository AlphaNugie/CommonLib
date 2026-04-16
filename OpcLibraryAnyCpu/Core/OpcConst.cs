using CommonLib.Clients;
using CommonLib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

#if DA
namespace OpcLibrary.Core
#elif UA
namespace OpcLibrary.Ua.Core
#endif
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
        /// OPC功能是否启用
        /// </summary>
        public static bool OpcEnabled { get; set; }

        /// <summary>
        /// OPC架构类型，默认为 <see cref="OpcConstructureType.OpcDa"/>
        /// </summary>
        public static OpcConstructureType OpcConstructureType { get; set; } = OpcConstructureType.OpcDa;

        /// <summary>
        /// OPC SERVER IP地址
        /// <para/>对于UA，将成为服务地址“opc.tcp://[OpcServerIp]:[OpcServerPort][/[OpcServerName]]”的一部分
        /// </summary>
        public static string OpcServerIp { get; set; }

        /// <summary>
        /// OPC SERVER 名称
        /// <para/>对于DA为服务名称；对于UA，假如不为空，将在URL最后添加“/[OpcServerName]”
        /// </summary>
        public static string OpcServerName { get; set; }

#if UA
        /// <summary>
        /// OPC UA 服务的完整名称，形式为“opc.tcp://[OpcServerIp]:[OpcServerPort][/[OpcServerName]]”
        /// </summary>
        public static string OpcServerUrl
        {
            get
            {
                return OpcUtilHelper.GetOpcServerUrl(OpcServerIp, OpcServerPort, OpcServerName);
                //return string.Format("opc.tcp://{0}:{1}{2}",
                //    OpcServerIp,
                //    OpcServerPort,
                //    string.IsNullOrWhiteSpace(OpcServerName) ? string.Empty : ("/" + OpcServerName));
            }
        }

        /// <summary>
        /// OPC SERVER 端口
        /// <para/>仅对UA有效，将成为服务地址“opc.tcp://[OpcServerIp]:[OpcServerPort][/[OpcServerName]]”的一部分
        /// <para/>KEPServerV6默认端口号为49320
        /// </summary>
        public static int OpcServerPort { get; set; }

        /// <summary>
        /// 用户名（仅对UA有效）
        /// </summary>
        public static string UserName { get; set; }

        /// <summary>
        /// 密码（仅对UA有效）
        /// </summary>
        public static string Password { get; set; }
#endif

                /// <summary>
                /// 是否写入PLC
                /// </summary>
        public static bool Write2Plc { get; set; }

        /// <summary>
        /// OPC读取与写入间隔（毫秒）
        /// </summary>
        public static int OpcLoopInterval { get; set; }

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

    /// <summary>
    /// OPC架构类型
    /// </summary>
    public enum OpcConstructureType
    {
        /// <summary>
        /// OPC DA架构
        /// </summary>
        OpcDa = 0,

        /// <summary>
        /// OPC UA架构
        /// </summary>
        OpcUa
    }
}
