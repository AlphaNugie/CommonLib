using CommonLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Events
{
    /// <summary>
    /// 服务状态变化事件参数
    /// </summary>
    public class ServiceStateEventArgs : EventArgs
    {
        /// <summary>
        /// 服务状态消息
        /// </summary>
        public string StateInfo { get; set; }

        /// <summary>
        /// 服务状态
        /// </summary>
        public ServiceState State { get; set; }

        /// <summary>
        /// 以给定的状态信息以及状态对象初始化
        /// </summary>
        /// <param name="stateInfo"></param>
        /// <param name="state"></param>
        public ServiceStateEventArgs(string stateInfo, ServiceState state)
        {
            StateInfo = stateInfo;
            State = state;
        }
    }
}
