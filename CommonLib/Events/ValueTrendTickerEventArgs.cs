using CommonLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Events
{
    /// <summary>
    /// 值变化趋势累积然后改变的事件的参数
    /// </summary>
    public class ValueTrendChangedEventArgs<T> : EventArgs where T : IComparable
    {
        /// <summary>
        /// 上一个值
        /// </summary>
        public T PrevValue { get; set; }

        /// <summary>
        /// 当前值
        /// </summary>
        public T CurrentValue { get; set; }

        /// <summary>
        /// 当前速度值（值的变化速度，只有当泛型为数值类型或为可比较类型时有效，单位为xxx每秒）
        /// </summary>
        public double CurrentSpeed { get; set; }

        /// <summary>
        /// 当前趋势
        /// </summary>
        public ValueTrend CurrentTrend { get; set; }

        /// <summary>
        /// 之前的趋势
        /// </summary>
        public ValueTrend PrevTrend { get; set; }

        /// <summary>
        /// 用给定的值初始化
        /// </summary>
        /// <param name="prevValue">之前的值</param>
        /// <param name="currValue">当前值</param>
        /// <param name="currSpeed">值变化速度</param>
        /// <param name="currTrend">当前变化趋势</param>
        /// <param name="prevTrend">之前变化趋势</param>
        public ValueTrendChangedEventArgs(T prevValue, T currValue, double currSpeed, ValueTrend currTrend, ValueTrend prevTrend)
        {
            PrevValue = prevValue;
            CurrentValue = currValue;
            CurrentSpeed = currSpeed;
            CurrentTrend = currTrend;
            PrevTrend = prevTrend;
        }
    }
}
