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
    public partial class FormDatabaseTest : Form
    {
        public FormDatabaseTest()
        {
            InitializeComponent();
        }

        private void button_OracleTest_Click(object sender, EventArgs e)
        {
            new FormOracleTest().Show();
        }
    }
}
