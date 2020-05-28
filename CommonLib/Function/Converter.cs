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
        ///// <summary>
        ///// 泛型类型转换
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="source">要转换的值</param>
        ///// <returns>返回转换后的实体类对象</returns>
        //public static object ConvertType(this object source, Type type)
        //{
        //    //假如原数据为空（或数据库空值），返回类型的新实例
        //    if (source == null || source.GetType().Name.Equals("DBNull"))
        //    {
        //        //假如是值类型，生成新实例，否则返回null
        //        if (type.IsValueType)
        //            return Activator.CreateInstance(type);
        //        else
        //            return null;
        //    }

        //    //泛型Nullable判断，取其中的类型
        //    if (type.IsGenericType)
        //        type = type.GetGenericArguments()[0];

        //    //反射获取TryParse方法
        //    return System.Convert.ChangeType(source, type);
        //}

        /// <summary>
        /// 泛型类型转换
        /// </summary>
        /// <param name="type">要转换的基础类型</param>
        /// <param name="source">要转换的值</param>
        /// <returns>返回转换后的实体类对象</returns>
        public static object Convert(Type type,  object source)
        {
            //假如原数据为空（或数据库空值），返回类型的新实例
            if (source == null || source.GetType().Name.Equals("DBNull"))
            {
                //假如是值类型，生成新实例，否则返回null
                if (type.IsValueType)
                    return Activator.CreateInstance(type);
                else
                    return null;
            }

            //泛型Nullable判断，取其中的类型
            if (type.IsGenericType)
                type = type.GetGenericArguments()[0];

            //反射获取TryParse方法
            return System.Convert.ChangeType(source, type);
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
        public static T ConvertType<T>(this object source)
        {
            return ConvertType<T>(source, default);
        }

        /// <summary>
        /// 泛型类型转换
        /// </summary>
        /// <typeparam name="T">要转换的基础类型</typeparam>
        /// <param name="source">要转换的值</param>
        /// <param name="def">假如值为空的默认值</param>
        /// <returns>返回转换后的实体类对象</returns>
        public static T ConvertType<T>(this object source, T def)
        {
            //假如原数据为空（或数据库空值），返回类型的新实例
            Type type = typeof(T);
            object value;
            if (source == null || source.GetType().Name.Equals("DBNull"))
            {
                return def;
                //value = def;
                //return (T)value;
            }

            //泛型Nullable判断，取其中的类型
            if (type.IsGenericType)
                type = type.GetGenericArguments()[0];

            //反射获取TryParse方法
            value = System.Convert.ChangeType(source, type);
            return (T)value;
        }

        ///// <summary>
        ///// 将某个值转换为特定类型，假如为空，则返回默认值
        ///// </summary>
        ///// <typeparam name="T">转换的目标类型</typeparam>
        ///// <param name="value">待转换值</param>
        ///// <param name="def">默认值</param>
        ///// <returns></returns>
        //public static T Convert<T>(object value, T def)
        //{
        //    if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        //        value = def;
        //    object obj = (object)value;
        //    return (T)obj;
        //}

        ///// <summary>
        ///// 将某个值转换为特定类型，假如为空，选择该类型默认值返回
        ///// </summary>
        ///// <typeparam name="T">转换的目标类型</typeparam>
        ///// <param name="value">待转换值</param>
        ///// <returns></returns>
        //public static T Convert<T>(object value)
        //{
        //    return Convert<T>(value, default(T));
        //}

        /// <summary>
        /// 将DataRow中某一列的值转换为特定类型的值，假如为空，则返回默认值
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="row">DataRow对象</param>
        /// <param name="column">列名称</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static T ConvertDataRow<T>(DataRow row, string column, T def)
        {
            bool flag = row == null || row.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value; //值是否为空
            object value = flag ? null : row[column];
            return ConvertType<T>(value, def);
            //object value = flag ? def : row[column];
            //return (T)(object)value;
        }

        /// <summary>
        /// 将DataRow中某一列的值转换为特定类型的值，假如为空，则返回该类型默认值
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="row">DataRow对象</param>
        /// <param name="column">列名称</param>
        /// <returns></returns>
        public static T ConvertDataRow<T>(DataRow row, string column)
        {
            return ConvertDataRow<T>(row, column, default);
        }
    }
}
