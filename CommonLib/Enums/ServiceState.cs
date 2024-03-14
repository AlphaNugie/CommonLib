using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enums
{
    /// <summary>
    /// 服务状态
    /// </summary>
    public enum ServiceState
    {
        /// <summary>
        /// 空闲
        /// </summary>
        Idle = -1,

        /// <summary>
        /// 已停止
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// 已启动
        /// </summary>
        Started = 1,

        /// <summary>
        /// 重新启动中
        /// </summary>
        Restarting = 2,

        /// <summary>
        /// 重新启动完成
        /// </summary>
        Restarted = 3,
    }
}
