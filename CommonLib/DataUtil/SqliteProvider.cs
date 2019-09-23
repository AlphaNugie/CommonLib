using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Clients;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// SQLite数据库基础操作类
    /// </summary>
    public class SqliteProvider
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnStr { get; private set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public SqliteProvider()
        {
            this.ConnStr = ConfigurationManager.AppSettings["SqliteClient"];
        }

        /// <summary>
        /// 执行单条SQL语句查询
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
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
        /// 执行多个SQL语句查询
        /// </summary>
        /// <param name="sqlStrings">待执行SQL语句</param>
        /// <returns>返回数据集</returns>
        public DataSet MultiQuery(IEnumerable<string> sqlStrings)
        {
            if (sqlStrings == null || sqlStrings.Count() == 0)
                return null;

            DataSet dataSet = new DataSet();
            string _string = string.Empty;
            
            using (SQLiteConnection connection = new SQLiteConnection(this.ConnStr))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(string.Empty, connection))
                {
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
                            adapter.SelectCommand.CommandText = sqlString.Trim(' ', ';'); //去除字符串前后的空格与分号（会报错吗？）
                            adapter.Fill(dataSet, string.Format("Table{0}", i++)); //执行SQL语句并填充以表名指定的DataTable，假如不存在将创建（为避免DataTable被覆盖，表名不要重复）
                        }
                    }
                    catch (Exception e)
                    {
                        dataSet = null;
                        FileClient.WriteExceptionInfo(e, "SQL语句执行出错", string.Format("SQL语句：{0}", _string));
                        throw; //假如不需要抛出异常，注释此行
                    }
                    finally
                    {
                        //不要忘记释放资源
                        connection.Dispose();
                        adapter.Dispose();
                    }
                }
            }
            
            return dataSet;
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString">执行的查询语句</param>
        /// <returns>返回影响的记录行数</returns>
        public int ExecuteSql(string sqlString)
        {
            if (string.IsNullOrWhiteSpace(sqlString))
                return 0;

            sqlString = sqlString.Trim(' ', ';');
            int result = 0;
            using (SQLiteConnection connection = new SQLiteConnection(this.ConnStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        result = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        result = 0;
                        FileClient.WriteExceptionInfo(e, "SQL语句执行出错", string.Format("SQL语句：{0}", sqlString));
                        throw; //假如不需要抛出异常，注释此行
                    }
                    finally
                    {
                        connection.Dispose();
                        command.Dispose();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <returns>假如所有语句执行成功，返回true，否则返回false</returns>
        public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings)
        {
            return this.ExecuteSqlTrans(sqlStrings, IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据事务
        /// </summary>
        /// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        /// <param name="level">事务隔离（锁定）级别</param>
        /// <returns>假如所有语句执行成功，返回true，否则返回false</returns>
        public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings, IsolationLevel level)
        {
            if (sqlStrings == null || sqlStrings.Count() == 0)
                return false;

            bool result = false;
            using (SQLiteConnection connection = new SQLiteConnection(this.ConnStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(string.Empty, connection))
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction(level);
                    command.Transaction = transaction;
                    string sql = string.Empty;

                    try
                    {
                        foreach (string sqlString in sqlStrings)
                        {
                            //假如为空白字符串（移除空格与分号后），跳到下一次循环
                            //去除字符串前后的空格和分号，否则报错（?存疑）
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
                        FileClient.WriteExceptionInfo(e, "SQL语句事务执行失败", string.Format("执行失败的SQL语句：{0}", sql));
                        throw; //假如不需要抛出异常，注释此行
                    }
                    finally
                    {
                        connection.Dispose();
                        command.Dispose();
                        transaction.Dispose();
                    }
                }
            }

            return result;
        }
    }
}
