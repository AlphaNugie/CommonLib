using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLibExample.Forms
{
    public partial class FormExitWindows : Form
    {
        public FormExitWindows()
        {
            InitializeComponent();
        }

        private void Button_Shutdown_Click(object sender, EventArgs e)
        {
            ExitWindowsUtils.Shutdown(checkBox_Force.Checked);
        }

        private void Button_Reboot_Click(object sender, EventArgs e)
        {
            ExitWindowsUtils.Reboot(checkBox_Force.Checked);
        }

        private void Button_Logoff_Click(object sender, EventArgs e)
        {
            ExitWindowsUtils.Logoff(checkBox_Force.Checked);
        }
    }
}
