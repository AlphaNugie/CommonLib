using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Function;
using System.IO;

namespace CommonLib.Clients.Object
{
    /// <summary>
    /// 日志文件写入类
    /// </summary>
    public class LogClient : FileClient
    {
        #region 私有成员
        private string logDir { get; set; }
        private string subDir { get; set; }
        private bool dateAdded = false;
        #endregion

        #region 属性
        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string LogDir
        {
            get { return this.logDir; }
            set
            {
                this.logDir = value;
                this.UpdateLogDirFull(this.logDir, this.SubDir);
                //this.LogDirFull = this.logDir + (string.IsNullOrWhiteSpace(this.logDir) ? string.Empty : Base.DirSeparator + this.SubDir);
            }
        }

        /// <summary>
        /// 日志文件路径下的子路径（假如为空或空字符串，则不创建）
        /// </summary>
        public string SubDir
        {
            get { return this.subDir; }
            set
            {
                this.subDir = value;
                this.UpdateLogDirFull(this.LogDir, this.subDir);
            }
        }

        /// <summary>
        /// 完整日志文件路径
        /// </summary>
        public string LogDirFull { get; set; }

        /// <summary>
        /// 添加日志时是否添加分隔符与日期时间记录
        /// </summary>
        public bool UsingSplitter { get; set; }

        /// <summary>
        /// 是否在文件名中添加时间
        /// </summary>
        public bool AddDate { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="logDir">日志目录（假如不带盘符则为程序根目录下路径）</param>
        /// <param name="subDir">日志目录下的子目录（假如为空或空字符串，则不创建）</param>
        /// <param name="fileName">文件名</param>
        /// <param name="usingSplitter">添加日志时是否添加分隔符与日期时间记录</param>
        /// <param name="addDate">文件名中是否添加日期</param>
        public LogClient(string logDir, string subDir, string fileName, bool usingSplitter, bool addDate) : base(logDir + (string.IsNullOrWhiteSpace(subDir) ? string.Empty : Base.DirSeparator + subDir), fileName, false)
        {
            this.LogDir = logDir;
            this.SubDir = subDir;
            //this.LogDirFull = this.LogDir + (string.IsNullOrWhiteSpace(this.LogDir) ? string.Empty : Base.DirSeparator + this.SubDir);
            this.UsingSplitter = usingSplitter;
            this.AddDate = addDate;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="logDir">日志目录（假如不带盘符则为程序根目录下路径）</param>
        /// <param name="subDir">日志目录下的子目录（假如为空或空字符串，则不创建）</param>
        /// <param name="fileName">文件名</param>
        public LogClient(string logDir, string subDir, string fileName) : this(logDir, subDir, fileName, true, true) { }

        /// <summary>
        /// 更新日志文件完整路径
        /// </summary>
        /// <param name="logDir"></param>
        /// <param name="subDir"></param>
        private void UpdateLogDirFull(string logDir, string subDir)
        {
            //日志存储路径，保存在程序启动目录下特定皮带秤文件夹中，移除LogFolder首部以及尾部的"\"，假如有的话
            if (!logDir.Contains(Base.VolumeSeparator) && !string.IsNullOrWhiteSpace(logDir))
                logDir = AppDomain.CurrentDomain.BaseDirectory + Base.DirSeparator + Functions.TrimFilePath(logDir);
            this.LogDirFull = logDir + (string.IsNullOrWhiteSpace(logDir) ? string.Empty : Base.DirSeparator + subDir);
            this.Path = this.LogDirFull;
        }

        /// <summary>
        /// 将日志写到文件
        /// </summary>
        /// <param name="lines">待写入的字符串数组</param>
        /// <param name="level">当前行在文本中的级别，0最高，每增加1级添加4个空格</param>
        public void WriteLogsToFile(IEnumerable<string> lines, int level)
        {
            try
            {
                string localDateTime = string.Format("{0:yyyy年M月d日 H时m分s秒}", DateTime.Now); //本地格式的日期时间，精确到秒
                //if (!this.dateAdded)
                //{
                //    this.FileName = Functions.AddDateToFileName(this.FileName);
                //    this.dateAdded = true;
                //}

                //假如添加日志分隔符或添加空格（每一行文本级别不为0）
                if (this.UsingSplitter || level != 0)
                {
                    var list = new List<string>();
                    //是否需要添加分隔符和时间记录
                    if (this.UsingSplitter)
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

                this.WriteLinesToFile(lines);
            }
            catch (IOException) { }
        }

        /// <summary>
        /// 将日志写到文件
        /// </summary>
        /// <param name="lines">待写入的字符串数组</param>
        public void WriteLogsToFile(IEnumerable<string> lines)
        {
            this.WriteLogsToFile(lines, 0);
        }

        /// <summary>
        /// 将日志写到文件
        /// </summary>
        /// <param name="line"></param>
        public void WriteLogsToFile(string line)
        {
            this.WriteLogsToFile(new string[] { line }, 0);
        }
    }
}
