using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using CommonLib.Clients;
using CommonLib.Enums;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// Oracle数据库基础操作类
    /// </summary>
    public class OracleProvider
    {
        private DataProvider dataProvider = new DataProvider();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnStr { get; private set; }

        /// <summary>
        /// 以默认配置初始化OracleProvider
        /// </summary>
        public OracleProvider() : this(ConfigurationManager.AppSettings["OracleClient"]) { }

        /// <summary>
        /// 用Oracle配置项名称初始化OracleProvider
        /// </summary>
        /// <param name="configurationName">项目在App.config文件中appSettings节点下的关键字名称</param>
        /// <param name="_">充数的参数，防止签名一致</param>
        public OracleProvider(string configurationName, object _) : this(ConfigurationManager.AppSettings[configurationName]) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        public OracleProvider(string connStr)
        {
            this.ConnStr = connStr;
        }

        /// <summary>
        /// 执行一条或多条SQL语句，返回查询结果集
        /// </summary>
        /// <param name="sqlStrings">包含SQL语句的字符串数组</param>
        /// <returns>返回结果集</returns>
        public DataSet MultiQuery(string[] sqlStrings)
        {
            return this.dataProvider.MultiQuery(DatabaseTypes.Oracle, this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，返回查询结果集
        /// </summary>
        /// <param name="sqlStrings">执行的查询语句（假如需要执行多条，以“;”分隔）</param>
        /// <returns>返回数据集</returns>
        public DataSet MultiQuery(string sqlStrings)
        {
            return this.dataProvider.MultiQuery(DatabaseTypes.Oracle, this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 进行单条SQL语句查询，返回数据表
        /// </summary>
        /// <param name="sqlString">待执行的一条SQL语句</param>
        /// <returns>返回数据表</returns>
        public DataTable Query(string sqlString)
        {
            return this.dataProvider.Query(DatabaseTypes.Oracle, this.ConnStr, sqlString);
        }

        /// <summary>
        /// 执行SQL语句并返回受影响行数
        /// </summary>
        /// <param name="sqlString">待执行的SQL语句</param>
        /// <returns>返回影响的记录行数</returns>
        public int ExecuteSql(string sqlString)
        {
            return this.dataProvider.ExecuteSql(DatabaseTypes.Oracle, this.ConnStr, sqlString);
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <param name="level">事务隔离（锁定）级别</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings, IsolationLevel level)
        {
            return this.dataProvider.ExecuteSqlTrans(DatabaseTypes.Oracle, this.ConnStr, sqlStrings, level);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings)
        {
            return this.dataProvider.ExecuteSqlTrans(DatabaseTypes.Oracle, this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">SQL语句拼接成的字符串，SQL语句以分号“;”分隔</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(string sqlStrings)
        {
            return this.dataProvider.ExecuteSqlTrans(DatabaseTypes.Oracle, this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns></returns>
        public int RunProcedure(string procedureName, IDataParameter[] parameters)
        {
            return this.dataProvider.RunProcedure(DatabaseTypes.Oracle, this.ConnStr, procedureName, parameters);
        }
    }
}
