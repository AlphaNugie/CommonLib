using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function
{
    /// <summary>
    /// 类功能扩展类
    /// </summary>
    public static class ExtensionClass
    {
        ///// <summary>
        ///// 判断是否在两个数值之间（或等于）
        ///// </summary>
        ///// <param name="input">待判断的数字</param>
        ///// <param name="number1">数值1</param>
        ///// <param name="number2">数值2</param>
        ///// <returns>假如在数值之间，返回true，否则返回false</returns>
        //public static bool Between(this double input, double number1, double number2)
        //{
        //    return (input >= number1 && input <= number2) || (input >= number2 && input <= number1);
        //}

        /// <summary>
        /// 泛型类的扩展方法，使用双缓存（适用于DataGridView / ListView等
        /// </summary>
        /// <typeparam name="T">欲扩展方法的类型</typeparam>
        /// <param name="obj">泛型对象，对泛型进行扩展</param>
        /// <param name="setting">是否启用双缓存</param>
        public static void SetDoubleBuffered<T>(this T obj, bool setting)
        {
            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            propertyInfo.SetValue(obj, setting, null);
        }
    }
}
