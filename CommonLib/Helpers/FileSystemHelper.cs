using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Helpers
{
    /// <summary>
    /// 文件系统Helper
    /// </summary>
    public static class FileSystemHelper
    {
        #region 属性
        /// <summary>
        /// 可执行文件的启动目录(而不是当前DLL的目录)
        /// </summary>
        public static string StartupPath { get => AppDomain.CurrentDomain.BaseDirectory; }

        /// <summary>
        /// 可执行文件所在盘符根目录(而不是当前DLL的盘符根目录)
        /// </summary>
        public static string DiskRoot { get => Directory.GetDirectoryRoot(StartupPath); }

        /// <summary>
        /// 盘符与路径的分隔符
        /// </summary>
        public static string VolumeSeparator { get { return Path.VolumeSeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString(); } }

        /// <summary>
        /// 当前环境（平台）中的目录分隔符（字符）
        /// </summary>
        public static char DirSeparatorChar { get { return Path.DirectorySeparatorChar; } }

        /// <summary>
        /// 当前环境（平台）中的目录分隔符（字符串）
        /// </summary>
        public static string DirSeparator { get { return Path.DirectorySeparatorChar.ToString(); } }

        /// <summary>
        /// 当前用户的应用程序数据漫游目录（%APPDATA%）
        /// </summary>
        public static string AppDataRoamingPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); } }

        /// <summary>
        /// 当前用户的应用程序数据本地目录（%LOCALAPPDATA%）
        /// </summary>
        public static string AppDataLocalPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); } }

        /// <summary>
        /// 所有用户共用的应用程序数据目录（%PROGRAMDATA%）
        /// </summary>
        public static string AppDataCommonPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); } }

        /// <summary>
        /// 默认的应用程序数据子目录名称，当无法获取产品名时使用
        /// </summary>
        public const string DEFAULT_APP_DIRECTORY = "AppData";
        #endregion

        #region 应用程序数据目录
        /// <summary>
        /// 获取入口程序集的产品名称（AssemblyProduct），获取失败时依次尝试调用程序集，均失败则返回null
        /// </summary>
        /// <returns>产品名称，获取失败返回null</returns>
#if NET45_OR_GREATER
        public static string GetProductName()
#elif NET9_0_OR_GREATER
        public static string? GetProductName()
#endif
        {
            try
            {
                //优先使用入口程序集（宿主exe），服务或单元测试环境下可能为null
                var assembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetCallingAssembly();
                var attribute = (System.Reflection.AssemblyProductAttribute)Attribute.GetCustomAttribute(
                    assembly, typeof(System.Reflection.AssemblyProductAttribute));
                return attribute == null ? null : attribute.Product;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取当前用户在Roaming（%APPDATA%）下的应用程序数据目录，目录不存在时自动创建
        /// <para/>目录名称为产品名称（AssemblyProduct），无法获取时使用<see cref="DEFAULT_APP_DIRECTORY"/>
        /// </summary>
        /// <param name="subDirectory">子目录名称（或次级路径，如xx\xx），为空则不添加子目录</param>
        /// <param name="roaming">是否使用Roaming目录（随用户配置漫游），为false时使用Local目录</param>
        /// <returns>完整的应用程序数据目录路径</returns>
        public static string GetAppDataDirectory(string subDirectory = null, bool roaming = true)
        {
            return GetAppDataDirectory(roaming, null, subDirectory);
        }

        /// <summary>
        /// 获取应用程序数据目录，目录不存在时自动创建
        /// </summary>
        /// <param name="roaming">是否使用Roaming目录（随用户配置漫游），为false时使用Local目录</param>
        /// <param name="appName">应用程序目录名称，为null或空白时使用产品名称，再无法获取时使用<see cref="DEFAULT_APP_DIRECTORY"/></param>
        /// <param name="subDirectory">子目录名称（或次级路径，如xx\xx），为空则不添加子目录</param>
        /// <returns>完整的应用程序数据目录路径</returns>
        public static string GetAppDataDirectory(bool roaming, string appName, string subDirectory = null)
        {
            string root = roaming ? AppDataRoamingPath : AppDataLocalPath;
            if (string.IsNullOrWhiteSpace(appName))
                appName = GetProductName();
            if (string.IsNullOrWhiteSpace(appName))
                appName = DEFAULT_APP_DIRECTORY;

            string dir = Path.Combine(root, TrimFilePath(appName));
            if (!string.IsNullOrWhiteSpace(subDirectory))
                dir = Path.Combine(dir, TrimFilePath(subDirectory));

            //确保目录存在
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        /// <summary>
        /// 获取应用程序数据目录下的文件完整路径，所在目录不存在时自动创建
        /// </summary>
        /// <param name="fileName">文件名称（可包含次级路径）</param>
        /// <param name="roaming">是否使用Roaming目录，为false时使用Local目录</param>
        /// <param name="appName">应用程序目录名称，为null或空白时使用产品名称</param>
        /// <returns>包含文件名的完整路径</returns>
        public static string GetAppDataFilePath(string fileName, bool roaming = true, string appName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("文件名称为空引用、空字符串或空白字符串", nameof(fileName));

            string dir = GetAppDataDirectory(roaming, appName);
            return Path.Combine(dir, TrimFilePath(fileName));
        }

        /// <summary>
        /// 在资源管理器中打开应用程序数据目录（目录不存在时自动创建）
        /// </summary>
        /// <param name="roaming">是否使用Roaming目录，为false时使用Local目录</param>
        /// <param name="appName">应用程序目录名称，为null或空白时使用产品名称</param>
        /// <param name="subDirectory">子目录名称，为空则不添加子目录</param>
        /// <returns>被打开的目录路径</returns>
        public static string OpenAppDataDirectory(bool roaming = true, string appName = null, string subDirectory = null)
        {
            string dir = GetAppDataDirectory(roaming, appName, subDirectory);
            Process.Start("Explorer.exe", dir);
            return dir;
        }
        #endregion

        #region 进程相关
        /// <summary>
        /// 杀掉所有与给定的文件对象相关的进程
        /// </summary>
        /// <param name="fileInfo">给定的文件对象</param>
        /// <returns>假如操作成功则返回true，否则假如有一个操作未成功则返回false</returns>
        public static bool KillRelatedProcesses(this FileInfo fileInfo)
        {
            return KillRelatedProcesses(fileInfo, out _);
        }

        /// <summary>
        /// 杀掉所有与给定的文件对象相关的进程，并输出错误信息
        /// </summary>
        /// <param name="fileInfo">给定的文件对象</param>
        /// <param name="error">返回的错误信息</param>
        /// <returns>假如操作成功则返回true，否则假如有一个操作未成功则返回false</returns>
        public static bool KillRelatedProcesses(this FileInfo fileInfo, out string error)
        {
            bool result = true;
            error = string.Empty;
            if (fileInfo == null || !fileInfo.Exists)
                goto RET;

            try
            {
                string name = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
                Process[] processes = Process.GetProcessesByName(name);
                foreach (Process process in processes)
                    process.Kill();
            }
            catch (Exception e)
            {
                error = e.Message;
                result = false;
            }
        RET:
            return result;
        }

        /// <summary>
        /// 启动一个与给定的文件对象相关的进程
        /// </summary>
        /// <param name="fileInfo">给定的文件对象</param>
        /// <returns>假如操作成功则返回true，否则返回false</returns>
        public static bool StartRelatedProcess(this FileInfo fileInfo)
        {
            return StartRelatedProcess(fileInfo, 1, out _);
        }

        /// <summary>
        /// 启动给定数量的与给定的文件对象相关的进程
        /// </summary>
        /// <param name="fileInfo">给定的文件对象</param>
        /// <param name="count">启动的进程数量</param>
        /// <returns>假如操作成功则返回true，否则假如有一个操作未成功则返回false</returns>
        public static bool StartRelatedProcess(this FileInfo fileInfo, int count)
        {
            return StartRelatedProcess(fileInfo, count, out _);
        }

        /// <summary>
        /// 启动给定数量的与给定的文件对象相关的进程，并输出错误信息
        /// </summary>
        /// <param name="fileInfo">给定的文件对象</param>
        /// <param name="count">启动的进程数量</param>
        /// <param name="error">返回的错误信息</param>
        /// <returns>假如操作成功则返回true，否则假如有一个操作未成功则返回false</returns>
        public static bool StartRelatedProcess(this FileInfo fileInfo, int count, out string error)
        {
            bool result = true;
            error = string.Empty;
            if (fileInfo == null || !fileInfo.Exists)
                goto RET;

            try
            {
                for (int i = 0; i < count; i++)
                    Process.Start(fileInfo.FullName);
            }
            catch (Exception e)
            {
                error = e.Message;
                result = false;
            }
        RET:
            return result;
        }
        #endregion

        /// <summary>
        /// 在资源管理器中打开指定的文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="ignoreIfNotExist">假如文件不存在，是否忽略并直接显示所在路径</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void OpenInExplorer(string filePath, bool ignoreIfNotExist = true)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("给定的文件名称为空引用、空字符串或空白字符串", nameof(filePath));
            var fileInfo = new FileInfo(filePath);
            //假如文件存在，直接在资源管理器中打开并选中
            if (fileInfo.Exists)
                Process.Start("Explorer.exe", "/select," + filePath);
            //假如不存在则进行一些额外判断
            else
            {
                //假如不忽略文件不存在的问题，则抛出异常
                if (!ignoreIfNotExist)
                    throw new FileNotFoundException("给定的文件不存在", filePath);
                //假如文件所在的目录存在则在资源管理器中打开，否则抛出异常
                var dir = fileInfo.DirectoryName;
                if (!Directory.Exists(dir))
                    throw new DirectoryNotFoundException($"给定的路径{dir}不存在");
                Process.Start("Explorer.exe", dir);
            }
        }

        #region 文件名操作
        /// <summary>
        /// 检查给定的路径（或给定的文件所在的路径）是否存在，不存在则创建
        /// </summary>
        /// <param name="path">路径名称，或包含路径的完整文件名称</param>
        /// <param name="isFilePath">是否为文件路径，假如为false则一律按照路径操作</param>
#if NET45_OR_GREATER
        public static void CheckForDirectory(string path, bool isFilePath = false)
#elif NET9_0_OR_GREATER
        public static void CheckForDirectory(string? path, bool isFilePath = false)
#endif
        {
            if (isFilePath)
            {
                if (path == null)
                    throw new ArgumentNullException(nameof(path), "文件路径不能为null");
#if NET45_OR_GREATER
                FileInfo fileInfo = new FileInfo(path);
#elif NET9_0_OR_GREATER
                FileInfo fileInfo = new(path);
#endif
                path = fileInfo.DirectoryName ?? string.Empty;
            }
            if (path == null)
                throw new ArgumentNullException(nameof(path), "路径不能为null");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 检查路径或包含路径的完整文件名，假如路径中不包含卷分隔符则在前添加启动目录
        /// </summary>
        /// <param name="path">路径，或包含路径的完整文件名</param>
        /// <param name="ignoreEmptyString">是否忽略传入的空白字符串（字符串为null、空、充满空白字符），为true时直接返回空字符串，否则依然添加启动目录（此时与启动目录本身等价）</param>
        /// <returns></returns>
        public static string CheckForStartupPath(string path, bool ignoreEmptyString = false)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = string.Empty;
            //假如路径中不包含卷分隔符，添加启动目录
            if ((!ignoreEmptyString || !string.IsNullOrWhiteSpace(path)) && !path.Contains(VolumeSeparator))
                //path = StartupPath + TrimFilePath(path) + DirSeparator;
                path = StartupPath + path;
            return path;
        }

        /// <summary>
        /// 将日期添加到文件名中（文件名包含后缀）
        /// </summary>
        /// <param name="fileName">待处理的文件名称，包括后缀</param>
        /// <param name="format">将DateTime格式化的格式字符串，默认为yyyyMMdd</param>
        /// <returns></returns>
        public static string AddDateToFileName(string fileName, string format = "yyyyMMdd")
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            string date = DateTime.Now.ToString(format);
#if NET45_OR_GREATER
            string[] parts = fileName.Split('.').Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
#elif NET9_0_OR_GREATER
            string[] parts = [.. fileName.Split('.').Where(p => !string.IsNullOrWhiteSpace(p))];
#endif
            if (parts.Length == 1)
                parts[0] += " " + date;
            else
#if NET45_OR_GREATER
                parts[parts.Length - 2] += " " + date;
#elif NET9_0_OR_GREATER
                parts[^2] += " " + date;
#endif
            return string.Join(".", parts);
        }

        /// <summary>
        /// 去除路径名称首部以及尾部的路径分隔符
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string TrimFilePath(string filePath)
        {
            //if (string.IsNullOrWhiteSpace(filePath))
            //    throw new ArgumentException("路径名称为空！");
            if (string.IsNullOrWhiteSpace(filePath))
                filePath = string.Empty;

#if NET45_OR_GREATER
            return filePath.Trim(new char[] { DirSeparatorChar });
#elif NET9_0_OR_GREATER
            return filePath.Trim([DirSeparatorChar]);
#endif
        }

        /// <summary>
        /// 根据路径与文件名称获取补完的路径与文件完整路径，假如文件名称为空则输出全为空字符串
        /// </summary>
        /// <param name="path">文件所在路径，假如为相对路径则在开头添加启动路径</param>
        /// <param name="fileName">文件名称，假如为空则输出全为空字符串</param>
        /// <param name="fileNameDate">带日期的文件名称</param>
        /// <param name="filePath">包含文件名称的完整路径</param>
        /// <param name="filePathDate">包含带日期的文件名称的完整路径</param>
        /// <param name="dateFormatInFileName">将文件名内的日期时间格式化的格式字符串，默认为yyyyMMdd</param>
        public static void UpdateFilePath(ref string path, string fileName, out string fileNameDate, out string filePath, out string filePathDate, string dateFormatInFileName = "yyyyMMdd")
        {
            fileNameDate = filePath = filePathDate = string.Empty;
            if (string.IsNullOrWhiteSpace(fileName))
                return;
            if (string.IsNullOrWhiteSpace(path))
                path = string.Empty;
            path = CheckForStartupPath(TrimFilePath(path) + DirSeparator);
            //path = TrimFilePath(path) + DirSeparator; //确保路径字符串末尾包含路径分隔符
            ////假如路径中不包含卷分隔符，添加根目录
            //if (!path.Contains(VolumeSeparator))
            //    //path = StartupPath + TrimFilePath(path) + DirSeparator;
            //    path = StartupPath + path;
            fileNameDate = AddDateToFileName(fileName, dateFormatInFileName); //带日期的文件名称
            //filePath = TrimFilePath(path) + DirSeparator + fileName; //包含文件名的路径
            //filePathDate = TrimFilePath(path) + DirSeparator + fileNameDate; //带日期的路径
            filePath = path + fileName; //包含文件名的路径
            filePathDate = path + fileNameDate; //带日期的路径
        }

        /// <summary>
        /// 根据路径与文件名称获取补完的路径与文件完整路径，假如文件名称为空则输出全为空字符串
        /// </summary>
        /// <param name="path">文件所在路径，假如为相对路径则在开头添加启动路径</param>
        /// <param name="fileName">文件名称，假如为空则输出全为空字符串</param>
        /// <param name="filePath">包含文件名称的完整路径</param>
        public static void UpdateFilePath(ref string path, string fileName, out string filePath)
        {
            UpdateFilePath(ref path, fileName, out _, out filePath, out _);
        }
        #endregion
    }
}
