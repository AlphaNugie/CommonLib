using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.Fitting
{
    /// <summary>
    /// 曲线拟合类
    /// </summary>
    public static class CurveFitting
    {
        ///<summary>
        ///用最小二乘法拟合二元多次曲线，获取曲线拟合函数的系数
        ///例如y=a0 + a1*x + a2*x^2 + ... an*x^n，方法将返回包含a0至an的的数组
        ///</summary>
        ///<param name="arrX">已知点的x坐标集合</param>
        ///<param name="arrY">已知点的y坐标集合</param>
        ///<param name="length">已知点的个数</param>
        ///<param name="dimension">方程的最高次数</param>
        ///<returns>返回一个数组，数组中包括从高次到低次每个次数的系数（最后一个系数为截距）</returns>
        public static double[] GetCurveCoefficients(IEnumerable<double> arrX, IEnumerable<double> arrY, int length, int dimension)
        {
            int count = dimension + 1; //n次方程需要求n+1个系数
            double[,] guass = new double[count, count + 1]; //高斯矩阵 例如：y=a0+a1*x+a2*x*x
            
            for (int i = 0; i < count; i++)
            {
                int j;
                //Guass[i,]中前count个数据
                for (j = 0; j < count; j++)
                    guass[i, j] = SumArr(arrX, j + i, length);
                //Guass[i,]中第count+1个数据
                guass[i, j] = SumArr(arrX, i, arrY, 1, length);
            }

            return ComputeGauss(guass, count);

        }

        /// <summary>
        /// 求数组元素的n次方的和
        /// </summary>
        /// <param name="arr">待计算数组</param>
        /// <param name="n"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static double SumArr(IEnumerable<double> arr, int n, int length)
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                //数组元素为0时不需要计算
                if (arr.ElementAt(i) == 0)
                    continue;

                //假如幂数为0，直接+1
                if (n != 0)
                    s += Math.Pow(arr.ElementAt(i), n);
                else
                    s += 1;
            }
            return s;
        }

        /// <summary>
        /// 两数组元素n1次方与n2次方乘积的和
        /// </summary>
        /// <param name="arr1">数组1</param>
        /// <param name="n1">幂数1</param>
        /// <param name="arr2">数组2</param>
        /// <param name="n2">幂数2</param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static double SumArr(IEnumerable<double> arr1, int n1, IEnumerable<double> arr2, int n2, int length)
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                //假如两数组元素有一个是0，不需要计算
                if (arr1.ElementAt(i) == 0 || arr2.ElementAt(i) == 0)
                    continue;

                //假如幂数均为0，直接+1
                if (n1 != 0 || n2 != 0)
                    s += Math.Pow(arr1.ElementAt(i), n1) * Math.Pow(arr2.ElementAt(i), n2);
                else
                    s += 1;
            }
            return s;

        }

        /// <summary>
        /// 根据高斯矩阵与系数个数，返回包含系数值的数组
        /// </summary>
        /// <param name="guass"></param>
        /// <param name="n"></param>
        /// <returns>返回值是函数的系数</returns>
        private static double[] ComputeGauss(double[,] guass, int n)
        {
            int i, j, k, m;
            double temp, max, s;
            double[] x = new double[n];

            for (i = 0; i < n; i++) x[i] = 0; //初始化

            for (j = 0; j < n; j++)
            {
                max = 0;

                k = j;
                for (i = j; i < n; i++)
                {
                    if (Math.Abs(guass[i, j]) > max)
                    {
                        max = guass[i, j];
                        k = i;
                    }
                }
                
                if (k != j)
                {
                    for (m = j; m < n + 1; m++)
                    {
                        temp = guass[j, m];
                        guass[j, m] = guass[k, m];
                        guass[k, m] = temp;

                    }
                }

                //此线性方程为奇异线性方程
                if (0 == max)
                    return x;
                
                for (i = j + 1; i < n; i++)
                {
                    s = guass[i, j];
                    for (m = j; m < n + 1; m++)
                        guass[i, m] = guass[i, m] - guass[j, m] * s / (guass[j, j]);
                }
            }
            
            for (i = n - 1; i >= 0; i--)
            {
                s = 0;
                for (j = i + 1; j < n; j++)
                    s += guass[i, j] * x[j];

                x[i] = Math.Round((guass[i, n] - s) / guass[i, i], 3);
            }

            return x.Reverse().ToArray();
        }
    }
}
