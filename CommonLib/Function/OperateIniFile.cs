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
    public static class OperateIniFile
    {
        #region API函数声明
        [DllImport("kernel32", CharSet = CharSet.Unicode)]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        #endregion

        #region 读Ini文件
        /// <summary>
        /// 读取INI文件内容
        /// </summary>
        /// <param name="Section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="Key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="NoText">没有该项配置的情况所反馈的信息</param>
        /// <param name="iniFilePath">INI配置文件路径</param>
        /// <returns>返回配置文件内容</returns>
        public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion

        #region 写Ini文件
        /// <summary>
        /// 写INI配置文件
        /// </summary>
        /// <param name="Section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="Key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="Value">配置待写入的值</param>
        /// <param name="iniFilePath">INI配置文件路径</param>
        /// <returns>假如操作成功，返回true，否则返回false</returns>
        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

    }
}