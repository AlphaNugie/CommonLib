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
    public partial class FormEncryption : Form
    {
        private readonly EncryptionClient encryptor = new EncryptionClient();

        public FormEncryption()
        {
            InitializeComponent();
        }

        private void Button_AesEncrypt_Click(object sender, EventArgs e)
        {
            this.textBox_AesResult.Text = this.encryptor.EncryptAES(this.textBox_AesSource.Text);
        }

        private void Button_AesDecrypt_Click(object sender, EventArgs e)
        {
            this.textBox_AesSource.Text = this.encryptor.DecryptAES(this.textBox_AesResult.Text);
        }

        private void Button_DesEncrypt_Click(object sender, EventArgs e)
        {
            this.textBox_DesResult.Text = this.encryptor.EncryptDES(this.textBox_DesSource.Text);
        }

        private void Button_DesDecrypt_Click(object sender, EventArgs e)
        {
            this.textBox_DesSource.Text = this.encryptor.DecryptDES(this.textBox_DesResult.Text);
        }
    }
}
