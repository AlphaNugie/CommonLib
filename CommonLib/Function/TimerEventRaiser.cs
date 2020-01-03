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
        private uint interval = DEFAULT_INTERVAL, counter, raised_counter, raise_threshold = DEFAULT_RAISE_THRESHOLD, raise_interval = DEFAULT_RAISE_THRESHOLD;
        #endregion

        #region 属性
        /// <summary>
        /// 计时间隔，两次计时累加间的时间长度，默认1000毫秒
        /// </summary>
        public uint Interval
        {
            get { return this.interval; }
            set
            {
                this.interval = value > 0 ? value : DEFAULT_INTERVAL; //为0则赋值为默认值
                this.timer.Interval = this.interval;
            }
        }

        /// <summary>
        /// 触发间隔，两次触发事件间允许的最短时间长度，默认5000毫秒
        /// </summary>
        public uint RaiseInterval
        {
            //未曾触发事件时触发间隔以触发阈值为准
            get { return this.raised_counter == 0 ? this.raise_threshold : this.raise_interval; }
            set { this.raise_interval = value > 0 ? value : DEFAULT_RAISE_THRESHOLD; }
        }

        /// <summary>
        /// 计时器，计时间隔的累加，大于触发间隔后不再累加
        /// </summary>
        public uint Counter
        {
            get { return this.counter; }
            set
            {
                //计时长度大于触发间隔后不再累加
                if (value <= this.RaiseInterval)
                    this.counter = value;
            }
        }

        /// <summary>
        /// 计时达到阈值的次数
        /// </summary>
        public uint RaisedTimes
        {
            get { return this.raised_counter; }
            set { this.raised_counter = value; }
        }

        /// <summary>
        /// 计时阈值，计时达到此值触发事件，默认5000毫秒
        /// </summary>
        public uint RaiseThreshold
        {
            get { return this.raise_threshold; }
            set { this.raise_threshold = value > 0 ? value : DEFAULT_RAISE_THRESHOLD; }
        }
        #endregion

        #region 构造器
        /// <summary>
        /// 以指定的计时间隔初始化
        /// </summary>
        /// <param name="interval">计时间隔（毫秒）</param>
        public TimerEventRaiser(uint interval)
        {
            this.Interval = interval;
            this.timer.Elapsed += new ElapsedEventHandler(this.TimerElapsed);
        }

        /// <summary>
        /// 以默认的计时间隔初始化
        /// </summary>
        public TimerEventRaiser() : this(0) { }
        #endregion

        #region 方法
        /// <summary>
        /// 开始计时
        /// </summary>
        public void Run()
        {
            this.timer.Start();
        }

        /// <summary>
        /// 结束计时
        /// </summary>
        public void Stop()
        {
            this.timer.Stop();
            this.Reset();
        }

        /// <summary>
        /// 重置计时器以及达到计时的次数
        /// </summary>
        public void Reset()
        {
            this.counter = 0;
            this.RaisedTimes = 0;
        }

        /// <summary>
        /// 事件触发
        /// </summary>
        public void Raise()
        {
            if (this.ThresholdReached != null)
                this.ThresholdReached.BeginInvoke(this, new ThresholdReachedEventArgs(this.counter, ++this.RaisedTimes), null, null);
            this.counter = 0;
        }

        /// <summary>
        /// 手动点击，提供指定信息
        /// </summary>
        /// <param name="message">点击信息</param>
        public void Click(string message)
        {
            this.Reset();
            if (this.Clicked != null)
                this.Clicked.BeginInvoke(this, new ClickedEventArgs(message), null, null);
        }

        /// <summary>
        /// 手动点击
        /// </summary>
        public void Click()
        {
            this.Click(string.Empty);
        }
        #endregion

        /// <summary>
        /// 计时器触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.Counter += this.Interval;
            if (this.counter >= this.RaiseThreshold && this.counter >= this.RaiseInterval)
                this.Raise();
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
        public uint Counter { get; set; }

        /// <summary>
        /// 触发的次数
        /// </summary>
        public uint RaisedTimes { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="counter">触发时的计时器大小</param>
        /// <param name="raised_times">触发次数</param>
        public ThresholdReachedEventArgs(uint counter, uint raised_times)
        {
            this.Counter = counter;
            this.RaisedTimes = raised_times;
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
            this.ClickMessage = message;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public ClickedEventArgs() : this(string.Empty) { }
    }
}
