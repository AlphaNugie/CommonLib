using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLib.Extensions;
using CommonLib.Function;

namespace CommonLib.UIControlUtil
{
    /// <summary>
    /// DataGridView相关方法
    /// </summary>
    public static class DataGridViewUtil
    {
        /// <summary>
        /// 将DataGridViewRow对象转换为实体类对象，列名应为 "XXX_[PropertyName]" 的形式
        /// </summary>
        /// <typeparam name="T">欲转换为的泛型类</typeparam>
        /// <param name="row">DataGridViewRow对象</param>
        /// <param name="throwing">是否抛出异常</param>
        /// <returns></returns>
        public static T Convert2Object<T>(this DataGridViewRow row, bool throwing) => ConvertDataGridViewRow2Object<T>(row, throwing);

        /// <summary>
        /// 将DataGridViewRow对象转换为实体类对象，列名应为 "XXX_[PropertyName]" 的形式
        /// </summary>
        /// <typeparam name="T">欲转换为的泛型类</typeparam>
        /// <param name="row">DataGridViewRow对象</param>
        /// <returns></returns>
        public static T Convert2Object<T>(this DataGridViewRow row) => ConvertDataGridViewRow2Object<T>(row, true);

        /// <summary>
        /// 将DataGridViewRow对象转换为实体类对象，列名应为 "XXX_[PropertyName]" 的形式
        /// （已过时，建议使用扩展方法Convert2Object）
        /// </summary>
        /// <typeparam name="T">欲转换为的泛型类</typeparam>
        /// <param name="row">DataGridViewRow对象</param>
        /// <returns></returns>
        [Obsolete]
        public static T ConvertDataGridViewRow2Object<T>(DataGridViewRow row) => ConvertDataGridViewRow2Object<T>(row, true);

        /// <summary>
        /// 将DataGridViewRow对象转换为实体类对象，列名应为 "XXX_[PropertyName]" 的形式
        /// </summary>
        /// <typeparam name="T">欲转换为的泛型类</typeparam>
        /// <param name="row">DataGridViewRow对象</param>
        /// <param name="throwing">是否抛出异常</param>
        /// <returns></returns>
        [Obsolete]
        public static T ConvertDataGridViewRow2Object<T>(DataGridViewRow row, bool throwing)
        {
            T obj = Activator.CreateInstance<T>();
            if (row == null)
                goto END;
            string columnName;
            string propName;
            //object value;
            foreach (DataGridViewCell cell in row.Cells)
            {
                try
                {
                    columnName = cell.OwningColumn.Name;
                    propName = columnName.Substring(columnName.LastIndexOf('_') + 1);
                    PropertyInfo property = obj.GetType().GetProperty(propName);
                    if (property != null)
                        property.SetValue(obj, Converter.Convert(property.PropertyType, cell.Value));
                }
                catch (Exception e)
                {
                    if (throwing)
                        throw e;
                }
            }

            END:
            return obj;
        }
    }

    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class DataGridViewExtentionClass
    {
        /// <summary>
        /// 为DataGridView启用双缓存
        /// </summary>
        /// <param name="gridView">DataGridView对象</param>
        /// <param name="setting">是否启用双缓存，假如为true则启用</param>
        public static void SetDoubleBuffered(this DataGridView gridView, bool setting)
        {
            ExtensionClass.SetDoubleBuffered(gridView, setting);
        }

        /// <summary>
        /// 获取DataGridView中的选中行或当前行
        /// </summary>
        /// <param name="gridView">DataGridView对象</param>
        /// <returns>返回List</returns>
        [Obsolete]
        public static List<DataGridViewRow> GetDataGridViewSelectedRows(this DataGridView gridView)
        {
            List<DataGridViewRow> list;
            DataGridViewSelectedRowCollection rows = gridView.SelectedRows;
            list = rows == null ? new List<DataGridViewRow>() : rows.Cast<DataGridViewRow>().ToList();
            list.Add(gridView.CurrentRow);

            return list;
        }

        /// <summary>
        /// 获取DataGridView的所有选中行，不受SelectionMode影响
        /// </summary>
        /// <param name="gridView">当前DataGridView</param>
        /// <returns></returns>
        public static List<DataGridViewRow> GetSelectedRows(this DataGridView gridView)
        {
            List<DataGridViewRow> gridViewRows = new List<DataGridViewRow>();
            if (gridView == null)
                goto END;
            //添加选中的行（根据选择模式(SelectionMode)可能数量为0）
            gridViewRows.AddRange(gridView.SelectedRows.Cast<DataGridViewRow>());
            //在选中的单元格中查找已选中同时还未被包括在列表中的行，找到后添加
            gridViewRows.AddRange(gridView.SelectedCells.Cast<DataGridViewCell>().Where(gridViewCell => !gridViewRows.Any(row => row.Index == gridViewCell.RowIndex)).Select(gridViewCell => gridViewCell.OwningRow));
            END:
            return gridViewRows;
        }

        /// <summary>
        /// 根据给定索引获取DataGridView的某一行，假如超出范围（小于0或大于等于总行数）则返回空
        /// </summary>
        /// <param name="gridView">当前DataGridView</param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static DataGridViewRow GetRowByIndex(this DataGridView gridView, int index)
        {
            return gridView == null || index < 0 || index >= gridView.Rows.Count ? null : gridView.Rows[index];
        }

        /// <summary>
        /// 在当前DataGridView中根据行索引选中给定的行
        /// </summary>
        /// <param name="gridView">当前DataGridView</param>
        /// <param name="index">给定行的索引，假如超出范围（小于0或大于等于总行数）则不继续向下进行任何操作</param>
        /// <param name="fullRowSelect">设置SelectionMode为FullRowSelect，假如为null或false则不设置</param>
        /// <param name="multiSelect">设置是否允许多选，假如为null则不设置</param>
        /// <returns></returns>
        public static bool SetRowSelected(this DataGridView gridView, int index, bool? fullRowSelect = null, bool? multiSelect = null)
        {
            return gridView == null || index < 0 || index >= gridView.Rows.Count ? false : SetRowSelected(gridView, gridView.GetRowByIndex(index), fullRowSelect, multiSelect);
        }

        /// <summary>
        /// 在当前DataGridView中选中给定的行
        /// </summary>
        /// <param name="gridView">当前DataGridView</param>
        /// <param name="viewRow">给定行对象，假如为空则不继续向下进行任何操作</param>
        /// <param name="fullRowSelect">设置SelectionMode为FullRowSelect，假如为null或false则不设置</param>
        /// <param name="multiSelect">设置是否允许多选，假如为null则不设置</param>
        /// <returns></returns>
        public static bool SetRowSelected(this DataGridView gridView, DataGridViewRow viewRow, bool? fullRowSelect = null, bool? multiSelect = null)
        {
            if (gridView == null || viewRow == null || viewRow.DataGridView == null) return false;
            //选中当前行
            viewRow.Selected = true;
            //获取第一个可见列及其名称（假如至少有一列可见）
            var column = gridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
            if (column == null) return false;
            //选择当前行的第一个可见单元格，选中单元格后滚动条将自动移动到当前行位置（设置不可见的单元格，程序将跳出）
            if (fullRowSelect != null && fullRowSelect.Value) gridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (multiSelect != null) gridView.MultiSelect = multiSelect.Value;
            gridView.CurrentCell = viewRow.Cells[column.Name];
            ////每行恢复默认背景色，然后将当前行颜色改变
            //foreach (DataGridViewRow row in dataGridView_Meters.Rows)
            //{
            //    row.DefaultCellStyle.BackColor = Color.White;
            //    row.DefaultCellStyle.ForeColor = Color.Black;
            //}
            //viewRow.DefaultCellStyle.BackColor = Color.Orange;
            //viewRow.DefaultCellStyle.ForeColor = Color.White;
            return true;
        }
    }
}
