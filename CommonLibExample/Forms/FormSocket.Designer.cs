namespace CommonLibExample
{
    partial class FormSocket
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_UdpIpAddress = new System.Windows.Forms.TextBox();
            this.numeric_UdpPort = new System.Windows.Forms.NumericUpDown();
            this.button_UdpInit = new System.Windows.Forms.Button();
            this.textBox_UdpReceive = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_UdpPort)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(129, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "UDP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 65);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 19);
            this.label2.TabIndex = 0;
            this.label2.Text = "IP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 97);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 19);
            this.label3.TabIndex = 0;
            this.label3.Text = "端口";
            // 
            // textBox_UdpIpAddress
            // 
            this.textBox_UdpIpAddress.Location = new System.Drawing.Point(82, 62);
            this.textBox_UdpIpAddress.Name = "textBox_UdpIpAddress";
            this.textBox_UdpIpAddress.Size = new System.Drawing.Size(148, 26);
            this.textBox_UdpIpAddress.TabIndex = 1;
            // 
            // numeric_UdpPort
            // 
            this.numeric_UdpPort.Location = new System.Drawing.Point(82, 97);
            this.numeric_UdpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numeric_UdpPort.Name = "numeric_UdpPort";
            this.numeric_UdpPort.Size = new System.Drawing.Size(148, 26);
            this.numeric_UdpPort.TabIndex = 2;
            // 
            // button_UdpInit
            // 
            this.button_UdpInit.Location = new System.Drawing.Point(252, 62);
            this.button_UdpInit.Name = "button_UdpInit";
            this.button_UdpInit.Size = new System.Drawing.Size(75, 61);
            this.button_UdpInit.TabIndex = 3;
            this.button_UdpInit.Text = "初始化";
            this.button_UdpInit.UseVisualStyleBackColor = true;
            this.button_UdpInit.Click += new System.EventHandler(this.Button_UdpInit_Click);
            // 
            // textBox_UdpReceive
            // 
            this.textBox_UdpReceive.Location = new System.Drawing.Point(26, 152);
            this.textBox_UdpReceive.Multiline = true;
            this.textBox_UdpReceive.Name = "textBox_UdpReceive";
            this.textBox_UdpReceive.Size = new System.Drawing.Size(301, 202);
            this.textBox_UdpReceive.TabIndex = 4;
            // 
            // FormSocket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 570);
            this.Controls.Add(this.textBox_UdpReceive);
            this.Controls.Add(this.button_UdpInit);
            this.Controls.Add(this.numeric_UdpPort);
            this.Controls.Add(this.textBox_UdpIpAddress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormSocket";
            this.Text = "FormSocket";
            ((System.ComponentModel.ISupportInitialize)(this.numeric_UdpPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_UdpIpAddress;
        private System.Windows.Forms.NumericUpDown numeric_UdpPort;
        private System.Windows.Forms.Button button_UdpInit;
        private System.Windows.Forms.TextBox textBox_UdpReceive;
    }
}