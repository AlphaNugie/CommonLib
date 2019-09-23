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

namespace CommonLib.Clients.Object
{
    /// <summary>
    /// 文件写入操作类
    /// </summary>
    public class FileClient
    {
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
            get { return this.path; }
            set
            {
                this.path = value;
                this.UpdateFilePath(this.path, this.FileName);
                //this.UpdateFilePath(this.path, this.FileName_WithDate);
                //this.UpdateFilePath(this.path, this.FileName);
                //this.FilePath = Functions.TrimFilePath(this.path) + Base.DirSeparator + this.FileName; //包含文件名的路径
            }
        }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get { return this.fileName; }
            set
            {
                this.fileName = value;
                this.UpdateFilePath(this.Path, this.fileName);
                //this.FileName_WithDate = Functions.AddDateToFileName(this.fileName);
                //this.UpdateFilePath(this.Path, this.fileName);
                //this.FilePath = Functions.TrimFilePath(this.Path) + Base.DirSeparator + this.FileName; //包含文件名的路径
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
            get { return this.filePath; }
            set
            {
                this.filePath = value;
                this.Extension = System.IO.Path.GetExtension(this.filePath).ToLower(); //文件扩展名，小写
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

                this.Path = path;
                this.FileName = fileName;
                //this.FilePath = Functions.TrimFilePath(this.Path) + Base.DirSeparator + this.FileName; //包含文件名的路径
                //this.Extension = System.IO.Path.GetExtension(this.FilePath).ToLower(); //文件扩展名，小写
                this.Overriding = overriding;
            }
            catch (Exception e)
            {
                this.LastErrorMessage = string.Format("文件{0}写入操作类初始化失败: {1}", fileName, e.Message);
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
            //假如路径中不包含卷分隔符，添加根目录
            if (!path.Contains(Base.VolumeSeparator))
                path = AppDomain.CurrentDomain.BaseDirectory + Functions.TrimFilePath(path);
            this.path = path;
            this.FileName_WithDate = Functions.AddDateToFileName(fileName); //带日期的文件名称
            this.FilePath = Functions.TrimFilePath(path) + Base.DirSeparator + fileName; //包含文件名的路径
            this.FilePath_WithDate = Functions.TrimFilePath(path) + Base.DirSeparator + this.FileName_WithDate; //带日期的路径
        }

        /// <summary>
        /// 将文本写入文件
        /// </summary>
        /// <param name="lines">要添加进文件的字符串集合</param>
        /// <param name="withDate">文件名是否带日期</param>
        public void WriteLinesToFile(IEnumerable<string> lines, bool withDate)
        {
            try
            {
                if (lines == null || lines.Count() == 0)
                    return;

                //检测目录是否存在，不存在则创建
                if (!Directory.Exists(this.Path))
                    Directory.CreateDirectory(this.Path);

                //this.UpdateFilePath(this.Path, this.FileName);
                this.FileName_WithDate = Functions.AddDateToFileName(this.FileName); //带日期的文件名称
                string filePath = withDate ? this.FilePath_WithDate : this.FilePath;
                //假如文件存在，添加文本，否则创建文件并写入(编码方式为UTF-8)
                if (File.Exists(filePath) && !this.Overriding)
                    File.AppendAllLines(filePath, lines, Encoding.UTF8);
                else
                    File.WriteAllLines(filePath, lines, Encoding.UTF8);
            }
            catch (IOException) { }
        }

        /// <summary>
        /// 将文本写入文件，文件名默认带日期
        /// </summary>
        /// <param name="lines">要添加进文件的字符串集合</param>
        public void WriteLinesToFile(IEnumerable<string> lines)
        {
            this.WriteLinesToFile(lines, true);
        }

        /// <summary>
        /// 将文本写入文件
        /// </summary>
        /// <param name="line">待写入文本</param>
        /// <param name="withDate">文件名是否带日期</param>
        public void WriteLineToFile(string line, bool withDate)
        {
            this.WriteLinesToFile(new string[] { line }, withDate);
        }

        /// <summary>
        /// 将文本写入文件，文件名默认带日期
        /// </summary>
        /// <param name="line">要添加进文件的字符串</param>
        public void WriteLineToFile(string line)
        {
            this.WriteLineToFile(line, true);
        }

        ///// <summary>
        ///// 将数据表写入指定名称的XML文件
        ///// </summary>
        ///// <param name="dataTable">储存数据的数据表</param>
        ///// <param name="subDir">数据目录下的子文件夹（假如为空则不创建）</param>
        ///// <param name="fileName">XML文件名(不包含文件类型后缀)</param>
        //public static void WriteDataTableToXML(DataTable dataTable, string subDir, string fileName)
        //{
        //    if (dataTable == null || dataTable.Columns.Count == 0 || dataTable.Rows.Count == 0)
        //        throw new ArgumentException("待写入XML文件的数据表为空！", "DataTable dataTable");
        //    else if (string.IsNullOrWhiteSpace(fileName))
        //        throw new ArgumentException("待写入XML文件的名称为空！", "string fileName");

        //    //XML文件路径，保存在程序启动目录下，移除DataFolder首部以及尾部的“\”，根据子目录细分文件夹（假如有的话）
        //    string path = AppDomain.CurrentDomain.BaseDirectory + Base.DirSeparator + Functions.TrimFilePath(Base.DataDir);
        //    path += string.IsNullOrWhiteSpace(subDir) ? string.Empty : (Base.DirSeparator + subDir);
        //    string filePath = string.Format(@"{0}{1}{2}.xml", path, Base.DirSeparator, fileName); //包含文件名的路径

        //    //检测目录是否存在，不存在则创建
        //    if (!Directory.Exists(path))
        //        Directory.CreateDirectory(path);

        //    //假如XML文件存在，添加数据，否则创建并添加
        //    if (File.Exists(filePath))
        //    {
        //        StringWriter writer = new StringWriter();
        //        dataTable.WriteXml(writer); //将表的Xml写入Stream中
        //        XmlDocument docPrevLog = new XmlDocument(); //旧Xml
        //        XmlDocument docNewLog = new XmlDocument(); //新Xml
        //        docPrevLog.LoadXml(File.ReadAllText(filePath)); //从文件中获取XML文件文本
        //        docNewLog.LoadXml(writer.ToString()); //加载新的XML文本
        //        XmlNode root = docPrevLog.DocumentElement;//获取XML根节点
        //        //向XML根节点中添加新增子节点
        //        foreach (XmlNode node in docNewLog.DocumentElement.ChildNodes)
        //        {
        //            XmlNode tempNode = docPrevLog.ImportNode(node, true);
        //            root.AppendChild(tempNode);
        //        }
        //        docPrevLog.Save(filePath);//保存XML文件
        //    }
        //    else
        //        dataTable.WriteXml(filePath, XmlWriteMode.IgnoreSchema);
        //}
    }
}
