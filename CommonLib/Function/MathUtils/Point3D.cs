using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.MathUtils
{
    /// <summary>
    /// 三维空间中坐标点对象
    /// </summary>
    public class Point3D : IEquatable<Point3D>
    {
        #region 属性
        /// <summary>
        /// 空间X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 空间Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 空间Z坐标
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// 反射率，默认为0
        /// <para/>对于激光雷达为反射率，对于毫米波雷达为RCS值
        /// </summary>
        public double Reflectivity { get; set; }

        /// <summary>
        /// 设备内部X坐标
        /// <para/>对于ARS408毫米波雷达为纵向距离
        /// </summary>
        public double InterX { get; set; }

        /// <summary>
        /// 设备内部Y坐标
        /// <para/>对于ARS408毫米波雷达为横向距离
        /// </summary>
        public double InterY { get; set; }

        /// <summary>
        /// 设备内部Z坐标
        /// </summary>
        public double InterZ { get; set; }

        /// <summary>
        /// 设备内部角度（单位：度）
        /// <para/>计算方式：取内部Y坐标除以内部X坐标后的反正切值（以X轴正向为0，向Y轴正向一侧旋转增大；当内部X坐标为0时，内部Y坐标为正时为+90度、为负时为-90度）
        /// </summary>
        public double InterAngle { get { return InterX == 0 ? Math.Sign(InterY) * 90 : Math.Atan(InterY / InterX) * 180 / Math.PI; } }
        #endregion

        #region 构造函数
        /// <summary>
        /// 默认构造器
        /// </summary>
        public Point3D() { }

        /// <summary>
        /// 用给定的XYZ坐标初始化
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="z">Z坐标</param>
        /// <param name="ri">反射率（或者对于毫米波雷达是RCS值）</param>
        /// <param name="ix">设备内部X坐标（对于ARS408毫米波雷达是纵向距离）</param>
        /// <param name="iy">设备内部Y坐标（对于ARS408毫米波雷达是横向距离）</param>
        /// <param name="iz">设备内部Z坐标</param>
        public Point3D(double x, double y = 0, double z = 0, double ri = 0, double ix = 0, double iy = 0, double iz = 0)
        {
            X = x;
            Y = y;
            Z = z;
            Reflectivity = ri;
            InterX = ix;
            InterY = iy;
            InterZ = iz;
        }

        /// <summary>
        /// 用给定数值集合初始化，集合内元素分别为XYZ坐标，假如数量不足则赋值优先顺序为X/Y/Z
        /// </summary>
        /// <param name="coors">给定XYZ坐标的数值集合</param>
        public Point3D(IEnumerable<double> coors)
        {
            if (coors == null) return;
            //if (coors.Count() < 3) throw new ArgumentException(nameof(coors), "坐标列表长度不足3");
            X = coors.ElementAtOrDefault(0);
            Y = coors.ElementAtOrDefault(1);
            Z = coors.ElementAtOrDefault(2);
        }
        #endregion

        #region 运算符
        /// <summary>
        /// 两点相减，得到从一点开始、另一点结束的向量
        /// </summary>
        /// <param name="p2"></param>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static Vector3D operator -(Point3D p2, Point3D p1)
        {
            return new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
        }

        /// <summary>
        /// 一点减去向量，得到另一点，后者在向量起点，前者在向量终点
        /// </summary>
        /// <param name="p2"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Point3D operator -(Point3D p2, Vector3D v)
        {
            return new Point3D(p2.X - v.X, p2.Y - v.Y, p2.Z - v.Z);
        }

        /// <summary>
        /// 一点与向量相加，得到另一点，后者在向量终点，前者在向量起点
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Point3D operator +(Point3D p1, Vector3D v)
        {
            return new Point3D(p1.X + v.X, p1.Y + v.Y, p1.Z + v.Z);
        }

        /// <summary>
        /// 一点与向量相加，得到另一点，后者在向量终点，前者在向量起点
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Point3D operator +(Vector3D v, Point3D p1)
        {
            return p1 + v;
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

        /// <summary>
        /// 计算距离另一个坐标对象的距离
        /// </summary>
        /// <param name="other"></param>
        /// <param name="project2Surface">是否投影到水平面，假如为true，则仅计算水平面投影的距离（忽略Z轴坐标）<para/>默认为false</param>
        /// <returns></returns>
        public double DistTo(Point3D other, bool project2Surface = false)
        {
            if (other == null)
                //return double.NaN;
                throw new ArgumentNullException("other", "相比较的另一个坐标对象为空");
            return Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2) + (project2Surface ? 0 : Math.Pow(other.Z - Z, 2)));
        }

        /// <summary>
        /// 计算相对于另一个坐标对象的方位（角度，仅考虑XY平面，以另一个坐标对象为中心）
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double AngleTo(Point3D other)
        {
            if (other == null)
                throw new ArgumentNullException("other", "相比较的另一个坐标对象为空");
            return MathUtil.GetAngleByCoordinates(X, Y, other.X, other.Y);
        }

        /// <summary>
        /// 读取给定文件路径的文件内容，每行转换为包含XYZ坐标信息的点对象（数字间用tab制表符、半角逗号或空格分隔）、每个点对象再作为一个集合并返回
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略
        /// </summary>
        /// <param name="filePath">读取坐标信息的文件的完整文件路径</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        public static List<Point3D> GetPointsInFileContent(string filePath)
        {
            return GetPointsInFileContent(filePath, out _);
        }

        /// <summary>
        /// 读取给定文件路径的文件内容，每行转换为包含XYZ坐标信息的点对象（数字间用tab制表符、半角逗号或空格分隔）、每个点对象再作为一个集合并返回
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略
        /// </summary>
        /// <param name="filePath">读取坐标信息的文件的完整文件路径</param>
        /// <param name="groupsOfNumbers">从文件内容中转换的XY坐标数组的列表</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        public static List<Point3D> GetPointsInFileContent(string filePath, out List<double[]> groupsOfNumbers)
        {
            groupsOfNumbers = new List<double[]>();
            try { groupsOfNumbers = MathUtil.GetNumberArraysInFileContent(filePath); }
            catch (ArgumentException e) { throw e; }
            catch (DirectoryNotFoundException e) { throw e; }
            //var lines = File.ReadAllLines(filePath);
            return groupsOfNumbers.Where(group => group != null && group.Count() >= 3 && !group.Any(member => double.IsNaN(member))).Select(group => new Point3D(group[0], group[1], group[2])).ToList();
        }

        #region 对象比较
        #region 是否相等的比较
        /// <inheritdoc/>
        public bool Equals(Point3D other)
        {
            if (other == null) return false;
            return other.X == X && other.Y == Y && other.Z == Z && other.Reflectivity == Reflectivity;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Point3D)) return false;
            return Equals((Point3D)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -107860405;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            hashCode = hashCode * -1521134295 + Reflectivity.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// 重新定义的相等符号
        /// </summary>
        /// <param name="left">左侧实例</param>
        /// <param name="right">右侧实例</param>
        /// <returns></returns>
        public static bool operator ==(Point3D left, Point3D right)
        {
            return !(left is null) && !(right is null) && left.Equals(right);
        }

        /// <summary>
        /// 重新定义的不等符号
        /// </summary>
        /// <param name="left">左侧实例</param>
        /// <param name="right">右侧实例</param>
        /// <returns></returns>
        public static bool operator !=(Point3D left, Point3D right)
        {
            return !(left == right);
        }
        #endregion
        #endregion
    }
}
