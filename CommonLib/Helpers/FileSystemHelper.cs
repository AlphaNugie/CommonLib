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

        #region 文件名操作
        /// <summary>
        /// 检查给定的路径（或给定的文件所在的路径）是否存在，不存在则创建
        /// </summary>
        /// <param name="path">路径名称，或包含路径的完整文件名称</param>
        /// <param name="isFilePath">是否为文件路径，假如为false则一律按照路径操作</param>
        public static void CheckForDirectory(string path, bool isFilePath = false)
        {
            if (isFilePath)
            {
                FileInfo fileInfo = new FileInfo(path);
                path = fileInfo.DirectoryName;
            }
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

            //string date = DateTime.Now.ToString("yyyyMMdd");
            string date = DateTime.Now.ToString(format);
            string[] parts = fileName.Split('.').Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
            if (parts.Length == 1)
                parts[0] += " " + date;
            else
                parts[parts.Length - 2] += " " + date;
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

            return filePath.Trim(new char[] { DirSeparatorChar });
        }

        /// <summary>
        /// 根据路径与文件名称获取补完的路径与文件完整路径，假如文件名称为空则输出全为空字符串
        /// </summary>
        /// <param name="path">文件所在路径，假如为相对路径则在开头添加启动路径</param>
        /// <param name="fileName">文件名称，假如为空则输出全为空字符串</param>
        /// <param name="fileNameDate">带日期的文件名称</param>
        /// <param name="filePath">包含文件名称的完整路径</param>
        /// <param name="filePathDate">包含带日期的文件名称的完整路径</param>
        public static void UpdateFilePath(ref string path, string fileName, out string fileNameDate, out string filePath, out string filePathDate)
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
            fileNameDate = AddDateToFileName(fileName); //带日期的文件名称
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
