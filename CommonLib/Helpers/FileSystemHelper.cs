﻿using System;
using System.Collections.Generic;
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
        /// <summary>
        /// 可执行文件的启动目录(而不是当前DLL的目录)
        /// </summary>
        public static string StartupPath { get { return AppDomain.CurrentDomain.BaseDirectory; } }

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
        /// 将日期添加到文件名中（文件名包含后缀）
        /// </summary>
        /// <param name="fileName">待处理的文件名称，包括后缀</param>
        /// <returns></returns>
        public static string AddDateToFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            string date = DateTime.Now.ToString("yyyyMMdd");
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

            return filePath.Trim(new char[] { DirSeparatorChar });
        }

        /// <summary>
        /// 根据路径与文件名称获取补完的路径与文件完整路径
        /// </summary>
        /// <param name="path">文件所在路径，假如为相对路径则在开头添加启动路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="fileNameDate">带日期的文件名称</param>
        /// <param name="filePath">包含文件名称的完整路径</param>
        /// <param name="filePathDate">包含带日期的文件名称的完整路径</param>
        public static void UpdateFilePath(ref string path, string fileName, out string fileNameDate, out string filePath, out string filePathDate)
        {
            path = TrimFilePath(path) + DirSeparator; //确保路径字符串末尾包含路径分隔符
            //假如路径中不包含卷分隔符，添加根目录
            if (!path.Contains(VolumeSeparator))
                //path = StartupPath + TrimFilePath(path) + DirSeparator;
                path = StartupPath + path;
            fileNameDate = AddDateToFileName(fileName); //带日期的文件名称
            //filePath = TrimFilePath(path) + DirSeparator + fileName; //包含文件名的路径
            //filePathDate = TrimFilePath(path) + DirSeparator + fileNameDate; //带日期的路径
            filePath = path + fileName; //包含文件名的路径
            filePathDate = path + fileNameDate; //带日期的路径
        }

        /// <summary>
        /// 根据路径与文件名称获取补完的路径与文件完整路径
        /// </summary>
        /// <param name="path">文件所在路径，假如为相对路径则在开头添加启动路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="filePath">包含文件名称的完整路径</param>
        public static void UpdateFilePath(ref string path, string fileName, out string filePath)
        {
            UpdateFilePath(ref path, fileName, out _, out filePath, out _);
        }
    }
}
