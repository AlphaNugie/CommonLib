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
    public partial class FormControls : Form
    {
        public FormControls()
        {
            InitializeComponent();
        }

        private void Button_TextBoxes_Click(object sender, EventArgs e)
        {
            new FormTextBoxes().Show();
        }

        private void Button_SineWaves_Click(object sender, EventArgs e)
        {
            new FormSineWaves().Show();
        }
    }
}
