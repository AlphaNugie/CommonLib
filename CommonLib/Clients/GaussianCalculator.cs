using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 一维高斯分布计算器
    /// </summary>
    public class GaussianCalculator
    {
        /// <summary>
        /// 幅值，高斯分布最高点的值，当 A=1/(sigma*pow(2*pi,0.5)) 时高斯分布积分为1
        /// </summary>
        public double Amplitude { get; set; }

        /// <summary>
        /// 高斯分布的期望（一般为0）
        /// </summary>
        public double Expected { get; set; }

        /// <summary>
        /// 标准差
        /// </summary>
        public double Sigma { get; set; }

        /// <summary>
        /// 一维高斯分布构造器
        /// </summary>
        /// <param name="amplitude">幅值，高斯分布最高点的值，当 A=1/(sigma*pow(2*pi,0.5)) 时高斯分布积分为1</param>
        /// <param name="expected">高斯分布的期望（一般为0）</param>
        /// <param name="sigma">标准差</param>
        public GaussianCalculator(double amplitude, double expected, double sigma)
        {
            this.Amplitude = amplitude;
            this.Expected = expected;
            this.Sigma = sigma;
        }

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
