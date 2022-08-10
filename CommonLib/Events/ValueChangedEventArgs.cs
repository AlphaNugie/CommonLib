using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Events
{
    /// <summary>
    /// 值改变事件的事件参数
    /// </summary>
    /// <typeparam name="T">值的类型（约束为可比较类型）</typeparam>
    public class ValueChangedEventArgs<T> : EventArgs where T : IComparable
    {
        /// <summary>
        /// 改变之前的值
        /// </summary>
        public T PrevValue { get; set; }

        /// <summary>
        /// 改变之后的当前值
        /// </summary>
        public T CurrValue { get; set; }

        /// <summary>
        /// 用给定的改变之前、改变之后的值初始化
        /// </summary>
        /// <param name="prev">改变之前的值</param>
        /// <param name="curr">改变之后的当前值</param>
        public ValueChangedEventArgs(T prev, T curr)
        {
            PrevValue = prev;
            CurrValue = curr;
        }
    }
}
