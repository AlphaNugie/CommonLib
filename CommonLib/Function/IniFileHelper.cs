using CommonLib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CommonLib.Function
{
    /// <summary>
    /// INI配置文件操作类
    /// </summary>
    public class IniFileHelper
    {
        private const string V = "kernel32";

        /// <summary>
        /// INI文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// INI配置项内容默认长度
        /// </summary>
        public int DefaultLength { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public IniFileHelper() : this(string.Empty, 1024) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="filePath">INI文件路径</param>
        public IniFileHelper(string filePath) : this(filePath, 1024) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="filePath">INI文件路径</param>
        /// <param name="length">INI配置项内容默认长度</param>
        public IniFileHelper(string filePath, int length)
        {
            FilePath = filePath;
            if (!FilePath.Contains(FileSystemHelper.VolumeSeparator))
                FilePath = AppDomain.CurrentDomain.BaseDirectory + FileSystemHelper.DirSeparator + FilePath;
            DefaultLength = length;
        }

        /// <summary>
        /// 读取INI文件内容
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="noText">没有该项配置的情况所反馈的信息</param>
        /// <param name="length">配置项内容最大长度</param>
        /// <param name="iniFilePath">INI配置文件路径</param>
        /// <returns>返回配置文件内容</returns>
        public static string ReadData(string section, string key, string noText, int length, string iniFilePath)
        {
            if (!File.Exists(iniFilePath) || string.IsNullOrWhiteSpace(key) || length <= 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder(length);
            //string noText = key + " not found";
            GetPrivateProfileString(section, key, noText, builder, length, iniFilePath);
            return builder.ToString();
        }

        /// <summary>
        /// 读取INI文件内容
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="noText">没有该项配置的情况所反馈的信息</param>
        /// <param name="length">配置项内容最大长度</param>
        /// <returns>返回配置文件内容</returns>
        public string ReadData(string section, string key, string noText, int length)
        {
            return ReadData(section, key, noText, length, FilePath);
        }

        /// <summary>
        /// 读取INI文件内容
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <returns>返回配置文件内容</returns>
        public string ReadData(string section, string key)
        {
            //return ReadData(section, key, key + " not found", 1024, FilePath);
            return ReadData(section, key, string.Empty, 1024, FilePath);
        }

        /// <summary>
        /// 读取INI文件内的整型数据
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">没有该项配置的默认值</param>
        /// <param name="iniFilePath">INI配置文件路径</param>
        /// <returns>返回配置文件内容</returns>
        public static int ReadInt(string section, string key, int def, string iniFilePath)
        {
            if (!File.Exists(iniFilePath) || string.IsNullOrWhiteSpace(key))
                return def;

            long result = GetPrivateProfileInt(section, key, def, iniFilePath);
            return (int)result;
        }

        /// <summary>
        /// 读取INI文件内的整型数据
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">没有该项配置的默认值</param>
        /// <returns>返回配置文件内容</returns>
        public int ReadInt(string section, string key, int def)
        {
            return ReadInt(section, key, def, FilePath);
        }

        /// <summary>
        /// 读取INI文件内的整型数据
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <returns>返回配置文件内容</returns>
        public int ReadInt(string section, string key)
        {
            return ReadInt(section, key, 0, FilePath);
        }

        /// <summary>
        /// 写INI配置文件
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="value">配置待写入的值</param>
        /// <param name="iniFilePath">INI配置文件路径</param>
        /// <returns>假如操作成功，返回true，否则返回false</returns>
        public static bool WriteData(string section, string key, string value, string iniFilePath)
        {
            if (!File.Exists(iniFilePath) || string.IsNullOrWhiteSpace(key))
                return false;

            return WritePrivateProfileString(section, key, value, iniFilePath) > 0;
        }

        /// <summary>
        /// 写INI配置文件
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="value">配置待写入的值</param>
        /// <returns>假如操作成功，返回true，否则返回false</returns>
        public bool WriteData(string section, string key, string value)
        {
            return WriteData(section, key, value, FilePath);
        }

        #region API函数声明
        /// <summary>
        /// 写入INI配置文件内容
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="val">待写入的值</param>
        /// <param name="filePath">INI配置文件路径</param>
        /// <returns></returns>
        [DllImport(V, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        /// <summary>
        /// 获取INI配置文件中字符串类型数据
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">默认值</param>
        /// <param name="retVal">负责拼接配置项内容的StringBuilder对象</param>
        /// <param name="size">配置项内容最大尺寸</param>
        /// <param name="filePath">INI配置文件路径</param>
        /// <returns></returns>
        [DllImport(V, CharSet = CharSet.Unicode)]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 获取INI配置文件中32位有符号整型类型数据
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">默认值</param>
        /// <param name="filePath">INI配置文件路径</param>
        /// <returns></returns>
        [DllImport(V, CharSet = CharSet.Unicode)]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileInt(string section, string key,
            int def, string filePath);
        #endregion
    }
}