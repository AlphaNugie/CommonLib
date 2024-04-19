using CommonLib.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// 基础Oracle数据服务类
    /// </summary>
    public abstract class BaseDataServiceOracle
    {
        /// <summary>
        /// SQLite基础操作类的对象
        /// </summary>
        protected OracleProvider _provider;

        #region 属性
        /// <summary>
        /// SQLite数据库操作对象
        /// </summary>
        public OracleProvider Provider { get => _provider; }

        /// <summary>
        /// 对应表名称
        /// </summary>
        public string TableName { get { return GetTableName(); } }

        /// <summary>
        /// 必须具备的字段
        /// </summary>
        public List<OracleColumnMapping> ColumnsMustHave { get; private set; }
        #endregion

        /// <summary>
        /// 根据给定的数据库相关信息以及默认端口号初始化
        /// </summary>
        /// <param name="hostAddress">数据库地址</param>
        /// <param name="serviceName">数据库服务名</param>
        /// <param name="userName">用户名称</param>
        /// <param name="password">用户密码</param>
        public BaseDataServiceOracle(string hostAddress, string serviceName, string userName, string password) : this(hostAddress, OracleProvider.DefaultPort, serviceName, userName, password) { }

        /// <summary>
        /// 根据给定的数据库相关信息初始化
        /// </summary>
        /// <param name="hostAddress">数据库地址</param>
        /// <param name="hostPort">端口号</param>
        /// <param name="serviceName">数据库服务名</param>
        /// <param name="userName">用户名称</param>
        /// <param name="password">用户密码</param>
        public BaseDataServiceOracle(string hostAddress, int hostPort, string serviceName, string userName, string password)
        {
            SetDbParams(hostAddress, hostPort, serviceName, userName, password);
            ColumnsMustHave = GetColumnsMustHave();
        }

        /// <summary>
        /// 获取并返回当前表的名称
        /// </summary>
        protected abstract string GetTableName();

        /// <summary>
        /// 获取并返回包含必须存在的字段的列表
        /// </summary>
        protected abstract List<OracleColumnMapping> GetColumnsMustHave();

        /// <summary>
        /// 设置Oracle数据库的相关信息
        /// </summary>
        /// <param name="hostAddress">数据库地址</param>
        /// <param name="serviceName">数据库服务名</param>
        /// <param name="userName">用户名称</param>
        /// <param name="password">用户密码</param>
        /// <exception cref="ArgumentNullException">未指定Oracle文件名称</exception>
        public void SetDbParams(string hostAddress, string serviceName, string userName, string password)
        {
            SetDbParams(hostAddress, OracleProvider.DefaultPort, serviceName, userName, password);
        }

        /// <summary>
        /// 设置Oracle数据库的相关信息
        /// </summary>
        /// <param name="hostAddress">数据库地址</param>
        /// <param name="hostPort">端口号</param>
        /// <param name="serviceName">数据库服务名</param>
        /// <param name="userName">用户名称</param>
        /// <param name="password">用户密码</param>
        /// <exception cref="ArgumentNullException">未指定Oracle文件名称</exception>
        public void SetDbParams(string hostAddress, int hostPort, string serviceName, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(hostAddress))
                throw new ArgumentNullException(nameof(hostAddress), "未指定数据库地址");
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName), "未指定数据库实例名");
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName), "未指定用户名");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password), "未指定密码");
            if (_provider == null)
                _provider = new OracleProvider(hostAddress, hostPort, serviceName, userName, password);
            else
                _provider.SetConnStr(hostAddress, hostPort, serviceName, userName, password);
        }

        /// <summary>
        /// 获取所有数据库字段信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public List<OracleColumnMapping> GetAllColumnMappings(out string message)
        {
            return _provider.GetColumnMappings(TableName, null, out message);
        }

        /// <summary>
        /// 根据给定的数据库字段列表获取数据库字段信息，假如列表为空则获取所有字段
        /// </summary>
        /// <param name="columnNames">给定的数据库字段列表，假如为空则获取所有字段</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public List<OracleColumnMapping> GetColumnMappings(IEnumerable<string> columnNames, out string message)
        {
            return _provider.GetColumnMappings(TableName, columnNames, out message);
        }

        /// <summary>
        /// 获取所有记录，并按特定字段排序
        /// </summary>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetAllRecords(string orderby)
        {
            return _provider.GetAllRecords(TableName, orderby);
        }

        #region CheckForTableColumns
        /// <summary>
        /// 检查数据表的字段，假如缺少字段则增加
        /// </summary>
        public virtual bool CheckForTableColumns()
        {
            return _provider.CheckForTableColumns(TableName, ColumnsMustHave, out _);
        }

        /// <summary>
        /// 根据设置的表名及列信息检查数据表的字段，假如表不存在先新增表，然后根据给定列判断字段是否存在，不存在则新增列，最终返回操作结果
        /// </summary>
        public virtual bool CheckForTableColumns(out string message)
        {
            return _provider.CheckForTableColumns(TableName, ColumnsMustHave, out message);
        }

        /// <summary>
        /// 判断
        /// </summary>
        /// <returns></returns>
        public bool TableExists(out DataTable table)
        {
            return _provider.TableExists(TableName, out table);
        }

        /// <summary>
        /// 检查数据表是否存在，不存在则新增
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool CheckForTable(out DataTable table)
        {
            return _provider.CheckForTable(TableName, ColumnsMustHave, out table, out _);
        }

        /// <summary>
        /// 检查数据表是否存在，不存在则新增
        /// </summary>
        /// <param name="table"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool CheckForTable(out DataTable table, out string message)
        {
            return _provider.CheckForTable(TableName, ColumnsMustHave, out table, out message);
        }

        /// <summary>
        /// 查找指定命名空间下继承BaseDataServiceOracle的符合给定条件的所有子类，并执行其中的CheckForTableColumns方法（无参）
        /// </summary>
        /// <param name="nameSpace">命名空间全名（或一部分），区分大小写</param>
        /// <param name="subSpaceIncl">是否查找子命名空间</param>
        /// <param name="typeNameIncl">查找时限定类名的一部分，假如为空则不限定</param>
        ///// <param name="baseType">查找类时限定的从中继承的类（仅检查类型名称及命名空间是否相同），假如为空则不限定</param>
        public static void CallMethodForWholeShebang_CheckForTableColumns(string nameSpace, bool subSpaceIncl = false, string typeNameIncl = null)
        {
            var serviceTypes = Assembly.GetEntryAssembly().GetTypesInNamespace(nameSpace, subSpaceIncl, typeNameIncl, typeof(BaseDataServiceOracle));
            if (serviceTypes != null && serviceTypes.Length > 0)
            {
                foreach (var type in serviceTypes)
                {
                    var obj = Activator.CreateInstance(type);
                    MethodInfo checkMethod = type.GetMethod("CheckForTableColumns", Type.EmptyTypes);
                    checkMethod.Invoke(obj, null);
                }
            }
        }

        /// <summary>
        /// 查找与当前类相同命名空间下继承BaseDataServiceOracle的符合给定条件的所有子类，并执行其中的CheckForTableColumns方法（无参）
        /// </summary>
        /// <param name="subSpaceIncl">是否查找子命名空间</param>
        /// <param name="typeNameIncl">查找时限定类名的一部分，假如为空则不限定</param>
        public void CallMethodForWholeShebang_CheckForTableColumns(bool subSpaceIncl = false, string typeNameIncl = null)
        {
            var type = GetType();
            CallMethodForWholeShebang_CheckForTableColumns(type.Namespace, subSpaceIncl, typeNameIncl);
        }
        #endregion
    }
}
