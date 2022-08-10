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
using CommonLib.Extensions;
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
            get { return baseClient; }
            private set { baseClient = value; }
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
            get { return local_endpoint; }
            set { local_endpoint = value; }
        }

        /// <summary>
        /// 远程IP终结点，未连接则为空
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return remote_endpoint; }
            set { remote_endpoint = value; }
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
            get { return receiveBufferSize; }
            set
            {
                receiveBufferSize = value;
                Buffer = new byte[receiveBufferSize];
                if (BaseClient != null)
                    BaseClient.ReceiveBufferSize = receiveBufferSize;
            }
        }

        /// <summary>
        /// 接收缓冲区
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// 是否自动接收（触发接收事件）
        /// </summary>
        public bool AutoReceive { get { return autoReceive; }
            set
            {
                bool prev = autoReceive;
                autoReceive = value;
                if (autoReceive == prev)
                    return;
                if (autoReceive)
                    NetStream.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(TcpCallBack), this);
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
        public bool IsConnected_Socket { get { return BaseClient != null && BaseClient.IsSocketConnected(); } }
        //public bool IsConnected_Socket { get; private set; }

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
            try { Name = BaseClient.Client.GetName(out remote_endpoint, out local_endpoint); }
            catch (Exception) { Name = string.Empty; }
        }

        /// <summary>
        /// 构造器，初始化一个未连接DerivedTcpClient对象，决定是否自动接收数据、无返回数据时是否重新连接以及是否维持相同本地端口
        /// </summary>
        /// <param name="autoReceive">是否自动接收</param>
        /// <param name="reconn_noreceive">无返回数据时是否重新连接</param>
        /// <param name="holdPort">重新连接时是否维持相同本地端口</param>
        public DerivedTcpClient(bool autoReceive, bool reconn_noreceive, bool holdPort)
        {
            AutoReceive = autoReceive;
            ReconnectWhenReceiveNone = reconn_noreceive;
            HoldLocalPort = holdPort;
            ReceiveBufferSize = 32768;
            Buffer = new byte[ReceiveBufferSize];
            LastErrorMessage = string.Empty;
            Name = string.Empty;
            ReceiveRestTime = 0;

            raiser.RaiseThreshold = 10000;
            raiser.RaiseInterval = 5000;
            raiser.ThresholdReached += new ThresholdReachedEventHandler(Raiser_ThresholdReached);
        }

        private void Raiser_ThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            if (!ReconnectWhenReceiveNone)
                return;

            Reconnect();
        }

        /// <summary>
        /// 构造器，初始化一个未连接DerivedTcpClient对象，决定是否自动接收数据以及是否维持相同本地端口
        /// </summary>
        /// <param name="autoReceive">是否自动接收</param>
        /// <param name="holdPort">重新连接时是否维持相同本地端口</param>
        public DerivedTcpClient(bool autoReceive, bool holdPort) : this(autoReceive, false, holdPort)
        {
            //AutoReceive = autoReceive;
            //HoldLocalPort = holdPort;
            //ReceiveBufferSize = 2048;
            //Buffer = new byte[ReceiveBufferSize];
            //LastErrorMessage = string.Empty;
            //Name = string.Empty;
            //ReceiveRestTime = 0;
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
            Connect(server, port);
        }

        /// <summary>
        /// 与TCP服务端连接
        /// </summary>
        /// <returns>假如建立连接成功，返回1，否则返回0</returns>
        public int Connect()
        {
            return Connect(ServerIp, ServerPort);
        }

        /// <summary>
        /// 与TCP服务端与端口连接
        /// </summary>
        /// <param name="server">TCP服务端IP</param>
        /// <param name="port">端口号</param>
        /// <returns>假如建立连接成功，返回1，否则返回0</returns>
        public int Connect(string server, int port)
        {
            return Connect(server, port, null, 0);
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
                ServerIp = server;
                ServerPort = port;
                //BaseClient = new TcpClient(ServerIp, ServerPort);
                BaseClient = !string.IsNullOrWhiteSpace(localIp) && localPort > 0 ? new TcpClient(new IPEndPoint(IPAddress.Parse(localIp), localPort)) : new TcpClient();
                BaseClient.Connect(ServerIp, ServerPort);
                SetName(); //修改连接名称

                //重置重连次数，同时调用事件委托
                ReconnTimer = 0;
                if (ReconnTimerChanged != null)
                    ReconnTimerChanged.BeginInvoke(Name, ReconnTimer, null, null);

                BaseClient.ReceiveBufferSize = ReceiveBufferSize; //接收缓冲区的大小
                NetStream = BaseClient.GetStream(); //发送与接收数据的数据流对象
                raiser.Run();
            }
            catch (Exception e)
            {
                LastErrorMessage = string.Format("无法建立TCP连接，IP{0}，端口号{1}：{2}", ServerIp, ServerPort, e.Message);
                if (logging)
                    FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                Close();
                result = 0;
                throw; //假如不需要抛出异常，注释此行
            }

            IsConnected = BaseClient.Connected;
            //IsSocketConnected();
            if (IsConnected)
            {
                //调用Tcp连接事件委托
                if (Connected != null)
                    Connected.BeginInvoke(Name, (new EventArgs()), null, null);
                if (Thread_TcpReconnect == null)
                {
                    Auto_TcpReconnect = new AutoResetEvent(true);
                    Thread_TcpReconnect = new Thread(new ThreadStart(TcpAutoReconnect)) { IsBackground = true };
                    //Thread_TcpReconnect.IsBackground = true;
                    Thread_TcpReconnect.Start();
                }
                else
                    Auto_TcpReconnect.Set();
                if (AutoReceive)
                    NetStream.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(TcpCallBack), this);
            }
            return result;
        }

        /// <summary>
        /// 重新连接方法
        /// </summary>
        internal void Reconnect()
        {
            string temp = string.Format("TCP主机地址：{0}，端口号：{1}", ServerIp, ServerPort); //TCP连接的主机地址与端口
            LastErrorMessage = "TCP连接意外断开，正在尝试重连。" + temp;
            //FileClient.WriteFailureInfo(LastErrorMessage);
            try
            {
                BaseClient.Close();
                BaseClient = HoldLocalPort ? new TcpClient(LocalEndPoint) : new TcpClient();
                BaseClient.Connect(ServerIp, ServerPort);
                SetName();

                //重连次数+1，同时调用事件委托
                ReconnTimer++;
                if (ReconnTimerChanged != null)
                    ReconnTimerChanged.BeginInvoke(Name, ReconnTimer, null, null);

                NetStream = BaseClient.GetStream();
                if (AutoReceive)
                    NetStream.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(TcpCallBack), this);
                //FileClient.WriteFailureInfo("TCP重新连接成功。" + temp);
            }
            //假如出现异常，将错误信息写入日志并进入下一次循环
            catch (Exception e)
            {
                LastErrorMessage = string.Format("TCP重新连接失败：{0}。", e.Message) + temp;
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
                if (!IsConnected)
                    Auto_TcpReconnect.WaitOne();
                //假如属性提示已连接但实际上连接已断开，尝试重连
                //else if (IsConnected && !IsSocketConnected())
                else if (IsConnected && !IsConnected_Socket)
                {
                    ////调用连接断开事件委托
                    //if (Disconnected != null)
                    //    Disconnected.BeginInvoke(Name, new EventArgs(), null, null);

                    Reconnect();
                    #region 原重连部分
                    //string temp = string.Format("TCP主机地址：{0}，端口号：{1}", ServerIp, ServerPort); //TCP连接的主机地址与端口
                    //LastErrorMessage = "TCP连接意外断开，正在尝试重连。" + temp;
                    //FileClient.WriteFailureInfo(LastErrorMessage);
                    //try
                    //{
                    //    //将这部分注释掉，防止出现访问被释放的资源异常
                    //    //NetStream.Close();
                    //    //NetStream.Dispose();
                    //    //Client.Close();
                    //    //BaseClient = new TcpClient(ServerIp, ServerPort);
                    //    BaseClient.Close();
                    //    BaseClient = HoldLocalPort ? new TcpClient(LocalEndPoint) : new TcpClient();
                    //    BaseClient.Connect(ServerIp, ServerPort);
                    //    SetName();

                    //    //重连次数+1，同时调用事件委托
                    //    ReconnTimer++;
                    //    if (ReconnTimerChanged != null)
                    //        ReconnTimerChanged.BeginInvoke(Name, ReconnTimer, null, null);
                    //    if (Connected != null)
                    //        Connected.BeginInvoke(Name, new EventArgs(), null, null);

                    //    NetStream = BaseClient.GetStream();
                    //    if (AutoReceive)
                    //        NetStream.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(TcpCallBack), this);
                    //    IsSocketConnected();
                    //    FileClient.WriteFailureInfo("TCP重新连接成功。" + temp);
                    //}
                    ////假如出现异常，将错误信息写入日志并进入下一次循环
                    //catch (Exception e)
                    //{
                    //    LastErrorMessage = string.Format("TCP重新连接失败：{0}。", e.Message) + temp;
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
                //receivedEventArgs.ReceivedData = recdata;
                //receivedEventArgs.ReceivedInfo = HexHelper.ByteArray2HexString
                if (recdata.Length > 0)
                {
                    if (DataReceived != null)
                        DataReceived.BeginInvoke(client.Name, new Events.DataReceivedEventArgs(recdata), null, null); //异步输出数据
                    raiser.Click();
                    if (ReceiveRestTime > 0)
                        Thread.Sleep(ReceiveRestTime);
                    if (AutoReceive)
                        ns.BeginRead(client.Buffer, 0, client.Buffer.Length, new AsyncCallback(TcpCallBack), client);
                }
            }
            catch (Exception ex)
            {
                FileClient.WriteExceptionInfo(ex, string.Format("从TcpServer获取数据的过程中出错：IP地址{0}，端口{1}", ServerIp, ServerPort), true);
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
            if (BaseClient == null || (available = BaseClient.Available) == 0)
                return;
            try
            {
                NetStream.Read(Buffer, 0, available);
                Events.DataReceivedEventArgs args = new Events.DataReceivedEventArgs(Buffer);
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
            Thread_TcpReconnect.Abort();
            Thread_TcpReconnect = null;
            Auto_TcpReconnect.Dispose();
        }

        /// <summary>
        /// 关闭TCP连接
        /// </summary>
        /// <returns>假如关闭成功，返回1，否则返回0</returns>
        public int Close()
        {
            LastErrorMessage = string.Empty;
            int result = 1;
            try
            {
                if (BaseClient != null && BaseClient.Connected)
                {
                    ThreadAbort(); //终止重连线程

                    //关闭流并释放资源
                    NetStream.Close();
                    NetStream.Dispose();
                    BaseClient.Close();
                    ServerIp = null;
                    ServerPort = 0;
                    IsConnected = false;
                    //IsConnected_Socket = false;
                    //调用连接断开事件委托
                    if (Disconnected != null)
                        Disconnected.BeginInvoke(Name, new EventArgs(), null, null);

                    BaseClient = null;
                    raiser.Stop();
                }
            }
            catch (Exception e)
            {
                LastErrorMessage = string.Format("关闭TCP连接{0}失败：{1}", Name, e.Message);
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                result = 0;
                throw; //假如不需要抛出异常，注释此行
            }

            return result;
        }

        ///// <summary>
        ///// 更新并返回TcpClient的连接状态
        ///// </summary>
        ///// <returns>假如处于连接状态，返回true，否则返回false</returns>
        //public bool IsSocketConnected()
        //{
        //    ////假如TcpClient对象为空
        //    //if (BaseClient == null || BaseClient.Client == null)
        //    //    return false;

        //    //Socket socket = BaseClient.Client;
        //    //return (IsConnected_Socket = (!socket.Poll(1000, SelectMode.SelectRead) || socket.Available != 0) && socket.Connected);

        //    return IsConnected_Socket = (BaseClient != null && BaseClient.IsSocketConnected());
        //}

        /// <summary>
        /// TCP客户端以byte数组或16进制格式字符串发送数据
        /// </summary>
        /// <param name="data_origin">待发送数据，byte数组或16进制格式字符串</param>
        /// <param name="errorMessage">返回的错误信息，假如未报错则为空</param>
        /// <returns>返回发送结果</returns>
        public bool SendData(object data_origin, out string errorMessage)
        {
            errorMessage = string.Empty;
            //if (!IsConnected || !IsSocketConnected())
            if (!IsConnected || !IsConnected_Socket)
            {
                errorMessage = string.Format("TCP服务端{0}未连接", Name);
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
                errorMessage = string.Format("数据格式不正确（{0}），无法向TCP服务端{1}发送数据", typeName, Name);
                FileClient.WriteFailureInfo(errorMessage);
                return false;
            }
            try { NetStream.Write(data, 0, data.Length); }
            catch (Exception ex)
            {
                errorMessage = string.Format("向TCP服务端{0}发送数据失败 {1}", Name, ex.Message);
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
            //if (!IsConnected || !IsSocketConnected() || string.IsNullOrEmpty(content))
            if (!IsConnected || !IsConnected_Socket || string.IsNullOrEmpty(content))
                return;

            try
            {
                //转换为byte类型数组并发送指令
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(content);//将字符串转换为byte数组
                NetStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                LastErrorMessage = "字符串发送失败：" + e.Message;
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
            //if (!IsConnected || !IsSocketConnected())
            if (!IsConnected || !IsConnected_Socket)
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
