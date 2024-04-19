using CommonLib.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function
{
    /// <summary>
    /// 转换类
    /// </summary>
    public static class Converter
    {
        #region 原Convert方法
        ///// <summary>
        ///// 泛型类型转换
        ///// </summary>
        ///// <param name="type">要转换的基础类型</param>
        ///// <param name="source">要转换的值</param>
        ///// <returns>返回转换后的实体类对象</returns>
        //public static object Convert(Type type, object source)
        //{
        //    //假如原数据为空（或数据库空值），返回类型的新实例
        //    if (source == null || source.GetType().Name.Equals("DBNull"))
        //        return type.CreateDefValue();

        //    //泛型Nullable判断，取其中的类型
        //    if (type.IsGenericType)
        //        type = type.GetGenericArguments()[0];

        //    //反射获取TryParse方法
        //    return System.Convert.ChangeType(source, type);
        //}
        #endregion

        /// <summary>
        /// 泛型类型转换
        /// </summary>
        /// <param name="type">要转换的基础类型</param>
        /// <param name="source">要转换的值</param>
        /// <param name="def"></param>
        /// <returns>返回转换后的实体类对象</returns>
        /// <exception cref="ArgumentNullException">类型type可能为空</exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static object Convert(Type type, object source, object def)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type), "待转换的目标类型为空");
            //假如原数据为空，或空白字符串，或数据库空值，返回类型的新实例
            if (source == null || (source is string str && string.IsNullOrWhiteSpace(str)) || source.GetType().Name.Equals("DBNull"))
                return def;

            //泛型Nullable判断，取其中的类型
            if (type.IsGenericType)
                type = type.GetGenericArguments()[0];

            //反射获取TryParse方法
            return System.Convert.ChangeType(source, type);
        }

        /// <summary>
        /// 泛型类型转换
        /// </summary>
        /// <param name="type">要转换的基础类型</param>
        /// <param name="source">要转换的值</param>
        /// <returns>返回转换后的实体类对象</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static object Convert(Type type, object source)
        {
            return Convert(type, source, type.CreateDefValue());
        }

        //private static readonly MethodInfo convert_type_method = typeof(Converter).GetMethod("ConvertType", new Type[] { typeof(object) });
        /// <summary>
        /// 表示静态类型转换方法ConvertType的方法属性，此方法为泛型方法，需根据泛型类型参数生成具体的泛型方法
        /// </summary>
        public static MethodInfo ConvertTypeMethod { get; } = typeof(Converter).GetMethod("ConvertType", new Type[] { typeof(object) });

        /// <summary>
        /// 泛型类型转换，为空时的默认值为基础类型默认值
        /// </summary>
        /// <typeparam name="T">要转换的基础类型</typeparam>
        /// <param name="source">要转换的值</param>
        /// <returns>返回转换后的实体类对象</returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static T ConvertType<T>(this object source)
        {
            //return ConvertType<T>(source, default);
            return ConvertType(source, (T)typeof(T).CreateDefValue());
        }

        /// <summary>
        /// 泛型类型转换
        /// </summary>
        /// <typeparam name="T">要转换的基础类型</typeparam>
        /// <param name="source">要转换的值</param>
        /// <param name="def">假如值为空的默认值</param>
        /// <returns>返回转换后的实体类对象</returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static T ConvertType<T>(this object source, T def)
        {
            //假如原数据为空（或数据库空值），返回类型的新实例
            Type type = typeof(T);
            object value = Convert(type, source, def);
            #region 原转换部分
            //object value;
            //if (source == null || source.GetType().Name.Equals("DBNull"))
            //    return def;

            ////泛型Nullable判断，取其中的类型
            //if (type.IsGenericType)
            //    type = type.GetGenericArguments()[0];

            ////反射获取TryParse方法
            //value = System.Convert.ChangeType(source, type);
            #endregion
            return (T)value;
        }

        /// <summary>
        /// 将DataRow中某一列的值转换为特定类型的值，假如为空，则返回该类型默认值
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="row">DataRow对象</param>
        /// <param name="column">列名称</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static T Convert<T>(this DataRow row, string column)
        {
            //return ConvertDataRow<T>(row, column, default);
            return Convert<T>(row, column, default);
        }

        /// <summary>
        /// 将DataRow中某一列的值转换为特定类型的值，假如为空，则返回默认值
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="row">DataRow对象</param>
        /// <param name="column">列名称</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static T Convert<T>(this DataRow row, string column, T def)
        {
            //return ConvertDataRow(row, column, def);
            bool flag = row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value; //值是否为空
            object value = flag ? null : row[column];
            return ConvertType(value, def);
        }

        /// <summary>
        /// 将DataRow中某一列的值转换为特定类型的值，假如为空，则返回默认值
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="row">DataRow对象</param>
        /// <param name="column">列名称</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        [Obsolete("请使用Convert<T>方法")]
        public static T ConvertDataRow<T>(this DataRow row, string column, T def)
        {
            bool flag = row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value; //值是否为空
            object value = flag ? null : row[column];
            return ConvertType(value, def);
        }

        /// <summary>
        /// 将DataRow中某一列的值转换为特定类型的值，假如为空，则返回该类型默认值
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="row">DataRow对象</param>
        /// <param name="column">列名称</param>
        /// <returns></returns>
        [Obsolete("请使用Convert<T>方法")]
        public static T ConvertDataRow<T>(this DataRow row, string column)
        {
            return ConvertDataRow<T>(row, column, default);
        }
    }
}
