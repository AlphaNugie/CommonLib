using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enums
{
    /// <summary>
    /// 字节序的类型
    /// </summary>
    public enum Endianness
    {
        /// <summary>
        /// 大字节/最高有效字节(Most Significant Byte)靠前
        /// </summary>
        BigEndian = 1,

        /// <summary>
        /// 小字节/最低有效字节(Least Significant Byte)靠前
        /// </summary>
        LittleEndian = 2,
    }
}
