using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.Fitting
{
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
    /// 手性类型
    /// </summary>
    public enum Chirality
    {
        /// <summary>
        /// 右手性
        /// </summary>
        RightHand = 1,

        /// <summary>
        /// 左手性
        /// </summary>
        LeftHand = -1,
    }

    /// <summary>
    /// 修改后的坐标中原XYZ坐标的系数
    /// </summary>
    public class CoordinateRatios
    {
        /// <summary>
        /// 原X坐标的系数
        /// </summary>
        public double Xratio { get; internal set; }

        /// <summary>
        /// 原Y坐标的系数
        /// </summary>
        public double Yratio { get; internal set; }

        /// <summary>
        /// 原Z坐标的系数（假如没有纵向坐标则应为0）
        /// </summary>
        public double Zratio { get; internal set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public CoordinateRatios() : this(0, 0, 0) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="xratio"></param>
        /// <param name="yratio"></param>
        /// <param name="zratio"></param>
        public CoordinateRatios(double xratio = 0, double yratio = 0, double zratio = 0)
        {
            Xratio = xratio;
            Yratio = yratio;
            Zratio = zratio;
        }

        /// <summary>
        /// 获取字符串描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0:f4}, {1:f4}, {2:f4}", Xratio, Yratio, Zratio);
        }
    }
}
