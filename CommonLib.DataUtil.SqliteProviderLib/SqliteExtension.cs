using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// SQLite扩展方法静态类
    /// </summary>
    public static class SqliteExtension
    {
        /// <summary>
        /// 根据给定的表名及数据库字段列表获取数据库字段信息，假如列表为空则获取所有字段
        /// </summary>
        /// <param name="provider">SQLite基础操作类对象，为空时返回null</param>
        /// <param name="tableName">表名，为空时返回null</param>
        /// <param name="columnNames">给定的数据库字段列表，假如为空则获取所有字段</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static List<SqliteColumnMapping> GetColumnMappings(this SqliteProvider provider, string tableName, IEnumerable<string> columnNames, out string message)
        {
            message = string.Empty;
            if (provider == null || string.IsNullOrWhiteSpace(tableName))
                return null;
            //List<SqliteColumnMapping> columns = new List<SqliteColumnMapping>();
            List<SqliteColumnMapping> columns = null;
            string sqlString = $"pragma table_info('{tableName}')";
            DataTable table = provider.Query(sqlString);
            if (table == null || table.Rows.Count == 0)
            {
                message = $"未找到表{tableName}的任何列信息";
                goto END;
            }
            //获取所有列信息，假如给定的列名不为空，则筛选出对应列
            columns = table.Rows.Cast<DataRow>().Select(dataRow => new SqliteColumnMapping(dataRow)).ToList();
            if (columnNames == null || columnNames.Count() == 0)
                goto END;
            columns = columns.Where(column => columnNames.Contains(column.ColumnName)).ToList();
        END:
            return columns;
        }

        /// <summary>
        /// 根据给定的表名判断此表是否存在
        /// </summary>
        /// <param name="provider">SQLite基础操作类对象，为空时返回false</param>
        /// <param name="tableName">表名，为空时返回false</param>
        /// <param name="table">假如此表存在，输出表内容</param>
        /// <returns></returns>
        public static bool TableExists(this SqliteProvider provider, string tableName, out DataTable table)
        {
            table = null;
            if (provider == null || string.IsNullOrWhiteSpace(tableName))
                return false;
            string sql = "select * from " + tableName;
            try
            {
                table = provider.Query(sql);
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// 根据给定的表名检查数据表是否存在，假如存在返回true，假如不存在则用给定列进行新增表操作，最终返回操作结果
        /// </summary>
        /// <param name="provider">SQLite基础操作类对象，为空时返回false</param>
        /// <param name="tableName">表名，为空时返回false</param>
        /// <param name="columnsMustHave">假如表不存在，新增时必须拥有的列，假如未指定将不会进行新增表操作（表不存在且新增列为空时返回false）</param>
        /// <param name="table">假如表已存在，或未存在但新增成功，则返回表内容</param>
        /// <param name="errorMessage">返回的信息，产生错误时返回错误信息，表存在但新增成功时返回提示信息，其余情况（例如表本身已存在）则不返回任何消息</param>
        /// <returns>所有操作完成之后返回表最终是否存在的判断结果</returns>
        public static bool CheckForTable(this SqliteProvider provider, string tableName, IEnumerable<SqliteColumnMapping> columnsMustHave, out DataTable table, out string errorMessage)
        {
            table = null;
            errorMessage = string.Empty;
            //if (string.IsNullOrWhiteSpace(tableName) || tableName == null || tableName.Count() == 0)
            //    return true;
            //if (TableExists(provider, tableName, out table))
            //    return true;
            //假如操作对象或给定表名为空，返回false
            if (provider == null || string.IsNullOrWhiteSpace(tableName))
            {
                errorMessage = "给定的SQLite操作对象或表名为空";
                return false;
            }
            //假如表存在，返回true
            if (provider.TableExists(tableName, out table))
                return true;
            if (columnsMustHave == null || columnsMustHave.Count() == 0)
            {
                errorMessage = "表不存在，因给定列为空，将不进行新增操作";
                return false;
            }
            string columns = string.Join(string.Empty, columnsMustHave.Select(column => string.Format("\r\n  {0},", column.GetStructure(true)))).TrimEnd(',');
            string sqlString = string.Format("create table [{0}] ({1})", tableName, columns);
            provider.ExecuteSql(sqlString);
            bool result = provider.TableExists(tableName, out table);
            errorMessage = result ? string.Format("已添加表{0}及其{1}个字段", tableName, columnsMustHave.Count()) : string.Format("表{0}添加失败", tableName);
            return result;
        }


        /// <summary>
        /// 根据给定的表名及列信息检查数据表的字段，假如表不存在先新增表，然后根据给定列判断字段是否存在，不存在则新增列，最终返回操作结果
        /// </summary>
        /// <param name="provider">SQLite基础操作类对象，为空时返回false</param>
        /// <param name="tableName">表名，为空时返回false</param>
        /// <param name="columnsMustHave">判断表内是否存在的列，不存在则新增，假如未指定将不会进行新增操作并返回true（此种情况没必要进行操作）</param>
        /// <param name="message">返回的消息</param>
        /// <returns></returns>
        public static bool CheckForTableColumns(this SqliteProvider provider, string tableName, IEnumerable<SqliteColumnMapping> columnsMustHave, out string message)
        {
            message = string.Empty;
            if (columnsMustHave == null || columnsMustHave.Count() == 0)
                return true;
            if (provider == null || string.IsNullOrWhiteSpace(tableName))
            {
                message = "基础操作对象或表名为空";
                return false;
            }

            //尝试新建表，假如尝试失败，或者原本表不存在但是新建成功，则退出，否则继续添加字段
            bool tableExists = provider.CheckForTable(tableName, columnsMustHave, out DataTable table, out message);
            if (!tableExists || !string.IsNullOrWhiteSpace(message))
                return tableExists;
            //全部转为小写以进行比对
            List<string> currCols = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName.ToLower()).ToList(), fields = new List<string>(), sqls = new List<string>();
            foreach (var column in columnsMustHave)
            {
                if (currCols.Contains(column.ColumnName.ToLower()))
                    continue;
                fields.Add(column.ColumnName);
                sqls.Add(string.Format("alter table {0} add column {1};", tableName, column.Structure));
            }
            bool result = sqls.Count == 0 || provider.ExecuteSqlTrans(sqls);
            if (result)
                message = sqls.Count > 0 ? string.Format("已添加字段{0}", string.Join(", ", fields.ToArray()).TrimEnd(',', ' ').ToUpper()) : string.Empty;
            else
                message = "至少有一个字段添加失败";
            return result;
        }

        /// <summary>
        /// 根据给定表名获取所有记录，并按特定字段排序
        /// </summary>
        /// <param name="provider">SQLite基础操作类对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public static DataTable GetAllRecords(this SqliteProvider provider, string tableName, string orderby)
        {
            if (provider == null || string.IsNullOrWhiteSpace(tableName))
                return null;
            string sql = string.Format("select t.*, 0 changed from {0} t {1}", tableName, string.IsNullOrWhiteSpace(orderby) ? string.Empty : "order by t." + orderby);
            return provider.Query(sql);
        }
    }
}
