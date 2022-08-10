using CommonLib.UIControlUtil.ControlTemplates;
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
    public partial class FormCustomMessageBox : Form
    {
        public FormCustomMessageBox()
        {
            InitializeComponent();
        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            MessageBox.Show(CustomMessageBox.Show(textBox_Message.Text, textBox_Caption.Text, CustomMessageBoxButtons.OK, CustomMessageBoxIcon.Information, Font).ToString(), "result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Button_OKCancel_Click(object sender, EventArgs e)
        {
            MessageBox.Show(CustomMessageBox.Show(textBox_Message.Text, textBox_Caption.Text, CustomMessageBoxButtons.OKCancel, CustomMessageBoxIcon.Warning, Font).ToString(), "result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Button_YesNo_Click(object sender, EventArgs e)
        {
            MessageBox.Show(CustomMessageBox.Show(textBox_Message.Text, textBox_Caption.Text, CustomMessageBoxButtons.YesNo, CustomMessageBoxIcon.Question, Font).ToString(), "result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Button_YesNoCancel_Click(object sender, EventArgs e)
        {
            MessageBox.Show(CustomMessageBox.Show(textBox_Message.Text, textBox_Caption.Text, CustomMessageBoxButtons.YesNoCancel, CustomMessageBoxIcon.Error, Font).ToString(), "result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
