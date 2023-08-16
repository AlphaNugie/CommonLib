using CommonLib.Enums;
using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace CommonLib.Clients.Tasks
{
    /// <summary>
    /// 基础任务类
    /// </summary>
    public abstract class Task : IDisposable
    {
        private readonly AutoResetEvent _auto = new AutoResetEvent(false);
        private bool _ended = false;
        private bool _paused = true;
        private List<string> _taskLogsBuffer = new List<string>(); //日志存放缓冲区，每次循环可以直接向里添加（Add）而不必清除（Clear）
        protected string _errorMessage = string.Empty;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        #region 事件
        /// <summary>
        /// 任务每次循环触发事件的委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void TaskContentLoopedEventHandler(object sender, TaskContentLoopedEventArgs e);

        /// <summary>
        /// 任务状态改变事件的委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void TaskStateChangedEventHandler(object sender, TaskStateChangedEventArgs e);

        /// <summary>
        /// 任务每经过一次循环就触发一次的事件
        /// </summary>
        public event TaskContentLoopedEventHandler ContentLooped;

        /// <summary>
        /// 任务状态改变后触发的事件（例如已启动或已停止）
        /// </summary>
        public event TaskStateChangedEventHandler StateChanged;

        /// <summary>
        /// 任务状态改变后触发的事件（例如已启动或已停止）（仅用于重启任务）
        /// </summary>
        private event TaskStateChangedEventHandler StateChangedAfterRestart;
        #endregion

        #region 属性
        /// <summary>
        /// 任务循环线程
        /// </summary>
        public Thread ThreadLoop { get; private set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set { _errorMessage = value; }
        }

        /// <summary>
        /// 任务日志
        /// </summary>
        public List<string> TaskLogs { get; private set; }

        /// <summary>
        /// 是否打印任务日志
        /// </summary>
        public bool AllowPrintTaskLog { get; set; }

        /// <summary>
        /// 任务循环运行间隔（毫秒）
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool Initialized { get; set; }

        /// <summary>
        /// 任务循环次数
        /// </summary>
        public ulong LoopCounter { get; set; }

        /// <summary>
        /// 是否为一次性任务
        /// </summary>
        public bool RunOnlyOnce { get; set; }

        /// <summary>
        /// 是否在循环的过程中每隔一段时间自动重新启动
        /// </summary>
        public bool AutoRestart { get; set; }

        /// <summary>
        /// 任务重新启动的时间间隔（毫秒），仅在任务开始运行后计量，假如不大于任务循环执行的时间间隔则不启用自动重启
        /// </summary>
        public long RestartInterval { get; set; }

        /// <summary>
        /// 任务重启次数
        /// </summary>
        public ulong RestartCounter { get; private set; }
        #endregion

        ///// <summary>
        ///// 初始化Task类，默认任务执行间隔1000毫秒，初始状态为暂停
        ///// </summary>
        //protected Task()
        //{
        //    Interval = 1000;
        //    AllowPrintTaskLog = true;
        //    Pause();
        //    //Init();
        //    ThreadLoop = new Thread(new ThreadStart(Loop)) { IsBackground = true };
        //    ThreadLoop.Start();
        //}

        /// <summary>
        /// 初始化Task类，默认任务执行间隔1000毫秒，不自动重启，初始状态为暂停
        /// </summary>
        protected Task() : this(1000, false, 0) { }

        /// <summary>
        /// 初始化Task类，使用给定的任务执行间隔、是否自动重启、重启时间间隔的设置，初始状态为暂停
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="autoRestart"></param>
        /// <param name="restartInterval"></param>
        protected Task(int interval = 1000, bool autoRestart = false, long restartInterval = 0)
        {
            Interval = interval;
            AutoRestart = autoRestart;
            RestartInterval = restartInterval;
            AllowPrintTaskLog = true;
            Pause();
            //Init();
            ThreadLoop = new Thread(new ThreadStart(Loop)) { IsBackground = true };
            ThreadLoop.Start();
        }

        #region 资源释放
        /// <summary>
        /// 释放当前实例所使用的所有资源
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            //此时循环应当是结束了，以防万一终止一下线程
            if (ThreadLoop != null)
            {
                ThreadLoop.Abort();
                ThreadLoop = null;
            }
            //释放WaitHandle的资源
            _auto.Close();
            //_auto.Dispose(); //与Close方法效果相同
            _stopwatch.Stop();

            //List为引用类型，由垃圾回收器自动管理内存，不需要手动释放，只需要在存储大量数据时手动将其置为null，以便更快地释放内存
            //_taskLogsBuffer.Clear();
            //_taskLogsBuffer = null;
            //if (TaskLogs != null)
            //{
            //    TaskLogs.Clear();
            //    TaskLogs = null;
            //}
        }
        #endregion

        #region 流程控制功能（启动、暂停、停止、重启等）
        /// <summary>
        /// 初始化任务，同时将添加的日志从缓冲区保存到正式列表中，也可进行一些其它内置的操作
        /// </summary>
        public void Initialize()
        {
            Init();
            FlushLogs();
            Initialized = true;
        }

        /// <summary>
        /// 任务初始化（不会保留添加的任务日志或进行其它任何其它操作）
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 任务运行
        /// </summary>
        public void Run()
        {
            _paused = false;
            _stopwatch.Start();
            _auto.Set();
        }

        public void RunOnceNStop()
        {
            RunOnlyOnce = true;
            Run();
        }

        /// <summary>
        /// 任务暂停
        /// </summary>
        public void Pause()
        {
            _paused = true;
        }

        /// <summary>
        /// 任务停止
        /// </summary>
        public void Stop()
        {
            Run();
            _ended = true;
            _stopwatch.Reset();
        }

        /////
        ///// 为重启任务所准备的方法，在这个方法中重新声明并用传入的参数初始化一个当前任务类型的实体，然后执行任务的初始化（Initialize）、运行（Run）方法
        /////
        //protected abstract void RestartUrself();

        /// <summary>
        /// 获取新实体的方法（目前仅为任务重启使用），在这个方法中重新声明、初始化一个当前任务类型的实体并返回（假如返回实体为空将不执行重启）
        /// </summary>
        ///// <param name="interval">任务循环运行的时间间隔（毫秒）</param>
        ///// <param name="autoRestart">是否自动重启</param>
        ///// <param name="restartInterval">自动重启的时间间隔（毫秒）</param>
        protected abstract Task GetNewInstance(/*int interval, bool autoRestart, long restartInterval*/);

        /// <summary>
        /// 任务重新启动
        /// </summary>
        public void Restart()
        {
            //用委托添加一个状态改变事件，然后再终止任务，这个事件会等到整个循环体完全结束之后再执行
            //StateChanged += new TaskStateChangedEventHandler(delegate (object obj, TaskStateChangedEventArgs args)
            StateChangedAfterRestart += new TaskStateChangedEventHandler(delegate (object obj, TaskStateChangedEventArgs args)
            {
                if (args.State == ServiceState.Started)
                    return;
                var task = GetNewInstance(/*Interval, AutoRestart, RestartInterval*/);
                if (task == null)
                    return;
                task.Interval = Interval;
                task.AutoRestart = AutoRestart;
                task.RestartInterval = RestartInterval;
                task.RestartCounter = RestartCounter + 1; //重启计数+1
                Dispose();
                //RestartUrself();
                task.Initialize();
                task.Run();
            });
            Stop();
        }
        #endregion

        #region 循环体
        /// <summary>
        /// 循环体内部内容
        /// </summary>
        public abstract void LoopContent();

        /// <summary>
        /// 循环体
        /// </summary>
        public void Loop()
        {
            ////结束条件：结束标志为true，或任务只运行1次且已经运行1次
            //while (!(_ended || (RunOnlyOnce && LoopCounter++ > 0)))
            //结束条件：结束标志为true，或任务已经运行1次且只需运行1次（LoopCounter自增在前，防止表达式断路后计数器不自增）
            while (!(_ended || (LoopCounter++ > 0 && RunOnlyOnce)))
            {
                Thread.Sleep(Interval);
                if (_paused)
                    _auto.WaitOne();
                //任务启动事件
                if (StateChanged != null && LoopCounter == 1)
                    StateChanged.BeginInvoke(this, new TaskStateChangedEventArgs(LoopCounter, ServiceState.Started, RestartCounter), null, null);
                //清空日志缓冲区
                if (_taskLogsBuffer == null)
                    _taskLogsBuffer = new List<string>();
                else
                    _taskLogsBuffer.Clear();
                //执行循环并捕捉异常
                try
                {
                    LoopContent();
                    _errorMessage = string.Empty;
                }
                catch (Exception e) { _errorMessage = string.Format("{0}: {1}", GetType().Name, e.Message); }
                //任务每循环一次完毕事件
                ContentLooped?.BeginInvoke(this, new TaskContentLoopedEventArgs(LoopCounter, _errorMessage), null, null);
                FlushLogs();
                PrintErrorMessage();
                if (AutoRestart && RestartInterval > Interval && _stopwatch.ElapsedMilliseconds >= RestartInterval)
                {
                    _stopwatch.Restart();
                    Restart();
                }
            }
            //任务停止事件
            StateChanged?.BeginInvoke(this, new TaskStateChangedEventArgs(LoopCounter, ServiceState.Stopped, RestartCounter), null, null);
            //任务停止事件（重启用）
            StateChangedAfterRestart?.BeginInvoke(this, new TaskStateChangedEventArgs(LoopCounter, ServiceState.Stopped, RestartCounter), null, null);
        }
        #endregion

        #region 日志控制与打印
        /// <summary>
        /// 添加单个日志
        /// </summary>
        /// <param name="log"></param>
        public void AddLog(string log)
        {
            AddLogs(log);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logs"></param>
        public void AddLogs(params string[] logs)
        {
            AddLogs(logs, true);
        }

        /// <summary>
        /// 批量添加日志
        /// </summary>
        /// <param name="logs"></param>
        /// <param name="trim">是否过滤空字符串与前后空格</param>
        public void AddLogs(IEnumerable<string> logs, bool trim)
        {
            if (logs == null || logs.Count() == 0)
                return;

            _taskLogsBuffer.AddRange(!trim ? logs : logs.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim())); //去除空字符串、前后空格
        }

        /// <summary>
        /// 将任务日志从缓冲区发送到正式列表中
        /// </summary>
        public void FlushLogs()
        {
            TaskLogs = _taskLogsBuffer.ToList();
        }

        /// <summary>
        /// 打印任务日志
        /// </summary>
        public void PrintTaskLogs()
        {
            if (!AllowPrintTaskLog || TaskLogs == null || TaskLogs.Count == 0)
                return;
            TaskLogs.ForEach(log =>
            {
                if (!string.IsNullOrWhiteSpace(log))
                    Console.WriteLine(log);
            });
        }

        /// <summary>
        /// 打印错误信息
        /// </summary>
        public void PrintErrorMessage()
        {
            if (string.IsNullOrWhiteSpace(_errorMessage))
                return;
            //Console.WriteLine(string.Format("{0}: {1}", GetType().Name, _errorMessage));
            Console.WriteLine(_errorMessage);
            //_errorMessage = string.Empty;
        }
        #endregion
    }

    /// <summary>
    /// 任务每次循环触发事件的事件参数类
    /// </summary>
    public class TaskContentLoopedEventArgs : EventArgs
    {
        /// <summary>
        /// 任务循环次数
        /// </summary>
        public ulong LoopCounter { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="counter">任务循环次数</param>
        /// <param name="errorMessage">错误信息</param>
        public TaskContentLoopedEventArgs(ulong counter, string errorMessage)
        {
            LoopCounter = counter;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public TaskContentLoopedEventArgs() : this(0, string.Empty) { }
    }

    /// <summary>
    /// 任务状态改变事件（已启动或已停止）的事件参数类
    /// </summary>
    public class TaskStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 任务循环次数
        /// </summary>
        public ulong LoopCounter { get; set; }

        /// <summary>
        /// 任务状态（已启动或已停止）
        /// </summary>
        public ServiceState State { get; set; }

        /// <summary>
        /// 任务重启次数
        /// </summary>
        public ulong RestartCounter { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="counter">任务循环次数</param>
        /// <param name="state">任务状态（已启动或已停止）</param>
        /// <param name="reCounter">任务重启次数</param>
        public TaskStateChangedEventArgs(ulong counter, ServiceState state, ulong reCounter)
        {
            LoopCounter = counter;
            State = state;
            RestartCounter = reCounter;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="state">任务状态（已启动或已停止）</param>
        public TaskStateChangedEventArgs(ServiceState state) : this(0, state, 0) { }
    }
}
