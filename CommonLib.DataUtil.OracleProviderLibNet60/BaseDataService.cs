using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Enums;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// Oracle基础数据库操作类（仿Batis）
    /// </summary>
    public class BaseDataService<T> : BaseDataServiceBatis<T> where T : BaseModel
    {
        /// <summary>
        /// Oracle基础操作类对象
        /// </summary>
        protected OracleProvider? _provider/* = new OracleProvider()*/;

        /// <summary>
        /// 数据库操作类构造器，使用App.config文件中"OracleClient"配置项内容初始化
        /// </summary>
        public BaseDataService() : base(string.Empty, false) { }

        /// <summary>
        /// 数据库操作类构造器，使用给定的连接字符串初始化
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        public BaseDataService(string connStr) : base(connStr, true) { }

        /// <summary>
        /// 数据库操作类构造器，在连接字符串不为空且使用远程数据库时用连接字符串初始化，否则使用"OracleClient"配置项
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="usingRemote">是否使用远程数据库（而非当前使用的数据库）</param>
        public BaseDataService(string connStr, bool usingRemote) : base(connStr, usingRemote) { }

        /// <summary>
        /// 初始化Provider对象，在连接字符串不为空且使用远程数据库时用连接字符串初始化，否则使用App.config文件中"OracleClient"配置项内容初始化
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="usingRemote">是否使用远程数据库（而非当前使用的数据库）</param>
        public override void InitProviderInstance(string connStr, bool usingRemote)
        {
            _provider = !string.IsNullOrWhiteSpace(connStr) && usingRemote ? new OracleProvider(connStr) : new OracleProvider();
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
    }
}
