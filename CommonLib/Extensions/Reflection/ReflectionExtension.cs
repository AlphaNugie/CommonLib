using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Extensions.Reflection
{
    /// <summary>
    /// 反射相关的拓展类
    /// </summary>
    public static class ReflectionExtension
    {
        /// <summary>
        /// 获取类型的默认值，假如类型对象不为空且为值类型则构造一个实例，否则返回null
        /// </summary>
        /// <param name="type">给定的类型实体</param>
        /// <returns></returns>
        public static object CreateDefValue(this Type type)
        {
            return type != null && type.IsValueType ? Activator.CreateInstance(type) : null;
        }

    }
}
