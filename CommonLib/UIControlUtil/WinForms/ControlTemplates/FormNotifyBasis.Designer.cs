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
            this.components = new System.ComponentModel.Container();
            this.notifyIcon_Main = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip_Main = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenu_Show = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenu_Hide = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenu_NotifyExit = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon_Main
            // 
            this.notifyIcon_Main.ContextMenuStrip = this.contextMenuStrip_Main;
            this.notifyIcon_Main.Visible = true;
            this.notifyIcon_Main.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_Main_MouseDoubleClick);
            // 
            // contextMenuStrip_Main
            // 
            this.contextMenuStrip_Main.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenu_Show,
            this.toolStripMenu_Hide,
            this.toolStripSeparator5,
            this.toolStripMenu_NotifyExit});
            this.contextMenuStrip_Main.Name = "contextMenuStrip1";
            this.contextMenuStrip_Main.Size = new System.Drawing.Size(111, 82);
            // 
            // toolStripMenu_Show
            // 
            this.toolStripMenu_Show.Name = "toolStripMenu_Show";
            this.toolStripMenu_Show.Size = new System.Drawing.Size(110, 24);
            this.toolStripMenu_Show.Text = "显示";
            this.toolStripMenu_Show.Click += new System.EventHandler(this.ToolStripMenu_Show_Click);
            // 
            // toolStripMenu_Hide
            // 
            this.toolStripMenu_Hide.Name = "toolStripMenu_Hide";
            this.toolStripMenu_Hide.Size = new System.Drawing.Size(110, 24);
            this.toolStripMenu_Hide.Text = "隐藏";
            this.toolStripMenu_Hide.Click += new System.EventHandler(this.ToolStripMenu_Hide_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(107, 6);
            // 
            // toolStripMenu_NotifyExit
            // 
            this.toolStripMenu_NotifyExit.Name = "toolStripMenu_NotifyExit";
            this.toolStripMenu_NotifyExit.Size = new System.Drawing.Size(110, 24);
            this.toolStripMenu_NotifyExit.Text = "退出";
            this.toolStripMenu_NotifyExit.Click += new System.EventHandler(this.ToolStripMenu_NotifyExit_Click);
            // 
            // FormNotifyBasis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "FormNotifyBasis";
            this.Text = "FormNotifyBasis";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.Resize += new System.EventHandler(this.FormNotifyBasis_Resize);
            this.contextMenuStrip_Main.ResumeLayout(false);
            this.ResumeLayout(false);

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