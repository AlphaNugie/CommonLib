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
using CommonLib.Helpers;

namespace CommonLib.Clients
{
    /// <summary>
    /// 文件写入操作类
    /// </summary>
    public class FileClient
    {
        #region static
        /// <summary>
        /// 储存日志文件的文件夹（或次级路径，如xx\xx等）
        /// </summary>
        public static string DefaultLogDir { get; set; } = "Logs";

        /// <summary>
        /// 错误日志目录
        /// </summary>
        public static string FailureLogDir { get { return DefaultLogDir + FileSystemHelper.DirSeparator + "Failure Logs"; } }

        /// <summary>
        /// 存放数据文件的目录(一般为XML文件)
        /// </summary>
        public static string DefaultDataDir { get; set; } = "Data";

        /// <summary>
        /// 默认文本文件类型后缀
        /// </summary>
        public const string TEXT_FILE_SUFFIX = ".txt";

        /// <summary>
        /// 默认日志文件类型后缀
        /// </summary>
        public const string LOG_FILE_SUFFIX = ".log";

        /// <summary>
        /// 文本分隔字符串
        /// </summary>
        public const string TEXT_SPLIT = "***********************************************************************";
        #endregion

        #region 私有成员
        private string path = string.Empty;
        private string fileName = string.Empty;
        //private string fileName_WithDate = string.Empty;
        private string filePath = string.Empty;
        //private string filePath_WithDate = string.Empty;
        #endregion

        #region 属性
        /// <summary>
        /// 文件路径（不包含文件名）
        /// </summary>
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                UpdateFilePath(path, FileName);
            }
        }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                UpdateFilePath(Path, fileName);
            }
        }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName_WithDate { get; set; }

        /// <summary>
        /// 文件完整路径（包含文件名）
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                Extension = System.IO.Path.GetExtension(filePath).ToLower(); //文件扩展名，小写
            }
        }

        /// <summary>
        /// 文件完整路径（包含文件名）
        /// </summary>
        public string FilePath_WithDate { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 每次写入是否对上一次覆盖
        /// </summary>
        public bool Overriding { get; set; }

        /// <summary>
        /// 上一个错误信息
        /// </summary>
        public string LastErrorMessage { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileName">文件名（包含后缀）</param>
        /// <param name="overriding">是否覆盖</param>
        public FileClient(string path, string fileName, bool overriding)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentException("待写入文件的路径为空！", "string path");
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentException("待写入文件的名称为空！", "string fileName");

                Path = path;
                FileName = fileName;
                //FilePath = Functions.TrimFilePath(Path) + Base.DirSeparator + FileName; //包含文件名的路径
                //Extension = System.IO.Path.GetExtension(FilePath).ToLower(); //文件扩展名，小写
                Overriding = overriding;
            }
            catch (Exception e)
            {
                LastErrorMessage = string.Format("文件{0}写入操作类初始化失败: {1}", fileName, e.Message);
                throw;
            }
        }

        /// <summary>
        /// 构造器，默认不覆盖
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileName">文件名（包含后缀）</param>
        public FileClient(string path, string fileName) : this(path, fileName, false) { }

        /// <summary>
        /// 更新文件完整路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName">文件名称</param>
        private void UpdateFilePath(string path, string fileName)
        {
            ////假如路径中不包含卷分隔符，添加根目录
            //if (!path.Contains(FileSystemHelper.VolumeSeparator))
            //    path = AppDomain.CurrentDomain.BaseDirectory + FileSystemHelper.TrimFilePath(path);
            //path = path;
            //FileName_WithDate = FileSystemHelper.AddDateToFileName(fileName); //带日期的文件名称
            //FilePath = FileSystemHelper.TrimFilePath(path) + FileSystemHelper.DirSeparator + fileName; //包含文件名的路径
            //FilePath_WithDate = FileSystemHelper.TrimFilePath(path) + FileSystemHelper.DirSeparator + FileName_WithDate; //带日期的路径
            FileSystemHelper.UpdateFilePath(ref path, fileName, out string fileNameDate, out string filePath, out string filePathDate);
            this.path = path;
            FileName_WithDate = fileNameDate; //带日期的文件名称
            FilePath = filePath; //包含文件名的路径
            FilePath_WithDate = filePathDate; //带日期的路径
        }

        /// <summary>
        /// 将文本写入文件
        /// </summary>
        /// <param name="lines">要添加进文件的字符串集合</param>
        /// <param name="fileNameWithDate">文件名是否带日期</param>
        public void WriteLinesToFile(IEnumerable<string> lines, bool fileNameWithDate = true)
        {
            if (lines == null || lines.Count() == 0)
                return;

            try
            {
                //检测目录是否存在，不存在则创建
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);

                UpdateFilePath(path, FileName);
                string filePath = fileNameWithDate ? FilePath_WithDate : FilePath;
                //假如文件存在，添加文本，否则创建文件并写入(编码方式为UTF-8)
                if (File.Exists(filePath) && !Overriding)
                    File.AppendAllLines(filePath, lines, Encoding.UTF8);
                else
                    File.WriteAllLines(filePath, lines, Encoding.UTF8);
            }
            catch (IOException) { }
        }

        ///// <summary>
        ///// 将文本写入文件，文件名默认带日期
        ///// </summary>
        ///// <param name="lines">要添加进文件的字符串集合</param>
        //public void WriteLinesToFile(IEnumerable<string> lines)
        //{
        //    WriteLinesToFile(lines, true);
        //}

        /// <summary>
        /// 将文本写入文件
        /// </summary>
        /// <param name="line">待写入文本</param>
        /// <param name="withDate">文件名是否带日期</param>
        public void WriteLineToFile(string line, bool withDate = true)
        {
            WriteLinesToFile(new string[] { line }, withDate);
        }

        ///// <summary>
        ///// 将文本写入文件，文件名默认带日期
        ///// </summary>
        ///// <param name="line">要添加进文件的字符串</param>
        //public void WriteLineToFile(string line)
        //{
        //    WriteLineToFile(line, true);
        //}

        #region old static methods
        /// <summary>
        /// 将文本写入文件
        /// </summary>
        /// <param name="lines">要添加进文件的字符串集合</param>
        /// <param name="path">文件路径（不包含文件名）</param>
        /// <param name="fileName">文件名（可包含文件类型后缀）</param>
        /// <param name="extension">文件扩展名</param>
        /// <param name="overriding">是否覆盖文本</param>
        public static void WriteLinesToFile(IEnumerable<string> lines, string path, string fileName, string extension = "", bool overriding = false)
        {
            if (lines == null || lines.Count() == 0)
                return;

            try
            {
                //if (lines == null)
                //    throw new ArgumentException("待写入文件的文本为空！", "string[] lines");
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentException("待写入文件的路径为空！", "string path");
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentException("待写入文件的名称为空！", "string fileName");

                //if (lines == null || lines.Count() == 0)
                //    return;

                string filePath = FileSystemHelper.TrimFilePath(path) + FileSystemHelper.DirSeparator + fileName; //包含文件名的路径

                //假如文件名不以.txt、.log或.xml结尾，则添加默认的文本文件后缀
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower(); //文件扩展名，小写
                if (!string.IsNullOrWhiteSpace(extension))
                    filePath += "." + extension;
                //else if (string.IsNullOrWhiteSpace(fileExtension))
                //    filePath += TEXT_FILE_SUFFIX;

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

        ///// <summary>
        ///// 将文本写入文件，默认不覆盖
        ///// </summary>
        ///// <param name="lines">要添加进文件的字符串集合</param>
        ///// <param name="path">文件路径（不包含文件名）</param>
        ///// <param name="fileName">文件名（可包含文件类型后缀）</param>
        ///// <param name="extension">文件扩展名</param>
        //public static void WriteLinesToFile(IEnumerable<string> lines, string path, string fileName, string extension)
        //{
        //    WriteLinesToFile(lines, path, fileName, extension, false);
        //}

        ///// <summary>
        ///// 将文本写入文件
        ///// </summary>
        ///// <param name="lines">要添加进文件的字符串数组</param>
        ///// <param name="path">文件路径（不包含文件名）</param>
        ///// <param name="fileName">文件名（可包含文件类型后缀）</param>
        //public static void WriteLinesToFile(IEnumerable<string> lines, string path, string fileName)
        //{
        //    WriteLinesToFile(lines, path, fileName, string.Empty);
        //}

        /// <summary>
        /// 将文本写入文件
        /// </summary>
        /// <param name="line">要添加进文件的字符串</param>
        /// <param name="path">文件路径（不包含文件名）</param>
        /// <param name="fileName">文件名（可包含文件类型后缀）</param>
        public static void WriteLinesToFile(string line, string path, string fileName)
        {
            WriteLinesToFile(new string[] { line }, path, fileName);
        }

        /// <summary>
        /// 将日志写到文件
        /// </summary>
        /// <param name="lines">待写入的字符串数组</param>
        /// <param name="subDir">日志目录下的子目录（假如为空或空字符串，则不创建）</param>
        /// <param name="fileName">文件名（不带文件后缀）</param>
        /// <param name="usingSplitter">添加日志时是否添加分隔符与日期时间记录</param>
        /// <param name="level">当前行在文本中的级别，0最高，每增加1级添加4个空格</param>
        public static void WriteLogsToFile(IEnumerable<string> lines, string subDir, string fileName, bool usingSplitter = true, int level = 0)
        {
            if (lines == null || lines.Count() == 0)
                return;

            try
            {
                //日志存储路径，保存在程序启动目录下特定皮带秤文件夹中，移除LogFolder首部以及尾部的"\"，假如有的话
                string path = AppDomain.CurrentDomain.BaseDirectory + FileSystemHelper.DirSeparator + FileSystemHelper.TrimFilePath(DefaultLogDir);
                path += string.IsNullOrWhiteSpace(subDir) ? string.Empty : (FileSystemHelper.DirSeparator + subDir);

                //某年某月某日 几时几分几秒 日期字符串
                Functions.GetDateTimeString(out string fullDate, out string localDateTime);
                localDateTime = string.IsNullOrWhiteSpace(localDateTime) ? (new DateTime()).ToString() : localDateTime;
                fileName += string.IsNullOrWhiteSpace(fullDate) ? string.Empty : " " + fullDate; //文件名中添加当前日期
                fileName += LOG_FILE_SUFFIX; //添加日志文件类型后缀

                //假如添加日志分隔符或添加空格（每一行文本级别不为0）
                if (usingSplitter || level != 0)
                {
                    var list = new List<string>();
                    //是否需要添加分隔符和时间记录
                    if (usingSplitter)
                    {
                        list.Add(Base.NewLine + TEXT_SPLIT + Base.NewLine);
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

                WriteLinesToFile(lines, path, fileName);
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
            WriteLogsToFile(lines, subDir, fileName, true, level);
        }

        ///// <summary>
        ///// 将日志写到文件
        ///// </summary>
        ///// <param name="lines">待写入的字符串数组</param>
        ///// <param name="subDir">日志目录下的子目录（假如为空或空字符串，则不创建）</param>
        ///// <param name="fileName">文件名（不带文件后缀）</param>
        ///// <param name="usingSplitter">添加日志时是否添加分隔符与日期时间记录</param>
        //public static void WriteLogsToFile(IEnumerable<string> lines, string subDir, string fileName, bool usingSplitter)
        //{
        //    WriteLogsToFile(lines, subDir, fileName, usingSplitter, 0);
        //}

        ///// <summary>
        ///// 将日志写到文件
        ///// </summary>
        ///// <param name="lines">待写入的字符串数组</param>
        ///// <param name="subDir">日志目录下的子目录（假如为空或空字符串，则不创建）</param>
        ///// <param name="fileName">文件名（不带文件后缀）</param>
        //public static void WriteLogsToFile(IEnumerable<string> lines, string subDir, string fileName)
        //{
        //    WriteLogsToFile(lines, subDir, fileName, true, 0);
        //}

        /// <summary>
        /// 将多行文本写到制定文件名的错误日志文件，不包含子目录
        /// </summary>
        /// <param name="lines">待写入的字符串数组</param>
        /// <param name="fileName">文件名（不带文件后缀）</param>
        public static void WriteLogsToFile(IEnumerable<string> lines, string fileName)
        {
            WriteLogsToFile(lines, string.Empty, fileName);
        }

        /// <summary>
        /// 将多行文本写入指定的错误日志文件
        /// </summary>
        /// <param name="lines">要添加进日志的字符串数组</param>
        public static void WriteFailureInfo(IEnumerable<string> lines)
        {
            string failureLogName = "FailureInfo"; //错误日志文件名称
            WriteFailureInfo(lines, failureLogName);
        }

        /// <summary>
        /// 将一行文本写入指定的错误日志文件
        /// </summary>
        /// <param name="line">要添加进日志的字符串</param>
        public static void WriteFailureInfo(string line)
        {
            WriteFailureInfo(new string[] { line });
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
            WriteFailureInfo(lines, string.Empty, fileName);
        }

        /// <summary>
        /// 将文本写入失败日志
        /// </summary>
        /// <param name="line">要添加进日志的字符串</param>
        /// <param name="subFolder">Failure Logs下的子文件夹名称</param>
        /// <param name="fileName">错误日志文件名称（不带文件类型后缀）</param>
        public static void WriteFailureInfo(string line, string subFolder, string fileName)
        {
            WriteFailureInfo(new string[] { line }, subFolder, fileName);
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
                subDir += System.IO.Path.DirectorySeparatorChar + subFolder;
            WriteLogsToFile(lines, subDir, fileName);
        }

        ///// <summary>
        ///// 将异常信息作为错误信息写入错误日志
        ///// </summary>
        ///// <param name="e">异常对象</param>
        ///// <param name="info">错误说明信息</param>
        ///// <param name="usingExcepMsg">错误说明信息中是否添加异常信息(string Exception.Message)</param>
        //public static void WriteExceptionInfo(Exception e, string info, bool usingExcepMsg)
        //{
        //    WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info, usingExcepMsg));
        //}

        /// <summary>
        /// 将异常信息作为错误信息写入错误日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="info">错误说明信息</param>
        /// <param name="usingExcepMsg">错误说明信息中是否添加异常信息(string Exception.Message)</param>
        /// <param name="extraInfos">额外包含的信息字符串数组</param>
        public static void WriteExceptionInfo(Exception e, string info = "出现异常", bool usingExcepMsg = true, string[] extraInfos = null)
        {
            WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info, usingExcepMsg, extraInfos));
        }

        /// <summary>
        /// 将异常信息作为错误信息写入错误日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="info">错误说明信息</param>
        /// <param name="extraInfos">额外包含的信息字符串数组</param>
        public static void WriteExceptionInfo(Exception e, string info, string[] extraInfos)
        {
            WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info, extraInfos));
        }

        /// <summary>
        /// 将异常信息作为错误信息写入错误日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="info">错误说明信息</param>
        /// <param name="extraInfo">额外包含的信息字符串</param>
        public static void WriteExceptionInfo(Exception e, string info, string extraInfo)
        {
            WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info, extraInfo));
        }

        ///// <summary>
        ///// 将异常信息作为错误信息写入错误日志
        ///// </summary>
        ///// <param name="e">异常对象</param>
        ///// <param name="info">异常说明信息</param>
        //public static void WriteExceptionInfo(Exception e, string info)
        //{
        //    WriteFailureInfo(FailureInfo.GetFailureInfoArray(e, info));
        //}

        ///// <summary>
        ///// 将异常的信息作为错误信息写入错误日志
        ///// </summary>
        ///// <param name="e">异常对象</param>
        //public static void WriteExceptionInfo(Exception e)
        //{
        //    WriteFailureInfo(FailureInfo.GetFailureInfoArray(e));
        //}

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
            string path = AppDomain.CurrentDomain.BaseDirectory + FileSystemHelper.DirSeparator + FileSystemHelper.TrimFilePath(DefaultDataDir);
            path += string.IsNullOrWhiteSpace(subDir) ? string.Empty : (FileSystemHelper.DirSeparator + subDir);
            string filePath = string.Format(@"{0}{1}{2}.xml", path, FileSystemHelper.DirSeparator, fileName); //包含文件名的路径

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
        #endregion
    }
}
