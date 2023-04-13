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
        /// 比特位的数组，从低位开始排列
        /// </summary>
        public bool[] Bits { get { return _bits; } }

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

        public void SetValue(T value)
        {
            if (_bits == null || _bits.Length == 0)
                return;
            string binary;
            //匹配对应整型类型，假如找不到则报出异常
            if (_baseType == typeof(byte))
                binary = Convert.ToString((byte)(object)value, 2).PadLeft(_bits.Length, '0');
            //else if (_baseType == typeof(sbyte) || _baseType == typeof(short))
            //    binary = Convert.ToString((short)(object)value, 2).PadLeft(_bits.Length, '0');
            //else if (_baseType == typeof(ushort) || _baseType == typeof(int))
            //    binary = Convert.ToString((int)(object)value, 2).PadLeft(_bits.Length, '0');
            //else if (_baseType == typeof(uint) || _baseType == typeof(long) || _baseType == typeof(ulong))
            //    binary = Convert.ToString((long)(object)value, 2).PadLeft(_bits.Length, '0');
            else if (_baseType == typeof(sbyte))
                binary = Convert.ToString((sbyte)(object)value, 2).PadLeft(_bits.Length, '0');
            else if (_baseType == typeof(short))
                binary = Convert.ToString((short)(object)value, 2).PadLeft(_bits.Length, '0');
            else if (_baseType == typeof(ushort))
                binary = Convert.ToString((ushort)(object)value, 2).PadLeft(_bits.Length, '0');
            else if (_baseType == typeof(int))
                binary = Convert.ToString((int)(object)value, 2).PadLeft(_bits.Length, '0');
            else if (_baseType == typeof(uint))
                binary = Convert.ToString((uint)(object)value, 2).PadLeft(_bits.Length, '0');
            else if (_baseType == typeof(long))
                binary = Convert.ToString((long)(object)value, 2).PadLeft(_bits.Length, '0');
            else if (_baseType == typeof(ulong))
                //binary = Convert.ToString(unchecked((long)(object)value), 2).PadLeft(_bits.Length, '0');
                binary = Convert.ToString(BitConverter.ToInt64(BitConverter.GetBytes((ulong)(object)value), 0), 2).PadLeft(_bits.Length, '0');
            else
                throw new ArgumentException("所对应泛型并非整型", nameof(T));
            for (int i = 0; i < _bits.Length; i++)
                _bits[i] = binary[_bits.Length - 1 - i] == '1';
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
            return _bits == null || _bits.Length == 0 ? default : GetValue(0, _bits.Length);
        }

        /// <summary>
        /// 获取从比特转来的整型值
        /// 获取从比特序列中任意索引处开始、长度若干的子比特序列转换而来的整型值
        /// </summary>
        /// <param name="index">子比特序列开始的索引位置</param>
        /// <param name="length">子比特序列的长度</param>
        /// <returns></returns>
        public T GetValue(int index, int length)
        {
            //string binary = GetBinary();
            string binary = GetBinary(index, length);
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
            return _bits == null || _bits.Length == 0 ? null : GetBinary(0, _bits.Length);
        }

        /// <summary>
        /// 获取从比特序列中任意索引处开始、长度若干的子比特序列转换而来的2进制字符串
        /// </summary>
        /// <param name="index">子比特序列开始的索引位置</param>
        /// <param name="length">子比特序列的长度</param>
        /// <returns></returns>
        public string GetBinary(int index, int length)
        {
            if (_bits == null || _bits.Length == 0) return null;
            index = index < 0 ? 0 : index;
            length = length + index > _bits.Length ? _bits.Length - index : length;
            //return string.Join(string.Empty, _bits.Reverse().Select(bit => bit ? 1 : 0).ToArray());
            //根据比特序列开始位置的索引以及持续的长度来提取比特序列
            var array = index == 0 && length == _bits.Length ? _bits.Reverse() : _bits.Skip(index).Take(length).Reverse();
            return string.Join(string.Empty, array.Select(bit => bit ? 1 : 0).ToArray());
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
