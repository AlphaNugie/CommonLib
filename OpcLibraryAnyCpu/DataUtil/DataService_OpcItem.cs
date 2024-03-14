using CommonLib.DataUtil;
using OPCAutomation;
using OpcLibrary.Core;
using OpcLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OpcLibrary.DataUtil
{
    /// <summary>
    /// OPC项的数据库服务类
    /// </summary>
    public class DataService_OpcItem : BaseDataServiceSqlite
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public DataService_OpcItem() : base(OpcConst.SqliteFileDir, OpcConst.SqliteFileName) { }

/// <inheritdoc/>
        protected override string GetTableName()
        {
            return "t_plc_opcitem";
        }

        /// <inheritdoc/>
        protected override List<SqliteColumnMapping> GetColumnsMustHave()
        {
            return new List<SqliteColumnMapping>()
            {
                new SqliteColumnMapping("record_id", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, null, false, ConflictClause.NONE, true, ConflictClause.FAIL, true),
                new SqliteColumnMapping("item_id", SqliteSqlType.VARCHAR2, 64, true, ConflictClause.FAIL),
                new SqliteColumnMapping("opcgroup_id", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL),
                new SqliteColumnMapping("field_name", SqliteSqlType.VARCHAR2, 64),
                new SqliteColumnMapping("enabled", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, 1),
                new SqliteColumnMapping("coeff", SqliteSqlType.DOUBLE, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("offset", SqliteSqlType.DOUBLE, null, true, ConflictClause.FAIL, 0)
            };
        }

        #region 查询
        ///// <summary>
        ///// 获取所有可用的OPC项信息
        ///// </summary>
        ///// <returns></returns>
        //public DataTable GetOpcInfo()
        //{
        //    string sql = "select * from t_plc_opcgroup g left join t_plc_opcitem i on g.group_id = i.opcgroup_id where i.enabled = 1 order by g.group_id, i.record_id";
        //    return Provider.Query(sql);
        //}

        /// <summary>
        /// 获取所有t_plc_opcitem记录
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllOpcItemRecords()
        {
            string sql = "select * from t_plc_opcitem";
            return Provider.Query(sql);
        }

        /// <summary>
        /// 获取所有OPC项，按ID排序
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllOpcItemsOrderbyId()
        {
            return GetAllOpcItems("record_id");
        }

        /// <summary>
        /// 获取所有OPC项，并按特定字段排序
        /// </summary>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetAllOpcItems(string orderby)
        {
            return GetOpcItems(0, orderby);
        }

        /// <summary>
        /// 根据所属OPC组的ID获取所有OPC项，并按特定字段排序
        /// </summary>
        /// <param name="opcgroup_id">OPC组的ID，为0则查询所有</param>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetOpcItems(int opcgroup_id, string orderby)
        {
            return GetOpcItems(opcgroup_id, orderby, out _);
        }

        /// <summary>
        /// 根据所属OPC组的ID获取所有OPC项，并按特定字段排序
        /// </summary>
        /// <param name="opcgroup_id">OPC组的ID，为0则查询所有</param>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <param name="message">查询时返回的消息</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">查询时检查表及字段是否存在，不存在则新增表或字段，操作失败时抛出此异常</exception>
        public DataTable GetOpcItems(int opcgroup_id, string orderby, out string message)
        {
            if (!CheckForTableColumns(out message))
                throw new InvalidOperationException(message);
            //CheckForTableColumns(out message);
            string sql = string.Format(@"
select i.*, 0 changed from t_plc_opcitem i
  left join t_plc_opcgroup g on g.group_id = i.opcgroup_id
  where {0} = 0 or g.group_id = {0} {1}", opcgroup_id, string.IsNullOrWhiteSpace(orderby) ? string.Empty : "order by i." + orderby);
            return Provider.Query(sql);
        }
        #endregion

        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public int DeleteOpcItemById(int id)
        {
            string sql = string.Format("delete from t_plc_opcitem where record_id = {0}", id);
            return Provider.ExecuteSql(sql);
        }

        /// <summary>
        /// 根据多个ID删除
        /// </summary>
        /// <param name="ids">多个ID的列表</param>
        /// <returns></returns>
        public int DeleteOpcItemByIds(IEnumerable<int> ids)
        {
            string sql = string.Format("delete from t_plc_opcitem where record_id in ({0})", string.Join(", ", ids.ToArray()));
            return Provider.ExecuteSql(sql);
        }

        /// <summary>
        /// 保存OPC项信息
        /// </summary>
        /// <param name="item">OPC项对象</param>
        /// <returns></returns>
        public int SaveOpcItem(OpcItem item)
        {
            return Provider.ExecuteSql(GetSqlString(item));
        }

        /// <summary>
        /// 批量保存OPC项信息
        /// </summary>
        /// <param name="items">多个OPC项对象</param>
        /// <returns></returns>
        public bool SaveOpcItems(IEnumerable<OpcItem> items)
        {
            string[] sqls = items?.Select(radar => GetSqlString(radar)).ToArray();
            return Provider.ExecuteSqlTrans(sqls);
        }

        /// <summary>
        /// 获取OPC项SQL字符串
        /// </summary>
        /// <param name="item">OPC项对象</param>
        /// <returns></returns>
        private string GetSqlString(OpcItem item)
        {
            string sql = string.Empty;
            if (item != null)
                sql = string.Format(item.RecordId <= 0 ? "insert into t_plc_opcitem (item_id, opcgroup_id, field_name, enabled, coeff, offset) values ('{1}', {2}, '{3}', {4}, {5}, {6})" : "update t_plc_opcitem set item_id = '{1}', opcgroup_id = {2}, field_name = '{3}', enabled = {4}, coeff = {5}, offset = {6} where record_id = {0}", item.RecordId, item.ItemId, item.OpcGroupId, item.FieldName, item.Enabled ? 1 : 0, item.Coeff, item.Offset);
            return sql;
        }
    }
}
