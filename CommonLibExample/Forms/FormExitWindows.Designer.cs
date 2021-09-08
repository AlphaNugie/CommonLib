namespace CommonLibExample.Forms
{
    partial class FormExitWindows
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
            this.checkBox_Force = new System.Windows.Forms.CheckBox();
            this.button_Shutdown = new System.Windows.Forms.Button();
            this.button_Reboot = new System.Windows.Forms.Button();
            this.button_Logoff = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBox_Force
            // 
            this.checkBox_Force.AutoSize = true;
            this.checkBox_Force.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_Force.Location = new System.Drawing.Point(43, 113);
            this.checkBox_Force.Name = "checkBox_Force";
            this.checkBox_Force.Size = new System.Drawing.Size(272, 25);
            this.checkBox_Force.TabIndex = 0;
            this.checkBox_Force.Text = "是否强制关闭所有应用程序";
            this.checkBox_Force.UseVisualStyleBackColor = true;
            // 
            // button_Shutdown
            // 
            this.button_Shutdown.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Shutdown.Location = new System.Drawing.Point(43, 27);
            this.button_Shutdown.Name = "button_Shutdown";
            this.button_Shutdown.Size = new System.Drawing.Size(198, 44);
            this.button_Shutdown.TabIndex = 1;
            this.button_Shutdown.Text = "关闭操作系统";
            this.button_Shutdown.UseVisualStyleBackColor = true;
            this.button_Shutdown.Click += new System.EventHandler(this.Button_Shutdown_Click);
            // 
            // button_Reboot
            // 
            this.button_Reboot.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Reboot.Location = new System.Drawing.Point(305, 27);
            this.button_Reboot.Name = "button_Reboot";
            this.button_Reboot.Size = new System.Drawing.Size(198, 44);
            this.button_Reboot.TabIndex = 1;
            this.button_Reboot.Text = "重启操作系统";
            this.button_Reboot.UseVisualStyleBackColor = true;
            this.button_Reboot.Click += new System.EventHandler(this.Button_Reboot_Click);
            // 
            // button_Logoff
            // 
            this.button_Logoff.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Logoff.Location = new System.Drawing.Point(566, 27);
            this.button_Logoff.Name = "button_Logoff";
            this.button_Logoff.Size = new System.Drawing.Size(198, 44);
            this.button_Logoff.TabIndex = 1;
            this.button_Logoff.Text = "注销用户";
            this.button_Logoff.UseVisualStyleBackColor = true;
            this.button_Logoff.Click += new System.EventHandler(this.Button_Logoff_Click);
            // 
            // FormExitWindows
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_Logoff);
            this.Controls.Add(this.button_Reboot);
            this.Controls.Add(this.button_Shutdown);
            this.Controls.Add(this.checkBox_Force);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FormExitWindows";
            this.Text = "操作系统操作";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_Force;
        private System.Windows.Forms.Button button_Shutdown;
        private System.Windows.Forms.Button button_Reboot;
        private System.Windows.Forms.Button button_Logoff;
    }
}