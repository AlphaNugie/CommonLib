using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Events
{
    /// <summary>
    /// 持续未接收到任何数据的事件数据类
    /// </summary>
    public class NoneReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 触发时间阈值（单位毫秒）
        /// </summary>
        public ulong RaiseThreshold { get; set; }

        /// <summary>
        /// 发送所接收数据的套接字
        /// </summary>
        public Socket Socket { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public NoneReceivedEventArgs() { }

        /// <summary>
        /// 触发事件阈值初始化
        /// </summary>
        /// <param name="threshold">触发时间阈值（单位毫秒）</param>
        public NoneReceivedEventArgs(ulong threshold)
        {
            RaiseThreshold = threshold;
        }
    }
}
