using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CommonLib.Function
{
    /// <summary>
    /// 计时事件触发器
    /// </summary>
    public class TimerEventRaiser
    {
        #region 事件
        /// <summary>
        /// 计时器达到计时阈值后触发事件的委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ThresholdReachedEventHandler(object sender, ThresholdReachedEventArgs e);

        /// <summary>
        /// 点击事件的委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ClickedEventHandler(object sender, ClickedEventArgs e);

        /// <summary>
        /// 计时器达到计时阈值
        /// </summary>
        public event ThresholdReachedEventHandler ThresholdReached;

        /// <summary>
        /// 点击
        /// </summary>
        public event ClickedEventHandler Clicked;
        #endregion

        #region 私有成员
        private const uint DEFAULT_INTERVAL = 1000, DEFAULT_RAISE_THRESHOLD = 5000;
        private readonly Timer timer = new Timer();
        private uint interval = DEFAULT_INTERVAL, /*counter, raise_threshold = DEFAULT_RAISE_THRESHOLD, */raised_counter, raise_interval = DEFAULT_RAISE_THRESHOLD;
        private ulong counter, raise_threshold = DEFAULT_RAISE_THRESHOLD;
        #endregion

        #region 属性
        /// <summary>
        /// 计时间隔，两次计时累加间的时间长度，单位毫秒，默认1000
        /// </summary>
        public uint Interval
        {
            get { return interval; }
            set
            {
                interval = value > 0 ? value : DEFAULT_INTERVAL; //为0则赋值为默认值
                timer.Interval = interval;
            }
        }

        /// <summary>
        /// 触发间隔，两次触发事件间允许的最短时间长度，单位毫秒，默认5000
        /// </summary>
        public uint RaiseInterval
        {
            //未曾触发事件时触发间隔以触发阈值为准
            //get { return raised_counter == 0 ? raise_threshold : raise_interval; }
            get { return raise_interval; }
            set { raise_interval = value > 0 ? value : DEFAULT_RAISE_THRESHOLD; }
        }

        /// <summary>
        /// 计时器，计时间隔的累加，大于触发间隔后不再累加
        /// </summary>
        public ulong Counter
        {
            get { return counter; }
            private set
            {
                ////计时长度大于触发间隔后不再累加
                //if (value <= RaiseInterval)
                    counter = value;
            }
        }

        /// <summary>
        /// 计时达到阈值的次数
        /// </summary>
        public uint RaisedTimes
        {
            get { return raised_counter; }
            private set { raised_counter = value; }
        }

        /// <summary>
        /// 计时阈值，计时达到此值触发事件，单位毫秒，默认5000
        /// </summary>
        public ulong RaiseThreshold
        {
            //get { return raise_threshold; }
            get { return raise_threshold + raised_counter * raise_interval; }
            set { raise_threshold = value > 0 ? value : DEFAULT_RAISE_THRESHOLD; }
        }
        #endregion

        #region 构造器
        /// <summary>
        /// 以指定的计时间隔初始化
        /// </summary>
        /// <param name="interval">计时间隔（毫秒）</param>
        public TimerEventRaiser(uint interval)
        {
            Interval = interval;
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
        }

        /// <summary>
        /// 以默认的计时间隔初始化
        /// </summary>
        public TimerEventRaiser() : this(DEFAULT_INTERVAL) { }
        #endregion

        #region 方法
        /// <summary>
        /// 开始计时
        /// </summary>
        public void Run()
        {
            timer.Start();
        }

        /// <summary>
        /// 结束计时
        /// </summary>
        public void Stop()
        {
            timer.Stop();
            Reset();
        }

        /// <summary>
        /// 重置计时器以及达到计时的次数
        /// </summary>
        public void Reset()
        {
            counter = 0;
            RaisedTimes = 0;
        }

        /// <summary>
        /// 事件触发
        /// </summary>
        public void Raise()
        {
            if (ThresholdReached != null)
                ThresholdReached.BeginInvoke(this, new ThresholdReachedEventArgs(counter, ++RaisedTimes), null, null);
            //counter = 0;
        }

        /// <summary>
        /// 手动点击，提供指定信息
        /// </summary>
        /// <param name="message">点击信息</param>
        public void Click(string message)
        {
            Reset();
            if (Clicked != null)
                Clicked.BeginInvoke(this, new ClickedEventArgs(message), null, null);
        }

        /// <summary>
        /// 手动点击
        /// </summary>
        public void Click()
        {
            Click(string.Empty);
        }
        #endregion

        /// <summary>
        /// 计时器触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Counter += Interval;
            //if (counter >= RaiseThreshold && counter >= RaiseInterval)
            if (counter >= RaiseThreshold)
                Raise();
        }
    }

    /// <summary>
    /// 计时器达到阈值后触发事件的事件参数类
    /// </summary>
    public class ThresholdReachedEventArgs : EventArgs
    {
        /// <summary>
        /// 触发时的计时器大小
        /// </summary>
        public ulong Counter { get; set; }

        /// <summary>
        /// 触发的次数
        /// </summary>
        public uint RaisedTimes { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="counter">触发时的计时器大小</param>
        /// <param name="raised_times">触发次数</param>
        public ThresholdReachedEventArgs(ulong counter, uint raised_times)
        {
            Counter = counter;
            RaisedTimes = raised_times;
        }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public ThresholdReachedEventArgs() : this(0, 0) { }
    }

    /// <summary>
    /// 点击事件参数类
    /// </summary>
    public class ClickedEventArgs
    {
        /// <summary>
        /// 点击信息
        /// </summary>
        public string ClickMessage { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="message">点击信息</param>
        public ClickedEventArgs(string message)
        {
            ClickMessage = message;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public ClickedEventArgs() : this(string.Empty) { }
    }
}
