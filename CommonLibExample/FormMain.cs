using CommonLib.UIControlUtil.ControlTemplates;
using CommonLibExample.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLibExample
{
    //public partial class FormMain : Form
    public partial class FormMain : FormNotifyBasis
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void Button_TimerEventRaiser_Click(object sender, EventArgs e)
        {
            new FormTimerEventRaiser().Show();
        }

        private void Button_DatabaseTest_Click(object sender, EventArgs e)
        {
            new FormDatabaseTest().Show();
        }

        private void Button_Encrypt_Click(object sender, EventArgs e)
        {
            new FormEncryption().Show();
        }

        private void Button_Socket_Click(object sender, EventArgs e)
        {
            new FormSocket().Show();
        }

        private void Button_Filters_Click(object sender, EventArgs e)
        {
            new FormFilters().Show();
        }

        private void Button_Opc_Click(object sender, EventArgs e)
        {
            new FormOpcTest().Show();
        }

        private void Button_ExitWindows_Click(object sender, EventArgs e)
        {
            new FormExitWindows().Show();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //窗体关闭原因为单击"关闭"按钮或Alt+F4  
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //e.Cancel = true; //取消关闭操作 表现为不关闭窗体
                //HideWindow();
                return;
            }
        }

        private void Button_CustomMessageBox_Click(object sender, EventArgs e)
        {
            new FormCustomMessageBox().Show();
        }
    }
}
