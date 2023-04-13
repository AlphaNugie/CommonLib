using MathNet.Numerics.Random;
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
    public partial class FormTextBoxes : Form
    {
        private bool _started;
        private readonly Random _random = new Random();
        private readonly string _template = @"
{0}hag
dkajfgjigejigjgid
okweojridjsdmdgnvg
1325664oi4194847
434646476234
13345
1
0000000000000066666
fffffffffffffffffffff
fhhhhhhhhhhhhhhhhhhh
jjjjj
g
{1}ooo
1111111111
-----------
=============
++++++++++++++++++++++++++++++
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
22222222222222222222222
5555555555555555
999999999999999999
0
-
=
`
{2}";

        public FormTextBoxes()
        {
            InitializeComponent();
        }

        private void Button_RefreshStart_Click(object sender, EventArgs e)
        {
            bool flag = button_Refresh.Text.Equals("刷新");
            button_Refresh.Text = flag ? "停止" : "刷新";
            _started = flag;
        }

        private void Timer_Refresh_Tick(object sender, EventArgs e)
        {
            if (!_started)
                return;
            var text = string.Format(_template, _random.NextFullRangeInt64(), _random.NextFullRangeInt32(), _random.NextInt64());
            var lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            textBox_MultiLines.MaxLength = text.Length;
            textBox_MultiLines.AppendText(text);
            //richTextBox_MultiLines.MaxLength = text.Length;
            richTextBox_MultiLines.Clear();
            richTextBox_MultiLines.AppendText(text);
            //richTextBox_MultiLines.Lines = lines;
            //richTextBox_MultiLines.SelectionStart = richTextBox_MultiLines.TextLength;
            //richTextBox_MultiLines.ScrollToCaret();
        }
    }
}
