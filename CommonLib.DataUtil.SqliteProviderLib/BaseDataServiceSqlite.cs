using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        #region 属性
        /// <summary>
        /// SQLite数据库操作对象
        /// </summary>
        public SqliteProvider Provider { get => _provider; }

        /// <summary>
        /// 对应表名称
        /// </summary>
        //public string TableName { get; protected set; }
        public string TableName { get { return GetTableName(); } }

        /// <summary>
        /// 必须具备的字段
        /// </summary>
        public List<SqliteColumnMapping> ColumnsMustHave { get; private set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public BaseDataServiceSqlite(string path, string name)
        {
            //ColumnsMustHave = new List<SqliteColumnMapping>();
            SetFilePath(path, name);
            ////if (string.IsNullOrWhiteSpace(name))
            ////    throw new ArgumentNullException(nameof(name), "未指定Sqlite文件名称");
            ////_provider = new SqliteProvider(path, name);
            //TableName = GetTableName();
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
            if (_provider == null)
                _provider = new SqliteProvider(filePath);
            else
                _provider.SetConnStr(filePath);
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
            if (_provider == null)
                _provider = new SqliteProvider(path, name);
            else
                _provider.SetConnStr(path, name);
        }

        /// <summary>
        /// 获取所有数据库字段信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public List<SqliteColumnMapping> GetAllColumnMappings(out string message)
        {
            return _provider.GetColumnMappings(TableName, null, out message);
            //return GetColumnMappings(null, out message);
        }

        /// <summary>
        /// 根据给定的数据库字段列表获取数据库字段信息，假如列表为空则获取所有字段
        /// </summary>
        /// <param name="columnNames">给定的数据库字段列表，假如为空则获取所有字段</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public List<SqliteColumnMapping> GetColumnMappings(IEnumerable<string> columnNames, out string message)
        {
            return _provider.GetColumnMappings(TableName, columnNames, out message);
        //    message = string.Empty;
        //    //List<SqliteColumnMapping> columns = new List<SqliteColumnMapping>();
        //    List<SqliteColumnMapping> columns = null;
        //    string sqlString = $"pragma table_info('{TableName}')";
        //    DataTable table = _provider.Query(sqlString);
        //    if (table == null || table.Rows.Count == 0)
        //    {
        //        message = $"未找到表{TableName}的任何列信息";
        //        goto END;
        //    }
        //    //获取所有列信息，假如给定的列名不为空，则筛选出对应列
        //    columns = table.Rows.Cast<DataRow>().Select(dataRow => new SqliteColumnMapping(dataRow)).ToList();
        //    if (columnNames == null || columnNames.Count() == 0)
        //        goto END;
        //    columns = columns.Where(column => columnNames.Contains(column.ColumnName)).ToList();
        //END:
        //    return columns;
        }

        /// <summary>
        /// 检查数据表的字段，假如缺少字段则增加
        /// </summary>
        public virtual bool CheckForTableColumns()
        {
            return _provider.CheckForTableColumns(TableName, ColumnsMustHave, out _);
            //return CheckForTableColumns(out _);
        }

        /// <summary>
        /// 检查数据表的字段，假如缺少字段则增加
        /// </summary>
        public virtual bool CheckForTableColumns(out string message)
        {
            return _provider.CheckForTableColumns(TableName, ColumnsMustHave, out message);
            //message = string.Empty;
            //if (string.IsNullOrWhiteSpace(TableName) || ColumnsMustHave == null || ColumnsMustHave.Count == 0)
            //    return true;

            ////string sql = "select * from " + TableName;
            ////DataTable table = Provider.Query(sql);
            ////尝试新建表，假如尝试失败，或者原本表不存在但是新建成功，则退出，否则继续添加字段
            //bool tableExists = CheckForTable(out DataTable table, out message);
            //if (!tableExists || !string.IsNullOrWhiteSpace(message))
            //    return tableExists;
            ////全部转为小写以进行比对
            //List<string> currCols = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName.ToLower()).ToList(), fields = new List<string>(), sqls = new List<string>();
            //ColumnsMustHave.ForEach(column =>
            //{
            //    if (currCols.Contains(column.ColumnName.ToLower()))
            //        return;
            //    fields.Add(column.ColumnName);
            //    sqls.Add(string.Format("alter table {0} add column {1};", TableName, column.Structure));
            //    //sqls.Add(string.Format("alter table {0} add column {1};", TableName, column.GetStructure()));
            //    //sqls.Add(string.Format("alter table {0} add column {1} {2} {3}{4}{5};", TableName, column.ColumnName, column.SqlType, column.NotNull ? "NOT NULL " : string.Empty, column.OnConflictFail ? "ON CONFLICT FAIL " : string.Empty, column.DefaultValue != null ? string.Format("DEFAULT {0}", column.DefaultValue.Value) : string.Empty));
            //});
            //bool result = sqls.Count == 0 || Provider.ExecuteSqlTrans(sqls);
            //if (result)
            //    message = sqls.Count > 0 ? string.Format("已添加字段{0}", string.Join(", ", fields.ToArray()).TrimEnd(',', ' ').ToUpper()) : string.Empty;
            //else
            //    message = "至少有一个字段添加失败";
            //return result;
        }

        /// <summary>
        /// 判断
        /// </summary>
        /// <returns></returns>
        public bool TableExists(out DataTable table)
        {
            return _provider.TableExists(TableName, out table);
            //string sql = "select * from " + TableName;
            //table = null;
            //try
            //{
            //    table = Provider.Query(sql);
            //    return true;
            //}
            //catch (Exception) { return false; }
        }

        /// <summary>
        /// 检查数据表是否存在，不存在则新增
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool CheckForTable(out DataTable table)
        {
            return _provider.CheckForTable(TableName, ColumnsMustHave, out table, out _);
            //return CheckForTable(out table, out _);
        }

        /// <summary>
        /// 检查数据表是否存在，不存在则新增
        /// </summary>
        /// <param name="table"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool CheckForTable(out DataTable table, out string message)
        {
            return _provider.CheckForTable(TableName, ColumnsMustHave, out table, out message);
            //table = null;
            //message = string.Empty;
            //if (string.IsNullOrWhiteSpace(TableName) || ColumnsMustHave == null || ColumnsMustHave.Count == 0)
            //    return true;
            //if (TableExists(out table))
            //    return true;
            //string columns = string.Join(string.Empty, ColumnsMustHave.Select(column => string.Format("\r\n  {0},", column.GetStructure(true)))).TrimEnd(',');
            //string sqlString = string.Format("create table [{0}] ({1})", TableName, columns);
            //Provider.ExecuteSql(sqlString);
            //bool result = TableExists(out table);
            //message = result ? string.Format("已添加表{0}及其{1}个字段", TableName, ColumnsMustHave.Count) : string.Format("表{0}添加失败", TableName);
            //return result;
        }

        /// <summary>
        /// 获取所有记录，并按特定字段排序
        /// </summary>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetAllRecords(string orderby)
        {
            return _provider.GetAllRecords(TableName, orderby);
            //string sql = string.Format("select t.*, 0 changed from {0} t {1}", TableName, string.IsNullOrWhiteSpace(orderby) ? string.Empty : "order by t." + orderby);
            //return Provider.Query(sql);
        }
    }
}
