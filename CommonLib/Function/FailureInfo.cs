using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Function;

namespace CommonLib.Function
{
    /// <summary>
    /// 异常信息方法类
    /// </summary>
    public class FailureInfo
    {
        ///// <summary>
        ///// 获取包含异常信息的字符串数组
        ///// </summary>
        ///// <param name="e">异常对象</param>
        ///// <returns>返回字符串数组，包括异常信息、方法以及堆栈信息等</returns>
        //public static string[] GetFailureInfoArray(Exception e)
        //{
        //    return GetFailureInfoArray(e, "出现异常");
        //}

        /// <summary>
        /// 获取包含异常信息与额外信息的字符串数组
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="info">错误说明信息</param>
        /// <param name="usingExcepMsg">错误说明信息中是否添加异常信息(string Exception.Message)</param>
        /// <param name="extraInfos">额外包含的信息字符串数组</param>
        /// <returns>返回字符串数组，包含异常信息、方法以及堆栈信息等</returns>
        public static string[] GetFailureInfoArray(Exception ex, string info = "出现异常", bool usingExcepMsg = true, IEnumerable<string> extraInfos = null)
        {
            List<string> list = new List<string>();
            list.Add(usingExcepMsg ? string.Format("{0}：{1}", info, ex.Message) : info); //是否需要异常信息
            list.Add(string.Format("出现异常的方法：{0}", ex.TargetSite.ToString()));
            list.Add(string.Format("方法所在的类：{0}", ex.TargetSite.ReflectedType.FullName)); //命名空间+类名
            list.Add(ex.ToString());
            if (extraInfos != null && extraInfos.Count() != 0)
                list.AddRange(extraInfos);

            return list.ToArray();
        }

        ///// <summary>
        ///// 获取包含异常信息的字符串数组
        ///// </summary>
        ///// <param name="e">异常对象</param>
        ///// <param name="info">错误说明信息</param>
        ///// <param name="usingExcepMsg">错误说明信息中是否添加异常信息(string Exception.Message)</param>
        ///// <returns>返回字符串数组，包含异常信息、方法以及堆栈信息等</returns>
        //public static string[] GetFailureInfoArray(Exception e, string info, bool usingExcepMsg)
        //{
        //    return GetFailureInfoArray(e, info, usingExcepMsg, null);
        //}

        /// <summary>
        /// 获取包含异常信息与额外信息的字符串数组
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="info">错误说明信息</param>
        /// <param name="extraInfos">额外包含的信息字符串数组</param>
        /// <returns>返回字符串数组，包含异常信息、方法以及堆栈信息等</returns>
        public static string[] GetFailureInfoArray(Exception e, string info, IEnumerable<string> extraInfos)
        {
            return GetFailureInfoArray(e, info, true, extraInfos);
        }

        /// <summary>
        /// 获取包含异常信息与额外信息的字符串数组
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="info">错误说明信息</param>
        /// <param name="extraInfo">额外包含的信息字符串</param>
        /// <returns>返回字符串数组，包含异常信息、方法以及堆栈信息等</returns>
        public static string[] GetFailureInfoArray(Exception e, string info, string extraInfo)
        {
            return GetFailureInfoArray(e, info, true, new string[] { extraInfo });
        }

        ///// <summary>
        ///// 获取包含错误说明+异常信息的字符串数组
        ///// </summary>
        ///// <param name="e">异常对象</param>
        ///// <param name="info">错误说明信息</param>
        ///// <returns>返回字符串数组，包含异常信息、方法以及堆栈信息等</returns>
        //public static string[] GetFailureInfoArray(Exception e, string info)
        //{
        //    return GetFailureInfoArray(e, info, true, null);
        //}
    }
}
