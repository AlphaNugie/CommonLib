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
    public partial class FormFilters : Form
    {
        public FormFilters()
        {
            InitializeComponent();
        }

        private void button_KalmanFilter_Click(object sender, EventArgs e)
        {
            new FormKalmanFilter().Show();
        }
    }
}
