//using CommonLib.Clients;
//using CommonLib.Events;
using CommonLib.Function;
using CommonLib.UIControlUtil;
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
    public partial class FormSocket : Form
    {
        private CommonLib.Clients.DerivedUdpClient _udpClient = null;

        public FormSocket()
        {
            InitializeComponent();
            this.textBox_UdpIpAddress.Text = Functions.GetIPAddressV4();
        }

        private void Button_UdpInit_Click(object sender, EventArgs e)
        {
            this._udpClient = new CommonLib.Clients.DerivedUdpClient(this.textBox_UdpIpAddress.Text, (int)this.numeric_UdpPort.Value);
            this._udpClient.DataReceived += new CommonLib.Events.DataReceivedEventHandler(UdpClient_DataReceived);
        }

        private void UdpClient_DataReceived(object sender, CommonLib.Events.DataReceivedEventArgs eventArgs)
        {
            this.textBox_UdpReceive.SafeInvoke(() => this.textBox_UdpReceive.Text = string.Format(@"{0:yyyy-MM-dd HH:mm:ss}=>
{1}", DateTime.Now, eventArgs.ReceivedInfo_String));
//            this.textBox_UdpReceive.Text = string.Format(@"{0:yyyy-MM-dd HH:mm:ss}=>
//{1}", DateTime.Now, eventArgs.ReceivedInfo_String);
            //throw new NotImplementedException();
        }
    }
}
