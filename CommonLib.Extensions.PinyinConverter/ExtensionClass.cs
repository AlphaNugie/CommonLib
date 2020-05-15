using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Extensions
{
    public static class ExtensionClass
    {
        /// <summary>
        /// 获取一段字符串中汉字的拼音首字母
        /// </summary>
        /// <param name="str">待转换的字符串</param>
        /// <returns>返回汉字的拼音首字母，字母或数字原样返回</returns>
        public static string GetPinyinInitials(string str)
        {
            string PYstr = string.Empty;
            foreach (char item in str.ToCharArray())
            {
                //假如为汉字，提取大写拼音首字母
                if (ChineseChar.IsValidChar(item))
                {
                    ChineseChar cc = new ChineseChar(item);
                    //PYstr += string.Join("", cc.Pinyins.ToArray());
                    PYstr += cc.Pinyins[0].Substring(0, 1).ToUpper();
                    //PYstr += cc.Pinyins[0].Substring(0, cc.Pinyins[0].Length - 1).Substring(0, 1).ToLower();
                }
                //假如不是汉字，原样返回
                else
                    PYstr += item.ToString();
            }
            return PYstr;
        }
    }
}
