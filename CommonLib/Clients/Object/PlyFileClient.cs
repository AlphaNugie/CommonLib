using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients.Object
{
    /// <summary>
    /// .ply点云文件操作对象（只保存顶点vertex）
    /// </summary>
    public class PlyFileClient
    {
        private string version = "1.0";
        private string comment, path, filename, customed = string.Empty;
        private int vertext_count = 0;
        private string colored = @"
property uchar red
property uchar green
property uchar blue";
        private string header_format = @"ply
format ascii {0}
comment {1}
element vertex {2}
property float x
property float y
property float z{3}{4}
element face 0
property list uchar int vertex_indices
end_header
";
        private string header = string.Empty;

        /// <summary>
        /// 格式版本号，默认1.0
        /// </summary>
        public string FormatVersion
        {
            get { return this.version; }
            set { this.version = value; }
        }

        /// <summary>
        /// 文件注释
        /// </summary>
        public string Comment
        {
            get { return this.comment; }
            set { this.comment = value; }
        }

        /// <summary>
        /// 顶点数目
        /// </summary>
        public int VertextCount
        {
            get { return this.vertext_count; }
            set { this.vertext_count = value; }
        }

        /// <summary>
        /// 保存文件是否包含RGB颜色数据
        /// </summary>
        public bool Colored { get; set; }

        /// <summary>
        /// 定制化顶点头部信息
        /// </summary>
        public string CustomedHeaderInfo
        {
            get { return this.customed; }
            set { this.customed = value == null ? string.Empty : value.Trim(); }
        }

        /// <summary>
        /// 待保存文件的路径（完整或相对路径）
        /// </summary>
        public string Path
        {
            get { return this.path; }
            set
            {
                this.path = value;
                this.SetFullFilePath();
            }
        }

        /// <summary>
        /// 文件名（后缀默认为.ply）
        /// </summary>
        public string FileName
        {
            get { return this.filename; }
            set
            {
                this.filename = value;
                this.SetFullFilePath();
            }
        }

        /// <summary>
        /// 完整文件路径
        /// </summary>
        public string FullFilePath { get; private set; }

        /// <summary>
        /// 设置完整文件路径
        /// </summary>
        private void SetFullFilePath()
        {
            if (!this.Path.Contains(Base.VolumeSeparator))
                this.Path = Base.StartupPath + Functions.TrimFilePath(this.Path);
            this.FullFilePath = this.Path + Base.DirSeparator + this.FileName + ".ply";
        }

        /// <summary>
        /// 每次执行数据写入方法，是否覆盖原有文件内容
        /// </summary>
        public bool Overriding { get; private set; }

        /// <summary>
        /// 待保存的订点列表
        /// </summary>
        public List<PlyDotObject> DotList { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="version">格式版本</param>
        /// <param name="comment">注释</param>
        /// <param name="colored">是否保存RGB颜色</param>
        /// <param name="overriding">每次写入是否覆盖</param>
        public PlyFileClient(string version, string comment, bool colored, bool overriding)
        {
            this.FormatVersion = version;
            this.Comment = comment;
            this.Colored = colored;
            this.Overriding = overriding;
        }

        /// <summary>
        /// 构造器，格式版本默认为1.0
        /// </summary>
        /// <param name="colored">是否保存RGB颜色</param>
        /// <param name="overriding">每次写入是否覆盖</param>
        public PlyFileClient(bool colored, bool overriding) : this("1.0", string.Empty, colored, overriding) { }

        /// <summary>
        /// 构造器，格式版本默认为1.0，每次写入均覆盖
        /// </summary>
        /// <param name="colored">是否保存RGB颜色</param>
        public PlyFileClient(bool colored) : this("1.0", string.Empty, colored, true) { }

        /// <summary>
        /// 保存顶点数据到.PLY文件
        /// 1：文件名为空；2：顶点列表为空
        /// </summary>
        /// <returns></returns>
        public int SaveVertexes()
        {
            return this.SaveVertexes(this.DotList);
        }

        /// <summary>
        /// 保存顶点数据到.PLY文件
        /// 返回：0 成功；1 文件名为空；2 顶点列表为空
        /// </summary>
        /// <param name="dots">待保存字符串</param>
        /// <returns></returns>
        public int SaveVertexes(IEnumerable<string> dots)
        {
            if (string.IsNullOrWhiteSpace(this.FileName))
                return 1;
            if (dots == null || dots.Count() == 0)
                return 2;

            //if (!this.Path.Contains(Base.VolumeSeparator))
            //    this.Path = Base.StartupPath + Functions.TrimFilePath(this.Path);
            //this.FullFilePath = this.Path + Base.DirSeparator + this.FileName + ".ply";
            this.customed = ("\r\n" + this.customed.Trim('\r', '\n')).TrimEnd('\r', '\n'); //确保头部定制属性只有最前面一处换行（假如不为空）
            this.header = string.Format(this.header_format, this.FormatVersion, this.Comment, dots.Count(), this.Colored ? this.colored : string.Empty, this.customed);
            if (!Directory.Exists(this.Path))
                Directory.CreateDirectory(this.Path);
            File.WriteAllText(this.FullFilePath, this.header, Encoding.ASCII);
            //假如文件存在，添加文本，否则创建文件并写入(编码方式为ASCII)
            if (File.Exists(this.FullFilePath) && !this.Overriding)
                File.AppendAllLines(this.FullFilePath, dots, Encoding.ASCII);
            else
                File.WriteAllLines(this.FullFilePath, dots, Encoding.ASCII);

            return 0;
        }

        /// <summary>
        /// 保存顶点数据到.PLY文件
        /// 返回：0 成功；1 文件名为空；2 顶点列表为空
        /// </summary>
        /// <param name="dotlist">待保存对象列表</param>
        /// <returns></returns>
        public int SaveVertexes(IEnumerable<PlyDotObject> dotlist)
        {
            IEnumerable<string> dots = dotlist == null || dotlist.Count() == 0 ? null : dotlist.Select(dot => string.Format("{0} {1} {2} {3} {4} {5} {6}", Math.Round(dot.X), Math.Round(dot.Y), Math.Round(dot.Z), dot.Red, dot.Green, dot.Blue, dot.CustomedInfo));
            return this.SaveVertexes(dots);
        }
    }
}
