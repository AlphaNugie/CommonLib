using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLib.Clients;
using CommonLib.Function;
using CommonLib.Helpers;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// Access数据库基础操作类
    /// </summary>
    public class AccessProvider
    {
        #region 公共属性
        /// <summary>
        /// Access数据库名称
        /// </summary>
        private string DbName { get; set; }

        /// <summary>
        /// Access数据库所在路径，包括文件名称及后缀
        /// </summary>
        private string DbFilePath { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnStr { get; private set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        public AccessProvider()
        {
            this.DbName = "AccessDatabase.mdb"; //数据库名称
            this.DbFilePath = AppDomain.CurrentDomain.BaseDirectory + FileSystemHelper.DirSeparator + this.DbName; //带文件名的文件路径
            this.ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.DbFilePath; //拼接Access连接字符串
        }

        /// <summary>
        /// 进行单条SQL语句查询，返回数据表
        /// </summary>
        /// <param name="sqlString">待执行的SQL语句</param>
        /// <returns>返回数据表</returns>
        public DataTable Query(string sqlString)
        {
            if (string.IsNullOrWhiteSpace(sqlString))
                return null;

            sqlString = sqlString.Trim(' ', ';'); //去除字符串前后的空格与分号，否则报错（ORA-00911: 无效字符）
            using (OleDbConnection conn = new OleDbConnection(this.ConnStr))
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(sqlString, conn))
                {
                    DataTable dataTable = new DataTable();

                    try
                    {
                        //dataSet.EnforceConstraints = false; //禁用约束检查
                        conn.Open();
                        adapter.Fill(dataTable);
                    }
                    catch (Exception e)
                    {
                        dataTable = null;
                        FileClient.WriteExceptionInfo(e, "Access SQL语句执行出错", string.Format("SQL语句：{0}", sqlString));
                        throw;
                    }

                    return dataTable;
                }

        }

        /// <summary>
        /// 执行SQL语句并返回受影响行数
        /// </summary>
        /// <param name="sqlString">待执行的SQL语句</param>
        /// <returns>返回影响的记录行数</returns>
        public int ExecuteSql(string sqlString)
        {
            using (OleDbConnection conn = new OleDbConnection(this.ConnStr))
                //去除SQL语句前后的空格与分号
                using (OleDbCommand command = new OleDbCommand(sqlString.Trim(' ', ';'), conn))
                {
                    int result = 0;

                    try
                    {
                        conn.Open();
                        result = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        result = 0;
                        FileClient.WriteExceptionInfo(e, "Access SQL语句执行出错", string.Format("SQL语句：{0}", sqlString));
                        throw; //假如不需要抛出异常，注释此行
                    }

                    return result;
                }

        }
    }
}
