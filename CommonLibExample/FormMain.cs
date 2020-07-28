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
    public partial class FormMain : Form
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
    }
}
