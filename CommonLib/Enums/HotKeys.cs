using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enums
{
    /// <summary>
    /// 热键组合枚举
    /// </summary>
    public enum HotKeys
    {
        /// <summary>
        /// Alt键
        /// </summary>
        ALT = 1,

        /// <summary>
        /// Ctrl键
        /// </summary>
        CTRL = 2,

        /// <summary>
        /// Ctrl + Alt 组合键
        /// </summary>
        CTRL_ALT = 3,

        /// <summary>
        /// Shift键
        /// </summary>
        SHIFT = 4,

        /// <summary>
        /// Alt + Shift 组合键
        /// </summary>
        ALT_SHIFT = 5,

        /// <summary>
        /// Ctrl + Shift 组合键
        /// </summary>
        CTRL_SHIFT = 6,

        /// <summary>
        /// Ctrl + Alt + Shift 组合键
        /// </summary>
        CTRL_ALT_SHIFT = 7
    }
}
