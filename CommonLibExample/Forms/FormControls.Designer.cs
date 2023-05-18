namespace CommonLibExample.Forms
{
    partial class FormControls
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
            this.button_TextBoxes = new System.Windows.Forms.Button();
            this.button_SineWaves = new System.Windows.Forms.Button();
            this.button_NormalDistribution = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_TextBoxes
            // 
            this.button_TextBoxes.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_TextBoxes.Location = new System.Drawing.Point(25, 25);
            this.button_TextBoxes.Name = "button_TextBoxes";
            this.button_TextBoxes.Size = new System.Drawing.Size(170, 40);
            this.button_TextBoxes.TabIndex = 1;
            this.button_TextBoxes.Text = "文本框";
            this.button_TextBoxes.UseVisualStyleBackColor = true;
            this.button_TextBoxes.Click += new System.EventHandler(this.Button_TextBoxes_Click);
            // 
            // button_SineWaves
            // 
            this.button_SineWaves.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_SineWaves.Location = new System.Drawing.Point(225, 25);
            this.button_SineWaves.Name = "button_SineWaves";
            this.button_SineWaves.Size = new System.Drawing.Size(170, 40);
            this.button_SineWaves.TabIndex = 1;
            this.button_SineWaves.Text = "绘制正弦波";
            this.button_SineWaves.UseVisualStyleBackColor = true;
            this.button_SineWaves.Click += new System.EventHandler(this.Button_SineWaves_Click);
            // 
            // button_NormalDistribution
            // 
            this.button_NormalDistribution.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_NormalDistribution.Location = new System.Drawing.Point(425, 25);
            this.button_NormalDistribution.Name = "button_NormalDistribution";
            this.button_NormalDistribution.Size = new System.Drawing.Size(170, 40);
            this.button_NormalDistribution.TabIndex = 1;
            this.button_NormalDistribution.Text = "绘制正态分布曲线";
            this.button_NormalDistribution.UseVisualStyleBackColor = true;
            this.button_NormalDistribution.Click += new System.EventHandler(this.Button_NormalDistribution_Click);
            // 
            // FormControls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_NormalDistribution);
            this.Controls.Add(this.button_SineWaves);
            this.Controls.Add(this.button_TextBoxes);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FormControls";
            this.Text = "控件测试";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_TextBoxes;
        private System.Windows.Forms.Button button_SineWaves;
        private System.Windows.Forms.Button button_NormalDistribution;
    }
}