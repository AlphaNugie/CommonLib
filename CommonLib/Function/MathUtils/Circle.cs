using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.MathUtils
{
    /// <summary>
    /// 三维空间中二维圆形实体类
    /// </summary>
    public class Circle
    {
        /// <summary>
        /// 圆心X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 圆心Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 圆心Z坐标
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// 半径长度
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public Circle() { }

        /// <summary>
        /// 用给定的圆心XYZ坐标以及半径长度初始化
        /// </summary>
        /// <param name="x">圆心X坐标</param>
        /// <param name="y">圆心Y坐标</param>
        /// <param name="z">圆心Z坐标</param>
        /// <param name="radius">半径长度</param>
        public Circle(double x, double y, double z, double radius)
        {
            X = x;
            Y = y;
            Z = z;
            Radius = radius;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format($"Center X: {X}, Center Y: {Y}, Center Z: {Z}, Radius: {Radius}");
        }
    }

    /// <summary>
    /// 圆形扩展类
    /// </summary>
    public static class CircleExtension
    {
        /// <summary>
        /// 计算多个圆形对象的XYZ坐标以及半径平均值
        /// </summary>
        /// <param name="circles">圆形对象集合</param>
        /// <returns></returns>
        public static Circle Average(this IEnumerable<Circle> circles)
        {
            if (circles == null) throw new ArgumentNullException(nameof(circles), "圆对象集合为空");
            double x = 0, y = 0, z = 0, radius = 0, count = circles.Count();
            if (count == 0) goto END;

            double xsum = 0, ysum = 0, zsum = 0, rsum = 0;
            foreach (var circle in circles)
            {
                xsum += circle.X;
                ysum += circle.Y;
                zsum += circle.Z;
                rsum += circle.Radius;
            }
            x = xsum / count; y = ysum / count; z = zsum / count; radius = rsum / count;

            END:
            return new Circle(x, y, z, radius);
        }
    }
}
