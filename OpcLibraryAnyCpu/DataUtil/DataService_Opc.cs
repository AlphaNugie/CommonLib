using CommonLib.DataUtil;
using OpcLibrary.Core;
using OpcLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OpcLibrary.DataUtil
{
    /// <summary>
    /// OpcTask任务使用的OPC数据源服务
    /// </summary>
    public class DataService_Opc
    {
        private readonly SqliteProvider provider = new SqliteProvider(OpcConst.SqliteFileDir, OpcConst.SqliteFileName);

        /// <summary>
        /// t_plc_opcgroup表的DDL语句
        /// </summary>
        public const string GroupDDL = @"
CREATE TABLE [t_plc_opcgroup](
  [group_id] INTEGER PRIMARY KEY ON CONFLICT FAIL AUTOINCREMENT NOT NULL ON CONFLICT FAIL, 
  [group_name] VARCHAR2(32) NOT NULL ON CONFLICT FAIL, 
  [group_type] INTEGER(1) NOT NULL ON CONFLICT FAIL DEFAULT 1, 
  [enabled] INTEGER NOT NULL ON CONFLICT FAIL DEFAULT 1);
";
        /// <summary>
        /// t_plc_opcitem表的DDL语句
        /// </summary>
        public const string ItemDDL = @"
CREATE TABLE [t_plc_opcitem](
  [record_id] INTEGER PRIMARY KEY ON CONFLICT FAIL AUTOINCREMENT NOT NULL ON CONFLICT FAIL, 
  [item_id] VARCHAR2(64) NOT NULL ON CONFLICT FAIL, 
  [opcgroup_id] INTEGER NOT NULL ON CONFLICT FAIL, 
  [field_name] VARCHAR2(64), 
  [enabled] INTEGER NOT NULL ON CONFLICT FAIL DEFAULT 1, 
  [coeff] DOUBLE NOT NULL ON CONFLICT FAIL DEFAULT 0,
  [offset] DOUBLE NOT NULL ON CONFLICT FAIL DEFAULT 0);
";

        /// <summary>
        /// 从OPC数据源获取基础数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">查询时检查表及字段是否存在，不存在则新增表或字段，操作失败时抛出此异常</exception>
        public DataTable GetOpcInfo()
        {
            return GetOpcInfo(out _);
        }

        /// <summary>
        /// 从OPC数据源获取基础数据
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">查询时检查表及字段是否存在，不存在则新增表或字段，操作失败时抛出此异常</exception>
        public DataTable GetOpcInfo(out string message)
        {
            if (!new DataService_OpcGroup().CheckForTableColumns(out message) || !new DataService_OpcItem().CheckForTableColumns(out message))
                throw new InvalidOperationException(message);
            //new DataService_OpcGroup().CheckForTableColumns(out string m_group);
            //new DataService_OpcItem().CheckForTableColumns(out string m_item);
            //message = string.IsNullOrWhiteSpace(m_group) ? m_item : m_group;
            string sql = "select * from t_plc_opcgroup g left join t_plc_opcitem i on g.group_id = i.opcgroup_id where i.enabled = 1 order by g.group_id, i.record_id";
            return provider.Query(sql);
        }
    }
}
