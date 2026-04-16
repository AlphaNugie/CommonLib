using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
#if NET45
    /// <summary>
    /// 一维高斯分布计算器（高斯分布亦称为正态分布，期望为0、标准差为1时为标准正态分布）
    /// </summary>
    public class GaussianCalculator
    {
        /// <summary>
        /// 幅值，高斯分布最高点/中心的y值，当 A=1/(σ*√2π) 时高斯分布积分为1
        /// </summary>
        public double Amplitude { get; set; }

        /// <summary>
        /// 高斯分布的期望，一般为0（即平均值，以μ表示，高斯分布最高点/中心的x值，期望减小或增大时将左右平移）
        /// </summary>
        public double Expected { get; set; }

        /// <summary>
        /// 标准差，以σ表示，σ越大，数据分布越分散，σ越小，数据分布越集中
        /// </summary>
        public double Sigma { get; set; }

        /// <summary>
        /// 一维高斯分布构造器（期望为0、标准差为1时为标准正态分布）
        /// </summary>
        /// <param name="amplitude">幅值，高斯分布最高点的值，当 A=1/(σ*√2π) 时高斯分布积分为1</param>
        /// <param name="expected">高斯分布的期望，一般为0（即平均值，以μ表示，高斯分布最高点/中心的x值，期望减小或增大时将左右平移）</param>
        /// <param name="sigma">标准差，以σ表示，σ越大，数据分布越分散，σ越小，数据分布越集中</param>
        public GaussianCalculator(double amplitude, double expected, double sigma)
        {
            Amplitude = amplitude;
            Expected = expected;
            Sigma = sigma;
        }
#elif NET9_0_OR_GREATER
    /// <summary>
    /// 一维高斯分布计算器（高斯分布亦称为正态分布，期望为0、标准差为1时为标准正态分布）
    /// </summary>
    /// <remarks>
    /// 一维高斯分布构造器（期望为0、标准差为1时为标准正态分布）
    /// </remarks>
    /// <param name="amplitude">幅值，高斯分布最高点的值，当 A=1/(σ*√2π) 时高斯分布积分为1</param>
    /// <param name="expected">高斯分布的期望，一般为0（即平均值，以μ表示，高斯分布最高点/中心的x值，期望减小或增大时将左右平移）</param>
    /// <param name="sigma">标准差，以σ表示，σ越大，数据分布越分散，σ越小，数据分布越集中</param>
    public class GaussianCalculator(double amplitude, double expected, double sigma)
    {
        /// <summary>
        /// 幅值，高斯分布最高点/中心的y值，当 A=1/(σ*√2π) 时高斯分布积分为1
        /// </summary>
        public double Amplitude { get; set; } = amplitude;

        /// <summary>
        /// 高斯分布的期望，一般为0（即平均值，以μ表示，高斯分布最高点/中心的x值，期望减小或增大时将左右平移）
        /// </summary>
        public double Expected { get; set; } = expected;

        /// <summary>
        /// 标准差，以σ表示，σ越大，数据分布越分散，σ越小，数据分布越集中
        /// </summary>
        public double Sigma { get; set; } = sigma;
#endif

        /// <summary>
        /// 根据X计算该坐标高斯分布的值
        /// </summary>
        /// <param name="x">高斯分布中的X坐标</param>
        /// <returns></returns>
        public double Calc(double x)
        {
            return Amplitude * Math.Exp(-0.5 * Math.Pow((x - Expected) / Sigma, 2));
        }
    }
}
