﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
//using Microsoft.International.Converters.PinYinConverter;
using CommonLib.Enums;
using Microsoft.Win32;
using System.Net;
using CommonLib.UIControlUtil.WPF;
using System.Runtime.InteropServices;
using CommonLib.Helpers;
using System.Web.UI.WebControls;
using System.Drawing.Design;
using CommonLib.Extensions;

namespace CommonLib.Function
{
    /// <summary>
    /// 基础的、公用的变量
    /// </summary>
    public struct Base
    {
        /// <summary>
        /// 全屏时的WPF窗口信息
        /// </summary>
        public static WindowStateInfo FullScreenInfo = WindowStateInfo.GetFullScreenInfo();

        /// <summary>
        /// 特殊字符，代表正文开始
        /// </summary>
        public const char STX = (char)2;

        /// <summary>
        /// 特殊字符，代表正文结束
        /// </summary>
        public const char ETX = (char)3;

        /// <summary>
        /// 当前环境（平台）中的回车换行符
        /// </summary>
        public static string NewLine { get { return Environment.NewLine; } }

        /// <summary>
        /// 代表必填项的红色*号（HTML格式）
        /// </summary>
        public const string MARK_NEEDED = "<color=red>*</color>";

        /// <summary>
        /// 代替密码明文的字符
        /// </summary>
        public const char PASSWORD_CHAR = '●';
    }

    /// <summary>
    /// 基础的、公用的操作
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// 根据给定的整数序列描述字符串（形如“-5,-3~-1,3,6~10”）输出整数序列
        /// </summary>
        /// <param name="descp">整数序列的描述字符串（形如“-5,-3~-1,3,6~10”）</param>
        /// <param name="splitChar">描述数字分隔关系的字符，默认为,</param>
        /// <param name="intervalChar">描述区间起始结束关系的字符，默认为~（为避免负数不要使用-）</param>
        /// <returns></returns>
        [Obsolete("应使用MathUtil下的静态方法")]
        public static List<int> GetIntegerListByString(string descp, char splitChar = ',', char intervalChar = '~')
        {
            return MathUtil.GetIntegerListByString(descp, splitChar, intervalChar);
        //    var list = new List<int>();
        //    if (string.IsNullOrWhiteSpace(descp))
        //        goto END;
        //    //string[] splits1 = descp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    string[] splits1 = descp.Split(new char[] { splitChar }, StringSplitOptions.RemoveEmptyEntries);
        //    if (splits1 == null || splits1.Length == 0)
        //        goto END;
        //    foreach (var split in splits1)
        //    {
        //        //string[] splits2 = split.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
        //        string[] splits2 = split.Split(new char[] { intervalChar }, StringSplitOptions.RemoveEmptyEntries);
        //        if (!int.TryParse(splits2[0], out int start))
        //            continue;
        //        int end = splits2.Length >= 2 && int.TryParse(splits2[1], out end) ? end : start;
        //        if (end < start)
        //            MathExtension.Swap(ref start, ref end);
        //        for (int i = start; i <= end; i++)
        //            list.Add(i);
        //    }
        //    //list.Sort();
        //END:
        //    return list;
        }

        /// <summary>
        /// 获取IPV4的地址
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddressV4()
        {
            return GetIpAddressByAddressFamily(AddressFamily.InterNetwork, string.Empty, out _);
        }

        /// <summary>
        /// 获取IPV4的地址
        /// </summary>
        /// <param name="start_with">用于过滤地址的地址起始匹配条件，假如为空则不过滤</param>
        /// <returns></returns>
        public static string GetIPAddressV4(string start_with)
        {
            return GetIpAddressByAddressFamily(AddressFamily.InterNetwork, start_with, out _);
        }

        /// <summary>
        /// 获取与给定的IPV4地址相似的地址（同网段），假如给定地址为空则不过滤
        /// </summary>
        /// <param name="otherAddress">用于过滤地址的同网段IPV4地址，假如为空则不过滤</param>
        /// <returns></returns>
        public static string GetIPAddressV4Alike(string otherAddress)
        {
            string address;
            if ("127.0.0.1".Equals(otherAddress) || "localhost".Equals(otherAddress))
                address = "127.0.0.1";
            else
            {
                //找出与给定IPV4地址第一个数字相同的地址
                string[] parts = otherAddress?.Split('.');
                address = GetIPAddressV4(parts != null && parts.Length > 0 ? parts[0] : null);
            }
            return address;
        }

        /// <summary>
        /// 获取IPV4的地址
        /// </summary>
        /// <param name="hostName">主机名称</param>
        /// <returns></returns>
        public static string GetIPAddressV4(out string hostName)
        {
            return GetIpAddressByAddressFamily(AddressFamily.InterNetwork, string.Empty, out hostName);
        }

        /// <summary>
        /// 获取IPV6的地址
        /// </summary>
        /// <param name="hostName">主机名称</param>
        /// <returns></returns>
        public static string GetIPAddressV6(out string hostName)
        {
            return GetIpAddressByAddressFamily(AddressFamily.InterNetworkV6, string.Empty, out hostName);
        }

        /// <summary>
        /// 根据地址类型获取第一个IP地址
        /// </summary>
        /// <param name="family">地址类型(IPV4，IPV6或其它)</param>
        /// <param name="hostName">主机名称</param>
        /// <returns></returns>
        public static string GetIpAddressByAddressFamily(AddressFamily family, out string hostName)
        {
            return GetIpAddressByAddressFamily(family, string.Empty, out hostName);
        }

        /// <summary>
        /// 根据地址类型获取第一个IP地址
        /// </summary>
        /// <param name="family">地址类型(IPV4，IPV6或其它)</param>
        /// <param name="start_with">用于过滤地址的地址起始匹配条件，假如为空则不过滤</param>
        /// <param name="hostName">主机名称</param>
        /// <returns></returns>
        public static string GetIpAddressByAddressFamily(AddressFamily family, string start_with, out string hostName)
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Environment.MachineName);
            hostName = ipHostEntry.HostName.ToString();
            IPAddress first_ip = ipHostEntry.AddressList.Where(p => p.AddressFamily == family).FirstOrDefault(p => string.IsNullOrWhiteSpace(start_with) || p.ToString().StartsWith(start_with)); //找出第一个IPV4地址
            string ip = first_ip == null ? string.Empty : first_ip.ToString();
            //ipHostEntry = Dns.GetHostEntry(ip);
            hostName = ipHostEntry.HostName.ToString();
            return ip;
        }

        /// <summary>
        /// 设置自动启动
        /// </summary>
        /// <param name="autoStart"></param>
        /// <param name="appTag"></param>
        /// <param name="appPath"></param>
        /// <param name="allUsers"></param>
        public static void SetAutoStart(bool autoStart, string appTag, string appPath, bool allUsers)
        {
            string subKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            appPath = string.Format(@"""{0}""", appPath);
            //RegistryKey key = all_users ? Registry.LocalMachine.CreateSubKey(subKey) : Registry.CurrentUser.CreateSubKey(subKey);
            RegistryKey key_AllUsers = Registry.LocalMachine.CreateSubKey(subKey);
            RegistryKey key_CurrentUser = Registry.CurrentUser.CreateSubKey(subKey);
            key_AllUsers.SetValue(appTag, autoStart && allUsers ? appPath : string.Empty);
            key_CurrentUser.SetValue(appTag, autoStart && !allUsers ? appPath : string.Empty);
        }

        /// <summary>
        /// 将当前时间添加到信息中
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string AddTimeToMessage(object obj)
        {
            return string.Format("{0} ==> {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), (string)obj);
        }

        /// <summary>
        /// 判断操作系统是否为XP或2003
        /// </summary>
        public static bool IsXpOr2003()
        {
            //获取操作系统信息与版本号
            OperatingSystem os = Environment.OSVersion;
            Version version = os.Version;

            //2000与XP的版本号分别为Windows NT 5.0 5.1，其中5为主要版本号，0、1为次要版本号
            return (os.Platform == PlatformID.Win32NT) && version.Major == 5 && version.Minor < 2;
            //if (os.Platform == PlatformID.Win32NT)
            //    if ((version.Major == 5) && (version.Minor != 0))
            //        return true;
            //    else
            //        return false;
            //else
            //    return false;
        }

        ///// <summary>
        ///// 泛型类型转换
        ///// </summary>
        ///// <param name="type">要转换的基础类型</param>
        ///// <param name="source">要转换的值</param>
        ///// <returns>返回转换后的实体类对象</returns>
        //public static object Convert(Type type, object source)
        //{
        //    //假如原数据为空（或数据库空值），返回类型的新实例
        //    if (source == null || source.GetType().Name.Equals("DBNull"))
        //    {
        //        //假如是值类型，生成新实例，否则返回null
        //        if (type.IsValueType)
        //            return Activator.CreateInstance(type);
        //        else
        //            return null;
        //    }

        //    //泛型Nullable判断，取其中的类型
        //    if (type.IsGenericType)
        //        type = type.GetGenericArguments()[0];

        //    //反射获取TryParse方法
        //    return System.Convert.ChangeType(source, type);
        //}

        ///// <summary>
        ///// 更新并返回TcpClient的连接状态
        ///// </summary>
        ///// <returns>假如处于连接状态，返回true，否则返回false</returns>
        //public static bool IsSocketConnected(TcpClient client)
        //{
        //    //假如TcpClient对象为空
        //    if (client == null || client.Client == null)
        //        return false;

        //    Socket socket = client.Client;
        //    return (!socket.Poll(1000, SelectMode.SelectRead) || socket.Available != 0) && socket.Connected;
        //}

        /// <summary>
        /// 将可枚举的集合转换为DataTable
        /// </summary>
        /// <typeparam name="T">要迭代的具体对象类型</typeparam>
        /// <param name="collection">待转换的集合对象</param>
        /// <returns>返回一个DataTable</returns>
        public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取当前日期时间的两种格式化字符串
        /// </summary>
        /// <param name="fullDate">当前日期的完整格式，yyyyMMdd格式</param>
        /// <param name="localDateTime">本地格式的当前完整日期时间，精确到秒（yyyy年M月d日 H时m分s秒）</param>
        public static void GetDateTimeString(out string fullDate, out string localDateTime)
        {
            DateTime dateTimeNow = DateTime.Now;
            fullDate = string.Format("{0:yyyyMMdd}", dateTimeNow); //完整日期，yyyyMMdd格式
            localDateTime = string.Format("{0:yyyy年M月d日 H时m分s秒}", dateTimeNow); //本地格式的日期时间，精确到秒
        }

        ///// <summary>
        ///// 将汉字转换为拼音首字母（引用微软官方类库）
        ///// </summary>
        ///// <param name="str">待转换的字符串</param>
        ///// <returns>返回汉字的拼音首字母，字母或数字原样返回</returns>
        //public static string ConvertToPinYin(string str)
        //{
        //    string PYstr = string.Empty;
        //    foreach (char item in str.ToCharArray())
        //    {
        //        //假如为汉字，提取大写拼音首字母
        //        if (ChineseChar.IsValidChar(item))
        //        {
        //            ChineseChar cc = new ChineseChar(item);
        //            //PYstr += string.Join("", cc.Pinyins.ToArray());
        //            PYstr += cc.Pinyins[0].Substring(0, 1).ToUpper();
        //            //PYstr += cc.Pinyins[0].Substring(0, cc.Pinyins[0].Length - 1).Substring(0, 1).ToLower();
        //        }
        //        //假如不是汉字，原样返回
        //        else
        //            PYstr += item.ToString();
        //    }
        //    return PYstr;
        //}

        /// <summary>
        /// 合并字符串数组
        /// </summary>
        /// <param name="firstArray">第一个字符串数组，假如为空或长度为0，直接返回第二个字符串数组</param>
        /// <param name="secondArray">第二个字符串数组，假如为空或长度为0，直接返回第一个字符串数组</param>
        /// <returns>返回合并后的结果</returns>
        public static string[] MergeStringArrays(string[] firstArray, string[] secondArray)
        {
            if (firstArray == null || firstArray.Length == 0)
                return secondArray;
            else if (secondArray == null || secondArray.Length == 0)
                return firstArray;

            string[] result = new string[firstArray.Length + secondArray.Length];
            firstArray.CopyTo(result, 0);
            secondArray.CopyTo(result, firstArray.Length);
            return result;
        }

        /// <summary>
        /// 获取字符串形式的随机代码（额外代码 + yyyyMMddHHmmss + 6位随机数）
        /// </summary>
        /// <param name="subInfo">放在数字前的额外代码</param>
        /// <returns>返回代码的字符串形式</returns>
        public static string CreateRandomCode(string subInfo)
        {
            DateTime dateTime = DateTime.Now;
            Random random = new Random(unchecked((int)dateTime.Ticks)); //以当前时间的计时周期数作为随机种子
            return string.Format("{0}{1:yyyyMMddHHmmss}{2}", subInfo, dateTime, random.Next(0, 1000000).ToString().PadLeft(6, '0')); //随机数控制在0-999999之间，假如随机数不到6位，用字符0填充
        }

        /// <summary>
        /// 获取字符串形式的随机代码（yyyyMMddHHmmss + 6位随机数）
        /// </summary>
        /// <returns>返回代码的字符串形式</returns>
        public static string CreateRandomCode()
        {
            return CreateRandomCode(string.Empty);
        }

        ///// <summary>
        ///// 将数据表写入指定名称的XML文件
        ///// </summary>
        ///// <param name="dt">储存数据的数据表</param>
        ///// <param name="fileName">XML文件名(不包含文件类型后缀)</param>
        //public static void WriteDataTableToXML(DataTable dt, string fileName)
        //{
        //    string tableName = dt.TableName;//DataTable名
        //    string path = System.Windows.Forms.Application.StartupPath + "\\Scanned Points";//XML文件路径
        //    string fullPath = string.Format("{0}\\{1}.xml", path, fileName);//包含文件名的路径
        //    //检测目录是否存在，不存在则创建
        //    if (!System.IO.Directory.Exists(path))
        //        System.IO.Directory.CreateDirectory(path);
        //    //假如XML文件存在，添加数据，否则创建并添加
        //    if (System.IO.File.Exists(fullPath))
        //    {
        //        System.IO.StringWriter writer = new System.IO.StringWriter();
        //        dt.WriteXml(writer);//将表的Xml写入Stream中
        //        XmlDocument docPrevLog = new XmlDocument();//旧Xml
        //        XmlDocument docNewLog = new XmlDocument();//新Xml
        //        docPrevLog.LoadXml(System.IO.File.ReadAllText(Base.XMLPath));
        //        docNewLog.LoadXml(writer.ToString());
        //        XmlNode root = docPrevLog.DocumentElement;//获取XML根节点
        //        //向XML根节点中添加新增子节点
        //        foreach (XmlNode node in docNewLog.DocumentElement.ChildNodes)
        //        {
        //            XmlNode tempNode = docPrevLog.ImportNode(node, true);
        //            root.AppendChild(tempNode);
        //        }
        //        docPrevLog.Save(fullPath);//保存XML文件
        //    }
        //    else
        //        dt.WriteXml(fullPath, XmlWriteMode.IgnoreSchema);
        //}
    }
}
