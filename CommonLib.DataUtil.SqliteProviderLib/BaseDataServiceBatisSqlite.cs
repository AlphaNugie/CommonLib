using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        /// <summary>
        /// 检查数据表的字段，假如缺少字段则增加
        /// </summary>
        public virtual bool CheckForTableColumns(out string message)
        {
            return _provider.CheckForTableColumns(TableName, ColumnsMustHave, out message);
        }
    }
}
