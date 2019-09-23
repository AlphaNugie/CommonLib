using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enums
{
    /// <summary>
    /// 数据维护页面所处状态
    /// </summary>
    public enum RoutineStatus
    {
        /// <summary>
        /// 默认状态
        /// </summary>
        DEFAULT = -1,

        /// <summary>
        /// 普通状态
        /// </summary>
        REGULAR = 0,

        /// <summary>
        /// 新增
        /// </summary>
        ADD = 1,

        /// <summary>
        /// 编辑
        /// </summary>
        EDIT = 2,

        /// <summary>
        /// 删除
        /// </summary>
        DELETE = 3,

        /// <summary>
        /// 逻辑删除（停用）
        /// </summary>
        DELETE_LOGIC = 4
    }
}
