﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommonLib.Clients;

namespace CommonLib.Function
{
    /// <summary>
    /// 进行正则表达式匹配的类
    /// </summary>
    public static class RegexMatcher
    {
        #region 常用正则表达式
        /// <summary>
        /// 表示0-65535的正整数的正则表达式
        /// </summary>
        public const string RegexPattern_65535 = @"0|[1-9]\d{0,3}|[1-5]\d{4}|6[0-4]\d{3}|65[0-4]\d{2}|655[0-2]\d|6553[0-5]";

        /// <summary>
        /// 表示IP地址的正则表达式（0-255.0-255.0-255.0-255或localhost）
        /// </summary>
        public const string RegexPattern_Ip = @"localhost|([1-9]?\d|1\d{0,2}|2[0-4]\d|25[0-5])(\.([1-9]?\d|1\d{0,2}|2[0-4]\d|25[0-5])){3}";

        /// <summary>
        /// 匹配日期或日期时间的表达式；格式：yyyy/m/d(或yyyy-m-d)( h:mm(:ss))
        /// </summary>
        public const string RegexPattern_DateTime = @"([123][0-9])?[0-9][0-9](/|-)(0?[1-9]|1[012])(/|-)([012]?[1-9]|[123]0|31)(\s(0?\d|1\d|2[0-3])\:[0-5]\d(\:[0-5]\d)?)?";

        /// <summary>
        /// 匹配数字+#符号的表达式，如：34..#
        /// </summary>
        public const string RegexPattern_DigitHash = @"\d+#";

        /// <summary>
        /// 匹配皮带秤类型（名称）的正则表达式，大写字母+若干数字（数字数目可为0），后面可能跟有_+至少一个数字
        /// </summary>
        public const string RegexPattern_BeltType = @"\p{L}+\d*\p{L}*(_\d+)?";

        /// <summary>
        /// 匹配MyBatis参数格式的正则表达式，如 #{property,jdbcType=INTEGER} 或 #{some}
        /// </summary>
        public const string RegexPattern_BatisParam = @"\#\{((_)?[0-9a-zA-Z](_)?)*(,(\s)*jdbcType=[A-Z]+)?\}";

        /// <summary>
        /// 匹配枚举数某元素索引的正则表达式（索引为为非负整数），形如：[123]
        /// </summary>
        public const string RegexPattern_EnumIndexes = @"\[[0-9]+\]";

        /// <summary>
        /// 匹配一对方括号的正则表达式，中间可包含任意数量的任意字符，形如：[1][2][3] 或 [a*b_c]
        /// </summary>
        public const string RegexPattern_Brackets = @"\[[\s\S]*\]";
        #endregion

        /// <summary>
        /// 带匹配选项地判断输入字符串是否符合某个正则表达式
        /// </summary>
        /// <param name="input">待判断字符串</param>
        /// <param name="regexPattern">正则表达式（不包括^, $）</param>
        /// <param name="option">正则匹配选项</param>
        /// <returns>假如符合，返回true</returns>
        public static bool IsMatchRegex(string input, string regexPattern, RegexOptions option)
        {
            return Regex.IsMatch(input, string.Format(@"^{0}$", regexPattern), option);
        }

        /// <summary>
        /// 判断输入字符串是否符合某个正则表达式
        /// </summary>
        /// <param name="input">待判断字符串</param>
        /// <param name="regexPattern">正则表达式（不包括^, $）</param>
        /// <returns>假如符合，返回true</returns>
        public static bool IsMatchRegex(string input, string regexPattern)
        {
            return IsMatchRegex(input, regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// 用正则表达式验证IP地址的格式是否正确
        /// </summary>
        /// <param name="ipAddress">待验证的IP地址字符串</param>
        /// <returns>假如符合IP地址的格式，返回true</returns>
        public static bool IsIpAddressValidated(string ipAddress)
        {
            return IsMatchRegex(ipAddress, RegexPattern_Ip, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 用正则表达式验证端口号是否在0-65535之间
        /// </summary>
        /// <param name="portNumber">待验证端口号</param>
        /// <returns>假如范围正确，返回true</returns>
        public static bool IsPortNumberValidated(string portNumber)
        {
            return IsMatchRegex(portNumber, RegexPattern_65535);
        }

        /// <summary>
        /// 用正则表达式验证是否为日期(yyyy/m/d或yyyy-m-d)或日期时间(h24:m)
        /// </summary>
        /// <param name="dateTime">待验证的日期时间字符串</param>
        /// <returns>假如为日期或日期时间格式，返回true</returns>
        public static bool IsDateTimeValidated(string dateTime)
        {
            return IsMatchRegex(dateTime, RegexPattern_DateTime);
        }

        /// <summary>
        /// 正则表达式验证字符串是否符合格式34..#（若干十进制数字+#）
        /// </summary>
        /// <param name="digitHash">待验证的字符串</param>
        /// <returns>假如格式相符，返回true，否则返回false</returns>
        public static bool IsMatchDigitHash(string digitHash)
        {
            return IsMatchRegex(digitHash, RegexPattern_DigitHash);
        }

        /// <summary>
        /// 从待查找字符串中提取出第一项匹配正则表达式的Match对象
        /// </summary>
        /// <param name="input">待查找字符串</param>
        /// <param name="pattern">匹配模式</param>
        /// <returns>返回包含匹配字符串的Match对象</returns>
        public static Match FindFirstMatch_Detail(string input, string pattern)
        {
            IEnumerable<Match> results = FindMatches_Detail(input, pattern);
            if (results == null || results.Count() == 0)
                return null;

            return results.First();
        }

        /// <summary>
        /// 从待查找字符串中提取出最后一个匹配正则表达式的字符子串
        /// </summary>
        /// <param name="input">待查找字符串</param>
        /// <param name="pattern">匹配模式</param>
        /// <returns>返回包含匹配字符串的字符串数组</returns>
        public static Match FindLastMatch_Detail(string input, string pattern)
        {
            IEnumerable<Match> results = FindMatches_Detail(input, pattern);
            if (results == null || results.Count() == 0)
                return null;

            return results.Last();
        }

        /// <summary>
        /// 从待查找字符串中提取出第一项匹配正则表达式的字符子串
        /// </summary>
        /// <param name="input">待查找字符串</param>
        /// <param name="pattern">匹配模式</param>
        /// <returns>返回包含匹配字符串的字符串数组</returns>
        public static string FindFirstMatch(string input, string pattern)
        {
            Match match = FindFirstMatch_Detail(input, pattern);
            return match?.Value;
        }

        /// <summary>
        /// 从待查找字符串中提取出最后一个匹配正则表达式的字符子串
        /// </summary>
        /// <param name="input">待查找字符串</param>
        /// <param name="pattern">匹配模式</param>
        /// <returns>返回包含匹配字符串的字符串数组</returns>
        public static string FindLastMatch(string input, string pattern)
        {
            Match match = FindLastMatch_Detail(input, pattern);
            return match?.Value;
        }

        /// <summary>
        /// 从待查找字符串中提取出匹配正则表达式的Match匹配对象
        /// </summary>
        /// <param name="input">待查找字符串</param>
        /// <param name="pattern">匹配模式</param>
        /// <returns>返回包含匹配字符串的Match数组</returns>
        public static IEnumerable<Match> FindMatches_Detail(string input, string pattern)
        {
            IEnumerable<Match> infos;
            MatchCollection collection;

            try
            {
                if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(pattern))
                    infos = null;
                else
                    infos = (collection = Regex.Matches(input, pattern)).Cast<Match>();
            }
            catch (Exception e)
            {
                FileClient.WriteExceptionInfo(e, string.Format("通过正则表达式从字符串获取匹配字符串时出错，输入字符串：{0}，匹配格式：{1}", input, pattern), true);
                throw;
            }

            return infos;
        }

        /// <summary>
        /// 从待查找字符串中提取出匹配正则表达式的字符子串
        /// </summary>
        /// <param name="input">待查找字符串</param>
        /// <param name="pattern">匹配模式</param>
        /// <returns>返回包含匹配字符串的字符串数组</returns>
        public static string[] FindMatches(string input, string pattern)
        {
            IEnumerable<Match> matches = FindMatches_Detail(input, pattern);
            string[] infos = matches?.Select(match => match.Value).ToArray();

            return infos;
        }
    }
}
