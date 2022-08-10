using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function
{
    /// <summary>
    /// 16进制操作相关操作类
    /// </summary>
    public static class HexHelper
    {
        #region 获取倒写的16进制BCD码
        /// <summary>
        /// 获取倒写的16进制BCD码，可根据最小长度补0、添加偏移量
        /// </summary>
        /// <param name="hexString">电表编码</param>
        /// <param name="minimum">最小长度，应为偶数，假如为0则忽略</param>
        /// <param name="shift">向每项BCD码添加的偏移量，假如为0则忽略</param>
        /// <returns>返回电表编码的16进制倒写形式</returns>
        public static string GetReverseHexString(string hexString, int minimum, int shift)
        {
            if (string.IsNullOrWhiteSpace(hexString))
                return string.Empty;

            //假如最小长度为0，不补0
            if (minimum > 0)
                hexString = hexString.PadLeft(minimum, '0');
            StringBuilder builder = new StringBuilder();
            string part;
            for (int i = (hexString.Length / 2) - 1; i >= 0; i--)
            {
                part = hexString.Substring(2 * i, 2);
                //假如偏移量不为0，计算偏移后的16进制BCD码
                if (shift != 0)
                    part = (Convert.ToInt32(part, 16) + shift).ToString("X2");
                builder.Append(part).Append(" ");
            }

            return builder.ToString().Trim();
        }

        /// <summary>
        /// 获取倒写的16进制BCD码，可根据最小长度补0
        /// </summary>
        /// <param name="hexString">电表编码</param>
        /// <param name="minimum">最小长度，应为偶数，假如为0则忽略</param>
        /// <returns>返回电表编码的16进制倒写形式</returns>
        public static string GetReverseHexString(string hexString, int minimum)
        {
            return HexHelper.GetReverseHexString(hexString, minimum, 0);
        }

        /// <summary>
        /// 获取倒写的16进制BCD码，给每个BCD码增加一个偏移量
        /// </summary>
        /// <param name="hexString">电表编码</param>
        /// <param name="shift">偏移量</param>
        /// <returns>返回电表编码的16进制倒写形式</returns>
        public static string GetReverseHexString_Shift(string hexString, int shift)
        {
            return HexHelper.GetReverseHexString(hexString, 0, shift);
        }
        #endregion

        #region byte转16进制
        /// <summary>
        /// 将byte数组转换为带空格的字符串
        /// </summary>
        /// <param name="bytes">待转换的byte数组</param>
        /// <returns>返回字符串</returns>
        public static string ByteArray2HexString(IEnumerable<byte> bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.Append(" ").Append(b.ToString("X2")); //每次都输出两位大写16进制数
            return builder.ToString().Trim();
        }

        /// <summary>
        /// byte转换为字符串
        /// </summary>
        /// <param name="b">待转换的byte值</param>
        /// <returns>返回字符串</returns>
        public static string Byte2HexString(byte b)
        {
            return b.ToString("X2");
        }

        /// <summary>
        /// 将byte数组转换为字符串数组（一一对应）
        /// </summary>
        /// <param name="bytes">待转换的byte数组</param>
        /// <returns>转换后的字符串</returns>
        public static string[] ByteArray2HexStringArray(IEnumerable<byte> bytes)
        {
            return bytes.Select(b => b.ToString("X2")).ToArray();
        }
        #endregion

        #region 16进制转byte
        /// <summary>
        /// 将16进制格式字符串数组转换为byte数组
        /// </summary>
        /// <param name="hexStrings">16进制格式字符串数组，如[ "FE", "FE", ... ]</param>
        /// <returns>返回byte数组</returns>
        public static byte[] HexStringArray2Bytes(IEnumerable<string> hexStrings)
        {
            if (hexStrings == null || hexStrings.Count() == 0)
                return null;

            return hexStrings.Select(p => string.IsNullOrWhiteSpace(p) ? (byte)0 : Convert.ToByte(p, 16)).ToArray();
        }

        /// <summary>
        /// 将16进制格式字符串转换为byte数组
        /// </summary>
        /// <param name="hexString">16进制格式字符串，如"FE FE FE ..."</param>
        /// <returns>返回byte数组</returns>
        public static byte[] HexString2Bytes(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
                return null;

            return HexStringArray2Bytes(hexString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// 将16进制字符串转为byte数组
        /// </summary>
        /// <param name="hexString">待转换的16进制字符串，假如为空白则返回null</param>
        /// <returns>返回16进制数组</returns>
        public static byte[] HexString2Bytes_Alternate(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
                return null;

            hexString = hexString.Replace(" ", string.Empty);
            byte[] bytes = new byte[hexString.Length / 2];
            int index = 0;
            for (int i = 0; i < hexString.Length; i += 2)
            {
                bytes[index] = Convert.ToByte(hexString.Substring(i, 2), 16);
                index++;
            }

            return bytes;
        }
        #endregion

        #region 模256校验码与验证（普通电表）
        /// <summary>
        /// 根据16进制格式字符串计算各字节的模256（当输入字符串为空时为0）
        /// </summary>
        /// <param name="hexString">16进制格式字符串，如"FE FE ..."</param>
        /// <returns>返回模256</returns>
        public static byte GetModel256(string hexString)
        {
            return GetModel256(HexString2Bytes(hexString));
        }

        /// <summary>
        /// 计算各字节和的模256（当没有输入时为0）
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <returns>返回模256</returns>
        public static byte GetModel256(IEnumerable<byte> bytes)
        {
            int sum = 0;
            foreach (byte b in bytes)
                if (b != 254) //当不是FE时
                    sum += b;

            return (byte)(sum % 256);
        }

        /// <summary>
        /// 验证16进制字符串中的模256校验码
        /// </summary>
        /// <param name="hexString">待验证的16进制字符串</param>
        /// <returns></returns>
        public static bool ValidateCommandModel256(string hexString)
        {
            return ValidateCommandModel256(HexString2Bytes(hexString));
        }

        /// <summary>
        /// 验证各字节中的模256校验码
        /// </summary>
        /// <param name="bytes">待验证的byte数组</param>
        /// <returns></returns>
        public static bool ValidateCommandModel256(IEnumerable<byte> bytes)
        {
            int length = bytes.Count();
            if (bytes == null || length < 3)
                return false;

            byte[] front = bytes.Take(length - 2).ToArray(); //校验码前的字节
            byte model = bytes.ElementAt(length - 2); //命令中的校验码
            byte validate = GetModel256(front); //计算出的实际校验码
            return model == validate; //校验码比对
        }
        #endregion

        #region CRC16校验码与验证（MODBUS电表）
        /// <summary>
        /// CRC16校验码生成
        /// </summary>
        /// <param name="data">待计算校验码的byte数组</param>
        /// <returns>返回字节数组</returns>
        public static byte[] GetCRC16(IEnumerable<byte> data)
        {
            if (data == null || data.Count() == 0)
                return new byte[0];
            byte[] outdata = new byte[2];
            #region CRC16码表
            byte[] auchCRCHi = {
0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
};
            byte[] auchCRCLow = {
0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3, 0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26, 0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5, 0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C, 0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80, 0x40
};
            #endregion
            byte crcHi = 0xff;
            byte crcLow = 0xff;
            //int CRC_LEN = 2;
            int CRC_LEN = 0; //数组中原有CRC校验码的数量
            for (int i = 0; i < data.Count() - CRC_LEN; i++)
            {

                int crcIndex = crcHi ^ data.ElementAt(i);
                crcHi = (byte)(crcLow ^ auchCRCHi[crcIndex]);
                crcLow = auchCRCLow[crcIndex];
            }
            outdata[0] = crcHi;
            outdata[1] = crcLow;
            //outdata[0] = crcLow;
            //outdata[1] = crcHi;
            return (outdata);
        }

        /// <summary>
        /// CRC16校验码生成（字符串形式）
        /// </summary>
        /// <param name="data">待计算校验码的byte数组</param>
        /// <returns>返回16进制格式字符串</returns>
        public static string GetCRC16_String(IEnumerable<byte> data)
        {
            byte[] bytes = GetCRC16(data);
            if (bytes == null || bytes.Length == 0)
                return string.Empty;
            return string.Join(" ", bytes.Select(b => b.ToString("X2")).ToArray());
        }

        /// <summary>
        /// CRC16校验码生成
        /// </summary>
        /// <param name="hexString">待计算校验码的16进制字符串</param>
        /// <returns>返回字节数组</returns>
        public static byte[] GetCRC16(string hexString)
        {
            return GetCRC16(HexString2Bytes(hexString));
        }

        /// <summary>
        /// CRC16校验码生成（字符串形式）
        /// </summary>
        /// <param name="hexString">待计算校验码的16进制字符串</param>
        /// <returns>返回16进制格式字符串</returns>
        public static string GetCRC16_String(string hexString)
        {
            return GetCRC16_String(HexString2Bytes(hexString));
        }

        /// <summary>
        /// 验证CRC16校验码
        /// </summary>
        /// <param name="hexString">待验证的16进制字符串</param>
        /// <returns></returns>
        public static bool ValidateCommandCRC16(string hexString)
        {
            return ValidateCommandCRC16(HexString2Bytes(hexString));
        }

        /// <summary>
        /// 验证CRC16校验码
        /// </summary>
        /// <param name="bytes">待验证的byte数组</param>
        /// <returns></returns>
        public static bool ValidateCommandCRC16(IEnumerable<byte> bytes)
        {
            int length = bytes.Count();
            if (bytes == null || length < 3)
                return false;

            byte[] front = bytes.Take(length - 2).ToArray(); //校验码前的字节
            byte[] model = bytes.Skip(length - 2).ToArray(); //命令中的校验码
            byte[] validate = GetCRC16(front); //计算出的实际校验码
            return model.SequenceEqual(validate); //校验码比对
        }
        #endregion

        #region CRC32校验码与验证（GNSS）
        private const ulong CRC32_POLYNOMIAL = 0XEDB88320L; //CRC32多项式

        /// <summary>
        /// 计算单个值、单个字符(char)的CRC32校验值
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static ulong GetCRC32Value(int c)
        {
            ulong ulCRC = (ulong)c;
            for (int j = 8; j > 0; j--)
            {
                if (ulCRC % 2 == 1)
                    ulCRC = (ulCRC >> 1) ^ CRC32_POLYNOMIAL;
                else
                    ulCRC >>= 1;
            }
            return ulCRC;
        }

        /// <summary>
        /// 根据输入字符串获取CRC32校验值（字符串为空时为0）
        /// </summary>
        /// <param name="input">待获取校验值的字符串</param>
        /// <returns></returns>
        public static ulong CalculateBlockCRC32(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            ulong ulTemp1, ulTemp2, ulCRC = 0;
            int index = 0;
            while (index != input.Length)
            {
                ulTemp1 = (ulCRC >> 8) & 0x00FFFFFFL;
                ulTemp2 = GetCRC32Value(((int)ulCRC ^ input[index]) & 0xff);
                ulCRC = ulTemp1 ^ ulTemp2;
                index++;
            }
            return ulCRC;
        }

        /// <summary>
        /// LOG信息是否通过CRC32校验
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool IsGnssCRC32Verified(string message)
        {
            string[] temps = message.Split(new char[] { '#', '*' }, StringSplitOptions.RemoveEmptyEntries);
            //待检验的校验和（16进制）
            string to_be_checked = temps[1].ToUpper(), crc = CalculateBlockCRC32(temps[0]).ToString("X2");
            return to_be_checked.Equals(crc);
        }
        #endregion

        #region 字符串校验和（GNSS)
        /// <summary>
        /// 计算字符串的异或校验和（字符串为空时返回0）
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>假如字符串为null或空字符串，返回-1</returns>
        public static int GetStringChecksum(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;
            int result = input[0];
            for (int i = 1; i < input.Length; i++)
                result ^= input[i]; //每一个字符按位异或
            return result;
        }

        /// <summary>
        /// GNSS消息是否通过异或校验
        /// </summary>
        /// <param name="message">符合GNSS格式的消息字符串</param>
        /// <returns></returns>
        public static bool IsGnssChecksumVerified(string message)
        {
            string[] temps = message.Split(new char[] { '$', '*' }, StringSplitOptions.RemoveEmptyEntries);
            //待检验的校验和（16进制）
            int to_be_checked = Convert.ToInt32(temps[1], 16), checksum = GetStringChecksum(temps[0]);
            return to_be_checked == checksum;
        }
        #endregion

        #region 字符串每一位字符累加校验和
        /// <summary>
        /// 获取字符串中每一位字符的累加校验和（字符串为空时为0）
        /// </summary>
        /// <param name="input">待计算累积和字符串</param>
        /// <returns></returns>
        public static int GetStringSum(string input)
        {
            return string.IsNullOrEmpty(input) ? 0 : input.ToCharArray().Sum(c => c);
        }

        /// <summary>
        /// 获取字符串中每一位字符的累加校验和并添加到字符串末尾，校验和与字符串之间用指定分隔字符连接（字符串为空时返回空字符串）
        /// </summary>
        /// <param name="input">待计算累积和字符串</param>
        /// <param name="bridge">连接字符</param>
        /// <returns></returns>
        public static string GetStringSumResult(string input, char bridge)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            input += bridge;
            int sum = GetStringSum(input);
            return string.Format("{0}{1}", input, sum);
        }

        /// <summary>
        /// 获取字符串是否符合字符累加校验和的校验，分隔字符为指定字符
        /// </summary>
        /// <param name="input">待计算累积和字符串</param>
        /// <param name="bridge">连接字符</param>
        /// <returns></returns>
        public static bool IsStringSumVerified(string input, char bridge)
        {
            input = input ?? string.Empty;
            //假如没有分隔符则返回false
            int index = input.LastIndexOf(bridge);
            if (index <= 0 || index >= input.Length - 1)
                return false;
            string first = input.Substring(0, index + 1), last = input.Substring(index + 1, input.Length - index - 1);
            int sum = GetStringSum(first);
            return sum.ToString().Equals(last);
        }

        /// <summary>
        /// 获取字符串中每一位字符的累加校验和并添加到字符串末尾，校验和与字符串之间用半角逗号连接（字符串为空时返回空字符串）
        /// </summary>
        /// <param name="input">待计算累积和字符串</param>
        /// <returns></returns>
        public static string GetStringSumResult(string input)
        {
            return GetStringSumResult(input, ',');
        }

        /// <summary>
        /// 获取字符串是否符合字符累加校验和的校验，分隔字符为半角逗号
        /// </summary>
        /// <param name="input">待计算累积和字符串</param>
        /// <returns></returns>
        public static bool IsStringSumVerified(string input)
        {
            return IsStringSumVerified(input, ',');
        }
        #endregion
    }
}
