using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Enums;

namespace CommonLib.Events
{
    /// <summary>
    /// RoutineStatus枚举类的值改变的事件数据类
    /// </summary>
    public class RoutineStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 上一个状态
        /// </summary>
        public RoutineStatus FormerStatus { get; set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public RoutineStatus CurrentStatus { get; set; }
    }
}
