using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLib.UIControlUtil
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

    //public static class ControlUtil
    //{
    //}

    /// <summary>
    /// 控件扩展基础类
    /// </summary>
    public static class ControlUtil
    {
        /// <summary>
        /// 控件线程安全访问（注意：当空间窗体句柄未创建时将不执行任何操作）
        /// .Net2.0中线程安全访问控件扩展方法，可以获取返回值，可能还有其它问题
        /// CrossThreadCalls.SafeInvoke(statusStrip1, new CrossThreadCalls.TaskDelegate(delegate()
        /// {
        ///    tssStatus.Text = "开始任务...";
        /// }));
        /// CrossThreadCalls.SafeInvoke(rtxtChat, new CrossThreadCalls.TaskDelegate(delegate()
        /// {
        ///     rtxtChat.AppendText("测试中");
        /// }));
        /// 参考：http://wenku.baidu.com/view/f0b3ac4733687e21af45a9f9.html
        /// .Net3.5用Lambda简化跨线程访问窗体控件,避免重复的delegate,Invoke
        /// statusStrip1.SafeInvoke(() =>
        /// {
        ///     tsStatus.Text = "开始任务....";
        /// });
        /// rtxtChat.SafeInvoke(() =>
        /// {
        ///     rtxtChat.AppendText("测试中");
        /// });
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <param name="handler">控件访问的委托方法</param>
        public static void SafeInvoke(this Control control, TaskDelegate handler)
        {
            ////当控件没有与其关联的句柄时，退出
            //if (!control.IsHandleCreated)
            //    return;
            //当控件没有与其关联的句柄时，直接执行，然后退出
            if (!control.IsHandleCreated)
            {
                handler.Invoke();
                return;
            }
            //假如需要跨线程
            if (control.InvokeRequired)
            {
                IAsyncResult result = control.BeginInvoke(new InvokeMethodDelegate(SafeInvoke), new object[] { control, handler });
                control.EndInvoke(result);//获取委托执行结果的返回值
                return;
            }
            IAsyncResult result2 = control.BeginInvoke(handler);
            control.EndInvoke(result2);
        }

        /// <summary>
        /// 获取控件中Button控件的数量
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static int GetButtonCount(this Control control)
        {
            IEnumerable<Control> childs = control.Controls.Cast<Control>();
            childs = childs.Where(child => child is Button/* && child.Visible*/);
            return childs.Count();
            //Type type = control.GetType();
            //IEnumerable<FieldInfo> infos = type.GetFields();
            //infos = infos.Where(fieldInfo => fieldInfo.DeclaringType.Name.Equals("Button"));
            //return type.GetFields().Where(fieldInfo => fieldInfo.DeclaringType.Name.Equals("Button")).Count();
        }
    }
}
