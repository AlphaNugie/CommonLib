using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Extensions
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的备注信息（在枚举内的项上方添加形如“[EnumDescription("XXX")]”的注释）
        /// </summary>
        /// <param name="em">枚举对象</param>
        /// <returns></returns>
        public static string GetDescription(this Enum em)
        {
            Type type = em.GetType();

            FieldInfo
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            field = type.GetField(em.ToString());

            if (field == null)
                return string.Empty;
            object[] attrs = field.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            string name = string.Empty;
            //foreach (EnumDescriptionAttribute attr in attrs)
            //将 object 类型显式强制转换为 EnumDescriptionAttribute 类型
            foreach (EnumDescriptionAttribute attr in attrs.Cast<EnumDescriptionAttribute>())
                name = attr.Description;
            return name;
        }

        /// <summary>
        /// 获取枚举的别名信息（在枚举内的项上方添加形如“[EnumAlias("XXX")]”的注释）
        /// </summary>
        /// <param name="em">枚举对象</param>
        /// <returns></returns>
        public static string GetAlias(this Enum em)
        {
            Type type = em.GetType();
            FieldInfo
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            field = type.GetField(em.ToString());
            if (field == null)
                return string.Empty;
            object[] attrs = field.GetCustomAttributes(typeof(EnumAliasAttribute), false);
            string alias = string.Empty;
            //foreach (EnumAliasAttribute attr in attrs)
            //将 object 类型显式强制转换为 EnumDescriptionAttribute 类型
            foreach (EnumAliasAttribute attr in attrs.Cast<EnumAliasAttribute>())
                alias = attr.Alias;
            return alias;
        }

        /// <summary>
        /// 获取枚举的数值（在枚举内的项上方添加形如“[EnumValue("XXX")]”的注释），假如没有找到对应Attribute或者内部不是数值格式，则返回null
        /// </summary>
        /// <param name="em">枚举对象</param>
        /// <returns></returns>
        public static double? GetValue(this Enum em)
        {
            Type type = em.GetType();
            FieldInfo
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            field = type.GetField(em.ToString());
            double? value = null;
            if (field == null)
                //return null;
                goto END;
            object[] attrs = field.GetCustomAttributes(typeof(EnumValueAttribute), false);
            //foreach (EnumValueAttribute attr in attrs)
            //将 object 类型显式强制转换为 EnumDescriptionAttribute 类型
            foreach (EnumValueAttribute attr in attrs.Cast<EnumValueAttribute>())
                value = attr.Value;
            END:
            return value;
        }
    }

    /// <summary>
    /// 枚举的数值（双精度浮点数）属性类
    /// </summary>
#if NET9_0_OR_GREATER
    [AttributeUsage(AttributeTargets.Enum)]
#endif
    public sealed class EnumValueAttribute : Attribute
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="value">给定的数值（双精度浮点数）</param>
        public EnumValueAttribute(string value)
        {
            if (double.TryParse(value, out double d))
                Value = d;
            else
                Value = null;
        }

        /// <summary>
        /// 枚举数值
        /// </summary>
        public double? Value { get; }
    }

#if NET45_OR_GREATER
    /// <summary>
    /// 枚举注释的自定义属性类
    /// </summary>
    public sealed class EnumDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="strPrinterName">注释内容</param>
        public EnumDescriptionAttribute(string strPrinterName)
        {
            Description = strPrinterName;
        }

        /// <summary>
        /// 枚举注释
        /// </summary>
        public string Description { get; }
    }

    /// <summary>
    /// 枚举别名的自定义属性类
    /// </summary>
    public sealed class EnumAliasAttribute : Attribute
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="alias">别名内容</param>
        public EnumAliasAttribute(string alias)
        {
            Alias = alias;
        }

        /// <summary>
        /// 枚举别名
        /// </summary>
        public string Alias { get; }
    }
#elif NET9_0_OR_GREATER
    /// <summary>
    /// 枚举注释的自定义属性类
    /// </summary>
    /// <remarks>
    /// 构造器
    /// </remarks>
    /// <param name="strPrinterName">注释内容</param>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EnumDescriptionAttribute(string strPrinterName) : Attribute
    {
        /// <summary>
        /// 枚举注释
        /// </summary>
        public string Description { get; } = strPrinterName;
    }

    /// <summary>
    /// 枚举别名的自定义属性类
    /// </summary>
    /// <remarks>
    /// 构造器
    /// </remarks>
    /// <param name="alias">别名内容</param>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EnumAliasAttribute(string alias) : Attribute
    {
        /// <summary>
        /// 枚举注释
        /// </summary>
        public string Alias { get; } = alias;
    }
#endif
}
