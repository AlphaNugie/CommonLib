namespace CommonLibExample
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.button_TimerEventRaiser = new System.Windows.Forms.Button();
            this.button_DatabaseTest = new System.Windows.Forms.Button();
            this.button_Encrypt = new System.Windows.Forms.Button();
            this.button_Socket = new System.Windows.Forms.Button();
            this.button_Filters = new System.Windows.Forms.Button();
            this.button_Opc = new System.Windows.Forms.Button();
            this.button_ExitWindows = new System.Windows.Forms.Button();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.button_CustomMessageBox = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // notifyIcon_Main
            // 
            this.notifyIcon_Main.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon_Main.Icon")));
            // 
            // button_TimerEventRaiser
            // 
            this.button_TimerEventRaiser.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_TimerEventRaiser.Location = new System.Drawing.Point(28, 28);
            this.button_TimerEventRaiser.Name = "button_TimerEventRaiser";
            this.button_TimerEventRaiser.Size = new System.Drawing.Size(176, 40);
            this.button_TimerEventRaiser.TabIndex = 0;
            this.button_TimerEventRaiser.Text = "TimerEventRaiser";
            this.button_TimerEventRaiser.UseVisualStyleBackColor = true;
            this.button_TimerEventRaiser.Click += new System.EventHandler(this.Button_TimerEventRaiser_Click);
            // 
            // button_DatabaseTest
            // 
            this.button_DatabaseTest.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_DatabaseTest.Location = new System.Drawing.Point(225, 28);
            this.button_DatabaseTest.Name = "button_DatabaseTest";
            this.button_DatabaseTest.Size = new System.Drawing.Size(170, 40);
            this.button_DatabaseTest.TabIndex = 1;
            this.button_DatabaseTest.Text = "数据库";
            this.button_DatabaseTest.UseVisualStyleBackColor = true;
            this.button_DatabaseTest.Click += new System.EventHandler(this.Button_DatabaseTest_Click);
            // 
            // button_Encrypt
            // 
            this.button_Encrypt.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Encrypt.Location = new System.Drawing.Point(415, 28);
            this.button_Encrypt.Name = "button_Encrypt";
            this.button_Encrypt.Size = new System.Drawing.Size(170, 40);
            this.button_Encrypt.TabIndex = 1;
            this.button_Encrypt.Text = "加密解密";
            this.button_Encrypt.UseVisualStyleBackColor = true;
            this.button_Encrypt.Click += new System.EventHandler(this.Button_Encrypt_Click);
            // 
            // button_Socket
            // 
            this.button_Socket.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Socket.Location = new System.Drawing.Point(606, 29);
            this.button_Socket.Name = "button_Socket";
            this.button_Socket.Size = new System.Drawing.Size(170, 40);
            this.button_Socket.TabIndex = 2;
            this.button_Socket.Text = "SOCKET";
            this.button_Socket.UseVisualStyleBackColor = true;
            this.button_Socket.Click += new System.EventHandler(this.Button_Socket_Click);
            // 
            // button_Filters
            // 
            this.button_Filters.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Filters.Location = new System.Drawing.Point(28, 95);
            this.button_Filters.Name = "button_Filters";
            this.button_Filters.Size = new System.Drawing.Size(176, 40);
            this.button_Filters.TabIndex = 0;
            this.button_Filters.Text = "滤波";
            this.button_Filters.UseVisualStyleBackColor = true;
            this.button_Filters.Click += new System.EventHandler(this.Button_Filters_Click);
            // 
            // button_Opc
            // 
            this.button_Opc.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Opc.Location = new System.Drawing.Point(225, 95);
            this.button_Opc.Name = "button_Opc";
            this.button_Opc.Size = new System.Drawing.Size(176, 40);
            this.button_Opc.TabIndex = 0;
            this.button_Opc.Text = "OPC";
            this.button_Opc.UseVisualStyleBackColor = true;
            this.button_Opc.Click += new System.EventHandler(this.Button_Opc_Click);
            // 
            // button_ExitWindows
            // 
            this.button_ExitWindows.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ExitWindows.Location = new System.Drawing.Point(415, 95);
            this.button_ExitWindows.Name = "button_ExitWindows";
            this.button_ExitWindows.Size = new System.Drawing.Size(170, 40);
            this.button_ExitWindows.TabIndex = 0;
            this.button_ExitWindows.Text = "操作系统操作";
            this.button_ExitWindows.UseVisualStyleBackColor = true;
            this.button_ExitWindows.Click += new System.EventHandler(this.Button_ExitWindows_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(210, 24);
            this.toolStripMenuItem2.Text = "333";
            // 
            // button_CustomMessageBox
            // 
            this.button_CustomMessageBox.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_CustomMessageBox.Location = new System.Drawing.Point(606, 95);
            this.button_CustomMessageBox.Name = "button_CustomMessageBox";
            this.button_CustomMessageBox.Size = new System.Drawing.Size(170, 40);
            this.button_CustomMessageBox.TabIndex = 2;
            this.button_CustomMessageBox.Text = "CustomMessageBox";
            this.button_CustomMessageBox.UseVisualStyleBackColor = true;
            this.button_CustomMessageBox.Click += new System.EventHandler(this.Button_CustomMessageBox_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_CustomMessageBox);
            this.Controls.Add(this.button_Socket);
            this.Controls.Add(this.button_Encrypt);
            this.Controls.Add(this.button_DatabaseTest);
            this.Controls.Add(this.button_ExitWindows);
            this.Controls.Add(this.button_Opc);
            this.Controls.Add(this.button_Filters);
            this.Controls.Add(this.button_TimerEventRaiser);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FormMain";
            this.Text = "测试";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_TimerEventRaiser;
        private System.Windows.Forms.Button button_DatabaseTest;
        private System.Windows.Forms.Button button_Encrypt;
        private System.Windows.Forms.Button button_Socket;
        private System.Windows.Forms.Button button_Filters;
        private System.Windows.Forms.Button button_Opc;
        private System.Windows.Forms.Button button_ExitWindows;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.Button button_CustomMessageBox;
    }
}

