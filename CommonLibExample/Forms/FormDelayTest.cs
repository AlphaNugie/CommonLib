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
    public partial class FormDelayTest : Form
    {
        public FormDelayTest()
        {
            InitializeComponent();
        }

        private void Button_DelayAction_Click(object sender, EventArgs e)
        {
            Task.Delay((int)numeric_SecondsDelayed.Value * 1000).Wait();
            label_Time.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
    }
}
