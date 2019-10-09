using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;
using CommonLib.Enums;

namespace CommonLib.DataUtil
{
    /// <summary>  
    /// 数据访问抽象基础类  
    /// </summary>  
    public class MySqlProvider
    {
        private readonly DataProvider dataProvider = new DataProvider();

        /// <summary>
        /// 数据库连接字符串，形如“Data Source=localhost; port=3306; Initial Catalog=xxx; Persist Security Info=True; user id=root; password=xxx;”
        /// port, Charset, Persist Security Info可选，Persist Security Info=True则代表连接方法在数据库连接成功后保存密码信息
        /// </summary>
        public string ConnStr { get; private set; }

        /// <summary>
        /// 以默认配置初始化MySqlProvider
        /// </summary>
        public MySqlProvider() : this(ConfigurationManager.AppSettings["MySqlClient"]) { }

        /// <summary>
        /// 用MySql配置项名称初始化MySqlProvider
        /// </summary>
        /// <param name="configurationName">项目在App.config文件中appSettings节点下的关键字名称</param>
        /// <param name="_">充数的参数，防止签名一致</param>
        public MySqlProvider(string configurationName, object _) : this(ConfigurationManager.AppSettings[configurationName]) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="connStr">连接字符串，形如“Data Source=localhost; port=3306; Initial Catalog=xxx; Persist Security Info=True; user id=root; password=xxx;”</param>
        public MySqlProvider(string connStr)
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
            return this.dataProvider.MultiQuery(DatabaseTypes.MySql, this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，返回查询结果集
        /// </summary>
        /// <param name="sqlStrings">执行的查询语句（假如需要执行多条，以“;”分隔）</param>
        /// <returns>返回数据集</returns>
        public DataSet MultiQuery(string sqlStrings)
        {
            return this.dataProvider.MultiQuery(DatabaseTypes.MySql, this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 进行单条SQL语句查询，返回数据表
        /// </summary>
        /// <param name="sqlString">待执行的一条SQL语句</param>
        /// <returns>返回数据表</returns>
        public DataTable Query(string sqlString)
        {
            return this.dataProvider.Query(DatabaseTypes.MySql, this.ConnStr, sqlString);
        }

        /// <summary>
        /// 执行SQL语句并返回受影响行数
        /// </summary>
        /// <param name="sqlString">待执行的SQL语句</param>
        /// <returns>返回影响的记录行数</returns>
        public int ExecuteSql(string sqlString)
        {
            return this.dataProvider.ExecuteSql(DatabaseTypes.MySql, this.ConnStr, sqlString);
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <param name="level">事务隔离（锁定）级别</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings, IsolationLevel level)
        {
            return this.dataProvider.ExecuteSqlTrans(DatabaseTypes.MySql, this.ConnStr, sqlStrings, level);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings)
        {
            return this.dataProvider.ExecuteSqlTrans(DatabaseTypes.MySql, this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">SQL语句拼接成的字符串，SQL语句以分号“;”分隔</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(string sqlStrings)
        {
            return this.dataProvider.ExecuteSqlTrans(DatabaseTypes.MySql, this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns></returns>
        public int RunProcedure(string procedureName, IDataParameter[] parameters)
        {
            return this.dataProvider.RunProcedure(DatabaseTypes.MySql, this.ConnStr, procedureName, parameters);
        }
    }
}