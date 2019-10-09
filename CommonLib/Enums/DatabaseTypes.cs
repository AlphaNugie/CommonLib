using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enums
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseTypes
    {
        /// <summary>
        /// Oracle 数据库
        /// </summary>
        Oracle = 1,

        /// <summary>
        /// SQL Server数据库
        /// </summary>
        SqlServer = 2, 

        /// <summary>
        /// SQLite文件数据库
        /// </summary>
        Sqlite = 3,

        /// <summary>
        /// MySQL数据库
        /// </summary>
        MySql = 4
    }
}
