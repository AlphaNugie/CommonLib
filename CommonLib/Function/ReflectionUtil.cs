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
        /// System.Linq.Enumerable.ElementAt[T]的方法对象
        /// </summary>
        public static MethodInfo ElementAtMethod { get; } = EnumerableType.GetMethod("ElementAt");

        /// <summary>
        /// 获取System.Core程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly GetSystemCoreAssembly()
        {
            return Assembly.GetAssembly(new List<int>().ToLookup(i => i).GetType()); //从System.Core程序集中的System.Linq.Lookup类获取程序集
        }
    }
}
