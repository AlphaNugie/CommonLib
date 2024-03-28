using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OPCAutomation;
using CommonLib.DataUtil;
using CommonLib.Function;
using CommonLib.UIControlUtil;
using CommonLib.Extensions;
using System.IO;
using OpcLibrary;
using OpcLibrary.Core;
using OpcLibrary.DataUtil;
using OpcLibrary.Model;
//using ScanOutputDemo.DataUtil;

namespace OpcLibrary.Controls.Forms
{
    /// <summary>
    /// 与OpcLibrary.OpcTaskBase配套的OPC项配置窗口，使用与OpcTaskBase匹配的数据源
    /// </summary>
    [Obsolete("请使用OpcLibraryAnyCpu.dll")]
    public partial class FormOpcConfig : Form
    {
        private readonly DataService_OpcItem _dataService = new DataService_OpcItem(); //数据库服务类
        private readonly OpcUtilHelper _opcHelper = new OpcUtilHelper(1000, true);
        private readonly BindingList<OpcItem> _binding = new BindingList<OpcItem>();

        #region 事件
        /// <summary>
        /// OPC连接成功事件的委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void OpcServerConnectedEventHandler(object sender, OpcServerConnectedEventArgs e);

        /// <summary>
        /// OPC连接成功事件
        /// </summary>
        public OpcServerConnectedEventHandler OpcServerConnected;
        #endregion

        #region 属性
        /// <summary>
        /// OPC服务端IP
        /// </summary>
        public string OpcServerIp
        {
            get { return textBox_OpcServerIp.Text; }
            private set { textBox_OpcServerIp.Text = value; }
        }

        /// <summary>
        /// OPC服务端IP
        /// </summary>
        public string OpcServerName
        {
            get { return string.IsNullOrWhiteSpace(comboBox_OpcServerList.Text) ? comboBox_OpcServerList.SelectedText : comboBox_OpcServerList.Text; }
            private set { comboBox_OpcServerList.Text = value; }
        }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        public FormOpcConfig()
        {
            InitializeComponent();
            InitControls();
            DataSourceRefresh();
        }

        private void FormOpcServerTest_Load(object sender, EventArgs e) { }

        private void InitControls()
        {
            OpcServerIp = OpcConst.OpcServerIp;
            OpcServerName = OpcConst.OpcServerName;
            dataGridView_Main.AutoGenerateColumns = false;
            dataGridView_Main.SetDoubleBuffered(true);
            dataGridView_Main.DataSource = _binding;
            Column_Enabled.TrueValue = true;
            Column_Enabled.FalseValue = false;
        }

        /// <summary>
        /// 刷新数据源
        /// </summary>
        private void DataSourceRefresh()
        {
            CheckForTableColumns();
            DataTable table;
            try { table = _dataService.GetAllOpcItemsOrderbyId(); }
            catch (Exception e)
            {
                string errorMessage = "查询时出错：" + e.Message;
                MessageBox.Show(errorMessage, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _binding.Clear();
            table.Rows.Cast<DataRow>().ToList().ForEach(row => _binding.Add(new OpcItem(row)));
            //dataGridView_Main.DataSource = table;
            //dataGridView_Main.DataSource = table;
        }

        /// <summary>
        /// 检查数据项表的字段，假如缺少字段则增加
        /// </summary>
        private void CheckForTableColumns()
        {
            bool result = _dataService.CheckForTableColumns(out string message);
            if (!string.IsNullOrWhiteSpace(message))
                MessageBox.Show(message, "提示", MessageBoxButtons.OK, result ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

        private void ServerEnum()
        {
            if (string.IsNullOrWhiteSpace(OpcServerIp))
            {
                MessageBox.Show("请输入IP地址！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (OpcServerIp != "localhost" && !RegexMatcher.IsIpAddressValidated(OpcServerIp))//用正则表达式验证IP地址
            {
                MessageBox.Show("请输入正确格式的IP地址！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            comboBox_OpcServerList.Items.Clear(); //清空已显示的OPC Server列表
            var servers = _opcHelper.ServerEnum(OpcServerIp, out string message);
            if (!string.IsNullOrWhiteSpace(message))
                MessageBox.Show("无法连接此IP地址的OPC Server：" + message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (servers == null || servers.Length == 0)
                return;
            comboBox_OpcServerList.Items.AddRange(servers);
            comboBox_OpcServerList.SelectedIndex = 0;
        }

        private void ServerConnect()
        {
            string server = OpcServerName;
            string ip = OpcServerIp;
            if (!_opcHelper.ConnectRemoteServer(ip, server, out string message))
            {
                MessageBox.Show(string.Format("位于{0}的OPC服务{1}连接失败：{2}", ip, server, message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //触发OPC连接成功事件
            OpcServerConnected?.BeginInvoke(this, new OpcServerConnectedEventArgs(ip, server), null, null);
            ////连接成功后保存到配置文件
            //BaseConst.IniHelper.WriteData("OPC", "ServerIp", ip);
            //BaseConst.IniHelper.WriteData("OPC", "ServerName", server);
            /*string */
            message = string.Format("位于{0}的OPC服务{1}连接成功", ip, server);
            MessageBox.Show(message, "提示");
        }

        #region 事件
        private void Button_ServerEnum_Click(object sender, EventArgs e)
        {
            ServerEnum();
        }

        private void Button_Connect_Click(object sender, EventArgs e)
        {
            ServerConnect();
        }

        private void TextBox_OpcServerIp_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (e.KeyCode == Keys.Enter)
                button_ServerEnum.PerformClick();
        }

        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //取消事件，完成代码处理后再添加事件（代码中改变单元格的值会导致死循环）
                dataGridView_Main.CellValueChanged -= new DataGridViewCellEventHandler(DataGridView_CellValueChanged);
                //dataGridView_Main.Rows[e.RowIndex].Cells["Column_Changed"].Value = 1;
                _binding[e.RowIndex].Changed = true;
                dataGridView_Main.CellValueChanged += new DataGridViewCellEventHandler(DataGridView_CellValueChanged);
            }
        }

        /// <summary>
        /// 刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Refresh_Click(object sender, EventArgs e)
        {
            DataSourceRefresh();
        }

        /// <summary>
        /// 新增按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Add_Click(object sender, EventArgs e)
        {
            ////按数据库字段的顺序排列，最后一列为remark
            //object[] values = new object[] { 0, string.Empty, 1, string.Empty, 0, 0, 0, 0 };
            //((DataTable)dataGridView_Main.DataSource).Rows.Add(values);
            OpcItem item = new OpcItem() { ItemId = string.Empty, OpcGroupId = 1, FieldName = string.Empty };
            _binding.Add(item);
            if (dataGridView_Main.Rows.Count == 0)
                return;
            dataGridView_Main.Rows[dataGridView_Main.Rows.Count - 1].Selected = true;
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Save_Click(object sender, EventArgs e)
        {
            if (dataGridView_Main.Rows.Count == 0)
                return;

            //List<OpcItem> list = new List<OpcItem>();
            //foreach (DataGridViewRow row in dataGridView_Main.Rows)
            //{
            //    //object obj = row.Cells["Column_RecordId"].Value;
            //    //obj = row.Cells["Column_Changed"].Value;
            //    //找到新增或修改行
            //    if (row.Cells["Column_RecordId"].Value.ToString().Equals("0") || row.Cells["Column_Changed"].Value.ToString().Equals("1"))
            //    {
            //        //OpcItem item = row.Convert2Object<OpcItem>(row, false); //不抛出异常
            //        OpcItem item = row.Convert2Object<OpcItem>(false); //不抛出异常
            //        if (item.OpcGroupId > 0) list.Add(item);
            //        else
            //        {
            //            MessageBox.Show("所属OPC组不得为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            return;
            //        }
            //    }
            //}
            List<OpcItem> list = _binding.Where(item => (item.RecordId == 0 || item.Changed) && item.OpcGroupId > 0).ToList(); //只要新增或修改行

            bool result;
            try { result = _dataService.SaveOpcItems(list); }
            catch (Exception ex)
            {
                string errorMessage = "保存时出现问题：" + ex.Message;
                MessageBox.Show(errorMessage, "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (result)
            {
                MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DataSourceRefresh();
            }
            else
                MessageBox.Show("保存失败", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Delete_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> gridViewRows = dataGridView_Main.GetSelectedRows();
            if (gridViewRows.Count == 0)
            {
                MessageBox.Show("未选中任何行", "提示");
                return;
            }
            if (MessageBox.Show("确定删除所选记录吗", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                return;
            List<int> list = new List<int>();
            gridViewRows.ForEach(gridViewRow =>
            {
                int id = int.Parse(gridViewRow.Cells["Column_RecordId"].Value.ToString());
                //假如为新增未保存的行，直接删除
                if (id == 0)
                    dataGridView_Main.Rows.Remove(gridViewRow);
                else
                    list.Add(id);
            });
            int result = 0;
            try { result = _dataService.DeleteOpcItemByIds(list); }
            catch (Exception ex)
            {
                MessageBox.Show("删除记录时出现问题 " + ex.Message);
                return;
            }
            MessageBox.Show(result > 0 ? string.Format("成功删除{0}条记录", list.Count) : "删除失败");
            if (result > 0)
                DataSourceRefresh();
        }

        private void Button_BrowseFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog_DbFile.ShowDialog() != DialogResult.OK)
                return;
            richTextBox_FolderPath.Text = openFileDialog_DbFile.FileName;
            _dataService.SetFilePath(openFileDialog_DbFile.FileName);
            DataSourceRefresh();
        }

        private void Button_Import_Click(object sender, EventArgs e)
        {
            if (openFileDialog_CsvFile.ShowDialog() != DialogResult.OK)
                return;
            List<OpcItem> items;
            string fileName = openFileDialog_CsvFile.FileName;
            //导入文件内容，假如导入失败则弹出提示
            try { items = File.ReadAllLines(fileName).Select(line => new OpcItem(line)).Where(item => item.RecordId != -1).ToList(); }
            catch (IOException ex)
            {
                MessageBox.Show(string.Format("导入文件{0}的操作失败：{1}", fileName, ex.Message));
                return;
            }
            //items.ForEach(item =>
            //{
            //    //按数据库字段的顺序排列
            //    object[] values = new object[] { 0, item.ItemId, item.OpcGroupId, item.FieldName, item.Enabled ? 1 : 0, item.Coeff, item.Offset, 0 };
            //    ((DataTable)dataGridView_Main.DataSource).Rows.Add(values);
            //});

            //读取与写入的标签分开查询
            List<OpcItem> itemsToRead = _binding.Where(item => item.OpcGroupId == 1).ToList(), itemsToWrite = _binding.Where(item => item.OpcGroupId > 1).ToList();
            //新增的标签数量，更新的标签数量
            int added = 0, updated = 0;
            items.ForEach(item =>
            {
                List<OpcItem> equals = null;
                if (item.OpcGroupId == 1)
                    equals = itemsToRead.Where(i => i.Equals(item)).ToList();
                else if (item.OpcGroupId > 1)
                    equals = itemsToWrite.Where(i => i.Equals(item)).ToList();
                if (equals == null || equals.Count == 0)
                {
                    _binding.Add(item);
                    added++;
                }
                else
                    equals.ForEach(i =>
                    {
                        i.Copy(item);
                        updated++;
                    });
            });
            if (items.Count == 0)
                MessageBox.Show("文件未包含任何待导入内容", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                dataGridView_Main.Refresh(); //先使工作区无效再执行Update()
                //dataGridView_Main.Update();
                ////MessageBox.Show(string.Format("已在下方表格末尾添加{0}行记录", items.Count), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show(string.Format("更新{0}条已有记录，在末尾添加{1}行新记录", updated, added), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Button_Export_Click(object sender, EventArgs e)
        {
            //转换为OpcItem对象并转为导出文件格式的字符串，假如数量不足则提示
            List<string> exports = dataGridView_Main.GetSelectedRows().Select(row => row.Convert2Object<OpcItem>(false)).OrderBy(item => item.RecordId).Select(item => item.ToExportString()).ToList();
            if (exports.Count == 0)
            {
                MessageBox.Show("未选中任何行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (saveFileDialog_CsvFile.ShowDialog() != DialogResult.OK)
                return;
            exports.Insert(0, "0,标签名称,OPC组,数据源实体类字段,是否启用,系数,偏移");
            bool result = false;
            string error = string.Empty;
            try
            {
                File.WriteAllLines(saveFileDialog_CsvFile.FileName, exports, Encoding.UTF8);
                result = true;
            }
            catch (Exception ex) { error = ex.Message; }
            MessageBox.Show(result ? "保存成功" : error, "提示", MessageBoxButtons.OK, result ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

        //private readonly string _lastItemId = string.Empty;
        private void DataGridView_Main_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int columnIndex = e.ColumnIndex/*, rowIndex = e.RowIndex*/;
            //判断读取或写入
            string columnnName = dataGridView_Main.Columns[columnIndex].Name;
            if (!columnnName.Equals("Column_GetValue") && !columnnName.Equals("Column_WriteValue"))
                return;
            bool writing = columnnName.Equals("Column_WriteValue");

            #region 添加Item
            if (_opcHelper.ServerState != OPCServerState.OPCRunning)
            {
                MessageBox.Show("OPC服务未连接", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string itemId = dataGridView_Main.Rows[e.RowIndex].Cells["Column_ItemId"].Value.ToString();
            if (!_opcHelper.SetItem(itemId, 1, out string message))
            {
                MessageBox.Show("未能添加该OPC Item：" + message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion
            #region 获取Item的值
            if (!_opcHelper.ReadItemValue(out string value, out message))
            {
                MessageBox.Show("未找到该OPC Item的值：" + message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!writing)
            {
                dataGridView_Main.Rows[e.RowIndex].Cells["Column_ItemValue"].Value = value;
                return;
            }
            //return;
            #endregion
            #region 向Item写入值
            var raw = dataGridView_Main.Rows[e.RowIndex].Cells["Column_ItemValue"].Value;
            value = raw == null || raw == DBNull.Value ? null : raw.ToString();
            if (!_opcHelper.WriteItemValue(value, out message))
                MessageBox.Show("未能向该OPC Item写入值：" + message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// OPC服务连接成功事件的参数实体类
    /// </summary>
    public class OpcServerConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// OPC服务端IP
        /// </summary>
        public string OpcServerIp { get; set; }

        /// <summary>
        /// OPC服务端名称
        /// </summary>
        public string OpcServerName { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="serverIp">OPC服务端IP</param>
        /// <param name="serverName">OPC服务端名称</param>
        public OpcServerConnectedEventArgs(string serverIp, string serverName)
        {
            OpcServerIp = serverIp;
            OpcServerName = serverName;
        }
    }
}
