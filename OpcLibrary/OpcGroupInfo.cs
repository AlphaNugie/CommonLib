using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpcLibrary
{
    /// <summary>
    /// OPC组信息实体类，每个OPC组信息实体对应单独的OPCGroup基本信息、标签ID Array、服务端句柄Array、客户端句柄Array以及OPC项信息List（供添加OPC项或添加OPC项后保存信息）
    /// 可根据这些信息为OPC服务添加OPC组（OpcUtilHelper中的CreateGroups方法）
    /// </summary>
    public class OpcGroupInfo : IDisposable
    {
        #region 私有变量
        private Array item_ids, server_handles, client_handles, errors;
        private readonly OpcItemInfo opc_pack_basic = new OpcItemInfo(string.Empty, 0);
        #endregion

        #region 属性
        /// <summary>
        /// OPCGroup对象
        /// </summary>
        public OPCGroup OpcGroup { get; private set; }

        /// <summary>
        /// OPC组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// OPC组所拥有的OPC项数量
        /// </summary>
        public int ItemCount { get { return this.OpcGroup == null ? 0 : this.OpcGroup.OPCItems.Count; } }

        /// <summary>
        /// OPC项ID Array，添加OPC项时变化
        /// </summary>
        public Array ItemIds
        {
            get { return this.item_ids; }
            private set { this.item_ids = value; }
        }

        /// <summary>
        /// OPC项服务端句柄 Array，添加、移除OPC项时变化
        /// </summary>
        public Array ServerHandles
        {
            get { return this.server_handles; }
            private set { this.server_handles = value; }
        }

        /// <summary>
        /// OPC项客户端句柄 Array，添加OPC项时变化
        /// </summary>
        public Array ClientHandles
        {
            get { return this.client_handles; }
            private set { this.client_handles = value; }
        }

        /// <summary>
        /// 错误信息Array，添加、移除OPC项，读取、写入值时变化
        /// </summary>
        public Array Errors
        {
            get { return this.errors; }
            set { this.errors = value; }
        }

        /// <summary>
        /// OPC项信息List，包含OPC项ID、客户端句柄、服务端句柄、值等信息，添加OPC组前设置此属性可在添加OPC组（OpcUtilHelper.CreateGroups方法）时直接添加OPC项
        /// </summary>
        public List<OpcItemInfo> ListItemInfo { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="groups">待创建的OPC组所属于的组集合，为null则无法创建</param>
        /// <param name="name">OPC组名称</param>
        public OpcGroupInfo(OPCGroups groups, string name)
        {
            this.GroupName = name;
            this.ListItemInfo = new List<OpcItemInfo>();
            if (groups == null)
                return;

            this.OpcGroup = groups.Add(name);
        }

        #region 功能
        /// <summary>
        /// 设置OPC组属性
        /// </summary>
        /// <param name="updateRate">OPC组更新速度</param>
        /// <param name="isGroupActive">OPC组激活状态</param>
        /// <param name="isGroupSubscribed">OPC组订阅状态</param>
        public void SetGroupProperty(int updateRate, bool isGroupActive, bool isGroupSubscribed)
        {
            if (this.OpcGroup != null)
            {
                this.OpcGroup.UpdateRate = updateRate;
                this.OpcGroup.IsActive = isGroupActive;
                this.OpcGroup.IsSubscribed = isGroupSubscribed;
            }
        }

        /// <summary>
        /// 获取所有OPC项的值并转换为Array
        /// </summary>
        /// <returns></returns>
        private Array GetValues()
        {
            return this.GetValues(null);
        }

        /// <summary>
        /// 获取在给定服务端句柄中存在对应OPC项的值并转换为Array，假如给定的服务端句柄为空，则给出所有值
        /// </summary>
        /// <param name="serverHandles">给定的服务端句柄数组，假如给定的服务端句柄为空，则给出所有值</param>
        /// <returns></returns>
        private Array GetValues(IEnumerable<int> serverHandles)
        {
            bool flag = serverHandles == null || serverHandles.Count() == 0; //特殊条件
            //假如给定的服务端句柄范围为空则选择所有OPC项的值，否则选择服务端在范围内的OPC项以及第一个OPC项的值
            return this.ListItemInfo == null ? new object[0] : this.ListItemInfo.Select((item, index) => new { item, index }).Where(p => (flag || serverHandles.Contains(p.item.ServerHandle)) || p.index == 0).Select(p => (object)p.item.Value).ToArray();
        }

        /// <summary>
        /// 获取在给定服务端句柄中存在对应OPC项的服务端句柄并转换为Array，假如给定的服务端句柄为空，则给出所有服务端句柄
        /// </summary>
        /// <param name="serverHandles">给定的服务端句柄数组，假如给定的服务端句柄为空，则给出所有服务端句柄</param>
        /// <returns></returns>
        private Array GetServerHandles(IEnumerable<int> serverHandles)
        {
            bool flag = serverHandles == null || serverHandles.Count() == 0; //特殊条件
            //假如给定的服务端句柄范围为空则选择所有OPC项的句柄，否则选择服务端在范围内的OPC项以及第一个OPC项的服务端句柄
            return this.ListItemInfo == null ? new int[0] : this.ListItemInfo.Select((item, index) => new { item, index }).Where(p => (flag || serverHandles.Contains(p.item.ServerHandle)) || p.index == 0).Select(p => p.item.ServerHandle).ToArray();
        }

        /// <summary>
        /// 根据给定的OPC项集合信息添加OPC项
        /// </summary>
        /// <param name="items">给出添加OPC项时所需信息的集合</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool SetItems(IEnumerable<OpcItemInfo> items, out string message)
        {
            message = string.Empty;
            if (items == null || items.Count() == 0)
                return false;

            try
            {
                //假如已存在OPC项，先移除
                if (this.ItemCount > 0)
                    this.OpcGroup.OPCItems.Remove(this.ItemCount, ref this.server_handles, out this.errors);
                List<OpcItemInfo> itemList = items.ToList(); //转换为新List对象，防止枚举改变对象时出现未知影响
                this.ListItemInfo.Clear();
                this.ListItemInfo.Add(this.opc_pack_basic);
                itemList.Insert(0, this.opc_pack_basic);
                this.item_ids = itemList.Select(p => p.ItemId).ToArray();
                this.client_handles = itemList.Select(p => p.ClientHandle).ToArray();
                this.OpcGroup.OPCItems.AddItems(itemList.Count - 1, ref this.item_ids, ref this.client_handles, out this.server_handles, out this.errors);
                //添加OPC项后根据ID找到OPC项信息对象并设置服务端句柄，向OPC项信息List中添加
                if (this.item_ids.Length > 1)
                    for (int i = 1; i < this.item_ids.Length; i++)
                    {
                        OpcItemInfo itemInfo = itemList.Find(p => p.ItemId.Equals(this.item_ids.GetValue(i)));
                        itemInfo.ServerHandle = int.Parse(this.server_handles.GetValue(i).ToString());
                        this.ListItemInfo.Add(itemInfo);
                    }
                //假如添加后的数量对不上，则至少有一个OPC项未添加成功
                if (this.ListItemInfo.Count < itemList.Count)
                    message = "至少有1个OPC项未添加成功";
            }
            catch (Exception ex)
            {
                message = string.Format("OPC组{0}移除或添加标签时发生错误:{1}", this.OpcGroup.Name, ex.Message);
                return false;
            }
            return this.ListItemInfo.Count > 1; //假如至少有1个添加成功，返回true
        }

        /// <summary>
        /// 为OPC组的所有OPC项读取数据
        /// </summary>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool ReadValues(out string message)
        {
            //return this.ReadValues(this.server_handles, out message);
            return this.ReadValues(null, out message);
        }

        /// <summary>
        /// 为OPC组OPC项List内与给定服务端句柄对应的OPC项读取数据
        /// </summary>
        /// <param name="serverHandles">服务端句柄Array</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool ReadValues(Array serverHandles, out string message)
        {
            message = string.Empty;
            object qualities, timeStamps;
            Array values;
            try
            {
                IEnumerable<int> temp = serverHandles == null || serverHandles.Length == 0 ? null : serverHandles.Cast<int>();
                Array handles = this.GetServerHandles(temp);
                int itemCount = handles.Length - 1;
                this.OpcGroup.SyncRead((short)OPCDataSource.OPCDevice, itemCount, ref handles, out values, out this.errors, out qualities, out timeStamps);
                //假如至少读取到1个值，根据服务端句柄找到OPC项信息并更新值
                if (values.Length > 0)
                    for (int i = 1; i <= values.Length; i++)
                    {
                        OpcItemInfo itemInfo = this.ListItemInfo.Find(item => item.ServerHandle.Equals(handles.GetValue(i)));
                        if (itemInfo != null)
                            itemInfo.Value = values.GetValue(i).ToString();
                    }
                if (values.Length < itemCount)
                    message = "至少有1个OPC项的值未找到";
                GC.Collect();
            }
            catch (Exception ex)
            {
                message = string.Format("从名称为{0}的OPC组读取值失败：{1}", this.OpcGroup.Name, ex.Message);
                return false;
            }
            return values.Length > 0;
        }

        /// <summary>
        /// 为OPC组的所有OPC项写入数据
        /// </summary>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool WriteValues(out string message)
        {
            //return this.WriteValues(this.server_handles, out message);
            return this.WriteValues(null, out message);
        }

        /// <summary>
        /// 为OPC组OPC项List内与给定服务端句柄对应的OPC项写入数据
        /// </summary>
        /// <param name="serverHandles">服务端句柄Array</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool WriteValues(Array serverHandles, out string message)
        {
            message = string.Empty;
            try
            {
                IEnumerable<int> temp = serverHandles == null || serverHandles.Length == 0 ? null : serverHandles.Cast<int>();
                Array handles = this.GetServerHandles(temp), values = this.GetValues(temp);
                int itemCount = handles.Length - 1;
                this.OpcGroup.SyncWrite(itemCount, ref handles, ref values, out this.errors);
                GC.Collect();
            }
            catch (Exception ex)
            {
                message = string.Format("向名称为{0}的OPC组写入值失败：{1}", this.OpcGroup.Name, ex.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">为true时释放所有资源，否则仅释放非托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.OpcGroup = null;
                    this.ItemIds = null;
                    this.ClientHandles = null;
                    this.Errors = null;
                    this.ServerHandles = null;
                    this.ListItemInfo.Clear();
                    this.ListItemInfo = null;
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~OpcGroupInfo()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
