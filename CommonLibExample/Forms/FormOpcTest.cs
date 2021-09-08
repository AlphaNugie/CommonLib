using CommonLib.Function;
using OPCAutomation;
using OpcLibrary;
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
    public partial class FormOpcTest : Form
    {
        /// <summary>
        /// IP Address
        /// </summary>
        public string IpAddress { get; private set; }

        private readonly OpcUtilHelper _opcHelper = new OpcUtilHelper(300, true);

        //public OPCServer KepServer { get; private set; }
        //public OPCGroups KepGroups { get; private set; }
        //public OPCGroup KepGroup { get; private set; }
        //public OPCItems KepItems { get; private set; }

        public FormOpcTest()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 枚举OPC SERVER
        /// </summary>
        private void ServerEnum()
        {
            string ipAddress = textBox_OpcServerIp.Text;
            comboBox_OpcServerList.Items.Clear(); //清空已显示的OPC Server列表
            string[] array = _opcHelper.ServerEnum(ipAddress, out string message);
            if (!string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //假如Server列表为空，退出方法，否则为ListBoxControl添加Item
            if (array.Length == 0)
                return;
            comboBox_OpcServerList.Items.AddRange(array);
            comboBox_OpcServerList.SelectedIndex = 0;
        }

        /// <summary>
        /// 连接OPC SERVER并创建组
        /// </summary>
        private bool ConnectRemoteServer_Init()
        {
            try
            {
                bool result = false;
                result = _opcHelper.ConnectRemoteServer(textBox_OpcServerIp.Text, comboBox_OpcServerList.Text, out string message);
                if (!result)
                {
                    MessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("初始化出错：" + err.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 连接或断开
        /// </summary>
        /// <param name="connecting">是否要连接</param>
        private void ConnectSlashDisconnect(bool connecting)
        {
            if (connecting) ConnectRemoteServer_Init();
            else _opcHelper.DisconnectRemoteServer();
            button_Connect.Text = connecting ? "断开" : "连接";
        }

        private void Button_ServerEnum_Click(object sender, EventArgs e)
        {
            ServerEnum();
        }

        private void Button_Connect_Click(object sender, EventArgs e)
        {
            ConnectSlashDisconnect(button_Connect.Text.Equals("连接"));
        }

        /// <summary>
        /// 添加测试写入标签，假如已添加标签，移除标签再重新添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddTestItem_Click(object sender, EventArgs e)
        {
            if (!_opcHelper.SetItem(textBox_TestItemId.Text, 1, out string message))
                MessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 测试标签写入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_WriteTestItem_Click(object sender, EventArgs e)
        {
            if (!_opcHelper.WriteItemValue(textBox_TestValueWrite.Text, out string message))
                MessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 测试标签读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ReadTestItem_Click(object sender, EventArgs e)
        {
            if (!_opcHelper.ReadItemValue(out string value, out string message))
                MessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
            textBox_TestValueRead.Text = value;
        }
    }
}
