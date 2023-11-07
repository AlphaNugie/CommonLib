using CommonLibExample.Tasks;
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
    public partial class FormTests : Form
    {
        public FormTests()
        {
            InitializeComponent();
        }

        private void Button_TimerEventRaiser_Click(object sender, EventArgs e)
        {
            ExampleTaskTest.Run();
        }
    }
}
