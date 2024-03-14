namespace OpcLibrary.Controls.Forms
{
    partial class FormOpcConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOpcConfig));
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_OpcServerIp = new System.Windows.Forms.TextBox();
            this.button_ServerEnum = new System.Windows.Forms.Button();
            this.comboBox_OpcServerList = new System.Windows.Forms.ComboBox();
            this.button_Connect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView_Main = new System.Windows.Forms.DataGridView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button_Refresh = new System.Windows.Forms.Button();
            this.button_Add = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button_Delete = new System.Windows.Forms.Button();
            this.richTextBox_FolderPath = new System.Windows.Forms.RichTextBox();
            this.button_BrowseFile = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_Export = new System.Windows.Forms.Button();
            this.button_Import = new System.Windows.Forms.Button();
            this.openFileDialog_DbFile = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog_CsvFile = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog_CsvFile = new System.Windows.Forms.SaveFileDialog();
            this.Column_RecordId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_ItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_OpcGroupId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_FieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Enabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_Coeff = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_GetValue = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column_ItemValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Changed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_WriteValue = new System.Windows.Forms.DataGridViewButtonColumn();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Main)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP地址";
            // 
            // textBox_OpcServerIp
            // 
            this.textBox_OpcServerIp.Location = new System.Drawing.Point(73, 28);
            this.textBox_OpcServerIp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox_OpcServerIp.Name = "textBox_OpcServerIp";
            this.textBox_OpcServerIp.Size = new System.Drawing.Size(165, 23);
            this.textBox_OpcServerIp.TabIndex = 1;
            this.textBox_OpcServerIp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_OpcServerIp_KeyDown);
            // 
            // button_ServerEnum
            // 
            this.button_ServerEnum.Location = new System.Drawing.Point(549, 28);
            this.button_ServerEnum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_ServerEnum.Name = "button_ServerEnum";
            this.button_ServerEnum.Size = new System.Drawing.Size(64, 28);
            this.button_ServerEnum.TabIndex = 2;
            this.button_ServerEnum.Text = "枚举";
            this.button_ServerEnum.UseVisualStyleBackColor = true;
            this.button_ServerEnum.Click += new System.EventHandler(this.Button_ServerEnum_Click);
            // 
            // comboBox_OpcServerList
            // 
            this.comboBox_OpcServerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_OpcServerList.FormattingEnabled = true;
            this.comboBox_OpcServerList.Location = new System.Drawing.Point(244, 28);
            this.comboBox_OpcServerList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox_OpcServerList.Name = "comboBox_OpcServerList";
            this.comboBox_OpcServerList.Size = new System.Drawing.Size(299, 25);
            this.comboBox_OpcServerList.TabIndex = 3;
            // 
            // button_Connect
            // 
            this.button_Connect.Location = new System.Drawing.Point(619, 28);
            this.button_Connect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(64, 28);
            this.button_Connect.TabIndex = 2;
            this.button_Connect.Text = "连接";
            this.button_Connect.UseVisualStyleBackColor = true;
            this.button_Connect.Click += new System.EventHandler(this.Button_Connect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox_OpcServerIp);
            this.groupBox1.Controls.Add(this.button_ServerEnum);
            this.groupBox1.Controls.Add(this.comboBox_OpcServerList);
            this.groupBox1.Controls.Add(this.button_Connect);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(880, 73);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "OPC服务器";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 179F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView_Main, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1065, 559);
            this.tableLayoutPanel1.TabIndex = 26;
            // 
            // dataGridView_Main
            // 
            this.dataGridView_Main.AllowUserToAddRows = false;
            this.dataGridView_Main.AllowUserToDeleteRows = false;
            this.dataGridView_Main.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Main.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_RecordId,
            this.Column_ItemId,
            this.Column_OpcGroupId,
            this.Column_FieldName,
            this.Column_Enabled,
            this.Column_Coeff,
            this.Column_Offset,
            this.Column_GetValue,
            this.Column_ItemValue,
            this.Column_Changed,
            this.Column_WriteValue});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridView_Main, 2);
            this.dataGridView_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Main.Location = new System.Drawing.Point(3, 134);
            this.dataGridView_Main.Name = "dataGridView_Main";
            this.dataGridView_Main.RowHeadersWidth = 51;
            this.dataGridView_Main.RowTemplate.Height = 27;
            this.dataGridView_Main.Size = new System.Drawing.Size(1059, 422);
            this.dataGridView_Main.TabIndex = 25;
            this.dataGridView_Main.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_Main_CellContentClick);
            this.dataGridView_Main.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellValueChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.button_Refresh);
            this.flowLayoutPanel1.Controls.Add(this.button_Add);
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Controls.Add(this.button_Delete);
            this.flowLayoutPanel1.Controls.Add(this.richTextBox_FolderPath);
            this.flowLayoutPanel1.Controls.Add(this.button_BrowseFile);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 84);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1059, 44);
            this.flowLayoutPanel1.TabIndex = 26;
            // 
            // button_Refresh
            // 
            this.button_Refresh.Location = new System.Drawing.Point(3, 3);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Size = new System.Drawing.Size(74, 35);
            this.button_Refresh.TabIndex = 17;
            this.button_Refresh.Text = "刷新";
            this.button_Refresh.UseVisualStyleBackColor = true;
            this.button_Refresh.Click += new System.EventHandler(this.Button_Refresh_Click);
            // 
            // button_Add
            // 
            this.button_Add.Location = new System.Drawing.Point(83, 3);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(74, 35);
            this.button_Add.TabIndex = 17;
            this.button_Add.Text = "新增";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.Button_Add_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(163, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 35);
            this.button1.TabIndex = 18;
            this.button1.Text = "保存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button_Save_Click);
            // 
            // button_Delete
            // 
            this.button_Delete.Location = new System.Drawing.Point(243, 3);
            this.button_Delete.Name = "button_Delete";
            this.button_Delete.Size = new System.Drawing.Size(74, 35);
            this.button_Delete.TabIndex = 19;
            this.button_Delete.Text = "删除";
            this.button_Delete.UseVisualStyleBackColor = true;
            this.button_Delete.Click += new System.EventHandler(this.Button_Delete_Click);
            // 
            // richTextBox_FolderPath
            // 
            this.richTextBox_FolderPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_FolderPath.Location = new System.Drawing.Point(323, 3);
            this.richTextBox_FolderPath.Name = "richTextBox_FolderPath";
            this.richTextBox_FolderPath.Size = new System.Drawing.Size(644, 35);
            this.richTextBox_FolderPath.TabIndex = 20;
            this.richTextBox_FolderPath.Text = "";
            // 
            // button_BrowseFile
            // 
            this.button_BrowseFile.Location = new System.Drawing.Point(973, 3);
            this.button_BrowseFile.Name = "button_BrowseFile";
            this.button_BrowseFile.Size = new System.Drawing.Size(75, 35);
            this.button_BrowseFile.TabIndex = 21;
            this.button_BrowseFile.Text = "浏览";
            this.button_BrowseFile.UseVisualStyleBackColor = true;
            this.button_BrowseFile.Click += new System.EventHandler(this.Button_BrowseFile_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_Export);
            this.groupBox2.Controls.Add(this.button_Import);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(889, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(173, 75);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "文件操作";
            // 
            // button_Export
            // 
            this.button_Export.Location = new System.Drawing.Point(87, 28);
            this.button_Export.Name = "button_Export";
            this.button_Export.Size = new System.Drawing.Size(75, 35);
            this.button_Export.TabIndex = 21;
            this.button_Export.Text = "导出";
            this.button_Export.UseVisualStyleBackColor = true;
            this.button_Export.Click += new System.EventHandler(this.Button_Export_Click);
            // 
            // button_Import
            // 
            this.button_Import.Location = new System.Drawing.Point(6, 28);
            this.button_Import.Name = "button_Import";
            this.button_Import.Size = new System.Drawing.Size(75, 35);
            this.button_Import.TabIndex = 21;
            this.button_Import.Text = "导入";
            this.button_Import.UseVisualStyleBackColor = true;
            this.button_Import.Click += new System.EventHandler(this.Button_Import_Click);
            // 
            // openFileDialog_DbFile
            // 
            this.openFileDialog_DbFile.FileName = "openFileDialog1";
            // 
            // openFileDialog_CsvFile
            // 
            this.openFileDialog_CsvFile.DefaultExt = "csv";
            this.openFileDialog_CsvFile.Filter = "CSV文件(*.csv)|*.csv|所有文件(*.*)|*.*";
            this.openFileDialog_CsvFile.InitialDirectory = "$(TargetDir)";
            this.openFileDialog_CsvFile.RestoreDirectory = true;
            this.openFileDialog_CsvFile.ShowHelp = true;
            this.openFileDialog_CsvFile.Title = "选择导入文件";
            // 
            // saveFileDialog_CsvFile
            // 
            this.saveFileDialog_CsvFile.DefaultExt = "csv";
            this.saveFileDialog_CsvFile.Filter = "CSV文件(*.csv)|*.csv|所有文件(*.*)|*.*";
            this.saveFileDialog_CsvFile.InitialDirectory = "$(TargetDir)";
            this.saveFileDialog_CsvFile.RestoreDirectory = true;
            this.saveFileDialog_CsvFile.ShowHelp = true;
            this.saveFileDialog_CsvFile.Title = "导出记录到文件";
            // 
            // Column_RecordId
            // 
            this.Column_RecordId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_RecordId.DataPropertyName = "RecordId";
            this.Column_RecordId.HeaderText = "ID";
            this.Column_RecordId.MinimumWidth = 6;
            this.Column_RecordId.Name = "Column_RecordId";
            this.Column_RecordId.ReadOnly = true;
            this.Column_RecordId.Width = 46;
            // 
            // Column_ItemId
            // 
            this.Column_ItemId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ItemId.DataPropertyName = "ItemId";
            this.Column_ItemId.HeaderText = "标签名称";
            this.Column_ItemId.MinimumWidth = 6;
            this.Column_ItemId.Name = "Column_ItemId";
            this.Column_ItemId.Width = 81;
            // 
            // Column_OpcGroupId
            // 
            this.Column_OpcGroupId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_OpcGroupId.DataPropertyName = "OpcGroupId";
            this.Column_OpcGroupId.HeaderText = "OPC组";
            this.Column_OpcGroupId.MinimumWidth = 6;
            this.Column_OpcGroupId.Name = "Column_OpcGroupId";
            this.Column_OpcGroupId.Width = 70;
            // 
            // Column_FieldName
            // 
            this.Column_FieldName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_FieldName.DataPropertyName = "FieldName";
            this.Column_FieldName.HeaderText = "数据源类字段";
            this.Column_FieldName.MinimumWidth = 6;
            this.Column_FieldName.Name = "Column_FieldName";
            this.Column_FieldName.Width = 105;
            // 
            // Column_Enabled
            // 
            this.Column_Enabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_Enabled.DataPropertyName = "Enabled";
            this.Column_Enabled.FalseValue = "false";
            this.Column_Enabled.HeaderText = "是否启用";
            this.Column_Enabled.MinimumWidth = 6;
            this.Column_Enabled.Name = "Column_Enabled";
            this.Column_Enabled.TrueValue = "true";
            this.Column_Enabled.Width = 62;
            // 
            // Column_Coeff
            // 
            this.Column_Coeff.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_Coeff.DataPropertyName = "Coeff";
            this.Column_Coeff.HeaderText = "系数";
            this.Column_Coeff.MinimumWidth = 6;
            this.Column_Coeff.Name = "Column_Coeff";
            this.Column_Coeff.Width = 57;
            // 
            // Column_Offset
            // 
            this.Column_Offset.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_Offset.DataPropertyName = "Offset";
            this.Column_Offset.HeaderText = "偏移量";
            this.Column_Offset.MinimumWidth = 6;
            this.Column_Offset.Name = "Column_Offset";
            this.Column_Offset.Width = 69;
            // 
            // Column_GetValue
            // 
            this.Column_GetValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_GetValue.HeaderText = "读取";
            this.Column_GetValue.MinimumWidth = 6;
            this.Column_GetValue.Name = "Column_GetValue";
            this.Column_GetValue.Width = 38;
            // 
            // Column_ItemValue
            // 
            this.Column_ItemValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ItemValue.HeaderText = "值";
            this.Column_ItemValue.MinimumWidth = 6;
            this.Column_ItemValue.Name = "Column_ItemValue";
            this.Column_ItemValue.Width = 45;
            // 
            // Column_Changed
            // 
            this.Column_Changed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_Changed.DataPropertyName = "Changed";
            this.Column_Changed.HeaderText = "是否改变";
            this.Column_Changed.MinimumWidth = 6;
            this.Column_Changed.Name = "Column_Changed";
            this.Column_Changed.Visible = false;
            this.Column_Changed.Width = 81;
            // 
            // Column_WriteValue
            // 
            this.Column_WriteValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_WriteValue.HeaderText = "写入";
            this.Column_WriteValue.Name = "Column_WriteValue";
            this.Column_WriteValue.Width = 38;
            // 
            // FormOpcConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1065, 559);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormOpcConfig";
            this.Text = "OPC配置";
            this.Load += new System.EventHandler(this.FormOpcServerTest_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Main)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_OpcServerIp;
        private System.Windows.Forms.Button button_ServerEnum;
        private System.Windows.Forms.ComboBox comboBox_OpcServerList;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button_Delete;
        private System.Windows.Forms.DataGridView dataGridView_Main;
        private System.Windows.Forms.OpenFileDialog openFileDialog_DbFile;
        private System.Windows.Forms.RichTextBox richTextBox_FolderPath;
        private System.Windows.Forms.Button button_BrowseFile;
        private System.Windows.Forms.Button button_Refresh;
        private System.Windows.Forms.Button button_Import;
        private System.Windows.Forms.OpenFileDialog openFileDialog_CsvFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_Export;
        private System.Windows.Forms.SaveFileDialog saveFileDialog_CsvFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_RecordId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_ItemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_OpcGroupId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_FieldName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column_Enabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Coeff;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Offset;
        private System.Windows.Forms.DataGridViewButtonColumn Column_GetValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_ItemValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Changed;
        private System.Windows.Forms.DataGridViewButtonColumn Column_WriteValue;
    }
}