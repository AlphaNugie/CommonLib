using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enums
{
    /// <summary>
    /// 数值变化趋势
    /// </summary>
    public enum ValueTrend
    {
        /// <summary>
        /// 下降
        /// </summary>
        Decreasing = -1,

        /// <summary>
        /// 不变
        /// </summary>
        Unchanged = 0,

        /// <summary>
        /// 上升
        /// </summary>
        Increasing = 1
    }
}
