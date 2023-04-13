using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Enums;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// Oracle基础数据库操作类（仿Batis）
    /// </summary>
    public class BaseDataService<T> : BaseDataServiceBatis<T> where T : BaseModel
    {
        /// <summary>
        /// Oracle基础操作类对象
        /// </summary>
        protected OracleProvider _provider/* = new OracleProvider()*/;

        /// <summary>
        /// 数据库操作类构造器，使用App.config文件中"OracleClient"配置项内容初始化
        /// </summary>
        public BaseDataService() : base(string.Empty, false) { }

        /// <summary>
        /// 数据库操作类构造器，使用给定的连接字符串初始化
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        public BaseDataService(string connStr) : base(connStr, true) { }

        /// <summary>
        /// 数据库操作类构造器，在连接字符串不为空且使用远程数据库时用连接字符串初始化，否则使用"OracleClient"配置项
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="usingRemote">是否使用远程数据库（而非当前使用的数据库）</param>
        public BaseDataService(string connStr, bool usingRemote) : base(connStr, usingRemote) { }

        /// <summary>
        /// 初始化Provider对象，在连接字符串不为空且使用远程数据库时用连接字符串初始化，否则使用App.config文件中"OracleClient"配置项内容初始化
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="usingRemote">是否使用远程数据库（而非当前使用的数据库）</param>
        public override void InitProviderInstance(string connStr, bool usingRemote)
        {
            _provider = !string.IsNullOrWhiteSpace(connStr) && usingRemote ? new OracleProvider(connStr) : new OracleProvider();
        }

        /// <inheritdoc/>
        public override int ProviderExecuteSql(string sqlString)
        {
            return _provider.ExecuteSql(sqlString);
        }

        /// <inheritdoc/>
        public override bool ProviderExecuteSqlTrans(IEnumerable<string> sqlStrings)
        {
            return _provider.ExecuteSqlTrans(sqlStrings);
        }

        /// <inheritdoc/>
        public override DataTable ProviderQuery(string sqlString)
        {
            return _provider.Query(sqlString);
        }

        ///// <summary>
        ///// Oracle基础操作类
        ///// </summary>
        //protected OracleProvider provider = new OracleProvider();

        ///// <summary>
        ///// Batis操作类
        ///// </summary>
        //protected BatisLike batisLike;

        //#region 公共属性
        ///// <summary>
        ///// 类型名称
        ///// </summary>
        //public string TypeName { get; protected set; }

        ///// <summary>
        ///// 最新错误代码
        ///// </summary>
        //public string LastErrorCode { get; protected set; }

        ///// <summary>
        ///// 最新错误信息
        ///// </summary>
        //public string LastErrorMessage { get; protected set; }
        //#endregion

        ///// <summary>
        ///// 数据库操作类构造器
        ///// </summary>
        //public BaseDataService() : this(string.Empty, false) { }

        ///// <summary>
        ///// 数据库操作类构造器
        ///// </summary>
        ///// <param name="connStr">连接字符串</param>
        //public BaseDataService(string connStr) : this(connStr, true) { }

        ///// <summary>
        ///// 数据库操作类构造器
        ///// </summary>
        ///// <param name="connStr">连接字符串</param>
        ///// <param name="usingRemote">是否使用远程数据库</param>
        //public BaseDataService(string connStr, bool usingRemote)
        //{
        //    if (!string.IsNullOrWhiteSpace(connStr) && usingRemote)
        //        provider = new OracleProvider(connStr);
        //    TypeName = typeof(T).Name;
        //    batisLike = new BatisLike(TypeName); //BatisLike对象中的映射名称为类型名称
        //    //batisLike.MapperName = TypeName; //BatisLike对象中的映射名称为类型名称
        //    LastErrorCode = string.Empty;
        //    LastErrorMessage = string.Empty;
        //}

        /////// <summary>
        /////// 通过SqlMap的关键词获取SQL语句
        /////// </summary>
        /////// <param name="sqlMapKey">SqlMap的关键词</param>
        /////// <returns></returns>
        ////public string GetSqlStringBySqlMapKey(string sqlMapKey, Dictionary<string, object> dict)
        ////{
        ////    string sqlString = batisLike.GetSql(TypeName + "." + sqlMapKey);
        ////    if (dict != null && dict.Keys.Count > 0)
        ////        sqlString = batisLike.ConvertSqlStringByKeyValue(sqlString, dict);
        ////    return sqlString;
        ////}

        /////// <summary>
        /////// 通过SqlMap的关键词获取SQL语句
        /////// </summary>
        /////// <param name="sqlMapKey">SqlMap的关键词</param>
        /////// <returns></returns>
        ////public string GetSqlStringBySqlMapKey(string sqlMapKey)
        ////{
        ////    return GetSqlStringBySqlMapKey(sqlMapKey, null);
        ////}

        /////// <summary>
        /////// 通过SqlMap的关键值查询信息
        /////// </summary>
        /////// <param name="sqlMapKey">SqlMap的关键词</param>
        /////// <returns></returns>
        ////public DataTable GetRecordBySqlMapKey(string sqlMapKey, Dictionary<string, object> dict)
        ////{
        ////    //string sqlString = batisLike.GetSql(TypeName + "." + sqlMapKey);
        ////    string sqlString = batisLike.GetSqlStringBySqlMapKey(sqlMapKey, dict);
        ////    return provider.Query(sqlString);
        ////}

        /////// <summary>
        /////// 通过SqlMap的关键值查询信息
        /////// </summary>
        /////// <param name="sqlMapKey">SqlMap的关键词</param>
        /////// <returns></returns>
        ////public DataTable GetRecordBySqlMapKey(string sqlMapKey)
        ////{
        ////    //string sqlString = batisLike.GetSql(TypeName + "." + sqlMapKey);
        ////    string sqlString = GetSqlStringBySqlMapKey(sqlMapKey);
        ////    return provider.Query(sqlString);
        ////}

        ///// <summary>
        ///// 将数据行的数据转换为实体类
        ///// </summary>
        ///// <param name="dataRow">包含待转换数据的数据行</param>
        ///// <returns>返回转换后的实体类对象</returns>
        //public T ConvertObjectByDataRow(DataRow dataRow)
        //{
        //    return batisLike.ConvertObjectByDataRow<T>(dataRow);
        //}

        ///// <summary>
        ///// 将数据表的数据转换为实体类List
        ///// </summary>
        ///// <param name="dataTable">包含待转换数据的数据表</param>
        ///// <returns>返回转换后的实体类对象List</returns>
        //public List<T> ConvertObjectListByDataTable(DataTable dataTable)
        //{
        //    return batisLike.ConvertObjectListByDataTable<T>(dataTable);
        //}

        ///// <summary>
        ///// 获取可用/不可用枚举
        ///// </summary>
        ///// <returns></returns>
        //public DataTable GetEnableEnums()
        //{
        //    string sqlString = @"
        //      select 1 code, '可用' name from dual
        //        union all
        //      select 0 code, '不可用' name from dual";
        //    return provider.Query(sqlString);
        //}

        ///// <summary>
        ///// 根据记录ID获取记录
        ///// </summary>
        ///// <param name="id">记录的ID</param>
        ///// <returns>返回数据表</returns>
        //public DataTable GetRecordById(int id)
        //{
        //    Dictionary<string, object> dict = new Dictionary<string, object> { { "Id", id } };
        //    DataTable table;
        //    try { table = GetRecords(dict); }
        //    catch (Exception e)
        //    {
        //        LastErrorCode = "Exception";
        //        LastErrorMessage = e.Message;
        //        //return null;
        //        throw;
        //    }

        //    if (table == null || table.Rows.Count == 0)
        //    {
        //        LastErrorCode = "001";
        //        LastErrorMessage = "未查询到任何数据";
        //    }
        //    else if (table.Rows.Count > 1)
        //    {
        //        LastErrorCode = "002";
        //        LastErrorMessage = "查询到多条记录";
        //    }

        //    return table;
        //}

        ///// <summary>
        ///// 根据数据库表ID获取数据库表实体类对象
        ///// </summary>
        ///// <param name="id">记录在数据库表中的ID</param>
        ///// <returns>返回实体类对象</returns>
        //public virtual T GetRecordObjectById(int id)
        //{
        //    List<T> list = ConvertObjectListByDataTable(GetRecordById(id));
        //    if (list == null || list.Count == 0)
        //        return default;
        //    return list[0];
        //}

        ///// <summary>
        ///// 根据条件获取符合条件的记录
        ///// </summary>
        ///// <param name="dict">包含条件的键值对，假如为空，则所有参数为空</param>
        ///// <returns>返回数据表</returns>
        //public DataTable GetRecords(Dictionary<string, object> dict)
        //{
        //    string sqlString = batisLike.GetSqlStringBySqlMapKey("Get", dict);
        //    //string sqlString = batisLike.GetQueryString(dict);
        //    return provider.Query(sqlString);
        //}

        ///// <summary>
        ///// 根据条件获取包含实体类对象的List
        ///// </summary>
        ///// <param name="dict">包含条件的键值对，假如为空，则所有参数为空</param>
        ///// <returns>返回包含实体类对象的List</returns>
        //public List<T> GetRecordObjects(Dictionary<string, object> dict)
        //{
        //    return ConvertObjectListByDataTable(GetRecords(dict));
        //}

        ///// <summary>
        ///// 批量新增、更新或删除記錄
        ///// </summary>
        ///// <param name="records">包含实体类的List</param>
        ///// <param name="statuses">包含新增、编辑或删除状态的List</param>
        ///// <returns>假如执行成功，返回true，否则返回false</returns>
        //public bool EditRecords(List<T> records, List<RoutineStatus> statuses)
        //{
        //    if (records == null || records.Count == 0)
        //        return true;
        //    else if (statuses == null || statuses.Count != records.Count)
        //        throw new ArgumentException("新增、更新或删除状态的数目不同于实体类对象的数目", "statuses");

        //    string[] sqlStrings = new string[records.Count];
        //    for (int i = 0; i < records.Count; i++)
        //        sqlStrings[i] = batisLike.GetDataManageString(records[i], statuses[i]);
        //    return provider.ExecuteSqlTrans(sqlStrings);
        //}

        ///// <summary>
        ///// 批量新增、编辑或删除记录
        ///// </summary>
        ///// <param name="records">包含实体类对象的List</param>
        ///// <param name="status">新增、编辑或删除状态</param>
        ///// <returns>假如执行成功，返回true，否则返回false</returns>
        //public bool EditRecords(List<T> records, RoutineStatus status)
        //{
        //    string[] sqlStrings = records.Select(record => batisLike.GetDataManageString(record, status)).ToArray();
        //    return provider.ExecuteSqlTrans(sqlStrings);
        //}

        ///// <summary>
        ///// 新增、编辑或删除记录
        ///// </summary>
        ///// <param name="record">实体类对象</param>
        ///// <param name="status">新增、编辑或删除状态</param>
        ///// <returns>返回影响记录条数</returns>
        //public int EditRecord(T record, RoutineStatus status)
        //{
        //    string sqlString = batisLike.GetDataManageString(record, status);
        //    return provider.ExecuteSql(sqlString);
        //}

        ///// <summary>
        ///// 根据条件删除记录
        ///// </summary>
        ///// <param name="dict">字典键值对对象</param>
        ///// <returns>返回影像记录条数</returns>
        //public int DeleteRecords(Dictionary<string, object> dict)
        //{
        //    //string sqlString = batisLike.GetDeleteString(dict);
        //    string sqlString = batisLike.GetSqlStringBySqlMapKey("Delete", dict);
        //    return provider.ExecuteSql(sqlString);
        //}

        ///// <summary>
        ///// 根据ID启用或停用记录
        ///// </summary>
        ///// <param name="obj">实体类对象</param>
        ///// <returns>返回影响记录条数</returns>
        //public int SetEnableById(T obj)
        //{
        //    string sqlString = batisLike.GetSqlStringBySqlMapKey("GetEnableById", obj);
        //    //string sqlString = batisLike.GetSetEnableString(obj);
        //    return provider.ExecuteSql(sqlString);
        //}

        ///// <summary>
        ///// 根据ID启用或停用记录
        ///// </summary>
        ///// <param name="id">记录的唯一ID</param>
        ///// <param name="enable">是否可用，0 不可用，1 可用</param>
        ///// <returns>返回影响记录条数</returns>
        //public int SetEnableById(int id, int enable)
        //{
        //    T obj = Activator.CreateInstance<T>();
        //    PropertyInfo property = obj.GetType().GetProperty("Id");
        //    property.SetValue(obj, id);
        //    property = obj.GetType().GetProperty("Enable");
        //    property.SetValue(obj, enable);
        //    return SetEnableById(obj);
        //}

    }
}
