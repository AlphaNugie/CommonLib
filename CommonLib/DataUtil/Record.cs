using CommonLib.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// 数据记录基础类
    /// </summary>
    public class Record : INotifyPropertyChanged
    {
        private int _id;
        /// <summary>
        /// 记录ID
        /// </summary>
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 显示的名称
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked { get; set; }

        //{
        //    get { return ; }
        //    set
        //    {
        //        if ( != value)
        //        {
        //             = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

        //public int Changed { get; set; }

        /// <summary>
        /// 记录状态
        /// </summary>
        public RoutineStatus RoutineStatus { get; set; }

        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 构造器
        /// </summary>
        public Record()
        {
            this.RoutineStatus = RoutineStatus.DEFAULT;
        }

        /// <summary>
        /// 通知属性改变
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.RoutineStatus == RoutineStatus.DEFAULT || this.RoutineStatus == RoutineStatus.REGULAR || this.RoutineStatus == RoutineStatus.EDIT)
                this.RoutineStatus = RoutineStatus.EDIT;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
