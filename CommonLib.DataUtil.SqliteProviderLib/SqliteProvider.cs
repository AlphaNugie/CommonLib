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
            return GetConnStr(filePath);
            //return string.Format(ConnStrModel, filePath);
        }

        /// <summary>
        /// 获取连接字符串，假如完整路径为空则输出空字符串
        /// </summary>
        /// <param name="filePath">数据库文件路径+名称（包括后缀），假如为不包含盘符的相对路径（不包括..\），则添加启动路径成为绝对路径（假如为空输出空字符串）</param>
        /// <returns>返回连接字符串</returns>
        public static string GetConnStr(string filePath)
        {
            return string.IsNullOrWhiteSpace(filePath) ? string.Empty : string.Format(ConnStrModel, filePath);
        }

        /// <summary>
        /// 测试以给定的文件路径以及文件名称描述的数据库连接是否正常（能够连接）
        /// </summary>
        /// <param name="fileDir">数据库文件路径，假如为不包含盘符的相对路径（不包括..\），则添加启动路径成为绝对路径</param>
        /// <param name="fileName">数据库文件名称，包括后缀</param>
        /// <returns>假如能够连接，返回true，否则返回false</returns>
        public static bool IsConnOpen(string fileDir, string fileName)
        {
            return IsConnOpen(GetConnStr(fileDir, fileName));
        }

        /// <summary>
        /// 测试以给定的连接字符串描述的sqlite数据库连接是否正常（能够连接）
        /// </summary>
        /// <param name="connStr">sqlite连接字符串</param>
        /// <returns>假如能够连接，返回true，否则返回false</returns>
        public static new bool IsConnOpen(string connStr)
        {
            return DataProvider<SQLiteConnection, SQLiteDataAdapter, SQLiteCommand, SQLiteTransaction, SQLiteParameter>.IsConnOpen(connStr);
        }
        #endregion

        #region 构造器
        /// <summary>
        /// 以默认配置初始化MySqlProvider
        /// </summary>
        public SqliteProvider() : base("SqliteClient", null) { }

        /// <summary>
        /// 用App.config配置项名称初始化
        /// </summary>
        /// <param name="configurationName">App.config文件中configuration/appSettings节点下的关键字名称</param>
        /// <param name="_">充数的参数，防止签名一致</param>
        public SqliteProvider(string configurationName, object _) : base(configurationName, null) { }

        /// <summary>
        /// 以给定的文件路径+文件名称初始化
        /// </summary>
        /// <param name="filePath">数据库文件路径+名称（包括后缀），假如为不包含盘符的相对路径（不包括..\），则添加启动路径成为绝对路径</param>
        public SqliteProvider(string filePath) : base(GetConnStr(filePath)) { }

        /// <summary>
        /// 以给定的文件路径与文件名称初始化
        /// </summary>
        /// <param name="fileDir">数据库文件路径（为空则查找可执行文件所在路径）</param>
        /// <param name="fileName">数据库文件名称（包括后缀）</param>
        public SqliteProvider(string fileDir, string fileName) : base(GetConnStr(fileDir, fileName)) { }
        #endregion

        /// <summary>
        /// 用给定的数据库文件路径和文件名称设置连接字符串
        /// </summary>
        /// <param name="fileDir">数据库文件路径，假如为不包含盘符的相对路径（不包括..\），则添加启动路径成为绝对路径</param>
        /// <param name="fileName">数据库文件名称，包括后缀</param>
        /// <returns>返回连接字符串</returns>
        public void SetConnStr(string fileDir, string fileName)
        {
            base.SetConnStr(GetConnStr(fileDir, fileName));
            //return string.Format(ConnStrModel, filePath);
        }

        /// <summary>
        /// 用给定的数据库文件完整路径设置连接字符串
        /// </summary>
        /// <param name="filePath">数据库文件路径+名称（包括后缀），假如为不包含盘符的相对路径（不包括..\），则添加启动路径成为绝对路径</param>
        /// <returns>返回连接字符串</returns>
        public new void SetConnStr(string filePath)
        {
            base.SetConnStr(GetConnStr(filePath));
        }

        /// <summary>
        /// 直接用给定的字符串来设置连接字符串，而不是数据库文件路径或名称
        /// </summary>
        /// <param name="connStr">连接字符串本身</param>
        /// <returns>返回连接字符串</returns>
        public void SetConnStrDirectly(string connStr)
        {
            base.SetConnStr(connStr);
        }
    }
}
