using CommonLib.Extensions;
using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CommonLib.Clients
{
    ///// <summary>
    ///// CRC参数的设置，当前设置为32位的CRC。如果需要切换到其他的CRC标准，只需要简单地修改这些参数即可。
    ///// The CRC parameters. Currently configured for 32 bit CRC. Simply modify these to switch to another CRC standard.
    ///// 多项式：CRC32校验是一种循环冗余校验，使用一个特定的多项式进行计算。CRC32_POLYNOMIAL常量指定了这个多项式的值。
    ///// 初始值：CRC32校验的初始值，通常为全1，即0xFFFFFFFF。
    ///// 最终异或值：CRC32校验计算完成后，会将计算结果与一个特定的值进行异或，以得到最终的校验值。CRC32_FINAL_XOR_VALUE常量指定了这个特定的值。
    ///// 位宽：CRC32校验的位宽，即校验值的位数。在这个实现中，位宽为32位。
    ///// 空值：CRC32校验中，如果要计算的数据为空，那么校验值就是CRC32_NULL常量指定的值。
    ///// </summary>
    //public struct CRC32Parameters
    //{
    //    /// <summary>
    //    /// CRC32校验中使用的多项式
    //    /// </summary>
    //    public const uint CRC32_POLYNOMIAL = 0x04c11db7;

    //    /// <summary>
    //    /// CRC32校验的初始值
    //    /// </summary>
    //    public const uint CRC32_INITIAL_REMAINDER = 0xFFFFFFFF;

    //    /// <summary>
    //    /// CRC32校验的最终异或值
    //    /// </summary>
    //    public const uint CRC32_FINAL_XOR_VALUE = 0xFFFFFFFF;

    //    /// <summary>
    //    /// CRC32校验的位宽
    //    /// </summary>
    //    public const int CRC32_WIDTH = 32;

    //    /// <summary>
    //    /// CRC32校验的空值
    //    /// </summary>
    //    public const uint CRC32_NULL = 0;
    //}

    /// <summary>
    /// CRC参数的设置，当前设置为32位的CRC。如果需要切换到其他的CRC标准，只需要简单地修改这些参数即可。
    /// The CRC parameters. Currently configured for 32 bit CRC. Simply modify these to switch to another CRC standard.
    /// 多项式：CRC32校验是一种循环冗余校验，使用一个特定的多项式进行计算。CRC32_POLYNOMIAL常量指定了这个多项式的值。
    /// 初始值：CRC32校验的初始值，通常为全1，即0xFFFFFFFF。
    /// 最终异或值：CRC32校验计算完成后，会将计算结果与一个特定的值进行异或，以得到最终的校验值。CRC32_FINAL_XOR_VALUE常量指定了这个特定的值。
    /// 位宽：CRC32校验的位宽，即校验值的位数。在这个实现中，位宽为32位。
    /// 空值：CRC32校验中，如果要计算的数据为空，那么校验值就是CRC32_NULL常量指定的值。
    /// </summary>
    public class CRC32Parameters
    {
        /// <summary>
        /// CRC32校验中使用的多项式
        /// </summary>
        public uint CRC32_POLYNOMIAL { get; set; } = 0x04c11db7;

        /// <summary>
        /// CRC32校验的初始值
        /// </summary>
        public uint CRC32_INITIAL_REMAINDER { get; set; } = 0xFFFFFFFF;

        /// <summary>
        /// CRC32校验的最终异或值
        /// </summary>
        public uint CRC32_FINAL_XOR_VALUE { get; set; } = 0xFFFFFFFF;

        /// <summary>
        /// CRC32校验的位宽
        /// </summary>
        public int CRC32_WIDTH { get; set; } = 32;

        /// <summary>
        /// CRC32校验的空值
        /// </summary>
        public uint CRC32_NULL { get; set; } = 0;

        /// <summary>
        /// 默认构造器
        /// </summary>
        public CRC32Parameters() { }

        /// <summary>
        /// 初始化CRC32计算参数的新实体
        /// </summary>
        /// <param name="polynomial">CRC32校验中使用的多项式</param>
        /// <param name="initialRemainder">CRC32校验的初始值</param>
        /// <param name="finalXorValue">CRC32校验的最终异或值</param>
        /// <param name="width">CRC32校验的位宽</param>
        /// <param name="nullValue">CRC32校验的空值</param>
        public CRC32Parameters(uint polynomial, uint initialRemainder, uint finalXorValue, int width, uint nullValue)
        {
            CRC32_POLYNOMIAL = polynomial;
            CRC32_INITIAL_REMAINDER = initialRemainder;
            CRC32_FINAL_XOR_VALUE = finalXorValue;
            CRC32_WIDTH = width;
            CRC32_NULL = nullValue;
        }
    }

    /// <summary>
    /// 用于CRC32校验计算的实体类
    /// </summary>
    public class CRC32
    {
        #region 私有变量
        private readonly CRC32Parameters mParams; //CRC32校验计算的参数实体
        private uint mLength; //number of bytes added

        /// <summary>
        /// 数据除以生成多项式后得到的余数
        /// this remainder is used in the Add() method. It is reset by GetCRC32()
        /// </summary>
        private uint mRemainder;

        /// <summary>
        /// a local flag indicating if the CRC table was initialized
        /// </summary>
        private /*static */readonly bool sCRC32IsInitialized;

        /// <summary>
        /// An array containing the pre-computed intermediate result for each possible byte of input. This is used to speed up the computation.
        /// </summary>
        private /*static */readonly uint[] sCRCTable = new uint[256];
        #endregion

        #region 属性
        /// <summary>
        /// CRC32校验计算的参数实体
        /// </summary>
        public CRC32Parameters CRC32Parameters { get { return mParams; } }

        /// <summary>
        /// The number of bytes added so far.
        /// </summary>
        public uint Length { get { return mLength; } }
        #endregion

        #region 构造器
        /// <summary>
        /// The constructor.
        /// Initialize the CRC table with default CRC32 parameters.
        /// </summary>
        public CRC32() : this(null) { }

        /// <summary>
        /// The constructor.
        /// Initialize the CRC table with given CRC32 parameters.
        /// </summary>
        /// <param name="crc32Params">给定的CRC32参数实体，假如为空则重新初始化并使用默认值</param>
        public CRC32(CRC32Parameters crc32Params)
        {
            mParams = crc32Params ?? new CRC32Parameters();
            if (!sCRC32IsInitialized)
            {
                // 初始化CRC表，预先计算每个可能输入字节的中间结果，以加快计算速度。
                // Compute the table of CRC remainders for all possible bytes, 256 values representing ASCII character codes.
                for (uint iCodes = 0; iCodes <= 0xFF; iCodes++)
                {
                    //sCRCTable[iCodes] = Reflect(iCodes, 8) << (CRC32Parameters.CRC32_WIDTH - 8);
                    //for (byte iPos = 0; iPos < 8; iPos++)
                    //{
                    //    sCRCTable[iCodes] = (sCRCTable[iCodes] << 1) ^ ((sCRCTable[iCodes] & (1u << 31)) != 0 ? CRC32Parameters.CRC32_POLYNOMIAL : CRC32Parameters.CRC32_NULL);
                    //}
                    sCRCTable[iCodes] = Reflect(iCodes, 8) << (mParams.CRC32_WIDTH - 8);
                    for (byte iPos = 0; iPos < 8; iPos++)
                    {
                        sCRCTable[iCodes] = (sCRCTable[iCodes] << 1) ^ ((sCRCTable[iCodes] & (1u << 31)) != 0 ? mParams.CRC32_POLYNOMIAL : mParams.CRC32_NULL);
                    }

                    sCRCTable[iCodes] = Reflect(sCRCTable[iCodes], 32);
                }

                sCRC32IsInitialized = true;
            }

            Clear();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~CRC32()
        {
            // has nothing to do.
        }
        #endregion

        /// <summary>
        /// 验证输入的CRC32校验值，判断是否符合当前已添加的所有数值所计算出的CRC32值
        /// </summary>
        /// <param name="crc32">输入的待验证的CRC32校验值</param>
        /// <returns></returns>
        public bool CheckCRC32(uint crc32)
        {
            return GetCRC32() == crc32;
        }

        /// <summary>
        /// 验证以16进制字符串格式输入（可不包含空格）的byte序列是否符合当前参数的CRC32校验，其中CRC32校验值在数组末尾，CRC32校验值在数组中占据的长度由给定参数指定
        /// </summary>
        /// <param name="input">添加要参与进行校验值评估的byte序列, 以16进制字符串格式输入（可不包含空格）</param>
        /// <param name="crcLen">数组内CRC32校验值占据的byte数量</param>
        /// <returns></returns>
        public bool CheckCRC32(string input, int crcLen)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            return CheckCRC32(HexHelper.HexString2Bytes_NoSpaces(input), crcLen);
        }

        /// <summary>
        /// 验证输入的byte数组是否符合当前参数的CRC32校验，其中CRC32校验值在数组末尾，CRC32校验值在数组中占据的长度由给定参数指定
        /// </summary>
        /// <param name="byteArray">添加要参与进行校验值评估的数值数组</param>
        /// <param name="crcLen">数组内CRC32校验值占据的byte数量</param>
        /// <returns></returns>
        public bool CheckCRC32(IEnumerable<byte> byteArray, int crcLen)
        {
            //byte集合长度应大于crc32占据的长度
            if (byteArray == null || byteArray.Count() < crcLen)
                return false;
            //byte集合中处crc32外的数据长度
            int dataLen = byteArray.Count() - crcLen;
            //根据数据算出的CRC32校验值，以及原byte集合中提供的CRC32校验值
            uint assumed = GetCRC32(byteArray, dataLen), checksum = byteArray.Skip(dataLen).ToUInt32();
            return assumed.Equals(checksum);
        }

        /// <summary>
        /// Compute the current CRC32 value, which is the 32 bit CRC for data which are previously added by add().
        /// </summary>
        /// <returns></returns>
        public uint GetCRC32()
        {
            //return mRemainder ^ CRC32Parameters.CRC32_FINAL_XOR_VALUE;
            return mRemainder ^ mParams.CRC32_FINAL_XOR_VALUE;
        }

        /// <summary>
        /// Compute a 32 bit CRC for the given data array.
        /// </summary>
        /// <param name="theDataPtr">添加要参与进行校验值计算的数值集合</param>
        /// <param name="theLength">数组内参加计算的byte数量</param>
        /// <returns></returns>
        //public uint GetCRC32(byte[] theDataPtr, int theLength)
        public uint GetCRC32(IEnumerable<byte> theDataPtr, int theLength)
        {
            Clear();
            return Add(theDataPtr, theLength);
        }

        /// <summary>
        /// 将输入的数值在位域上进行首尾方向上的调换（范围是从第1位到指定长度位置的位）
        /// Performs a reflection on a value: swaps bit 7 with 0, 6 with 1 and so on. Reflection is a requirement to conform to the official CRC-32 standard.
        /// </summary>
        /// <param name="value">即将进行位域调换的值</param>
        /// <param name="theBits">进行位域调换部分的位的长度</param>
        /// <returns></returns>
        private uint Reflect(uint value, byte theBits)
        {
            uint reflection = 0;

            // Reflect the data about the center bit.
            // Swap bit 0 for bit 7 bit 1 For bit 6, etc....
            //for (byte bit = 0; bit < theBits; ++bit)
            for (int bit = 1; bit < (theBits + 1); bit++)
            {
                // If the LSB bit is set, set the reflection of it.
                if ((value & 1) != 0)
                {
                    //reflection |= (uint)(1 << (theBits - bit - 1));
                    reflection |= 1u << (theBits - bit);
                }
                value >>= 1;
            }

            return reflection;
        }

        ///// <summary>
        ///// Add some more data to compute a 32 bit CRC of several data.
        ///// Note: you should not use this function to add little endian integers that are transmitted in big endian network byte order.
        ///// Use addInt() instead.
        ///// </summary>
        ///// <param name="theDataPtr">添加要参与进行校验值计算的数值数组</param>
        ///// <param name="theLength">数组内参加计算的byte数量</param>
        ///// <returns></returns>
        //public uint Add(byte[] theDataPtr, int theLength)
        //{
        //    // check parameter. Return default if there is nothing to do.
        //    if (theDataPtr == null || theLength == 0)
        //    {
        //        //return 0;
        //        //return CRC32Parameters.CRC32_NULL;
        //        return mParams.CRC32_NULL;
        //    }

        //    // 用多项式除传入的数值，每次一个字节
        //    // Divide the message by the polynomial, a byte at time.
        //    for (int i = 0; i < theLength; ++i)
        //    {
        //        mRemainder = (mRemainder >> 8) ^ sCRCTable[(mRemainder & 0xFF) ^ theDataPtr[i]];
        //    }

        //    // The final mRemainder is the CRC result.
        //    mLength += (uint)theLength;
        //    //return mRemainder ^ CRC32Parameters.CRC32_FINAL_XOR_VALUE;
        //    return mRemainder ^ mParams.CRC32_FINAL_XOR_VALUE;
        //}

        /// <summary>
        /// Add some more data to compute a 32 bit CRC of several data.
        /// Note: you should not use this function to add little endian integers that are transmitted in big endian network byte order.
        /// Use addInt() instead.
        /// </summary>
        /// <param name="theDataPtr">添加要参与进行校验值计算的数值集合</param>
        /// <param name="theLength">数组内参加计算的byte数量</param>
        /// <returns></returns>
        public uint Add(IEnumerable<byte> theDataPtr, int theLength)
        {
            // check parameter. Return default if there is nothing to do.
            if (theDataPtr == null || theLength == 0)
            {
                //return 0;
                //return CRC32Parameters.CRC32_NULL;
                return mParams.CRC32_NULL;
            }

            // 用多项式除传入的数值，每次一个字节
            // Divide the message by the polynomial, a byte at time.
            for (int i = 0; i < theLength; ++i)
            {
                mRemainder = (mRemainder >> 8) ^ sCRCTable[(mRemainder & 0xFF) ^ theDataPtr.ElementAt(i)];
            }

            // The final mRemainder is the CRC result.
            mLength += (uint)theLength;
            //return mRemainder ^ CRC32Parameters.CRC32_FINAL_XOR_VALUE;
            return mRemainder ^ mParams.CRC32_FINAL_XOR_VALUE;
        }

        /// <summary>
        /// 重置校验后的余数到初始值
        /// Reset the checksum to initial stage.
        /// </summary>
        public void Clear()
        {
            mLength = 0;
            //mRemainder = CRC32Parameters.CRC32_INITIAL_REMAINDER;
            mRemainder = mParams.CRC32_INITIAL_REMAINDER;
        }

    }
}
