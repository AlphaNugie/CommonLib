using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using CommonLib.Function;

namespace CommonLib.Clients
{
    /// <summary>
    /// 文件写入操作类
    /// </summary>
    public class FileClient
    {
        /// <summary>
        /// 将文本写入文件
        /// </summary>
        /// <param name="lines">要添加进文件的字符串集合</param>
        /// <param name="path">文件路径（不包含文件名）</param>
        /// <param name="fileName">文件名（可包含文件类型后缀）</param>
        /// <param name="extension">文件扩展名</param>
        /// <param name="overriding">是否覆盖文本</param>
        public static void WriteLinesToFile(IEnumerable<string> lines, string path, string fileName, string extension, bool overriding)
        {
            try
            {
                //if (lines == null)
                //    throw new ArgumentException("待写入文件的文本为空！", "string[] lines");
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentException("待写入文件的路径为空！", "string path");
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentException("待写入文件的名称为空！", "string fileName");

                if (lines == null || lines.Count() == 0)
                    return;

                string filePath = Functions.TrimFilePath(path) + Base.DirSeparator + fileName; //包含文件名的路径

                //假如文件名不以.txt、.log或.xml结尾，则添加默认的文本文件后缀
                string fileExtension = Path.GetExtension(filePath).ToLower(); //文件扩展名，小写
                if (!string.IsNullOrWhiteSpace(extension))
                    filePath += "." + extension;
                else if (string.IsNullOrWhiteSpace(fileExtension))
                    filePath += Base.TextFileSuffix;

                //检测目录是否存在，不存在则创建
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //假如文件存在，添加文本，否则创建文件并写入(编码方式为UTF-8)
                if (File.Exists(filePath) && !overriding)
                    File.AppendAllLines(filePath, lines, Encoding.UTF8);
                else
                    File.WriteAllLines(filePath, lines, Encoding.UTF8);
            }
            catch (IOException) { }
        }
        
        /// <summary>
        /// 将文本写入文件，默认不覆盖
        /// </summary>
        /// <param name="lines">要添加进文件的字符串集合</param>
        /// <param name="path">文件路径（不包含文件名）</param>
        /// <param name="fileName">文件名（可包含文件类型后缀）</param>
        /// <param name="extension">文件扩展名</param>
        public static void WriteLinesToFile(IEnumerable<string> lines, string path, string fileName, string extension)
        {
            FileClient.WriteLinesToFile(lines, path, fileName, extension, false);
        }

        /// <summary>
        /// 将文本写入文件
        /// </summary>
        /// <param name="lines">要添加进文件的字符串数组</param>
        /// <param name="path">文件路径（不包含文件名）</param>
        /// <param name="fileName">文件名（可包含文件类型后缀）</param>
        public static void WriteLinesToFile(IEnumerable<string> lines, string path, string fileName)
        {
            FileClient.WriteLinesToFile(lines, path, fileName, string.Empty);
        }

        /// <summary>
        /// 将文本写入文件
        /// </summary>
        /// <param name="line">要添加进文件的字符串</param>
        /// <param name="path">文件路径（不包含文件名）</param>
        /// <param name="fileName">文件名（可包含文件类型后缀）</param>
        public static void WriteLinesToFile(string line, string path, string fileName)
        {
            FileClient.WriteLinesToFile(new string[] { line }, path, fileName);
        }

        /// <summary>
        /// 将日志写到文件
        /// </summary>
        /// <param name="lines">待写入的字符串数组</param>
        /// <param name="subDir">日志目录下的子目录（假如为空或空字符串，则不创建）</param>
        /// <param name="fileName">文件名（不带文件后缀）</param>
        /// <param name="usingSplitter">添加日志时是否添加分隔符与日期时间记录</param>
        /// <param name="level">当前行在文本中的级别，0最高，每增加1级添加4个空格</param>
        public static void WriteLogsToFile(IEnumerable<string> lines, string subDir, string fileName, bool usingSplitter, int level)
        {
            try
            {
                //日志存储路径，保存在程序启动目录下特定皮带秤文件夹中，移除LogFolder首部以及尾部的"\"，假如有的话
                string path = AppDomain.CurrentDomain.BaseDirectory + Base.DirSeparator + Functions.TrimFilePath(Base.LogDir);
                path += string.IsNullOrWhiteSpace(subDir) ? string.Empty : (Base.DirSeparator + subDir);

                string fullDate,      //yyyyMMdd 日期字符串
                       localDateTime; //某年某月某日 几时几分几秒 日期字符串
                Functions.GetDateTimeString(out fullDate, out localDateTime);
                localDateTime = string.IsNullOrWhiteSpace(localDateTime) ? (new DateTime()).ToString() : localDateTime;
                fileName += string.IsNullOrWhiteSpace(fullDate) ? string.Empty : " " + fullDate; //文件名中添加当前日期
                fileName += Base.LogFileSuffix; //添加日志文件类型后缀

                //假如添加日志分隔符或添加空格（每一行文本级别不为0）
                if (usingSplitter || level != 0)
                {
                    var list = new List<string>();
                    //是否需要添加分隔符和时间记录
                    if (usingSplitter)
                    {
                        list.Add(Base.NewLine + Base.TextSplit + Base.NewLine);
                        list.Add(localDateTime + "：" + Base.NewLine); //待写入的文本中添加日期时间
                    }
                    //添加原有文本行
                    foreach (var line in lines)
                    {
                        string prefix = new string(' ', level * 4); //根据每一行的级别添加空格
                        list.Add(prefix + line);
                    }

                    lines = list.ToArray();
                }

                FileClient.WriteLinesToFile(lines, path, fileName);
            }
            catch (IOException) { }
        }

        /// <summary>
        /// 将日志写到文件
        /// </summary>
        /// <param name="lines">待写入的字符串数组</param>
        /// <param name="subDir">日志目录下的子目录（假如为空或空字符串，则不创建）</param>
        /// <param name="fileName">文件名（不带文件后缀）</param>
        /// <param name="level">当前行在文本中的级别，0最高，每增加1级添加4个空格</param>
        public static void WriteLogsToFile(IEnumerable<string> lines, string subDir, string fileName, int level)
        {
            FileClient.WriteLogsToFile(lines, subDir, fileName, true, level);
        }

        /// <summary>
        /// 将日志写到文件
        /// </summary>
        /// <param name="lines">待写入的字符串数组</param>
        /// <param name="subDir">日志目录下的子目录（假如为空或空字符串，则不创建）</param>
        /// <param name="fileName">文件名（不带文件后缀）</param>
        /// <param name="usingSplitter">添加日志时是否添加分隔符与日期时间记录</param>
        public static void WriteLogsToFile(IEnumerable<string> lines, string subDir, string fileName, bool usingSplitter)
        {
            FileClient.WriteLogsToFile(lines, subDir, fileName, usingSplitter, 0);
        }

        /// <summary>
        /// 将日志写到文件
        /// </summary>
        /// <param name="lines">待写入的字符串数组</param>
        /// <param name="subDir">日志目录下的子目录（假如为空或空字符串，则不创建）</param>
        /// <param name="fileName">文件名（不带文件后缀）</param>
        public static void WriteLogsToFile(IEnumerable<string> lines, string subDir, string fileName)
        {
            FileClient.WriteLogsToFile(lines, subDir, fileName, true, 0);
        }

        /// <summary>
        /// 将多行文本写到制定文件名的错误日志文件，不包含子目录
        /// </summary>
        /// <param name="lines">待写入的字符串数组</param>
        /// <param name="fileName">文件名（不带文件后缀）</param>
        public static void WriteLogsToFile(IEnumerable<string> lines, string fileName)
        {
            FileClient.WriteLogsToFile(lines, string.Empty, fileName);
        }

        /// <summary>
        /// 将多行文本写入指定的错误日志文件
        /// </summary>
        /// <param name="lines">要添加进日志的字符串数组</param>
        public static void WriteFailureInfo(IEnumerable<string> lines)
        {
            string failureLogName = "FailureInfo"; //错误日志文件名称
            FileClient.WriteFailureInfo(lines, failureLogName);
        }

        /// <summary>
        /// 将一行文本写入指定的错误日志文件
        /// </summary>
        /// <param name="line">要添加进日志的字符串</param>
        public static void WriteFailureInfo(string line)
        {
            FileClient.WriteFailureInfo(new string[] { line });
        }

        /// <summary>
        /// 将文本写入失败日志
        /// </summary>
        /// <param name="lines">要添加进日志的字符串数组</param>
        /// <param name="fileName">错误日志文件名称（不带文件类型后缀）</param>
        public static void WriteFailureInfo(IEnumerable<string> lines, string fileName)
        {
            //string subDir = "Failure Logs"; //错误日志所在子目录
            //FileClient.WriteLogsToFile(lines, subDir, fileName);
            FileClient.WriteFailureInfo(lines, string.Empty, fileName);
        }

        /// <summary>
        /// 将文本写入失败日志
        /// </summary>
        /// <param name="line">要添加进日志的字符串</param>
        /// <param name="subFolder">Failure Logs下的子文件夹名称</param>
        /// <param name="fileName">错误日志文件名称（不带文件类型后缀）</param>
        public static void WriteFailureInfo(string line, string subFolder, string fileName)
        {
            FileClient.WriteFailureInfo(new string[] { line }, subFolder, fileName);
        }

        /// <summary>
        /// 将文本写入失败日志
        /// </summary>
        /// <param name="lines">要添加进日志的字符串数组</param>
        /// <param name="subFolder">Failure Logs下的子文件夹名称</param>
        /// <param name="fileName">错误日志文件名称（不带文件类型后缀）</param>
        public static void WriteFailureInfo(IEnumerable<string> lines, string subFolder, string fileName)
        {
            string subDir = "Failure Logs"; //错误日志所在子目录
            if (!string.IsNullOrWhiteSpace(subFolder))
                subDir += Path.DirectorySeparatorChar + subFolder;
            FileClient.WriteLogsToFile(lines, subDir, fileName);
        }

        /// <summary>
        /// 将异常信息作为错误信息写入错误日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="info">错误说明信息</param>
        /// <param name="usingExcepMsg">错误说明信息中是否添加异常信息(string Exception.Message)</param>
        public static void WriteExceptionInfo(Exception e, string info, bool usingExcepMsg)
        {
            FileClient.WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info, usingExcepMsg));
        }

        /// <summary>
        /// 将异常信息作为错误信息写入错误日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="info">错误说明信息</param>
        /// <param name="extraInfos">额外包含的信息字符串数组</param>
        public static void WriteExceptionInfo(Exception e, string info, string[] extraInfos)
        {
            FileClient.WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info, extraInfos));
        }

        /// <summary>
        /// 将异常信息作为错误信息写入错误日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="info">错误说明信息</param>
        /// <param name="extraInfo">额外包含的信息字符串</param>
        public static void WriteExceptionInfo(Exception e, string info, string extraInfo)
        {
            FileClient.WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info, extraInfo));
        }

        /// <summary>
        /// 将异常信息作为错误信息写入错误日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="info">异常说明信息</param>
        public static void WriteExceptionInfo(Exception e, string info)
        {
            FileClient.WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info));
        }

        /// <summary>
        /// 将异常的信息作为错误信息写入错误日志
        /// </summary>
        /// <param name="e">异常对象</param>
        public static void WriteExceptionInfo(Exception e)
        {
            FileClient.WriteFailureInfo(FailureInfo.GetFailureInfoArray(e));
        }

        /// <summary>
        /// 将数据表写入指定名称的XML文件
        /// </summary>
        /// <param name="dataTable">储存数据的数据表</param>
        /// <param name="subDir">数据目录下的子文件夹（假如为空则不创建）</param>
        /// <param name="fileName">XML文件名(不包含文件类型后缀)</param>
        public static void WriteDataTableToXML(DataTable dataTable, string subDir, string fileName)
        {
            if (dataTable == null || dataTable.Columns.Count == 0 || dataTable.Rows.Count == 0)
                throw new ArgumentException("待写入XML文件的数据表为空！", "DataTable dataTable");
            else if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("待写入XML文件的名称为空！", "string fileName");

            //XML文件路径，保存在程序启动目录下，移除DataFolder首部以及尾部的“\”，根据子目录细分文件夹（假如有的话）
            string path = AppDomain.CurrentDomain.BaseDirectory + Base.DirSeparator + Functions.TrimFilePath(Base.DataDir);
            path += string.IsNullOrWhiteSpace(subDir) ? string.Empty : (Base.DirSeparator + subDir);
            string filePath = string.Format(@"{0}{1}{2}.xml", path, Base.DirSeparator, fileName); //包含文件名的路径

            //检测目录是否存在，不存在则创建
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //假如XML文件存在，添加数据，否则创建并添加
            if (File.Exists(filePath))
            {
                StringWriter writer = new StringWriter();
                dataTable.WriteXml(writer); //将表的Xml写入Stream中
                XmlDocument docPrevLog = new XmlDocument(); //旧Xml
                XmlDocument docNewLog = new XmlDocument(); //新Xml
                docPrevLog.LoadXml(File.ReadAllText(filePath)); //从文件中获取XML文件文本
                docNewLog.LoadXml(writer.ToString()); //加载新的XML文本
                XmlNode root = docPrevLog.DocumentElement;//获取XML根节点
                //向XML根节点中添加新增子节点
                foreach (XmlNode node in docNewLog.DocumentElement.ChildNodes)
                {
                    XmlNode tempNode = docPrevLog.ImportNode(node, true);
                    root.AppendChild(tempNode);
                }
                docPrevLog.Save(filePath);//保存XML文件
            }
            else
                dataTable.WriteXml(filePath, XmlWriteMode.IgnoreSchema);
        }
    }
}
