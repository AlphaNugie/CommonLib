using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function
{
    /// <summary>
    /// 计算功能类
    /// </summary>
    public static class MathUtil
    {
        /// <summary>
        /// 计算输入值的半正矢，假如输入为角度，自动转换为弧度
        /// </summary>
        /// <param name="input">输入角度或弧度</param>
        /// <param name="input_as_degree">是否为角度</param>
        /// <returns></returns>
        public static double Haversine(double input, bool input_as_degree)
        {
            input = input_as_degree ? input * Math.PI / 180 : input;
            return Math.Pow(Math.Sin(input / 2), 2); //半正矢公式为(1-cosθ)/2=sin(θ/2)^2
        }

        /// <summary>
        /// 判断是否在两个数值之间（或等于）
        /// </summary>
        /// <param name="input">待判断的数字</param>
        /// <param name="number1">数值1</param>
        /// <param name="number2">数值2</param>
        /// <returns>假如在数值之间，返回true，否则返回false</returns>
        public static bool Between(this double input, double number1, double number2)
        {
            return (input >= number1 && input <= number2) || (input >= number2 && input <= number1);
        }

        /// <summary>
        /// 判断是否在两个数值之间（或等于）
        /// </summary>
        /// <param name="input">待判断的数字</param>
        /// <param name="number1">数值1</param>
        /// <param name="number2">数值2</param>
        /// <returns>假如在数值之间，返回true，否则返回false</returns>
        public static bool Between(this int input, double number1, double number2)
        {
            return (input >= number1 && input <= number2) || (input >= number2 && input <= number1);
        }

        /// <summary>
        /// 根据样本计算方差
        /// </summary>
        /// <param name="numbers">样本</param>
        /// <returns></returns>
        public static double Variance(this IEnumerable<double> numbers)
        {
            if (numbers == null || numbers.Count() == 0)
                throw new ArgumentException("参数不包含任何元素!");

            //double average = numbers.Average(); //平均值
            //double result = numbers.Select(number => Math.Pow(number - average, 2)).Average(); //方差
            //return result;
            return Variance(numbers, numbers.Average());
        }

        /// <summary>
        /// 根据样本与平均值计算方差
        /// </summary>
        /// <param name="numbers">样本</param>
        /// <param name="average">平均值</param>
        /// <returns></returns>
        public static double Variance(this IEnumerable<double> numbers, double average)
        {
            if (numbers == null || numbers.Count() == 0)
                throw new ArgumentException("参数不包含任何元素!");

            //double average = numbers.Average(); //平均值
            double result = numbers.Select(number => Math.Pow(number - average, 2)).Average(); //方差
            return result;
        }

        /// <summary>
        /// 计算标准差
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static double Standard(this IEnumerable<double> numbers)
        {
            if (numbers == null || numbers.Count() == 0)
                throw new ArgumentException("参数不包含任何元素!");

            return Math.Sqrt(Variance(numbers));
        }
    }
}
