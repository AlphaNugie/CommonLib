using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Events;

namespace CommonLib.Clients
{
    /// <summary>
    /// TCP服务端（Tcp Server）
    /// </summary>
    public class DerivedTcpListener
    {
        #region 事件
        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event DataReceivedEventHandler DataReceived;
        #endregion

        #region 私有成员
        /// <summary>
        /// 接收缓冲区大小（字节数）
        /// </summary>
        private int receiveBufferSize = 1024;

        /// <summary>
        /// 客户端列表
        /// </summary>
        private List<TcpClient> clientList = new List<TcpClient>();

        ///// <summary>
        ///// 客户端字典
        ///// </summary>
        //private Dictionary<string, TcpClient> clientDict = new Dictionary<string, TcpClient>();
        #endregion

        #region 属性
        /// <summary>
        /// TcpListener对象
        /// </summary>
        public TcpListener BaseListener { get; private set; }

        /// <summary>
        /// TcpClient用于发送与接收数据的数据流对象
        /// </summary>
        public NetworkStream NetStream { get; private set; }

        /// <summary>
        /// 服务端IP地址
        /// </summary>
        public string ServerIp { get; set; }

        /// <summary>
        /// 服务端端口号
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// 服务端名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 最新错误信息
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
                if (receiveBufferSize > 0)
                {
                    BaseListener.Server.ReceiveBufferSize = receiveBufferSize;
                    Buffer = new byte[receiveBufferSize];
                }
            }
        }

        /// <summary>
        /// 接收缓冲区
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// 客户端列表
        /// </summary>
        public List<TcpClient> ClientList
        {
            get { return clientList; }
            private set { clientList = value; }
        }

        //public Dictionary<string>
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        public DerivedTcpListener() { }

        /// <summary>
        /// TcpListener构造器
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="port">端口号</param>
        public DerivedTcpListener(string ipAddress, int port)
        {
            ServerIp = ipAddress;
            ServerPort = port;
            Start();
        }
        
        /// <summary>
        /// 设置Tcp服务端的名称，格式：本地IP:端口
        /// </summary>
        public void SetName()
        {
            Name = ServerIp + ":" + ServerPort;
            //try
            //{
            //    IPEndPoint local = (IPEndPoint)BaseListener.Server.LocalEndPoint;
            //    Name = local.ToString();
            //    Name = ServerIp + ":" + ServerPort;
            //}
            //catch { }
        }

        /// <summary>
        /// 启动
        /// </summary>
        public bool Start()
        {
            Stop();
            try
            {
                BaseListener = new TcpListener(new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort));
                ReceiveBufferSize = 2048;
                BaseListener.Start();
                SetName();
                BaseListener.BeginAcceptTcpClient(new AsyncCallback(TcpClientAcceptCallBack), BaseListener); //开始异步接受TCP客户端的连接
            }
            catch (Exception e)
            {
                LastErrorMessage = e.Message;
                throw;
            }
            return true;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public bool Stop()
        {
            try
            {
                ClientList?.ForEach(client => client.Close());
                BaseListener?.Stop();
            }
            catch (Exception e)
            {
                LastErrorMessage = "服务端停止失败：" + e.Message;
                //return false;
                throw;
            }
            return true;
        }

        public void SendData()
        {

        }

        /// <summary>
        /// 接受TCP客户端连接的回调函数
        /// </summary>
        /// <param name="asyncResult"></param>
        private void TcpClientAcceptCallBack(IAsyncResult asyncResult)
        {
            TcpListener listener = (TcpListener)asyncResult.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(asyncResult);
            ClientList.Add(client);

            try
            {
                NetworkStream stream = client.GetStream();
                stream.BeginRead(Buffer, 0, ReceiveBufferSize, new AsyncCallback(SocketCallBack), client);
            }
            catch (Exception ex)
            {
                //LastErrorMessage = string.Format("从TCP客户端{0}异步获取数据时出错: {1}", client.ToString());
                FileClient.WriteExceptionInfo(ex, string.Format("从Tcp客户端异步获取数据时出错：IP{0}，端口{1}", ServerIp, ServerPort), true);
            }
        }

        /// <summary>
        /// 从Tcp服务端获取信息的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void SocketCallBack(IAsyncResult ar)
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
                    DataReceived?.BeginInvoke(client.Name, new DataReceivedEventArgs(recdata), null, null); //异步输出数据
                    ns.BeginRead(client.Buffer, 0, client.Buffer.Length, new AsyncCallback(SocketCallBack), client);
                }
            }
            catch (Exception ex)
            {
                FileClient.WriteExceptionInfo(ex, string.Format("从TcpServer获取数据的过程中出错：IP地址{0}，端口{1}", ServerIp, ServerPort), true);
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~DerivedTcpListener()
        {
            BaseListener.Stop();
        }
    }
}
