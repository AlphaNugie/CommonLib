using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.AppCrash
{
    /// <summary>
    /// 使用此类测试程序崩溃效果
    /// </summary>
    public class AppCrashTest
    {
        /// <summary>
        /// Tcp客户端连接线程
        /// </summary>
        private Thread TcpThread { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        public AppCrashTest() { }

        #region 方法
        /// <summary>
        /// 尝试异步运行以制造程序崩溃
        /// </summary>
        public void RunAsync()
        {
            new Thread(new ThreadStart(Run)).Start();
        }
 
        /// <summary>
        /// 尝试运行以制造程序崩溃
        /// </summary>
        public void Run()
        {
            TcpThread = new Thread(new ThreadStart(ThrowOutOfMemoryException)) { IsBackground = true };
            //IsOnlineCheckStart();
            try
            {
                //假如不具有以下状态，则启动线程
                if ((TcpThread.ThreadState & ThreadState.Aborted) == 0 && (TcpThread.ThreadState & ThreadState.AbortRequested) == 0)
                    TcpThread.Start();
            }
            catch (Exception) { }
        }

        ///// <summary>
        /////  线程接收Socket上传的数据
        ///// </summary>
        //private void StartTcpThread()
        //{
        //    ThrowOutOfMemoryException();
        //}

        /// <summary>
        /// 抛出OutOfMemory异常
        /// </summary>
        private void ThrowOutOfMemoryException()
        {
            throw new OutOfMemoryException();
        }
        #endregion
    }
}
