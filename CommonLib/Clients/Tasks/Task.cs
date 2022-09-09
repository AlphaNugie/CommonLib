using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using static CommonLib.Function.TimerEventRaiser;

namespace CommonLib.Clients.Tasks
{
    /// <summary>
    /// 基础任务类
    /// </summary>
    public abstract class Task
    {
        private readonly AutoResetEvent _auto = new AutoResetEvent(false);
        private bool _ended = false;
        private bool _paused = true;
        private List<string> _taskLogsBuffer = new List<string>(); //日志存放缓冲区，每次循环可以直接向里添加（Add）而不必清除（Clear）
        protected string _errorMessage = string.Empty;

        #region 事件
        /// <summary>
        /// 任务每次循环触发事件的委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void TaskContentLoopedEventHandler(object sender, TaskContentLoopedEventArgs e);

        /// <summary>
        /// 任务每经过一次循环就触发一次的事件
        /// </summary>
        public event TaskContentLoopedEventHandler ContentLooped;
        #endregion

        #region 属性
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
        /// 任务循环运行间隔，毫秒
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
        #endregion

        /// <summary>
        /// 初始化Task类，默认任务执行间隔1000毫秒，初始状态为暂停
        /// </summary>
        protected Task()
        {
            Interval = 1000;
            AllowPrintTaskLog = true;
            Pause();
            //Init();
            ThreadLoop = new Thread(new ThreadStart(Loop)) { IsBackground = true };
            ThreadLoop.Start();
        }

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
        }

        /// <summary>
        /// 任务循环线程
        /// </summary>
        public Thread ThreadLoop { get; private set; }

        /// <summary>
        /// 循环体内部内容
        /// </summary>
        public abstract void LoopContent();

        /// <summary>
        /// 循环体
        /// </summary>
        public void Loop()
        {
            //结束条件：结束标志为true，或任务只运行1次且已经运行1次
            while (!(_ended || (RunOnlyOnce && LoopCounter++ > 0)))
            {
                Thread.Sleep(Interval);
                if (_paused)
                    _auto.WaitOne();
                if (_taskLogsBuffer == null)
                    _taskLogsBuffer = new List<string>();
                else
                    _taskLogsBuffer.Clear();
                try { LoopContent(); }
                catch (Exception e) { _errorMessage = e.Message; }
                if (ContentLooped != null)
                    ContentLooped.BeginInvoke(this, new TaskContentLoopedEventArgs(LoopCounter, _errorMessage), null, null);
                //TaskLogs = _taskLogsBuffer.ToList();
                FlushLogs();
                PrintErrorMessage();
            }
        }

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
            Console.WriteLine(string.Format("{0}: {1}", GetType().Name, _errorMessage));
            _errorMessage = string.Empty;
        }
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
}
