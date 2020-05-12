using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLib.Events;
using CommonLib.Function;
using static CommonLib.Function.TimerEventRaiser;

namespace CommonLib.Clients
{
    /// <summary>
    /// TCP连接客户端
    /// </summary>
    public class DerivedTcpClient
    {
        #region 事件
        /// <summary>
        /// Tcp连接事件
        /// </summary>
        public event ConnectedEventHandler Connected;

        /// <summary>
        /// Tcp断开事件
        /// </summary>
        public event DisconnectedEventHandler Disconnected;

        /// <summary>
        /// TcpClient重连成功次数改变事件
        /// </summary>
        public event ReconnTimerChangedEventHandler ReconnTimerChanged;

        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event Events.DataReceivedEventHandler DataReceived;

        ///// <summary>
        ///// 数据接收事件事件的事件数据类
        ///// </summary>
        //private CommonLib.Events.DataReceivedEventArgs receivedEventArgs = new CommonLib.Events.DataReceivedEventArgs();
        #endregion
        #region 私有成员变量
        /// <summary>
        /// TcpClient对象
        /// </summary>
        private TcpClient baseClient = null;
        private readonly bool logging = false;
        private int receiveBufferSize = 32768;
        private bool autoReceive = true;
        private IPEndPoint remote_endpoint, local_endpoint;
        private readonly TimerEventRaiser raiser = new TimerEventRaiser(1000);
        #endregion
        #region 成员属性
        /// <summary>
        /// TcpClient对象
        /// </summary>
        public TcpClient BaseClient
        {
            get { return this.baseClient; }
            private set { this.baseClient = value; }
        }

        /// <summary>
        /// TcpClient用于发送与接收数据的数据流对象
        /// </summary>
        public NetworkStream NetStream { get; private set; }

        /// <summary>
        /// 与之建立TCP连接的主机IP地址
        /// </summary>
        public string ServerIp { get; set; }

        /// <summary>
        /// 建立TCP连接的端口
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// 本地IP终结点，未初始化则为空
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get { return this.local_endpoint; }
            set { this.local_endpoint = value; }
        }

        /// <summary>
        /// 远程IP终结点，未连接则为空
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return this.remote_endpoint; }
            set { this.remote_endpoint = value; }
        }

        /// <summary>
        /// 重新连接时是否保持同一个端口
        /// </summary>
        public bool HoldLocalPort { get; set; }

        /// <summary>
        /// 最新的错误信息
        /// </summary>
        public string LastErrorMessage { get; private set; }

        /// <summary>
        /// 接收缓冲区大小（字节数）
        /// </summary>
        public int ReceiveBufferSize
        {
            get { return this.receiveBufferSize; }
            set
            {
                this.receiveBufferSize = value;
                this.Buffer = new byte[this.receiveBufferSize];
                if (this.BaseClient != null)
                    this.BaseClient.ReceiveBufferSize = this.receiveBufferSize;
            }
        }

        /// <summary>
        /// 接收缓冲区
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// 是否自动接收（触发接收事件）
        /// </summary>
        public bool AutoReceive { get { return this.autoReceive; }
            set
            {
                bool prev = this.autoReceive;
                this.autoReceive = value;
                if (this.autoReceive == prev)
                    return;
                if (this.autoReceive)
                    this.NetStream.BeginRead(this.Buffer, 0, this.Buffer.Length, new AsyncCallback(TcpCallBack), this);
            }
        }

        /// <summary>
        /// 数据接收停顿时间（单位：毫秒，自上次接收后等待这些时间再接收）
        /// </summary>
        public int ReceiveRestTime { get; set; }

        /// <summary>
        /// Tcp连接名称，格式：本地端口->服务端IP:端口
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Tcp Socket的真实连接状态
        /// </summary>
        public bool IsConnected_Socket { get; private set; }

        /// <summary>
        /// 在无返回数据时是否重新连接
        /// </summary>
        public bool ReconnectWhenReceiveNone { get; set; }

        /// <summary>
        /// 重新连接成功的次数
        /// </summary>
        public int ReconnTimer { get; private set; }

        /// <summary>
        /// 监听TCP服务端，并在连接断开后试着重新连接
        /// </summary>
        private Thread Thread_TcpReconnect { get; set; }

        /// <summary>
        /// 控制TCP重连线程的AutoResetEvent，初始状态为非终止
        /// </summary>
        private AutoResetEvent Auto_TcpReconnect { get; set; }
        #endregion

        /// <summary>
        /// 设置Tcp连接名称，格式：本地端口->服务端IP:端口
        /// </summary>
        public void SetName()
        {
            try { this.Name = this.BaseClient.Client.GetName(out this.remote_endpoint, out this.local_endpoint); }
            catch (Exception) { this.Name = string.Empty; }
        }

        /// <summary>
        /// 构造器，初始化一个未连接DerivedTcpClient对象，决定是否自动接收数据、无返回数据时是否重新连接以及是否维持相同本地端口
        /// </summary>
        /// <param name="autoReceive">是否自动接收</param>
        /// <param name="reconn_noreceive">无返回数据时是否重新连接</param>
        /// <param name="holdPort">重新连接时是否维持相同本地端口</param>
        public DerivedTcpClient(bool autoReceive, bool reconn_noreceive, bool holdPort)
        {
            this.AutoReceive = autoReceive;
            this.ReconnectWhenReceiveNone = reconn_noreceive;
            this.HoldLocalPort = holdPort;
            this.ReceiveBufferSize = 32768;
            this.Buffer = new byte[this.ReceiveBufferSize];
            this.LastErrorMessage = string.Empty;
            this.Name = string.Empty;
            this.ReceiveRestTime = 0;

            this.raiser.RaiseThreshold = 10000;
            this.raiser.RaiseInterval = 5000;
            this.raiser.ThresholdReached += new ThresholdReachedEventHandler(this.Raiser_ThresholdReached);
        }

        private void Raiser_ThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            if (!this.ReconnectWhenReceiveNone)
                return;

            this.Reconnect();
        }

        /// <summary>
        /// 构造器，初始化一个未连接DerivedTcpClient对象，决定是否自动接收数据以及是否维持相同本地端口
        /// </summary>
        /// <param name="autoReceive">是否自动接收</param>
        /// <param name="holdPort">重新连接时是否维持相同本地端口</param>
        public DerivedTcpClient(bool autoReceive, bool holdPort) : this(autoReceive, false, holdPort)
        {
            //this.AutoReceive = autoReceive;
            //this.HoldLocalPort = holdPort;
            //this.ReceiveBufferSize = 2048;
            //this.Buffer = new byte[this.ReceiveBufferSize];
            //this.LastErrorMessage = string.Empty;
            //this.Name = string.Empty;
            //this.ReceiveRestTime = 0;
        }

        /// <summary>
        /// 构造器，初始化一个未连接DerivedTcpClient对象，不维持本地端口
        /// </summary>
        /// <param name="autoReceive">是否自动接收</param>
        public DerivedTcpClient(bool autoReceive) : this(autoReceive, false, false) { }

        /// <summary>
        /// 默认构造器，初始化一个未连接DerivedTcpClient对象，不维持本地端口
        /// </summary>
        public DerivedTcpClient() : this(true, false, false) { }

        /// <summary>
        /// 构造器，初始化一个DerivedTcpClient对象并连接，不维持本地端口
        /// </summary>
        /// <param name="server">Tcp服务端地址</param>
        /// <param name="port">Tcp服务端端口号</param>
        public DerivedTcpClient(string server, int port) : this()
        {
            this.Connect(server, port);
        }

        /// <summary>
        /// 与TCP服务端连接
        /// </summary>
        /// <returns>假如建立连接成功，返回1，否则返回0</returns>
        public int Connect()
        {
            return this.Connect(this.ServerIp, this.ServerPort);
        }

        /// <summary>
        /// 与TCP服务端与端口连接
        /// </summary>
        /// <param name="server">TCP服务端IP</param>
        /// <param name="port">端口号</param>
        /// <returns>假如建立连接成功，返回1，否则返回0</returns>
        public int Connect(string server, int port)
        {
            return this.Connect(server, port, null, 0);
        }

        /// <summary>
        /// 利用特定的本地端口与TCP服务端连接
        /// </summary>
        /// <param name="server">TCP服务端IP</param>
        /// <param name="port">端口号</param>
        /// <param name="localIp">本地IP</param>
        /// <param name="localPort">指定的本地端口（假如小于等于0则随机）</param>
        /// <returns>假如建立连接成功，返回1，否则返回0</returns>
        public int Connect(string server, int port, string localIp, int localPort)
        {
            //尝试建立连接
            int result = 1;
            try
            {
                this.ServerIp = server;
                this.ServerPort = port;
                //this.BaseClient = new TcpClient(this.ServerIp, this.ServerPort);
                this.BaseClient = !string.IsNullOrWhiteSpace(localIp) && localPort > 0 ? new TcpClient(new IPEndPoint(IPAddress.Parse(localIp), localPort)) : new TcpClient();
                this.BaseClient.Connect(this.ServerIp, this.ServerPort);
                this.SetName(); //修改连接名称

                //重置重连次数，同时调用事件委托
                this.ReconnTimer = 0;
                if (this.ReconnTimerChanged != null)
                    this.ReconnTimerChanged.BeginInvoke(this.Name, this.ReconnTimer, null, null);

                this.BaseClient.ReceiveBufferSize = this.ReceiveBufferSize; //接收缓冲区的大小
                this.NetStream = this.BaseClient.GetStream(); //发送与接收数据的数据流对象
                this.raiser.Run();
            }
            catch (Exception e)
            {
                this.LastErrorMessage = string.Format("无法建立TCP连接，IP{0}，端口号{1}：{2}", this.ServerIp, this.ServerPort, e.Message);
                FileClient.WriteExceptionInfo(e, this.LastErrorMessage, false);
                this.Close();
                result = 0;
                throw; //假如不需要抛出异常，注释此行
            }

            this.IsConnected = this.BaseClient.Connected;
            this.IsSocketConnected();
            if (this.IsConnected)
            {
                //调用Tcp连接事件委托
                if (this.Connected != null)
                    this.Connected.BeginInvoke(this.Name, (new EventArgs()), null, null);
                if (this.Thread_TcpReconnect == null)
                {
                    this.Auto_TcpReconnect = new AutoResetEvent(true);
                    this.Thread_TcpReconnect = new Thread(new ThreadStart(this.TcpAutoReconnect)) { IsBackground = true };
                    //this.Thread_TcpReconnect.IsBackground = true;
                    this.Thread_TcpReconnect.Start();
                }
                else
                    this.Auto_TcpReconnect.Set();
                if (this.AutoReceive)
                    this.NetStream.BeginRead(this.Buffer, 0, this.Buffer.Length, new AsyncCallback(TcpCallBack), this);
            }
            return result;
        }

        /// <summary>
        /// 重新连接方法
        /// </summary>
        internal void Reconnect()
        {
            string temp = string.Format("TCP主机地址：{0}，端口号：{1}", this.ServerIp, this.ServerPort); //TCP连接的主机地址与端口
            this.LastErrorMessage = "TCP连接意外断开，正在尝试重连。" + temp;
            //FileClient.WriteFailureInfo(LastErrorMessage);
            try
            {
                this.BaseClient.Close();
                this.BaseClient = this.HoldLocalPort ? new TcpClient(this.LocalEndPoint) : new TcpClient();
                this.BaseClient.Connect(this.ServerIp, this.ServerPort);
                this.SetName();

                //重连次数+1，同时调用事件委托
                this.ReconnTimer++;
                if (this.ReconnTimerChanged != null)
                    this.ReconnTimerChanged.BeginInvoke(this.Name, this.ReconnTimer, null, null);

                this.NetStream = BaseClient.GetStream();
                if (this.AutoReceive)
                    this.NetStream.BeginRead(this.Buffer, 0, this.Buffer.Length, new AsyncCallback(TcpCallBack), this);
                //FileClient.WriteFailureInfo("TCP重新连接成功。" + temp);
            }
            //假如出现异常，将错误信息写入日志并进入下一次循环
            catch (Exception e)
            {
                this.LastErrorMessage = string.Format("TCP重新连接失败：{0}。", e.Message) + temp;
            }
        }

        /// <summary>
        /// TCP重新连接方法
        /// </summary>
        private void TcpAutoReconnect()
        {
            while (true)
            {
                Thread.Sleep(1000);

                //假如属性提示未连接，则TCP重连线程挂起
                if (!this.IsConnected)
                    this.Auto_TcpReconnect.WaitOne();

                //假如属性提示已连接但实际上连接已断开，尝试重连
                else if (this.IsConnected && !this.IsSocketConnected())
                {
                    ////调用连接断开事件委托
                    //if (this.Disconnected != null)
                    //    this.Disconnected.BeginInvoke(this.Name, new EventArgs(), null, null);

                    this.Reconnect();
                    #region 原重连部分
                    //string temp = string.Format("TCP主机地址：{0}，端口号：{1}", this.ServerIp, this.ServerPort); //TCP连接的主机地址与端口
                    //this.LastErrorMessage = "TCP连接意外断开，正在尝试重连。" + temp;
                    //FileClient.WriteFailureInfo(LastErrorMessage);
                    //try
                    //{
                    //    //将这部分注释掉，防止出现访问被释放的资源异常
                    //    //NetStream.Close();
                    //    //NetStream.Dispose();
                    //    //Client.Close();
                    //    //this.BaseClient = new TcpClient(this.ServerIp, this.ServerPort);
                    //    this.BaseClient.Close();
                    //    this.BaseClient = this.HoldLocalPort ? new TcpClient(this.LocalEndPoint) : new TcpClient();
                    //    this.BaseClient.Connect(this.ServerIp, this.ServerPort);
                    //    this.SetName();

                    //    //重连次数+1，同时调用事件委托
                    //    this.ReconnTimer++;
                    //    if (this.ReconnTimerChanged != null)
                    //        this.ReconnTimerChanged.BeginInvoke(this.Name, this.ReconnTimer, null, null);
                    //    if (this.Connected != null)
                    //        this.Connected.BeginInvoke(this.Name, new EventArgs(), null, null);

                    //    this.NetStream = BaseClient.GetStream();
                    //    if (this.AutoReceive)
                    //        this.NetStream.BeginRead(this.Buffer, 0, this.Buffer.Length, new AsyncCallback(TcpCallBack), this);
                    //    this.IsSocketConnected();
                    //    FileClient.WriteFailureInfo("TCP重新连接成功。" + temp);
                    //}
                    ////假如出现异常，将错误信息写入日志并进入下一次循环
                    //catch (Exception e)
                    //{
                    //    this.LastErrorMessage = string.Format("TCP重新连接失败：{0}。", e.Message) + temp;
                    //    FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                    //    //TODO: 是否抛出异常？
                    //    //throw; //假如不需要抛出异常，注释此句
                    //}
                    #endregion
                }
            }
        }

        /// <summary>
        /// 从Tcp服务端获取信息的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void TcpCallBack(IAsyncResult ar)
        {
            DerivedTcpClient client = (DerivedTcpClient)ar.AsyncState;
            NetworkStream ns = client.NetStream;

            try
            {
                byte[] recdata = new byte[ns.EndRead(ar)];
                
                Array.Copy(client.Buffer, recdata, recdata.Length);
                //this.receivedEventArgs.ReceivedData = recdata;
                //this.receivedEventArgs.ReceivedInfo = HexHelper.ByteArray2HexString
                if (recdata.Length > 0)
                {
                    if (this.DataReceived != null)
                        this.DataReceived.BeginInvoke(client.Name, new Events.DataReceivedEventArgs(recdata), null, null); //异步输出数据
                    this.raiser.Click();
                    if (this.ReceiveRestTime > 0)
                        Thread.Sleep(this.ReceiveRestTime);
                    if (this.AutoReceive)
                        ns.BeginRead(client.Buffer, 0, client.Buffer.Length, new AsyncCallback(TcpCallBack), client);
                }
            }
            catch (Exception ex)
            {
                FileClient.WriteExceptionInfo(ex, string.Format("从TcpServer获取数据的过程中出错：IP地址{0}，端口{1}", this.ServerIp, this.ServerPort), true);
            }
        }

        /// <summary>
        /// 从服务端读信息
        /// </summary>
        /// <param name="asc">ASCII字符串</param>
        /// <param name="hex">16进制格式字符串</param>
        public void Read(out string asc, out string hex)
        {
            asc = hex = string.Empty;
            int available;
            if (this.BaseClient == null || (available = this.BaseClient.Available) == 0)
                return;
            try
            {
                this.NetStream.Read(this.Buffer, 0, available);
                Events.DataReceivedEventArgs args = new Events.DataReceivedEventArgs(this.Buffer);
                asc = args.ReceivedInfo_String;
                hex = args.ReceivedInfo_HexString;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 终止线程
        /// </summary>
        private void ThreadAbort()
        {
            this.Thread_TcpReconnect.Abort();
            this.Thread_TcpReconnect = null;
            this.Auto_TcpReconnect.Dispose();
        }

        /// <summary>
        /// 关闭TCP连接
        /// </summary>
        /// <returns>假如关闭成功，返回1，否则返回0</returns>
        public int Close()
        {
            this.LastErrorMessage = string.Empty;
            int result = 1;
            try
            {
                if (this.BaseClient != null && this.BaseClient.Connected)
                {
                    this.ThreadAbort(); //终止重连线程

                    //关闭流并释放资源
                    this.NetStream.Close();
                    this.NetStream.Dispose();
                    this.BaseClient.Close();
                    this.ServerIp = null;
                    this.ServerPort = 0;
                    this.IsConnected = false;
                    this.IsConnected_Socket = false;
                    //调用连接断开事件委托
                    if (this.Disconnected != null)
                        this.Disconnected.BeginInvoke(this.Name, new EventArgs(), null, null);

                    this.BaseClient = null;
                    this.raiser.Stop();
                }
            }
            catch (Exception e)
            {
                this.LastErrorMessage = string.Format("关闭TCP连接{0}失败：{1}", this.Name, e.Message);
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                result = 0;
                throw; //假如不需要抛出异常，注释此行
            }

            return result;
        }

        /// <summary>
        /// 更新并返回TcpClient的连接状态
        /// </summary>
        /// <returns>假如处于连接状态，返回true，否则返回false</returns>
        public bool IsSocketConnected()
        {
            //假如TcpClient对象为空
            if (this.BaseClient == null || this.BaseClient.Client == null)
                return false;

            Socket socket = BaseClient.Client;
            return (this.IsConnected_Socket = (!socket.Poll(1000, SelectMode.SelectRead) || socket.Available != 0) && socket.Connected);
        }

        /// <summary>
        /// TCP客户端以byte数组或16进制格式字符串发送数据
        /// </summary>
        /// <param name="data_origin">待发送数据，byte数组或16进制格式字符串</param>
        /// <param name="errorMessage">返回的错误信息，假如未报错则为空</param>
        /// <returns>返回发送结果</returns>
        public bool SendData(object data_origin, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!this.IsConnected || !this.IsSocketConnected())
            {
                errorMessage = string.Format("TCP服务端{0}未连接", this.Name);
                if (logging)
                    FileClient.WriteFailureInfo(errorMessage);
                return false;
            }
            byte[] data;
            string typeName = data_origin.GetType().Name;
            if (typeName == "Byte[]")
                data = (byte[])data_origin;
            else if (typeName == "String")
                data = HexHelper.HexString2Bytes((string)data_origin);
            else
            {
                errorMessage = string.Format("数据格式不正确（{0}），无法向TCP服务端{1}发送数据", typeName, this.Name);
                FileClient.WriteFailureInfo(errorMessage);
                return false;
            }
            try { this.NetStream.Write(data, 0, data.Length); }
            catch (Exception ex)
            {
                errorMessage = string.Format("向TCP服务端{0}发送数据失败 {1}", this.Name, ex.Message);
                FileClient.WriteExceptionInfo(ex, errorMessage, false);
                //MessageBox.Show(selClient.Name + ":" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送字符串
        /// </summary>
        /// <param name="content">字符串</param>
        public void SendString(string content)
        {
            //假如未连接，退出方法
            if (!this.IsConnected || !this.IsSocketConnected() || string.IsNullOrEmpty(content))
                return;

            try
            {
                //转换为byte类型数组并发送指令
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(content);//将字符串转换为byte数组
                NetStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                this.LastErrorMessage = "字符串发送失败：" + e.Message;
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                throw; //如果不需要抛出异常，注释此行
            }
        }

        /// <summary>
        /// 从TCP服务端接收信息并转换为字符串
        /// </summary>
        /// <returns>返回从TCP服务器接收到的信息</returns>
        public string ReceiveInfo()
        {
            //假如未连接，返回空字符串
            if (!this.IsConnected || !this.IsSocketConnected())
                return string.Empty;

            try
            {
                //接收信息并转换为字符串
                byte[] buffer = new byte[ReceiveBufferSize];
                NetStream.Flush(); //不知道有没有用
                NetStream.Read(buffer, 0, buffer.Length);
                string info = System.Text.Encoding.ASCII.GetString(buffer).Trim('\0'); //去除字符串头尾的空白字符
                if (info == null || info.Length == 0)
                    return string.Empty;

                //（根据正文开始、正文结束字符）从字符串中截取正式信息
                //信息开始处字符的索引，假如有正文开始字符，索引为该字符索引+1，否则为0
                int startIndex = info.Contains(Base.STX) ? info.IndexOf(Base.STX) : 0;
                //信息中包含的字符数量
                int length = info.Contains(Base.ETX) ? info.IndexOf(Base.ETX) - startIndex : info.Length - startIndex;
                info = info.Substring(startIndex, length);
                return info;
            }
            //假如出现异常，将错误信息记入日志并返回空字符串
            catch (Exception e)
            {
                LastErrorMessage = "信息接收失败：" + e.Message;
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                return string.Empty;
                throw; //如果不需要抛出异常，注释此行
            }
        }
    }
}
