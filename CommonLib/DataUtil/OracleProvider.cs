using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Clients;
using Oracle.ManagedDataAccess.Client;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// Oracle数据库基础操作类
    /// </summary>
    public class OracleProvider : DataProvider<OracleConnection, OracleDataAdapter, OracleCommand, OracleTransaction>
    {
        #region static
        /// <summary>
        /// Oracle连接字符串模板
        /// </summary>
        public const string ConnStrModel = @"Data Source =
  (DESCRIPTION =
    (ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = {1}))
    (CONNECT_DATA =
      (SERVICE_NAME = {2})
    )
  ); User Id = {3}; Password = {4};";

        /// <summary>
        /// 默认端口号
        /// </summary>
        public const int DefaultPort = 1521;

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
        #endregion

        ///// <summary>
        ///// 数据库连接字符串，形如“Data Source=ORCL1;User Id=test;Password=123;”，其中ORCL1可由具体的(DESCRIPTION...)代替
        ///// </summary>
        //public new string ConnStr { get; private set; }

        #region 构造器
        /// <summary>
        /// 以默认配置初始化OracleProvider
        /// </summary>
        public OracleProvider() : this(ConfigurationManager.AppSettings["OracleClient"]) { }

        /// <summary>
        /// 用App.config配置项名称初始化
        /// </summary>
        /// <param name="configurationName">App.config文件中configuration/appSettings节点下的关键字名称</param>
        /// <param name="_">充数的参数，防止签名一致</param>
        public OracleProvider(string configurationName, object _) : base(configurationName, _) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="connStr">连接字符串，形如“Data Source=ORCL1;User Id=test;Password=123;”，其中ORCL1可由具体的(DESCRIPTION...)代替</param>
        public OracleProvider(string connStr) : base(connStr) { }

        /// <summary>
        /// 根据给定的数据库相关信息初始化
        /// </summary>
        /// <param name="hostAddress">数据库地址</param>
        /// <param name="hostPort">端口号</param>
        /// <param name="serviceName">数据库服务名</param>
        /// <param name="userName">用户名称</param>
        /// <param name="password">用户密码</param>
        public OracleProvider(string hostAddress, int hostPort, string serviceName, string userName, string password) : base(ConnStrModel, hostAddress, hostPort, serviceName, userName, password) { }

        /// <summary>
        /// 根据给定的数据库相关信息以及默认端口号初始化
        /// </summary>
        /// <param name="hostAddress">数据库地址</param>
        /// <param name="serviceName">数据库服务名</param>
        /// <param name="userName">用户名称</param>
        /// <param name="password">用户密码</param>
        public OracleProvider(string hostAddress, string serviceName, string userName, string password) : base(ConnStrModel, hostAddress, DefaultPort, serviceName, userName, password) { }
        #endregion
    }
}
