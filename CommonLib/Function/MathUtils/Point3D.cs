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
    public class Point3D
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Z坐标
        /// </summary>
        public double Z { get; set; }

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
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
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
    }
}
