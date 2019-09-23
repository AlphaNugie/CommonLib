using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Events
{
    /// <summary>
    /// 数据发送事件的事件数据类
    /// </summary>
    public class DataSendEventArgs : EventArgs
    {
        /// <summary>
        /// 发送的byte数组
        /// </summary>
        public byte[] SentData { get; set; }

        /// <summary>
        /// 发送的字符串
        /// </summary>
        public string SentInfo { get; set; }
    }
}
