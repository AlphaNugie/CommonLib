using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 短UUID生成器
    /// </summary>
    public class ShortUuidGenerator
    {
        private static readonly Random random =
#if NET45_OR_GREATER
            new Random();
#elif NET9_0_OR_GREATER
            new();
#endif

        /// <summary>
        /// 生成指定位数的UUID，每一位在数字以及大小写ABCDEF中随机挑选
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GenerateShortUuid(uint len = 8)
        {
            if (len == 0)
                return string.Empty;
            const string chars = "0123456789abcdefABCDEF";
            var uuid = new char[len];
            for (int i = 0; i < len; i++)
            {
                uuid[i] = chars[random.Next(chars.Length)];
            }
            return new string(uuid);
        }
    }
}
