using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enums
{
    /// <summary>
    /// 整型类型的标志
    /// </summary>
    public enum IntFlag
    {
        Byte = 4608,

        SByte = 5120,

        Int16 = 6144,

        UInt16 = 4608,

        Int32 = 4096,

        UInt32 = 4608,

        Int64 = 4096,

        UInt64 = 4608
    }

    //public static class IntFlagExt
    //{
    //    public IntFlag GetIntFlagByType(Type _baseType)
    //    {
    //        //匹配对应整型类型，假如找不到则报出异常
    //        if (_baseType == typeof(byte))
    //            _bits = new bool[8];
    //        else if (_baseType == typeof(sbyte))
    //            _bits = new bool[8];
    //        else if (_baseType == typeof(short))
    //            _bits = new bool[16];
    //        else if (_baseType == typeof(ushort))
    //            _bits = new bool[16];
    //        else if (_baseType == typeof(int))
    //            _bits = new bool[32];
    //        else if (_baseType == typeof(uint))
    //            _bits = new bool[32];
    //        else if (_baseType == typeof(long))
    //            _bits = new bool[64];
    //        else if (_baseType == typeof(ulong))
    //            _bits = new bool[64];
    //        else
    //            throw new ArgumentException("所对应泛型并非整型", nameof(T));
    //    }
    //}
}
