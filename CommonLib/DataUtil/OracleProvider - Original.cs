using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using CommonLib.Clients;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// Oracle数据库基础操作类
    /// </summary>
    public class OracleProvider
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnStr { get; private set; }

        /// <summary>
        /// Oracle基础操作类构造器
        /// </summary>
        public OracleProvider()
        {
            this.ConnStr = ConfigurationManager.AppSettings["OracleClient"]; //从配置文件中获取数据库连接字符串
        }

        /// <summary>
        /// 执行一条或多条SQL语句，返回查询结果集
        /// </summary>
        /// <param name="sqlStrings">包含SQL语句的字符串数组</param>
        /// <returns>返回结果集</returns>
        public DataSet MultiQuery(string[] sqlStrings)
        {
            //假如字符数组为空，返回空
            if (sqlStrings == null || sqlStrings.Length == 0)
                return null;

            //using块结束后，connection、adapter占用资源将被回收（自动执行Dispose方法）
            using (OracleConnection connection = new OracleConnection(this.ConnStr))
                using (OracleDataAdapter adapter = new OracleDataAdapter(string.Empty, connection))
                {
                    DataSet dataSet = new DataSet();
                    string _string = string.Empty;

                    try
                    {
                        //dataSet.EnforceConstraints = false; //禁用约束检查
                        connection.Open();

                        int i = 1;
                        //循环执行SQL语句，using块结束后command将被回收
                        foreach (var sqlString in sqlStrings)
                        {
                            _string = sqlString; //记录每个循环中的sql语句
                            //跳过空白字符串
                            if (string.IsNullOrWhiteSpace(sqlString))
                                continue;
                            adapter.SelectCommand.CommandText = sqlString.Trim(' ', ';'); //去除字符串前后的空格与分号，否则报错（ORA-00911: 无效字符）
                            adapter.Fill(dataSet, string.Format("Table{0}", i)); //执行SQL语句并填充以表名指定的DataTable，假如不存在将创建（为避免DataTable被覆盖，表名不要重复）
                            i++;
                        }
                    }
                    //假如出现异常，写入日志，重新抛出异常
                    catch (Exception e)
                    {
                        dataSet = null;
                        FileClient.WriteExceptionInfo(e, "Oracle SQL语句执行出错", string.Format("SQL语句：{0}", _string));
                        throw; //假如不需要抛出异常，注释此行
                    }

                    return dataSet;
                }

        }

        /// <summary>
        /// 执行一条或多条SQL语句，返回查询结果集
        /// </summary>
        /// <param name="sqlStrings">执行的查询语句（假如需要执行多条，以“;”分隔）</param>
        /// <returns>返回数据集</returns>
        public DataSet MultiQuery(string sqlStrings)
        {
            string[] queries = sqlStrings.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries); //将源字符串拆分为字符串数组，忽略空字符串
            return this.MultiQuery(queries);
        }

        /// <summary>
        /// 进行单条SQL语句查询，返回数据表
        /// </summary>
        /// <param name="sqlString">待执行的一条SQL语句</param>
        /// <returns>返回数据表</returns>
        public DataTable Query(string sqlString)
        {
            DataTable dataTable = null;
            DataSet dataSet = this.MultiQuery(new string[] { sqlString });
            if (dataSet != null && dataSet.Tables.Count > 0)
                dataTable = dataSet.Tables[0];

            dataSet.Dispose();
            return dataTable;
        }

        /// <summary>
        /// 执行SQL语句并返回受影响行数
        /// </summary>
        /// <param name="sqlString">待执行的SQL语句</param>
        /// <returns>返回影响的记录行数</returns>
        public int ExecuteSql(string sqlString)
        {
            using (OracleConnection connection = new OracleConnection(this.ConnStr))
                //去除SQL语句前后的空格与分号
                using (OracleCommand command = new OracleCommand(sqlString.Trim(' ', ';'), connection))
                {
                    int result = 0;
                    try
                    {
                        connection.Open();
                        result = command.ExecuteNonQuery();
                        //using (OracleCommand command = new OracleCommand(sqlString, connection))
                        //return command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        result = 0;
                        FileClient.WriteExceptionInfo(e, "Oracle SQL语句执行出错", string.Format("SQL语句：{0}", sqlString));
                        throw; //假如不需要抛出异常，注释此行
                    }

                    return result;
                }

        }

        /// <summary>
        /// 执行多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <param name="level">事务隔离（锁定）级别</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(string[] sqlStrings, IsolationLevel level)
        {
            //假如数组为空，返回false
            if (sqlStrings == null || sqlStrings.Length == 0)
                return true;

            using (OracleConnection connection = new OracleConnection(this.ConnStr))
            {
                connection.Open();

                using (OracleTransaction transaction = connection.BeginTransaction(level))
                    using (OracleCommand command = new OracleCommand(string.Empty, connection))
                    {
                        command.Transaction = transaction;
                        bool result = true;
                        string sql = string.Empty;

                        try
                        {
                            foreach (string sqlString in sqlStrings)
                            {
                                //假如为空白字符串，跳到下一次循环
                                if (string.IsNullOrWhiteSpace(sqlString))
                                    continue;
                                command.CommandText = sqlString.Trim(' ', ';'); //去除字符串前后的空格和分号，否则报错（ORA-00911: 无效字符）
                                sql = command.CommandText;
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            result = false;
                            transaction.Rollback();
                            FileClient.WriteExceptionInfo(e, "SQL语句事务执行失败", string.Format("执行失败的SQL语句：{0}", sql));
                            throw; //假如不需要抛出异常，注释此行
                        }

                        return result;
                    }

            }
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(string[] sqlStrings)
        {
            return this.ExecuteSqlTrans(sqlStrings, IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// 执行一条或多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">SQL语句拼接成的字符串，SQL语句以分号“;”分隔</param>
        /// <returns>假如执行成功，返回true</returns>
        public bool ExecuteSqlTrans(string sqlStrings)
        {
            //将源字符串拆分为字符串数组，忽略空字符串
            //假如字符串为空白字符串，数组为空
            string[] sqlArray = string.IsNullOrWhiteSpace(sqlStrings) ? null : sqlStrings.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            return this.ExecuteSqlTrans(sqlArray);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns></returns>
        public int RunProcedure(string procedureName, IDataParameter[] parameters)
        {
            using (OracleConnection connection = new OracleConnection(this.ConnStr))
                using (OracleCommand command = this.BuildOracleCommand(connection, procedureName, parameters))
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
                        FileClient.WriteExceptionInfo(e, "存储过程执行失败");
                        throw; //假如不需要抛出异常，将此行注释
                    }

                    return result;
                }

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
    }
}
