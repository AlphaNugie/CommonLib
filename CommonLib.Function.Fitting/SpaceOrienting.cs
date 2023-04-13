using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.Fitting
{
    /// <summary>
    /// 空间定位、变换工具类
    /// </summary>
    public static class SpaceOrienting
    {
        /// <summary>
        /// 当某坐标系绕某个特定的轴旋转特定角度时，从旧坐标系向新坐标系的变换矩阵（角度正方向为从一个轴的正向向另一个轴正向旋转，如X轴向Y轴，Y轴向Z轴，X轴向Z轴）
        /// </summary>
        /// <param name="angle">绕特定轴旋转的角度（单位：角度）</param>
        /// <param name="axis">将坐标系以其为中心旋转的特定轴</param>
        /// <returns></returns>
        public static Matrix<double> GetAngleOrientedMatrix(double angle, AxisType axis)
        {
            int axisIndex = (int)axis;
            double sin = Math.Sin(angle * Math.PI / 180), cos = Math.Cos(angle * Math.PI / 180);
            //除去绕其旋转的特定轴之后，剩下两轴所形成的2x2矩阵的每列元素的序列，以及与旋转轴对应的列元素序列（目前均为0，后面要插入矩阵中）
            //cos   -sin
            //sin   cos
            List<double> list1 = new List<double> { cos, sin }, list2 = new List<double> { -1 * sin, cos }, listZero = new List<double> { 0, 0 };
            //上面每一列中在与旋转轴对应的位置处插入元素，再将旋转轴的列元素插入另外两轴列元素数组所形成的List中间
            //假如旋转轴为X轴，效果则为
            //1 0   0
            //0 cos -sin
            //0 sin cos
            list1.Insert(axisIndex, 0);
            list2.Insert(axisIndex, 0);
            listZero.Insert(axisIndex, 1);
            List<double[]> finalList = new List<double[]> { list1.ToArray(), list2.ToArray() };
            finalList.Insert(axisIndex, listZero.ToArray());
            return Matrix<double>.Build.DenseOfColumnArrays(finalList.ToArray());
        }
    }

    /// <summary>
    /// 坐标轴类型
    /// </summary>
    public enum AxisType
    {
        /// <summary>
        /// X轴
        /// </summary>
        X = 0,

        /// <summary>
        /// Y轴
        /// </summary>
        Y = 1,

        /// <summary>
        /// Z轴
        /// </summary>
        Z = 2,
    }

    /// <summary>
    /// 修改后的坐标中原XY坐标的系数
    /// </summary>
    public class CoordinateRatios
    {
        /// <summary>
        /// 原X坐标的系数
        /// </summary>
        public double Xratio;

        /// <summary>
        /// 原Y坐标的系数
        /// </summary>
        public double Yratio;

        /// <summary>
        /// 获取字符串描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0:f4}, {1:f4}", Xratio, Yratio);
        }
    }
}
