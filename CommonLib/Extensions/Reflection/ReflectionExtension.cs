using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// 克隆一个新的对象出来，完全复制所有属性值（属性需同时支持读写）
        /// <para/>注意：引用类型将只复制引用
        /// </summary>
        /// <typeparam name="T">待克隆对象的类型</typeparam>
        /// <param name="obj">待克隆对象</param>
        /// <returns></returns>
        /// <exception cref="MissingMethodException">克隆对象的目标类型缺少无参构造器</exception>
        public static T Clone<T>(this T obj)
        {
            Type type = typeof(T);
            T newObj;
            //初始化一个目标类型的对象
            try { newObj = (T)Activator.CreateInstance(type); }
            catch (MissingMethodException e) { throw new MissingMethodException(string.Format($"待克隆的目标类型{type.Name}缺少默认的无参构造器"), e); }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                //仅支持可读写的属性
                if (!prop.CanRead || !prop.CanWrite)
                    continue;
                prop.SetValue(newObj, prop.GetValue(obj));
            }
            return newObj;
        }

        /// <summary>
        /// 获取类型的默认值，假如类型对象不为空且为值类型则构造一个实例，否则返回null
        /// </summary>
        /// <param name="type">给定的类型实体</param>
        /// <returns></returns>
        public static object CreateDefValue(this Type type)
        {
            return type != null && type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// 在命名空间中查找类
        /// </summary>
        /// <param name="assembly">程序集对象</param>
        /// <param name="nameSpace">命名空间全名（或一部分），区分大小写</param>
        /// <param name="subSpaceIncl">是否查找子命名空间</param>
        /// <param name="typeNameIncl">查找时限定类名的一部分，假如为空则不限定</param>
        /// <param name="baseType">查找类时限定的从中继承的类（仅检查类型名称及命名空间是否相同），假如为空则不限定</param>
        /// <returns></returns>
        public static Type[] GetTypesInNamespace(this Assembly assembly, string nameSpace, bool subSpaceIncl = false, string typeNameIncl = null, Type baseType = null)
        {
            return assembly.GetTypes().Where(type =>
            {
                bool
                //命名空间是否符合要求
                nameSpaceQual = !string.IsNullOrWhiteSpace(type.Namespace) && (subSpaceIncl ? type.Namespace.StartsWith(nameSpace, StringComparison.Ordinal) : type.Namespace.Equals(nameSpace, StringComparison.Ordinal)),
                //类名是否符合要求
                typeNameQual = string.IsNullOrWhiteSpace(typeNameIncl) || type.Name.Contains(typeNameIncl),
                baseTypeQual = baseType == null || (type.BaseType.Name.Equals(baseType.Name) && type.BaseType.Namespace.Equals(baseType.Namespace));
                return nameSpaceQual && typeNameQual && baseTypeQual;
            }).ToArray();
        }
    }
}
