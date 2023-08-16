using CommonLib.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// SQLite基础数据库操作类（仿Batis）
    /// </summary>
    public abstract class BaseDataServiceBatisSqlite<T> : BaseDataServiceBatis<T> where T : BaseModel
    {
        /// <summary>
        /// SQLite基础操作类对象
        /// </summary>
        protected SqliteProvider _provider;

        #region 属性
        /// <summary>
        /// 对应表的名称
        /// </summary>
        //public string TableName { get; protected set; }
        public string TableName { get { return GetTableName(); } }

        /// <summary>
        /// 必须具备的字段
        /// </summary>
        public List<SqliteColumnMapping> ColumnsMustHave { get; private set; }
        #endregion

        #region 构造器
        /// <summary>
        /// 数据库操作类构造器，使用App.config文件中"SqliteClient"配置项内容初始化
        /// </summary>
        public BaseDataServiceBatisSqlite() : this(string.Empty, false) { }
        //public BaseDataServiceBatisSqlite() : base(string.Empty, false) { }

        /// <summary>
        /// 数据库操作类构造器，使用给定的数据库文件完整路径初始化
        /// </summary>
        /// <param name="filePath">数据库文件路径+名称（包括后缀），假如为不包含盘符的相对路径（不包括..\），则添加启动路径成为绝对路径（假如为空输出空字符串）</param>
        public BaseDataServiceBatisSqlite(string filePath) : this(filePath, true) { }
        //public BaseDataServiceBatisSqlite(string filePath) : base(SqliteProvider.GetConnStr(filePath), true) { }

        /// <summary>
        /// 数据库操作类构造器，在连接字符串不为空且使用远程数据库时用连接字符串初始化，否则使用"SqliteClient"配置项
        /// </summary>
        /// <param name="filePath">数据库文件路径+名称（包括后缀），假如为不包含盘符的相对路径（不包括..\），则添加启动路径成为绝对路径（假如为空输出空字符串）</param>
        /// <param name="usingRemote">是否使用远程数据库（而非当前使用的数据库）</param>
        public BaseDataServiceBatisSqlite(string filePath, bool usingRemote) : base(SqliteProvider.GetConnStr(filePath), usingRemote)
        {
            //TableName = GetTableName();
            ColumnsMustHave = GetColumnsMustHave();
        }

        /// <summary>
        /// 数据库操作类构造器，使用给定的数据库文件完整路径初始化
        /// </summary>
        /// <param name="fileDir">数据库文件路径，假如为不包含盘符的相对路径（不包括..\），则添加启动路径成为绝对路径</param>
        /// <param name="fileName">数据库文件名称，包括后缀</param>
        public BaseDataServiceBatisSqlite(string fileDir, string fileName) : this(fileDir, fileName, true) { }
        //public BaseDataServiceBatisSqlite(string fileDir, string fileName) : base(SqliteProvider.GetConnStr(fileDir, fileName), true) { }

        /// <summary>
        /// 数据库操作类构造器，在连接字符串不为空且使用远程数据库时用连接字符串初始化，否则使用"SqliteClient"配置项
        /// </summary>
        /// <param name="fileDir">数据库文件路径，假如为不包含盘符的相对路径（不包括..\），则添加启动路径成为绝对路径</param>
        /// <param name="fileName">数据库文件名称，包括后缀</param>
        /// <param name="usingRemote">是否使用远程数据库（而非当前使用的数据库）</param>
        public BaseDataServiceBatisSqlite(string fileDir, string fileName, bool usingRemote) : base(SqliteProvider.GetConnStr(fileDir, fileName), usingRemote)
        {
            //TableName = GetTableName();
            ColumnsMustHave = GetColumnsMustHave();
        }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 获取并返回当前表的名称
        /// </summary>
        protected abstract string GetTableName();

        /// <summary>
        /// 获取并返回包含必须存在的字段的列表
        /// </summary>
        protected abstract List<SqliteColumnMapping> GetColumnsMustHave();
        #endregion

        #region 覆写父类方法
        /// <summary>
        /// 初始化Provider对象，在连接字符串不为空且使用远程数据库时用连接字符串初始化，否则使用App.config文件中"SqliteClient"配置项内容初始化
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="usingRemote">是否使用远程数据库（而非当前使用的数据库）</param>
        public override void InitProviderInstance(string connStr, bool usingRemote)
        {
            //_provider = !string.IsNullOrWhiteSpace(connStr) && usingRemote ? new SqliteProvider(connStr) : new SqliteProvider();
            _provider = new SqliteProvider();
            if (!string.IsNullOrWhiteSpace(connStr) && usingRemote)
                _provider.SetConnStrDirectly(connStr);
        }

        /// <inheritdoc/>
        public override int ProviderExecuteSql(string sqlString)
        {
            return _provider.ExecuteSql(sqlString);
        }

        /// <inheritdoc/>
        public override bool ProviderExecuteSqlTrans(IEnumerable<string> sqlStrings)
        {
            return _provider.ExecuteSqlTrans(sqlStrings);
        }

        /// <inheritdoc/>
        public override DataTable ProviderQuery(string sqlString)
        {
            return _provider.Query(sqlString);
        }
        #endregion

        #region CheckForTableColumns
        /// <summary>
        /// 检查数据表的字段，假如缺少字段则增加
        /// </summary>
        public virtual bool CheckForTableColumns()
        {
            return _provider.CheckForTableColumns(TableName, ColumnsMustHave, out _);
        }

        /// <summary>
        /// 检查数据表的字段，假如缺少字段则增加
        /// </summary>
        public virtual bool CheckForTableColumns(out string message)
        {
            return _provider.CheckForTableColumns(TableName, ColumnsMustHave, out message);
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
            var serviceTypes = Assembly.GetEntryAssembly().GetTypesInNamespace(nameSpace, subSpaceIncl, typeNameIncl, typeof(BaseDataServiceBatisSqlite<T>));
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
