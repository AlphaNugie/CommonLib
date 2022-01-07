using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enums
{
    /// <summary>
    /// 与System.Drawing.Graphics的填充函数所关联的一系列形状枚举
    /// </summary>
    public enum ShapeEnum
    {
        /// <summary>
        /// 椭圆
        /// </summary>
        Ellipse = 0,

        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle,

        /// <summary>
        /// 封闭曲线
        /// </summary>
        ClosedCurve,

        /// <summary>
        /// 扇形（椭圆的一部分）
        /// </summary>
        Pie,

        /// <summary>
        /// 多边形
        /// </summary>
        Polygon
    }
}
