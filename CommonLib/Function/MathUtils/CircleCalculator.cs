using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.MathUtils
{
    /// <summary>
    /// 圆形计算工具
    /// </summary>
    public static class CircleCalculator
    {
        /// <summary>
        /// 三维空间中，由给定的点对象集合内圆上3点的XYZ坐标计算圆心坐标与半径长度
        /// </summary>
        /// <param name="points">给定的点对象集合</param>
        public static Circle Get3DCircle(this IEnumerable<Point3D> points)
        {
            if (points == null) throw new ArgumentNullException(nameof(points), "点对象集合为空");
            if (points.Count() < 3) throw new ArgumentException(nameof(points), "点对象集合内元素数量不足3");
            return Get3DCircle(points.ElementAt(0), points.ElementAt(1), points.ElementAt(2));
        }

        /// <summary>
        /// 三维空间中，由圆上3点的XYZ坐标计算圆心坐标与半径长度
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <param name="p3">点3</param>
        /// <returns></returns>
        public static Circle Get3DCircle(Point3D p1, Point3D p2, Point3D p3)
        {
            //Vector3D a = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            //Vector3D b = new Vector3D(p3.X - p1.X, p3.Y - p1.Y, p3.Z - p1.Z);
            //Vector3D c = a * b;

            //double aLengthSquared = a.LengthSquared;
            //double bLengthSquared = b.LengthSquared;
            //double cLengthSquared = c.LengthSquared;

            //Vector3D center = new Vector3D(
            //    p1.X + (a * b * (b.LengthSquared * a - a.LengthSquared * b)).X / (2 * c.LengthSquared),
            //    p1.Y + (a * b * (b.LengthSquared * a - a.LengthSquared * b)).Y / (2 * c.LengthSquared),
            //    p1.Z + (a * b * (b.LengthSquared * a - a.LengthSquared * b)).Z / (2 * c.LengthSquared)
            //);

            //double radius = Math.Sqrt(Math.Pow(center.X - p1.X, 2) + Math.Pow(center.Y - p1.Y, 2) + Math.Pow(center.Z - p1.Z, 2));

            Vector3D A = p2 - p1, B = p3 - p1, C = A * B;
            Point3D center = p1 + (A * B * (B.LengthSquared * A - A.LengthSquared * B) / (2 * C.LengthSquared)); //圆心位置
            double radius = (center - p1).Length; //由圆边缘指向圆心的向量的模，等于半径长度
            return new Circle(center.X, center.Y, center.Z, radius);
        }

        /// <summary>
        /// 二维空间中，由给定的点对象集合内圆上3点的XY坐标计算圆心坐标与半径长度
        /// </summary>
        /// <param name="points">给定的点对象集合</param>
        public static Circle Get2DCircle(this IEnumerable<Point3D> points)
        {
            if (points == null) throw new ArgumentNullException(nameof(points), "点对象集合为空");
            if (points.Count() < 3) throw new ArgumentException(nameof(points), "点对象集合内元素数量不足3");
            return Get2DCircle(points.ElementAt(0), points.ElementAt(1), points.ElementAt(2));
        }

        /// <summary>
        /// 二维空间中，由圆上3点的XY坐标计算圆心坐标与半径长度
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <param name="p3">点3</param>
        public static Circle Get2DCircle(Point3D p1, Point3D p2, Point3D p3)
        {
            double slope1 = (p2.X - p1.X) / (p1.Y - p2.Y);
            double slope2 = (p3.X - p2.X) / (p2.Y - p3.Y);

            double mid_x1 = (p1.X + p2.X) / 2;
            double mid_y1 = (p1.Y + p2.Y) / 2;
            double mid_x2 = (p2.X + p3.X) / 2;
            double mid_y2 = (p2.Y + p3.Y) / 2;

            double
                centrex = (slope1 * mid_x1 - slope2 * mid_x2 + mid_y2 - mid_y1) / (slope1 - slope2),
                centrey = slope1 * (centrex - mid_x1) + mid_y1,
                radius = Math.Sqrt(Math.Pow(centrex - p1.X, 2) + Math.Pow(centrey - p1.Y, 2));

            return new Circle(centrex, centrey, 0, radius);
        }
    }

    /// <summary>
    /// 维度数量
    /// </summary>
    public enum Dimensions
    {
        /// <summary>
        /// 二维空间
        /// </summary>
        Two,

        /// <summary>
        /// 三维空间
        /// </summary>
        Three
    }
}
