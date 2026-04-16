//using CommonLib.Function;
using CommonLib.Helpers;
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
        private string comment = string.Empty, path = string.Empty, filename = string.Empty;
        private int vertext_count = 0;
        //颜色属性模板
        private readonly string colored = @"
property uchar red
property uchar green
property uchar blue";
        //头部信息模板
        private readonly string header_format = @"ply
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
        //完整的头部信息
        private string _header = string.Empty;
        // 已注册的自定义属性列表
#if NET45
        private readonly List<CustomProperty> _regCustomProperties = new List<CustomProperty>();
#elif NET9_0_OR_GREATER
        private readonly List<CustomProperty> _regCustomProperties = [];
#endif

        /// <summary>
        /// 格式版本号，默认1.0
        /// </summary>
        public string FormatVersion
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// 文件注释
        /// </summary>
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        /// <summary>
        /// 顶点数目
        /// </summary>
        public int VertextCount
        {
            get { return vertext_count; }
            set { vertext_count = value; }
        }

        /// <summary>
        /// 保存文件是否包含RGB颜色数据
        /// </summary>
        public bool Colored { get; set; }

        /// <summary>
        /// 待保存文件的路径（完整或相对路径）
        /// </summary>
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                SetFullFilePath();
            }
        }

        /// <summary>
        /// 文件名（后缀默认为.ply）
        /// </summary>
        public string FileName
        {
            get { return filename; }
            set
            {
                filename = value;
                SetFullFilePath();
            }
        }

        /// <summary>
        /// 完整文件路径
        /// </summary>
        public string FullFilePath { get; private set; } = string.Empty;

        /// <summary>
        /// 设置完整文件路径
        /// </summary>
        private void SetFullFilePath()
        {
            if (!Path.Contains(FileSystemHelper.VolumeSeparator))
                Path = FileSystemHelper.StartupPath + FileSystemHelper.TrimFilePath(Path);
            FullFilePath = Path + FileSystemHelper.DirSeparator + FileName + ".ply";
        }

        /// <summary>
        /// 待保存的订点列表
        /// </summary>
#if NET45
        public List<PlyDotObject> DotList { get; set; } = new List<PlyDotObject>();
#elif NET9_0_OR_GREATER
        public List<PlyDotObject> DotList { get; set; } = [];
#endif

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="version">格式版本</param>
        /// <param name="comment">注释</param>
        /// <param name="colored">是否保存RGB颜色</param>
        ///// <param name="overriding">每次写入是否覆盖</param>
        public PlyFileClient(string version, string comment, bool colored/*, bool overriding*/)
        {
            FormatVersion = version;
            Comment = comment;
            Colored = colored;
            //Overriding = overriding;
        }

        ///// <summary>
        ///// 构造器，格式版本默认为1.0
        ///// </summary>
        ///// <param name="colored">是否保存RGB颜色</param>
        ///// <param name="overriding">每次写入是否覆盖</param>
        //public PlyFileClient(bool colored, bool overriding) : this("1.0", string.Empty, colored, overriding) { }

        /// <summary>
        /// 构造器，格式版本默认为1.0，每次写入均覆盖
        /// </summary>
        /// <param name="colored">是否保存RGB颜色</param>
        public PlyFileClient(bool colored) : this("1.0", string.Empty, colored/*, true*/) { }

        #region 自定义属性注册
        /// <summary>
        /// 注册自定义属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dataType"></param>
        public void RegisterCustomProperty(string name, Type dataType)
        {
            _regCustomProperties.Add(new CustomProperty
            {
                Name = name?.Trim(),
                DataType = dataType ?? typeof(float)
            });
        }

        /// <summary>
        /// 生成头部信息中的自定义属性部分
        /// </summary>
        /// <returns></returns>
        private string GenerateCustomPropertyHeader()
        {
            if (_regCustomProperties.Count == 0) return string.Empty;

            var sb = new StringBuilder();
            foreach (var prop in _regCustomProperties)
            {
                var typeName = GetPlyTypeName(prop.DataType);
                sb.Append($"\nproperty {typeName} {prop.Name}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 类型映射方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
#if NET45
        private static string GetPlyTypeName(Type type)
#elif NET9_0_OR_GREATER
        private static string GetPlyTypeName(Type? type)
#endif
        {
            if (type == typeof(byte)) return "uchar";
            if (type == typeof(sbyte)) return "char";
            if (type == typeof(short)) return "short";
            if (type == typeof(ushort)) return "ushort";
            if (type == typeof(int)) return "int";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            return "float"; // 默认类型
        }
        #endregion

        /// <summary>
        /// 保存顶点数据到.PLY文件
        /// 1：文件名为空；2：顶点列表为空
        /// </summary>
        /// <returns></returns>
        public int SaveVertexes()
        {
            return SaveVertexes(DotList);
        }

        /// <summary>
        /// 保存顶点数据到.PLY文件
        /// 返回：0 成功；1 文件名为空；2 顶点列表为空
        /// </summary>
        /// <param name="dots">待保存字符串</param>
        /// <returns></returns>
#if NET45
        public int SaveVertexes(IEnumerable<string> dots)
#elif NET9_0_OR_GREATER
        public int SaveVertexes(IEnumerable<string>? dots)
#endif
        {
            if (string.IsNullOrWhiteSpace(FileName))
                return 1;
            if (dots == null || !dots.Any())
#if NET45
            { dots = new List<string>(); }
            var list = dots == null ? new List<string>() : dots.ToList();
#elif NET9_0_OR_GREATER
            { dots = []; }
            var list = dots == null ? [] : dots.ToList();
#endif

            //customed = ("\r\n" + customed.Trim('\r', '\n')).TrimEnd('\r', '\n'); //确保头部定制属性只有最前面一处换行（假如不为空）
            _header = string.Format(header_format, FormatVersion, Comment, list.Count, Colored ? colored : string.Empty, /*customed*/GenerateCustomPropertyHeader());
            FileSystemHelper.CheckForDirectory(path);
            //头部信息中的vertex数量必须与下面列出的行数一致，因此每次都重新覆盖写入，否则假如对不上会报出“Unespected eof”的错误
#if NET45
            List<string> lines = new List<string>() { _header };
#elif NET9_0_OR_GREATER
            List<string> lines = new() { _header };
#endif
            lines.AddRange(list);
            File.WriteAllLines(FullFilePath, lines, Encoding.ASCII);
            //File.WriteAllText(FullFilePath, header, Encoding.ASCII);
            //File.AppendAllLines(FullFilePath, dots, Encoding.ASCII);
            //////假如文件存在，添加文本，否则创建文件并写入(编码方式为ASCII)
            ////if (File.Exists(FullFilePath) && !Overriding)
            ////    File.AppendAllLines(FullFilePath, dots, Encoding.ASCII);
            ////else
            ////    File.WriteAllLines(FullFilePath, dots, Encoding.ASCII);

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
            //IEnumerable<string> dots = dotlist == null || dotlist.Count() == 0 ? null : dotlist.Select(dot => string.Format("{0} {1} {2} {3} {4} {5} {6}", Math.Round(dot.X), Math.Round(dot.Y), Math.Round(dot.Z), dot.Red, dot.Green, dot.Blue, dot.CustomedInfo));
#if NET45
            IEnumerable<string> dots = dotlist?.Select(dot =>
            {
                var baseInfo = $"{Math.Round(dot.X)} {Math.Round(dot.Y)} {Math.Round(dot.Z)}";
                var colorInfo = Colored ? $" {dot.Red} {dot.Green} {dot.Blue}" : "";

                // 生成自定义属性值
                var customValues = string.Join(" ", dot.CustomProperties
                    .Select((v, i) => FormatCustomValue(v, _regCustomProperties[i].DataType)));

                return $"{baseInfo}{colorInfo} {customValues}".TrimEnd();
            });
#elif NET9_0_OR_GREATER
            IEnumerable<string>? dots = dotlist?.Select(dot =>
            {
                var baseInfo = $"{Math.Round(dot.X)} {Math.Round(dot.Y)} {Math.Round(dot.Z)}";
                var colorInfo = Colored ? $" {dot.Red} {dot.Green} {dot.Blue}" : "";

                // 生成自定义属性值
                var customValues = string.Join(" ", dot.CustomProperties
                    .Select((v, i) => FormatCustomValue(v, _regCustomProperties[i].DataType)));

                return $"{baseInfo}{colorInfo} {customValues}".TrimEnd();
            });
#endif
            return SaveVertexes(dots);
        }

        /// <summary>
        /// 新增值格式化方法
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
#if NET45
        private static string FormatCustomValue(object value, Type targetType)
#elif NET9_0_OR_GREATER
        private static string? FormatCustomValue(object value, Type? targetType)
#endif
        {
            if (value == null || targetType == null)
                return "0";

            // C# 8.0 引入的模式匹配增强特性，需要 Visual Studio 2019+ 和 .NET Core 3.0+/.NET 5+ 才能支持。
            //return targetType switch
            //{
            //    _ when targetType == typeof(byte) => ((byte)value).ToString(),
            //    _ when targetType == typeof(int) => ((int)value).ToString(),
            //    _ when targetType == typeof(float) => ((float)value).ToString("F4"),
            //    _ when targetType == typeof(double) => ((double)value).ToString("F6"),
            //    _ => value.ToString()
            //};
#if NET45
            switch (Type.GetTypeCode(targetType))
            {
                case TypeCode.Byte:
                    return ((byte)value).ToString();
                case TypeCode.SByte:
                    return ((sbyte)value).ToString();
                case TypeCode.UInt16:
                    return ((ushort)value).ToString();
                case TypeCode.Int16:
                    return ((short)value).ToString();
                case TypeCode.UInt32:
                    return ((uint)value).ToString();
                case TypeCode.Int32:
                    return ((int)value).ToString();
                case TypeCode.Single:
                    return ((float)value).ToString("F4");
                case TypeCode.Double:
                    return ((double)value).ToString("F6");
                default:
                    return value.ToString();
            }
#elif NET9_0_OR_GREATER
            return targetType switch
            {
                _ when targetType == typeof(byte) => ((byte)value).ToString(),
                _ when targetType == typeof(sbyte) => ((sbyte)value).ToString(),
                _ when targetType == typeof(short) => ((short)value).ToString(),
                _ when targetType == typeof(ushort) => ((ushort)value).ToString(),
                _ when targetType == typeof(int) => ((int)value).ToString(),
                _ when targetType == typeof(uint) => ((uint)value).ToString(),
                _ when targetType == typeof(float) => ((float)value).ToString("F4"),
                _ when targetType == typeof(double) => ((double)value).ToString("F6"),
                _ => value.ToString()
            };
#endif
        }
    }

    /// <summary>
    /// 自定义属性描述类
    /// </summary>
    public class CustomProperty
    {
        /// <summary>
        /// 属性名称
        /// </summary>
#if NET45
        public string Name { get; set; }
#elif NET9_0_OR_GREATER
        public string? Name { get; set; }
#endif

        /// <summary>
        /// 属性数据类型
        /// </summary>
#if NET45
        public Type DataType { get; set; }
#elif NET9_0_OR_GREATER
        public Type? DataType { get; set; }
#endif
    }
}
