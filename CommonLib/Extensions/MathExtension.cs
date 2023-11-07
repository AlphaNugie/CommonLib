using CommonLib.Function;
using CommonLib.Function.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;

namespace CommonLib.Extensions
{
    /// <summary>
    /// 计算功能类
    /// </summary>
    public static class MathExtension
    {
        #region 常用数学公式/参数计算
        /// <summary>
        /// 在给出的点对象集合中，以3个点为一组，每组确定一个二维或三维空间中圆的圆心XYZ坐标（二维空间中Z为0）和半径长度 <para/>如此按顺序执行若干次（等于点数量除以3并向下取整），计算圆心XYZ坐标（二维空间中Z为0）和半径长度的平均值并返回圆形对象
        /// </summary>
        /// <param name="points">圆上每点的点对象的集合</param>
        /// <param name="dimension">要解出圆形信息的空间维度类型</param>
        /// <returns></returns>
        public static Circle GetAveragedCircleNumbers(this IEnumerable<Point3D> points, Dimensions dimension = Dimensions.Three)
        {
            Circle result = new Circle();
            if (points == null) goto END;
            List<Point3D> groups = points.Where(point => point != null).ToList();
            //保证至少有3个点，以3个点为1组（组内相邻点索引位置的差值为组的数量）
            if (groups.Count < 3) goto END;
            int groupCount = groups.Count / 3;
            List<Circle> circles = new List<Circle>();
            //每一组的内部索引
            var groupIndexes = new int[] { 0, 1, 2 };
            //从索引是0的位置开始循环，每次步进1个索引位置、执行次数为3点1组的数量（下面的循环），每次循环内部找出每组3个点的索引值及该索引处的点对象
            for (int i = 0; i <= groupCount - 1; i++)
            {
                var trinity = groupIndexes.Select(j => groups.ElementAt(i + groupCount * j)); //这一步计算组内每个点的索引值并取出点对象
                var circle = dimension == Dimensions.Three ? trinity.Get3DCircle() : trinity.Get2DCircle(); //解出所在圆信息
                circles.Add(circle);
            }
            result = circles.Average();
        END:
            return result;
        }

        #region 三维空间中圆上3点计算圆心坐标与半径长度
        ///// <summary>
        ///// 在给出的若干对XY坐标集合的集合中，找出16对XY坐标，排除收尾2对（剔除异常点），在剩下14个点中，找出相邻的3个点，根据XY坐标确定圆心的XY坐标和半径长度 <para/>如此按顺序执行12次，计算这12次测到的圆心XY坐标和半径长度的平均值并输出
        ///// </summary>
        ///// <param name="groupsOfCoors">圆上每点一对XY坐标的数值集合的集合</param>
        ///// <param name="centrex">圆心X坐标</param>
        ///// <param name="centrey">圆心Y坐标</param>
        ///// <param name="centrez">圆心Z坐标</param>
        ///// <param name="radius">圆周半径长度，单位与坐标轴单位相同</param>
        //public static void GetAveragedCircleNumbers3d(this IEnumerable<IEnumerable<double>> groupsOfCoors, out double centrex, out double centrey, out double centrez, out double radius)
        //{
        //    centrex = centrey = centrez = radius = 0;
        //    if (groupsOfCoors == null) return;
        //    //筛选出所有成组的坐标（数量至少为3并没有NaN）
        //    List<IEnumerable<double>> groups = groupsOfCoors.Where(group => group != null && group.Count() >= 3 && !group.Any(member => double.IsNaN(member))).ToList();
        //    //保证至少有3个点，以3个点为1组（组内相邻点索引位置的差值为组的数量）
        //    if (groups.Count < 3) return;
        //    int groupCount = groups.Count / 3;
        //    List<double> centrexs = new List<double>(), centreys = new List<double>(), centrezs = new List<double>(), radiuses = new List<double>();
        //    //每一组的内部索引
        //    var groupIndexes = new int[] { 0, 1, 2 };
        //    //从索引是0的位置开始循环，每次步进1个索引位置、执行次数为组的数量（下面的循环），每次循环内部找出每组3个点的索引值及该索引处的点对象
        //    for (int i = 0; i <= groupCount - 1; i++)
        //    {
        //        var doubles = groupIndexes.Select(j => groups.ElementAt(i + groupCount * j)).ToList(); //这一步计算组内每个点的索引值并取出点对象
        //        var c = CircleCalculator.Do(new Point3D(doubles[0]), new Point3D(doubles[1]), new Point3D(doubles[2]));
        //        centrexs.Add(c.X);
        //        centreys.Add(c.Y);
        //        centrezs.Add(c.Z);
        //        radiuses.Add(c.Radius);
        //    }
        //    centrex = centrexs.Average();
        //    centrey = centreys.Average();
        //    centrez = centrezs.Average();
        //    radius = radiuses.Average();
        //}

        /// <summary>
        /// 在给出的点对象集合中，以3个点为一组，每组确定一个三维空间中圆的圆心XYZ坐标和半径长度 <para/>如此按顺序执行若干次（等于点数量除以3并向下取整），计算圆心XYZ坐标和半径长度的平均值并返回圆形对象
        /// </summary>
        /// <param name="points">圆上每点的点对象的集合</param>
        /// <returns></returns>
        public static Circle GetAveragedCircleNumbers3D(this IEnumerable<Point3D> points)
        {
            return points.GetAveragedCircleNumbers(Dimensions.Three);
            //Circle result = new Circle();
            //if (points == null) goto END;
            //List<Point3D> groups = points.Where(point => point != null).ToList();
            ////保证至少有3个点，以3个点为1组（组内相邻点索引位置的差值为组的数量）
            //if (groups.Count < 3) goto END;
            //int groupCount = groups.Count / 3;
            //List<Circle> circles = new List<Circle>();
            ////每一组的内部索引
            //var groupIndexes = new int[] { 0, 1, 2 };
            ////从索引是0的位置开始循环，每次步进1个索引位置、执行次数为3点1组的数量（下面的循环），每次循环内部找出每组3个点的索引值及该索引处的点对象
            //for (int i = 0; i <= groupCount - 1; i++)
            //{
            //    var trinity = groupIndexes.Select(j => groups.ElementAt(i + groupCount * j)); //这一步计算组内每个点的索引值并取出点对象
            //    var circle = trinity.Get3DCircle(); //解出所在圆信息
            //    circles.Add(circle);
            //}
            //result = circles.Average();
            //END:
            //return result;
        }
        #endregion

        #region 二维空间中圆上3点计算圆心坐标与半径长度
        ///// <summary>
        ///// 在给出的若干对XY坐标集合的集合中，找出16对XY坐标，排除收尾2对（剔除异常点），在剩下14个点中，找出相邻的3个点，根据XY坐标确定圆心的XY坐标和半径长度 <para/>如此按顺序执行12次，计算这12次测到的圆心XY坐标和半径长度的平均值并输出
        ///// </summary>
        ///// <param name="groupsOfCoors">圆上每点一对XY坐标的数值集合的集合</param>
        ///// <param name="centrex">圆心X坐标</param>
        ///// <param name="centrey">圆心Y坐标</param>
        ///// <param name="radius">圆周半径长度，单位与坐标轴单位相同</param>
        //public static void GetAveragedCircleNumbers(this IEnumerable<IEnumerable<double>> groupsOfCoors, out double centrex, out double centrey, out double radius)
        //{
        //    centrex = centrey = radius = 0;
        //    if (groupsOfCoors == null) return;
        //    //筛选出所有成对的坐标（数量至少为2并没有NaN）
        //    List<IEnumerable<double>> groups = groupsOfCoors.Where(group => group != null && group.Count() >= 2 && !group.Any(member => double.IsNaN(member))).ToList();
        //    //保证至少有3个点，以3个点为1组（组内相邻点索引位置的差值为组的数量）
        //    if (groups.Count < 3) return;
        //    int groupCount = groups.Count / 3;
        //    List<double> centrexs = new List<double>(), centreys = new List<double>(), radiuses = new List<double>();
        //    //每一组的内部索引
        //    var groupIndexes = new int[] { 0, 1, 2 };
        //    //从索引是0的位置开始循环，每次步进1个索引位置、执行次数为3点1组的数量（下面的循环），每次循环内部找出每组3个点的索引值及该索引处的点对象
        //    for (int i = 0; i <= groupCount - 1; i++)
        //    {
        //        var doubles = groupIndexes.Select(j => groups.ElementAt(i + groupCount * j)); //这一步计算组内每个点的索引值并取出点对象
        //        doubles.GetCircleNumbers(out double cx, out double cy, out double r);
        //        centrexs.Add(cx);
        //        centreys.Add(cy);
        //        radiuses.Add(r);
        //    }
        //    #region 对List排序并排除最大的以及最小的10%元素（对结果影响不大且影响性能）
        //    ////假如组数量不小于10，则按升序排列并缩减元素数量到80%
        //    //if (groupCount >= 10)
        //    //{
        //    //    centrexs = centrexs.OrderBy(v => v).Shrink(0.8, true).ToList();
        //    //    centreys = centreys.OrderBy(v => v).Shrink(0.8, true).ToList();
        //    //    radiuses = radiuses.OrderBy(v => v).Shrink(0.8, true).ToList();
        //    //}
        //    #endregion
        //    centrex = centrexs.Average();
        //    centrey = centreys.Average();
        //    radius = radiuses.Average();
        //}

        /// <summary>
        /// 在给出的点对象集合中，以3个点为一组，每组确定一个二维空间中圆的圆心XY坐标和半径长度 <para/>如此按顺序执行若干次（等于点数量除以3并向下取整），计算圆心XY坐标和半径长度的平均值并返回圆形对象
        /// </summary>
        /// <param name="points">圆上每点的点对象的集合</param>
        /// <returns></returns>
        public static Circle GetAveragedCircleNumbers2D(this IEnumerable<Point3D> points)
        {
            return points.GetAveragedCircleNumbers(Dimensions.Two);
            //Circle result = new Circle();
            //if (points == null) goto END;
            //List<Point3D> groups = points.Where(point => point != null).ToList();
            ////保证至少有3个点，以3个点为1组（组内相邻点索引位置的差值为组的数量）
            //if (groups.Count < 3) goto END;
            //int groupCount = groups.Count / 3;
            //List<Circle> circles = new List<Circle>();
            ////每一组的内部索引
            //var groupIndexes = new int[] { 0, 1, 2 };
            ////从索引是0的位置开始循环，每次步进1个索引位置、执行次数为3点1组的数量（下面的循环），每次循环内部找出每组3个点的索引值及该索引处的点对象
            //for (int i = 0; i <= groupCount - 1; i++)
            //{
            //    var trinity = groupIndexes.Select(j => groups.ElementAt(i + groupCount * j)); //这一步计算组内每个点的索引值并取出点对象
            //    var circle = trinity.Get2DCircle(); //解出所在圆信息
            //    circles.Add(circle);
            //}
            //result = circles.Average();
            //END:
            //return result;
        }

        ///// <summary>
        ///// 根据圆上3个点的XY坐标确定圆心的XY坐标和半径长度，其中圆上3点的坐标由一个包含6个double数值的集合提供，按顺序分别为3个点的XY坐标
        ///// </summary>
        ///// <param name="pairsOfCoors">提供圆上3点XY坐标的数值集合</param>
        ///// <param name="centrex">圆心X坐标</param>
        ///// <param name="centrey">圆心Y坐标</param>
        ///// <param name="radius">圆周半径长度，单位与坐标轴单位相同</param>
        //public static void GetCircleNumbers(this IEnumerable<double> pairsOfCoors, out double centrex, out double centrey, out double radius)
        //{
        //    centrex = centrey = radius = 0;
        //    if (pairsOfCoors == null || pairsOfCoors.Count() < 6) return;
        //    double
        //        x1 = pairsOfCoors.ElementAt(0),
        //        y1 = pairsOfCoors.ElementAt(1),
        //        x2 = pairsOfCoors.ElementAt(2),
        //        y2 = pairsOfCoors.ElementAt(3),
        //        x3 = pairsOfCoors.ElementAt(4),
        //        y3 = pairsOfCoors.ElementAt(5);
        //    MathUtil.GetCircleNumbers(x1, y1, x2, y2, x3, y3, out centrex, out centrey, out radius);
        //}

        ///// <summary>
        ///// 根据圆上3个点的XY坐标确定圆心的XY坐标和半径长度，其中圆上3点的坐标由一个包含3对double数值集合的集合提供，按顺序分别为3个点的XY坐标
        ///// </summary>
        ///// <param name="pairsOfCoors">提供圆上3点XY坐标的数值集合的集合</param>
        ///// <param name="centrex">圆心X坐标</param>
        ///// <param name="centrey">圆心Y坐标</param>
        ///// <param name="radius">圆周半径长度，单位与坐标轴单位相同</param>
        //public static void GetCircleNumbers(this IEnumerable<IEnumerable<double>> pairsOfCoors, out double centrex, out double centrey, out double radius)
        //{
        //    centrex = centrey = radius = 0;
        //    if (pairsOfCoors == null) return;
        //    List<IEnumerable<double>> pairs = pairsOfCoors.Where(pair => pair != null && pair.Count() >= 2).ToList();
        //    if (pairs.Count < 3) return;
        //    double
        //        x1 = pairs[0].ElementAt(0),
        //        y1 = pairs[0].ElementAt(1),
        //        x2 = pairs[1].ElementAt(0),
        //        y2 = pairs[1].ElementAt(1),
        //        x3 = pairs[2].ElementAt(0),
        //        y3 = pairs[2].ElementAt(1);
        //    MathUtil.GetCircleNumbers(x1, y1, x2, y2, x3, y3, out centrex, out centrey, out radius);
        //}
        #endregion

        /// <summary>
        /// 计算正态分布函数的公式，计算时使用给定的平均值μ(mu)与标准差σ(sigma)，输入x坐标，输出正太分布曲线在这个位置的y坐标值
        /// https://i.postimg.cc/P5NxZ0WX/image.png
        /// </summary>
        /// <param name="x">x坐标值</param>
        /// <param name="mean">平均值μ(mu)，分布的峰值在此处</param>
        /// <param name="stdDev">标准差σ(sigma)，越大则分布的曲线越宽，反之则曲线越窄，标准差为1时为标准正态分布</param>
        /// <returns></returns>
        public static double NormalDistribution(this double x, double mean, double stdDev)
        {
            //乘以a是为了保证曲线与x轴之间面积为1
            double a = 1 / (stdDev * Math.Sqrt(2 * Math.PI));
            //假如只是为了得到钟形曲线，而不在意曲线与x轴之间面积是否为1的话，常数e可用大于1的数字代替，譬如π
            double b = Math.Pow(Math.E, -Math.Pow(x - mean, 2) / (2 * Math.Pow(stdDev, 2)));
            return a * b;
        }

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
        #endregion

        #region 整型与byte序列互转
        /// <summary>
        /// 将32位无符号整数转换为byte数组（长度为4）（默认大字节在前）
        /// </summary>
        /// <param name="input">输入的32位无符号整数</param>
        /// <param name="isBigEndian">是否大字节在前</param>
        /// <returns>长度为4的字节数组</returns>
        public static byte[] ToBytes(this uint input, bool isBigEndian = true)
        {
            //BitConverter.GetBytes方法的输出中，小字节在前（索引为0的方向），因此转换前按需求换向
            var bytes = BitConverter.GetBytes(input);
            return isBigEndian ? bytes.Reverse().ToArray() : bytes;
        }

        /// <summary>
        /// 将64位无符号整数转换为byte数组（长度为8）（默认大字节在前）
        /// </summary>
        /// <param name="input">输入的64位无符号整数</param>
        /// <param name="isBigEndian">是否大字节在前</param>
        /// <returns>长度为8的字节数组</returns>
        public static byte[] ToBytes(this ulong input, bool isBigEndian = true)
        {
            //BitConverter.GetBytes方法的输出中，小字节在前（索引为0的方向），因此转换前按需求换向
            var bytes = BitConverter.GetBytes(input);
            return isBigEndian ? bytes.Reverse().ToArray() : bytes;
        }

        ///// <summary>
        ///// 将一组byte序列（长度不超过4，可为空）转换为32位无符号整数（默认大字节在前）
        ///// </summary>
        ///// <param name="numbers">提供的byte序列</param>
        ///// <param name="isBigEndian">是否大字节在前</param>
        ///// <returns></returns>
        ///// <exception cref="ArgumentException"></exception>
        //public static uint ToUInt32(this IEnumerable<byte> numbers, /*int startIndex = 0, */bool isBigEndian = true)
        //{
        //    if (numbers == null || numbers.Count() == 0) return 0;
        //    int len = numbers.Count();
        //    if (len > 4) throw new ArgumentException("提供的byte序列长度超过32位无符号整数的最大字节数4", nameof(numbers));
        //    //BitConverter.ToUInt32方法的输入参数中，小字节在前（索引为0的方向），因此转换前按需求换向
        //    var array = isBigEndian ? numbers.Reverse().ToArray() : numbers.ToArray();
        //    //var array = numbers.ToArray();
        //    uint result = BitConverter.ToUInt32(array, 0);
        //    ////BitConverter.ToUInt32方法的输入参数中，小字节在前（索引为0的方向），因此假如要用大字节在前的方式显示，需从网络字节顺序转为主机字节顺序
        //    //if (isBigEndian)
        //    //    result = (uint)IPAddress.NetworkToHostOrder((int)result);
        //    return result;
        //}

        /// <summary>
        /// 将一组byte序列从索引位置转换为32位有符号整数（默认大字节在前）
        /// </summary>
        /// <param name="numbers">提供的byte序列</param>
        /// <param name="startIndex">计算的起始索引，从此索引开始找出4个字节用于转换</param>
        /// <param name="isBigEndian">是否大字节在前</param>
        /// <returns></returns>
        public static int ToInt32(this IEnumerable<byte> numbers, int startIndex = 0, bool isBigEndian = true)
        {
            var enums = numbers.Take4AfterSkip(startIndex);
            //BitConverter方法的输入参数中，小字节在前（索引为0的方向），因此转换前按需求换向
            enums = isBigEndian ? enums.Reverse() : enums;
            return BitConverter.ToInt32(enums.ToArray(), 0);
        }

        /// <summary>
        /// 将一组byte序列从索引位置转换为32位无符号整数（默认大字节在前）
        /// </summary>
        /// <param name="numbers">提供的byte序列</param>
        /// <param name="startIndex">计算的起始索引，从此索引开始找出4个字节用于转换</param>
        /// <param name="isBigEndian">是否大字节在前</param>
        /// <returns></returns>
        public static uint ToUInt32(this IEnumerable<byte> numbers, int startIndex = 0, bool isBigEndian = true)
        {
            var enums = numbers.Take4AfterSkip(startIndex);
            //BitConverter方法的输入参数中，小字节在前（索引为0的方向），因此转换前按需求换向
            enums = isBigEndian ? enums.Reverse() : enums;
            return BitConverter.ToUInt32(enums.ToArray(), 0);
        }

        ///// <summary>
        ///// 将一组byte序列（长度不超过8，可为空）转换为64位无符号整数（默认大字节在前）
        ///// </summary>
        ///// <param name="numbers">提供的byte序列</param>
        ///// <param name="isBigEndian">是否大字节在前</param>
        ///// <returns></returns>
        ///// <exception cref="ArgumentException"></exception>
        //public static ulong ToUInt64(this IEnumerable<byte> numbers, bool isBigEndian = true)
        //{
        //    if (numbers == null || numbers.Count() == 0) return 0;
        //    int len = numbers.Count();
        //    if (len > 8) throw new ArgumentException("提供的byte序列长度超过64位无符号整数的最大字节数8", nameof(numbers));
        //    //BitConverter.ToUInt64方法的输入参数中，小字节在前（索引为0的方向），因此转换前按需求换向
        //    var array = isBigEndian ? numbers.Reverse().ToArray() : numbers.ToArray();
        //    //var array = numbers.ToArray();
        //    ulong result = BitConverter.ToUInt64(array, 0);
        //    ////BitConverter.ToUInt64方法的输入参数中，小字节在前（索引为0的方向），因此假如要用大字节在前的方式显示，需从网络字节顺序转为主机字节顺序
        //    //if (isBigEndian)
        //    //    result = (ulong)IPAddress.NetworkToHostOrder((long)result);
        //    return result;
        //}

        /// <summary>
        /// 将一组byte序列从索引位置转换为64位无符号整数（默认大字节在前）
        /// </summary>
        /// <param name="numbers">提供的byte序列</param>
        /// <param name="startIndex">计算的起始索引，从此索引开始找出8个字节用于转换</param>
        /// <param name="isBigEndian">是否大字节在前</param>
        /// <returns></returns>
        public static long ToInt64(this IEnumerable<byte> numbers, int startIndex = 0, bool isBigEndian = true)
        {
            var enums = numbers.Take8AfterSkip(startIndex);
            //BitConverter方法的输入参数中，小字节在前（索引为0的方向），因此转换前按需求换向
            enums = isBigEndian ? enums.Reverse() : enums;
            return BitConverter.ToInt64(enums.ToArray(), 0);
        }

        /// <summary>
        /// 将一组byte序列从索引位置转换为64位无符号整数（默认大字节在前）
        /// </summary>
        /// <param name="numbers">提供的byte序列</param>
        /// <param name="startIndex">计算的起始索引，从此索引开始找出8个字节用于转换</param>
        /// <param name="isBigEndian">是否大字节在前</param>
        /// <returns></returns>
        public static ulong ToUInt64(this IEnumerable<byte> numbers, int startIndex = 0, bool isBigEndian = true)
        {
            var enums = numbers.Take8AfterSkip(startIndex);
            //BitConverter方法的输入参数中，小字节在前（索引为0的方向），因此转换前按需求换向
            enums = isBigEndian ? enums.Reverse() : enums;
            return BitConverter.ToUInt64(enums.ToArray(), 0);
        }
        #endregion

        ///// <summary>
        ///// 使输入的方位角保持在合理的范围之内（-180°到180°之间，包含-180°，不包含180°），假如超过180°则减360°，小于-180°则加360°，循环计算直到进入范围为止
        ///// </summary>
        ///// <param name="angle">待修改的输入方位角</param>
        //[Obsolete]
        //public static void KeepAzimuthInRange(ref double angle)
        //{
        //    MathUtil.KeepAzimuthInRange(ref angle);
        //    ////假如为180°，修改为-180°，确保同一个位置不会出现2种值
        //    //if (angle == 180)
        //    //    angle = -180;
        //    ////假如绝对值大于180，则向当前值符号相反的方向修正360°，直到在范围内为止
        //    //while (Math.Abs(angle) > 180)
        //    //    angle -= 360 * Math.Sign(angle);
        //}

        /// <summary>
        /// 根据输入的方位角计算出保持在合理的范围之内（-180°到180°之间）的方位角的值并返回，假如超过180°则减360°，小于-180°则加360°，循环计算直到进入范围为止
        /// </summary>
        /// <param name="angle">待进行计算的输入方位角</param>
        /// <returns></returns>
        public static double GetAzimuthThatsInRange(this double angle)
        {
            MathUtil.KeepAzimuthInRange(ref angle);
            return angle;
        }

        /// <summary>
        /// （使用MathUtil版本）根据给定的顶端以及回转处坐标计算（初步的）回转角度（y轴正向为回转0°方位）
        /// </summary>
        /// <param name="x1">顶端X坐标</param>
        /// <param name="y1">顶端Y坐标</param>
        /// <param name="xa">回转处X坐标</param>
        /// <param name="ya">回转处Y坐标</param>
        /// <returns></returns>
        [Obsolete]
        public static double GetAngleByCoordinates(double x1, double y1, double xa, double ya)
        {
            ////两点距离，加上一个极小值防止为0
            //double xdiff = x1 - xa, ydiff = y1 - ya, dist = Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) + 1e-20;
            ////初步角度，值区间为[-90, 90]
            //double angle = Math.Asin(xdiff / dist) * 180 / Math.PI;
            ////假如y坐标差为负值，则需要相对X轴做对称
            //if (ydiff < 0)
            //    angle = 180 * (xdiff > 0 ? 1 : -1) - angle;
            //return angle;
            return MathUtil.GetAngleByCoordinates(x1, y1, xa, ya);
        }

        /// <summary>
        /// （使用Swap）对两个值类型的值进行交换
        /// </summary>
        /// <typeparam name="T">要交换的值的类型</typeparam>
        /// <param name="t1">值1</param>
        /// <param name="t2">值2</param>
        [Obsolete]
        public static void Exchange<T>(ref T t1, ref T t2) where T : struct
        {
            //谨慎使用元组交换（部署后依赖可能会出问题）
            T temp = t1;
            t1 = t2;
            t2 = temp;
        }

        /// <summary>
        /// 对两个值类型的值进行交换
        /// </summary>
        /// <typeparam name="T">要交换的值的类型</typeparam>
        /// <param name="t1">值1</param>
        /// <param name="t2">值2</param>
        public static void Swap<T>(ref T t1, ref T t2) where T : struct
        {
            //谨慎使用元组交换（部署后依赖可能会出问题）
            T temp = t1;
            t1 = t2;
            t2 = temp;
        }

        /// <summary>
        /// 尝试获取值小数点后的数值位数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetDecimalPlaces(this object value)
        {
            int places = 0;
            try
            {
                string[] parts = value.ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                places = parts == null || parts.Length < 2 ? 0 : parts[1].Length;
            }
            catch (Exception) { }
            return places;
        }

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
        /// 判断一个双精度浮点数是否落在给定的若干个区间范围中的任意一个（包括等于），假如区间范围为空（不存在任何有效的区间范围），则返回默认值
        /// </summary>
        /// <param name="input">待判断的数字</param>
        /// <param name="ranges">给定的若干个区间范围</param>
        /// <param name="def">当区间范围为空时返回的默认值</param>
        /// <returns>假如在数值之间，返回true，否则返回false</returns>
        public static bool Between(this double input, IEnumerable<IEnumerable<double>> ranges, bool def = false)
        {
            if (ranges == null)
                return def;
            ranges = ranges.Where(range => range != null  && range.Count() >= 2).ToList();
            if (ranges.Count() == 0)
                return def;
            //对于每一组区间范围，进行一次范围比较，然后返回比较结果中的最大值：假如有任意一次满足则为true，否则为false
            return ranges.Select(range => input.Between(range.ElementAt(0), range.ElementAt(1))).Max();
        }

        /// <summary>
        /// 判断一个双精度浮点数是否在两个数值之间（或等于）
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
        /// 判断一个32位整型数是否在两个数值之间（或等于）
        /// </summary>
        /// <param name="input">待判断的数字</param>
        /// <param name="number1">数值1</param>
        /// <param name="number2">数值2</param>
        /// <returns>假如在数值之间，返回true，否则返回false</returns>
        public static bool Between(this int input, double number1, double number2)
        {
            return Between((double)input, number1, number2);
        }

        /// <summary>
        /// 判断一个16位整型数是否在两个数值之间（或等于）
        /// </summary>
        /// <param name="input">待判断的数字</param>
        /// <param name="number1">数值1</param>
        /// <param name="number2">数值2</param>
        /// <returns>假如在数值之间，返回true，否则返回false</returns>
        public static bool Between(this short input, double number1, double number2)
        {
            return Between((double)input, number1, number2);
        }

        /// <summary>
        /// 判断一个字节型数是否在两个数值之间（或等于）
        /// </summary>
        /// <param name="input">待判断的数字</param>
        /// <param name="number1">数值1</param>
        /// <param name="number2">数值2</param>
        /// <returns>假如在数值之间，返回true，否则返回false</returns>
        public static bool Between(this byte input, double number1, double number2)
        {
            return Between((double)input, number1, number2);
        }

        /// <summary>
        /// 获取数据集中所有不为0的数中最小的数，假如数据集为空或不存在非0的数据，则返回0
        /// </summary>
        /// <param name="set">待从中寻找符合条件的数据的源数据集</param>
        /// <param name="def">当数据集为空或不存在非0的数据时返回的默认值，默认为0</param>
        /// <returns></returns>
        public static double MinExceptZero(this IEnumerable<double> set, double def = 0)
        {
            //if (set == null)
            //    return 0;
            //set = set.Where(d => d != 0);
            //double min = 0;
            //try { min = set.Count() == 0 ? 0 : set.Min(); }
            //catch (Exception) { }
            //return min;
            double min = def;
            if (set == null)
                goto END;
            set = set.Where(d => d != 0);
            try { if (set.Count() > 0) min = set.Min(); }
            catch (Exception) { }
            END:
            return min;
        }

        /// <summary>
        /// 获取数据集中所有不为0的数中最大的数，假如数据集为空或不存在非0的数据，则返回0
        /// </summary>
        /// <param name="set">待从中寻找符合条件的数据的源数据集</param>
        /// <param name="def">当数据集为空或不存在非0的数据时返回的默认值，默认为0</param>
        /// <returns></returns>
        public static double MaxExceptZero(this IEnumerable<double> set, double def = 0)
        {
            //if (set == null)
            //    return 0;
            //set = set.Where(d => d != 0);
            //double max = 0;
            //try { max = set.Count() == 0 ? 0 : set.Max(); }
            //catch (Exception) { }
            //return max;
            double max = def;
            if (set == null)
                goto END;
            set = set.Where(d => d != 0);
            try { if (set.Count() > 0) max = set.Max(); }
            catch (Exception) { }
            END:
            return max;
        }

        /// <summary>
        /// 将一组原始数据(S)和一组对照数据(S')进行比较并计算差异参数（sum1→n[abs(Sn-S'n]/sum1→n[Sn]），数值区间在0到+∞（实际在等于或大于1时差异已明显过大），比较时指定原始数据在下标(索引)上和数值上的位移
        /// </summary>
        /// <param name="datas">原始数据组</param>
        /// <param name="comparisons">对照数据组</param>
        /// <param name="index_offset">原始数据在下标(索引)上的位移（假如偏移后超出范围则不参与计算）</param>
        /// <param name="peak_offset">原始数据在数值上的位移</param>
        /// <returns></returns>
        public static double GetSequenceDifference(this IEnumerable<double> datas, IEnumerable<double> comparisons, int index_offset = 0, double peak_offset = 0)
        {
            return GetSequenceDifference(datas, comparisons, out _, index_offset, peak_offset);
        }

        /// <summary>
        /// 将一组原始数据(S)和一组对照数据(S')进行比较并计算差异参数（sum1→n[abs(Sn-S'n]/sum1→n[Sn]），数值区间在0到+∞（实际在等于或大于1时差异已明显过大），比较时指定原始数据在下标(索引)上和数值上的位移
        /// </summary>
        /// <param name="datas">原始数据组</param>
        /// <param name="comparisons">对照数据组</param>
        /// <param name="differences">对外输出的差值（绝对值）序列</param>
        /// <param name="index_offset">原始数据在下标(索引)上的位移（假如偏移后超出范围则不参与计算）</param>
        /// <param name="peak_offset">原始数据在数值上的位移</param>
        /// <returns></returns>
        public static double GetSequenceDifference(this IEnumerable<double> datas, IEnumerable<double> comparisons, out List<double> differences, int index_offset = 0, double peak_offset = 0)
        {
            //原始数据数量，对照数据数量
            int countd, countc;
            differences = new List<double>();
            if (datas == null || (countd = datas.Count()) == 0 || comparisons == null || (countc = comparisons.Count()) == 0)
                return 1;
            double sum_diff = 0, sum_data = 0;
            for (int j = 0; j < countc; j++)
            {
                //原始数据下标(索引)
                int i = j - index_offset;
                if (i < 0)
                    continue;
                else if (i >= countd)
                    break;
                double data = datas.ElementAt(i) + peak_offset, comp = comparisons.ElementAt(j), diff = Math.Abs(data - comp);
                sum_diff += diff;
                sum_data += data;
                differences.Add(diff);
            }
            return sum_diff / sum_data;
        }
    }
}
