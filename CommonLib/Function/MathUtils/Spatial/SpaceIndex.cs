using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.MathUtils.Spatial
{

    /// <summary>
    /// 空间中网格立方体(GridCube)所在位置的索引
    /// </summary>
    public struct SpaceIndex : IComparable<SpaceIndex>
    {
        /// <summary>
        /// X坐标索引
        /// </summary>
        public int XIndex { get; set; }

        /// <summary>
        /// Y坐标索引
        /// </summary>
        public int YIndex { get; set; }

        /// <summary>
        /// Z坐标索引
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// 用给定的XYZ坐标轴索引初始化
        /// </summary>
        /// <param name="xi"></param>
        /// <param name="yi"></param>
        /// <param name="zi"></param>
        public SpaceIndex(int xi, int yi, int zi)
        {
            XIndex = xi;
            YIndex = yi;
            ZIndex = zi;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format($"XIndex: {XIndex}, YIndex: {YIndex}, ZIndex: {ZIndex}");
        }

        /// <summary>
        /// 将当前实例与另一实例相比较，并返回比较结果符号：-1 小于，0 相等，1 大于
        /// </summary>
        /// <param name="other">与当前实例比较的另一实例</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int CompareTo(SpaceIndex other)
        {
            int xcmp2 = XIndex.CompareTo(other.XIndex);
            int ycmp2 = YIndex.CompareTo(other.YIndex);
            int zcmp2 = ZIndex.CompareTo(other.ZIndex);
            //优先比较X索引，X索引相同再比较Y索引，Y相同则再比较Z索引
            if (xcmp2 != 0) return xcmp2;
            if (ycmp2 != 0) return ycmp2;
            return zcmp2;
        }
    }
}
