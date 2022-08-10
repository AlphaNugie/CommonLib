using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 按位转换为对应泛型（整型）值的实体类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomBitConverter<T> where T : IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        private readonly Type _baseType;
        private readonly bool[] _bits; //比特位的数组，从低位开始排列

        /// <summary>
        /// 比特序列对应的2进制字符串
        /// </summary>
        public string Binary { get { return GetBinary(); } }

        /// <summary>
        /// 比特序列对应的整型的值
        /// </summary>
        public T Value { get { return GetValue(); } }

        /// <summary>
        /// 初始化位转换实体类
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public CustomBitConverter()
        {
            _baseType = typeof(T);
            //匹配对应整型类型，假如找不到则报出异常
            if (_baseType == typeof(byte))
                _bits = new bool[8];
            else if (_baseType == typeof(sbyte))
                _bits = new bool[8];
            else if (_baseType == typeof(short))
                _bits = new bool[16];
            else if (_baseType == typeof(ushort))
                _bits = new bool[16];
            else if (_baseType == typeof(int))
                _bits = new bool[32];
            else if (_baseType == typeof(uint))
                _bits = new bool[32];
            else if (_baseType == typeof(long))
                _bits = new bool[64];
            else if (_baseType == typeof(ulong))
                _bits = new bool[64];
            else
                throw new ArgumentException("所对应泛型并非整型", nameof(T));
        }

        /// <summary>
        /// 设置对应索引位置比特位的值，低位索引位置为0，假如索引位置不存在则不进行任何操作
        /// </summary>
        /// <param name="value">设置的比特位的值，是0为false，除0之外均视为true</param>
        /// <param name="index">比特位的索引，从低位开始</param>
        public void SetBit(int value, int index)
        {
            SetBit(value != 0, index);
        }

        /// <summary>
        /// 设置对应索引位置比特位的值，低位索引位置为0，假如索引位置不存在则不进行任何操作
        /// </summary>
        /// <param name="value">设置的比特位的值</param>
        /// <param name="index">比特位的索引，从低位开始</param>
        public void SetBit(bool value, int index)
        {
            //假如要写入的位不存在，直接退出
            if (index < 0 || index >= _bits.Length)
                return;
            _bits[index] = value;
        }

        /// <summary>
        /// 获取从比特转来的整型值
        /// </summary>
        /// <returns></returns>
        public T GetValue()
        {
            //string binary = string.Join(string.Empty, _bits.Reverse().Select(bit => bit ? 1 : 0).ToArray());
            string binary = GetBinary();
            var value = default(T);
            //匹配对应整型类型，假如找不到则报出异常
            if (_baseType == typeof(byte))
                value = (T)(object)Convert.ToByte(binary, 2);
            else if (_baseType == typeof(sbyte))
                value = (T)(object)Convert.ToSByte(binary, 2);
            else if (_baseType == typeof(short))
                value = (T)(object)Convert.ToInt16(binary, 2);
            else if (_baseType == typeof(ushort))
                value = (T)(object)Convert.ToUInt16(binary, 2);
            else if (_baseType == typeof(int))
                value = (T)(object)Convert.ToInt32(binary, 2);
            else if (_baseType == typeof(uint))
                value = (T)(object)Convert.ToUInt32(binary, 2);
            else if (_baseType == typeof(long))
                value = (T)(object)Convert.ToInt64(binary, 2);
            else if (_baseType == typeof(ulong))
                value = (T)(object)Convert.ToUInt64(binary, 2);
            return value;
        }

        /// <summary>
        /// 获取从比特序列转换而来的2进制字符串
        /// </summary>
        /// <returns></returns>
        public string GetBinary()
        {
            return string.Join(string.Empty, _bits.Reverse().Select(bit => bit ? 1 : 0).ToArray());
        }

        /// <summary>
        /// 重置所有比特的值为false
        /// </summary>
        public void Reset()
        {
            Reset(false);
        }

        /// <summary>
        /// 重置所有比特为给定的值
        /// </summary>
        /// <param name="value">设置的比特位的值，是0为false，除0之外均视为true</param>
        public void Reset(int value)
        {
            Reset(value != 0);
        }

        /// <summary>
        /// 重置所有比特为给定的值
        /// </summary>
        /// <param name="value">重置的比特位的值</param>
        public void Reset(bool value)
        {
            for (int i = 0; i < _bits.Length; i++)
                _bits[i] = value;
        }
    }
}
