using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Function;

namespace CommonLib.Clients
{
    /// <summary>
    /// UDP连接客户端
    /// </summary>
    public class BaseUdpClient
    {
        #region 私有成员变量
        /// <summary>
        /// UDP主机名称（IP地址）
        /// </summary>
        private static string hostName = (hostName = ConfigurationManager.AppSettings["Udp_HostName"]) == "localhost" ? "127.0.0.1" : hostName;

        /// <summary>
        /// UDP主机端口
        /// </summary>
        private static int hostPort = int.Parse(ConfigurationManager.AppSettings["Udp_HostPort"]);

        /// <summary>
        /// 最新的错误信息
        /// </summary>
        private static string lastErrorMessage = string.Empty;
        #endregion
        #region 成员属性
        /// <summary>
        /// UDP主机名称（IP地址）
        /// </summary>
        public static string HostName
        {
            get { return hostName; }
            private set { hostName = value; }
        }

        /// <summary>
        /// UDP主机端口
        /// </summary>
        public static int HostPort
        {
            get { return hostPort; }
            private set { hostPort = value; }
        }

        /// <summary>
        /// UdpClient对象
        /// </summary>
        private static UdpClient Client { get; set; }

        /// <summary>
        /// 与之建立UDP连接的主机IP地址
        /// </summary>
        public static string ServerIp { get; private set; }

        /// <summary>
        /// 建立UDP连接的端口
        /// </summary>
        public static int ServerPort { get; private set; }

        /// <summary>
        /// 与之连接的UDP主机的网络端点
        /// </summary>
        private static IPEndPoint ServerEndPoint { get; set; }

        /// <summary>
        /// 最新的错误信息
        /// </summary>
        public static string LastErrorMessage
        {
            get { return lastErrorMessage; }
            set { lastErrorMessage = value; }
        }

        /// <summary>
        /// 是否已连接
        /// </summary>
        public static bool IsConnected { get; private set; }
        #endregion

        /// <summary>
        /// 利用特定的端口与UDP服务端连接
        /// </summary>
        /// <param name="server">UDP服务端IP</param>
        /// <param name="portStr">端口号</param>
        /// <returns>假如建立连接成功，返回1，否则返回0</returns>
        public static int Connect(string server, int port)
        {
            //尝试建立连接
            int result = 1;
            try
            {
                //假如UDP主机客户端对象为空，则初始化
                //if (Client == null)
                //    Client = new UdpClient(new IPEndPoint(IPAddress.Parse(HostName), HostPort));

                Client = new UdpClient(new IPEndPoint(IPAddress.Parse(HostName), HostPort));
                ServerEndPoint = new IPEndPoint(IPAddress.Parse(server), port);
                Client.Connect(ServerEndPoint);
                ServerIp = server; //待连接的UDP主机IP地址
                ServerPort = port; //待连接的UDP主机端口
                IsConnected = true;
                //IsConnected = true;
            }
            catch (Exception e)
            {
                LastErrorMessage = string.Format("无法建立UDP连接：{0}", e.Message);
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                ////IP地址，端口重置
                //ServerIp = string.Empty;
                //ServerPort = 0;
                //IsConnected = false;
                result = 0;
                throw; //假如不需要抛出异常，注释此行
            }

            return result;
        }

        /// <summary>
        /// 关闭UDP连接
        /// </summary>
        /// <returns>假如关闭成功，返回1，否则返回0</returns>
        public static int Close()
        {
            //string _class = MethodBase.GetCurrentMethod().ReflectedType.FullName; //命名空间+类名
            //string _method = string.Format("Int32 {0}", new StackTrace().GetFrame(0).GetMethod().Name); //方法名称

            ////假如未连接，返回错误信息
            //if (!IsConnected)
            //{
            //    LastErrorMessage = "并未与任何主机建立UDP连接，无需断开连接！";
            //    DataClient.WriteFailureInfo(new string[] { LastErrorMessage, string.Format("类名称：{0}，方法名称：{1}", _class, _method) });
            //    return 0;
            //}

            LastErrorMessage = string.Empty;
            int result = 1;
            try
            {
                if (Client != null && IsConnected)
                {
                    //关闭流并释放资源
                    Client.Close(); // Close Client
                    ServerIp = string.Empty;
                    ServerPort = 0;
                    ServerEndPoint = null;
                    IsConnected = false;
                }
            }
            catch (Exception e)
            {
                LastErrorMessage = string.Format("关闭UDP连接失败：{0}", e.Message);
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                result = 0;
                throw; //假如不需要抛出异常，注释此行
            }

            return result;
        }

        /// <summary>
        /// 向设备发送指令，接收设备返回的指令或错误信息
        /// </summary>
        /// <param name="command">指令</param>
        /// <returns>返回发送的字节数</returns>
        public static void SendCommand(string command)
        {
            try
            {
                //转换为byte类型数组并发送指令
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Base.STX + command + Base.ETX); //将字符串转换为byte数组
                Client.Send(bytes, bytes.Length);
            }
            catch (Exception e)
            {
                LastErrorMessage = "指令发送失败：" + e.Message;
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                throw; //如果不需要抛出异常，注释此行
            }
        }

        /// <summary>
        /// 从UDP主机接收信息并转换为字符串
        /// </summary>
        /// <returns></returns>
        public static string ReceiveInfo()
        {
            try
            {
                //接收信息并转换为字符串
                IPEndPoint endPoint = ServerEndPoint;
                byte[] buffer = Client.Receive(ref endPoint); //接收UDP主机发送的信息
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
            catch (Exception e)
            {
                LastErrorMessage = "信息接收失败：" + e.Message;
                FileClient.WriteExceptionInfo(e, LastErrorMessage, false);
                throw; //如果不需要抛出异常，注释此行
            }
        }
    }
}
