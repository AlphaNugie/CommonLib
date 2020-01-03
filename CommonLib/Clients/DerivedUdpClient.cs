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

namespace CommonLib.Clients
{
    /// <summary>
    /// TCP连接客户端
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
        #endregion
        #region 私有成员变量
        /// <summary>
        /// UdpClient对象
        /// </summary>
        private UdpClient baseClient = null;
        private bool logging = false;
        private bool autoReceive = true;
        private IPEndPoint remote_endpoint, local_endpoint;
        #endregion
        #region 成员属性
        /// <summary>
        /// UdpClient对象
        /// </summary>
        public UdpClient BaseClient
        {
            get { return this.baseClient; }
            private set { this.baseClient = value; }
        }

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
        /// 是否自动接收（触发接收事件）
        /// </summary>
        public bool AutoReceive
        {
            get { return this.autoReceive; }
            set
            {
                bool prev = this.autoReceive;
                this.autoReceive = value;
                if (this.autoReceive == prev)
                    return;
                //if (this.autoReceive)
                //    this.NetStream.BeginRead(this.Buffer, 0, this.Buffer.Length, new AsyncCallback(TcpCallBack), this);
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
        public bool IsConnected_Socket { get; private set; }

        /// <summary>
        /// 重新连接成功的次数
        /// </summary>
        public int ReconnTimer { get; private set; }

        /// <summary>
        /// 监听TCP服务端，并在连接断开后试着重新连接
        /// </summary>
        private Thread Thread_Reconnect { get; set; }

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
            //try
            //{
            //    IPEndPoint iepR = this.BaseClient.Client.RemoteEndPoint == null ? null : (IPEndPoint)this.BaseClient.Client.RemoteEndPoint;
            //    IPEndPoint iepL = this.BaseClient.Client.LocalEndPoint == null ? null : (IPEndPoint)this.BaseClient.Client.LocalEndPoint;
            //    this.Name = (iepL == null ? string.Empty : iepL.ToString()) + (iepR == null ? string.Empty : ("->" + iepR.ToString()));
            //    this.RemoteEndPoint = iepR;
            //    this.LocalEndPoint = iepL;
            //}
            //catch (Exception) { }
            try { this.Name = this.BaseClient.Client.GetName(out this.remote_endpoint, out this.local_endpoint); }
            catch (Exception e) { this.Name = string.Empty; }
        }

        /// <summary>
        /// 构造器，以指定本地IP和端口号初始化一个未连接DerivedTcpClient对象，决定本地端口和是否接收数据
        /// </summary>
        /// <param name="local_ip">本地IP</param>
        /// <param name="local_port">本地端口号</param>
        /// <param name="auto_receive">是否自动接收</param>
        /// <param name="hold_port">重新连接时是否维持相同本地端口</param>
        public DerivedUdpClient(string local_ip, int local_port, bool auto_receive, bool hold_port)
        {
            this.AutoReceive = auto_receive;
            this.HoldLocalPort = hold_port;
            this.LastErrorMessage = string.Empty;
            this.Name = string.Empty;
            this.ReceiveRestTime = 0;
            //if (!string.IsNullOrWhiteSpace(local_ip) && local_port > 0)
                //this.BaseClient = new UdpClient(new IPEndPoint(IPAddress.Parse(local_ip), local_port));
            this.BaseClient = !string.IsNullOrWhiteSpace(local_ip) && local_port > 0 ? new UdpClient(new IPEndPoint(IPAddress.Parse(local_ip), local_port)) : new UdpClient();
            this.IsStartListening = true;
            this.SetName();
            if (this.AutoReceive)
                this.BaseClient.BeginReceive(new AsyncCallback(ReceiveCallBack), this);
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

        ///// <summary>
        ///// 构造器，初始化一个DerivedTcpClient对象并连接，不维持本地端口
        ///// </summary>
        ///// <param name="server">Udp远程地址</param>
        ///// <param name="port">Udp远程端口号</param>
        //public DerivedUdpClient(string remote_server, int remote_port) : this()
        //{
        //    this.Connect(remote_server, remote_port);
        //}

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
                if (this.BaseClient == null)
                    this.BaseClient = !string.IsNullOrWhiteSpace(localIp) && localPort > 0 ? new UdpClient(new IPEndPoint(IPAddress.Parse(localIp), localPort)) : new UdpClient();
                this.BaseClient.Connect(this.ServerIp, this.ServerPort);
                this.SetName(); //修改连接名称

                //重置重连次数，同时调用事件委托
                this.ReconnTimer = 0;
                if (this.ReconnTimerChanged != null)
                    this.ReconnTimerChanged.BeginInvoke(this.Name, this.ReconnTimer, null, null);

                //this.BaseClient.ReceiveBufferSize = this.ReceiveBufferSize; //接收缓冲区的大小
                //this.NetStream = this.BaseClient.GetStream(); //发送与接收数据的数据流对象
            }
            catch (Exception e)
            {
                this.LastErrorMessage = string.Format("无法建立TCP连接，IP{0}，端口号{1}：{2}", this.ServerIp, this.ServerPort, e.Message);
                FileClient.WriteExceptionInfo(e, this.LastErrorMessage, false);
                this.Close();
                result = 0;
                //throw; //假如不需要抛出异常，注释此行
            }

            this.IsConnected = true;
            this.IsSocketConnected();
            if (this.IsConnected)
            {
                //调用Tcp连接事件委托
                if (this.Connected != null)
                    this.Connected.BeginInvoke(this.Name, (new EventArgs()), null, null);
                if (this.Thread_Reconnect == null)
                {
                    this.Auto_TcpReconnect = new AutoResetEvent(true);
                    this.Thread_Reconnect = new Thread(new ThreadStart(this.TcpAutoReconnect)) { IsBackground = true };
                    //this.Thread_TcpReconnect.IsBackground = true;
                    this.Thread_Reconnect.Start();
                }
                else
                    this.Auto_TcpReconnect.Set();
                //if (this.AutoReceive)
                //    this.BaseClient.BeginReceive(new AsyncCallback(ReceiveCallBack), this);
                    //this.NetStream.BeginRead(this.Buffer, 0, this.Buffer.Length, new AsyncCallback(TcpCallBack), this);
            }
            return result;
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
                    string temp = string.Format("TCP主机地址：{0}，端口号：{1}", this.ServerIp, this.ServerPort); //TCP连接的主机地址与端口
                    this.LastErrorMessage = "TCP连接意外断开，正在尝试重连。" + temp;
                    FileClient.WriteFailureInfo(LastErrorMessage);
                    try
                    {
                        this.BaseClient.Close();
                        this.BaseClient = this.HoldLocalPort ? new UdpClient(this.LocalEndPoint) : new UdpClient();
                        this.BaseClient.Connect(this.ServerIp, this.ServerPort);
                        this.SetName();

                        //重连次数+1，同时调用事件委托
                        this.ReconnTimer++;
                        if (this.ReconnTimerChanged != null)
                            this.ReconnTimerChanged.BeginInvoke(this.Name, this.ReconnTimer, null, null);
                        if (this.Connected != null)
                            this.Connected.BeginInvoke(this.Name, new EventArgs(), null, null);

                        //this.NetStream = BaseClient.GetStream();
                        if (this.AutoReceive)
                            this.BaseClient.BeginReceive(new AsyncCallback(ReceiveCallBack), this);
                            //this.NetStream.BeginRead(this.Buffer, 0, this.Buffer.Length, new AsyncCallback(TcpCallBack), this);
                        this.IsSocketConnected();
                        FileClient.WriteFailureInfo("TCP重新连接成功。" + temp);
                    }
                    //假如出现异常，将错误信息写入日志并进入下一次循环
                    catch (Exception e)
                    {
                        this.LastErrorMessage = string.Format("TCP重新连接失败：{0}。", e.Message) + temp;
                        FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                        continue;
                        //TODO: 是否抛出异常？
                        //throw; //假如不需要抛出异常，注释此句
                    }
                }
            }
        }

        /// <summary>
        /// 从Tcp服务端获取信息的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            DerivedUdpClient client = (DerivedUdpClient)ar.AsyncState;

            try
            {
                //IPEndPoint point = this.RemoteEndPoint;
                byte[] recdata = this.BaseClient.EndReceive(ar, ref this.remote_endpoint);
                //this.RemoteEndPoint = point;

                if (recdata.Length > 0)
                {
                    if (this.DataReceived != null)
                        this.DataReceived.BeginInvoke(client.Name, new Events.DataReceivedEventArgs(recdata), null, null); //异步输出数据
                    if (this.ReceiveRestTime > 0)
                        Thread.Sleep(this.ReceiveRestTime);
                    if (this.AutoReceive)
                        this.BaseClient.BeginReceive(new AsyncCallback(ReceiveCallBack), client);
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
            int available = 0;
            if (this.BaseClient == null || (available = this.BaseClient.Available) == 0)
                return;
            try
            {
                //IPEndPoint point = this.RemoteEndPoint;
                byte[] data = this.BaseClient.Receive(ref this.remote_endpoint);
                //this.RemoteEndPoint = point;
                //this.NetStream.Read(this.Buffer, 0, available);
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
            if (this.Thread_Reconnect != null)
            {
                this.Thread_Reconnect.Abort();
                this.Thread_Reconnect = null;
            }
            if (this.Auto_TcpReconnect != null)
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
                if (this.BaseClient != null)
                {
                    this.ThreadAbort(); //终止重连线程

                    //关闭流并释放资源
                    //this.NetStream.Close();
                    //this.NetStream.Dispose();
                    this.BaseClient.Close();
                    this.IsStartListening = false;
                    this.ServerIp = null;
                    this.ServerPort = 0;
                    this.IsConnected = false;
                    this.IsConnected_Socket = false;
                    //调用连接断开事件委托
                    if (this.Disconnected != null)
                        this.Disconnected.BeginInvoke(this.Name, new EventArgs(), null, null);

                    this.BaseClient.Close();
                    this.BaseClient = null;
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
            try { this.BaseClient.Send(data, data.Length, this.RemoteEndPoint); }
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
                this.BaseClient.Send(bytes, bytes.Length, this.RemoteEndPoint);
            }
            catch (Exception e)
            {
                this.LastErrorMessage = "字符串发送失败：" + e.Message;
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                throw; //如果不需要抛出异常，注释此行
            }
        }
    }
}
