using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace OpcLibrary
{
    /// <summary>
    /// OPC功能包装类
    /// </summary>
    public class OpcUtilHelper
    {
        #region 私有变量
        private bool is_groups_active = true, is_group_active = true, is_group_subscribed = true; //OPC组集合活动状态，OPC组激活状态、订阅状态
        private float groups_deadband = 0; //OPC组集合不敏感区
        private int group_update_rate = 250; //OPC组更新速度
        private const string DEFAULT_GROUP_NAME = "OPCDOTNETGROUP"; //默认OPC组的名称
        #endregion

        #region 属性
        /// <summary>
        /// OPC重连线程
        /// </summary>
        public Thread Thread_Reconn { get; private set; }

        /// <summary>
        /// 是否重连
        /// </summary>
        public bool ReconnEnabled { get; set; }

        /// <summary>
        /// OPC服务
        /// </summary>
        public OPCServer OpcServer { get; set; }

        /// <summary>
        /// OPC服务IP
        /// </summary>
        public string OpcServerIp { get; set; }

        /// <summary>
        /// OPC服务名称
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

        /// <summary>
        /// OPC读取速率（毫秒）
        /// </summary>
        public int OpcUpdateRate { get; set; }

        /// <summary>
        /// 标签名称_默认
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// OPC服务连接状态
        /// </summary>
        public bool OpcConnected { get; set; }

        /// <summary>
        /// 客户端句柄_默认
        /// </summary>
        public int ItemHandleClient { get; set; }

        /// <summary>
        /// 服务端句柄_默认
        /// </summary>
        public int ItemHandleServer { get; set; }

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

        #region OPC服务信息
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
        #endregion
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="updateRate">OPC读取速率（毫秒）</param>
        /// <param name="reconn_enabled">是否重连</param>
        public OpcUtilHelper(int updateRate, bool reconn_enabled)
        {
            ReconnEnabled = reconn_enabled;
            //OpcServer = new OPCServer();
            OpcUpdateRate = updateRate;
            ListGroupInfo = new List<OpcGroupInfo>();
            ItemId = string.Empty;
        }

        /// <summary>
        /// 构造器，OPC读取速率1000毫秒，默认不重连
        /// </summary>
        public OpcUtilHelper() : this(1000, false) { }

        #region 功能
        /// <summary>
        /// 更新OPC服务信息，包括启动时间、版本与状态
        /// </summary>
        public void UpdateServerInfo()
        {
            //ServerStartTime = OpcServer == null ? string.Empty : string.Format("启动时间:{0}", OpcServer.StartTime.ToString());
            //ServerVersion = OpcServer == null ? string.Empty : string.Format("版本:{0}.{1}.{2}", OpcServer.MajorVersion, OpcServer.MinorVersion, OpcServer.BuildNumber);
            //ServerState = OpcServer == null ? string.Empty : (OpcServer.ServerState == (int)OPCServerState.OPCRunning ? string.Format("已连接:{0}", OpcServer.ServerName) : string.Format("状态：{0}", OpcServer.ServerState.ToString()));
        }

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
                //TODO 根据对象自身具有的OPC组信息List创建OPC组，假如连接前未在ListGroupInfo属性中设置OPC组信息，则在连接后用CreateGroups方法创建OPC组
                CreateGroups(ListGroupInfo, out message);
                //try { if (Thread_Reconn != null) Thread_Reconn.Abort(); }
                //catch (Exception e) { }
                //假如线程为空，初始化重连线程；假如线程不为空，则线程已经开始运行
                if (Thread_Reconn == null)
                {
                    Thread_Reconn = new Thread(new ThreadStart(Reconn_Recursive)) { IsBackground = true };
                    Thread_Reconn.Start();
                }
            }
            catch (Exception ex)
            {
                message = "连接远程服务器出现错误：" + ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 与OPC服务断开
        /// </summary>
        public void DisconnectRemoteServer()
        {
            if (Thread_Reconn != null)
            {
                Thread_Reconn.Abort();
                Thread_Reconn = null;
            }
            if (!OpcConnected)
                return;

            if (OpcServer != null)
            {
                OpcServer.OPCGroups.RemoveAll();
                OpcServer.Disconnect();
                OpcServer = null;
                ListGroupInfo.ForEach(g => g.Dispose());
                ListGroupInfo.Clear();
            }

            OpcConnected = false;
        }

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
                //if (OpcServer.ServerState != (int)OPCServerState.OPCRunning)
                if (ServerState != OPCServerState.OPCRunning)
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
                OpcServer = new OPCServer();
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
                try { OpcServer.OPCGroups.Remove(DEFAULT_GROUP_NAME); } catch (Exception) { } //试着移除已存在组
                DefaultGroupInfo = new OpcGroupInfo(OpcServer.OPCGroups, DEFAULT_GROUP_NAME);
                DefaultGroupInfo.SetGroupProperty(GroupUpdateRate, IsGroupActive, IsGroupSubscribed);
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
                    try { OpcServer.OPCGroups.Remove(name); } catch (Exception) { } //试着移除已存在组
                    #region 新添加组方法
                    groupInfo.SetOpcGroup(OpcServer.OPCGroups, name); //重新添加OPC组
                    groupInfo.SetGroupProperty(GroupUpdateRate, IsGroupActive, IsGroupSubscribed);
                    //TODO 假如OPC组信息中已设置OPC项信息，则根据这些OPC项信息添加OPC项，否则创建组之后调用SetItems方法
                    groupInfo.SetItems(itemInfos, out message);
                    if (!ListGroupInfo.Contains(groupInfo))
                        ListGroupInfo.Add(groupInfo);
                    #endregion
                    #region 旧添加组方法
                    ////初始化OPC组信息并设置OPC组属性、添加OPC项
                    //OpcGroupInfo group = new OpcGroupInfo(OpcServer.OPCGroups, name);
                    //group.SetGroupProperty(GroupUpdateRate, IsGroupActive, IsGroupSubscribed);
                    ////假如OPC组信息中已设置OPC项信息，则根据这些OPC项信息添加OPC项，否则创建组之后调用SetItems方法
                    //group.SetItems(itemInfos, out message);

                    ////假如List中已存在，则移除
                    //if (ListGroupInfo.Contains(groupInfo))
                    //{
                    //    ListGroupInfo.Remove(groupInfo);
                    //    groupInfo.Dispose();
                    //}
                    //ListGroupInfo.Add(group);
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

        /// <summary>
        /// 设置默认的OPC项，假如已添加，则移除后再重新添加（同一时刻默认标签只有一个）
        /// </summary>
        /// <param name="itemId">标签ID</param>
        /// <param name="clientHandle">标签的客户端句柄</param>
        /// <param name="message">返回的错误信息</param>
        /// <returns></returns>
        public bool SetItem(string itemId, int clientHandle, out string message)
        {
            try
            {
                if (DefaultGroupInfo == null)
                {
                    message = "未找到默认组";
                    return false;
                }

                //初始化OPC项信息并在默认OPC组中添加
                List<OpcItemInfo> list = new List<OpcItemInfo>() { new OpcItemInfo(itemId, clientHandle) };
                DefaultGroupInfo.SetItems(list, out message);
                if (DefaultGroupInfo.ItemCount > 0)
                {
                    OpcItemInfo item = DefaultGroupInfo.ListItemInfo.Last();
                    //保存默认OPC项的客户端句柄，服务端句柄，标签名称
                    ItemHandleClient = item.ClientHandle;
                    ItemId = item.ItemId;
                    ItemHandleServer = item.ServerHandle;
                }
            }
            catch (Exception ex)
            {
                ItemHandleClient = 0;
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
                message = string.Format("从服务端句柄为{0}、标签ID为{1}的标签读取值失败：{2}", ItemHandleServer, ItemId, ex.Message);
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
                message = string.Format("向服务端句柄为{0}的标签写入值{1}失败：{2}", ItemHandleServer, value, ex.Message);
                return false;
            }
            return true;
        }

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
            return SetItem(itemName, clientHandle, out message) && ReadItemValue(out value, out message);
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
            return SetItem(itemName, clientHandle, out message) && WriteItemValue(value, out message);
        }
        #endregion
    }
}
