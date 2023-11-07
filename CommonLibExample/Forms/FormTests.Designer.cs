namespace CommonLibExample.Forms
{
    partial class FormTests
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
            this.button_TimerEventRaiser = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_TimerEventRaiser
            // 
            this.button_TimerEventRaiser.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_TimerEventRaiser.Location = new System.Drawing.Point(12, 12);
            this.button_TimerEventRaiser.Name = "button_TimerEventRaiser";
            this.button_TimerEventRaiser.Size = new System.Drawing.Size(170, 40);
            this.button_TimerEventRaiser.TabIndex = 1;
            this.button_TimerEventRaiser.Text = "ExampleTaskTest";
            this.button_TimerEventRaiser.UseVisualStyleBackColor = true;
            this.button_TimerEventRaiser.Click += new System.EventHandler(this.Button_TimerEventRaiser_Click);
            // 
            // FormTests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 488);
            this.Controls.Add(this.button_TimerEventRaiser);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FormTests";
            this.Text = "FormTests";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_TimerEventRaiser;
    }
}