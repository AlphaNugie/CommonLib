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
        /// <param name="gridView"></param>
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
    }
}
