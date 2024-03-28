using CommonLib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Web.UI.WebControls;

namespace CommonLib.Function
{
    /// <summary>
    /// INI配置文件操作类
    /// </summary>
    public class IniFileHelper
    {
        private const string V = "kernel32";

        #region 属性
        /// <summary>
        /// INI文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// INI配置项内容默认长度
        /// </summary>
        public int DefaultLength { get; set; }
        #endregion

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

        #region 读取
        #region ReadData
        /// <summary>
        /// 读取INI文件内容
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <returns>返回配置文件内容</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string ReadData(string section, string key)
        {
            return ReadData(section, key, string.Empty, 1024, FilePath);
        }

        /// <summary>
        /// 读取INI文件内容
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="noText">没有该项配置的情况所反馈的信息</param>
        /// <param name="length">配置项内容最大长度</param>
        /// <returns>返回配置文件内容</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string ReadData(string section, string key, string noText, int length)
        {
            return ReadData(section, key, noText, length, FilePath);
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
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ReadData(string section, string key, string noText, int length, string iniFilePath)
        {
            //if (!File.Exists(iniFilePath) || string.IsNullOrWhiteSpace(key) || length <= 0)
            //    return string.Empty;
            if (!File.Exists(iniFilePath))
                throw new FileNotFoundException("没有找到INI配置文件", iniFilePath);
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentOutOfRangeException(nameof(section), "给定的内容区域为空");
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException(nameof(key), "给定的配置项名称为空");
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "配置项内容最大长度至少为1");

            StringBuilder builder = new StringBuilder(length);
            //string noText = key + " not found";
            GetPrivateProfileString(section, key, noText, builder, length, iniFilePath);
            return builder.ToString();
        }
        #endregion

        #region ReadBool
        /// <summary>
        /// 读取INI文件内的布尔型变量，当配置项不存在时返回false
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <returns>返回转换后的布尔型变量</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public bool ReadBool(string section, string key)
        {
            //考虑到当配置项不存在时转换结果将由输入的整型默认值决定，输入默认值0使转换结果为false
            var valuei = ReadInt(section, key, 0);
            return valuei == 1;
        }

        /// <summary>
        /// 读取INI文件内的布尔型变量，当配置项不存在时返回给定的默认值
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">没有该项配置时的默认值</param>
        /// <returns>返回转换后的布尔型变量</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public bool ReadBool(string section, string key, bool def)
        {
            //考虑到当配置项不存在时转换结果将由输入的整型默认值决定，则需要默认转换结果为true时输入默认值1，否则输入默认值0
            var valuei = ReadInt(section, key, def ? 1 : 0);
            return valuei == 1;
        }
        #endregion

        #region ReadInt
        /// <summary>
        /// 读取INI文件内的32位有符号整型数据(Int32)，当配置项不存在时返回0
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <returns>返回转换后的32位有符号整型数据(Int32)</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public int ReadInt(string section, string key)
        {
            return ReadInt(section, key, 0/*, FilePath*/);
        }

        /// <summary>
        /// 读取INI文件内的32位有符号整型数据(Int32)，当配置项不存在时返回给定的默认值
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">没有该项配置时的默认值</param>
        /// <returns>返回转换后的32位有符号整型数据(Int32)</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public int ReadInt(string section, string key, int def)
        {
            var data = ReadData(section, key);
            //假如没有找到配置项，则返回默认值，否则返回转换为整型之后的值
            return string.IsNullOrWhiteSpace(data) ? def : int.Parse(data);
        }
        #endregion

        #region ReadDecimal
        /// <summary>
        /// 读取INI文件内的10进制浮点数(Decimal)，当配置项不存在时返回0
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <returns>返回转换后的10进制浮点数(Decimal)</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public decimal ReadDecimal(string section, string key)
        {
            return ReadDecimal(section, key, 0/*, FilePath*/);
        }

        /// <summary>
        /// 读取INI文件内的10进制浮点数(Decimal)，当配置项不存在时返回给定的默认值
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">没有该项配置时的默认值</param>
        /// <returns>返回转换后的10进制浮点数(Decimal)</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public decimal ReadDecimal(string section, string key, decimal def)
        {
            var data = ReadData(section, key);
            //假如没有找到配置项，则返回默认值，否则返回转换为整型之后的值
            return string.IsNullOrWhiteSpace(data) ? def : decimal.Parse(data);
        }
        #endregion

        #region ReadDouble
        /// <summary>
        /// 读取INI文件内的双精度浮点数(Double)，当配置项不存在时返回0
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <returns>返回转换后的双精度浮点数(Double)</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public double ReadDouble(string section, string key)
        {
            return ReadDouble(section, key, 0);
        }

        /// <summary>
        /// 读取INI文件内的双精度浮点数(Double)，当配置项不存在时返回给定的默认值
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">没有该项配置时的默认值</param>
        /// <returns>返回转换后的双精度浮点数(Double)</returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public double ReadDouble(string section, string key, double def)
        {
            var data = ReadData(section, key);
            //假如没有找到配置项，则返回默认值，否则返回转换为双精度浮点数之后的值
            return string.IsNullOrWhiteSpace(data) ? def : double.Parse(data);
        }
        #endregion

        #region TryReadBool
        /// <summary>
        /// 读取INI文件的配置项并尝试转换为bool变量（配置项值为1时为true，为非1的整数时为false），转换结果由以out符号修饰的参数返回
        /// <para/>假如配置项存在但无法转为整型数值，则转换失败；假如配置项不存在，视为转换成功，但转换结果默认为false
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="parsed">返回的转换结果，只有在转换成功时才可使用</param>
        /// <returns>假如配置项存在但转换失败，将返回false；假如配置项不存在，则返回true</returns>
        public bool TryReadBool(string section, string key, out bool parsed)
        {
            return TryReadBool(section, key, false, out parsed);
        }

        /// <summary>
        /// 读取INI文件的配置项并尝试转换为bool变量（配置项值为1时为true，为非1的整数时为false），转换结果由以out符号修饰的参数返回
        /// <para/>假如配置项存在但无法转为整型数值，则转换失败；假如配置项不存在，视为转换成功，但转换结果为给定的默认值
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">（假如没有找到配置项时的）默认转换结果</param>
        /// <param name="parsed">返回的转换结果，只有在转换成功时才可使用</param>
        /// <returns>假如配置项存在但转换失败，将返回false；假如配置项不存在，则返回true</returns>
        public bool TryReadBool(string section, string key, bool def, out bool parsed)
        {
            //假如无法转换为整型则转换失败
            //考虑到当配置项不存在时转换结果将由输入的整型默认值决定，则需要默认转换结果为true时输入默认值1，否则输入默认值0
            //bool result = TryReadInt(section, key, def ? 1 : 0, out int valuei);
            bool result = TryReadDouble(section, key, def ? 1 : 0, out double valued);
            //对于转换结果，值为1时为true，为非1的整数时为false
            //parsed = valuei == 1;
            parsed = valued == 1;
            return result;
            #region crap
            //bool result = true;
            //parsed = def;
            //var data = ReadData(section, key);
            ////假如没有找到配置项，则转换结果为true、转换值直接返回默认值
            //if (string.IsNullOrWhiteSpace(data))
            //    goto END;
            ////假如无法转换为整型值，则转换结果为false
            //result = int.TryParse(data, out int valuei);
            //if (!result)
            //    goto END;
            ////假如转换成功，则给转换结果赋值
            //parsed = valuei == 1;
            //END:
            //return result;
            #endregion
        }
        #endregion

        #region TryReadInt
        /// <summary>
        /// 读取INI文件的配置项并尝试转换为32位有符号整型变量(Int32)，转换结果由以out符号修饰的参数返回
        /// <para/>假如配置项存在但无法转为整型数值，则转换失败；假如配置项不存在，视为转换成功，但转换结果默认为0
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="parsed">返回的转换结果，只有在转换成功时才可使用</param>
        /// <returns>假如配置项存在但转换失败，将返回false；假如配置项不存在，则返回true</returns>
        public bool TryReadInt(string section, string key, out int parsed)
        {
            return TryReadInt(section, key, 0, out parsed);
        }

        /// <summary>
        /// 读取INI文件的配置项并尝试转换为32位有符号整型变量(Int32)，转换结果由以out符号修饰的参数返回
        /// <para/>假如配置项存在但无法转为整型数值，则转换失败；假如配置项不存在，视为转换成功，但转换结果为给定的默认值
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">（假如没有找到配置项时的）默认转换结果</param>
        /// <param name="parsed">返回的转换结果，只有在转换成功时才可使用</param>
        /// <returns>假如配置项存在但转换失败，将返回false；假如配置项不存在，则返回true</returns>
        public bool TryReadInt(string section, string key, int def, out int parsed)
        {
            bool result = TryReadDouble(section, key, def, out double valued);
            parsed = (int)valued;
            return result;
        }
        #endregion

        #region TryReadDecimal
        /// <summary>
        /// 读取INI文件的配置项并尝试转换为10进制浮点数(Decimal)，转换结果由以out符号修饰的参数返回
        /// <para/>假如配置项存在但无法转为整型数值，则转换失败；假如配置项不存在，视为转换成功，但转换结果默认为0
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="parsed">返回的转换结果，只有在转换成功时才可使用</param>
        /// <returns>假如配置项存在但转换失败，将返回false；假如配置项不存在，则返回true</returns>
        public bool TryReadDecimal(string section, string key, out decimal parsed)
        {
            return TryReadDecimal(section, key, 0, out parsed);
        }

        /// <summary>
        /// 读取INI文件的配置项并尝试转换为10进制浮点数(Decimal)，转换结果由以out符号修饰的参数返回
        /// <para/>假如配置项存在但无法转为整型数值，则转换失败；假如配置项不存在，视为转换成功，但转换结果为给定的默认值
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">（假如没有找到配置项时的）默认转换结果</param>
        /// <param name="parsed">返回的转换结果，只有在转换成功时才可使用</param>
        /// <returns>假如配置项存在但转换失败，将返回false；假如配置项不存在，则返回true</returns>
        public bool TryReadDecimal(string section, string key, int def, out decimal parsed)
        {
            bool result = TryReadDouble(section, key, def, out double valued);
            parsed = (decimal)valued;
            return result;
        }
        #endregion

        #region TryReadDouble
        /// <summary>
        /// 读取INI文件的配置项并尝试转换为双精度浮点数(Double)，转换结果由以out符号修饰的参数返回
        /// <para/>假如配置项存在但无法转为双精度浮点数，则转换失败；假如配置项不存在，视为转换成功，但转换结果默认为0
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="parsed">返回的转换结果，只有在转换成功时才可使用</param>
        /// <returns>假如配置项存在但转换失败，将返回false；假如配置项不存在，则返回true</returns>
        public bool TryReadDouble(string section, string key, out double parsed)
        {
            return TryReadDouble(section, key, 0, out parsed);
        }

        /// <summary>
        /// 读取INI文件的配置项并尝试转换为双精度浮点数(Double)，转换结果由以out符号修饰的参数返回
        /// <para/>假如配置项存在但无法转为双精度浮点数，则转换失败；假如配置项不存在，视为转换成功，但转换结果为给定的默认值
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="def">（假如没有找到配置项时的）默认转换结果</param>
        /// <param name="parsed">返回的转换结果，只有在转换成功时才可使用</param>
        /// <returns>假如配置项存在但转换失败，将返回false；假如配置项不存在，则返回true</returns>
        public bool TryReadDouble(string section, string key, double def, out double parsed)
        {
            bool result = true;
            parsed = def;
            var data = ReadData(section, key);
            //假如没有找到配置项，则转换成功、转换值直接返回默认值
            if (string.IsNullOrWhiteSpace(data))
                goto END;
            //假如无法转换为整型值，则转换失败
            result = double.TryParse(data, out double valuei);
            if (!result)
                goto END;
            //假如转换成功，则给转换结果赋值
            parsed = valuei;
        END:
            return result;
        }
        #endregion
        #endregion

        #region 写入
        /// <summary>
        /// 按指定的格式将双精度浮点数(Double)写INI配置文件，格式默认为null
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="value">待写入的值</param>
        /// <param name="format">写入的格式，如"0.000"可保留3位小数，假如不需要任何格式则为null</param>
        /// <returns></returns>
        public bool WriteDouble(string section, string key, double value, string format = null)
        {
            return WriteData(section, key, value.ToString(string.IsNullOrWhiteSpace(format) ? null : format));
        }

        /// <summary>
        /// 将32位有符号整型值(Int32)写INI配置文件
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="value">待写入的值</param>
        /// <returns>假如操作成功，返回true，否则返回false</returns>
        public bool WriteInt(string section, string key, int value)
        {
            return WriteData(section, key, value.ToString());
        }

        /// <summary>
        /// 将10进制浮点数(Decimal)写INI配置文件
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="value">待写入的值</param>
        /// <returns>假如操作成功，返回true，否则返回false</returns>
        public bool WriteDecimal(string section, string key, decimal value)
        {
            return WriteData(section, key, value.ToString());
        }

        /// <summary>
        /// 将布尔值写INI配置文件
        /// </summary>
        /// <param name="section">内容区域（用中括号[ ] 括起的部分）</param>
        /// <param name="key">每条配置的关键字（形如Key = Value）</param>
        /// <param name="value">待写入的值</param>
        /// <returns>假如操作成功，返回true，否则返回false</returns>
        public bool WriteBool(string section, string key, bool value)
        {
            return WriteData(section, key, value ? "1" : "0");
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
            //if (!File.Exists(iniFilePath) || string.IsNullOrWhiteSpace(key))
            //    return false;
            if (!File.Exists(iniFilePath))
                throw new FileNotFoundException("没有找到INI配置文件", iniFilePath);
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentOutOfRangeException(nameof(section), "给定的内容区域为空");
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException(nameof(key), "给定的配置项名称为空");

            bool result = false;
            try { result = WritePrivateProfileString(section, key, value, iniFilePath) > 0; }
            catch (Exception) { }
            return result;
            //return WritePrivateProfileString(section, key, value, iniFilePath) > 0;
        }
#endregion

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