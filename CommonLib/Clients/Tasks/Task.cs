using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
        protected List<string> _taskLogsBuffer = new List<string>(); //日志存放缓冲区，每次循环可以直接向里添加（Add）而不必清除（Clear）
        protected string _errorMessage = string.Empty;

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
        /// 任务初始化
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 任务运行
        /// </summary>
        public void Run()
        {
            ////假如未初始化，先初始化
            //if (!Initialized)
            //{
            //    Init();
            //    Initialized = true;
            //}
            _paused = false;
            _auto.Set();
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
                TaskLogs = _taskLogsBuffer.ToList();
                PrintErrorMessage();
            }
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
            Console.WriteLine(_errorMessage);
            _errorMessage = string.Empty;
        }
    }
}
