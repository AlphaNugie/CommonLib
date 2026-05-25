#if DA
using OPCAutomation;
using OpcLibrary.Model;
#elif UA
using Opc.Ua;
using OpcUaHelper;
using OpcLibrary.Ua.Model;
#endif
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

#if DA
namespace OpcLibrary
#elif UA
namespace OpcLibrary.Ua
#endif
{
    /// <summary>
    /// OPC功能包装类
    /// TODO 集成D:\OPC\DLL\OPC_Models.dll
    /// </summary>
    public class OpcUtilHelper
    {
        #region 私有变量
#if DA
        private bool is_groups_active = true, is_group_active = true, is_group_subscribed = true; //OPC组集合活动状态，OPC组激活状态、订阅状态
        private float groups_deadband = 0; //OPC组集合不敏感区
        private int group_update_rate = 250; //OPC组更新速度
#endif
        private const string DEFAULT_GROUP_NAME = "OPCDOTNETGROUP"; //默认OPC组的名称
        #endregion

        #region 属性
#if DA
        /// <summary>
        /// 是否重连
        /// </summary>
        public bool ReconnEnabled { get; set; }
#elif UA
        /// <summary>
        /// 是否重连
        /// <para/>对于UA，判断 <see cref="OpcUaClient.ReconnectPeriod"/> 是否大于0，设为true时将 <see cref="OpcUaClient.ReconnectPeriod"/> 设为5
        /// </summary>
        public bool ReconnEnabled
        {
            get { return OpcUaClient != null && OpcUaClient.ReconnectPeriod > 0; }
            set
            {
                if (OpcUaClient != null)
                    OpcUaClient.ReconnectPeriod = 5;
            }
        }
#endif

#if DA
        /// <summary>
        /// OPC重连线程
        /// </summary>
        public Thread Thread_Reconn { get; private set; }

        /// <summary>
        /// OPC服务
        /// </summary>
        public OPCServer OpcServer { get; set; }
#elif UA
        /// <summary>
        /// OPC UA客户端
        /// </summary>
        public OpcUaClient OpcUaClient { get; set; }
#endif

        /// <summary>
        /// OPC服务IP
        /// <para/>对于UA，将成为服务地址“opc.tcp://[OpcServerIp]:[OpcServerPort][/[OpcServerName]]”的一部分
        /// </summary>
        public string OpcServerIp { get; set; }

#if UA
        /// <summary>
        /// OPC SERVER 端口
        /// <para/>仅对UA有效，将成为服务地址“opc.tcp://[OpcServerIp]:[OpcServerPort][/[OpcServerName]]”的一部分
        /// <para/>KEPServerV6默认端口号为49320
        /// </summary>
        public int OpcServerPort { get; set; }

        /// <summary>
        /// OPC UA 服务的完整名称，形式为“opc.tcp://[OpcServerIp]:[OpcServerPort][/[OpcServerName]]”
        /// </summary>
        public string OpcServerUrl
        {
            get
            {
                return GetOpcServerUrl(OpcServerIp, OpcServerPort, OpcServerName);
                //return string.Format("opc.tcp://{0}:{1}{2}",
                //    OpcServerIp,
                //    OpcServerPort,
                //    string.IsNullOrWhiteSpace(OpcServerName) ? string.Empty : ("/" + OpcServerName));
            }
        }
#endif

        /// <summary>
        /// OPC服务名称
        /// <para/>对于DA为服务名称；对于UA，假如不为空，将在URL最后添加“/[OpcServerName]”
        /// </summary>
        public string OpcServerName { get; set; }

        /// <summary>
        /// 默认OPC组信息
        /// </summary>
        public OpcGroupInfo DefaultGroupInfo { get; set; }

        /// <summary>
        /// OPC组信息List，包含OPC组名称，OPC项信息等信息，OPCServer连接前设置此属性可在连接时（ConnectRemoteServer方法）直接添加组
        /// </summary>
        public List<OpcGroupInfo> ListGroupInfo { get; set; }

        ///// <summary>
        ///// OPC读取速率（毫秒）
        ///// </summary>
        //public int OpcUpdateRate { get; set; }

        /// <summary>
        /// 标签名称_默认
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// OPC服务连接状态
        /// </summary>
        public bool OpcConnected { get; set; }

#if DA
        /// <summary>
        /// 客户端句柄_默认
        /// </summary>
        public int ItemHandleClient { get; set; }

        /// <summary>
        /// 服务端句柄_默认
        /// </summary>
        public int ItemHandleServer { get; set; }
#endif

#region OPC组集合与组的状态
#if DA
        /// <summary>
        /// OPC组集合活动状态
        /// </summary>
        public bool IsGroupsActive
        {
            get { return is_groups_active; }
            set
            {
                is_groups_active = value;
                OpcServer.OPCGroups.DefaultGroupIsActive = is_groups_active;
            }
        }

        /// <summary>
        /// OPC组集合不敏感区
        /// </summary>
        public float GroupsDeadband
        {
            get { return groups_deadband; }
            set
            {
                groups_deadband = value;
                OpcServer.OPCGroups.DefaultGroupDeadband = groups_deadband;
            }
        }

        /// <summary>
        /// OPC组激活状态
        /// </summary>
        public bool IsGroupActive
        {
            get { return is_group_active; }
            set
            {
                is_group_active = value;
                ListGroupInfo.ForEach(groupInfo => groupInfo.OpcGroup.IsActive = is_group_active);
            }
        }

        /// <summary>
        /// OPC组订阅状态
        /// </summary>
        public bool IsGroupSubscribed
        {
            get { return is_group_subscribed; }
            set
            {
                is_group_subscribed = value;
                ListGroupInfo.ForEach(groupInfo => groupInfo.OpcGroup.IsSubscribed = is_group_subscribed);
            }
        }

        /// <summary>
        /// OPC组更新速度
        /// </summary>
        public int GroupUpdateRate
        {
            get { return group_update_rate; }
            set
            {
                group_update_rate = value;
                ListGroupInfo.ForEach(groupInfo => groupInfo.OpcGroup.UpdateRate = group_update_rate);
            }
        }
#endif
#endregion

#region OPC服务信息
#if DA
        /// <summary>
        /// OPC服务名称
        /// </summary>
        public string ServerName { get { return OpcServer == null ? string.Empty : OpcServer.ServerName; } }

        /// <summary>
        /// OPC服务启动时间
        /// </summary>
        public DateTime? ServerStartTime { get { return OpcServer?.StartTime; } }

        /// <summary>
        /// OPC服务启动时间（字符串）
        /// </summary>
        public string ServerStartTimeStr { get { return ServerStartTime == null ? string.Empty : string.Format("启动时间:{0}", ServerStartTime.ToString()); } }
        //public string ServerStartTime { get; set; }

        /// <summary>
        /// OPC服务版本
        /// </summary>
        public string ServerVersionStr { get { return OpcServer == null ? string.Empty : string.Format("版本:{0}.{1}.{2}", OpcServer.MajorVersion, OpcServer.MinorVersion, OpcServer.BuildNumber); } }
        //public string ServerVersion { get; set; }

        /// <summary>
        /// OPC服务状态
        /// </summary>
        public OPCServerState ServerState { get { return OpcServer == null ? OPCServerState.OPCDisconnected : (OPCServerState)OpcServer.ServerState; } }

        /// <summary>
        /// OPC服务状态（字符串）
        /// </summary>
        public string ServerStateStr { get { return ServerState == OPCServerState.OPCRunning ? string.Format("已连接:{0}", ServerName) : string.Format("状态：{0}", ServerState.ToString()); } }
#endif
#endregion
#endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="updateRate">OPC读取速率（毫秒）</param>
        /// <param name="reconn_enabled">是否重连</param>
        [Obsolete("已过时，请使用OpcUtilHelper(bool reconn_enabled)构造器")]
        public OpcUtilHelper(int updateRate, bool reconn_enabled)
        {
            ReconnEnabled = reconn_enabled;
            //OpcUpdateRate = updateRate;
            ListGroupInfo = new List<OpcGroupInfo>();
            ItemId = string.Empty;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reconn_enabled">是否重连</param>
        public OpcUtilHelper(bool reconn_enabled)
        {
            ReconnEnabled = reconn_enabled;
            //OpcUpdateRate = updateRate;
            ListGroupInfo = new List<OpcGroupInfo>();
            ItemId = string.Empty;
        }

        /// <summary>
        /// 构造器，OPC读取速率1000毫秒，默认不重连
        /// </summary>
        public OpcUtilHelper() : this(/*1000, */false) { }

        #region 功能
        /// <summary>
        /// 获取OPC UA 服务的完整名称，形式为“opc.tcp://[OpcServerIp]:[OpcServerPort][/[OpcServerName]]”
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public static string GetOpcServerUrl(string ipAddress, int port, string serverName)
        {
            return string.Format("opc.tcp://{0}:{1}{2}",
                ipAddress,
                port,
                string.IsNullOrWhiteSpace(serverName) ? string.Empty : ("/" + serverName));
        }

        /// <summary>
        /// 更新OPC服务信息，包括启动时间、版本与状态
        /// </summary>
        public void UpdateServerInfo()
        {
            //TODO 添加OPC UA的服务属性刷新方式

            //ServerStartTime = OpcServer == null ? string.Empty : string.Format("启动时间:{0}", OpcServer.StartTime.ToString());
            //ServerVersion = OpcServer == null ? string.Empty : string.Format("版本:{0}.{1}.{2}", OpcServer.MajorVersion, OpcServer.MinorVersion, OpcServer.BuildNumber);
            //ServerState = OpcServer == null ? string.Empty : (OpcServer.ServerState == (int)OPCServerState.OPCRunning ? string.Format("已连接:{0}", OpcServer.ServerName) : string.Format("状态：{0}", OpcServer.ServerState.ToString()));
        }

        #region DA连接操作
#if DA
        /// <summary>
        /// OPC服务枚举
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public string[] ServerEnum(string ipAddress, out string message)
        {
            Array array = null;
            message = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    message = "IP地址为空";
                    return null;
                }

                if (OpcServer == null)
                    OpcServer = new OPCServer();
                array = (Array)(object)OpcServer.GetOPCServers(ipAddress);
            }
            //假如获取OPC Server过程中引发COMException，即代表无法连接此IP的OPC Server
            catch (Exception ex) { message = "无法连接此IP地址的OPC Server！" + ex.Message; }
            return array?.Cast<string>().ToArray();
        }

        /// <summary>
        /// 管理重连线程
        /// </summary>
        private void ManageReconnectionThread()
        {
            //假如线程为空，初始化重连线程；假如线程不为空，则线程已经开始运行
            if (Thread_Reconn == null)
            {
                Thread_Reconn = new Thread(new ThreadStart(Reconn_Recursive)) { IsBackground = true };
                Thread_Reconn.Start();
            }
        }

        /// <summary>
        /// 连接OPC服务器，连接成功后刷新OPC服务信息并创建默认组，同时根据ListGroupInfo属性（OPC组信息List）创建OPC组
        /// </summary>
        /// <param name="remoteServerIP">OPCServerIP</param>
        /// <param name="remoteServerName">OPCServer名称</param>
        /// <param name="message">返回的错误消息</param>
        /// <returns></returns>
        public bool ConnectRemoteServer(string remoteServerIP, string remoteServerName, out string message)
        {
            message = string.Empty;
            try
            {
                OpcServer = new OPCServer();
                OpcServer.Connect(remoteServerName, remoteServerIP);
                OpcServerName = remoteServerName;
                OpcServerIp = remoteServerIP;
                OpcConnected = true;
                UpdateServerInfo(); //刷新OPC服务信息
                SetGroupsProperty(IsGroupsActive, GroupsDeadband); //设置组集合属性
                CreateDefaultGroup(out message); //创建默认OPC组
                //根据对象自身具有的OPC组信息List创建OPC组，假如连接前未在ListGroupInfo属性中设置OPC组信息，则在连接后用CreateGroups方法创建OPC组
                CreateGroups(ListGroupInfo, out message);
                ManageReconnectionThread(); //管理重连线程
            }
            catch (Exception ex)
            {
                message = "连接远程服务器出现错误：" + ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 实际连接操作封装
        /// </summary>
        /// <param name="remoteServerIP">OPCServerIP</param>
        /// <param name="remoteServerName">OPCServer名称</param>
        /// <param name="ct">取消令牌</param>
        private OpInfoSource ConnectInternal(string remoteServerIP, string remoteServerName, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            string errorMsg = string.Empty;
            var source = new OpInfoSource() { Message = "未进行任何操作" };

            try
            {
                OpcServer = new OPCServer();
                // 注册取消令牌，用于取消时的操作
                ct.Register(() =>
                {
                    source.Success = false;
                    source.OperationCancelled = true;
                    source.Message = "连接操作被取消";
                    // 资源清理
                    DisposeOpcResources();
                });

                // 同步连接操作（依赖OPC库的内部超时机制）
                OpcServer.Connect(remoteServerName, remoteServerIP);
                ct.ThrowIfCancellationRequested();

                // 后续初始化逻辑
                OpcServerName = remoteServerName;
                OpcServerIp = remoteServerIP;
                OpcConnected = true;
                UpdateServerInfo();
                SetGroupsProperty(IsGroupsActive, GroupsDeadband);
                CreateDefaultGroup(out errorMsg);
                CreateGroups(ListGroupInfo, out errorMsg);
                ////假如线程为空，初始化重连线程；假如线程不为空，则线程已经开始运行
                //if (Thread_Reconn == null)
                //{
                //    Thread_Reconn = new Thread(new ThreadStart(Reconn_Recursive)) { IsBackground = true };
                //    Thread_Reconn.Start();
                //}
                ManageReconnectionThread(); //管理重连线程

                source.Success = string.IsNullOrEmpty(errorMsg);
                source.Message = errorMsg;
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                source.Success = false;
                source.Message = $"连接远程服务器出现错误：{ex.Message}";
            }
            return source;
        }

        /// <summary>
        /// 连接OPC服务器，连接成功后刷新OPC服务信息并创建默认组，同时根据ListGroupInfo属性（OPC组信息List）创建OPC组
        /// </summary>
        /// <param name="remoteServerIP">OPCServerIP</param>
        /// <param name="remoteServerName">OPCServer名称</param>
        /// <param name="timeoutMilliseconds">超时时间（毫秒）</param>
        /// <returns></returns>
        ///// <param name="message">返回的错误消息</param>
        public async Task<OpInfoSource> ConnectRemoteServerAsync(string remoteServerIP, string remoteServerName, int timeoutMilliseconds = 5000)
        {
            var faultedInfoSource = new OpInfoSource();
            using (var cts = new CancellationTokenSource(timeoutMilliseconds))
            {
                try
                {
                    //using var cts = new CancellationTokenSource(timeoutMilliseconds);
                    return await System.Threading.Tasks.Task.Run(() => ConnectInternal(remoteServerIP, remoteServerName, cts.Token), cts.Token);
                }
                catch (OperationCanceledException)
                {
                    DisposeOpcResources();
                    faultedInfoSource.OperationCancelled = true;
                    faultedInfoSource.Message = $"连接超时（{timeoutMilliseconds}ms）";
                    return faultedInfoSource;
                }
                catch (Exception ex)
                {
                    faultedInfoSource.Message = $"连接异常：{ex.Message}";
                }
                return faultedInfoSource;
            }
        }
#endif
        #endregion

        #region UA连接操作
#if UA
        /// <summary>
        /// 连接OPC服务器，连接成功后刷新OPC服务信息并创建默认组，同时根据ListGroupInfo属性（OPC组信息List）创建OPC组
        /// </summary>
        /// <param name="remoteServerIP">OPCServerIP</param>
        /// <param name="remoteServerPort">OPCServer端口</param>
        /// <param name="remoteServerName">OPCServer名称</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="message">返回的错误消息</param>
        /// <returns></returns>
        public async Task<OpInfoSource> ConnectRemoteServerAsync(string remoteServerIP, int remoteServerPort, string remoteServerName, string userName, string password/*, out string message*/)
        {
            OpInfoSource opInfoSource = new OpInfoSource() { Message = string.Empty };
            //message = string.Empty;
            string url = GetOpcServerUrl(remoteServerIP, remoteServerPort, remoteServerName);
            try
            {
                //OpcUaClient = new OpcUaClient()
                //{
                //    UserIdentity = string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) ? new UserIdentity(new AnonymousIdentityToken()) : new UserIdentity(userName, password)
                //};
                OpcUaClient = new OpcUaClient();
                if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
                    OpcUaClient.UserIdentity = new UserIdentity(userName, password);
                OpcUaClient.OpcStatusChange += new EventHandler<OpcUaStatusEventArgs>(Client_OpcStatusChange);
                await OpcUaClient.ConnectServer(url);
            }
            catch (Exception ex)
            {
                opInfoSource.Message = string.Format("连接UA服务{0}时出现错误：{1}", url, ex.Message);
                opInfoSource.Success = false;
                ClientUtils.HandleException(string.Format("连接UA服务{0}时出现错误", url), ex);
                DisposeOpcResources();
                //return false;
                return opInfoSource;
            }
            try
            {
                CreateDefaultGroup(out string message); //创建默认OPC组
                //根据对象自身具有的OPC组信息List创建OPC组，假如连接前未在ListGroupInfo属性中设置OPC组信息，则在连接后用CreateGroups方法创建OPC组
                CreateGroups(ListGroupInfo, out message);
                opInfoSource.Message = message;
            }
            catch (Exception ex)
            {
                opInfoSource.Message = "创建组时出现错误：" + ex.Message;
                opInfoSource.Success = false;
                //return false;
                return opInfoSource;
            }
            opInfoSource.Success = true;
            //return true;
            return opInfoSource;
        }

        /// <summary>
        /// OPC UA 服务连接状态改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_OpcStatusChange(object sender, OpcUaStatusEventArgs e)
        {
            //断连之后会接收到Disconnected开头的信息
            //连接上之后则会每隔几秒就收到Connected开头的信息
            if (!e.Error)
                OpcConnected = e.Text.StartsWith("Connected");
        }
#endif
        #endregion

        /// <summary>
        /// 资源清理
        /// </summary>
        private void DisposeOpcResources()
        {
            try
            {
#if DA
                if (OpcServer != null)
                {
                    OpcServer.Disconnect();
                    OpcServer = null;
                    OpcConnected = false;
                }
#elif UA
                if (OpcUaClient != null)
                {
                    OpcUaClient.Disconnect();
                    OpcUaClient = null;
                    OpcConnected = false;
                    
                }
#endif
            }
            catch { /* 确保资源释放不会抛出异常 */ }
        }

        /// <summary>
        /// 与OPC服务断开
        /// </summary>
        public void DisconnectRemoteServer()
        {
#if DA
            if (Thread_Reconn != null)
            {
                Thread_Reconn.Abort();
                Thread_Reconn = null;
            }
#endif
            if (!OpcConnected)
                return;

#if DA
            if (OpcServer != null)
            {
                OpcServer.OPCGroups.RemoveAll();
                OpcServer.Disconnect();
                OpcServer = null;
                ListGroupInfo.ForEach(g => g.Dispose());
                ListGroupInfo.Clear();
            }
            OpcConnected = false;
#elif UA
            if (OpcUaClient != null)
            {
                OpcUaClient.Disconnect();
                OpcUaClient = null;
                ListGroupInfo.ForEach(g => g.Dispose());
                ListGroupInfo.Clear();
            }
            //OpcConnected 状态统一在UA的状态改变事件中处理
#endif
        }

        #region 重连功能
#if DA
        /// <summary>
        /// 循环连接OPC，用于重连OPC线程
        /// </summary>
        private void Reconn_Recursive()
        {
            //string info;
            while (true)
            {
                if (!ReconnEnabled)
                    break;
                Thread.Sleep(5000);
                //Reconn(out info);
                Reconn(out _);
            }
        }

        /// <summary>
        /// 重新连接OPC，返回连接信息
        /// </summary>
        public void Reconn(out string info)
        {
            info = string.Empty;
            try
            {
#if DA

                if (ServerState != OPCServerState.OPCRunning)
#elif UA
                //TODO 进行UA的断连判断
#endif
                ReconnDetail(out info);
            }
            //假如捕捉到COMException
            catch (COMException)
            {
                try { ReconnDetail(out info); }
                catch { }
            }
            catch (Exception e) { info = string.Format("准备重连OPC服务{0} (IP {1}) 时出现异常: {2}", OpcServerName, OpcServerIp, e.Message); }
        }

        /// <summary>
        /// 重新连接OPC
        /// </summary>
        /// <param name="info">返回信息</param>
        public void ReconnDetail(out string info)
        {
            info = string.Empty;
            try
            {
#if DA
                OpcServer = new OPCServer();
#elif UA
                //TODO 进行UA的重连初始化工作
#endif
                info = string.Format("OPC服务{0} (IP {1}) 连接失败，尝试重连", OpcServerName, OpcServerIp);
                ConnectRemoteServer(OpcServerIp, OpcServerName, out info);
                //OpcServer.Connect(OpcServerName, OpcServerIp);
                info = string.Format("OPC服务{0} (IP {1}) 重连成功", OpcServerName, OpcServerIp);
                //OpcServer.OPCGroups.RemoveAll();
                //if (CreateDefaultGroup(out info))
                //    info = string.Format("OPC服务{0} (IP {1}) 的OPC组创建成功", OpcServerName, OpcServerIp);
            }
            catch (Exception e) { info = string.Format("OPC服务{0} (IP {1}) 重连失败: {2}", OpcServerName, OpcServerIp, e.Message); }
        }
#endif
#endregion

        /// <summary>
        /// 创建默认OPC组
        /// </summary>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool CreateDefaultGroup(out string message)
        {
            message = string.Empty;
            try
            {
#if DA
                try { OpcServer.OPCGroups.Remove(DEFAULT_GROUP_NAME); } catch (Exception) { } //试着移除已存在组
                DefaultGroupInfo = new OpcGroupInfo(OpcServer.OPCGroups, DEFAULT_GROUP_NAME);
                DefaultGroupInfo.SetGroupProperty(GroupUpdateRate, IsGroupActive, IsGroupSubscribed);
#elif UA
                DefaultGroupInfo = new OpcGroupInfo(OpcUaClient, DEFAULT_GROUP_NAME);
#endif
            }
            catch (Exception ex)
            {
                message = "创建组出现错误：" + ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据若干个OPC组信息创建OPC组
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool CreateGroups(IEnumerable<OpcGroupInfo> groups, out string message)
        {
            message = string.Empty;
            if (groups == null || groups.Count() == 0)
            {
                //message = "未提供任何OPC组信息，无法创建OPC组";
                return false;
            }
            try
            {
                List<OpcGroupInfo> groupList = groups.ToList(); //转换为新List对象，防止枚举改变对象时出现未知影响
                foreach (var groupInfo in groupList)
                {
                    if (groupInfo == null)
                        continue;
                    string name = groupInfo.GroupName; //OPC组名称
                    List<OpcItemInfo> itemInfos = groupInfo.ListItemInfo; //OPC项信息集合
#if DA
                    try { OpcServer.OPCGroups.Remove(name); } catch (Exception) { } //试着移除已存在组
#endif

                    #region 添加组方法
#if DA
                    groupInfo.SetOpcGroup(OpcServer.OPCGroups, name); //重新添加OPC组
                    groupInfo.SetGroupProperty(GroupUpdateRate, IsGroupActive, IsGroupSubscribed);
#elif UA
                    groupInfo.SetOpcUaClient(OpcUaClient/*, name*/); //重新添加OPC组
#endif
                    //假如OPC组信息中已设置OPC项信息，则根据这些OPC项信息添加OPC项，否则创建组之后调用SetItems方法
                    groupInfo.SetItems(itemInfos, out message);
                    if (!ListGroupInfo.Contains(groupInfo))
                        ListGroupInfo.Add(groupInfo);
#endregion
                }
            }
            catch (Exception ex)
            {
                message = "创建组出现错误：" + ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 创建给定名字的OPC组，不添加OPC项
        /// </summary>
        /// <param name="groupNames">待创建的OPC组名称</param>
        /// <param name="message">返回的错误信息</param>
        /// <returns></returns>
        public bool CreateGroups(IEnumerable<string> groupNames, out string message)
        {
            IEnumerable<OpcGroupInfo> groupInfos = groupNames?.Select(n => new OpcGroupInfo(null, n));
            return CreateGroups(groupInfos, out message);
        }

#if DA
        /// <summary>
        /// 设置组集合属性
        /// </summary>
        /// <param name="isGroupsActive">OPC组集合活动状态</param>
        /// <param name="deadband">OPC组集合不敏感区</param>
        public void SetGroupsProperty(bool isGroupsActive, float deadband)
        {
            if (OpcServer.OPCGroups != null)
            {
                OpcServer.OPCGroups.DefaultGroupIsActive = isGroupsActive;
                OpcServer.OPCGroups.DefaultGroupDeadband = deadband;
            }
        }
#endif

        /// <summary>
        /// 设置默认的OPC项，假如已添加，则移除后再重新添加（同一时刻默认标签只有一个）
        /// </summary>
        /// <param name="itemId">标签ID</param>
        /// <param name="clientHandle">标签的客户端句柄</param>
        /// <param name="message">返回的错误信息</param>
        /// <returns></returns>
        ///// <param name="groupType">组的类型，读或写，仅在决定从数据源读值或向数据源写值时起作用（目前仅对UA起作用）</param>
#if DA
        public bool SetItem(string itemId, int clientHandle, out string message/*, GroupType groupType = GroupType.READ*/)
#elif UA
        public bool SetItem(string itemId, out string message/*, GroupType groupType = GroupType.READ*/)
#endif
        {
            try
            {
                if (DefaultGroupInfo == null)
                {
                    message = "未找到默认组";
                    return false;
                }

                //初始化OPC项信息并在默认OPC组中添加
#if DA
                List<OpcItemInfo> list = new List<OpcItemInfo>() { new OpcItemInfo(itemId, clientHandle/*, groupType*/) };
#elif UA
                List<OpcItemInfo> list = new List<OpcItemInfo>() { new OpcItemInfo(itemId/*, groupType*/) };
#endif
                DefaultGroupInfo.SetItems(list, out message);
                if (DefaultGroupInfo.ItemCount > 0)
                {
                    OpcItemInfo item = DefaultGroupInfo.ListItemInfo.Last();
                    //保存默认OPC项的客户端句柄，服务端句柄，标签名称
                    ItemId = item.ItemId;
#if DA
                    ItemHandleClient = item.ClientHandle;
                    ItemHandleServer = item.ServerHandle;
#endif
                }
            }
            catch (Exception ex)
            {
#if DA
                ItemHandleClient = 0;
#endif
                message = "移除或添加标签时发生错误:" + ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 从默认的OPC项读取值
        /// </summary>
        /// <param name="value">待读取的值</param>
        /// <param name="message">返回的错误信息</param>
        public bool ReadItemValue(out string value, out string message)
        {
            value = string.Empty;
            try
            {
                if (DefaultGroupInfo == null)
                {
                    message = "未找到默认组";
                    return false;
                }

                if (!DefaultGroupInfo.ReadValues(out message))
                    return false;
                if (DefaultGroupInfo.ItemCount > 0)
                    value = DefaultGroupInfo.ListItemInfo.Last().Value;
                GC.Collect();
            }
            catch (Exception ex)
            {
#if DA
                message = string.Format("从服务端句柄为{0}、标签ID为{1}的标签读取值失败：{2}", ItemHandleServer, ItemId, ex.Message);
#elif UA
                message = string.Format("从标签ID为{0}的标签读取值失败：{1}", ItemId, ex.Message);
#endif
                return false;
            }
            return true;
        }

        /// <summary>
        /// 向默认的OPC项写入值
        /// </summary>
        /// <param name="value">待写入的值</param>
        /// <param name="message">返回的错误信息</param>
        public bool WriteItemValue(string value, out string message)
        {
            try
            {
                if (DefaultGroupInfo == null)
                {
                    message = "未找到默认组";
                    return false;
                }

                if (DefaultGroupInfo.ItemCount > 0)
                    DefaultGroupInfo.ListItemInfo.Last().Value = value;
                if (!DefaultGroupInfo.WriteValues(out message))
                    return false;
                GC.Collect();
            }
            catch (Exception ex)
            {
#if DA
                message = string.Format("向服务端句柄为{0}的标签写入值{1}失败：{2}", ItemHandleServer, value, ex.Message);
#elif UA
                message = string.Format("向标签ID为{0}的标签写入值{1}失败：{2}", ItemId, value, ex.Message);
#endif
                return false;
            }
            return true;
        }

#if DA
        /// <summary>
        /// 从对应指定客户端句柄的指定OPC项读取值（先根据OPC项ID与客户端句柄添加OPC项，然后再读取）
        /// </summary>
        /// <param name="itemName">标签ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        /// <param name="value">待写入值</param>
        /// <param name="message">返回的错误信息</param>
        public bool ReadOpc(string itemName, int clientHandle, out string value, out string message)
        {
            value = string.Empty;
            return SetItem(itemName, clientHandle, out message/*, GroupType.READ*/) && ReadItemValue(out value, out message);
        }

        /// <summary>
        /// 向对应指定客户端句柄的指定OPC项写入值（先根据OPC项ID与客户端句柄添加OPC项，然后再写入）
        /// </summary>
        /// <param name="itemName">标签ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        /// <param name="value">待写入值</param>
        /// <param name="message">返回的错误信息</param>
        public bool WriteOpc(string itemName, int clientHandle, string value, out string message)
        {
            return SetItem(itemName, clientHandle, out message/*, GroupType.WRITE*/) && WriteItemValue(value, out message);
        }
#endif

#if UA
        /// <summary>
        /// 从对应指定客户端句柄的指定OPC项读取值（先根据OPC项ID与客户端句柄添加OPC项，然后再读取）
        /// </summary>
        /// <param name="itemName">标签ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        /// <param name="value">待写入值</param>
        /// <param name="message">返回的错误信息</param>
        public bool ReadOpc(string itemName, out string value, out string message)
        {
            value = string.Empty;
            return SetItem(itemName, out message/*, GroupType.READ*/) && ReadItemValue(out value, out message);
        }

        /// <summary>
        /// 向对应指定客户端句柄的指定OPC项写入值（先根据OPC项ID与客户端句柄添加OPC项，然后再写入）
        /// </summary>
        /// <param name="itemName">标签ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        /// <param name="value">待写入值</param>
        /// <param name="message">返回的错误信息</param>
        public bool WriteOpc(string itemName, string value, out string message)
        {
            return SetItem(itemName, out message/*, GroupType.WRITE*/) && WriteItemValue(value, out message);
        }
#endif
#endregion
    }
}
