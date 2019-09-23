//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CommonLib.Clients.Object
//{
//    /// <summary>
//    /// 错误日志写入对象
//    /// </summary>
//    public class FailureLogClient : LogClient
//    {
//        public FailureLogClient()
//        {

//        }

//        /// <summary>
//        /// 将多行文本写入指定的错误日志文件
//        /// </summary>
//        /// <param name="lines">要添加进日志的字符串数组</param>
//        public static void WriteFailureInfo(IEnumerable<string> lines)
//        {
//            string failureLogName = "FailureInfo"; //错误日志文件名称
//            FileClient.WriteFailureInfo(lines, failureLogName);
//        }

//        /// <summary>
//        /// 将一行文本写入指定的错误日志文件
//        /// </summary>
//        /// <param name="line">要添加进日志的字符串</param>
//        public static void WriteFailureInfo(string line)
//        {
//            FileClient.WriteFailureInfo(new string[] { line });
//        }

//        /// <summary>
//        /// 将文本写入失败日志
//        /// </summary>
//        /// <param name="lines">要添加进日志的字符串数组</param>
//        /// <param name="fileName">错误日志文件名称（不带文件类型后缀）</param>
//        public static void WriteFailureInfo(IEnumerable<string> lines, string fileName)
//        {
//            //string subDir = "Failure Logs"; //错误日志所在子目录
//            //FileClient.WriteLogsToFile(lines, subDir, fileName);
//            FileClient.WriteFailureInfo(lines, string.Empty, fileName);
//        }

//        /// <summary>
//        /// 将文本写入失败日志
//        /// </summary>
//        /// <param name="line">要添加进日志的字符串</param>
//        /// <param name="subFolder">Failure Logs下的子文件夹名称</param>
//        /// <param name="fileName">错误日志文件名称（不带文件类型后缀）</param>
//        public static void WriteFailureInfo(string line, string subFolder, string fileName)
//        {
//            FileClient.WriteFailureInfo(new string[] { line }, subFolder, fileName);
//        }

//        /// <summary>
//        /// 将文本写入失败日志
//        /// </summary>
//        /// <param name="lines">要添加进日志的字符串数组</param>
//        /// <param name="subFolder">Failure Logs下的子文件夹名称</param>
//        /// <param name="fileName">错误日志文件名称（不带文件类型后缀）</param>
//        public static void WriteFailureInfo(IEnumerable<string> lines, string subFolder, string fileName)
//        {
//            string subDir = "Failure Logs"; //错误日志所在子目录
//            if (!string.IsNullOrWhiteSpace(subFolder))
//                subDir += System.IO.Path.DirectorySeparatorChar + subFolder;
//            FileClient.WriteLogsToFile(lines, subDir, fileName);
//        }

//        /// <summary>
//        /// 将异常信息作为错误信息写入错误日志
//        /// </summary>
//        /// <param name="e">异常对象</param>
//        /// <param name="info">错误说明信息</param>
//        /// <param name="usingExcepMsg">错误说明信息中是否添加异常信息(string Exception.Message)</param>
//        public static void WriteExceptionInfo(Exception e, string info, bool usingExcepMsg)
//        {
//            FileClient.WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info, usingExcepMsg));
//        }

//        /// <summary>
//        /// 将异常信息作为错误信息写入错误日志
//        /// </summary>
//        /// <param name="e">异常对象</param>
//        /// <param name="info">错误说明信息</param>
//        /// <param name="extraInfos">额外包含的信息字符串数组</param>
//        public static void WriteExceptionInfo(Exception e, string info, string[] extraInfos)
//        {
//            FileClient.WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info, extraInfos));
//        }

//        /// <summary>
//        /// 将异常信息作为错误信息写入错误日志
//        /// </summary>
//        /// <param name="e">异常对象</param>
//        /// <param name="info">错误说明信息</param>
//        /// <param name="extraInfo">额外包含的信息字符串</param>
//        public static void WriteExceptionInfo(Exception e, string info, string extraInfo)
//        {
//            FileClient.WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info, extraInfo));
//        }

//        /// <summary>
//        /// 将异常信息作为错误信息写入错误日志
//        /// </summary>
//        /// <param name="e">异常对象</param>
//        /// <param name="info">异常说明信息</param>
//        public static void WriteExceptionInfo(Exception e, string info)
//        {
//            FileClient.WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info));
//        }

//        /// <summary>
//        /// 将异常的信息作为错误信息写入错误日志
//        /// </summary>
//        /// <param name="e">异常对象</param>
//        public static void WriteExceptionInfo(Exception e)
//        {
//            FileClient.WriteFailureInfo(FailureInfo.GetFailureInfoArray(e));
//        }
//    }
//}
