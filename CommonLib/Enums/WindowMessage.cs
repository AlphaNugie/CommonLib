using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enums
{
    /// <summary>
    /// 窗口消息枚举
    /// </summary>
    public enum WindowMessage
    {
        /// <summary>
        /// 窗体创建
        /// </summary>
        CREATE = 0x0001,

        /// <summary>
        /// 窗体销毁
        /// </summary>
        DESTROY = 0x0002,

        /// <summary>
        /// 热键
        /// </summary>
        HOTKEY = 0x0312
    }
}
