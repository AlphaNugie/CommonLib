using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Reflection;
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

        /// <summary>
        /// 数据源字段名称
        /// </summary>
        public string FieldName { get; set; }

        private PropertyInfo prop = null;
        /// <summary>
        /// 根据数据源字段名称获取的属性，假如OpcGroupInfo.DataSource属性为空，或找到该字段，则属性为空
        /// </summary>
        internal PropertyInfo Property
        {
            get { return this.prop; }
            set
            {
                this.prop = value;
                this.ConvertTypeMethod = this.prop == null ? null : Converter.ConvertTypeMethod.MakeGenericMethod(this.prop.PropertyType);
            }
        }

        /// <summary>
        /// 转换类型的静态泛型方法（类型参数为字段类型），假如属性为空则为空
        /// </summary>
        internal MethodInfo ConvertTypeMethod { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="itemId">OPC项ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        public OpcItemInfo(string itemId, int clientHandle) : this(itemId, clientHandle, null) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="itemId">OPC项ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        /// <param name="fieldName">数据源中字段名称</param>
        public OpcItemInfo(string itemId, int clientHandle, string fieldName)
        {
            this.ItemId = itemId;
            this.ClientHandle = clientHandle;
            this.Value = string.Empty;
            this.FieldName = fieldName;
        }

        /// <summary>
        /// 获取经过转换方法转换后的数据源字段值，假如转换方法为空则返回空
        /// </summary>
        /// <returns></returns>
        public object GetPropertyValue()
        {
            return this.ConvertTypeMethod?.Invoke(null, new object[] { this.Value });
        }

        /// <summary>
        /// 假如属性或给定的数据源不为空，则为数据源设置经过转换方法转换后的数据源字段值
        /// </summary>
        /// <param name="dataSource"></param>
        public void SetPropertyValue(object dataSource)
        {
            if (this.Property != null && dataSource != null)
                this.Property.SetValue(dataSource, this.GetPropertyValue());
        }

        /// <summary>
        /// 假如属性或给定的数据源不为空，则从数据源向Item赋值
        /// </summary>
        /// <param name="dataSource"></param>
        public void SetItemValue(object dataSource)
        {
            if (this.Property != null && dataSource != null)
                this.Value = this.Property.GetValue(dataSource).ToString();
        }
    }
}
