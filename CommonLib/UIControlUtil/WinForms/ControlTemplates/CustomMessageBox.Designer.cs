namespace CommonLib.UIControlUtil.ControlTemplates
{
    partial class CustomMessageBox
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomMessageBox));
            lblMessageText = new System.Windows.Forms.Label();
            pnlShowMessage = new System.Windows.Forms.Panel();
            imageList1 = new System.Windows.Forms.ImageList(components);
            pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).BeginInit();
            SuspendLayout();
            // 
            // lblMessageText
            // 
            lblMessageText.BackColor = System.Drawing.Color.Transparent;
            lblMessageText.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblMessageText.Location = new System.Drawing.Point(85, 11);
            lblMessageText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMessageText.Name = "lblMessageText";
            lblMessageText.Size = new System.Drawing.Size(378, 30);
            lblMessageText.TabIndex = 0;
            lblMessageText.Text = "label1";
            // 
            // pnlShowMessage
            // 
            pnlShowMessage.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            pnlShowMessage.BackColor = System.Drawing.Color.Transparent;
            pnlShowMessage.Location = new System.Drawing.Point(86, 60);
            pnlShowMessage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            pnlShowMessage.Name = "pnlShowMessage";
            pnlShowMessage.Size = new System.Drawing.Size(387, 47);
            pnlShowMessage.TabIndex = 1;
            // 
            // imageList1
            // 
            imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            imageList1.TransparentColor = System.Drawing.Color.Transparent;
            imageList1.Images.SetKeyName(0, "Error");
            imageList1.Images.SetKeyName(1, "Information");
            imageList1.Images.SetKeyName(2, "Question");
            imageList1.Images.SetKeyName(3, "Warning");
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new System.Drawing.Point(15, 13);
            pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(32, 32);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // CustomMessageBox
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new System.Drawing.Size(477, 110);
            Controls.Add(pictureBox1);
            Controls.Add(pnlShowMessage);
            Controls.Add(lblMessageText);
            Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CustomMessageBox";
            Opacity = 0.98D;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            FontChanged += new System.EventHandler(CustomMessageBox_FontChanged);
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessageText;
        private System.Windows.Forms.Panel pnlShowMessage;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}