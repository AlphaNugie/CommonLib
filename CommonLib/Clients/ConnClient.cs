using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Clients;
using CommonLib.Enums;
using CommonLib.Function;

namespace CommonLib.Clients
{
    /// <summary>
    /// 传输层连接的Client，或UDP或TCP
    /// </summary>
    [Obsolete]
    public class ConnClient
    {
        #region 私有成员变量
        /// <summary>
        /// UDP主机名称（IP地址）
        /// </summary>
        private string hostName = ConfigurationManager.AppSettings["Udp_HostName"];

        /// <summary>
        /// UDP主机端口
        /// </summary>
        private int hostPort = int.Parse(ConfigurationManager.AppSettings["Udp_HostPort"]);

        /// <summary>
        /// 与之建立TCP连接的主机IP地址
        /// </summary>
        private string ip = string.Empty;

        /// <summary>
        /// 最新的错误代码
        /// </summary>
        private string lastErrorCode = string.Empty;

        /// <summary>
        /// 最新的错误信息
        /// </summary>
        private string lastErrorMessage = string.Empty;
        #endregion
        #region 公共属性
        /// <summary>
        /// 建立连接的传输层协议类型
        /// </summary>
        public ConnTypes ConnType { get; private set; }

        /// <summary>
        /// UDP主机名称（IP地址）
        /// </summary>
        public string HostName
        {
            get { return hostName; }
            private set { hostName = value; }
        }

        /// <summary>
        /// UDP主机端口
        /// </summary>
        public int HostPort
        {
            get { return hostPort; }
            private set { hostPort = value; }
        }

        /// <summary>
        /// 与之建立连接的IP地址
        /// </summary>
        public string ServerIp
        {
            get { return ip; }
            private set { ip = value; }
        }

        /// <summary>
        /// 与之建立连接的端口号
        /// </summary>
        public int ServerPort { get; private set; }

        /// <summary>
        /// 最新的错误代码
        /// </summary>
        public string LastErrorCode { get; private set; }

        /// <summary>
        /// 最新的错误信息
        /// </summary>
        public string LastErrorMessage
        {
            get { return lastErrorMessage; }
            private set { lastErrorMessage = value; }
        }

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public DerivedTcpClient TcpClient { get; private set; }
        #endregion

        /// <summary>
        /// 通讯连接
        /// </summary>
        /// <param name="serverIp">待连接的主机地址（IP地址）</param>
        /// <param name="portStr">待连接的端口号</param>
        /// <param name="connType">连接类型（UDP, TCP）</param>
        /// <returns>假如连接成功，返回1，否则返回0</returns>
        public int Connect(string serverIp, string portStr, ConnTypes connType)
        {
            string _class = MethodBase.GetCurrentMethod().ReflectedType.FullName; //命名空间+类名
            string _method = string.Format("Int32 {0}", new StackTrace().GetFrame(0).GetMethod().Name); //方法名称
            string connTypeName = Enum.GetName(typeof(ConnTypes), (int)connType); //连接类型的名称

            //假如已连接，返回错误信息
            if (IsConnected)
            {
                LastErrorCode = "001";
                LastErrorMessage = string.Format("已与主机 {0} 在端口 {1} 建立{2}连接，无法再次连接！", ServerIp, ServerPort.ToString(), connTypeName);
                FileClient.WriteFailureInfo(new string[] { LastErrorMessage, string.Format("类名称：{0}，方法名称：{1}", _class, _method) });
                return 0;
            }

            //假如两个参数中有一个参数为空，则生成错误信息并抛出异常
            string param = string.Empty; //参数名称
            if (string.IsNullOrWhiteSpace(serverIp))
            {
                param = "string serverIp";
                LastErrorCode = "002";
                LastErrorMessage = string.Format("建立{0}连接的主机地址不得为空！", connTypeName);
            }
            else if (string.IsNullOrWhiteSpace(portStr))
            {
                param = "string portStr";
                LastErrorCode = "003";
                LastErrorMessage = string.Format("建立{0}连接的端口不得为空！", connTypeName);
            }

            if (!string.IsNullOrWhiteSpace(LastErrorCode))
            {
                FileClient.WriteFailureInfo(new string[] { LastErrorMessage, string.Format("类名称：{0}，方法名称：{1}", _class, _method) });
                throw new ArgumentException(LastErrorMessage, param); //假如不需要抛出异常，注释此行
                //return 0;
            }

            serverIp = serverIp.Equals("localhost", StringComparison.OrdinalIgnoreCase) ? "127.0.0.1" : serverIp; //判断localhost
            int port = int.Parse(portStr); //端口转换为数值类型
            int result = 1;

            //判断连接类型并进行相应的连接，保存主机地址、端口与连接状态；假如报错，获取错误信息
            if (connType == ConnTypes.TCP)
            {
                try
                {
                    TcpClient = new DerivedTcpClient(serverIp, port);
                    ServerIp = TcpClient.ServerIp;
                    ServerPort = TcpClient.ServerPort;
                    IsConnected = TcpClient.IsConnected;
                }
                catch (Exception) { LastErrorMessage = TcpClient.LastErrorMessage; throw; }
            }
            //TODO 编写其它连接方式的连接方法
            else
            {
                //try
                //{
                //    result = BaseUdpClient.Connect(serverIp, port);
                //    ServerIp = BaseUdpClient.ServerIp;
                //    ServerPort = BaseUdpClient.ServerPort;
                //    IsConnected = BaseUdpClient.IsConnected;
                //}
                //catch (Exception) { LastErrorMessage = BaseUdpClient.LastErrorMessage; throw; }
            }

            ConnType = connType;
            return result;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns>假如关闭成功，返回1，否则返回0</returns>
        public int Close()
        {
            string _class = MethodBase.GetCurrentMethod().ReflectedType.FullName; //命名空间+类名
            string _method = string.Format("Int32 {0}", new StackTrace().GetFrame(0).GetMethod().Name); //方法名称

            //假如未连接，返回错误信息
            if (!IsConnected)
            {
                LastErrorMessage = "并未与任何主机建立连接，无需断开连接！";
                FileClient.WriteFailureInfo(new string[] { LastErrorMessage, string.Format("类名称：{0}，方法名称：{1}", _class, _method) });
                return 0;
            }

            LastErrorMessage = string.Empty;
            int result = 1;
            if (ConnType == ConnTypes.TCP)
            {
                try
                {
                    result = TcpClient.Close();
                    ServerIp = TcpClient.ServerIp;
                    ServerPort = TcpClient.ServerPort;
                    IsConnected = TcpClient.IsConnected;
                }
                catch (Exception) { LastErrorMessage = TcpClient.LastErrorMessage; throw; }
            }
            //TODO 编写其它连接方式的断开方法
            else
            {
                //try
                //{
                //    result = BaseUdpClient.Close();
                //    ServerIp = BaseUdpClient.ServerIp;
                //    ServerPort = BaseUdpClient.ServerPort;
                //    IsConnected = BaseUdpClient.IsConnected;
                //}
                //catch (Exception) { LastErrorMessage = BaseUdpClient.LastErrorMessage; throw; }
            }

            ConnType = result == 1 ? ConnTypes.UNCONNECTED : ConnType;
            return result;
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="command">指令字符串</param>
        /// <param name="connType"></param>
        public void SendCommand(string command, ConnTypes connType)
        {
            //TODO 完善其它连接方式的发送方法
            try
            {
                if (connType == ConnTypes.TCP)
                    TcpClient.SendString(command);
                //else if (connType == ConnTypes.UDP)
                //    BaseUdpClient.SendCommand(command);
            }
            catch (Exception)
            {
                if (connType == ConnTypes.TCP)
                    LastErrorMessage = TcpClient.LastErrorMessage;
                //else if (connType == ConnTypes.UDP)
                //    LastErrorMessage = BaseUdpClient.LastErrorMessage;

                throw; //假如不需要抛出异常，注释掉
            }
        }

        /// <summary>
        /// 通过当前的连接方式发送指令
        /// </summary>
        /// <param name="command">指令字符串</param>
        public void SendCommand(string command)
        {
            SendCommand(command, ConnType);
        }

        /// <summary>
        /// 接收信息并转换为字符串
        /// </summary>
        /// <returns>返回接收的信息字符串</returns>
        public string ReceiveInfo(ConnTypes connType)
        {
            //TODO 完善其它连接方式的接收方法
            string info = string.Empty;
            try
            {
                switch (connType)
                {
                    case ConnTypes.TCP:
                        info = TcpClient.ReceiveInfo();
                        break;
                    case ConnTypes.UDP:
                        //info = BaseUdpClient.ReceiveInfo();
                        break;
                }
            }
            catch (Exception)
            {
                switch (connType)
                {
                    case ConnTypes.TCP:
                        LastErrorMessage = TcpClient.LastErrorMessage;
                        break;
                    case ConnTypes.UDP:
                        //LastErrorMessage = BaseUdpClient.LastErrorMessage;
                        break;
                }

                info = string.Empty;
                throw;
            }

            return info;
        }

        /// <summary>
        /// 接收当前连接方式传输的信息并转换为字符串
        /// </summary>
        /// <returns>返回接收的信息字符串</returns>
        public string ReceiveInfo()
        {
            return ReceiveInfo(ConnType);
        }
    }
}
