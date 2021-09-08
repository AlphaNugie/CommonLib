namespace CommonLibExample
{
    partial class FormFilters
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
            this.button_KalmanFilter = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_KalmanFilter
            // 
            this.button_KalmanFilter.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_KalmanFilter.Location = new System.Drawing.Point(48, 32);
            this.button_KalmanFilter.Name = "button_KalmanFilter";
            this.button_KalmanFilter.Size = new System.Drawing.Size(145, 50);
            this.button_KalmanFilter.TabIndex = 1;
            this.button_KalmanFilter.Text = "卡尔曼滤波";
            this.button_KalmanFilter.UseVisualStyleBackColor = true;
            this.button_KalmanFilter.Click += new System.EventHandler(this.button_KalmanFilter_Click);
            // 
            // FormFilters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_KalmanFilter);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FormFilters";
            this.Text = "滤波";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_KalmanFilter;
    }
}