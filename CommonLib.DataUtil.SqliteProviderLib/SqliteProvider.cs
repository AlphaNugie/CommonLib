using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Clients;
using CommonLib.Helpers;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// SQLite数据库基础操作类
    /// </summary>
    public class SqliteProvider : DataProvider<SQLiteConnection, SQLiteDataAdapter, SQLiteCommand, SQLiteTransaction, SQLiteParameter>
    {
        #region static
        /// <summary>
        /// SQLite连接字符串模板，格式形如“[路径\]xxx.db”
        /// </summary>
        //public const string ConnStrModel = @"Data Source={0}{1}";
        public const string ConnStrModel = @"Data Source={0}";

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="fileDir">数据库文件路径，假如为不包含盘符的相对路径（不包括..\），则添加启动路径成为绝对路径</param>
        /// <param name="fileName">数据库文件名称，包括后缀</param>
        /// <returns>返回连接字符串</returns>
        public static string GetConnStr(string fileDir, string fileName)
        {
            FileSystemHelper.UpdateFilePath(ref fileDir, fileName, out string filePath);
            return string.Format(ConnStrModel, filePath);
            //return string.Format(ConnStrModel, string.IsNullOrWhiteSpace(fileDir) ? AppDomain.CurrentDomain.BaseDirectory : fileDir + "\\", fileName);
        }

        ///// <summary>
        ///// 测试数据库连接是否正常（能够连接）
        ///// </summary>
        ///// <param name="connStr">连接字符串，格式形如“[路径\]xxx.db”</param>
        ///// <returns>假如能够连接，返回true，否则返回false</returns>
        //public static bool IsConnOpen(string connStr)
        //{
        //    return IsConnOpen(connStr);
        //}
        #endregion

        ///// <summary>
        ///// 连接字符串，格式形如“[路径\]xxx.db”
        ///// </summary>
        //public new string ConnStr { get; private set; }

        #region 构造器
        /// <summary>
        /// 以默认配置初始化MySqlProvider
        /// </summary>
        public SqliteProvider() : this(ConfigurationManager.AppSettings["SqliteClient"]) { }

        /// <summary>
        /// 用App.config配置项名称初始化
        /// </summary>
        /// <param name="configurationName">App.config文件中configuration/appSettings节点下的关键字名称</param>
        /// <param name="_">充数的参数，防止签名一致</param>
        public SqliteProvider(string configurationName, object _) : base(configurationName, _) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="connStr">连接字符串，格式形如“[路径\]xxx.db”</param>
        public SqliteProvider(string connStr) : base(connStr) { }

        /// <summary>
        /// 以给定的文件路径与文件名称初始化
        /// </summary>
        /// <param name="fileDir">数据库文件路径（为空则查找可执行文件所在路径）</param>
        /// <param name="fileName">数据库文件名称（包括后缀）</param>
        public SqliteProvider(string fileDir, string fileName) : base(GetConnStr(fileDir, fileName)) { }
        #endregion

        #region old
        ///// <summary>
        ///// 执行单条SQL语句查询
        ///// </summary>
        ///// <param name="sqlString"></param>
        ///// <returns></returns>
        //public DataTable Query(string sqlString)
        //{
        //    DataTable dataTable = null;
        //    DataSet dataSet = this.MultiQuery(new string[] { sqlString });
        //    if (dataSet != null && dataSet.Tables.Count > 0)
        //        dataTable = dataSet.Tables[0];

        //    dataSet.Dispose();
        //    return dataTable;
        //}

        ///// <summary>
        ///// 执行多个SQL语句查询
        ///// </summary>
        ///// <param name="sqlStrings">待执行SQL语句</param>
        ///// <returns>返回数据集</returns>
        //public DataSet MultiQuery(IEnumerable<string> sqlStrings)
        //{
        //    if (sqlStrings == null || sqlStrings.Count() == 0)
        //        return null;

        //    DataSet dataSet = new DataSet();
        //    string _string = string.Empty;

        //    using (SQLiteConnection connection = new SQLiteConnection(this.ConnStr))
        //    {
        //        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(string.Empty, connection))
        //        {
        //            try
        //            {
        //                //dataSet.EnforceConstraints = false; //禁用约束检查
        //                connection.Open();
        //                int i = 1;
        //                foreach (var sqlString in sqlStrings)
        //                {
        //                    _string = sqlString; //记录每个循环中的sql语句
        //                    //跳过空白字符串
        //                    if (string.IsNullOrWhiteSpace(sqlString))
        //                        continue;
        //                    adapter.SelectCommand.CommandText = sqlString.Trim(' ', ';'); //去除字符串前后的空格与分号（会报错吗？）
        //                    adapter.Fill(dataSet, string.Format("Table{0}", i++)); //执行SQL语句并填充以表名指定的DataTable，假如不存在将创建（为避免DataTable被覆盖，表名不要重复）
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                dataSet = null;
        //                FileClient.WriteExceptionInfo(e, "SQL语句执行出错", string.Format("SQL语句：{0}", _string));
        //                throw; //假如不需要抛出异常，注释此行
        //            }
        //            finally
        //            {
        //                //不要忘记释放资源
        //                connection.Dispose();
        //                adapter.Dispose();
        //            }
        //        }
        //    }

        //    return dataSet;
        //}

        ///// <summary>
        ///// 执行SQL语句，返回影响的记录数
        ///// </summary>
        ///// <param name="sqlString">执行的查询语句</param>
        ///// <returns>返回影响的记录行数</returns>
        //public int ExecuteSql(string sqlString)
        //{
        //    if (string.IsNullOrWhiteSpace(sqlString))
        //        return 0;

        //    sqlString = sqlString.Trim(' ', ';');
        //    int result = 0;
        //    using (SQLiteConnection connection = new SQLiteConnection(this.ConnStr))
        //    {
        //        using (SQLiteCommand command = new SQLiteCommand(sqlString, connection))
        //        {
        //            try
        //            {
        //                connection.Open();
        //                result = command.ExecuteNonQuery();
        //            }
        //            catch (Exception e)
        //            {
        //                result = 0;
        //                FileClient.WriteExceptionInfo(e, "SQL语句执行出错", string.Format("SQL语句：{0}", sqlString));
        //                throw; //假如不需要抛出异常，注释此行
        //            }
        //            finally
        //            {
        //                connection.Dispose();
        //                command.Dispose();
        //            }
        //        }
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 执行多条SQL语句，实现数据事务
        ///// </summary>
        ///// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        ///// <returns>假如所有语句执行成功，返回true，否则返回false</returns>
        //public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings)
        //{
        //    return this.ExecuteSqlTrans(sqlStrings, IsolationLevel.ReadCommitted);
        //}

        ///// <summary>
        ///// 执行多条SQL语句，实现数据事务
        ///// </summary>
        ///// <param name="sqlStrings">存储SQL语句的字符串数组</param>
        ///// <param name="level">事务隔离（锁定）级别</param>
        ///// <returns>假如所有语句执行成功，返回true，否则返回false</returns>
        //public bool ExecuteSqlTrans(IEnumerable<string> sqlStrings, IsolationLevel level)
        //{
        //    if (sqlStrings == null || sqlStrings.Count() == 0)
        //        return false;

        //    bool result = false;
        //    using (SQLiteConnection connection = new SQLiteConnection(this.ConnStr))
        //    {
        //        using (SQLiteCommand command = new SQLiteCommand(string.Empty, connection))
        //        {
        //            connection.Open();
        //            SQLiteTransaction transaction = connection.BeginTransaction(level);
        //            command.Transaction = transaction;
        //            string sql = string.Empty;

        //            try
        //            {
        //                foreach (string sqlString in sqlStrings)
        //                {
        //                    //假如为空白字符串（移除空格与分号后），跳到下一次循环
        //                    //去除字符串前后的空格和分号，否则报错（?存疑）
        //                    sql = sqlString == null ? null : sqlString.Trim(' ', ';');
        //                    if (string.IsNullOrWhiteSpace(sql))
        //                        continue;
        //                    command.CommandText = sql;
        //                    command.ExecuteNonQuery();
        //                }
        //                transaction.Commit();
        //                result = true;
        //            }
        //            catch (Exception e)
        //            {
        //                result = false;
        //                transaction.Rollback();
        //                FileClient.WriteExceptionInfo(e, "SQL语句事务执行失败", string.Format("执行失败的SQL语句：{0}", sql));
        //                throw; //假如不需要抛出异常，注释此行
        //            }
        //            finally
        //            {
        //                connection.Dispose();
        //                command.Dispose();
        //                transaction.Dispose();
        //            }
        //        }
        //    }

        //    return result;
        //}
        #endregion
    }
}
