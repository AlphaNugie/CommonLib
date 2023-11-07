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
            this.button_KeepBusy = new System.Windows.Forms.Button();
            this.button_Exception = new System.Windows.Forms.Button();
            this.button_Controls = new System.Windows.Forms.Button();
            this.button_OutOfMemoryException = new System.Windows.Forms.Button();
            this.button_AppCrash = new System.Windows.Forms.Button();
            this.button_RunTests = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // notifyIcon_Main
            // 
            this.notifyIcon_Main.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon_Main.Icon")));
            this.notifyIcon_Main.Text = "测试";
            // 
            // button_TimerEventRaiser
            // 
            this.button_TimerEventRaiser.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_TimerEventRaiser.Location = new System.Drawing.Point(25, 25);
            this.button_TimerEventRaiser.Name = "button_TimerEventRaiser";
            this.button_TimerEventRaiser.Size = new System.Drawing.Size(170, 40);
            this.button_TimerEventRaiser.TabIndex = 0;
            this.button_TimerEventRaiser.Text = "TimerEventRaiser";
            this.button_TimerEventRaiser.UseVisualStyleBackColor = true;
            this.button_TimerEventRaiser.Click += new System.EventHandler(this.Button_TimerEventRaiser_Click);
            // 
            // button_DatabaseTest
            // 
            this.button_DatabaseTest.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_DatabaseTest.Location = new System.Drawing.Point(225, 25);
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
            this.button_Encrypt.Location = new System.Drawing.Point(425, 25);
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
            this.button_Socket.Location = new System.Drawing.Point(625, 25);
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
            this.button_Filters.Location = new System.Drawing.Point(25, 95);
            this.button_Filters.Name = "button_Filters";
            this.button_Filters.Size = new System.Drawing.Size(170, 40);
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
            this.button_Opc.Size = new System.Drawing.Size(170, 40);
            this.button_Opc.TabIndex = 0;
            this.button_Opc.Text = "OPC";
            this.button_Opc.UseVisualStyleBackColor = true;
            this.button_Opc.Click += new System.EventHandler(this.Button_Opc_Click);
            // 
            // button_ExitWindows
            // 
            this.button_ExitWindows.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ExitWindows.Location = new System.Drawing.Point(425, 95);
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
            this.button_CustomMessageBox.Location = new System.Drawing.Point(625, 95);
            this.button_CustomMessageBox.Name = "button_CustomMessageBox";
            this.button_CustomMessageBox.Size = new System.Drawing.Size(170, 40);
            this.button_CustomMessageBox.TabIndex = 2;
            this.button_CustomMessageBox.Text = "CustomMsgBox";
            this.button_CustomMessageBox.UseVisualStyleBackColor = true;
            this.button_CustomMessageBox.Click += new System.EventHandler(this.Button_CustomMessageBox_Click);
            // 
            // button_KeepBusy
            // 
            this.button_KeepBusy.BackColor = System.Drawing.SystemColors.HotTrack;
            this.button_KeepBusy.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.button_KeepBusy.Location = new System.Drawing.Point(12, 390);
            this.button_KeepBusy.Name = "button_KeepBusy";
            this.button_KeepBusy.Size = new System.Drawing.Size(52, 48);
            this.button_KeepBusy.TabIndex = 3;
            this.button_KeepBusy.Text = "循环";
            this.button_KeepBusy.UseVisualStyleBackColor = false;
            this.button_KeepBusy.Click += new System.EventHandler(this.Button_Busy_Click);
            // 
            // button_Exception
            // 
            this.button_Exception.BackColor = System.Drawing.Color.Salmon;
            this.button_Exception.Location = new System.Drawing.Point(70, 390);
            this.button_Exception.Name = "button_Exception";
            this.button_Exception.Size = new System.Drawing.Size(52, 48);
            this.button_Exception.TabIndex = 3;
            this.button_Exception.Text = "异常";
            this.button_Exception.UseVisualStyleBackColor = false;
            this.button_Exception.Click += new System.EventHandler(this.Button_Exception_Click);
            // 
            // button_Controls
            // 
            this.button_Controls.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Controls.Location = new System.Drawing.Point(25, 165);
            this.button_Controls.Name = "button_Controls";
            this.button_Controls.Size = new System.Drawing.Size(170, 40);
            this.button_Controls.TabIndex = 0;
            this.button_Controls.Text = "控件";
            this.button_Controls.UseVisualStyleBackColor = true;
            this.button_Controls.Click += new System.EventHandler(this.Button_Controls_Click);
            // 
            // button_OutOfMemoryException
            // 
            this.button_OutOfMemoryException.BackColor = System.Drawing.Color.Salmon;
            this.button_OutOfMemoryException.Location = new System.Drawing.Point(128, 390);
            this.button_OutOfMemoryException.Name = "button_OutOfMemoryException";
            this.button_OutOfMemoryException.Size = new System.Drawing.Size(67, 48);
            this.button_OutOfMemoryException.TabIndex = 3;
            this.button_OutOfMemoryException.Text = "OutOfMemoryException";
            this.button_OutOfMemoryException.UseVisualStyleBackColor = false;
            this.button_OutOfMemoryException.Click += new System.EventHandler(this.Button_OutOfMemoryException_Click);
            // 
            // button_AppCrash
            // 
            this.button_AppCrash.BackColor = System.Drawing.Color.Orange;
            this.button_AppCrash.Location = new System.Drawing.Point(201, 390);
            this.button_AppCrash.Name = "button_AppCrash";
            this.button_AppCrash.Size = new System.Drawing.Size(52, 48);
            this.button_AppCrash.TabIndex = 3;
            this.button_AppCrash.Text = "AppCrash";
            this.button_AppCrash.UseVisualStyleBackColor = false;
            this.button_AppCrash.Click += new System.EventHandler(this.Button_AppCrash_Click);
            // 
            // button_RunTests
            // 
            this.button_RunTests.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_RunTests.Location = new System.Drawing.Point(225, 165);
            this.button_RunTests.Name = "button_RunTests";
            this.button_RunTests.Size = new System.Drawing.Size(170, 40);
            this.button_RunTests.TabIndex = 0;
            this.button_RunTests.Text = "测试";
            this.button_RunTests.UseVisualStyleBackColor = true;
            this.button_RunTests.Click += new System.EventHandler(this.Button_RunTests_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 450);
            this.Controls.Add(this.button_OutOfMemoryException);
            this.Controls.Add(this.button_AppCrash);
            this.Controls.Add(this.button_Exception);
            this.Controls.Add(this.button_KeepBusy);
            this.Controls.Add(this.button_CustomMessageBox);
            this.Controls.Add(this.button_Socket);
            this.Controls.Add(this.button_Encrypt);
            this.Controls.Add(this.button_DatabaseTest);
            this.Controls.Add(this.button_ExitWindows);
            this.Controls.Add(this.button_Opc);
            this.Controls.Add(this.button_RunTests);
            this.Controls.Add(this.button_Controls);
            this.Controls.Add(this.button_Filters);
            this.Controls.Add(this.button_TimerEventRaiser);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
        private System.Windows.Forms.Button button_KeepBusy;
        private System.Windows.Forms.Button button_Exception;
        private System.Windows.Forms.Button button_Controls;
        private System.Windows.Forms.Button button_OutOfMemoryException;
        private System.Windows.Forms.Button button_AppCrash;
        private System.Windows.Forms.Button button_RunTests;
    }
}

