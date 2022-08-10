using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// SQLite数据库内字段类型
    /// </summary>
    public enum SqliteSqlType
    {
        /// <summary>
        /// 双精度类型
        /// </summary>
        DOUBLE,

        /// <summary>
        /// 整型
        /// </summary>
        INTEGER,

        /// <summary>
        /// 数值类型
        /// </summary>
        NUMBER,

        /// <summary>
        /// 字符串类型1
        /// </summary>
        VARCHAR,

        /// <summary>
        /// 字符串类型2
        /// </summary>
        VARCHAR2,
    }

    /// <summary>
    /// 引起冲突时的从句格式
    /// </summary>
    public enum ConflictClause
    {
        /// <summary>
        /// 不执行操作
        /// </summary>
        NONE,

        /// <summary>
        /// 操作回滚（针对事务，非事务语句表现为ABORT）
        /// </summary>
        ROLLBACK,

        /// <summary>
        /// 放弃操作
        /// </summary>
        ABORT,

        /// <summary>
        /// 操作失败（事务中仅放弃出现冲突的语句，而保留前面成功的语句）
        /// </summary>
        FAIL,

        /// <summary>
        /// 忽略（事务中忽略出现冲突的语句，继续执行后面的语句）
        /// </summary>
        IGNORE,

        /// <summary>
        /// 替换（当执行当前语句在唯一UNIQUE或主键PRIMARY KEY约束出现冲突时，删除前面已有的记录中引起冲突的部分并继续执行当前语句；非空NOT NULL约束出现冲突时，将NULL值赋以默认值，假如未指定默认值则执行ABORT；CHECK或外键FOREIGN KEY约束出现冲突时体现为ABORT）
        /// </summary>
        REPLACE
    }
}
