using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Helpers
{
    /// <summary>
    /// DateTime类
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// 获取当前时间戳（精度毫秒）
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan tmp = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timeStamp = Convert.ToInt64(tmp.TotalMilliseconds).ToString();
            return timeStamp;
        }

        /// <summary>
        /// 获取当前时间戳（精度秒）
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStampBySeconds()
        {
            TimeSpan tmp = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timeStamp = Convert.ToInt64(tmp.TotalSeconds).ToString();
            return timeStamp;
        }
    }
}
