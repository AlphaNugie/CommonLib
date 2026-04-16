using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufNetLibrary.Enums
{
    /// <summary>
    /// 通过ProtobufNetLibrary传输的消息类型
    /// </summary>
    public enum ProtoInfoType
    {
        /// <summary>
        /// GNSS消息
        /// </summary>
        GNSS = 1,

        /// <summary>
        /// 雷达消息
        /// </summary>
        RADAR = 2,

        /// <summary>
        /// 雷达详细信息
        /// </summary>
        RADAR_DETAIL = 3,

        /// <summary>
        /// GPS进程通信控制
        /// </summary>
        GNSS_PROC_CTRL = 4,

        /// <summary>
        /// 扫描仪消息
        /// </summary>
        SCANNER_BUILDER = 5,
    }
}
