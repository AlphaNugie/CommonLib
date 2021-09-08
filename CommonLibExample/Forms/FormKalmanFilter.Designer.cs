namespace CommonLibExample
{
    partial class FormKalmanFilter
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
            this.textBox_Eval = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Add = new System.Windows.Forms.Button();
            this.numeric_Value = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numeric_Q = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numeric_R = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numeric_Velocity = new System.Windows.Forms.NumericUpDown();
            this.button_Init = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_Value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_Q)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_R)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_Velocity)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(56, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "观测值";
            // 
            // textBox_Eval
            // 
            this.textBox_Eval.Enabled = false;
            this.textBox_Eval.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_Eval.Location = new System.Drawing.Point(407, 196);
            this.textBox_Eval.Name = "textBox_Eval";
            this.textBox_Eval.Size = new System.Drawing.Size(127, 28);
            this.textBox_Eval.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(307, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "预测值";
            // 
            // button_Add
            // 
            this.button_Add.Enabled = false;
            this.button_Add.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Add.Location = new System.Drawing.Point(584, 115);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(99, 34);
            this.button_Add.TabIndex = 2;
            this.button_Add.Text = "添加";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.Button_Add_Click);
            // 
            // numeric_Value
            // 
            this.numeric_Value.DecimalPlaces = 2;
            this.numeric_Value.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numeric_Value.Location = new System.Drawing.Point(156, 120);
            this.numeric_Value.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numeric_Value.Minimum = new decimal(new int[] {
            9999999,
            0,
            0,
            -2147483648});
            this.numeric_Value.Name = "numeric_Value";
            this.numeric_Value.Size = new System.Drawing.Size(135, 28);
            this.numeric_Value.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(101, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 21);
            this.label3.TabIndex = 1;
            this.label3.Text = "Q";
            // 
            // numeric_Q
            // 
            this.numeric_Q.DecimalPlaces = 2;
            this.numeric_Q.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numeric_Q.Location = new System.Drawing.Point(156, 39);
            this.numeric_Q.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numeric_Q.Minimum = new decimal(new int[] {
            9999999,
            0,
            0,
            -2147483648});
            this.numeric_Q.Name = "numeric_Q";
            this.numeric_Q.Size = new System.Drawing.Size(135, 28);
            this.numeric_Q.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(355, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 21);
            this.label4.TabIndex = 1;
            this.label4.Text = "R";
            // 
            // numeric_R
            // 
            this.numeric_R.DecimalPlaces = 2;
            this.numeric_R.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numeric_R.Location = new System.Drawing.Point(407, 39);
            this.numeric_R.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numeric_R.Minimum = new decimal(new int[] {
            9999999,
            0,
            0,
            -2147483648});
            this.numeric_R.Name = "numeric_R";
            this.numeric_R.Size = new System.Drawing.Size(127, 28);
            this.numeric_R.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(327, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 21);
            this.label5.TabIndex = 1;
            this.label5.Text = "速度";
            // 
            // numeric_Velocity
            // 
            this.numeric_Velocity.DecimalPlaces = 4;
            this.numeric_Velocity.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numeric_Velocity.Location = new System.Drawing.Point(407, 120);
            this.numeric_Velocity.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numeric_Velocity.Minimum = new decimal(new int[] {
            9999999,
            0,
            0,
            -2147483648});
            this.numeric_Velocity.Name = "numeric_Velocity";
            this.numeric_Velocity.Size = new System.Drawing.Size(127, 28);
            this.numeric_Velocity.TabIndex = 3;
            // 
            // button_Init
            // 
            this.button_Init.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Init.Location = new System.Drawing.Point(584, 34);
            this.button_Init.Name = "button_Init";
            this.button_Init.Size = new System.Drawing.Size(99, 34);
            this.button_Init.TabIndex = 2;
            this.button_Init.Text = "初始化";
            this.button_Init.UseVisualStyleBackColor = true;
            this.button_Init.Click += new System.EventHandler(this.Button_Init_Click);
            // 
            // FormKalmanFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.numeric_R);
            this.Controls.Add(this.numeric_Q);
            this.Controls.Add(this.numeric_Velocity);
            this.Controls.Add(this.numeric_Value);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button_Init);
            this.Controls.Add(this.button_Add);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_Eval);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FormKalmanFilter";
            this.Text = "卡尔曼滤波";
            ((System.ComponentModel.ISupportInitialize)(this.numeric_Value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_Q)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_R)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_Velocity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Eval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.NumericUpDown numeric_Value;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numeric_Q;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numeric_R;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numeric_Velocity;
        private System.Windows.Forms.Button button_Init;
    }
}