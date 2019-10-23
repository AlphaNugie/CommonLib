using System;
using System.Collections.Generic;
using System.Text;

namespace OpcLibrary
{
    /// <summary>
    /// OPC项信息实体类，每个OPC项信息实体对应单独的OPC项ID、服务端句柄、客户端句柄以及值（供读取或写入）
    /// 可根据这些信息为OPC组添加OPC项（OpcGroupInfo中的SetItems方法）
    /// </summary>
    public class OpcItemInfo
    {
        private int client_handle;

        #region 属性
        /// <summary>
        /// OPC项ID（名称）
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// OPC服务端句柄
        /// </summary>
        public int ServerHandle { get; set; }

        /// <summary>
        /// OPC客户端句柄
        /// </summary>
        public int ClientHandle
        {
            get { return this.client_handle; }
            set { this.client_handle = value; }
        }

        /// <summary>
        /// 读取或待写入的值
        /// </summary>
        public string Value { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="itemId">OPC项ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        public OpcItemInfo(string itemId, int clientHandle)
        {
            this.ItemId = itemId;
            this.ClientHandle = clientHandle;
            this.Value = string.Empty;
        }
    }
}
