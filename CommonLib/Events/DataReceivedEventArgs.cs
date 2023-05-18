using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Function;

namespace CommonLib.Events
{
    /// <summary>
    /// 数据接收事件的事件数据类
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 接收到的byte数组
        /// </summary>
        public byte[] ReceivedData { get; set; }

        /// <summary>
        /// 接收数据的字符串格式
        /// </summary>
        public string ReceivedInfo_String { get; set; }

        /// <summary>
        /// 接收数据的16进制字符串格式
        /// </summary>
        public string ReceivedInfo_HexString { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public DataReceivedEventArgs() { }

        /// <summary>
        /// 用byte数组初始化
        /// </summary>
        /// <param name="data">接收到的byte[]数组</param>
        public DataReceivedEventArgs(byte[] data)
        {
            ReceivedData = data;
            if (data != null && data.Length > 0)
            {
                ReceivedInfo_HexString = HexHelper.ByteArray2HexString(data);
                ReceivedInfo_String = Encoding.Default.GetString(data);
            }
            else
            {
                ReceivedInfo_HexString = string.Empty;
                ReceivedInfo_String = string.Empty;
            }
        }
    }
}
