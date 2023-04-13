namespace CommonLibExample.Forms
{
    partial class FormTextBoxes
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
            this.components = new System.ComponentModel.Container();
            this.textBox_MultiLines = new System.Windows.Forms.TextBox();
            this.richTextBox_MultiLines = new System.Windows.Forms.RichTextBox();
            this.button_Refresh = new System.Windows.Forms.Button();
            this.button_RefreshStop = new System.Windows.Forms.Button();
            this.timer_Refresh = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // textBox_MultiLines
            // 
            this.textBox_MultiLines.Location = new System.Drawing.Point(12, 12);
            this.textBox_MultiLines.Multiline = true;
            this.textBox_MultiLines.Name = "textBox_MultiLines";
            this.textBox_MultiLines.Size = new System.Drawing.Size(284, 262);
            this.textBox_MultiLines.TabIndex = 0;
            // 
            // richTextBox_MultiLines
            // 
            this.richTextBox_MultiLines.Location = new System.Drawing.Point(302, 12);
            this.richTextBox_MultiLines.Name = "richTextBox_MultiLines";
            this.richTextBox_MultiLines.Size = new System.Drawing.Size(284, 262);
            this.richTextBox_MultiLines.TabIndex = 1;
            this.richTextBox_MultiLines.Text = "";
            // 
            // button_Refresh
            // 
            this.button_Refresh.Location = new System.Drawing.Point(592, 12);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Size = new System.Drawing.Size(83, 41);
            this.button_Refresh.TabIndex = 2;
            this.button_Refresh.Text = "刷新";
            this.button_Refresh.UseVisualStyleBackColor = true;
            this.button_Refresh.Click += new System.EventHandler(this.Button_RefreshStart_Click);
            // 
            // button_RefreshStop
            // 
            this.button_RefreshStop.Location = new System.Drawing.Point(592, 59);
            this.button_RefreshStop.Name = "button_RefreshStop";
            this.button_RefreshStop.Size = new System.Drawing.Size(83, 41);
            this.button_RefreshStop.TabIndex = 2;
            this.button_RefreshStop.Text = "停止";
            this.button_RefreshStop.UseVisualStyleBackColor = true;
            // 
            // timer_Refresh
            // 
            this.timer_Refresh.Enabled = true;
            this.timer_Refresh.Tick += new System.EventHandler(this.Timer_Refresh_Tick);
            // 
            // FormTextBoxes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_RefreshStop);
            this.Controls.Add(this.button_Refresh);
            this.Controls.Add(this.richTextBox_MultiLines);
            this.Controls.Add(this.textBox_MultiLines);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FormTextBoxes";
            this.Text = "文本框测试";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_MultiLines;
        private System.Windows.Forms.RichTextBox richTextBox_MultiLines;
        private System.Windows.Forms.Button button_Refresh;
        private System.Windows.Forms.Button button_RefreshStop;
        private System.Windows.Forms.Timer timer_Refresh;
    }
}