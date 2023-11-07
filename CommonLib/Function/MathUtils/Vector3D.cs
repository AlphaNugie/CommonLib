using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.MathUtils
{
    /// <summary>
    /// 三维空间中向量实体类
    /// </summary>
    public class Vector3D
    {
        #region 属性
        /// <summary>
        /// X方向分量
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y方向分量
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Z方向分量
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// 向量的模（长度）
        /// </summary>
        public double Length { get { return Math.Sqrt(LengthSquared); } }

        /// <summary>
        /// 向量模（长度）的平方
        /// </summary>
        public double LengthSquared { get { return X * X + Y * Y + Z * Z; } }
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        public Vector3D() { }

        /// <summary>
        /// 用给定XYZ方向分量值初始化
        /// </summary>
        /// <param name="x">X方向分量</param>
        /// <param name="y">Y方向分量</param>
        /// <param name="z">Z方向分量</param>
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #region 运算符
        /// <summary>
        /// 常数与向量相乘
        /// </summary>
        /// <param name="scalar">与向量相乘的常数（标量）</param>
        /// <param name="vector">与常数（标量）相乘的向量</param>
        /// <returns></returns>
        public static Vector3D operator *(double scalar, Vector3D vector)
        {
            return new Vector3D(
                scalar * vector.X,
                scalar * vector.Y,
                scalar * vector.Z
                );
        }

        /// <summary>
        /// 向量与常数相乘
        /// </summary>
        /// <param name="scalar">与向量相乘的常数（标量）</param>
        /// <param name="vector">与常数（标量）相乘的向量</param>
        /// <returns></returns>
        public static Vector3D operator *(Vector3D vector, double scalar)
        {
            return scalar * vector;
        }

        /// <summary>
        /// 将向量用常数去除
        /// </summary>
        /// <param name="scalar">与向量相乘的常数（标量）</param>
        /// <param name="vector">与常数（标量）相乘的向量</param>
        /// <returns></returns>
        public static Vector3D operator /(Vector3D vector, double scalar)
        {
            return new Vector3D(
                vector.X / scalar,
                vector.Y / scalar,
                vector.Z / scalar
                );
        }

        /// <summary>
        /// 向量之间相减
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z
                );
        }

        /// <summary>
        /// 向量之间叉乘（交换位置会影响计算结果）
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3D operator *(Vector3D a, Vector3D b)
        {
            return new Vector3D(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
                );
        }
        #endregion

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format($"X: {X}, Y: {Y}, Z: {Z}");
        }
    }
}
