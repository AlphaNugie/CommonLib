using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// MySql数据库基础操作类
    /// </summary>
    public class MySqlProvider : DataProvider<MySqlConnection, MySqlDataAdapter, MySqlCommand, MySqlTransaction>
    {
        #region static
        /// <summary>
        /// MySql连接字符串模板
        /// 形如“Data Source=localhost; port=3306; Initial Catalog=xxx; Persist Security Info=True; user id=root; password=xxx;”，port, Charset, Persist Security Info可选，Persist Security Info=True则代表连接方法在数据库连接成功后保存密码信息，=False则不保存
        /// </summary>
        public const string ConnStrModel = @"
Data Source = {0};
port = {1};
Initial Catalog = {2};
Persist Security Info = True;
user id = {3};
password = {4};";

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="hostAddress">数据库主机地址</param>
        /// <param name="hostPort">数据库主机端口</param>
        /// <param name="serviceName">数据库名称</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回连接字符串</returns>
        public static string GetConnStr(string hostAddress, int hostPort, string serviceName, string userName, string password)
        {
            return GetConnStr(ConnStrModel, hostAddress, hostPort, serviceName, userName, password);
            //return string.Format(ConnStrModel, hostAddress, hostPort, serviceName, userName, password);
        }

        ///// <summary>
        ///// 测试数据库连接是否正常（能够连接）
        ///// </summary>
        ///// <param name="connStr">连接字符串</param>
        ///// <returns>假如能够连接，返回true，否则返回false</returns>
        //public static bool IsConnOpen(string connStr)
        //{
        //    return IsConnOpen(connStr);
        //    //using (var connection = new MySqlConnection(connStr))
        //    //{
        //    //    ConnectionState state = ConnectionState.Closed;
        //    //    try
        //    //    {
        //    //        connection.Open();
        //    //        state = connection.State;
        //    //    }
        //    //    catch (Exception) { state = ConnectionState.Closed; }
        //    //    return state == ConnectionState.Open;
        //    //}
        //}
        #endregion

        ///// <summary>
        ///// 数据库连接字符串，形如“Data Source=localhost; port=3306; Initial Catalog=xxx; Persist Security Info=True; user id=root; password=xxx;”，port, Charset, Persist Security Info可选，Persist Security Info=True则代表连接方法在数据库连接成功后保存密码信息
        ///// </summary>
        //public new string ConnStr { get; private set; }

        #region 构造器
        /// <summary>
        /// 以默认配置初始化MySqlProvider
        /// </summary>
        public MySqlProvider() : this(ConfigurationManager.AppSettings["MySqlClient"]) { }

        /// <summary>
        /// 用App.config配置项名称初始化
        /// </summary>
        /// <param name="configurationName">App.config文件中configuration/appSettings节点下的关键字名称</param>
        /// <param name="_">充数的参数，防止签名一致</param>
        public MySqlProvider(string configurationName, object _) : base(configurationName, _) { }
        //public MySqlProvider(string configurationName, object _) : this(ConfigurationManager.AppSettings[configurationName]) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="connStr">连接字符串，形如“Data Source=localhost; port=3306; Initial Catalog=xxx; Persist Security Info=True; user id=root; password=xxx;”</param>
        public MySqlProvider(string connStr) : base(connStr) { }
        //{
        //    this.ConnStr = connStr;
        //}

        /// <summary>
        /// 根据给定的数据库相关信息初始化
        /// </summary>
        /// <param name="hostAddress">数据库地址</param>
        /// <param name="hostPort">端口号</param>
        /// <param name="serviceName">数据库服务名</param>
        /// <param name="userName">用户名称</param>
        /// <param name="password">用户密码</param>
        public MySqlProvider(string hostAddress, int hostPort, string serviceName, string userName, string password) : base(ConnStrModel, hostAddress, hostPort, serviceName, userName, password) { }
        #endregion

        #region without connstr as parameter
        ///// <summary>
        ///// 执行一条或多条SQL语句，返回查询结果集
        ///// </summary>
        ///// <param name="sqlStrings">包含SQL语句的字符串数组</param>
        ///// <returns>返回结果集</returns>
        //public DataSet MultiQuery(string[] sqlStrings)
        //{
        //    return this.MultiQuery(this.ConnStr, sqlStrings);
        //}

        ///// <summary>
        ///// 执行一条或多条SQL语句，返回查询结果集
        ///// </summary>
        ///// <param name="sqlStrings">执行的查询语句（假如需要执行多条，以“;”分隔）</param>
        ///// <returns>返回数据集</returns>
        //public DataSet MultiQuery(string sqlStrings)
        //{
        //    return this.MultiQuery(this.ConnStr, sqlStrings);
        //}

        ///// <summary>
        ///// 进行单条SQL语句查询，返回数据表
        ///// </summary>
        ///// <param name="sqlString">待执行的一条SQL语句</param>
        ///// <returns>返回数据表</returns>
        //public DataTable Query(string sqlString)
        //{
        //    return this.Query(this.ConnStr, sqlString);
        //}

        ///// <summary>
        ///// 执行SQL语句并返回受影响行数
        ///// </summary>
        ///// <param name="sqlString">待执行的SQL语句</param>
        ///// <returns>返回影响的记录行数</returns>
        //public int ExecuteSql(string sqlString)
        //{
        //    return this.ExecuteSql(this.ConnStr, sqlString);
        //}

        ///// <summary>
        ///// 执行多条SQL语句，实现数据事务
        ///// </summary>
        ///// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        ///// <param name="level">事务隔离（锁定）级别</param>
        ///// <returns>假如执行成功，返回true</returns>
        //public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings, IsolationLevel level)
        //{
        //    return this.ExecuteSqlTrans(this.ConnStr, sqlStrings, level);
        //}

        ///// <summary>
        ///// 执行一条或多条SQL语句，实现数据事务
        ///// </summary>
        ///// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        ///// <returns>假如执行成功，返回true</returns>
        //public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings)
        //{
        //    return this.ExecuteSqlTrans(this.ConnStr, sqlStrings);
        //}

        ///// <summary>
        ///// 执行一条或多条SQL语句，实现数据事务
        ///// </summary>
        ///// <param name="sqlStrings">SQL语句拼接成的字符串，SQL语句以分号“;”分隔</param>
        ///// <returns>假如执行成功，返回true</returns>
        //public bool ExecuteSqlTrans(string sqlStrings)
        //{
        //    return this.ExecuteSqlTrans(this.ConnStr, sqlStrings);
        //}

        ///// <summary>
        ///// 执行存储过程
        ///// </summary>
        ///// <param name="procedureName">存储过程名称</param>
        ///// <param name="parameters">存储过程参数</param>
        ///// <returns></returns>
        //public int RunProcedure(string procedureName, IDataParameter[] parameters)
        //{
        //    return this.RunProcedure(this.ConnStr, procedureName, parameters);
        //}
        #endregion
    }
}