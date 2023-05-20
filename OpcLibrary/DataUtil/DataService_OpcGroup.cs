using CommonLib.DataUtil;
using OpcLibrary.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OpcLibrary.DataUtil
{
    /// <summary>
    /// OPC组的数据库服务类
    /// </summary>
    public class DataService_OpcGroup : BaseDataServiceSqlite
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public DataService_OpcGroup() : base(OpcConst.SqliteFileDir, OpcConst.SqliteFileName) { }

        /// <inheritdoc/>
        protected override string GetTableName()
        {
            return "t_plc_opcgroup";
        }

        /// <inheritdoc/>
        protected override List<SqliteColumnMapping> GetColumnsMustHave()
        {
            return new List<SqliteColumnMapping>()
            {
                new SqliteColumnMapping("group_id", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, null, false, ConflictClause.NONE, true, ConflictClause.FAIL, true),
                new SqliteColumnMapping("group_name", SqliteSqlType.VARCHAR2, 32, true, ConflictClause.FAIL),
                new SqliteColumnMapping("group_type", SqliteSqlType.INTEGER, 1, true, ConflictClause.FAIL, 1),
                new SqliteColumnMapping("enabled", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, 1),
            };
        }

        /// <inheritdoc/>
        public override bool CheckForTableColumns(out string message)
        {
            if (!base.CheckForTableColumns(out message))
                return false;
            var groupNames = _provider.Query("select * from " + TableName).Rows.Cast<DataRow>().Select(row => row["group_name"].ToString()).ToList();
            var sqls = new List<string>();
            if (!groupNames.Contains("OPC_GROUP_READ"))
                sqls.Add(string.Format("insert into {0} (group_name, group_type) values ('OPC_GROUP_READ', 1)", TableName));
            if (!groupNames.Contains("OPC_GROUP_WRITE"))
                sqls.Add(string.Format("insert into {0} (group_name, group_type) values ('OPC_GROUP_WRITE', 1)", TableName));
            if (sqls.Count == 0)
                return true;
            //string[] sqls = new string[]
            //{
            //    string.Format("insert into {0} (group_name, group_type) values ('OPC_GROUP_READ', 1)", TableName),
            //    string.Format("insert into {0} (group_name, group_type) values ('OPC_GROUP_WRITE', 2)", TableName),
            //};
            bool result = _provider.ExecuteSqlTrans(sqls);
            if (!result)
                message = _provider.ErrorMessage;
            return result;
        }
    }
}
