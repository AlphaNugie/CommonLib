namespace CommonLibExample
{
    partial class FormTimerEventRaiser
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
            this.button_Click = new System.Windows.Forms.Button();
            this.button_Reset = new System.Windows.Forms.Button();
            this.label_Message = new System.Windows.Forms.Label();
            this.button_Start = new System.Windows.Forms.Button();
            this.button_End = new System.Windows.Forms.Button();
            this.label_Count = new System.Windows.Forms.Label();
            this.label_RaiseCount = new System.Windows.Forms.Label();
            this.textBox_Message = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_Click
            // 
            this.button_Click.Location = new System.Drawing.Point(167, 34);
            this.button_Click.Name = "button_Click";
            this.button_Click.Size = new System.Drawing.Size(86, 37);
            this.button_Click.TabIndex = 0;
            this.button_Click.Text = "点击";
            this.button_Click.UseVisualStyleBackColor = true;
            this.button_Click.Click += new System.EventHandler(this.Button_Click_Click);
            // 
            // button_Reset
            // 
            this.button_Reset.Location = new System.Drawing.Point(167, 77);
            this.button_Reset.Name = "button_Reset";
            this.button_Reset.Size = new System.Drawing.Size(86, 37);
            this.button_Reset.TabIndex = 0;
            this.button_Reset.Text = "重置";
            this.button_Reset.UseVisualStyleBackColor = true;
            this.button_Reset.Click += new System.EventHandler(this.Button_Reset_Click);
            // 
            // label_Message
            // 
            this.label_Message.AutoSize = true;
            this.label_Message.Location = new System.Drawing.Point(274, 164);
            this.label_Message.Name = "label_Message";
            this.label_Message.Size = new System.Drawing.Size(65, 15);
            this.label_Message.TabIndex = 1;
            this.label_Message.Text = "message";
            // 
            // button_Start
            // 
            this.button_Start.Location = new System.Drawing.Point(65, 34);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(86, 37);
            this.button_Start.TabIndex = 0;
            this.button_Start.Text = "开始";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.Button_Start_Click);
            // 
            // button_End
            // 
            this.button_End.Location = new System.Drawing.Point(65, 77);
            this.button_End.Name = "button_End";
            this.button_End.Size = new System.Drawing.Size(86, 37);
            this.button_End.TabIndex = 0;
            this.button_End.Text = "结束";
            this.button_End.UseVisualStyleBackColor = true;
            this.button_End.Click += new System.EventHandler(this.Button_End_Click);
            // 
            // label_Count
            // 
            this.label_Count.AutoSize = true;
            this.label_Count.Location = new System.Drawing.Point(317, 88);
            this.label_Count.Name = "label_Count";
            this.label_Count.Size = new System.Drawing.Size(44, 15);
            this.label_Count.TabIndex = 2;
            this.label_Count.Text = "count";
            // 
            // label_RaiseCount
            // 
            this.label_RaiseCount.AutoSize = true;
            this.label_RaiseCount.Location = new System.Drawing.Point(474, 88);
            this.label_RaiseCount.Name = "label_RaiseCount";
            this.label_RaiseCount.Size = new System.Drawing.Size(80, 15);
            this.label_RaiseCount.TabIndex = 3;
            this.label_RaiseCount.Text = "raise_count";
            // 
            // textBox_Message
            // 
            this.textBox_Message.Location = new System.Drawing.Point(277, 34);
            this.textBox_Message.Multiline = true;
            this.textBox_Message.Name = "textBox_Message";
            this.textBox_Message.Size = new System.Drawing.Size(485, 37);
            this.textBox_Message.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(274, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "计数";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(422, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "触发";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(274, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 15);
            this.label5.TabIndex = 1;
            this.label5.Text = "信息";
            // 
            // FormTimerEventRaiser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox_Message);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label_RaiseCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label_Count);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label_Message);
            this.Controls.Add(this.button_End);
            this.Controls.Add(this.button_Start);
            this.Controls.Add(this.button_Reset);
            this.Controls.Add(this.button_Click);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FormTimerEventRaiser";
            this.Text = "FormTimerEventRaiser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Click;
        private System.Windows.Forms.Button button_Reset;
        private System.Windows.Forms.Label label_Message;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.Button button_End;
        private System.Windows.Forms.Label label_Count;
        private System.Windows.Forms.Label label_RaiseCount;
        private System.Windows.Forms.TextBox textBox_Message;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}