using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enums
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
            FieldInfo field = type.GetField(em.ToString());
            if (field == null)
                return string.Empty;
            object[] attrs = field.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            string name = string.Empty;
            foreach (EnumDescriptionAttribute attr in attrs)
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
            FieldInfo field = type.GetField(em.ToString());
            if (field == null)
                return string.Empty;
            object[] attrs = field.GetCustomAttributes(typeof(EnumAliasAttribute), false);
            string alias = string.Empty;
            foreach (EnumAliasAttribute attr in attrs)
                alias = attr.Alias;
            return alias;
        }
    }

    /// <summary>
    /// 枚举注释的自定义属性类
    /// </summary>
    public class EnumDescriptionAttribute : Attribute
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
    public class EnumAliasAttribute : Attribute
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
        /// 枚举注释
        /// </summary>
        public string Alias { get; }
    }
}
