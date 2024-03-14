using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLib.Enums;
using CommonLib.Events;
using CommonLib.Extensions.Reflection;
using CommonLib.Function;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// 基础实体类
    /// </summary>
    public class BaseModel : INotifyPropertyChanged
    {
        #region 事件
        /// <summary>
        /// ID改变事件
        /// </summary>
        public event IdChangedEventHandler IdChanged;

        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        ///// <summary>
        ///// ID改变事件的事件数据类
        ///// </summary>
        //private readonly IdChangedEventArgs eventArgs = new IdChangedEventArgs();

        #region 属性
        #region 数据库通用字段
        private int _id;
        /// <summary>
        /// 记录的唯一编号
        /// </summary>
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id == value)
                    return;
                var eventArgs = new IdChangedEventArgs() { FormerId = _id, CurrentId = value };
                //eventArgs.FormerId = _id;
                _id = value;
                //eventArgs.CurrentId = _id;
                if (IdChanged != null/* && eventArgs.CurrentId != eventArgs.FormerId*/)
                    IdChanged.BeginInvoke(GetType().Name, eventArgs, null, null);
                NotifyPropertyChanged();
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
        #endregion

        /// <summary>
        /// 查询后的记录排序<para/>在底层方法BatisLike.ConvertObjectListByDataTable方法中赋值
        /// </summary>
        public int Rownumber { get; set; }

        /// <summary>
        /// 显示的名称
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 当前记录的状态（新增、修改或删除）
        /// </summary>
        public RoutineStatus RoutineStatus { get; set; }

        /// <summary>
        /// 上一个错误信息
        /// </summary>
        public string LastErrorMessage { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        public BaseModel()
        {
            Id = 0;
            Enable = 1; //是否可用默认为1
            RoutineStatus = RoutineStatus.REGULAR;
        }

        /// <summary>
        /// 通知属性改变
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (/*RoutineStatus == RoutineStatus.DEFAULT || */RoutineStatus == RoutineStatus.REGULAR || RoutineStatus == RoutineStatus.EDIT)
                RoutineStatus = RoutineStatus.EDIT;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
