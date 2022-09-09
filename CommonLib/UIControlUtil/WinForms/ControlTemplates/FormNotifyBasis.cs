using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLib.UIControlUtil.ControlTemplates
{
    /// <summary>
    /// 带有通知区域图标与隐藏功能的窗体，假如新窗体继承自此窗体，则需要在新窗体的FormClosing事件中（假如有的话）加上如下代码
    /// if (e.CloseReason == CloseReason.UserClosing)
    ///     return;
    /// </summary>
    public partial class FormNotifyBasis : Form
    {
        private FormWindowState _prevWindowState, _prevWindowStateNonMini; //上一个窗体状态，以及上一个非最小化窗体状态
        private Point _prevLocation;
        private Size _prevSize;
        private bool _resizing; //是否在调整尺寸，假如为true则暂时不响应Resized事件

        public FormNotifyBasis()
        {
            InitializeComponent();
            _prevWindowStateNonMini = WindowState != FormWindowState.Minimized ? WindowState : FormWindowState.Normal;
            _prevLocation = DesktopLocation;
            _prevSize = Size;
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            //窗体关闭原因为单击"关闭"按钮或Alt+F4  
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //取消关闭操作 表现为不关闭窗体
                HideWindow();
                return;
            }
        }

        ///// <summary>
        ///// 点击任务栏实现最小化与还原
        ///// </summary>
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        const int WS_MINIMIZEBOX = 0x00020000;
        //        CreateParams cp = base.CreateParams;
        //        cp.Style |= WS_MINIMIZEBOX;   // 允许最小化操作  
        //        return cp;
        //    }
        //}

        #region 功能
        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public void HideWindow()
        {
            _prevWindowState = WindowState;
            Hide();
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        public void ShowWindow()
        {
            Show();
            //if (_prevWindowState == FormWindowState.Minimized)
            //    WindowState = FormWindowState.Normal;
            if (_prevWindowState == FormWindowState.Minimized)
            {
                _resizing = true;
                WindowState = _prevWindowStateNonMini;
                DesktopLocation = _prevLocation;
                Size = _prevSize;
            }
            _resizing = false;
            Activate();
        }

        /// <summary>
        /// 退出
        /// </summary>
        public void Exit()
        {
            if (MessageBox.Show("是否退出程序？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                return;

            notifyIcon_Main.Visible = false;
            Close();
            Environment.Exit(0);
            //Application.Exit();
        }
        #endregion

        #region 通知区域事件
        private void NotifyIcon_Main_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            ShowWindow();
        }

        /// <summary>
        /// 提示栏退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_NotifyExit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        /// <summary>
        /// 提示栏显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_Show_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        /// <summary>
        /// 提示栏隐藏窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_Hide_Click(object sender, EventArgs e)
        {
            HideWindow();
        }
        #endregion

        private void FormNotifyBasis_TextChanged(object sender, EventArgs e)
        {
            notifyIcon_Main.Text = Text;
        }

        private void FormNotifyBasis_Resize(object sender, EventArgs e)
        {
            if (_resizing)
                return;

            //最小化时隐藏窗体
            if (WindowState == FormWindowState.Minimized)
                HideWindow();
            else
            {
                _prevWindowStateNonMini = WindowState;
                _prevLocation = DesktopLocation;
                _prevSize = Size;
            }
        }
    }
}
