using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Extensions
{
    /// <summary>
    /// 计算功能类
    /// </summary>
    public static class MathExtension
    {
        /// <summary>
        /// 计算输入值的半正矢，假如输入为角度，自动转换为弧度
        /// </summary>
        /// <param name="input">输入角度或弧度</param>
        /// <param name="input_as_degree">是否为角度</param>
        /// <returns></returns>
        public static double Haversine(this double input, bool input_as_degree)
        {
            input = input_as_degree ? input * Math.PI / 180 : input;
            return Math.Pow(Math.Sin(input / 2), 2); //半正矢公式为(1-cosθ)/2=sin(θ/2)^2
        }

        /// <summary>
        /// 计算输入值的半正矢，默认输入为角度
        /// </summary>
        /// <param name="input">输入角度</param>
        /// <returns></returns>
        public static double Haversine(this double input) { return Haversine(input, true); }

        ///// <summary>
        ///// [不准确]根据两个坐标的经纬度与地球半径计算坐标间的距离（米）
        ///// </summary>
        ///// <param name="lat1">纬度1</param>
        ///// <param name="lon1">经度1</param>
        ///// <param name="lat2">纬度1</param>
        ///// <param name="lon2">经度2</param>
        ///// <param name="earth_radius">地球半径，米</param>
        ///// <returns></returns>
        //public static double GetCoordinateDistance(double lat1, double lon1, double lat2, double lon2, int earth_radius)
        //{
        //    double h = Haversine(lat2 - lat1) + Math.Cos(lat1) * Math.Cos(lat2) * Haversine(lon2 - lon1);
        //    //return Math.Acos(1 - 2 * h) * earth_radius; //根据反余弦计算
        //    return Math.Asin(Math.Sqrt(h)) * 2 * earth_radius; //根据反正弦计算
        //}

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

        /// <summary>
        /// 获取数据集中所有不为0的数中最小的数，假如数据集为空或不存在非0的数据，则返回0
        /// </summary>
        /// <param name="set">待从中寻找符合条件的数据的源数据集</param>
        /// <returns></returns>
        public static double MinExceptZero(this IEnumerable<double> set)
        {
            if (set == null)
                return 0;
            set = set.Where(d => d != 0);
            double min = 0;
            try { min = set.Count() == 0 ? 0 : set.Min(); }
            catch (Exception) { }
            return min;
        }

        /// <summary>
        /// 获取数据集中所有不为0的数中最大的数，假如数据集为空或不存在非0的数据，则返回0
        /// </summary>
        /// <param name="set">待从中寻找符合条件的数据的源数据集</param>
        /// <returns></returns>
        public static double MaxExceptZero(this IEnumerable<double> set)
        {
            if (set == null)
                return 0;
            set = set.Where(d => d != 0);
            double max = 0;
            try { max = set.Count() == 0 ? 0 : set.Max(); }
            catch (Exception) { }
            return max;
        }

        /// <summary>
        /// 对两个值类型的值进行交换
        /// </summary>
        /// <typeparam name="T">要交换的值的类型</typeparam>
        /// <param name="t1">值1</param>
        /// <param name="t2">值2</param>
        public static void Exchange<T>(ref T t1, ref T t2) where T: struct
        {
            T temp = t1;
            t1 = t2;
            t2 = temp;
        }
    }
}
