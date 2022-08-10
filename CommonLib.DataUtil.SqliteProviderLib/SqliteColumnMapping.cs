using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// 对应SQLite数据库字段的类
    /// </summary>
    public class SqliteColumnMapping
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public SqliteSqlType SqlType { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool PrimaryKey { get; set; }

        /// <summary>
        /// 引起主键冲突时的操作
        /// </summary>
        public ConflictClause PrimaryKeyConflictClause { get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool AutoIncrement { get; set; }

        /// <summary>
        /// 是否受非空约束
        /// </summary>
        public bool NotNull { get; set; }

        /// <summary>
        /// 引起非空约束冲突时的操作
        /// </summary>
        public ConflictClause NotNullConflictClause { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public double? DefaultValue { get; set; }

        /// <summary>
        /// 是否受唯一约束
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// 引起唯一约束冲突时的操作
        /// </summary>
        public ConflictClause UniqueConflictClause { get; set; }

        /// <summary>
        /// 字段架构
        /// </summary>
        public string Structure { get => GetStructure(); }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="columnName">字段名称</param>
        /// <param name="sqlType">字段类型</param>
        /// <param name="size">字段长度</param>
        /// <param name="notNull">是否非空</param>
        /// <param name="notNullConflictClause">非空冲突的操作</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="unique">是否受唯一约束</param>
        /// <param name="uniqueConflictClause">唯一约束冲突的操作</param>
        /// <param name="primaryKey">是否主键</param>
        /// <param name="primaryKeyConflictClause">主键冲突的操作</param>
        /// <param name="autoIncrement">作为主键是否自增</param>
        public SqliteColumnMapping(string columnName, SqliteSqlType sqlType, int? size = null, bool notNull = false, ConflictClause notNullConflictClause = ConflictClause.FAIL, double? defaultValue = null, bool unique = false, ConflictClause uniqueConflictClause = ConflictClause.FAIL, bool primaryKey = false, ConflictClause primaryKeyConflictClause = ConflictClause.FAIL, bool autoIncrement = false)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentNullException("字段名称不可为空", nameof(columnName));
            ColumnName = columnName;
            SqlType = sqlType;
            Size = size;
            NotNull = notNull;
            NotNullConflictClause = notNullConflictClause;
            DefaultValue = defaultValue;
            Unique = unique;
            UniqueConflictClause = uniqueConflictClause;
            PrimaryKey = primaryKey;
            PrimaryKeyConflictClause = primaryKeyConflictClause;
            AutoIncrement = autoIncrement;
        }

        /// <summary>
        /// 获取当前字段对象的架构语句，默认为修改表
        /// </summary>
        /// <returns></returns>
        public string GetStructure()
        {
            return GetStructure(false);
        }

        /// <summary>
        /// 获取当前字段对象的架构语句，并制定是新建表还是修改表
        /// </summary>
        /// <param name="creating">是否在创建表，为false则代表修改表</param>
        /// <returns></returns>
        public string GetStructure(bool creating)
        {
            string columnName = creating ? ColumnName.Replace(ColumnName, string.Format("[{0}]", ColumnName)) : ColumnName;
            string sqlType = SqlType.ToString() + (Size == null ? string.Empty : "(" + Size.Value + ")");
            string primaryKeyClause = string.Empty, notNullClause = string.Empty, uniqueClause = string.Empty;
            if (PrimaryKey)
            {
                primaryKeyClause = "PRIMARY KEY ";
                if (PrimaryKeyConflictClause != ConflictClause.NONE)
                    primaryKeyClause += string.Format("ON CONFLICT {0} ", PrimaryKeyConflictClause);
                if (AutoIncrement)
                    primaryKeyClause += "AUTOINCREMENT ";
            }
            if (NotNull)
            {
                notNullClause = "NOT NULL ";
                if (NotNullConflictClause != ConflictClause.NONE)
                    notNullClause += string.Format("ON CONFLICT {0} ", NotNullConflictClause);
            }
            if (Unique)
            {
                uniqueClause = "NOT NULL ";
                if (UniqueConflictClause != ConflictClause.NONE)
                    uniqueClause += string.Format("ON CONFLICT {0} ", UniqueConflictClause);
            }
            //string notNullClause = NotNull && NotNullConflictClause != ConflictClause.NONE ? string.Format("NOT NULL ON CONFLICT {0} ", NotNullConflictClause) : string.Empty;
            //string uniqueClause = Unique && UniqueConflictClause != ConflictClause.NONE ? string.Format("UNIQUE ON CONFLICT {0} ", UniqueConflictClause) : string.Empty;
            string defClause = DefaultValue != null ? string.Format("DEFAULT {0}", DefaultValue.Value) : string.Empty;
            return string.Format("{0} {1} {2}{3}{4}{5}", columnName, sqlType, primaryKeyClause, notNullClause, uniqueClause, defClause);
        }
    }
}
