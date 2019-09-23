using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Events
{
    /// <summary>
    /// BaseModel中ID改变事件的事件数据类
    /// </summary>
    public class IdChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 上一个ID
        /// </summary>
        public int FormerId { get; set; }

        /// <summary>
        /// 现在的ID
        /// </summary>
        public int CurrentId { get; set; }
    }
}
