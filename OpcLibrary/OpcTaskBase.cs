using CommonLib.Clients.Tasks;
using CommonLib.Function;
using OpcLibrary.Core;
using OpcLibrary.DataUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace OpcLibrary
{
    /// <summary>
    /// OPC读取、写入任务
    /// </summary>
    [Obsolete("请使用OpcLibraryAnyCpu.dll")]
    public abstract class OpcTaskBase : Task
    {
        /// <summary>
        /// OPC功能包装类的实体，进行具体的OPC操作
        /// </summary>
        protected OpcUtilHelper opcHelper = new OpcUtilHelper(1000, true);

        /// <summary>
        /// 包含的OPC组ID，不在此范围内的ID不读取或写入，假如为空（为null或长度为0）则初始化所有OPC组
        /// </summary>
        protected int[] _idsIncl = null;

        ///// <summary>
        ///// 构造器
        ///// </summary>
        //public OpcTaskBase() : this(null) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="idsIncl">包含的OPC组ID，不在此范围内的ID不读取或写入，假如为空则初始化所有OPC组</param>
        public OpcTaskBase(params int[] idsIncl) : base()
        {
            _idsIncl = idsIncl;
        }

/// <inheritdoc/>
        protected override void Init()
        {
            OpcInit();
            SetOpcGroupsDataSource();
        }

/// <inheritdoc/>
        protected override void LoopContent()
        {
            Interval = OpcConst.OpcLoopInterval;
            LoopUrContentBeforeRW();
            OpcReadValues();
            OpcWriteValues();
            LoopUrContentAfterRW();
        }

/// <inheritdoc/>
        protected override Task GetNewInstance()
        {
            //return null;
            return GetNewOpcInstance();
        }

        #region 抽象方法
        /// <summary>
        /// 在继承的子类中，在每次循环的OPC写入与读取之前，调用自己想在循环中使用的额外方法
        /// </summary>
        protected abstract void LoopUrContentBeforeRW();

        /// <summary>
        /// 在继承的子类中，在每次循环的OPC写入与读取之后，调用自己想在循环中使用的额外方法
        /// </summary>
        protected abstract void LoopUrContentAfterRW();

        /// <summary>
        /// 获取新OpcTask实体的方法（目前仅为任务重启使用），在这个方法中重新声明、初始化一个当前OpcTask类型的实体并返回（假如返回实体为空将不执行重启）
        /// </summary>
        /// <returns></returns>
        protected abstract OpcTaskBase GetNewOpcInstance();

        /// <summary>
        /// 获取数据源，用于OPC读取()与写入()
        /// </summary>
        /// <returns></returns>
        protected abstract object GetOpcDatasource();
        #endregion

        /// <summary>
        /// OPC初始化
        /// </summary>
        protected void OpcInit()
        {
            if (!OpcConst.OpcEnabled)
                return;

            OpcConst.WriteConsoleLog(string.Format("开始连接IP地址为{0}的OPC SERVER {1}...", OpcConst.OpcServerIp, OpcConst.OpcServerName));
            //DataService_Opc dataService_Opc = new DataService_Opc();
            opcHelper = new OpcUtilHelper(1000, true);
            string[] servers = opcHelper.ServerEnum(OpcConst.OpcServerIp, out _errorMessage);
            if (!string.IsNullOrWhiteSpace(_errorMessage))
            {
                OpcConst.WriteConsoleLog(string.Format("枚举过程中出现问题：{0}", _errorMessage));
                goto END_OF_OPC;
            }
            if (servers == null || !servers.Contains(OpcConst.OpcServerName))
            {
                OpcConst.WriteConsoleLog(string.Format("无法找到指定OPC SERVER：{0}", OpcConst.OpcServerName));
                goto END_OF_OPC;
            }
            //DataTable table = new DataService_Opc().GetOpcInfo();
            //查询OPC信息时产生的错误信息或新增表、字段的提示
            string message;
            DataTable table;
            try { table = new DataService_Opc().GetOpcInfo(out message); }
            catch (Exception e)
            {
                OpcConst.WriteConsoleLog(string.Format("查询OPC记录时产生异常：{0}\r\n{1}", e.Message, e.ToString()));
                goto END_OF_OPC;
            }
            //正常情况下返回的消息一般为新增表或字段
            if (!string.IsNullOrWhiteSpace(message))
                OpcConst.WriteConsoleLog(message);
            if (table == null || table.Rows.Count == 0)
            {
                OpcConst.WriteConsoleLog(string.Format("在表中未找到任何OPC记录，将不进行读取或写入", OpcConst.OpcServerName));
                goto END_OF_OPC;
            }
            List<OpcGroupInfo> groups = new List<OpcGroupInfo>();
            List<DataRow> dataRows = table.Rows.Cast<DataRow>().ToList();
            List<OpcItemInfo> items = null;
            int id = 0;
            foreach (var row in dataRows)
            {
                //string itemId = row["item_id"].ConvertType<string>();
                string itemId = row.Convert<string>("item_id");
                if (string.IsNullOrWhiteSpace(itemId))
                    continue;
                //int groupId = row["group_id"].ConvertType<int>(), clientHandle = row["record_id"].ConvertType<int>();
                //string groupName = row["group_name"].ConvertType<string>(), fieldName = row["field_name"].ConvertType<string>();
                int groupId = row.Convert<int>("group_id"), clientHandle = row.Convert<int>("record_id");
                string groupName = row.Convert<string>("group_name"), fieldName = row.Convert<string>("field_name");
                ////值的系数与偏移量，假如列不存在默认为0
                //double coeff = 0, offset = 0;
                //try { coeff = row["coeff"].ConvertType<double>(); } catch (Exception) { }
                //try { offset = row["offset"].ConvertType<double>(); } catch (Exception) { }
                //GroupType type = (GroupType)row["group_type"].ConvertType<int>();
                //值的系数与偏移量，假如列不存在默认为0
                double coeff = row.Convert("coeff", 0.0), offset = row.Convert("offset", 0.0);
                GroupType type = (GroupType)row.Convert<int>("group_type");
                //if (groupId != id)
                //假如OPC组的ID包含在范围内
                if (groupId != id)
                {
                    if (_idsIncl != null && _idsIncl.Length > 0 && !_idsIncl.Contains(groupId))
                        continue;
                    id = groupId;
                    groups.Add(new OpcGroupInfo(null, groupName/*, OpcDatasource*/) { GroupType = type, ListItemInfo = new List<OpcItemInfo>() });
                    OpcGroupInfo groupInfo = groups.Last();
                    items = groupInfo.ListItemInfo;
                }
                items.Add(new OpcItemInfo(itemId, clientHandle, fieldName, coeff, offset));
            }
            opcHelper.ListGroupInfo = groups;
            opcHelper.ConnectRemoteServer(OpcConst.OpcServerIp, OpcConst.OpcServerName, out _errorMessage);
            OpcConst.WriteConsoleLog(string.Format("OPC连接状态：{0}", opcHelper.OpcConnected));
            if (!string.IsNullOrWhiteSpace(_errorMessage))
                OpcConst.WriteConsoleLog(string.Format("连接过程中出现问题：{0}", _errorMessage));
            END_OF_OPC:;
        }

        /// <summary>
        /// 设置数据源
        /// </summary>
        protected void SetOpcGroupsDataSource()
        {
            if (opcHelper != null && opcHelper.ListGroupInfo != null)
                //opcHelper.ListGroupInfo.ForEach(group => group.DataSource = OpcConst.OpcDatasource);
                opcHelper.ListGroupInfo.ForEach(group => group.DataSource = GetOpcDatasource());
        }

        /// <summary>
        /// 读取值
        /// </summary>
        protected void OpcReadValues()
        {
            if (!OpcConst.OpcEnabled)
                return;

            opcHelper.ListGroupInfo.ForEach(group =>
            {
                if (group.GroupType != GroupType.READ)
                    return;

                if (!group.ReadValues(out _errorMessage))
                    OpcConst.WriteConsoleLog(string.Format("读取PLC失败，读取过程中出现问题：{0}", _errorMessage));
            });
        }

        /// <summary>
        /// 写入值
        /// </summary>
        protected void OpcWriteValues()
        {
            if (!OpcConst.OpcEnabled || !OpcConst.Write2Plc)
                return;

            opcHelper.ListGroupInfo.ForEach(group =>
            {
                if (group.GroupType != GroupType.WRITE)
                    return;

                if (!group.WriteValues(out _errorMessage))
                    OpcConst.WriteConsoleLog(string.Format("写入PLC失败，写入过程中出现问题：{0}", _errorMessage));
            });
        }
    }
}
