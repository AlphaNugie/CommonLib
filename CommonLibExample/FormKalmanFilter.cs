using CommonLib.Clients;
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
    public partial class FormKalmanFilter : Form
    {
        private KalmanFilter _kalmanFilter;
        private double _value;

        public FormKalmanFilter()
        {
            InitializeComponent();
        }

        private void Button_Init_Click(object sender, EventArgs e)
        {
            _kalmanFilter = new KalmanFilter((double)numeric_Q.Value, (double)numeric_R.Value);
            button_Add.Enabled = true;
            textBox_Eval.Text = string.Empty;
        }

        private void Button_Add_Click(object sender, EventArgs e)
        {
            if (_kalmanFilter == null)
                return;
            _value = (double)numeric_Value.Value;
            _kalmanFilter.SetValue(ref _value, (double)numeric_Velocity.Value);
            textBox_Eval.Text = _value.ToString();
        }
    }
}
