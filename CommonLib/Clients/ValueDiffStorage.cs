using CommonLib.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 数值存储器，存储当前值、上一个值以及差异值
    /// </summary>
    /// <typeparam name="T">数值的类型（只能是可比较类型）</typeparam>
    public class ValueDiffStorage<T> where T : IComparable
    {
        #region 事件
        /// <summary>
        /// 变化趋势改变事件
        /// </summary>
        public event ValueTrendTickerReachedEventHandler ValueTrendTickerReached;

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event ValueChangedEventHandler<T> ValueChangedEvent;
        #endregion

        private T _currValue; //当前值
        private int _currTrend; //当前变化趋势（-1,0,1）
        private uint _tickerThres = 5; //差异计数器临界值

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 差异值计数器，差异值符号改变时重置，否则每次数值改变都自增一次
        /// </summary>
        public uint DiffTicker { get; set; }

        /// <summary>
        /// 差异计数器阈值，当差异计数等于这个值时触发事件（大于0）
        /// </summary>
        public uint DiffTickerThreshold
        {
            get { return _tickerThres; }
            set { _tickerThres = value > 0 ? value : _tickerThres; }
        }

        /// <summary>
        /// 上一个不同的值
        /// </summary>
        public T PrevValue { get; set; }

        /// <summary>
        /// 当前值，改变后将历史值存入FormerValue字段
        /// </summary>
        public T CurrentValue
        {
            get { return _currValue; }
            set
            {
                ValueChanged = !value.Equals(_currValue);
                if (ValueChanged)
                {
                    PrevValue = _currValue;
                    _currValue = value;
                    if (ValueChangedEvent != null)
                        ValueChangedEvent.BeginInvoke(this, new ValueChangedEventArgs<T>(PrevValue, _currValue), null, null);
                    try { CurrentTrend = _currValue.CompareTo(PrevValue); }
                    catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// 当前值是否有改变
        /// </summary>
        public bool ValueChanged { get; set; }

        /// <summary>
        /// 上一个不同的变化趋势
        /// </summary>
        public int PrevTrend { get; set; }

        /// <summary>
        /// 当前变化趋势（-1,0,1）
        /// </summary>
        public int CurrentTrend
        {
            get { return _currTrend; }
            set
            {
                PrevTrend = _currTrend;
                _currTrend = value;
                int temp = PrevTrend * _currTrend;
                //假如之前变化趋势和当前变化趋势有一个是0，则不采取动作
                if (temp == 0)
                    return;
                //假如变化趋势相反则重置变化趋势计数，否则计数+1
                DiffTicker = temp < 0 ? 0 : DiffTicker + 1;
                //当变化趋势计数达到临界值时触发事件
                if (DiffTicker == DiffTickerThreshold)
                    ValueTrendTickerReached.BeginInvoke(this, new ValueTrendTickerReachedEventArgs(PrevTrend, _currTrend), null, null);
            }
        }

        /// <summary>
        /// 初始化而不指定代码
        /// </summary>
        public ValueDiffStorage() : this(string.Empty) { }

        /// <summary>
        /// 用指定代码初始化
        /// </summary>
        /// <param name="code">代码</param>
        public ValueDiffStorage(string code)
        {
            Code = code;
        }
    }
}
