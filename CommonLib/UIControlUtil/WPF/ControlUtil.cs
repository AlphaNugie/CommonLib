using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CommonLib.UIControlUtil.WPF
{
    /// <summary>
    /// 控件访问方法的委托
    /// </summary>
    public delegate void TaskDelegate();

    /// <summary>
    /// 控件安全防卫方法(SafeInvoke)的委托
    /// </summary>
    /// <param name="control"></param>
    /// <param name="handler"></param>
    public delegate void InvokeMethodDelegate(Control control, TaskDelegate handler);
}
