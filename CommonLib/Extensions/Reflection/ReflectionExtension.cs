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
#if NET45_OR_GREATER
        public static T Clone<T>(this T obj)
        {
            Type type = typeof(T);
            T newObj;
            //初始化一个目标类型的对象
            try { newObj = (T)Activator.CreateInstance(type); }
#elif NET9_0_OR_GREATER
        public static T? Clone<T>(this T obj)
        {
            Type type = typeof(T);
            T? newObj;
            //初始化一个目标类型的对象
            try { newObj = (T?)Activator.CreateInstance(type); }
#endif
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
        /// 获取类型的默认值，假如类型对象不为空则尝试构造一个实例，假如成功返回此实例，否则返回null
        /// </summary>
        /// <param name="type">给定的类型实体</param>
        /// <returns></returns>
#if NET45_OR_GREATER
        public static object CreateDefValue(this Type type)
        {
            ////调用Activator.CreateInstance方法时需要判断是否为值类型，否则假如引用类型没有默认构造器将会抛出异常
            //return type != null && type.IsValueType ? Activator.CreateInstance(type) : null;
            object result = null;
#elif NET9_0_OR_GREATER
        public static object? CreateDefValue(this Type type)
        {
            ////调用Activator.CreateInstance方法时需要判断是否为值类型，否则假如引用类型没有默认构造器将会抛出异常
            //return type != null && type.IsValueType ? Activator.CreateInstance(type) : null;
            object? result = null;
#endif
            if (type != null)
                try { result = Activator.CreateInstance(type); } catch { }
            return result;
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
        public static Type[] GetTypesInNamespace(this Assembly assembly, string nameSpace, bool subSpaceIncl = false,
#if NET45_OR_GREATER
string typeNameIncl = null, Type baseType = null)
#elif NET9_0_OR_GREATER
string? typeNameIncl = null, Type? baseType = null)
#endif
        {
            //临时方法，这样能够在需要执行的时候再计算
            //检查命名空间是否符合要求
            bool isNamespaceQual(Type type)
            {
                return !string.IsNullOrWhiteSpace(type.Namespace) &&
                       (subSpaceIncl ? type.Namespace.StartsWith(nameSpace, StringComparison.Ordinal) : type.Namespace.Equals(nameSpace, StringComparison.Ordinal));
            }
            //检查类名是否符合要求
            bool isTypeNameQual(Type type)
            {
                return string.IsNullOrWhiteSpace(typeNameIncl) || type.Name.Contains(typeNameIncl);
            }
            //检查是否继承自指定基类（仅检查基类类型名称及命名空间是否相同，假如基类为null则不限定）
            bool isBaseTypeQual(Type type)
            {
                return baseType == null || (type.BaseType != null && type.BaseType.Name.Equals(baseType.Name) && !string.IsNullOrWhiteSpace(type.BaseType.Namespace) && type.BaseType.Namespace.Equals(baseType.Namespace));
            }
            return assembly.GetTypes().Where(type =>
            {
                //bool
                ////命名空间是否符合要求
                //nameSpaceQual = !string.IsNullOrWhiteSpace(type.Namespace) && (subSpaceIncl ? type.Namespace.StartsWith(nameSpace, StringComparison.Ordinal) : type.Namespace.Equals(nameSpace, StringComparison.Ordinal)),
                ////类名是否符合要求
                //typeNameQual = string.IsNullOrWhiteSpace(typeNameIncl) || type.Name.Contains(typeNameIncl),
                ////是否继承自指定基类（仅检查基类类型名称及命名空间是否相同，假如基类为null则不限定）
                //baseTypeQual = baseType == null || (type.BaseType != null && type.BaseType.Name.Equals(baseType.Name) && type.BaseType.Namespace.Equals(baseType.Namespace));
                //return nameSpaceQual && typeNameQual && baseTypeQual;
                return isNamespaceQual(type) && isTypeNameQual(type) && isBaseTypeQual(type);
            }).ToArray();
        }
    }
}
