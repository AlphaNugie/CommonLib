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
            this.FilePath = filePath;
            if (!this.FilePath.Contains(Base.VolumeSeparator))
                this.FilePath = AppDomain.CurrentDomain.BaseDirectory + Base.DirSeparator + this.FilePath;
            this.DefaultLength = length;
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
        public string ReadData(string section, string key, string noText, int length, string iniFilePath)
        {
            if (!File.Exists(iniFilePath) || string.IsNullOrWhiteSpace(key) || length <= 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder(length);
            //string noText = key + " not found";
            IniFileHelper.GetPrivateProfileString(section, key, noText, builder, length, iniFilePath);
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
            return this.ReadData(section, key, noText, length, this.FilePath);
        }

        /// <summary>
        /// 读取INI文件内容
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <returns>返回配置文件内容</returns>
        public string ReadData(string section, string key)
        {
            return this.ReadData(section, key, key + " not found", 1024, this.FilePath);
        }

        /// <summary>
        /// 写INI配置文件
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="value">配置待写入的值</param>
        /// <param name="iniFilePath">INI配置文件路径</param>
        /// <returns>假如操作成功，返回true，否则返回false</returns>
        public bool WriteData(string section, string key, string value, string iniFilePath)
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
            return this.WriteData(section, key, value, this.FilePath);
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
        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        /// <summary>
        /// 获取INI配置文件内容
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">默认值</param>
        /// <param name="retVal">负责拼接配置项内容的StringBuilder对象</param>
        /// <param name="size">配置项内容最大尺寸</param>
        /// <param name="filePath">INI配置文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);
        #endregion
    }
}