namespace CommonLibExample
{
    partial class FormOracleTest
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
            this.textBox_Address = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_ServiceName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.button_GetConnStr = new System.Windows.Forms.Button();
            this.textBox_ConnStr = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBox_Mode = new System.Windows.Forms.ComboBox();
            this.button_MultiQuery = new System.Windows.Forms.Button();
            this.button_Query = new System.Windows.Forms.Button();
            this.button_ExecuteSql = new System.Windows.Forms.Button();
            this.button_ExecuteSqlTrans = new System.Windows.Forms.Button();
            this.button_IsConnOopen = new System.Windows.Forms.Button();
            this.label_IsConnOpen = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "地址";
            // 
            // textBox_Address
            // 
            this.textBox_Address.Location = new System.Drawing.Point(97, 12);
            this.textBox_Address.Name = "textBox_Address";
            this.textBox_Address.Size = new System.Drawing.Size(143, 23);
            this.textBox_Address.TabIndex = 1;
            this.textBox_Address.Text = "localhost";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "端口";
            // 
            // textBox_Port
            // 
            this.textBox_Port.Location = new System.Drawing.Point(97, 41);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(143, 23);
            this.textBox_Port.TabIndex = 2;
            this.textBox_Port.Text = "1521";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "服务名";
            // 
            // textBox_ServiceName
            // 
            this.textBox_ServiceName.Location = new System.Drawing.Point(97, 70);
            this.textBox_ServiceName.Name = "textBox_ServiceName";
            this.textBox_ServiceName.Size = new System.Drawing.Size(143, 23);
            this.textBox_ServiceName.TabIndex = 3;
            this.textBox_ServiceName.Text = "orcl";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "用户名";
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.Location = new System.Drawing.Point(97, 99);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(143, 23);
            this.textBox_UserName.TabIndex = 4;
            this.textBox_UserName.Text = "test";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 131);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "密码";
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(97, 128);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.Size = new System.Drawing.Size(143, 23);
            this.textBox_Password.TabIndex = 5;
            this.textBox_Password.Text = "123";
            // 
            // button_GetConnStr
            // 
            this.button_GetConnStr.Location = new System.Drawing.Point(262, 61);
            this.button_GetConnStr.Name = "button_GetConnStr";
            this.button_GetConnStr.Size = new System.Drawing.Size(61, 39);
            this.button_GetConnStr.TabIndex = 6;
            this.button_GetConnStr.Text = ">>";
            this.button_GetConnStr.UseVisualStyleBackColor = true;
            this.button_GetConnStr.Click += new System.EventHandler(this.Button_GetConnStr_Click);
            // 
            // textBox_ConnStr
            // 
            this.textBox_ConnStr.Location = new System.Drawing.Point(349, 12);
            this.textBox_ConnStr.Multiline = true;
            this.textBox_ConnStr.Name = "textBox_ConnStr";
            this.textBox_ConnStr.Size = new System.Drawing.Size(364, 139);
            this.textBox_ConnStr.TabIndex = 3;
            this.textBox_ConnStr.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(82, 180);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "测试表名";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(164, 180);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(159, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "T_TEST_PROVIDERTEST";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(402, 180);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 15);
            this.label8.TabIndex = 0;
            this.label8.Text = "模式";
            // 
            // comboBox_Mode
            // 
            this.comboBox_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Mode.FormattingEnabled = true;
            this.comboBox_Mode.Items.AddRange(new object[] {
            "App.config",
            "动态参数组合"});
            this.comboBox_Mode.Location = new System.Drawing.Point(466, 177);
            this.comboBox_Mode.Name = "comboBox_Mode";
            this.comboBox_Mode.Size = new System.Drawing.Size(131, 23);
            this.comboBox_Mode.TabIndex = 7;
            this.comboBox_Mode.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Mode_SelectedIndexChanged);
            // 
            // button_MultiQuery
            // 
            this.button_MultiQuery.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_MultiQuery.Location = new System.Drawing.Point(30, 335);
            this.button_MultiQuery.Name = "button_MultiQuery";
            this.button_MultiQuery.Size = new System.Drawing.Size(196, 60);
            this.button_MultiQuery.TabIndex = 9;
            this.button_MultiQuery.Text = "MultiQuery";
            this.button_MultiQuery.UseVisualStyleBackColor = true;
            this.button_MultiQuery.Click += new System.EventHandler(this.Button_MultiQuery_Click);
            // 
            // button_Query
            // 
            this.button_Query.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Query.Location = new System.Drawing.Point(466, 335);
            this.button_Query.Name = "button_Query";
            this.button_Query.Size = new System.Drawing.Size(196, 60);
            this.button_Query.TabIndex = 10;
            this.button_Query.Text = "Query";
            this.button_Query.UseVisualStyleBackColor = true;
            this.button_Query.Click += new System.EventHandler(this.Button_Query_Click);
            // 
            // button_ExecuteSql
            // 
            this.button_ExecuteSql.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ExecuteSql.Location = new System.Drawing.Point(30, 423);
            this.button_ExecuteSql.Name = "button_ExecuteSql";
            this.button_ExecuteSql.Size = new System.Drawing.Size(196, 60);
            this.button_ExecuteSql.TabIndex = 11;
            this.button_ExecuteSql.Text = "ExecuteSql";
            this.button_ExecuteSql.UseVisualStyleBackColor = true;
            this.button_ExecuteSql.Click += new System.EventHandler(this.Button_ExecuteSql_Click);
            // 
            // button_ExecuteSqlTrans
            // 
            this.button_ExecuteSqlTrans.Font = new System.Drawing.Font("等线", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ExecuteSqlTrans.Location = new System.Drawing.Point(466, 423);
            this.button_ExecuteSqlTrans.Name = "button_ExecuteSqlTrans";
            this.button_ExecuteSqlTrans.Size = new System.Drawing.Size(196, 60);
            this.button_ExecuteSqlTrans.TabIndex = 12;
            this.button_ExecuteSqlTrans.Text = "ExecuteSqlTrans";
            this.button_ExecuteSqlTrans.UseVisualStyleBackColor = true;
            this.button_ExecuteSqlTrans.Click += new System.EventHandler(this.Button_ExecuteSqlTrans_Click);
            // 
            // button_IsConnOopen
            // 
            this.button_IsConnOopen.Location = new System.Drawing.Point(288, 229);
            this.button_IsConnOopen.Name = "button_IsConnOopen";
            this.button_IsConnOopen.Size = new System.Drawing.Size(131, 57);
            this.button_IsConnOopen.TabIndex = 8;
            this.button_IsConnOopen.Text = "IsConnOpen";
            this.button_IsConnOopen.UseVisualStyleBackColor = true;
            this.button_IsConnOopen.Click += new System.EventHandler(this.Button_IsConnOopen_Click);
            // 
            // label_IsConnOpen
            // 
            this.label_IsConnOpen.AutoSize = true;
            this.label_IsConnOpen.Location = new System.Drawing.Point(331, 300);
            this.label_IsConnOpen.Name = "label_IsConnOpen";
            this.label_IsConnOpen.Size = new System.Drawing.Size(39, 15);
            this.label_IsConnOpen.TabIndex = 0;
            this.label_IsConnOpen.Text = "False";
            // 
            // FormOracleTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 526);
            this.Controls.Add(this.button_IsConnOopen);
            this.Controls.Add(this.button_ExecuteSqlTrans);
            this.Controls.Add(this.button_ExecuteSql);
            this.Controls.Add(this.button_Query);
            this.Controls.Add(this.button_MultiQuery);
            this.Controls.Add(this.comboBox_Mode);
            this.Controls.Add(this.textBox_ConnStr);
            this.Controls.Add(this.button_GetConnStr);
            this.Controls.Add(this.textBox_Password);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_UserName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_ServiceName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_Port);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_Address);
            this.Controls.Add(this.label_IsConnOpen);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("等线", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FormOracleTest";
            this.Text = "FormOracleTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Address;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_ServiceName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.Button button_GetConnStr;
        private System.Windows.Forms.TextBox textBox_ConnStr;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox_Mode;
        private System.Windows.Forms.Button button_MultiQuery;
        private System.Windows.Forms.Button button_Query;
        private System.Windows.Forms.Button button_ExecuteSql;
        private System.Windows.Forms.Button button_ExecuteSqlTrans;
        private System.Windows.Forms.Button button_IsConnOopen;
        private System.Windows.Forms.Label label_IsConnOpen;
    }
}