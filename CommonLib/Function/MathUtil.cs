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
