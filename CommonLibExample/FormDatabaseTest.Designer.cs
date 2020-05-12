namespace CommonLibExample
{
    partial class FormDatabaseTest
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
            this.button_OracleTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_OracleTest
            // 
            this.button_OracleTest.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_OracleTest.Location = new System.Drawing.Point(55, 34);
            this.button_OracleTest.Name = "button_OracleTest";
            this.button_OracleTest.Size = new System.Drawing.Size(145, 50);
            this.button_OracleTest.TabIndex = 0;
            this.button_OracleTest.Text = "Oracle";
            this.button_OracleTest.UseVisualStyleBackColor = true;
            this.button_OracleTest.Click += new System.EventHandler(this.button_OracleTest_Click);
            // 
            // FormDatabaseTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 462);
            this.Controls.Add(this.button_OracleTest);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FormDatabaseTest";
            this.Text = "FormDatabaseTest";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_OracleTest;
    }
}