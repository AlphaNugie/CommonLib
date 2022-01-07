using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLib.Events;
using CommonLib.Function;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// 基础实体类
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// ID改变事件
        /// </summary>
        public event IdChangedEventHandler IdChanged;

        /// <summary>
        /// ID改变事件的事件数据类
        /// </summary>
        private readonly IdChangedEventArgs eventArgs = new IdChangedEventArgs();

        /// <summary>
        /// 记录的唯一编号
        /// </summary>
        private int id = 0;

        /// <summary>
        /// 记录的唯一编号
        /// </summary>
        public int Id
        {
            get { return id; }
            set
            {
                eventArgs.FormerId = id;
                id = value;
                eventArgs.CurrentId = id;
                if (IdChanged != null && eventArgs.CurrentId != eventArgs.FormerId)
                    IdChanged.BeginInvoke(GetType().Name, eventArgs, null, null);
            }
        }

        /// <summary>
        /// 是否可用，可用为1，否则为0
        /// </summary>
        public int Enable { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        public BaseModel()
        {
            Id = 0;
            Enable = 1; //是否可用默认为1
        }
    }
}
