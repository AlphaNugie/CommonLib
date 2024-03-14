namespace CommonLib.UIControlUtil.ControlTemplates
{
    partial class FormNotifyBasis
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
            notifyIcon_Main = new System.Windows.Forms.NotifyIcon(components);
            contextMenuStrip_Main = new System.Windows.Forms.ContextMenuStrip(components);
            toolStripMenu_Show = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenu_Hide = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenu_NotifyExit = new System.Windows.Forms.ToolStripMenuItem();
            contextMenuStrip_Main.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon_Main
            // 
            notifyIcon_Main.ContextMenuStrip = contextMenuStrip_Main;
            notifyIcon_Main.Visible = true;
            notifyIcon_Main.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_Main_MouseDoubleClick);
            // 
            // contextMenuStrip_Main
            // 
            contextMenuStrip_Main.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextMenuStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenu_Show,
            toolStripMenu_Hide,
            toolStripSeparator5,
            toolStripMenu_NotifyExit});
            contextMenuStrip_Main.Name = "contextMenuStrip1";
            contextMenuStrip_Main.Size = new System.Drawing.Size(109, 82);
            // 
            // toolStripMenu_Show
            // 
            toolStripMenu_Show.Name = "toolStripMenu_Show";
            toolStripMenu_Show.Size = new System.Drawing.Size(108, 24);
            toolStripMenu_Show.Text = "显示";
            toolStripMenu_Show.Click += new System.EventHandler(ToolStripMenu_Show_Click);
            // 
            // toolStripMenu_Hide
            // 
            toolStripMenu_Hide.Name = "toolStripMenu_Hide";
            toolStripMenu_Hide.Size = new System.Drawing.Size(108, 24);
            toolStripMenu_Hide.Text = "隐藏";
            toolStripMenu_Hide.Click += new System.EventHandler(ToolStripMenu_Hide_Click);
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(105, 6);
            // 
            // toolStripMenu_NotifyExit
            // 
            toolStripMenu_NotifyExit.Name = "toolStripMenu_NotifyExit";
            toolStripMenu_NotifyExit.Size = new System.Drawing.Size(108, 24);
            toolStripMenu_NotifyExit.Text = "退出";
            toolStripMenu_NotifyExit.Click += new System.EventHandler(ToolStripMenu_NotifyExit_Click);
            // 
            // FormNotifyBasis
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Name = "FormNotifyBasis";
            Text = "FormNotifyBasis";
            FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form_FormClosing);
            TextChanged += new System.EventHandler(FormNotifyBasis_TextChanged);
            Resize += new System.EventHandler(FormNotifyBasis_Resize);
            contextMenuStrip_Main.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem toolStripMenu_Show;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenu_Hide;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenu_NotifyExit;
        protected System.Windows.Forms.NotifyIcon notifyIcon_Main;
        protected System.Windows.Forms.ContextMenuStrip contextMenuStrip_Main;
    }
}