using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function
{
    /// <summary>
    /// 反射功能类（程序集、类型等）
    /// </summary>
    public static class ReflectionUtil
    {
        /// <summary>
        /// System.Core程序集
        /// </summary>
        public static Assembly SystemCoreAssembly { get; } = GetSystemCoreAssembly();

        /// <summary>
        /// System.Linq.Enumerable的类型对象
        /// </summary>
        public static Type EnumerableType { get; } = typeof(Enumerable);

        /// <summary>
        /// System.Array的类型对象
        /// </summary>
        public static Type ArrayType { get; } = typeof(Array);

        /// <summary>
        /// System.Linq.Enumerable.ElementAt[T]的静态方法对象
        /// </summary>
        public static MethodInfo ElementAtMethod { get; } = EnumerableType.GetMethod("ElementAt");

        /// <summary>
        /// System.Array.SetValue的实体方法对象
        /// </summary>
        public static MethodInfo SetValueMethod { get; } = ArrayType.GetMethod("SetValue", new Type[] { typeof(object), typeof(int) });

        /// <summary>
        /// 获取System.Core程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly GetSystemCoreAssembly()
        {
            return Assembly.GetAssembly(new List<int>().ToLookup(i => i).GetType()); //从System.Core程序集中的System.Linq.Lookup类获取程序集
        }

        /// <summary>
        /// 获取泛型类中作为泛型参数的（第一种）类型，假如是数组的话则获取数组元素的类型，假如既不是泛型类也不是数组则返回空
        /// </summary>
        /// <param name="entityType">从中提取基本类型参数的泛类型</param>
        /// <returns></returns>
        public static Type GetGenericType(this Type entityType)
        {
            Type genericType = null;
            //var assemply = Assembly.GetExecutingAssembly();
            //assemply = Assembly.Load("IntercommConsole.OpcOnly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            //assemply = Assembly.GetEntryAssembly();
            if (entityType == null)
                goto END;
            Type[] genericTypes = entityType.GenericTypeArguments;
            //假如是具有泛型参数的泛型类
            if (genericTypes != null && genericTypes.Length > 0)
                genericType = genericTypes[0];
            //假如是数组
            else if (entityType.FullName.EndsWith("[]"))
            {
                string fullName = entityType.FullName.TrimEnd('[', ']');
                genericType = Type.GetType(fullName);
                //假如找不到数组基类型，则在入口可执行程序所在的程序集中查找
                if (genericType == null)
                    genericType = Assembly.GetEntryAssembly().GetType(fullName);
            }
            END:
            return genericType;
        }
    }
}
