using CommonLib.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// 基础Sqlite数据服务类
    /// </summary>
    public abstract class BaseDataServiceSqlite
    {
        /// <summary>
        /// SQLite基础操作类的对象
        /// </summary>
        protected SqliteProvider _provider;
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

        #region CheckForTableColumns
        /// <summary>
        /// 检查数据表的字段，假如缺少字段则增加
        /// </summary>
        public virtual bool CheckForTableColumns()
        {
            return _provider.CheckForTableColumns(TableName, ColumnsMustHave, out _);
            //return CheckForTableColumns(out _);
        }

        /// <summary>
        /// 根据设置的表名及列信息检查数据表的字段，假如表不存在先新增表，然后根据给定列判断字段是否存在，不存在则新增列，最终返回操作结果
        /// </summary>
        public virtual bool CheckForTableColumns(out string message)
        {
            return _provider.CheckForTableColumns(TableName, ColumnsMustHave, out message);
        }

        /// <summary>
        /// 判断
        /// </summary>
        /// <returns></returns>
        public bool TableExists(out DataTable table)
        {
            return _provider.TableExists(TableName, out table);
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
        }

        /// <summary>
        /// 查找指定命名空间下继承BaseDataServiceBatisSqlite的符合给定条件的所有子类，并执行其中的CheckForTableColumns方法（无参）
        /// </summary>
        /// <param name="nameSpace">命名空间全名（或一部分），区分大小写</param>
        /// <param name="subSpaceIncl">是否查找子命名空间</param>
        /// <param name="typeNameIncl">查找时限定类名的一部分，假如为空则不限定</param>
        ///// <param name="baseType">查找类时限定的从中继承的类（仅检查类型名称及命名空间是否相同），假如为空则不限定</param>
        public static void CallMethodForWholeShebang_CheckForTableColumns(string nameSpace, bool subSpaceIncl = false, string typeNameIncl = null/*, Type baseType = null*/)
        {
            var serviceTypes = Assembly.GetEntryAssembly().GetTypesInNamespace(nameSpace, subSpaceIncl, typeNameIncl, typeof(BaseDataServiceSqlite));
            if (serviceTypes != null && serviceTypes.Length > 0)
            {
                foreach (var type in serviceTypes)
                {
                    var obj = Activator.CreateInstance(type);
                    MethodInfo checkMethod = type.GetMethod("CheckForTableColumns", Type.EmptyTypes);
                    checkMethod.Invoke(obj, null);
                }
            }
        }

        /// <summary>
        /// 查找与当前类相同命名空间下继承BaseDataServiceBatisSqlite的符合给定条件的所有子类，并执行其中的CheckForTableColumns方法（无参）
        /// </summary>
        /// <param name="subSpaceIncl">是否查找子命名空间</param>
        /// <param name="typeNameIncl">查找时限定类名的一部分，假如为空则不限定</param>
        public void CallMethodForWholeShebang_CheckForTableColumns(bool subSpaceIncl = false, string typeNameIncl = null)
        {
            var type = GetType();
            CallMethodForWholeShebang_CheckForTableColumns(type.Namespace, subSpaceIncl, typeNameIncl);
        }
        #endregion
    }
}
