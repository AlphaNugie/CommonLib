namespace CommonLibExample
{
    partial class FormOpcTest
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
            this.textBox_OpcServerIp = new System.Windows.Forms.TextBox();
            this.button_ServerEnum = new System.Windows.Forms.Button();
            this.comboBox_OpcServerList = new System.Windows.Forms.ComboBox();
            this.button_Connect = new System.Windows.Forms.Button();
            this.button_WriteTestItem = new System.Windows.Forms.Button();
            this.button_AddTestItem = new System.Windows.Forms.Button();
            this.textBox_TestValueWrite = new System.Windows.Forms.TextBox();
            this.textBox_TestItemId = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_TestValueRead = new System.Windows.Forms.TextBox();
            this.button_ReadTestItem = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(20, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "IP地址";
            // 
            // textBox_OpcServerIp
            // 
            this.textBox_OpcServerIp.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_OpcServerIp.Location = new System.Drawing.Point(92, 33);
            this.textBox_OpcServerIp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox_OpcServerIp.Name = "textBox_OpcServerIp";
            this.textBox_OpcServerIp.Size = new System.Drawing.Size(165, 28);
            this.textBox_OpcServerIp.TabIndex = 5;
            this.textBox_OpcServerIp.Text = "127.0.0.1";
            // 
            // button_ServerEnum
            // 
            this.button_ServerEnum.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ServerEnum.Location = new System.Drawing.Point(568, 33);
            this.button_ServerEnum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_ServerEnum.Name = "button_ServerEnum";
            this.button_ServerEnum.Size = new System.Drawing.Size(64, 29);
            this.button_ServerEnum.TabIndex = 6;
            this.button_ServerEnum.Text = "枚举";
            this.button_ServerEnum.UseVisualStyleBackColor = true;
            this.button_ServerEnum.Click += new System.EventHandler(this.Button_ServerEnum_Click);
            // 
            // comboBox_OpcServerList
            // 
            this.comboBox_OpcServerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_OpcServerList.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_OpcServerList.FormattingEnabled = true;
            this.comboBox_OpcServerList.Location = new System.Drawing.Point(263, 33);
            this.comboBox_OpcServerList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox_OpcServerList.Name = "comboBox_OpcServerList";
            this.comboBox_OpcServerList.Size = new System.Drawing.Size(299, 29);
            this.comboBox_OpcServerList.TabIndex = 8;
            // 
            // button_Connect
            // 
            this.button_Connect.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Connect.Location = new System.Drawing.Point(638, 33);
            this.button_Connect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(64, 29);
            this.button_Connect.TabIndex = 7;
            this.button_Connect.Text = "连接";
            this.button_Connect.UseVisualStyleBackColor = true;
            this.button_Connect.Click += new System.EventHandler(this.Button_Connect_Click);
            // 
            // button_WriteTestItem
            // 
            this.button_WriteTestItem.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_WriteTestItem.Location = new System.Drawing.Point(568, 168);
            this.button_WriteTestItem.Name = "button_WriteTestItem";
            this.button_WriteTestItem.Size = new System.Drawing.Size(68, 30);
            this.button_WriteTestItem.TabIndex = 14;
            this.button_WriteTestItem.Text = "写入";
            this.button_WriteTestItem.UseVisualStyleBackColor = true;
            this.button_WriteTestItem.Click += new System.EventHandler(this.Button_WriteTestItem_Click);
            // 
            // button_AddTestItem
            // 
            this.button_AddTestItem.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_AddTestItem.Location = new System.Drawing.Point(568, 79);
            this.button_AddTestItem.Name = "button_AddTestItem";
            this.button_AddTestItem.Size = new System.Drawing.Size(68, 28);
            this.button_AddTestItem.TabIndex = 13;
            this.button_AddTestItem.Text = "添加";
            this.button_AddTestItem.UseVisualStyleBackColor = true;
            this.button_AddTestItem.Click += new System.EventHandler(this.Button_AddTestItem_Click);
            // 
            // textBox_TestValueWrite
            // 
            this.textBox_TestValueWrite.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_TestValueWrite.Location = new System.Drawing.Point(92, 168);
            this.textBox_TestValueWrite.Name = "textBox_TestValueWrite";
            this.textBox_TestValueWrite.Size = new System.Drawing.Size(470, 28);
            this.textBox_TestValueWrite.TabIndex = 12;
            this.textBox_TestValueWrite.Text = "100";
            // 
            // textBox_TestItemId
            // 
            this.textBox_TestItemId.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_TestItemId.Location = new System.Drawing.Point(92, 79);
            this.textBox_TestItemId.Name = "textBox_TestItemId";
            this.textBox_TestItemId.Size = new System.Drawing.Size(470, 28);
            this.textBox_TestItemId.TabIndex = 10;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(36, 82);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(50, 21);
            this.label14.TabIndex = 9;
            this.label14.Text = "标签";
            // 
            // textBox_TestValueRead
            // 
            this.textBox_TestValueRead.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_TestValueRead.Location = new System.Drawing.Point(92, 124);
            this.textBox_TestValueRead.Name = "textBox_TestValueRead";
            this.textBox_TestValueRead.Size = new System.Drawing.Size(470, 28);
            this.textBox_TestValueRead.TabIndex = 12;
            this.textBox_TestValueRead.Text = "100";
            // 
            // button_ReadTestItem
            // 
            this.button_ReadTestItem.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ReadTestItem.Location = new System.Drawing.Point(568, 124);
            this.button_ReadTestItem.Name = "button_ReadTestItem";
            this.button_ReadTestItem.Size = new System.Drawing.Size(68, 30);
            this.button_ReadTestItem.TabIndex = 14;
            this.button_ReadTestItem.Text = "读取";
            this.button_ReadTestItem.UseVisualStyleBackColor = true;
            this.button_ReadTestItem.Click += new System.EventHandler(this.Button_ReadTestItem_Click);
            // 
            // FormOpcTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_ReadTestItem);
            this.Controls.Add(this.button_WriteTestItem);
            this.Controls.Add(this.button_AddTestItem);
            this.Controls.Add(this.textBox_TestValueRead);
            this.Controls.Add(this.textBox_TestValueWrite);
            this.Controls.Add(this.textBox_TestItemId);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_OpcServerIp);
            this.Controls.Add(this.button_ServerEnum);
            this.Controls.Add(this.comboBox_OpcServerList);
            this.Controls.Add(this.button_Connect);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FormOpcTest";
            this.Text = "FormOpcTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_OpcServerIp;
        private System.Windows.Forms.Button button_ServerEnum;
        private System.Windows.Forms.ComboBox comboBox_OpcServerList;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.Button button_WriteTestItem;
        private System.Windows.Forms.Button button_AddTestItem;
        private System.Windows.Forms.TextBox textBox_TestValueWrite;
        private System.Windows.Forms.TextBox textBox_TestItemId;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_TestValueRead;
        private System.Windows.Forms.Button button_ReadTestItem;
    }
}