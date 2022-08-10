using CommonLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Events
{
    /// <summary>
    /// 值变化趋势累积事件的事件参数
    /// </summary>
    public class ValueTrendTickerReachedEventArgs : EventArgs
    {
        /// <summary>
        /// 之前的比较
        /// </summary>
        public int PrevComparison { get; set; }

        /// <summary>
        /// 当前的比较
        /// </summary>
        public int CurrComparison { get; set; }

        /// <summary>
        /// 当前趋势
        /// </summary>
        public ValueTrend Trend { get; set; }

        /// <summary>
        /// 用给定比较值初始化
        /// </summary>
        /// <param name="prev">之前的比较值</param>
        /// <param name="curr">当前的的比较值</param>
        public ValueTrendTickerReachedEventArgs(int prev, int curr)
        {
            PrevComparison = prev;
            CurrComparison = curr;
            Trend = (ValueTrend)Math.Sign(curr);
        }
    }
}
