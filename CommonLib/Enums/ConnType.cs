using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enums
{
    /// <summary>
    /// 连接方式
    /// </summary>
    public enum ConnTypes
    {
        /// <summary>
        /// 用户数据报协议（User Datagram Protocol）
        /// </summary>
        UDP = 0,

        /// <summary>
        /// 传输控制协议（Transmission Control Protocol）
        /// </summary>
        TCP = 1,

        /// <summary>
        /// 未连接
        /// </summary>
        UNCONNECTED = 2
    }
}
