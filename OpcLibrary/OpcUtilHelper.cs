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
            get { return this.is_groups_active; }
            set
            {
                this.is_groups_active = value;
                this.OpcServer.OPCGroups.DefaultGroupIsActive = is_groups_active;
            }
        }

        /// <summary>
        /// OPC组集合不敏感区
        /// </summary>
        public float GroupsDeadband
        {
            get { return this.groups_deadband; }
            set
            {
                this.groups_deadband = value;
                this.OpcServer.OPCGroups.DefaultGroupDeadband = groups_deadband;
            }
        }

        /// <summary>
        /// OPC组激活状态
        /// </summary>
        public bool IsGroupActive
        {
            get { return this.is_group_active; }
            set
            {
                this.is_group_active = value;
                this.ListGroupInfo.ForEach(groupInfo => groupInfo.OpcGroup.IsActive = this.is_group_active);
            }
        }

        /// <summary>
        /// OPC组订阅状态
        /// </summary>
        public bool IsGroupSubscribed
        {
            get { return this.is_group_subscribed; }
            set
            {
                this.is_group_subscribed = value;
                this.ListGroupInfo.ForEach(groupInfo => groupInfo.OpcGroup.IsSubscribed = this.is_group_subscribed);
            }
        }

        /// <summary>
        /// OPC组更新速度
        /// </summary>
        public int GroupUpdateRate
        {
            get { return this.group_update_rate; }
            set
            {
                this.group_update_rate = value;
                this.ListGroupInfo.ForEach(groupInfo => groupInfo.OpcGroup.UpdateRate = this.group_update_rate);
            }
        }

        /// <summary>
        /// OPC服务启动时间
        /// </summary>
        public string ServerStartTime { get; set; }

        /// <summary>
        /// OPC服务版本
        /// </summary>
        public string ServerVersion { get; set; }

        /// <summary>
        /// OPC服务状态
        /// </summary>
        public string ServerState { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="updateRate">OPC读取速率（毫秒）</param>
        /// <param name="reconn_enabled">是否重连</param>
        public OpcUtilHelper(int updateRate, bool reconn_enabled)
        {
            this.ReconnEnabled = reconn_enabled;
            //this.OpcServer = new OPCServer();
            this.OpcUpdateRate = updateRate;
            this.ListGroupInfo = new List<OpcGroupInfo>();
            this.ItemId = string.Empty;
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
            this.ServerStartTime = this.OpcServer == null ? string.Empty : string.Format("启动时间:{0}", this.OpcServer.StartTime.ToString());
            this.ServerVersion = this.OpcServer == null ? string.Empty : string.Format("版本:{0}.{1}.{2}", this.OpcServer.MajorVersion, this.OpcServer.MinorVersion, this.OpcServer.BuildNumber);
            this.ServerState = this.OpcServer == null ? string.Empty : (this.OpcServer.ServerState == (int)OPCServerState.OPCRunning ? string.Format("已连接:{0}", this.OpcServer.ServerName) : string.Format("状态：{0}", this.OpcServer.ServerState.ToString()));
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

                if (this.OpcServer == null)
                    this.OpcServer = new OPCServer();
                array = (Array)(object)this.OpcServer.GetOPCServers(ipAddress);
            }
            //假如获取OPC Server过程中引发COMException，即代表无法连接此IP的OPC Server
            catch (Exception ex) { message = "无法连接此IP地址的OPC Server！" + ex.Message; }
            return array == null ? null : array.Cast<string>().ToArray();
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
                this.OpcServer = new OPCServer();
                this.OpcServer.Connect(remoteServerName, remoteServerIP);
                this.OpcServerName = remoteServerName;
                this.OpcServerIp = remoteServerIP;
                this.OpcConnected = true;
                this.UpdateServerInfo(); //刷新OPC服务信息
                this.SetGroupsProperty(this.IsGroupsActive, this.GroupsDeadband); //设置组集合属性
                this.CreateDefaultGroup(out message); //创建默认OPC组
                //TODO 根据对象自身具有的OPC组信息List创建OPC组，假如连接前未在ListGroupInfo属性中设置OPC组信息，则在连接后用CreateGroups方法创建OPC组
                this.CreateGroups(this.ListGroupInfo, out message);
                //try { if (this.Thread_Reconn != null) this.Thread_Reconn.Abort(); }
                //catch (Exception e) { }
                //假如线程为空，初始化重连线程；假如线程不为空，则线程已经开始运行
                if (this.Thread_Reconn == null)
                {
                    this.Thread_Reconn = new Thread(new ThreadStart(this.Reconn_Recursive)) { IsBackground = true };
                    this.Thread_Reconn.Start();
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
            if (this.Thread_Reconn != null)
            {
                this.Thread_Reconn.Abort();
                this.Thread_Reconn = null;
            }
            if (!this.OpcConnected)
                return;

            if (this.OpcServer != null)
            {
                this.OpcServer.OPCGroups.RemoveAll();
                this.OpcServer.Disconnect();
                this.OpcServer = null;
                this.ListGroupInfo.ForEach(g => g.Dispose());
                this.ListGroupInfo.Clear();
            }

            this.OpcConnected = false;
        }

        /// <summary>
        /// 循环连接OPC，用于重连OPC线程
        /// </summary>
        private void Reconn_Recursive()
        {
            string info;
            while (true)
            {
                if (!this.ReconnEnabled)
                    break;
                Thread.Sleep(5000);
                this.Reconn(out info);
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
                if (this.OpcServer.ServerState != (int)OPCServerState.OPCRunning)
                    this.ReconnDetail(out info);
            }
            //假如捕捉到COMException
            catch (COMException)
            {
                try { this.ReconnDetail(out info); }
                catch { }
            }
            catch (Exception e) { info = string.Format("准备重连OPC服务{0} (IP {1}) 时出现异常: {2}", this.OpcServerName, this.OpcServerIp, e.Message); }
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
                this.OpcServer = new OPCServer();
                info = string.Format("OPC服务{0} (IP {1}) 连接失败，尝试重连", this.OpcServerName, this.OpcServerIp);
                this.ConnectRemoteServer(this.OpcServerIp, this.OpcServerName, out info);
                //this.OpcServer.Connect(this.OpcServerName, this.OpcServerIp);
                info = string.Format("OPC服务{0} (IP {1}) 重连成功", this.OpcServerName, this.OpcServerIp);
                //this.OpcServer.OPCGroups.RemoveAll();
                //if (this.CreateDefaultGroup(out info))
                //    info = string.Format("OPC服务{0} (IP {1}) 的OPC组创建成功", this.OpcServerName, this.OpcServerIp);
            }
            catch (Exception e) { info = string.Format("OPC服务{0} (IP {1}) 重连失败: {2}", this.OpcServerName, this.OpcServerIp, e.Message); }
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
                try { this.OpcServer.OPCGroups.Remove(DEFAULT_GROUP_NAME); } catch (Exception e) { } //试着移除已存在组
                this.DefaultGroupInfo = new OpcGroupInfo(this.OpcServer.OPCGroups, DEFAULT_GROUP_NAME);
                this.DefaultGroupInfo.SetGroupProperty(this.GroupUpdateRate, this.IsGroupActive, this.IsGroupSubscribed);
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
                    try { this.OpcServer.OPCGroups.Remove(name); } catch (Exception) { } //试着移除已存在组
                    //初始化OPC组信息并设置OPC组属性、添加OPC项
                    OpcGroupInfo group = new OpcGroupInfo(this.OpcServer.OPCGroups, name);
                    group.SetGroupProperty(this.GroupUpdateRate, this.IsGroupActive, this.IsGroupSubscribed);
                    //TODO 假如OPC组信息中已设置OPC项信息，则根据这些OPC项信息添加OPC项，否则创建组之后调用SetTags方法
                    group.SetItems(itemInfos, out message);

                    //假如List中已存在，则移除
                    if (this.ListGroupInfo.Contains(groupInfo))
                    {
                        this.ListGroupInfo.Remove(groupInfo);
                        groupInfo.Dispose();
                    }
                    this.ListGroupInfo.Add(group);
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
            IEnumerable<OpcGroupInfo> groupInfos = groupNames == null ? null : groupNames.Select(n => new OpcGroupInfo(null, n));
            return this.CreateGroups(groupInfos, out message);
        }

        /// <summary>
        /// 设置组集合属性
        /// </summary>
        /// <param name="isGroupsActive">OPC组集合活动状态</param>
        /// <param name="deadband">OPC组集合不敏感区</param>
        public void SetGroupsProperty(bool isGroupsActive, float deadband)
        {
            if (this.OpcServer.OPCGroups != null)
            {
                this.OpcServer.OPCGroups.DefaultGroupIsActive = isGroupsActive;
                this.OpcServer.OPCGroups.DefaultGroupDeadband = deadband;
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
                if (this.DefaultGroupInfo == null)
                {
                    message = "未找到默认组";
                    return false;
                }

                //初始化OPC项信息并在默认OPC组中添加
                List<OpcItemInfo> list = new List<OpcItemInfo>() { new OpcItemInfo(itemId, clientHandle) };
                this.DefaultGroupInfo.SetItems(list, out message);
                if (this.DefaultGroupInfo.ItemCount > 0)
                {
                    OpcItemInfo item = this.DefaultGroupInfo.ListItemInfo.Last();
                    //保存默认OPC项的客户端句柄，服务端句柄，标签名称
                    this.ItemHandleClient = item.ClientHandle;
                    this.ItemId = item.ItemId;
                    this.ItemHandleServer = item.ServerHandle;
                }
            }
            catch (Exception ex)
            {
                this.ItemHandleClient = 0;
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
                if (this.DefaultGroupInfo == null)
                {
                    message = "未找到默认组";
                    return false;
                }

                if (!this.DefaultGroupInfo.ReadValues(out message))
                    return false;
                if (this.DefaultGroupInfo.ItemCount > 0)
                    value = this.DefaultGroupInfo.ListItemInfo.Last().Value;
                GC.Collect();
            }
            catch (Exception ex)
            {
                message = string.Format("从服务端句柄为{0}、标签ID为{1}的标签读取值失败：{2}", this.ItemHandleServer, this.ItemId, ex.Message);
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
                if (this.DefaultGroupInfo == null)
                {
                    message = "未找到默认组";
                    return false;
                }

                if (this.DefaultGroupInfo.ItemCount > 0)
                    this.DefaultGroupInfo.ListItemInfo.Last().Value = value;
                if (!this.DefaultGroupInfo.WriteValues(out message))
                    return false;
                GC.Collect();
            }
            catch (Exception ex)
            {
                message = string.Format("向服务端句柄为{0}的标签写入值{1}失败：{2}", this.ItemHandleServer, value, ex.Message);
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
