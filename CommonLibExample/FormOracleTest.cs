using CommonLib.DataUtil;
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
    public partial class FormOracleTest : Form
    {
        private readonly OracleProvider provider_def = new OracleProvider();
        private OracleProvider provider_dynamic = null;
        private OracleProvider provider = null;
        private int index = 1;

        public FormOracleTest()
        {
            InitializeComponent();
            this.provider = this.provider_def;
            this.comboBox_Mode.SelectedIndex = 0;
        }

        private void Button_GetConnStr_Click(object sender, EventArgs e)
        {
            try
            {
                this.textBox_ConnStr.Text = OracleProvider.GetConnStr(this.textBox_Address.Text, int.Parse(this.textBox_Port.Text), this.textBox_ServiceName.Text, this.textBox_UserName.Text, this.textBox_Password.Text);
            }
            catch (Exception) { }
        }

        private void ComboBox_Mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.index = this.comboBox_Mode.SelectedIndex + 1;
        }

        private bool IsInputValid()
        {
            bool result = false;
            if (this.index == 1)
            {
                result = true;
                this.provider = this.provider_def;
            }
            else if (this.index == 2)
            {
                result = !string.IsNullOrWhiteSpace(this.textBox_Address.Text) && !string.IsNullOrWhiteSpace(this.textBox_Port.Text) && !string.IsNullOrWhiteSpace(this.textBox_ServiceName.Text) && !string.IsNullOrWhiteSpace(this.textBox_UserName.Text) && !string.IsNullOrWhiteSpace(this.textBox_Password.Text);
                if (result)
                    this.provider_dynamic = new OracleProvider(this.textBox_Address.Text, int.Parse(this.textBox_Port.Text), this.textBox_ServiceName.Text, this.textBox_UserName.Text, this.textBox_Password.Text);
                this.provider = this.provider_dynamic;
            }
            if (!result)
                MessageBox.Show("输入不符合要求");
            return result;
        }

        private void Button_IsConnOopen_Click(object sender, EventArgs e)
        {
            this.label_IsConnOpen.Text = "False";
            if (!this.IsInputValid())
                return;
            this.label_IsConnOpen.Text = this.provider.IsConnOpen().ToString();
        }

        private void Button_MultiQuery_Click(object sender, EventArgs e)
        {
            if (!this.IsInputValid())
                return;
            string sql1 = "select * from t_test_providertest where id > 3", sql2 = "select * from t_test_providertest where height > 1.71";
            DataSet dataSet = this.provider.MultiQuery(new string[] { sql1, sql2 });
            MessageBox.Show(dataSet == null ? "null" : dataSet.ToString());
        }

        private void Button_Query_Click(object sender, EventArgs e)
        {
            if (!this.IsInputValid())
                return;
            string sql1 = "select * from t_test_providertest where date_arrival > to_date('2020-04-30 00:00:05', 'yyyy-mm-dd hh24:mi:ss')";
            DataTable table = this.provider.Query(sql1);
            MessageBox.Show(table == null ? "null" : table.ToString());
        }

        private void Button_ExecuteSql_Click(object sender, EventArgs e)
        {
            if (!this.IsInputValid())
                return;
            string sql1 = "insert into t_test_providertest (id, name, age, height, date_arrival, motto) values (1, 'David Jenson', 19, 1.85, sysdate, 'no man can beat me!')";
            int result = this.provider.ExecuteSql(sql1);
            MessageBox.Show(result.ToString());
        }

        private void Button_ExecuteSqlTrans_Click(object sender, EventArgs e)
        {
            if (!this.IsInputValid())
                return;
            string sql1 = "insert into t_test_providertest (id, name, age, height, date_arrival, motto) values (1, 'David Jenson', 19, 1.85, sysdate, 'no man can beat me!')", sql2 = "insert into t_test_providertest (id, name, age, height, date_arrival, motto) values (2, 'Jennifer Swift', 23, 1.72, sysdate, 'lets do this sis')", sql3 = "insert into t_test_providertest (id, name, age, height, date_arrival, motto) values (4, 'Thomas Nova', 17, 1.67, sysdate, 'do not bother alright?')";
            bool result = this.provider.ExecuteSqlTrans(new string[] { sql1, sql2, sql3 });
            MessageBox.Show(result.ToString());
        }
    }
}
