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
        private readonly Random _random = new Random();
        #region 私有变量
        private Array _itemIds, _serverHandles, _clientHandles, _errors;
        private readonly OpcItemInfo _opcPackBasic = new OpcItemInfo(string.Empty, 0);
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
        /// 组的类型，读或写，仅在决定从数据源读值或向数据源写值时起作用
        /// </summary>
        public GroupType GroupType { get; set; }

        private object _data_source = null;
        /// <summary>
        /// 数据源，需要在连接前设置（否则无效），从OPC读取向数据源写入或从数据源读取向OPC写入，写入时假如数据源不为空则优先从数据源取值
        /// </summary>
        public object DataSource
        {
            get { return _data_source; }
            set
            {
                _data_source = value;
                Type type = _data_source?.GetType();
                foreach (var itemInfo in ListItemInfo)
                    if (type != null && !string.IsNullOrWhiteSpace(itemInfo.FieldName))
                        itemInfo.Property = type.GetProperty(itemInfo.FieldName);
            }
        }

        /// <summary>
        /// OPC组所拥有的OPC项数量
        /// </summary>
        public int ItemCount { get { return OpcGroup == null ? 0 : OpcGroup.OPCItems.Count; } }

        /// <summary>
        /// OPC项ID Array，添加OPC项时变化
        /// </summary>
        public Array ItemIds
        {
            get { return _itemIds; }
            private set { _itemIds = value; }
        }

        /// <summary>
        /// OPC项服务端句柄 Array，添加、移除OPC项时变化
        /// </summary>
        public Array ServerHandles
        {
            get { return _serverHandles; }
            private set { _serverHandles = value; }
        }

        /// <summary>
        /// OPC项客户端句柄 Array，添加OPC项时变化
        /// </summary>
        public Array ClientHandles
        {
            get { return _clientHandles; }
            private set { _clientHandles = value; }
        }

        /// <summary>
        /// 错误信息Array，添加、移除OPC项，读取、写入值时变化
        /// </summary>
        public Array Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }

        /// <summary>
        /// OPC项信息List，包含OPC项ID、客户端句柄、服务端句柄、值等信息，添加OPC组前设置此属性可在添加OPC组（OpcUtilHelper.CreateGroups方法）时直接添加OPC项
        /// </summary>
        public List<OpcItemInfo> ListItemInfo { get; set; }

        ///// <summary>
        ///// 错误信息
        ///// </summary>
        //public string ErrorMessage { get; private set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="groups">待创建的OPC组所属于的组集合，为null则无法创建</param>
        /// <param name="name">OPC组名称</param>
        public OpcGroupInfo(OPCGroups groups, string name) : this(groups, name, null) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="groups">待创建的OPC组所属于的组集合，为null则无法创建</param>
        /// <param name="name">OPC组名称</param>
        /// <param name="data_source">数据源</param>
        public OpcGroupInfo(OPCGroups groups, string name, object data_source)
        {
            GroupName = name;
            ListItemInfo = new List<OpcItemInfo>();
            DataSource = data_source;
            GroupType = GroupType.READ;
            SetOpcGroup(groups, name);
        }

        #region 功能
        /// <summary>
        /// 设置OPC组
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="name"></param>
        public void SetOpcGroup(OPCGroups groups, string name)
        {
            if (groups == null)
                return;

            OpcGroup = groups.Add(name);
        }

        /// <summary>
        /// 设置OPC组属性
        /// </summary>
        /// <param name="updateRate">OPC组更新速度</param>
        /// <param name="isGroupActive">OPC组激活状态</param>
        /// <param name="isGroupSubscribed">OPC组订阅状态</param>
        public void SetGroupProperty(int updateRate, bool isGroupActive, bool isGroupSubscribed)
        {
            if (OpcGroup != null)
            {
                OpcGroup.UpdateRate = updateRate;
                OpcGroup.IsActive = isGroupActive;
                OpcGroup.IsSubscribed = isGroupSubscribed;
            }
        }

        /// <summary>
        /// 根据给定的OPC服务端句柄获取符合条件的OpcItemInfo对象，列表最前方包含一个内容为空的OpcItemInfo对象
        /// </summary>
        /// <param name="serverHandles">给定的服务端句柄数组，假如给定的服务端句柄为空，则给出所有服务端句柄</param>
        /// <returns></returns>
        private List<OpcItemInfo> GetFilteredItems(IEnumerable<int> serverHandles)
        {
            //特殊条件：服务端句柄范围为空
            bool flag = serverHandles == null || serverHandles.Count() == 0;
            //假如给定的服务端句柄范围为空则选择所有OPC项，否则选择服务端在范围内的OPC项（服务端句柄不可为0）
            List<OpcItemInfo> list = ListItemInfo?.Where(item => (flag || serverHandles.Contains(item.ServerHandle)) && item.ServerHandle != 0).ToList();
            //在最前方插入一个空OPC项，占位用
            list.Insert(0, _opcPackBasic);
            return list;
        }

        /// <summary>
        /// 获取在给定服务端句柄中（第1项须为0）存在对应OPC项的服务端句柄并转换为Array，假如给定的服务端句柄为空，则给出所有服务端句柄
        /// </summary>
        /// <param name="serverHandles">给定的服务端句柄数组，假如给定的服务端句柄为空，则给出所有服务端句柄</param>
        /// <returns></returns>
        private Array GetServerHandles(IEnumerable<int> serverHandles)
        {
            var list = GetFilteredItems(serverHandles);
            return list == null ? new int[0] : list.Select(item => item.ServerHandle).ToArray();
        }

        /// <summary>
        /// 获取在给定服务端句柄中存在对应OPC项的值并转换为Array，假如给定的服务端句柄为空，则给出所有值
        /// </summary>
        /// <param name="serverHandles">给定的服务端句柄数组，假如给定的服务端句柄为空，则给出所有值</param>
        /// <returns></returns>
        public Array GetValues(IEnumerable<int> serverHandles)
        {
            ListItemInfo.ForEach(item => item.SetItemValue(_data_source)); //根据数据源为item赋值（假如数据源不为空或字段名称正确）
            var list = GetFilteredItems(serverHandles);
            return list == null ? new object[0] : list.Select(item => (object)item.Value).ToArray();
        }

        /// <summary>
        /// 获取所有OPC项的值并转换为Array
        /// </summary>
        /// <returns></returns>
        public Array GetValues()
        {
            return GetValues(null);
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
                if (ItemCount > 0)
                    OpcGroup.OPCItems.Remove(ItemCount, ref _serverHandles, out _errors);
                List<OpcItemInfo> itemList = items.Where(item => !string.IsNullOrWhiteSpace(item.ItemId)).ToList(); //排除id为空的项（后面将重新添加），转换为新List对象，防止枚举改变对象时出现未知影响
                ListItemInfo.Clear();
                ListItemInfo.Add(_opcPackBasic);
                itemList.Insert(0, _opcPackBasic);
                _itemIds = itemList.Select(p => p.ItemId).ToArray();
                _clientHandles = itemList.Select(p => p.ClientHandle).ToArray();
                OpcGroup.OPCItems.AddItems(itemList.Count - 1, ref _itemIds, ref _clientHandles, out _serverHandles, out _errors);
                //添加OPC项后根据ID找到OPC项信息对象并设置服务端句柄，向OPC项信息List中添加
                if (_itemIds.Length > 1)
                {
                    Type type = _data_source?.GetType();
                    for (int i = 1; i < _itemIds.Length; i++)
                    {
                        OpcItemInfo itemInfo = itemList.Find(p => p.ItemId.Equals(_itemIds.GetValue(i)));
                        itemInfo.ServerHandle = int.Parse(_serverHandles.GetValue(i).ToString());
                        if (type != null && !string.IsNullOrWhiteSpace(itemInfo.FieldName))
                            itemInfo.Property = type.GetProperty(itemInfo.FieldName);
                        ListItemInfo.Add(itemInfo);
                    }
                }
                //假如添加后的数量对不上，则至少有一个OPC项未添加成功
                if (ListItemInfo.Count < itemList.Count)
                    message = "至少有1个OPC项未添加成功";
            }
            catch (Exception ex)
            {
                message = string.Format("OPC组{0}移除或添加标签时发生错误:{1}", OpcGroup.Name, ex.Message);
                return false;
            }
            return ListItemInfo.Count > 1; //假如至少有1个添加成功，返回true
        }

        /// <summary>
        /// 为OPC组的所有OPC项读取数据
        /// </summary>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool ReadValues(out string message)
        {
            return ReadValues(null, out message);
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
            Array values;
            try
            {
                IEnumerable<int> temp = serverHandles == null || serverHandles.Length == 0 ? null : serverHandles.Cast<int>();
                Array handles = GetServerHandles(temp);
                int itemCount = handles.Length - 1;
                //OPCDataSource指定的是OPCCache或者OPCDevice
                //它的含义是当你执行同步读操作时，读取的数据来自OPC服务器的缓存还是来自硬件设备
                OpcGroup.SyncRead((short)OPCDataSource.OPCDevice, itemCount, ref handles, out values, out _errors, out object qualities, out object timeStamps);
                //假如至少读取到1个值，根据服务端句柄找到OPC项信息并更新值
                if (values.Length > 0)
                    for (int i = 1; i <= values.Length; i++)
                    {
                        OpcItemInfo itemInfo = ListItemInfo.Find(item => item.ServerHandle.Equals(handles.GetValue(i)));
                        if (itemInfo != null)
                        {
                            itemInfo.Value = values.GetValue(i).ToString();
                            itemInfo.SetPropertyValue(_data_source);
                            //if (itemInfo.Property != null/* && GroupType == GroupType.READ*/)
                            //    itemInfo.Property.SetValue(DataSource, itemInfo.GetPropertyValue());
                        }
                    }
                if (values.Length < itemCount)
                    message = "至少有1个OPC项的值未找到";
                GC.Collect();
            }
            catch (Exception ex)
            {
                message = string.Format("从名称为{0}的OPC组读取值失败：{1}", OpcGroup == null ? "null" : OpcGroup.Name, ex.Message);
                return false;
            }
            return values.Length > 0;
        }

        /// <summary>
        /// 为OPC组的所有OPC项写入数据，默认使用同步写入
        /// </summary>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool WriteValues(out string message)
        {
            return WriteValues(null, false, out message);
        }

        /// <summary>
        /// 为OPC组的所有OPC项写入数据，指定是否使用异步写入
        /// </summary>
        /// <param name="using_async">是否使用异步写入</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool WriteValues(bool using_async, out string message)
        {
            return WriteValues(null, using_async, out message);
        }

        /// <summary>
        /// 为OPC组OPC项List内与给定服务端句柄对应的OPC项写入数据，默认使用同步写入
        /// </summary>
        /// <param name="serverHandles">服务端句柄Array</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool WriteValues(Array serverHandles, out string message)
        {
            return WriteValues(serverHandles, false, out message);
        }

        /// <summary>
        /// 为OPC组OPC项List内与给定服务端句柄对应的OPC项写入数据，指定是否使用异步写入
        /// </summary>
        /// <param name="serverHandles">服务端句柄Array</param>
        /// <param name="using_async">是否使用异步写入</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool WriteValues(Array serverHandles, bool using_async, out string message)
        {
            message = string.Empty;
            try
            {
                IEnumerable<int> temp = serverHandles == null || serverHandles.Length == 0 ? null : serverHandles.Cast<int>();
                Array handles = GetServerHandles(temp), values = GetValues(temp);
                int itemCount = handles.Length - 1;
                if (!using_async)
                    OpcGroup.SyncWrite(itemCount, ref handles, ref values, out _errors);
                else
                    OpcGroup.AsyncWrite(itemCount, ref handles, ref values, out _errors, _random.Next(1, 1000), out int cancelId);
                GC.Collect();
            }
            catch (Exception ex)
            {
                message = string.Format("向名称为{0}的OPC组写入值失败：{1}", OpcGroup.Name, ex.Message);
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
                    OpcGroup = null;
                    ItemIds = null;
                    ClientHandles = null;
                    Errors = null;
                    ServerHandles = null;
                    ListItemInfo.Clear();
                    ListItemInfo = null;
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

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    /// <summary>
    /// 组类型，读或写
    /// </summary>
    public enum GroupType
    {
        /// <summary>
        /// 读
        /// </summary>
        READ = 1,

        /// <summary>
        /// 写
        /// </summary>
        WRITE = 2
    }
}
