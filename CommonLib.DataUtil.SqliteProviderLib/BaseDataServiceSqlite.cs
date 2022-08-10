using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// 基础Sqlite数据服务类
    /// </summary>
    public abstract class BaseDataServiceSqlite
    {
        private SqliteProvider _provider;
        //private string _tableName;

        /// <summary>
        /// SQLite数据库操作对象
        /// </summary>
        public SqliteProvider Provider { get => _provider; }

        /// <summary>
        /// 对应表名称
        /// </summary>
        public string TableName { get; protected set; }

        /// <summary>
        /// 必须具备的字段
        /// </summary>
        public List<SqliteColumnMapping> ColumnsMustHave { get; private set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public BaseDataServiceSqlite(string path, string name)
        {
            //ColumnsMustHave = new List<SqliteColumnMapping>();
            SetFilePath(path, name);
            //if (string.IsNullOrWhiteSpace(name))
            //    throw new ArgumentNullException(nameof(name), "未指定Sqlite文件名称");
            //_provider = new SqliteProvider(path, name);
            TableName = GetTableName();
            ColumnsMustHave = GetColumnsMustHave();
        }

        /// <summary>
        /// 获取并返回当前表的名称
        /// </summary>
        protected abstract string GetTableName();

        /// <summary>
        /// 获取并返回包含必须存在的字段的列表
        /// </summary>
        protected abstract List<SqliteColumnMapping> GetColumnsMustHave();

        /// <summary>
        /// 设置Sqlite文件的完整路径
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <exception cref="ArgumentException">未指定Sqlite文件完整路径</exception>
        public void SetFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException(nameof(filePath), "未指定Sqlite文件完整路径");
            _provider = new SqliteProvider(filePath);
        }

        /// <summary>
        /// 设置Sqlite文件的路径与路径下文件名称
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="name">文件名称</param>
        /// <exception cref="ArgumentNullException">未指定Sqlite文件名称</exception>
        public void SetFilePath(string path, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "未指定Sqlite文件名称");
            _provider = new SqliteProvider(path, name);
        }

        /// <summary>
        /// 检查数据表的字段，假如缺少字段则增加
        /// </summary>
        public bool CheckForTableColumns(out string message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(TableName) || ColumnsMustHave == null || ColumnsMustHave.Count == 0)
                return true;

            //string sql = "select * from " + TableName;
            //DataTable table = Provider.Query(sql);
            //尝试新建表，假如尝试失败，或者原本表不存在但是新建成功，则退出，否则继续添加字段
            bool tableExists = CheckForTable(out DataTable table, out message);
            if (!tableExists || !string.IsNullOrWhiteSpace(message))
                return tableExists;
            //全部转为小写以进行比对
            List<string> currCols = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName.ToLower()).ToList(), fields = new List<string>(), sqls = new List<string>();
            ColumnsMustHave.ForEach(column =>
            {
                if (currCols.Contains(column.ColumnName.ToLower()))
                    return;
                fields.Add(column.ColumnName);
                sqls.Add(string.Format("alter table {0} add column {1};", TableName, column.Structure));
                //sqls.Add(string.Format("alter table {0} add column {1};", TableName, column.GetStructure()));
                //sqls.Add(string.Format("alter table {0} add column {1} {2} {3}{4}{5};", TableName, column.ColumnName, column.SqlType, column.NotNull ? "NOT NULL " : string.Empty, column.OnConflictFail ? "ON CONFLICT FAIL " : string.Empty, column.DefaultValue != null ? string.Format("DEFAULT {0}", column.DefaultValue.Value) : string.Empty));
            });
            bool result = sqls.Count == 0 || Provider.ExecuteSqlTrans(sqls);
            if (result)
                message = sqls.Count > 0 ? string.Format("已添加字段{0}", string.Join(", ", fields.ToArray()).TrimEnd(',', ' ').ToUpper()) : string.Empty;
            else
                message = "至少有一个字段添加失败";
            return result;
        }

        /// <summary>
        /// 判断
        /// </summary>
        /// <returns></returns>
        public bool TableExists(out DataTable table)
        {
            string sql = "select * from " + TableName;
            table = null;
            try
            {
                table = Provider.Query(sql);
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// 检查数据表是否存在，不存在则新增
        /// </summary>
        /// <param name="table"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool CheckForTable(out DataTable table, out string message)
        {
            table = null;
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(TableName) || ColumnsMustHave == null || ColumnsMustHave.Count == 0)
                return true;
            //QUERY:
            //string sql = "select * from " + TableName;
            //try
            //{
            //    table = Provider.Query(sql);
            //    return true;
            //}
            //catch (Exception) { }
            if (TableExists(out table))
                return true;
            string columns = string.Join(string.Empty, ColumnsMustHave.Select(column => string.Format("\r\n  {0},", column.GetStructure(true)))).TrimEnd(',');
            string sqlString = string.Format("create table [{0}] ({1})", TableName, columns);
            Provider.ExecuteSql(sqlString);
            bool result = TableExists(out table);
            message = result ? string.Format("已添加表{0}及其{1}个字段", TableName, ColumnsMustHave.Count) : string.Format("表{0}添加失败", TableName);
            //if (result)
            //{
            //    message = string.Format("已添加表{0}及其{1}个字段", TableName, ColumnsMustHave.Count);
            //    //goto QUERY;
            //}
            //else
            //    message = string.Format("表{0}添加失败", TableName);
            return result;
        }

        /// <summary>
        /// 获取所有记录，并按特定字段排序
        /// </summary>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetAllRecords(string orderby)
        {
            string sql = string.Format("select t.*, 0 changed from {0} t {1}", TableName, string.IsNullOrWhiteSpace(orderby) ? string.Empty : "order by t." + orderby);
            return Provider.Query(sql);
        }
    }
}
