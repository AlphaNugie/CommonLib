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
        public static T ConvertDataGridViewRow2Obect<T>(DataGridViewRow row, bool throwing)
        {
            T obj = Activator.CreateInstance<T>();
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

            return obj;
        }

        /// <summary>
        /// 将DataGridViewRow对象转换为实体类对象，列名应为 "XXX_[PropertyName]" 的形式
        /// </summary>
        /// <typeparam name="T">欲转换为的泛型类</typeparam>
        /// <param name="row">DataGridViewRow对象</param>
        /// <returns></returns>
        public static T ConvertDataGridViewRow2Obect<T>(DataGridViewRow row)
        {
            return ConvertDataGridViewRow2Obect<T>(row, true);
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
        public static List<DataGridViewRow> GetDataGridViewSelectedRows(this DataGridView gridView)
        {
            List<DataGridViewRow> list;
            DataGridViewSelectedRowCollection rows = gridView.SelectedRows;
            list = rows == null ? new List<DataGridViewRow>() : rows.Cast<DataGridViewRow>().ToList();
            list.Add(gridView.CurrentRow);

            return list;
        }
    }
}
