using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// 与特定数据库连接相关的数据库操作类
    /// </summary>
    /// <typeparam name="Connection">数据库连接类型</typeparam>
    /// <typeparam name="Adapter">用于更新DataSet、更新数据源的类型</typeparam>
    /// <typeparam name="Command">数据库命令类</typeparam>
    /// <typeparam name="Transaction">事务类</typeparam>
    public class DataProvider<Connection, Adapter, Command, Transaction>
        where Connection : DbConnection, IDisposable
        where Adapter : DbDataAdapter, IDisposable
        where Command : DbCommand, IDisposable
        where Transaction : DbTransaction, IDisposable
    {
        #region static
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="connStr">连接字符串模板，按顺序为后面5个参数留出替换格式项（如{0}, {1}）</param>
        /// <param name="hostAddress">数据库主机地址</param>
        /// <param name="hostPort">数据库主机端口</param>
        /// <param name="serviceName">数据库名称</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回连接字符串</returns>
        public static string GetConnStr(string connStr, string hostAddress, int hostPort, string serviceName, string userName, string password)
        {
            return string.Format(connStr, hostAddress, hostPort, serviceName, userName, password);
        }

        /// <summary>
        /// 测试以给定的连接字符串描述的数据库连接是否正常（能够连接）
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <returns>假如能够连接，返回true，否则返回false</returns>
        public static bool IsConnOpen(string connStr)
        {
            using (var connection = (Connection)Activator.CreateInstance(typeof(Connection), connStr))
            {
                ConnectionState state = ConnectionState.Closed;
                try
                {
                    connection.Open();
                    state = connection.State;
                }
                catch (Exception) { state = ConnectionState.Closed; }
                return state == ConnectionState.Open;
            }
        }
        #endregion

        /// <summary>
        /// 数据库连接字符串
        /// Oracle形如“Data Source=ORCL1;User Id=test;Password=123;”，其中ORCL1可由具体的(DESCRIPTION...)代替
        /// </summary>
        public string ConnStr { get; protected set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; protected set; }

        #region 构造器
        /// <summary>
        /// 用App.config配置项名称初始化
        /// </summary>
        /// <param name="configurationName">App.config文件中configuration/appSettings节点下的关键字名称</param>
        /// <param name="_">充数的参数，防止签名一致</param>
        public DataProvider(string configurationName, object _) : this(ConfigurationManager.AppSettings[configurationName]) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        public DataProvider(string connStr)
        {
            this.ConnStr = connStr;
        }

        /// <summary>
        /// 根据给定的连接字符串模板与数据库相关信息初始化
        /// </summary>
        /// <param name="connStr">连接字符串模板，按顺序为后面5个参数留出替换格式项（如{0}, {1}）</param>
        /// <param name="hostAddress">数据库地址</param>
        /// <param name="hostPort">端口号</param>
        /// <param name="serviceName">数据库服务名</param>
        /// <param name="userName">用户名称</param>
        /// <param name="password">用户密码</param>
        public DataProvider(string connStr, string hostAddress, int hostPort, string serviceName, string userName, string password) : this(GetConnStr(connStr, hostAddress, hostPort, serviceName, userName, password)) { }
        #endregion

        #region with connstr as parameter
        /// <summary>
        /// 执行一条或多条SQL语句，返回查询结果集
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="sqlStrings">待执行的查询语句数组</param>
        /// <returns></returns>
        public DataSet MultiQuery(string connStr, IEnumerable<string> sqlStrings)
        {
            //假如字符数组为空，返回空
            if (sqlStrings == null || sqlStrings.Count() == 0)
                return null;

            using (var connection = (Connection)Activator.CreateInstance(typeof(Connection), connStr))
            {
                using (var adapter = (Adapter)Activator.CreateInstance(typeof(Adapter), string.Empty, connection))
                {
                    DataSet dataSet = new DataSet();
                    string _string = string.Empty;
                    try
                    {
                        //dataSet.EnforceConstraints = false; //禁用约束检查
                        connection.Open();
                        int i = 1;
                        foreach (var sqlString in sqlStrings)
                        {
                            _string = sqlString; //记录每个循环中的sql语句
                            //跳过空白字符串
                            if (string.IsNullOrWhiteSpace(sqlString))
                                continue;
                            adapter.SelectCommand.CommandText = sqlString.Trim(' ', ';'); //去除字符串前后的空格与分号，否则Oracle中报错（ORA-00911: 无效字符）
                            adapter.Fill(dataSet, string.Format("Table{0}", i++)); //执行SQL语句并填充以表名指定的DataTable，假如不存在将创建（为避免DataTable被覆盖，表名不要重复）
                        }
                    }
                    //假如出现异常，写入日志，重新抛出异常
                    catch (Exception e)
                    {
                        dataSet.Dispose();
                        this.ErrorMessage = string.Format("SQL语句执行出错: {0}, SQL语句: {1}", e.Message, _string);
                        throw; //假如不需要抛出异常，注释此行
                    }

                    return dataSet;
                }
            }
        }

        /// <summary>
        /// 执行一条或多条SQL语句，返回查询结果集
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="sqlStrings">待执行的多条查询语句（假如需要执行多条，以“;”分隔）</param>
        /// <returns>返回数据集</returns>
        public DataSet MultiQuery(string connStr, string sqlStrings)
        {
            string[] queries = sqlStrings.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries); //将源字符串拆分为字符串数组，忽略空字符串
            return this.MultiQuery(connStr, queries);
        }

        /// <summary>
        /// 执行sql语句，返回查询结果集DateTable
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="sqlString">执行的查询语句</param>
        /// <returns>返回查询结果数据表</returns>
        public DataTable Query(string connStr, string sqlString)
        {
            DataTable dataTable = null;
            using (DataSet dataSet = this.MultiQuery(connStr, new string[] { sqlString }))
            {
                if (dataSet != null && dataSet.Tables.Count > 0)
                    dataTable = dataSet.Tables[0];
                return dataTable;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="sqlString">执行的查询语句</param>
        /// <returns>返回影响的记录行数</returns>
        public int ExecuteSql(string connStr, string sqlString)
        {
            if (string.IsNullOrWhiteSpace(sqlString))
                return 0;

            using (var connection = (Connection)Activator.CreateInstance(typeof(Connection), connStr))
            {
                using (var command = (Command)Activator.CreateInstance(typeof(Command), sqlString, connection))
                {
                    sqlString = sqlString.Trim(' ', ';'); //去除SQL语句前后的空格与分号
                    int result = 0;
                    try
                    {
                        connection.Open();
                        result = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        result = 0;
                        this.ErrorMessage = string.Format("SQL语句执行出错: {0}, SQL语句: {1}", e.Message, sqlString);
                        throw; //假如不需要抛出异常，注释此行
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <param name="level">事务隔离（锁定）级别</param>
        /// <returns>假如所有语句执行成功，返回true，否则返回false</returns>
        public bool ExecuteSqlTrans(string connStr, IEnumerable<string> sqlStrings, IsolationLevel level)
        {
            //假如数组为空，返回false
            if (sqlStrings == null || sqlStrings.Count() == 0)
                return false;

            using (var connection = (Connection)Activator.CreateInstance(typeof(Connection), connStr))
            {
                using (var command = (Command)Activator.CreateInstance(typeof(Command), string.Empty, connection))
                {
                    bool result = false;
                    try { connection.Open(); }
                    catch (Exception e)
                    {
                        result = false;
                        this.ErrorMessage = string.Format("数据库连接打开失败: {0}", e.Message);
                        throw;
                    }

                    string sql = string.Empty;
                    //connection必须先Open
                    using (var transaction = (Transaction)connection.BeginTransaction(level))
                    {
                        try
                        {
                            command.Transaction = transaction;
                            foreach (string sqlString in sqlStrings)
                            {
                                //假如为空白字符串（移除空格与分号后），跳到下一次循环
                                //需去除字符串前后的空格和分号，否则Oracle中报错（ORA-00911: 无效字符）
                                sql = sqlString?.Trim(' ', ';');
                                if (string.IsNullOrWhiteSpace(sql))
                                    continue;
                                command.CommandText = sql;
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                            result = true;
                        }
                        catch (Exception e)
                        {
                            result = false;
                            transaction.Rollback();
                            this.ErrorMessage = string.Format("SQL语句事务执行失败: {0}, 执行失败的SQL语句: {1}", e.Message, sql);
                            throw; //假如不需要抛出异常，注释此行
                        }

                        return result;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(string connStr, IEnumerable<string> sqlStrings)
        {
            return this.ExecuteSqlTrans(connStr, sqlStrings, IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="sqlStrings">SQL语句拼接成的字符串，SQL语句以分号“;”分隔</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(string connStr, string sqlStrings)
        {
            //将源字符串拆分为字符串数组，忽略空字符串
            //假如字符串为空白字符串，数组为空
            string[] sqlArray = string.IsNullOrWhiteSpace(sqlStrings) ? null : sqlStrings.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            return this.ExecuteSqlTrans(connStr, sqlArray);
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数 
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="parameters">输入参数</param>
        /// <returns>返回影响的行数（？）</returns>
        public int RunProcedure(string connStr, string procedureName, IDataParameter[] parameters)
        {
            using (var connection = (Connection)Activator.CreateInstance(typeof(Connection), connStr))
            {
                using (var command = BuildCommand(connection, procedureName, parameters))
                {
                    int result = 0;
                    try
                    {
                        connection.Open();
                        result = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        result = 0;
                        this.ErrorMessage = string.Format("存储过程{0}执行失败: {1}", procedureName, e.Message);
                        throw; //假如不需要抛出异常，将此行注释
                    }

                    return result;
                }
            }
        }
        #endregion

        #region without connstr as parameter
        /// <summary>
        /// 测试当前数据库连接是否正常（能够连接）
        /// </summary>
        /// <returns></returns>
        public bool IsConnOpen()
        {
            return IsConnOpen(this.ConnStr);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，返回查询结果集
        /// </summary>
        /// <param name="sqlStrings">包含SQL语句的字符串数组</param>
        /// <returns>返回结果集</returns>
        public DataSet MultiQuery(string[] sqlStrings)
        {
            return this.MultiQuery(this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，返回查询结果集
        /// </summary>
        /// <param name="sqlStrings">执行的查询语句（假如需要执行多条，以“;”分隔）</param>
        /// <returns>返回数据集</returns>
        public DataSet MultiQuery(string sqlStrings)
        {
            return this.MultiQuery(this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 进行单条SQL语句查询，返回数据表
        /// </summary>
        /// <param name="sqlString">待执行的一条SQL语句</param>
        /// <returns>返回数据表</returns>
        public DataTable Query(string sqlString)
        {
            return this.Query(this.ConnStr, sqlString);
        }

        /// <summary>
        /// 执行SQL语句并返回受影响行数
        /// </summary>
        /// <param name="sqlString">待执行的SQL语句</param>
        /// <returns>返回影响的记录行数</returns>
        public int ExecuteSql(string sqlString)
        {
            return this.ExecuteSql(this.ConnStr, sqlString);
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <param name="level">事务隔离（锁定）级别</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings, IsolationLevel level)
        {
            return this.ExecuteSqlTrans(this.ConnStr, sqlStrings, level);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings)
        {
            return this.ExecuteSqlTrans(this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">SQL语句拼接成的字符串，SQL语句以分号“;”分隔</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(string sqlStrings)
        {
            return this.ExecuteSqlTrans(this.ConnStr, sqlStrings);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns></returns>
        public int RunProcedure(string procedureName, IDataParameter[] parameters)
        {
            return this.RunProcedure(this.ConnStr, procedureName, parameters);
        }
        #endregion

        /// <summary>
        /// 为存储过程构建Command对象
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">存储过程输入参数</param>
        /// <param name="bindByName">是否根据名称绑定参数（假如为OracleCommand）</param>
        /// <returns>返回Command对象</returns>
        protected static Command BuildCommand(Connection connection, string procedureName, IDataParameter[] parameters, bool bindByName)
        {
            Type commandType = typeof(Command);
            Command command = (Command)Activator.CreateInstance(commandType, procedureName, connection);
            command.CommandType = CommandType.StoredProcedure; //Command类型设为存储过程
            //假如是OracleCommand，通过反射实现根据名称绑定参数
            if (commandType.Name.Equals("OracleCommand"))
            {
                PropertyInfo property = commandType.GetProperty("BindByName");
                property.SetValue(command, bindByName);
            }
            command.Parameters.Clear();
            command.Parameters.AddRange(parameters);
            return command;
        }

        /// <summary>
        /// 为存储过程构建Command对象，默认根据名称绑定参数（假如为OracleCommand）
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">存储过程输入参数</param>
        /// <returns>返回Command对象</returns>
        protected static Command BuildCommand(Connection connection, string procedureName, IDataParameter[] parameters)
        {
            return BuildCommand(connection, procedureName, parameters, true);
        }
    }
}
