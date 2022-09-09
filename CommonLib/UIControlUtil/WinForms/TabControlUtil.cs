using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Controls;
using System.Windows.Forms;

namespace CommonLib.UIControlUtil
{
    public static class TabControlUtil
    {
        /// <summary>
        /// 在TabControl页集合中加载窗体对象，停靠方式默认为填充(Fill)，假如已存在名称(Name)相同的TabPage则放弃操作
        /// </summary>
        /// <param name="tabControl">需加载Form窗体的TabControl控件</param>
        /// <param name="form">需在TabPage页中加载或显示的窗体对象</param>
        public static void ShowForm(this TabControl tabControl, Form form)
        {
            tabControl.ShowForm(form, DockStyle.Fill);
        }

        public delegate void ShowFormHandler(Form form, DockStyle? dock);
        /// <summary>
        /// 在TabControl页集合中加载窗体对象，指定停靠方式，假如已存在名称(Name)相同的TabPage则放弃操作
        /// </summary>
        /// <param name="tabControl">需加载Form窗体的TabControl控件</param>
        /// <param name="form">需在TabPage页中加载或显示的窗体对象</param>
        /// <param name="dock">停靠方式，假如为null则不停靠</param>
        public static void ShowForm(this TabControl tabControl, Form form, DockStyle? dock)
        {
            if (tabControl == null || form == null)
                return;

            if (tabControl.InvokeRequired)
            {
                ShowFormHandler handler = new ShowFormHandler(tabControl.ShowForm);
                tabControl.Invoke(handler, form, dock);
                return;
            }

            //假如Tab页已存在，选中该页面
            foreach (TabPage tabPage in tabControl.TabPages)
            {
                if (tabPage.Name == form.Name)
                {
                    tabControl.SelectedTab = tabPage;
                    return;
                }
            }

            //在TabControl中显示包含该页面的TabPage
            form.TopLevel = false; //不置顶
            if (dock != null)
                form.Dock = dock.Value; //控件停靠方式
            form.FormBorderStyle = FormBorderStyle.None; //页面无边框
            TabPage page = new TabPage
            {
                Text = form.Text,
                Name = form.Name,
                AutoScroll = true
            };
            page.Controls.Add(form);
            tabControl.TabPages.Add(page);
            tabControl.SelectedTab = page;
            form.Show();
            //tabControl.Invoke(new MethodInvoker(delegate
            //{
            //    tabControl.TabPages.Add(page);
            //    tabControl.SelectedTab = page;
            //    form.Show();
            //}));
        }

        /// <summary>
        /// 释放TabControl的TabPage资源(WinForm)
        /// </summary>
        /// <param name="tabControl">需释放TabPage资源的TabControl控件</param>
        /// <param name="page">TabPage对象</param>
        public static void DisposeTabPage(this TabControl tabControl, TabPage page)
        {
            if (tabControl == null || page == null)
                return;

            page.DisposeTabPage();
            //if (page.Controls.Count > 0 && page.Controls[0] is Form)
            //{
            //    Form form = (Form)page.Controls[0];
            //    if (form != null)
            //    {
            //        form.Close();
            //        form.Dispose();
            //    }
            //}

            //page.Dispose();
        }

        /// <summary>
        /// 释放TabPage资源(WinForm)
        /// </summary>
        /// <param name="page">需进行资源释放的TabPage对象</param>
        public static void DisposeTabPage(this TabPage page)
        {
            if (page == null)
                return;

            if (page.Controls.Count > 0 && page.Controls[0] is Form form)
            {
                if (form != null)
                {
                    form.Close();
                    form.Dispose();
                }
            }
            //if (page.Controls.Count > 0 && page.Controls[0] is Form)
            //{
            //    Form form = (Form)page.Controls[0];
            //    if (form != null)
            //    {
            //        form.Close();
            //        form.Dispose();
            //    }
            //}

            page.Dispose();
        }

        /// <summary>
        /// 释放TabControl的所有TabPage资源
        /// </summary>
        /// <param name="tabControl">需释放所有TabPage资源的TabControl控件</param>
        public static void DisposeAllTabPages(this TabControl tabControl)
        {
            if (tabControl == null)
                return;
            foreach (TabPage page in tabControl.TabPages)
                tabControl.DisposeTabPage(page);
        }

        /// <summary>
        /// 释放TabControl的选中TabPage资源
        /// </summary>
        /// <param name="tabControl">需释放选中TabPage资源的TabControl控件</param>
        public static void DisposeSelectedTabPage(this TabControl tabControl)
        {
            tabControl.DisposeTabPage(tabControl.SelectedTab);
        }
    }
}
