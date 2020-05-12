using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Clients;
using CommonLib.Enums;
using MySql.Data.MySqlClient;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// 与特定数据库连接相关的数据库操作类
    /// </summary>
    public class DataProviderOld
    {
//        private static string _connStr_Oracle = @"
//Data Source =
//  (DESCRIPTION =
//    (ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = {1}))
//    (CONNECT_DATA =
//      (SERVICE_NAME = {2})
//    )
//  );
//  User Id = {3};
//  Password = {4}";

//        private static string _connStr_SqlServer = @"
//Data Source = {0};
//Initial Catalog = ProjectRandom;
//User ID = {1};
//Password = {2};
//database = {3}";
//        private static string _connStr_SqlServer = @"
//Data Source = {0},{1}\{2};
//Initial Catalog = ProjectRandom;
//User ID = {3};
//Password = {4};";
        //        //Persist Security Info=True则代表连接方法在数据库连接成功后保存密码信息，=False则不保存
        //        private static string _connStr_MySql = @"
        //Data Source = {0};
        //port = {1};
        //Initial Catalog = {2};
        //Persist Security Info = True;
        //user id = {3};
        //password = {4};";

        ///// <summary>
        ///// Oracle连接字符串模板
        ///// </summary>
        //public static string ConnStrModel_Oracle { get { return _connStr_Oracle; } }

        ///// <summary>
        ///// SqlServer连接字符串模板
        ///// </summary>
        //public static string ConnStrModel_SqlServer { get { return _connStr_SqlServer; } }

        ///// <summary>
        ///// MySql连接字符串模板
        ///// </summary>
        //public static string ConnStrModel_MySql { get { return _connStr_MySql; } }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public DataProviderOld() { }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="database">数据库类型</param>
        /// <param name="hostAddress">数据库主机地址</param>
        /// <param name="hostPort">数据库主机端口</param>
        /// <param name="serviceName">Oracle:数据库服务名称;MySQL:数据库名称</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回Oracle或SqlServer的连接字符串</returns>
        public static string GetConnStr(DatabaseTypes database, string hostAddress, int hostPort, string serviceName, string userName, string password)
        {
            //if (database == DatabaseTypes.Oracle)
            //    return string.Format(ConnStrModel_Oracle, hostAddress, hostPort, serviceName, userName, password);
            ///*else */if (database == DatabaseTypes.SqlServer)
            //    return string.Format(ConnStrModel_SqlServer, hostAddress, userName, password, serviceName);
            //else if (database == DatabaseTypes.MySql)
            //    return string.Format(ConnStrModel_MySql, hostAddress, hostPort, serviceName, userName, password);

            return string.Empty;
        }

        /// <summary>
        /// 测试数据库连接是否正常（能够连接）
        /// </summary>
        /// <param name="type">数据库类型</param>
        /// <param name="connStr">连接字符串</param>
        /// <returns>假如能够连接，返回true，否则返回false</returns>
        public static bool IsConnOpen(DatabaseTypes type, string connStr)
        {
            dynamic connection = null;
            ConnectionState state = ConnectionState.Closed;
            if (type == DatabaseTypes.Oracle)
                connection = new OracleConnection(connStr);
            else if (type == DatabaseTypes.SqlServer)
                connection = new SqlConnection(connStr);
            else if (type == DatabaseTypes.MySql)
                connection = new MySqlConnection(connStr);
            try
            {
                connection.Open();
                state = connection.State;
            }
            catch (Exception) { state = ConnectionState.Closed; }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return state == ConnectionState.Open;
        }

        /// <summary>
        /// 执行一条或多条SQL语句，返回查询结果集
        /// </summary>
        /// <param name="database">数据库类型，Oracle或SqlServer</param>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="sqlStrings">待执行的查询语句数组</param>
        /// <returns></returns>
        public DataSet MultiQuery(DatabaseTypes database, string connStr, IEnumerable<string> sqlStrings)
        {
            //假如字符数组为空，返回空
            if (sqlStrings == null || sqlStrings.Count() == 0)
                return null;

            //动态类，根据数据库类型初始化；假如数据库类型不正确，返回空
            dynamic connection, adapter;
            if (database == DatabaseTypes.Oracle)
            {
                connection = new OracleConnection(connStr);
                adapter = new OracleDataAdapter(string.Empty, connection);
            }
            else if (database == DatabaseTypes.SqlServer)
            {
                connection = new SqlConnection(connStr);
                adapter = new SqlDataAdapter(string.Empty, connection);
            }
            else if (database == DatabaseTypes.MySql)
            {
                connection = new MySqlConnection(connStr);
                adapter = new MySqlDataAdapter(string.Empty, connection);
            }
            else
                return null;
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
                dataSet = null;
                FileClient.WriteExceptionInfo(e, string.Format("{0} SQL语句执行出错", database.ToString()), string.Format("SQL语句：{0}", _string));
                throw; //假如不需要抛出异常，注释此行
            }
            finally
            {
                //不要忘记释放资源
                connection.Close();
                connection.Dispose();
                adapter.Dispose();
            }

            return dataSet;
        }

        /// <summary>
        /// 执行一条或多条SQL语句，返回查询结果集
        /// </summary>
        /// <param name="database">数据库类型，Oracle或SqlServer</param>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="sqlStrings">待执行的多条查询语句（假如需要执行多条，以“;”分隔）</param>
        /// <returns>返回数据集</returns>
        public DataSet MultiQuery(DatabaseTypes database, string connStr, string sqlStrings)
        {
            string[] queries = sqlStrings.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries); //将源字符串拆分为字符串数组，忽略空字符串
            return this.MultiQuery(database, connStr, queries);
        }

        /// <summary>
        /// 执行sql语句，返回查询结果集DateTable
        /// </summary>
        /// <param name="database">数据库类型：Oracle或Sql Server</param>
        /// <param name="connStr">连接字符串</param>
        /// <param name="sqlString">执行的查询语句</param>
        /// <returns>返回查询结果数据表</returns>
        public DataTable Query(DatabaseTypes database, string connStr, string sqlString)
        {
            DataTable dataTable = null;
            DataSet dataSet = this.MultiQuery(database, connStr, new string[] { sqlString });
            if (dataSet != null && dataSet.Tables.Count > 0)
                dataTable = dataSet.Tables[0];

            dataSet.Dispose();
            return dataTable;
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="database">数据库类型：Oracle或Sql Server</param>
        /// <param name="connStr">连接字符串</param>
        /// <param name="sqlString">执行的查询语句</param>
        /// <returns>返回影响的记录行数</returns>
        public int ExecuteSql(DatabaseTypes database, string connStr, string sqlString)
        {
            if (string.IsNullOrWhiteSpace(sqlString))
                return 0;

            dynamic connection, command;
            sqlString = sqlString.Trim(' ', ';'); //去除SQL语句前后的空格与分号
            if (database == DatabaseTypes.Oracle)
            {
                connection = new OracleConnection(connStr);
                command = new OracleCommand(sqlString, connection);
            }
            else if (database == DatabaseTypes.SqlServer)
            {
                connection = new SqlConnection(connStr);
                command = new SqlCommand(sqlString, connection);
            }
            else if (database == DatabaseTypes.MySql)
            {
                connection = new MySqlConnection(connStr);
                command = new MySqlCommand(sqlString, connection);
            }
            else
                return 0;
            int result = 0;
            try {
                connection.Open();
                result = command.ExecuteNonQuery();
            } catch (Exception e) {
                result = 0;
                FileClient.WriteExceptionInfo(e, string.Format("{0} SQL语句执行出错", database.ToString()), string.Format("SQL语句：{0}", sqlString));
                throw; //假如不需要抛出异常，注释此行
            }
            finally
            {
                //不要忘记释放资源
                connection.Close();
                connection.Dispose();
                command.Dispose();
            }

            return result;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="database">数据库类型：Oracle或Sql Server</param>
        /// <param name="connStr">连接字符串</param>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <param name="level">事务隔离（锁定）级别</param>
        /// <returns>假如所有语句执行成功，返回true，否则返回false</returns>
        public bool ExecuteSqlTrans(DatabaseTypes database, string connStr, IEnumerable<string> sqlStrings, IsolationLevel level)
        {
            //假如数组为空，返回false
            if (sqlStrings == null || sqlStrings.Count() == 0)
                return false;

            //假如数据库类型不正确，返回false
            dynamic connection, command, transaction;
            if (database == DatabaseTypes.Oracle)
            {
                connection = new OracleConnection(connStr);
                command = new OracleCommand(string.Empty, connection);
            }
            else if (database == DatabaseTypes.SqlServer)
            {
                connection = new SqlConnection(connStr);
                command = new SqlCommand(string.Empty, connection);
            }
            else if (database == DatabaseTypes.MySql)
            {
                connection = new MySqlConnection(connStr);
                command = new MySqlCommand(string.Empty, connection);
            }
            else
                return false;

            bool result = false;
            try { connection.Open(); }
            catch (Exception e)
            {
                result = false;
                FileClient.WriteExceptionInfo(e, string.Format("{0} 数据库连接打开失败", database.ToString()));
                connection.Close();
                connection.Dispose();
                command.Dispose();
                throw;
            }

            string sql = string.Empty;
            transaction = connection.BeginTransaction(level); //connection必须先Open
            try
            {
                command.Transaction = transaction;
                foreach (string sqlString in sqlStrings)
                {
                    //假如为空白字符串（移除空格与分号后），跳到下一次循环
                    //需去除字符串前后的空格和分号，否则Oracle中报错（ORA-00911: 无效字符）
                    sql = sqlString == null ? null : sqlString.Trim(' ', ';');
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
                FileClient.WriteExceptionInfo(e, string.Format("{0} SQL语句事务执行失败", database.ToString()), string.Format("执行失败的SQL语句：{0}", sql));
                throw; //假如不需要抛出异常，注释此行
            }
            finally
            {
                //不要忘记释放资源
                connection.Close();
                connection.Dispose();
                command.Dispose();
                transaction.Dispose();
            }

            return result;
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="database">数据库类型：Oracle或Sql Server</param>
        /// <param name="connStr">连接字符串</param>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(DatabaseTypes database, string connStr, IEnumerable<string> sqlStrings)
        {
            return this.ExecuteSqlTrans(database, connStr, sqlStrings, IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="database">数据库类型：Oracle或Sql Server</param>
        /// <param name="connStr">连接字符串</param>
        /// <param name="sqlStrings">SQL语句拼接成的字符串，SQL语句以分号“;”分隔</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(DatabaseTypes database, string connStr, string sqlStrings)
        {
            //将源字符串拆分为字符串数组，忽略空字符串
            //假如字符串为空白字符串，数组为空
            string[] sqlArray = string.IsNullOrWhiteSpace(sqlStrings) ? null : sqlStrings.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            return this.ExecuteSqlTrans(database, connStr, sqlArray);
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数 
        /// </summary>
        /// <param name="database">数据库类型</param>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="parameters">输入参数</param>
        /// <returns>返回影响的行数（？）</returns>
        public int RunProcedure(DatabaseTypes database, string connStr, string procedureName, IDataParameter[] parameters)
        {
            //if (database != DatabaseTypes.Oracle && database != DatabaseTypes.SqlServer)
            //    return 0;

            dynamic connection, command;
            if (database == DatabaseTypes.Oracle)
            {
                connection = new OracleConnection(connStr);
                command = this.BuildOracleCommand(connection, procedureName, parameters);
            }
            else if (database == DatabaseTypes.SqlServer)
            {
                connection = new SqlConnection(connStr);
                command = this.BuildSqlServerCommand(connection, procedureName, parameters);
            }
            else if (database == DatabaseTypes.MySql)
            {
                connection = new MySqlConnection(connStr);
                command = this.BuildMySqlCommand(connection, procedureName, parameters);
            }
            else
                return 0;
            int result = 0;
            try
            {
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                result = 0;
                FileClient.WriteExceptionInfo(e, string.Format("{0} 存储过程执行失败", database.ToString()));
                throw; //假如不需要抛出异常，将此行注释
            }
            finally
            {
                //不要忘记释放资源
                connection.Close();
                connection.Dispose();
                command.Dispose();
            }

            return result;
        }

        /// <summary>
        /// 为存储过程构建OracleCommand对象
        /// </summary>
        /// <param name="connection">Oracle数据库链接对象</param>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">存储过程输入参数</param>
        /// <param name="bindByName">是否根据名称绑定参数</param>
        /// <returns>返回OracleCommand对象</returns>
        private OracleCommand BuildOracleCommand(OracleConnection connection, string procedureName, IDataParameter[] parameters, bool bindByName)
        {
            OracleCommand command = new OracleCommand(procedureName, connection);
            command.CommandType = CommandType.StoredProcedure; //Command类型设为存储过程
            command.BindByName = bindByName; //根据名称绑定参数
            command.Parameters.Clear();
            command.Parameters.AddRange(parameters);
            return command;
        }

        /// <summary>
        /// 为存储过程构建OracleCommand对象
        /// </summary>
        /// <param name="connection">Oracle数据库链接对象</param>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">存储过程输入参数</param>
        /// <returns>返回OracleCommand对象</returns>
        private OracleCommand BuildOracleCommand(OracleConnection connection, string procedureName, IDataParameter[] parameters)
        {
            return this.BuildOracleCommand(connection, procedureName, parameters, true);
        }

        /// <summary>
        /// 为存储过程构建SQLCommand对象
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="parameters">输入参数</param>    
        /// <returns>返回SqlCommand对象</returns>
        private SqlCommand BuildSqlServerCommand(SqlConnection connection, string procedureName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(procedureName, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Clear();
            command.Parameters.AddRange(parameters);
            return command;
        }

        /// <summary>
        /// 为存储过程构建MySQLCommand对象
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="parameters">输入参数</param>    
        /// <returns>返回SqlCommand对象</returns>
        private MySqlCommand BuildMySqlCommand(MySqlConnection connection, string procedureName, IDataParameter[] parameters)
        {
            MySqlCommand command = new MySqlCommand(procedureName, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Clear();
            command.Parameters.AddRange(parameters);
            return command;
        }
    }
}
