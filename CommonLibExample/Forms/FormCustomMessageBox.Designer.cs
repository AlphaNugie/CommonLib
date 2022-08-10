namespace CommonLibExample.Forms
{
    partial class FormCustomMessageBox
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
            this.button_OK = new System.Windows.Forms.Button();
            this.button_OKCancel = new System.Windows.Forms.Button();
            this.button_YesNo = new System.Windows.Forms.Button();
            this.button_YesNoCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Caption = new System.Windows.Forms.TextBox();
            this.textBox_Message = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(78, 97);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(101, 40);
            this.button_OK.TabIndex = 0;
            this.button_OK.Text = "OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.Button_OK_Click);
            // 
            // button_OKCancel
            // 
            this.button_OKCancel.Location = new System.Drawing.Point(198, 97);
            this.button_OKCancel.Name = "button_OKCancel";
            this.button_OKCancel.Size = new System.Drawing.Size(101, 40);
            this.button_OKCancel.TabIndex = 0;
            this.button_OKCancel.Text = "OKCancel";
            this.button_OKCancel.UseVisualStyleBackColor = true;
            this.button_OKCancel.Click += new System.EventHandler(this.Button_OKCancel_Click);
            // 
            // button_YesNo
            // 
            this.button_YesNo.Location = new System.Drawing.Point(318, 97);
            this.button_YesNo.Name = "button_YesNo";
            this.button_YesNo.Size = new System.Drawing.Size(101, 40);
            this.button_YesNo.TabIndex = 0;
            this.button_YesNo.Text = "YesNo";
            this.button_YesNo.UseVisualStyleBackColor = true;
            this.button_YesNo.Click += new System.EventHandler(this.Button_YesNo_Click);
            // 
            // button_YesNoCancel
            // 
            this.button_YesNoCancel.Location = new System.Drawing.Point(438, 97);
            this.button_YesNoCancel.Name = "button_YesNoCancel";
            this.button_YesNoCancel.Size = new System.Drawing.Size(101, 40);
            this.button_YesNoCancel.TabIndex = 0;
            this.button_YesNoCancel.Text = "YesNoCancel";
            this.button_YesNoCancel.UseVisualStyleBackColor = true;
            this.button_YesNoCancel.Click += new System.EventHandler(this.Button_YesNoCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(85, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "标题";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(85, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "内容";
            // 
            // textBox_Caption
            // 
            this.textBox_Caption.Location = new System.Drawing.Point(150, 17);
            this.textBox_Caption.Name = "textBox_Caption";
            this.textBox_Caption.Size = new System.Drawing.Size(146, 23);
            this.textBox_Caption.TabIndex = 2;
            // 
            // textBox_Message
            // 
            this.textBox_Message.Location = new System.Drawing.Point(150, 48);
            this.textBox_Message.Name = "textBox_Message";
            this.textBox_Message.Size = new System.Drawing.Size(386, 23);
            this.textBox_Message.TabIndex = 2;
            // 
            // FormCustomMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 316);
            this.Controls.Add(this.textBox_Message);
            this.Controls.Add(this.textBox_Caption);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_YesNoCancel);
            this.Controls.Add(this.button_YesNo);
            this.Controls.Add(this.button_OKCancel);
            this.Controls.Add(this.button_OK);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormCustomMessageBox";
            this.Text = "FormCustomMessageBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button button_OKCancel;
        private System.Windows.Forms.Button button_YesNo;
        private System.Windows.Forms.Button button_YesNoCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Caption;
        private System.Windows.Forms.TextBox textBox_Message;
    }
}