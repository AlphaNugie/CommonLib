namespace OpcLibrary.Ua.Forms
{
    partial class FormOpcUaConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // label_IpAddress
            // 
            this.label_IpAddress.Size = new System.Drawing.Size(56, 17);
            this.label_IpAddress.Text = "服务地址";
            // 
            // textBox_OpcServerIp
            // 
            this.textBox_OpcServerIp.Location = new System.Drawing.Point(72, 22);
            this.textBox_OpcServerIp.Size = new System.Drawing.Size(540, 23);
            this.textBox_OpcServerIp.Text = "opc.tcp://192.0.244.32:49320";
            // 
            // comboBox_OpcServerList
            // 
            this.comboBox_OpcServerList.Location = new System.Drawing.Point(666, -1);
            this.comboBox_OpcServerList.Size = new System.Drawing.Size(41, 25);
            this.comboBox_OpcServerList.Visible = false;
            // 
            // button_ServerEnum
            // 
            this.button_ServerEnum.Location = new System.Drawing.Point(713, -1);
            this.button_ServerEnum.Size = new System.Drawing.Size(38, 28);
            this.button_ServerEnum.Visible = false;
            // 
            // button_Connect
            // 
            this.button_Connect.Location = new System.Drawing.Point(618, 19);
            this.button_Connect.Click += new System.EventHandler(this.Button_Connect_Click);
            // 
            // FormOpcUaConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 492);
            this.Name = "FormOpcUaConfig";
            this.Text = "FormOpcUaConfig";
            this.ResumeLayout(false);

        }

        #endregion
    }
}