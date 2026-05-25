using OpcLibrary.Controls.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpcLibrary.Ua.Forms
{
    /// <summary>
    /// OPC UA 配置窗口，继承自 <see cref="FormOpcConfig"/>，适配OPC UA连接方式
    /// <para/>IP地址输入框改为服务地址输入框（格式如 opc.tcp://192.0.244.32:49320），隐藏DA服务器枚举相关控件
    /// </summary>
    public partial class FormOpcUaConfig: FormOpcConfig
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public FormOpcUaConfig()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 连接按钮点击事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        private void Button_Connect_Click(object sender, EventArgs e)
        {

        }
    }
}
