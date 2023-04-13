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
        /// 时间戳计算初始时间节点
        /// </summary>
        public static DateTime TimeStampZero = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// 获取当前UTC时间的时间戳（精度毫秒）
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp() { return GetTimeStamp(DateTime.UtcNow); }

        /// <summary>
        /// 根据给定时间获取对应的时间戳（精度毫秒）
        /// </summary>
        /// <param name="dateTime">给定的时间</param>
        /// <returns></returns>
        public static string GetTimeStamp(DateTime dateTime)
        {
            TimeSpan tmp = dateTime - TimeStampZero;
            string timeStamp = Convert.ToInt64(tmp.TotalMilliseconds).ToString();
            return timeStamp;
        }

        /// <summary>
        /// 获取当前UTC时间的时间戳（精度秒）
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStampBySeconds() { return GetTimeStampBySeconds(DateTime.UtcNow); }

        /// <summary>
        /// 根据给定时间获取对应的时间戳（精度秒）
        /// </summary>
        /// <param name="dateTime">给定的时间</param>
        /// <returns></returns>
        public static string GetTimeStampBySeconds(DateTime dateTime)
        {
            TimeSpan tmp = dateTime - TimeStampZero;
            string timeStamp = Convert.ToUInt64(tmp.TotalSeconds).ToString();
            return timeStamp;
        }

        /// <summary>
        /// 根据时间戳（秒）获取当前时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetUtcTimeByTimeStampSec(string timeStamp)
        {
            ulong seconds;
            seconds = ulong.TryParse(timeStamp, out seconds) ? seconds : 0;
            return TimeStampZero.AddSeconds(seconds);
        }

        /// <summary>
        /// 根据时间戳（毫秒）获取当前时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetUtcTimeByTimeStampMillisec(string timeStamp)
        {
            ulong milli;
            milli = ulong.TryParse(timeStamp, out milli) ? milli : 0;
            return TimeStampZero.AddMilliseconds(milli);
        }
    }
}
