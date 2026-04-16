namespace CommonLibExample.Forms
{
    partial class FormDelayTest
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
            this.numeric_SecondsDelayed = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.button_DelayAction = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label_Time = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_SecondsDelayed)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numeric_SecondsDelayed
            // 
            this.numeric_SecondsDelayed.Location = new System.Drawing.Point(37, 16);
            this.numeric_SecondsDelayed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numeric_SecondsDelayed.Name = "numeric_SecondsDelayed";
            this.numeric_SecondsDelayed.Size = new System.Drawing.Size(55, 24);
            this.numeric_SecondsDelayed.TabIndex = 0;
            this.numeric_SecondsDelayed.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(98, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "秒";
            // 
            // button_DelayAction
            // 
            this.button_DelayAction.Location = new System.Drawing.Point(138, 10);
            this.button_DelayAction.Name = "button_DelayAction";
            this.button_DelayAction.Size = new System.Drawing.Size(83, 33);
            this.button_DelayAction.TabIndex = 2;
            this.button_DelayAction.Text = "延迟操作";
            this.button_DelayAction.UseVisualStyleBackColor = true;
            this.button_DelayAction.Click += new System.EventHandler(this.Button_DelayAction_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(29, 47);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 38);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(138, 47);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(255, 62);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 44);
            this.button3.TabIndex = 5;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(364, 55);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(119, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBox1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Location = new System.Drawing.Point(37, 113);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(596, 276);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(219, 129);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(230, 106);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "a wise man\na dumb person\nwtf\nmilf";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(29, 129);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(155, 24);
            this.textBox1.TabIndex = 7;
            this.textBox1.Text = "5555";
            // 
            // label_Time
            // 
            this.label_Time.AutoSize = true;
            this.label_Time.Location = new System.Drawing.Point(318, 18);
            this.label_Time.Name = "label_Time";
            this.label_Time.Size = new System.Drawing.Size(40, 17);
            this.label_Time.TabIndex = 8;
            this.label_Time.Text = "时间";
            // 
            // FormDelayTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 480);
            this.Controls.Add(this.label_Time);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_DelayAction);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numeric_SecondsDelayed);
            this.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.Name = "FormDelayTest";
            this.Text = "FormDelayTest";
            ((System.ComponentModel.ISupportInitialize)(this.numeric_SecondsDelayed)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numeric_SecondsDelayed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_DelayAction;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label_Time;
    }
}