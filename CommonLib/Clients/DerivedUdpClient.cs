﻿using System;
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

namespace CommonLib.Clients
{
    /// <summary>
    /// UDP连接客户端
    /// </summary>
    public class DerivedUdpClient
    {
        #region 事件
        /// <summary>
        /// UDP连接事件
        /// </summary>
        public event ConnectedEventHandler Connected;

        /// <summary>
        /// UDP断开事件
        /// </summary>
        public event DisconnectedEventHandler Disconnected;

        /// <summary>
        /// UDP重连成功次数改变事件
        /// </summary>
        public event ReconnTimerChangedEventHandler ReconnTimerChanged;

        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event Events.DataReceivedEventHandler DataReceived;

        /// <summary>
        /// 持续一段时间未接收到任何数据的事件
        /// </summary>
        public event NoneReceivedEventHandler OnNoneReceived;
        #endregion

        #region 私有成员变量
        /// <summary>
        /// UdpClient对象
        /// </summary>
        //private bool logging = false;
        private bool autoReceive = true;
        //private IPEndPoint remote_endpoint, local_endpoint;
        private readonly TimerEventRaiser raiser = new TimerEventRaiser(1000);
        #endregion

        #region 成员属性
        /// <summary>
        /// UdpClient对象
        /// </summary>
        public UdpClient BaseClient { get; private set; } = null;

        /// <summary>
        /// 与之建立UDP连接的主机IP地址
        /// </summary>
        public string ServerIp { get; set; }

        /// <summary>
        /// 建立UDP连接的端口
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// 远程IP终结点，未连接则为空
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; set; }
        //{
        //    get { return remote_endpoint; }
        //    set { remote_endpoint = value; }
        //}

        /// <summary>
        /// 指定的本地IP
        /// </summary>
        public string LocalIp { get; set; }

        /// <summary>
        /// 指定的本地端口
        /// </summary>
        public int LocalPort { get; set; }

        /// <summary>
        /// 本地IP终结点，未初始化则为空
        /// </summary>
        public IPEndPoint LocalEndPoint { get; set; }
        //{
        //    get { return local_endpoint; }
        //    set { local_endpoint = value; }
        //}

        /// <summary>
        /// 重新连接时是否保持同一个端口
        /// </summary>
        public bool HoldLocalPort { get; set; }

        /// <summary>
        /// 最新的错误信息
        /// </summary>
        public string LastErrorMessage { get; private set; }

        /// <summary>
        /// 是否自动接收（触发接收事件）
        /// </summary>
        public bool AutoReceive
        {
            get { return autoReceive; }
            set
            {
                bool prev = autoReceive;
                autoReceive = value;
                if (autoReceive == prev)
                    return;
                //if (autoReceive)
                //    NetStream.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(TcpCallBack), this);
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
        /// 是否已启动监听
        /// </summary>
        public bool IsStartListening { get; set; }

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Tcp Socket的
        /// </summary>
        //public bool IsConnected_Socket { get; private set; }
        public bool IsConnected_Socket { get { return (BaseClient != null && BaseClient.IsSocketConnected()); } }

        /// <summary>
        /// 在无返回数据时是否重新连接（默认为false），超过5秒重连，触发最短间隔5秒
        /// </summary>
        public bool ReconnectWhenReceiveNone { get; set; }

        /// <summary>
        /// 计时阈值，计时达到此值触发事件，单位毫秒，默认5000
        /// </summary>
        public int RaiseThreshold
        {
            get { return (int)raiser.RaiseThreshold; }
            set { raiser.RaiseThreshold = value < 0 ? 0 : (ulong)value; }
        }

        /// <summary>
        /// 触发间隔，两次触发事件间允许的最短时间长度，单位毫秒，默认5000
        /// </summary>
        public int RaiseInterval
        {
            get { return (int)raiser.RaiseInterval; }
            set { raiser.RaiseInterval = value < 0 ? 0 : (uint)value; }
        }

        /// <summary>
        /// 重新连接成功的次数
        /// </summary>
        public int ReconnTimer { get; private set; }

        /// <summary>
        /// 监听UDP服务端，并在连接断开后试着重新连接
        /// </summary>
        private Thread Thread_Reconnect { get; set; }

        /// <summary>
        /// 控制UDP重连线程的AutoResetEvent，初始状态为非终止
        /// </summary>
        private AutoResetEvent Auto_UdpReconnect { get; set; }
        #endregion

        /// <summary>
        /// 设置Tcp连接名称，格式：本地端口->服务端IP:端口
        /// </summary>
        public void SetName()
        {
            //try { Name = BaseClient.Client.GetName(out remote_endpoint, out local_endpoint); }
            try { Name = BaseClient.Client.GetName(); }
            catch (Exception) { Name = string.Empty; }
        }

        #region 构造器
        /// <summary>
        /// 构造器，以指定本地IP和端口号初始化一个未连接DerivedTcpClient对象，决定本地端口和是否接收数据
        /// </summary>
        /// <param name="local_ip">本地IP</param>
        /// <param name="local_port">本地端口号</param>
        /// <param name="auto_receive">是否自动接收</param>
        /// <param name="hold_port">重新连接时是否维持相同本地端口</param>
        public DerivedUdpClient(string local_ip, int local_port, bool auto_receive, bool hold_port)
        {
            LocalIp = local_ip;
            LocalPort = local_port;
            AutoReceive = auto_receive;
            HoldLocalPort = hold_port;
            LastErrorMessage = string.Empty;
            Name = string.Empty;
            ReceiveRestTime = 0;
            ReconnectWhenReceiveNone = false;
            RaiseThreshold = 5000;
            RaiseInterval = 5000;
            raiser.ThresholdReached += new TimerEventRaiser.ThresholdReachedEventHandler(Raiser_ThresholdReached);
            //BaseClient = !string.IsNullOrWhiteSpace(LocalIp) && LocalPort > 0 ? new UdpClient(new IPEndPoint(IPAddress.Parse(LocalIp), LocalPort)) : new UdpClient();
            InitBaseClient();
            //bool localDefined = !string.IsNullOrWhiteSpace(LocalIp) && LocalPort > 0; //本地节点被定义
            //BaseClient = localDefined ? new UdpClient(new IPEndPoint(IPAddress.Parse(LocalIp), LocalPort)) : new UdpClient();
            //LocalEndPoint = localDefined ? new IPEndPoint(IPAddress.Parse(LocalIp), LocalPort) : null;

            IsStartListening = true;
            raiser.Run();
            SetName();
            try
            {
                if (AutoReceive)
                    BaseClient.BeginReceive(new AsyncCallback(ReceiveCallBack), this);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~DerivedUdpClient()
        {
            raiser.Stop();
        }

        /// <summary>
        /// 构造器，以指定本地IP和端口号初始化一个未连接DerivedTcpClient对象，本地端口固定，默认接收数据
        /// </summary>
        /// <param name="local_ip">本地IP</param>
        /// <param name="local_port">本地端口号</param>
        public DerivedUdpClient(string local_ip, int local_port) : this(local_ip, local_port, true, true) { }

        /// <summary>
        /// 构造器，初始化一个未连接DerivedTcpClient对象，决定本地端口和是否接收数据
        /// </summary>
        /// <param name="autoReceive">是否自动接收</param>
        /// <param name="holdPort">重新连接时是否维持相同本地端口</param>
        public DerivedUdpClient(bool autoReceive, bool holdPort) : this(null, 0, autoReceive, holdPort) { }

        /// <summary>
        /// 构造器，初始化一个未连接DerivedTcpClient对象，不维持本地端口
        /// </summary>
        /// <param name="autoReceive">是否自动接收</param>
        public DerivedUdpClient(bool autoReceive) : this(autoReceive, false) { }

        /// <summary>
        /// 默认构造器，初始化一个未连接DerivedTcpClient对象，不维持本地端口
        /// </summary>
        public DerivedUdpClient() : this(true, false) { }
        #endregion

        /// <summary>
        /// 初始化本地BaseClient对象，同时更新LocalEndPoint
        /// </summary>
        private void InitBaseClient()
        {
            bool localDefined = !string.IsNullOrWhiteSpace(LocalIp) && LocalPort > 0; //本地节点被定义
            //BaseClient = !string.IsNullOrWhiteSpace(LocalIp) && LocalPort > 0 ? new UdpClient(new IPEndPoint(IPAddress.Parse(LocalIp), LocalPort)) : new UdpClient();
            BaseClient = localDefined ? new UdpClient(new IPEndPoint(IPAddress.Parse(LocalIp), LocalPort)) : new UdpClient();
            LocalEndPoint = localDefined ? new IPEndPoint(IPAddress.Parse(LocalIp), LocalPort) : null;
        }

        private void Raiser_ThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            OnNoneReceived?.Invoke(this, new NoneReceivedEventArgs((ulong)RaiseThreshold)); //调用超时未接收的事件委托
            if (!ReconnectWhenReceiveNone)
                return;

            Reconnect_TheWholeDeal();
        }

        #region 连接
        /// <summary>
        /// 与UDP服务端连接
        /// </summary>
        /// <returns>假如建立连接成功，返回1，否则返回0</returns>
        public int Connect()
        {
            //return Connect(ServerIp, ServerPort);
            return Connect(ServerIp, ServerPort, LocalIp, LocalPort);
        }

        /// <summary>
        /// 与UDP服务端与端口连接
        /// </summary>
        /// <param name="server">UDP服务端IP</param>
        /// <param name="port">端口号</param>
        /// <returns>假如建立连接成功，返回1，否则返回0</returns>
        public int Connect(string server, int port)
        {
            //return Connect(server, port, null, 0);
            return Connect(server, port, LocalIp, LocalPort);
        }

        /// <summary>
        /// 利用特定的本地端口与UDP服务端连接
        /// </summary>
        /// <param name="server">UDP服务端IP</param>
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
                LocalIp = localIp;
                LocalPort = localPort;
                if (BaseClient == null)
                    //BaseClient = !string.IsNullOrWhiteSpace(LocalIp) && LocalPort > 0 ? new UdpClient(new IPEndPoint(IPAddress.Parse(LocalIp), LocalPort)) : new UdpClient();
                    InitBaseClient();
                BaseClient.Connect(ServerIp, ServerPort);
                RemoteEndPoint = !string.IsNullOrWhiteSpace(ServerIp) && ServerPort > 0 ? new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort) : null;
                SetName(); //修改连接名称

                //重置重连次数，同时调用事件委托
                ReconnTimer = 0;
                ReconnTimerChanged?.BeginInvoke(Name, ReconnTimer, null, null);

                //BaseClient.ReceiveBufferSize = ReceiveBufferSize; //接收缓冲区的大小
                //NetStream = BaseClient.GetStream(); //发送与接收数据的数据流对象
            }
            catch (Exception e)
            {
                LastErrorMessage = string.Format("无法建立UDP连接，IP{0}，端口号{1}：{2}", ServerIp, ServerPort, e.Message);
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                Close();
                result = 0;
                //throw; //假如不需要抛出异常，注释此行
            }

            IsConnected = true;
            //IsSocketConnected();
            if (IsConnected)
            {
                //调用Tcp连接事件委托
                Connected?.BeginInvoke(Name, (new EventArgs()), null, null);
                if (Thread_Reconnect == null)
                {
                    Auto_UdpReconnect = new AutoResetEvent(true);
                    Thread_Reconnect = new Thread(new ThreadStart(TcpAutoReconnect)) { IsBackground = true };
                    //Thread_TcpReconnect.IsBackground = true;
                    Thread_Reconnect.Start();
                }
                else
                    Auto_UdpReconnect.Set();
                //if (AutoReceive)
                try { BaseClient.BeginReceive(new AsyncCallback(ReceiveCallBack), this); }
                catch (Exception) { }
            }
            return result;
        }

        /// <summary>
        /// UDP重新连接方法
        /// </summary>
        private void TcpAutoReconnect()
        {
            while (true)
            {
                Thread.Sleep(1000);

                //假如属性提示未连接，则UDP重连线程挂起
                if (!IsConnected)
                    Auto_UdpReconnect.WaitOne();
                //假如属性提示已连接但实际上连接已断开，尝试重连
                //else if (IsConnected && !IsSocketConnected())
                else if (IsConnected && !IsConnected_Socket)
                {
                    string temp = string.Format("UDP主机地址：{0}，端口号：{1}", ServerIp, ServerPort); //UDP连接的主机地址与端口
                    LastErrorMessage = "UDP连接意外断开，正在尝试重连。" + temp;
                    FileClient.WriteFailureInfo(LastErrorMessage);
                    try
                    {
                        BaseClient.Close();
                        BaseClient = HoldLocalPort ? new UdpClient(LocalEndPoint) : new UdpClient();
                        BaseClient.Connect(ServerIp, ServerPort);
                        SetName();

                        //重连次数+1，同时调用事件委托
                        ReconnTimer++;
                        ReconnTimerChanged?.BeginInvoke(Name, ReconnTimer, null, null);
                        Connected?.BeginInvoke(Name, new EventArgs(), null, null);

                        //NetStream = BaseClient.GetStream();
                        if (AutoReceive)
                            BaseClient.BeginReceive(new AsyncCallback(ReceiveCallBack), this);
                        //NetStream.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(TcpCallBack), this);
                        //IsSocketConnected();
                        FileClient.WriteFailureInfo("UDP重新连接成功。" + temp);
                    }
                    //假如出现异常，将错误信息写入日志并进入下一次循环
                    catch (Exception e)
                    {
                        LastErrorMessage = string.Format("UDP重新连接失败：{0}。", e.Message) + temp;
                        FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                        continue;
                        //TODO: 是否抛出异常？
                        //throw; //假如不需要抛出异常，注释此句
                    }
                }
            }
        }

        ///// <summary>
        ///// 更新并返回TcpClient的连接状态
        ///// </summary>
        ///// <returns>假如处于连接状态，返回true，否则返回false</returns>
        //public bool IsSocketConnected()
        //{
        //    //return IsConnected_Socket = (BaseClient != null && BaseClient.IsSocketConnected());
        //    return IsConnected_Socket;
        //}

        /// <summary>
        /// 关闭UDP连接
        /// </summary>
        /// <returns>假如关闭成功，返回1，否则返回0</returns>
        public int Close()
        {
            LastErrorMessage = string.Empty;
            int result = 1;
            try
            {
                if (BaseClient != null)
                {
                    ThreadAbort(); //终止重连线程

                    //关闭流并释放资源
                    //NetStream.Close();
                    //NetStream.Dispose();
                    BaseClient.Close();
                    IsStartListening = false;
                    ServerIp = null;
                    ServerPort = 0;
                    IsConnected = false;
                    //IsConnected_Socket = false;
                    //调用连接断开事件委托
                    Disconnected?.BeginInvoke(Name, new EventArgs(), null, null);

                    BaseClient.Close();
                    BaseClient = null;
                }
            }
            catch (Exception e)
            {
                LastErrorMessage = string.Format("关闭UDP连接{0}失败：{1}", Name, e.Message);
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                result = 0;
                throw; //假如不需要抛出异常，注释此行
            }

            return result;
        }

        /// <summary>
        /// 重连方法，假如曾建立过连接，则断开然后重新连接一下
        /// </summary>
        public void Reconnect_TheWholeDeal()
        {
            try
            {
                if (IsConnected)
                {
                    Close();
                    Connect();
                }
            }
            catch (Exception) { }
        }
        #endregion

        /// <summary>
        /// 从Tcp服务端获取信息的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            DerivedUdpClient client = (DerivedUdpClient)ar.AsyncState;

            try
            {
                if (BaseClient == null)
                    return;
                //byte[] recdata = BaseClient.EndReceive(ar, ref remote_endpoint);
                IPEndPoint point = null;
                byte[] recdata = BaseClient.EndReceive(ar, ref point);
                //RemoteEndPoint = point;

                if (recdata.Length > 0)
                {
                    DataReceived?.BeginInvoke(client.Name, new Events.DataReceivedEventArgs(recdata), null, null); //异步输出数据
                    raiser.Click();
                    if (ReceiveRestTime > 0)
                        Thread.Sleep(ReceiveRestTime);
                    if (AutoReceive)
                        BaseClient.BeginReceive(new AsyncCallback(ReceiveCallBack), client);
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
            if (BaseClient == null || BaseClient.Available == 0)
                return;
            try
            {
                //byte[] data = BaseClient.Receive(ref remote_endpoint);
                IPEndPoint point = null;
                byte[] data = BaseClient.Receive(ref point);
                Events.DataReceivedEventArgs args = new Events.DataReceivedEventArgs(data);
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
            if (Thread_Reconnect != null)
            {
                Thread_Reconnect.Abort();
                Thread_Reconnect = null;
            }
            Auto_UdpReconnect?.Dispose();
        }

        #region 数据发送
        /// <summary>
        /// UDP客户端以byte数组或16进制格式字符串发送数据
        /// </summary>
        /// <param name="data_origin">待发送数据，byte数组或16进制格式字符串</param>
        /// <param name="errorMessage">返回的错误信息，假如未报错则为空</param>
        /// <returns>返回发送结果</returns>
        public bool SendData(object data_origin, out string errorMessage)
        {
            //return SendData(data_origin, RemoteEndPoint, out errorMessage);
            //在建立连接的情况下，不要指定EndPoint，否则报错误“建立连接时无法将数据包发送给任意主机”
            return SendData(data_origin, null, out errorMessage);
        }

        /// <summary>
        /// UDP客户端以byte数组或16进制格式字符串发送数据
        /// </summary>
        /// <param name="data_origin">待发送数据，byte数组或16进制格式字符串</param>
        /// <param name="ip">远程IP</param>
        /// <param name="port">远程端口</param>
        /// <param name="errorMessage">返回的错误信息，假如未报错则为空</param>
        /// <returns>返回发送结果</returns>
        public bool SendData(object data_origin, string ip, int port, out string errorMessage)
        {
            return SendData(data_origin, new IPEndPoint(IPAddress.Parse(ip), port), out errorMessage);
        }

        /// <summary>
        /// UDP客户端以byte数组或16进制格式字符串发送数据
        /// </summary>
        /// <param name="data_origin">待发送数据，byte数组或16进制格式字符串</param>
        /// <param name="endPoint">远程IP终结点</param>
        /// <param name="errorMessage">返回的错误信息，假如未报错则为空</param>
        /// <returns>返回发送结果</returns>
        public bool SendData(object data_origin, IPEndPoint endPoint, out string errorMessage)
        {
            errorMessage = string.Empty;
            //if (!IsConnected || !IsSocketConnected())
            //{
            //    errorMessage = string.Format("UDP服务端{0}未连接", Name);
            //    if (logging)
            //        FileClient.WriteFailureInfo(errorMessage);
            //    return false;
            //}
            byte[] data;
            string typeName = data_origin.GetType().Name;
            if (typeName == "Byte[]")
                data = (byte[])data_origin;
            else if (typeName == "String")
                data = HexHelper.HexString2Bytes((string)data_origin);
            else
            {
                errorMessage = string.Format("数据格式不正确（{0}），无法向UDP服务端{1}发送数据", typeName, Name);
                FileClient.WriteFailureInfo(errorMessage);
                return false;
            }
            try { BaseClient.Send(data, data.Length, endPoint); }
            catch (Exception ex)
            {
                errorMessage = string.Format("向UDP服务端{0}发送数据失败 {1}", Name, ex.Message);
                FileClient.WriteExceptionInfo(ex, errorMessage, false);
                return false;
            }
            return true;
        }

        #region 发送字符串
        /// <summary>
        /// 发送字符串
        /// </summary>
        /// <param name="content">字符串</param>
        public void SendString(string content)
        {
            //SendString(content, RemoteEndPoint);
            //在建立连接的情况下，不要指定EndPoint，否则报错误“建立连接时无法将数据包发送给任意主机”
            SendString(content, null);
        }

        /// <summary>
        /// 发送字符串
        /// </summary>
        /// <param name="content">字符串</param>
        /// <param name="ip">远程IP</param>
        /// <param name="port">远程端口</param>
        public void SendString(string content, string ip, int port)
        {
            SendString(content, new IPEndPoint(IPAddress.Parse(ip), port));
        }

        /// <summary>
        /// 发送字符串
        /// </summary>
        /// <param name="content">字符串</param>
        /// <param name="endPoint">远程IP终结点</param>
        public void SendString(string content, IPEndPoint endPoint)
        {
            ////假如未连接，退出方法
            //if (/*!IsConnected || */!IsSocketConnected() || string.IsNullOrEmpty(content))
            //    return;

            try
            {
                //转换为byte类型数组并发送指令
                byte[] bytes = Encoding.ASCII.GetBytes(content);//将字符串转换为byte数组
                BaseClient.Send(bytes, bytes.Length, endPoint);
            }
            catch (Exception e)
            {
                LastErrorMessage = "字符串发送失败：" + e.Message;
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                throw; //如果不需要抛出异常，注释此行
            }
        }
        #endregion
        #endregion
    }
}
