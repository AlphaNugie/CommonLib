using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// 对应Oracle数据库字段的类
    /// </summary>
    public class OracleColumnMapping
    {
        #region 属性
        /// <summary>
        /// 字段名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public OracleSqlType SqlType { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// 是否受非空约束
        /// </summary>
        public bool NotNull { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 字段架构
        /// </summary>
        public string Structure { get => GetStructure(); }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="columnName">字段名称</param>
        /// <param name="sqlType">字段类型</param>
        /// <param name="comment">字段注释</param>
        /// <param name="size">字段长度</param>
        /// <param name="notNull">是否非空</param>
        /// <param name="defaultValue">默认值</param>
        public OracleColumnMapping(string columnName, OracleSqlType sqlType, string comment = null, int? size = null, bool notNull = false, double? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentNullException("字段名称不可为空", nameof(columnName));
            ColumnName = columnName;
            SqlType = sqlType;
            Comment = comment;
            Size = size;
            NotNull = notNull;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// 根据表结构查询结果初始化，查询语句：
        /// <para/>SELECT utc.column_name, utc.data_type, utc.data_length, utc.nullable, utc.data_default, ucc.comments
        /// <para/>FROM user_tab_columns utc
        /// <para/>JOIN user_col_comments ucc ON utc.table_name = ucc.table_name AND utc.column_name = ucc.column_name
        /// <para/>WHERE utc.table_name = 'T_RCMS_MACHINEPOSTURE_TIME';
        /// </summary>
        /// <param name="dataRow">pragma命令的每一个数据行</param>
        public OracleColumnMapping(DataRow dataRow)
        {
            if (dataRow == null || string.IsNullOrWhiteSpace(dataRow.Convert<string>("column_name")))
                throw new ArgumentNullException("列名不可为空", nameof(dataRow));
            ColumnName = dataRow.Convert<string>("column_name");
            SqlType = Enum.TryParse(dataRow.Convert<string>("data_type"), ignoreCase: true, out OracleSqlType type) ? type : OracleSqlType.NONE;
            Size = dataRow.Convert("data_length", 0);
            NotNull = dataRow.Convert<string>("nullable").ToUpper().Equals("Y");
            string def = dataRow.Convert<string>("data_default");
            if (string.IsNullOrWhiteSpace(def))
                DefaultValue = DBNull.Value;
            else if (SqlType == OracleSqlType.VARCHAR2)
                DefaultValue = def;
            else
                DefaultValue = double.TryParse(def, out double value) ? value : 0;
            Comment = dataRow.Convert<string>("comments");
        }

        /// <summary>
        /// 获取当前字段对象的架构语句
        /// </summary>
        /// <returns></returns>
        public string GetStructure()
        {
            string sqlType = SqlType.ToString() + (Size == null ? string.Empty : "(" + Size.Value + ")");
            string defClause = DefaultValue != null ? string.Format(SqlType == OracleSqlType.VARCHAR2 ? " DEFAULT '{0}'" : " DEFAULT {0}", DefaultValue) : string.Empty;
            string notNullClause = NotNull ? " NOT NULL" : string.Empty;
            return string.Format("{0} {1}{2}{3}", ColumnName, sqlType, defClause, notNullClause);
        }
    }
}
