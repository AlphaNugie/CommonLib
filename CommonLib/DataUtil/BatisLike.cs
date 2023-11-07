using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CommonLib.Clients;
using CommonLib.Enums;
using CommonLib.Function;
using CommonLib.Helpers;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// 实验性质的iBatis相似类
    /// 
    /// 在调用DLL的解决方案App.Config文件中添加如下行：
    /// <![CDATA[
    /// <!-- SQL语句XML文件地址，为EXE文件同级目录的子目录 --> <add key="SqlMapperFolder" value="SqlMapper"/>
    /// ]]>
    /// 其中SqlMapper为存放SQL语句XML文件的文件夹，部署后与启动项目同级
    /// 开发时将这个文件夹放入项目下的Mapper文件夹（没有就新建）
    /// 
    /// 查询：Get
    /// 新增：Insert
    /// 更新：Update
    /// 删除：Delete
    /// 
    /// XML文件格式：
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <SqlMapping>
    ///   <!-- 数据维护模块 -->
    ///   <SqlMaps key="Protocol">
    ///     <ResultMap id="PropertyMap">
    ///       <Result property="fkfsdm" jdbcType="VARCHAR" column="fkfsdm"/>
    ///     </ResultMap>
    ///     <Sql key="Insert">
    ///                  insert into protocol ...
    ///     </Sql>
    ///     <Sql key="Update">
    ///                  update protocol set ...
    ///     </Sql>
    ///   </SqlMaps>
    /// </SqlMapping>
    /// ]]>
    /// </summary>
    public class BatisLike
    {
        private string mapperName = string.Empty;

        /// <summary>
        /// Mapper文件的目录
        /// </summary>
        public static string DefaultMapperPath { get; set; } = FileSystemHelper.StartupPath + FileSystemHelper.DirSeparator + ConfigurationManager.AppSettings["SqlMapperFolder"];

        /// <summary>
        /// Mapper文件的目录
        /// </summary>
        //public /*static */string MapperPath { get; set; } = FileSystemHelper.StartupPath + FileSystemHelper.DirSeparator + ConfigurationManager.AppSettings["SqlMapperFolder"];
        public /*static */string MapperPath { get; set; } = DefaultMapperPath;

        /// <summary>
        /// Mapper文件名称
        /// </summary>
        public string MapperName
        {
            get { return mapperName; }
            set
            {
                mapperName = value;
                PropertyMappers = GetPropertyMappers(mapperName + ".PropertyMap");
            }
        }

        /// <summary>
        /// 实体类属性与数据库字段对应关系
        /// </summary>
        public PropertyMapper[] PropertyMappers { get; set; }

        /// <summary>
        /// 构造器，Mapper文件名称默认为空字符串
        /// </summary>
        public BatisLike() : this(string.Empty) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="mapperName">Mapper文件名称</param>
        public BatisLike(string mapperName)
        {
            MapperName = mapperName;
            //PropertyMappers = GetPropertyMappers(MapperName + ".PropertyMap");
        }

        /// <summary>
        /// 通过SqlMap的关键词与另外提供的键值对获取SQL语句
        /// </summary>
        /// <param name="sqlMapKey">SqlMap的关键词</param>
        /// <param name="dict">提供其他信息的键值对</param>
        /// <returns></returns>
        public string GetSqlStringBySqlMapKey(string sqlMapKey, Dictionary<string, object> dict)
        {
            string sqlString = GetSql(MapperName + "." + sqlMapKey); //获取源字符串
            sqlString = ConvertSqlStringByKeyValue(sqlString, dict); //处理
            //string sqlString = GetSqlStringBySqlMapKey(sqlMapKey);
            //if (dict != null && dict.Keys.Count > 0)
            //    sqlString = ConvertSqlStringByKeyValue(sqlString, dict);
            return sqlString;
        }

        /// <summary>
        /// 通过SqlMap的关键词与另外提供的键值对获取SQL语句
        /// </summary>
        /// <typeparam name="T">用作转换参考的实体类</typeparam>
        /// <param name="sqlMapKey">SqlMap的关键词</param>
        /// <param name="obj">提供其他信息的实体类对象</param>
        /// <returns></returns>
        public string GetSqlStringBySqlMapKey<T>(string sqlMapKey, T obj)
        {
            string sqlString = GetSql(MapperName + "." + sqlMapKey); //获取源字符串
            sqlString = ConvertSqlStringByObject<T>(sqlString, obj); //处理
            //string sqlString = GetSqlStringBySqlMapKey(sqlMapKey);
            //if (obj != null)
            //    sqlString = ConvertSqlStringByObject<T>(sqlString, obj);
            return sqlString;
        }

        /// <summary>
        /// 通过SqlMap的关键词获取SQL语句
        /// </summary>
        /// <param name="sqlMapKey">SqlMap的关键词</param>
        /// <returns></returns>
        public string GetSqlStringBySqlMapKey(string sqlMapKey)
        {
            return GetSqlStringBySqlMapKey(sqlMapKey, null);
            //return GetSql(MapperName + "." + sqlMapKey);
        }

        ///// <summary>
        ///// 获取Get查询语句并根据键值对进行转换
        ///// </summary>
        ///// <param name="dict">字典键值对对象，可为空</param>
        ///// <returns>返回查询语句</returns>
        //public string GetQueryString(Dictionary<string, object> dict)
        //{
        //    //return ConvertSqlStringByKeyValue(GetSql(MapperName + ".Get"), dict);
        //    return GetSqlStringBySqlMapKey("Get", dict);
        //}

        /// <summary>
        /// 获取数据操纵（Insert、Update或Delete）语句，并根据对象属性进行转换
        /// </summary>
        /// <typeparam name="T">用作转换参考的实体类</typeparam>
        /// <param name="obj">实体类对象，可为空</param>
        /// <param name="status">表示新增、更新或删除的枚举对象</param>
        /// <returns>返回新增、更新或删除语句</returns>
        public string GetDataManageString<T>(T obj, RoutineStatus status)
        {
            if (status == RoutineStatus.REGULAR)
                return string.Empty;

            string keyTail = string.Empty;
            switch ((int)status)
            {
                case (int)RoutineStatus.ADD:
                    //keyTail = ".Insert";
                    keyTail = "Insert";
                    break;
                case (int)RoutineStatus.EDIT:
                    //keyTail = ".Update";
                    keyTail = "Update";
                    break;
                case (int)RoutineStatus.DELETE:
                    //keyTail = ".Delete";
                    keyTail = "Delete";
                    break;
            }
            //string sqlString = ConvertSqlStringByObject<T>(GetSql(MapperName + keyTail), obj);
            string sqlString = GetSqlStringBySqlMapKey(keyTail, obj);
            return sqlString;
        }

        ///// <summary>
        ///// 获取Delete语句并根据键值对进行转换
        ///// </summary>
        ///// <param name="dict">字典键值对对象，可为空</param>
        ///// <returns>返回删除语句</returns>
        //public string GetDeleteString(Dictionary<string, object> dict)
        //{
        //    //return ConvertSqlStringByKeyValue(GetSql(MapperName + ".Delete"), dict);
        //    return GetSqlStringBySqlMapKey("Delete", dict);
        //}

        ///// <summary>
        ///// 获取启用/停用语句并转换（根据是否可用属性改变是否可用字段的值）
        ///// </summary>
        ///// <typeparam name="T">转换参考实体类</typeparam>
        ///// <param name="obj">实体类对象</param>
        ///// <returns>返回逻辑删除字符串</returns>
        //public string GetSetEnableString<T>(T obj)
        //{
        //    //return ConvertSqlStringByObject<T>(GetSql(MapperName + ".SetEnableById"), obj);
        //    return GetSqlStringBySqlMapKey("SetEnableById", obj);
        //}

        /// <summary>
        /// 将含参数的SQL语句根据键值对进行转换（如Select或Delete语句），参数形为#{some,jdbcType=XXX}或#{prop}
        /// </summary>
        /// <param name="sqlString">待转换的的查询语句</param>
        /// <param name="dict">字典键值对对象，可为空</param>
        /// <returns>返回转换后的语句</returns>
        public string ConvertSqlStringByKeyValue(string sqlString, Dictionary<string, object> dict)
        {
            //找出所有符合正则表达式的字符串，假如没有匹配项，直接返回源字符串
            string[] parameters = RegexMatcher.FindMatches(sqlString, RegexMatcher.RegexPattern_BatisParam);
            if (parameters == null || parameters.Length == 0)
                //return null;
                return sqlString;
            else if (dict == null)
                dict = new Dictionary<string, object>();

            string keyName; //字典中key的名称
            string jdbcType; //待转换数据库类型名称
            string realParam; //待存储属性的值
            int startIndex, midIndex, endIndex;
            try
            {
                foreach (string param in parameters)
                {
                    //找出各字符索引，并根据索引找出属性名称与转换类型名称
                    startIndex = param.IndexOf('{');
                    endIndex = param.IndexOf('}');
                    midIndex = param.Contains(',') ? param.IndexOf(',') : endIndex;
                    keyName = param.Substring(startIndex + 1, midIndex - startIndex - 1);
                    jdbcType = param.Contains(",jdbcType=") ? param.Substring(param.IndexOf('=') + 1, endIndex - param.IndexOf('=') - 1) : string.Empty;

                    //根据对象属性名称找出属性类型，并根据类型决定属性存储方式(值)
                    //属性值
                    object realParamObj = dict.ContainsKey(keyName) ? dict[keyName] : null;
                    ////假如属性为空
                    //if (realParam_obj == null)
                    //    realParam = jdbcType.Equals("VARCHAR") ? string.Empty : "null"; //根据转换类型是否为VARCHAR决定储存的值
                    //else
                    //    realParam = realParam_obj.ToString();
                    ////假如欲转换为VARCHAR或属性类型为string，字符串前后添加单引号，并替换参数
                    //realParam = string.Format(jdbcType.Equals("VARCHAR") ? "'{0}'" : "{0}", realParam);
                    //假如属性值为空，则存储值为字符串"null"，否则为实际值的字符串形式
                    bool isNull = realParamObj == null;
                    realParam = isNull ? "null" : realParamObj.ToString();

                    //假如欲转换为VARCHAR或属性类型为string（且值不为空），则字符串前后添加单引号，并替换参数
                    realParam = string.Format(jdbcType.Equals("VARCHAR") && !isNull ? "'{0}'" : "{0}", realParam);
                    sqlString = sqlString.Replace(param, realParam);
                }
            }
            catch (Exception e)
            {
                FileClient.WriteExceptionInfo(e, string.Format("解析Insert语句时出错，Insert语句：{0}", sqlString), true);
                throw;
                //return null;
            }

            return sqlString;
        }

        /// <summary>
        /// 将含参数的SQL语句通过实体类进行转换（如INSERT或UPDATE语句），参数形为#{some,jdbcType=XXX}或#{prop}
        /// </summary>
        /// <typeparam name="T">用作转换参考的实体类</typeparam>
        /// <param name="sqlString">待转换的INSERT或UPDATE语句</param>
        /// <param name="obj">实体类对象，可为空</param>
        /// <returns>返回转换后的语句</returns>
        public string ConvertSqlStringByObject<T>(string sqlString, T obj)
        {
            //找出所有符合正则表达式的字符串，假如没有匹配项或对象为空，直接返回源字符串
            string[] parameters = RegexMatcher.FindMatches(sqlString, RegexMatcher.RegexPattern_BatisParam);
            if (parameters == null || parameters.Length == 0 || obj == null)
                //return null;
                return sqlString;
            else if (obj == null)
                obj = Activator.CreateInstance<T>();

            string propName; //属性名称
            string jdbcType; //待转换数据库类型名称
            string realParam; //待存储属性的值
            int startIndex, midIndex, endIndex;
            try
            {
                foreach (string param in parameters)
                {
                    //找出各字符索引，并根据索引找出属性名称与转换类型名称
                    startIndex = param.IndexOf('{');
                    endIndex = param.IndexOf('}');
                    midIndex = param.Contains(',') ? param.IndexOf(',') : endIndex;
                    propName = param.Substring(startIndex + 1, midIndex - startIndex - 1);
                    jdbcType = param.Contains(",jdbcType=") ? param.Substring(param.IndexOf('=') + 1, endIndex - param.IndexOf('=') - 1) : string.Empty;

                    //根据对象属性名称找出属性类型，并根据类型决定属性存储方式(值)，假如property为null则抛出错误
                    PropertyInfo property = obj.GetType().GetProperty(propName) ?? throw new MissingMemberException(string.Format("{0}类中不存在属性{1}", obj.GetType().ToString(), propName));
                    object realParam_obj = property.GetValue(obj); //属性值
                    //假如属性为空
                    if (realParam_obj == null)
                        realParam = jdbcType.Equals("VARCHAR") ? string.Empty : "null"; //根据转换类型是否为VARCHAR决定储存的值
                        ////假如欲转换为VARCHAR2
                        //if (jdbcType.Equals("VARCHAR"))
                        //    realParam = string.Empty;
                        ////假如为其他类型空值
                        //else
                        //    realParam = "null";
                    else
                        realParam = realParam_obj.ToString();

                    //假如欲转换为VARCHAR或属性类型为string，字符串前后添加单引号，并替换参数
                    realParam = string.Format(jdbcType.Equals("VARCHAR") || property.PropertyType.Name.Equals("String") ? "'{0}'" : "{0}", realParam);
                    sqlString = sqlString.Replace(param, realParam);
                    //sqlString = sqlString.Replace(param, string.Format(jdbcType.Equals("VARCHAR") || property.GetType().ToString().Equals("System.String") ? "'{0}'" : "{0}", obj.GetType().GetProperty(propName).GetValue(obj).ToString()));
                }
            }
            catch (Exception e)
            {
                FileClient.WriteExceptionInfo(e, string.Format("解析Insert语句时出错，Insert语句：{0}，实体类：{1}", sqlString, obj.GetType()), true);
                throw;
                //return null;
            }

            return sqlString;
        }

        /// <summary>
        /// 读取指定SQL语句，假如指定SQL语句的key不存在，则返回null
        /// </summary>
        /// <param name="key">在XML文件中定义的SQL语句名称，格式形为“[实体类名称].[节点ID]”</param>
        /// <returns>返回sql字符串</returns>
        public string GetSql(string key)
        {
            string sqlString = null;
            try
            {
                string[] sqlLayers = key.Split('.');
                XmlNode xmlNode = ReadMapping(sqlLayers[0]);
                if (xmlNode == null || !xmlNode.HasChildNodes)
                    return sqlString;
                XmlNodeList list = xmlNode.SelectNodes("Sql"); //获取SqlMapper下的所有Sql XmlNode节点（即SQL语句）

                //根据Key获取SQL语句
                sqlString = list.Cast<XmlNode>().ToList().Find(node => node.Attributes["key"].Value.Equals(sqlLayers[1])).InnerText;
                //sqlString = string.IsNullOrWhiteSpace(sqlString) ? xmlNode.SelectNodes("SqlMap[@key='" + sqlLayers[1] + "']")[0].InnerText : sqlString;

                //foreach (XmlNode node in list)
                //    if (node.Attributes["key"].Value.Equals(sqlLayers[1]))
                //        return node.InnerText;
                //sqlString = xmlNode.SelectNodes("SqlMap[@key='" + sqlLayers[1] + "']")[0].InnerText;
            }
            catch (Exception e)
            {
                FileClient.WriteExceptionInfo(e, string.Format("从XML文件中获取SQL语句出错，key: {0}", key), true);
                throw;
            }

            return sqlString;
        }

        /// <summary>
        /// 根据XML节点标识(key)，获取属性、字段对应关系实体类数组，假如key不存在，则返回null
        /// </summary>
        /// <param name="key">在XML文件中定义的ResultMap节点标识，格式形为“[实体类名称].[节点ID]”</param>
        /// <returns>返回属性字段对应实体类数组</returns>
        public PropertyMapper[] GetPropertyMappers(string key)
        {
            PropertyMapper[] mappers = null;
            try
            {
                string[] sqlLayers = key.Split('.');
                XmlNode xmlNode = ReadMapping(sqlLayers[0]);
                if (xmlNode == null || !xmlNode.HasChildNodes)
                    return mappers;
                XmlNodeList list = xmlNode.SelectNodes("ResultMap"); //获取SqlMapper下的所有ResultMap XmlNode节点

                //根据Key获取SQL语句
                XmlNode resultMapNode = list.Cast<XmlNode>().ToList().Find(node => node.Attributes["id"].Value.Equals(sqlLayers[1]));
                if (resultMapNode != null && resultMapNode.HasChildNodes)
                    mappers = resultMapNode.SelectNodes("Result").Cast<XmlNode>().Select(node => new PropertyMapper() { PropertyName = node.Attributes["property"].Value, JdbcType = node.Attributes["jdbcType"].Value, ColumnName = node.Attributes["column"].Value }).ToArray();
            }
            catch (Exception e)
            {
                FileClient.WriteExceptionInfo(e, string.Format("从XML文件中获取实体类属性字段对应关系出错，key: {0}", key), true);
                throw;
            }

            return mappers;
        }

        /// <summary>
        /// 读取配置文件中的Mapping节点
        /// </summary>
        /// <param name="key">mapping节点名称，形如Protocol.GetNavigation</param>
        /// <returns>Mapping节点下的sqlMap</returns>
        private XmlNode ReadMapping(string key)
        {
            string fileName = MapperPath + FileSystemHelper.DirSeparator + MapperName + ".xml";
            XmlDocument Xdoc = new XmlDocument();
            Xdoc.Load(fileName);
            XmlNodeList list = Xdoc.SelectNodes("SqlMapping/SqlMaps");

            //假如未找到，返回null
            return list.Cast<XmlNode>().ToList().Find(node => node.Attributes["key"].Value.Equals(key));

            //foreach (XmlNode node in list)
            //    if (node.Attributes["key"].Value.Equals(key))
            //        return node;
            //return null;
        }

        /// <summary>
        /// 将数据行的数据转换为实体类
        /// </summary>
        /// <typeparam name="T">最终转换为的实体类</typeparam>
        /// <param name="dataRow">包含待转换数据的数据行</param>
        /// <returns>返回转换后的实体类对象</returns>
        public T ConvertObjectByDataRow<T>(DataRow dataRow) where T : BaseModel/* where T : Record*/
        {
            T obj = Activator.CreateInstance<T>();
            if (PropertyMappers == null || PropertyMappers.Length == 0)
                return obj;

            foreach (PropertyMapper mapper in PropertyMappers)
            {
                PropertyInfo property = obj.GetType().GetProperty(mapper.PropertyName);
                if (property == null)
                    continue;
                object value;
                string rawValue = string.Empty;
                try { rawValue = dataRow[mapper.ColumnName].ToString(); }
                catch { }
                if (mapper.JdbcType == "INTEGER")
                    value = string.IsNullOrWhiteSpace(rawValue) ? (int)0 : int.Parse(rawValue);
                else if (mapper.JdbcType == "BYTE")
                    value = string.IsNullOrWhiteSpace(rawValue) ? (byte)0 : byte.Parse(rawValue);
                else if (mapper.JdbcType == "USHORT")
                    value = string.IsNullOrWhiteSpace(rawValue) ? (ushort)0 : ushort.Parse(rawValue);
                else if (mapper.JdbcType == "DOUBLE")
                    value = string.IsNullOrWhiteSpace(rawValue) ? 0 : double.Parse(rawValue);
                else if (mapper.JdbcType == "VARCHAR")
                    value = rawValue;
                else if (mapper.JdbcType == "DATE")
                    value = string.IsNullOrWhiteSpace(rawValue) ? new DateTime() : DateTime.ParseExact(rawValue, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                else
                    value = dataRow[mapper.ColumnName];
                //if (mapper.JdbcType == "BYTE")
                //{
                //    value = Convert.ToByte(rawValue);
                //    property.SetValue(obj, value);
                //}
                property.SetValue(obj, value);
            }
            //if (obj is Record)
            //   //(obj as Record).RoutineStatus = RoutineStatus.DEFAULT;
            //   (obj as Record).RoutineStatus = RoutineStatus.REGULAR;
            obj.RoutineStatus = RoutineStatus.REGULAR;

            return obj;
        }

        /// <summary>
        /// 将数据表的数据转换为实体类List
        /// </summary>
        /// <typeparam name="T">最终转换为的实体类</typeparam>
        /// <param name="dataTable">包含待转换数据的数据表</param>
        /// <returns>返回转换后的实体类对象List</returns>
        public List<T> ConvertObjectListByDataTable<T>(DataTable dataTable) where T : BaseModel/* where T : Record*/
        {
            List<T> list = new List<T>();
            //if (dataTable == null || dataTable.Rows.Count == 0)
            //    return new List<T>();
            int rownum = 1;
            if (dataTable != null && dataTable.Rows.Count != 0)
                //list = dataTable.Rows.Cast<DataRow>().Select(dataRow => ConvertObjectByDataRow<T>(dataRow)).ToList();
                list = dataTable.Rows.Cast<DataRow>().Select(dataRow =>
                {
                    var obj = ConvertObjectByDataRow<T>(dataRow);
                    obj.Rownumber = rownum++;
                    return obj;
                }).ToList();
            dataTable?.Dispose();
            ////更新记录的排序序号
            //if (list != null && list.Count > 0)
            //    for (int i = 1; i <= list.Count; i++)
            //        list[i - 1].Rownumber = i;
            return list;
        }
    }

    /// <summary>
    /// 体现实体类属性与数据库表字段对应关系的实体类
    /// </summary>
    public class PropertyMapper
    {
        /// <summary>
        /// 实体类属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string JdbcType { get; set; }

        /// <summary>
        /// 数据库表字段名称
        /// </summary>
        public string ColumnName { get; set; }
    }
}
