using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

#if NET45_OR_GREATER
namespace CommonLib.Function
#elif NET9_0_OR_GREATER
namespace CommonLib.Clients
#endif
{
    /// <summary>
    /// 计时事件触发器
    /// </summary>
#if NET45_OR_GREATER
    public class TimerEventRaiser
#elif NET9_0_OR_GREATER
    public class TimerEventRaiser : IDisposable
#endif
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
#if NET45_OR_GREATER
        public event ThresholdReachedEventHandler ThresholdReached;
#elif NET9_0_OR_GREATER
        public event ThresholdReachedEventHandler? ThresholdReached;
#endif

        /// <summary>
        /// 点击
        /// </summary>
#if NET45_OR_GREATER
        public event ClickedEventHandler Clicked;
#elif NET9_0_OR_GREATER
        public event ClickedEventHandler? Clicked;
#endif
        #endregion

        #region 私有成员
        private const uint DEFAULT_INTERVAL = 1000, DEFAULT_RAISE_THRESHOLD = 5000;
#if NET45_OR_GREATER
        private readonly Timer _timer = new Timer();
#elif NET9_0_OR_GREATER
        private readonly System.Timers.Timer _timer = new();
#endif
        private uint _interval = DEFAULT_INTERVAL, /*_raisedTimes, */_raiseInterval = DEFAULT_RAISE_THRESHOLD;
        private ulong /*_counter, */_raiseThreshold = DEFAULT_RAISE_THRESHOLD;
        #endregion

        #region 属性
        /// <summary>
        /// 计时间隔，两次计时累加间的时间长度，单位毫秒，默认1000
        /// </summary>
        public uint Interval
        {
            get { return _interval; }
            set
            {
                _interval = value > 0 ? value : DEFAULT_INTERVAL; //为0则赋值为默认值
                _timer.Interval = _interval;
            }
        }

        /// <summary>
        /// 触发间隔，两次触发事件间允许的最短时间长度，单位毫秒，默认5000
        /// </summary>
        public uint RaiseInterval
        {
            get { return _raiseInterval; }
            set { _raiseInterval = value > 0 ? value : DEFAULT_RAISE_THRESHOLD; }
        }

        /// <summary>
        /// 计时器，计时间隔的累加，大于触发间隔后不再累加
        /// </summary>
        public ulong Counter { get;set; }
        //{
        //    get { return _counter; }
        //    private set
        //    {
        //        ////计时长度大于触发间隔后不再累加
        //        //if (value <= RaiseInterval)
        //            _counter = value;
        //    }
        //}

        /// <summary>
        /// 计时达到阈值的次数
        /// </summary>
        public uint RaisedTimes { get; private set; }
        //{
        //    get { return _raisedTimes; }
        //    private set { _raisedTimes = value; }
        //}

        /// <summary>
        /// 计时阈值，计时达到此值触发事件，单位毫秒，默认5000
        /// </summary>
        public ulong RaiseThreshold
        {
            //get { return _raiseThreshold; }
            //假如不重置，计时器将一直累加，每次判断的阈值都加上触发的次数x触发间隔，以达到每两次触发之间至少有一个触发间隔的时间长度的效果
            //get { return _raiseThreshold + _raisedTimes * _raiseInterval; }
            get { return _raiseThreshold + RaisedTimes * _raiseInterval; }
            set { _raiseThreshold = value > 0 ? value : DEFAULT_RAISE_THRESHOLD; }
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
            _timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
        }

        /// <summary>
        /// 以默认的计时间隔初始化
        /// </summary>
        public TimerEventRaiser() : this(DEFAULT_INTERVAL) { }
        #endregion

        #region 资源释放
        /// <summary>
        /// 释放当前实例所使用的所有资源
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放当前实例所使用的所有资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            _timer.Stop();
            _timer.Dispose();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 开始计时
        /// </summary>
        public void Run()
        {
            _timer.Start();
        }

        /// <summary>
        /// 结束计时
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
            Reset();
        }

        /// <summary>
        /// 重置计时器以及达到计时的次数
        /// </summary>
        public void Reset()
        {
            //_counter = 0;
            //_raisedTimes = 0;
            Counter = 0;
            RaisedTimes = 0;
        }

        /// <summary>
        /// 事件触发
        /// </summary>
        public void Raise()
        {
            //ThresholdReached?.BeginInvoke(this, new ThresholdReachedEventArgs(_counter, ++_raisedTimes), null, null);
            ThresholdReached?.BeginInvoke(this, new ThresholdReachedEventArgs(Counter, ++RaisedTimes), null, null);
            //counter = 0;
        }

        /// <summary>
        /// 手动点击，提供指定信息
        /// </summary>
        /// <param name="message">点击信息</param>
        public void Click(string message)
        {
            Reset();
            Clicked?.BeginInvoke(this, new ClickedEventArgs(message), null, null);
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
#if NET45_OR_GREATER
        public void TimerElapsed(object sender, ElapsedEventArgs e)
#elif NET9_0_OR_GREATER
        public void TimerElapsed(object? sender, ElapsedEventArgs e)
#endif
        {
            Counter += _interval;
            if (Counter >= RaiseThreshold)
                Raise();
        }
    }

    #region .net framework 4.5 版本
#if NET45_OR_GREATER
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
#endif
    #endregion

    #region .net 9 版本
#if NET9_0_OR_GREATER
    /// <summary>
    /// 计时器达到阈值后触发事件的事件参数类
    /// </summary>
    /// <remarks>
    /// 构造器
    /// </remarks>
    /// <param name="counter">触发时的计时器大小</param>
    /// <param name="raised_times">触发次数</param>
    public class ThresholdReachedEventArgs(ulong counter, uint raised_times) : EventArgs
    {
        /// <summary>
        /// 触发时的计时器大小
        /// </summary>
        public ulong Counter { get; set; } = counter;

        /// <summary>
        /// 触发的次数
        /// </summary>
        public uint RaisedTimes { get; set; } = raised_times;

        /// <summary>
        /// 默认构造器
        /// </summary>
        public ThresholdReachedEventArgs() : this(0, 0) { }
    }

    /// <summary>
    /// 点击事件参数类
    /// </summary>
    /// <remarks>
    /// 构造器
    /// </remarks>
    /// <param name="message">点击信息</param>
    public class ClickedEventArgs(string message)
    {
        /// <summary>
        /// 点击信息
        /// </summary>
        public string ClickMessage { get; set; } = message;

        /// <summary>
        /// 构造器
        /// </summary>
        public ClickedEventArgs() : this(string.Empty) { }
    }
#endif
    #endregion
}
