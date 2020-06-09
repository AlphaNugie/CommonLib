namespace CommonLibExample
{
    partial class FormEncryption
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
            this.button_AesEncrypt = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_AesSource = new System.Windows.Forms.TextBox();
            this.button_AesDecrypt = new System.Windows.Forms.Button();
            this.textBox_AesResult = new System.Windows.Forms.TextBox();
            this.button_DesEncrypt = new System.Windows.Forms.Button();
            this.button_DesDecrypt = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_DesSource = new System.Windows.Forms.TextBox();
            this.textBox_DesResult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_AesEncrypt
            // 
            this.button_AesEncrypt.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_AesEncrypt.Location = new System.Drawing.Point(58, 229);
            this.button_AesEncrypt.Margin = new System.Windows.Forms.Padding(4);
            this.button_AesEncrypt.Name = "button_AesEncrypt";
            this.button_AesEncrypt.Size = new System.Drawing.Size(78, 46);
            this.button_AesEncrypt.TabIndex = 0;
            this.button_AesEncrypt.Text = "↓";
            this.button_AesEncrypt.UseVisualStyleBackColor = true;
            this.button_AesEncrypt.Click += new System.EventHandler(this.Button_AesEncrypt_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(140, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "AES";
            // 
            // textBox_AesSource
            // 
            this.textBox_AesSource.Location = new System.Drawing.Point(12, 79);
            this.textBox_AesSource.Multiline = true;
            this.textBox_AesSource.Name = "textBox_AesSource";
            this.textBox_AesSource.Size = new System.Drawing.Size(294, 126);
            this.textBox_AesSource.TabIndex = 2;
            // 
            // button_AesDecrypt
            // 
            this.button_AesDecrypt.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_AesDecrypt.Location = new System.Drawing.Point(180, 229);
            this.button_AesDecrypt.Margin = new System.Windows.Forms.Padding(4);
            this.button_AesDecrypt.Name = "button_AesDecrypt";
            this.button_AesDecrypt.Size = new System.Drawing.Size(72, 46);
            this.button_AesDecrypt.TabIndex = 0;
            this.button_AesDecrypt.Text = "↑";
            this.button_AesDecrypt.UseVisualStyleBackColor = true;
            this.button_AesDecrypt.Click += new System.EventHandler(this.Button_AesDecrypt_Click);
            // 
            // textBox_AesResult
            // 
            this.textBox_AesResult.Location = new System.Drawing.Point(12, 303);
            this.textBox_AesResult.Multiline = true;
            this.textBox_AesResult.Name = "textBox_AesResult";
            this.textBox_AesResult.Size = new System.Drawing.Size(294, 136);
            this.textBox_AesResult.TabIndex = 2;
            // 
            // button_DesEncrypt
            // 
            this.button_DesEncrypt.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_DesEncrypt.Location = new System.Drawing.Point(389, 229);
            this.button_DesEncrypt.Margin = new System.Windows.Forms.Padding(4);
            this.button_DesEncrypt.Name = "button_DesEncrypt";
            this.button_DesEncrypt.Size = new System.Drawing.Size(78, 46);
            this.button_DesEncrypt.TabIndex = 0;
            this.button_DesEncrypt.Text = "↓";
            this.button_DesEncrypt.UseVisualStyleBackColor = true;
            this.button_DesEncrypt.Click += new System.EventHandler(this.Button_DesEncrypt_Click);
            // 
            // button_DesDecrypt
            // 
            this.button_DesDecrypt.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_DesDecrypt.Location = new System.Drawing.Point(511, 229);
            this.button_DesDecrypt.Margin = new System.Windows.Forms.Padding(4);
            this.button_DesDecrypt.Name = "button_DesDecrypt";
            this.button_DesDecrypt.Size = new System.Drawing.Size(72, 46);
            this.button_DesDecrypt.TabIndex = 0;
            this.button_DesDecrypt.Text = "↑";
            this.button_DesDecrypt.UseVisualStyleBackColor = true;
            this.button_DesDecrypt.Click += new System.EventHandler(this.Button_DesDecrypt_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(471, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "DES";
            // 
            // textBox_DesSource
            // 
            this.textBox_DesSource.Location = new System.Drawing.Point(343, 79);
            this.textBox_DesSource.Multiline = true;
            this.textBox_DesSource.Name = "textBox_DesSource";
            this.textBox_DesSource.Size = new System.Drawing.Size(294, 126);
            this.textBox_DesSource.TabIndex = 2;
            // 
            // textBox_DesResult
            // 
            this.textBox_DesResult.Location = new System.Drawing.Point(343, 303);
            this.textBox_DesResult.Multiline = true;
            this.textBox_DesResult.Name = "textBox_DesResult";
            this.textBox_DesResult.Size = new System.Drawing.Size(294, 136);
            this.textBox_DesResult.TabIndex = 2;
            // 
            // FormEncryption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 477);
            this.Controls.Add(this.textBox_DesResult);
            this.Controls.Add(this.textBox_AesResult);
            this.Controls.Add(this.textBox_DesSource);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_AesSource);
            this.Controls.Add(this.button_DesDecrypt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_DesEncrypt);
            this.Controls.Add(this.button_AesDecrypt);
            this.Controls.Add(this.button_AesEncrypt);
            this.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormEncryption";
            this.Text = "FormEncryption";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_AesEncrypt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_AesSource;
        private System.Windows.Forms.Button button_AesDecrypt;
        private System.Windows.Forms.TextBox textBox_AesResult;
        private System.Windows.Forms.Button button_DesEncrypt;
        private System.Windows.Forms.Button button_DesDecrypt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_DesSource;
        private System.Windows.Forms.TextBox textBox_DesResult;
    }
}