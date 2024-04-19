using CommonLib.Extensions;
using CommonLib.Function.MathUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function
{
    /// <summary>
    /// 数学计算功能类
    /// </summary>
    public static class MathUtil
    {
        #region 二维或三维空间中圆上3点计算圆心坐标与半径长度
        /// <summary>
        /// 读取给定文件路径的文件内容，每行转换为一组XY坐标的数组（数字间用tab制表符、半角逗号或空格分隔）、每个数组再作一个集合，用这些集合中的坐标计算圆心XY坐标和半径长度的平均值并输出
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略，同时假设这些坐标都在某个圆上或附近
        /// </summary>
        /// <param name="filePath">读取XY坐标内容的文件的完整文件路径</param>
        /// <param name="dimension">要解出圆形信息的空间维度类型</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        public static Circle GetAveragedCircleNumbers(string filePath, Dimensions dimension = Dimensions.Three)
        {
            return GetAveragedCircleNumbers(filePath, dimension, out _);
        }

        /// <summary>
        /// 读取给定文件路径的文件内容，每行转换为一组XY坐标的数组（数字间用tab制表符、半角逗号或空格分隔）、每个数组再作一个集合，用这些集合中的坐标计算圆心XY坐标和半径长度的平均值并输出
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略，同时假设这些坐标都在某个圆上或附近
        /// </summary>
        /// <param name="filePath">读取XY坐标内容的文件的完整文件路径</param>
        /// <param name="dimension">要解出圆形信息的空间维度类型</param>
        /// <param name="groupsOfCoors">从文件内容中转换的XY坐标数组的列表</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        public static Circle GetAveragedCircleNumbers(string filePath, Dimensions dimension, out List<double[]> groupsOfCoors)
        {
            groupsOfCoors = new List<double[]>();
            try { groupsOfCoors = GetNumberArraysInFileContent(filePath); }
            catch (ArgumentException e) { throw e; }
            catch (DirectoryNotFoundException e) { throw e; }
            var points = groupsOfCoors.Where(group => group != null && group.Count() >= 3 && !group.Any(member => double.IsNaN(member))).Select(group => new Point3D(group[0], group[1], group[2])).ToList();
            return points.GetAveragedCircleNumbers(dimension);
        }
        #endregion

        #region 三维空间中圆上3点计算圆心坐标与半径长度
        /// <summary>
        /// 读取给定文件路径的文件内容，每行转换为一组XY坐标的数组（数字间用tab制表符、半角逗号或空格分隔）、每个数组再作一个集合，用这些集合中的坐标计算圆心XY坐标和半径长度的平均值并输出
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略，同时假设这些坐标都在某个圆上或附近
        /// </summary>
        /// <param name="filePath">读取XY坐标内容的文件的完整文件路径</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        public static Circle GetAveragedCircleNumbers3d(string filePath)
        {
            return GetAveragedCircleNumbers(filePath, Dimensions.Three, out _);
        }

        /// <summary>
        /// 读取给定文件路径的文件内容，每行转换为一组XY坐标的数组（数字间用tab制表符、半角逗号或空格分隔）、每个数组再作一个集合，用这些集合中的坐标计算圆心XY坐标和半径长度的平均值并输出
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略，同时假设这些坐标都在某个圆上或附近
        /// </summary>
        /// <param name="filePath">读取XY坐标内容的文件的完整文件路径</param>
        /// <param name="groupsOfCoors">从文件内容中转换的XY坐标数组的列表</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        public static Circle GetAveragedCircleNumbers3d(string filePath, out List<double[]> groupsOfCoors)
        {
            return GetAveragedCircleNumbers(filePath, Dimensions.Three, out groupsOfCoors);
        }
        #endregion

        #region 二维空间中圆上3点计算圆心坐标与半径长度
        /// <summary>
        /// 读取给定文件路径的文件内容，每行转换为一组XY坐标的数组（数字间用tab制表符、半角逗号或空格分隔）、每个数组再作一个集合，用这些集合中的坐标计算圆心XY坐标和半径长度的平均值并输出
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略，同时假设这些坐标都在某个圆上或附近
        /// </summary>
        /// <param name="filePath">读取XY坐标内容的文件的完整文件路径</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        public static Circle GetAveragedCircleNumbers(string filePath)
        {
            return GetAveragedCircleNumbers(filePath, Dimensions.Two, out _);
        }

        /// <summary>
        /// 读取给定文件路径的文件内容，每行转换为一组XY坐标的数组（数字间用tab制表符、半角逗号或空格分隔）、每个数组再作一个集合，用这些集合中的坐标计算圆心XY坐标和半径长度的平均值并输出
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略，同时假设这些坐标都在某个圆上或附近
        /// </summary>
        /// <param name="filePath">读取XY坐标内容的文件的完整文件路径</param>
        /// <param name="groupsOfCoors">从文件内容中转换的XY坐标数组的列表</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        public static Circle GetAveragedCircleNumbers(string filePath, out List<double[]> groupsOfCoors)
        {
            return GetAveragedCircleNumbers(filePath, Dimensions.Two, out groupsOfCoors);
        }

        #region 旧计算方法
        ///// <summary>
        ///// 根据圆上3个点的XY坐标确定圆心的XY坐标和半径长度
        ///// </summary>
        ///// <param name="x1">第1点的X坐标</param>
        ///// <param name="y1">第1点的Y坐标</param>
        ///// <param name="x2">第2点的X坐标</param>
        ///// <param name="y2">第2点的Y坐标</param>
        ///// <param name="x3">第3点的X坐标</param>
        ///// <param name="y3">第3点的Y坐标</param>
        ///// <param name="centrex">圆心X坐标</param>
        ///// <param name="centrey">圆心Y坐标</param>
        ///// <param name="radius">圆周半径长度，单位与坐标轴单位相同</param>
        //public static void GetCircleNumbers(double x1, double y1, double x2, double y2, double x3, double y3, out double centrex, out double centrey, out double radius)
        //{
        //    double slope1 = (x2 - x1) / (y1 - y2);
        //    double slope2 = (x3 - x2) / (y2 - y3);

        //    double mid_x1 = (x1 + x2) / 2;
        //    double mid_y1 = (y1 + y2) / 2;
        //    double mid_x2 = (x2 + x3) / 2;
        //    double mid_y2 = (y2 + y3) / 2;

        //    centrex = (slope1 * mid_x1 - slope2 * mid_x2 + mid_y2 - mid_y1) / (slope1 - slope2);
        //    centrey = slope1 * (centrex - mid_x1) + mid_y1;
        //    radius = Math.Sqrt(Math.Pow(centrex - x1, 2) + Math.Pow(centrey - y1, 2));
        //}
        #endregion
        #endregion

        #region 文本转换为数值
        /// <summary>
        /// 根据给定的数值区间范围描述字符串（形如“-3.4~-1.2, 6.772~10.838”）输出成对的区间范围
        /// </summary>
        /// <param name="descp">数值区间范围的描述字符串（形如“-3.4~-1.2, 6.772~10.838”）</param>
        /// <param name="splitChar">描述数字分隔关系的字符，默认为“,”</param>
        /// <param name="intervalChar">描述区间起始结束关系的连接字符，默认为~（为避免负数不要使用-）</param>
        /// <returns></returns>
        public static List<double[]> GetNumberPairsByString(string descp, char splitChar = ',', char intervalChar = '~')
        {
            var list = new List<double[]>();
            if (string.IsNullOrWhiteSpace(descp))
                goto END;
            string[] splits1 = descp.Split(new char[] { splitChar }, StringSplitOptions.RemoveEmptyEntries);
            if (splits1 == null || splits1.Length == 0)
                goto END;
            foreach (var split in splits1)
            {
                string[] splits2 = split.Trim().Split(new char[] { intervalChar }, StringSplitOptions.RemoveEmptyEntries);
                //假如每一段区间描述字符串用连接字符分割后的子字符串数量小于2，或者甚至于没有连接字符（这时字符串数量至多为1），则忽略
                if (splits2 == null || splits2.Length < 2 || !double.TryParse(splits2[0], out double start) || !double.TryParse(splits2[1], out double end))
                    continue;
                if (end < start)
                    MathExtension.Swap(ref start, ref end);
                list.Add(new double[] { start, end });
            }
        END:
            return list;
        }

        /// <summary>
        /// 根据给定的整数序列描述字符串（形如“-5, -3~-1, 3, 6~10”）输出整数序列
        /// </summary>
        /// <param name="descp">整数序列的描述字符串（形如“-5, -3~-1, 3, 6~10”）</param>
        /// <param name="splitChar">描述数字分隔关系的字符，默认为“,”</param>
        /// <param name="intervalChar">描述区间起始结束关系的字符，默认为~（为避免负数不要使用-）</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<int> GetIntegerListByString(string descp, char splitChar = ',', char intervalChar = '~')
        {
            var list = new List<int>();
            if (string.IsNullOrWhiteSpace(descp))
                goto END;
            string[] splits1 = descp.Split(new char[] { splitChar }, StringSplitOptions.RemoveEmptyEntries);
            if (splits1 == null || splits1.Length == 0)
                goto END;
            foreach (var split in splits1)
            {
                //string[] splits2 = split.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
                string[] splits2 = split.Trim().Split(new char[] { intervalChar }, StringSplitOptions.RemoveEmptyEntries);
                if (!int.TryParse(splits2[0], out int start))
                    continue;
                int end = splits2.Length >= 2 && int.TryParse(splits2[1], out end) ? end : start;
                if (end < start)
                    MathExtension.Swap(ref start, ref end);
                for (int i = start; i <= end; i++)
                    list.Add(i);
            }
        //list.Sort();
        END:
            return list;
        }

        /// <summary>
        /// 读取给定文件路径的文件内容，每行转换为包含若干数字的数组（数字间用tab制表符、半角逗号或空格分隔）、每个数组再作为一个集合并返回
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略
        /// </summary>
        /// <param name="filePath">读取数值内容的文件的完整文件路径</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        [Obsolete("请转用GetNumberArraysInFileContent方法")]
        public static List<double[]> GetNumbersInFileContent(string filePath)
        {
            return GetNumberArraysInFileContent(filePath);
        }

        /// <summary>
        /// 读取给定文件路径的文件内容，每行转换为包含若干数字的数组（数字间用tab制表符、半角逗号或空格分隔）、每个数组再作为一个集合并返回
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略
        /// </summary>
        /// <param name="filePath">读取数值内容的文件的完整文件路径</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        public static List<double[]> GetNumberArraysInFileContent(string filePath)
        {
            string[] lines;
            //var groupsOfNumbers = new List<double[]>();
            try { lines = File.ReadAllLines(filePath); }
            catch (ArgumentException e) { throw e; }
            catch (DirectoryNotFoundException e) { throw e; }
            //var lines = File.ReadAllLines(filePath);
            return GetNumberArraysInStringArray(lines);
            //if (lines == null || lines.Length == 0)
            //    return new List<double[]>();
            //var groupsOfNumbers = lines.Select(line => {
            //    //把每一行用tab制表符、半角逗号或空格分割并转换为数字（非数字则转为NaN）数组
            //    return line.Split(new char[] { '\t', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(part => {
            //        return double.TryParse(part, out double num) ? num : double.NaN;
            //    }).ToArray();
            //}).Where(group => !group.Any(member => double.IsNaN(member))).ToList(); //排除非数字
            //return groupsOfNumbers;
        }

        /// <summary>
        /// 读取给定的字符串，每行转换为包含若干数字的数组（数字间用tab制表符、半角逗号或空格分隔）、每个数组再作为一个集合并返回
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略
        /// </summary>
        /// <param name="content">读取数值内容的字符串</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        public static List<double[]> GetNumberArraysInString(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new List<double[]>();
            var lines = content.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            return GetNumberArraysInStringArray(lines);
        }

        /// <summary>
        /// 读取给定的字符串数组，每行转换为包含若干数字的数组（数字间用tab制表符、半角逗号或空格分隔）、每个数组再作为一个集合并返回
        /// <para/>假如某行除分隔符外有非数字，则该行将被忽略
        /// </summary>
        /// <param name="lines">读取数值内容的字符串数组</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <returns></returns>
        public static List<double[]> GetNumberArraysInStringArray(string[] lines)
        {
            if (lines == null || lines.Length == 0)
                return new List<double[]>();
            var groupsOfNumbers = lines
                //排除空字符串
                .Where(line => !string.IsNullOrWhiteSpace(line))
                //把每一行用tab制表符、半角逗号或空格分割并转换为数字（非数字则转为NaN）数组
                .Select(line => {
                    return line.Split(new char[] { '\t', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => double.TryParse(part, out double num) ? num : double.NaN).ToArray();
                })
                //排除包含非数字的数组
                .Where(group => !group.Any(member => double.IsNaN(member))).ToList();
            return groupsOfNumbers;
        }
        #endregion

        #region 数值转换为文本
        /// <summary>
        /// 根据给定的成对的区间范围，输出数值区间范围描述字符串（形如“-3.4~-1.2, 6.772~10.838”）
        /// </summary>
        /// <param name="list">数值区间范围的描述字符串（形如“-3.4~-1.2, 6.772~10.838”）</param>
        /// <param name="splitChar">描述数字分隔关系的字符，默认为“,”</param>
        /// <param name="intervalChar">描述区间起始结束关系的连接字符，默认为~（为避免负数不要使用-）</param>
        /// <returns></returns>
        public static string GetStringByNumberPairs(List<double[]> list, char splitChar = ',', char intervalChar = '~')
        {
            string output = string.Empty;
            if (list == null || list.Count == 0)
                goto END;
            output = string.Join(splitChar + " ",
                //描述区间关系的数组长度至少为2
                list.Where(pair => pair != null && pair.Length >= 2)
                .Select(pair => string.Join(intervalChar.ToString(), pair))
                );
        END:
            return output;
        }
        #endregion

        #region 计算各点间平均距离 / 排除离散点
        /// <summary>
        /// 从双精度浮点数(double)的集合中排除与其它值相差太大的值，同时需给定采样的比例（或数量）以及离群值的过滤系数
        /// </summary>
        /// <param name="points">待处理的双精度浮点数(double)集合</param>
        /// <param name="discarded">存放被过滤掉的双精度浮点数(double)的集合</param>
        /// <param name="dist_ex_count">计算每个值距其它值的平均差距时，所保留的值数量，假如大于0小于1，则为比例（假如小于等于0则强制设置数量为1）</param>
        /// <param name="removalCoeff">离群值的过滤系数，标准值为1，越小越严格，小于等于0将过滤掉所有点</param>
        /// <returns>返回计算出的各点间平均距离</returns>
        public static double RemoveDistantPoints(ref List<double> points, ref List<double> discarded, double dist_ex_count, double removalCoeff)
        {
            return RemoveDistantPoints(ref points, ref discarded, d => new double[] { d }, 1, dist_ex_count, removalCoeff);
        }

        /// <summary>
        /// 从一些样本的集合中排除离散的样本，判断依据是距其它样本的平均距离，用于计算此距离的坐标（以double数组的形式）可以通过给定的转换函数从样本元素中投影出来，同时需给定采样的比例（或数量）以及离群点的过滤系数
        /// </summary>
        /// <param name="points">待处理的样本集合，样本中的每个元素可以通过提供的转换函数投影到最终计算平均距离的数值坐标上（以double数组的形式）</param>
        /// <param name="discarded">存放被过滤掉的样本的集合</param>
        /// <param name="selector">应用于每个元素的转换函数</param>
        /// <param name="dimension">点所在空间的维度，至少为1</param>
        /// <param name="dist_ex_count">计算每个点距其它所有点的平均距离时，所保留的样本点的数量，假如大于0小于1，则为比例（假如小于等于0则强制设置数量为1）</param>
        /// <param name="removalCoeff">离群点过滤系数，标准值为1，越小越严格，小于等于0将过滤掉所有点</param>
        /// <returns>返回计算出的各点间平均距离</returns>
        public static double RemoveDistantPoints<TSource>(ref List<TSource> points, ref List<TSource> discarded, Func<TSource, double[]> selector, int dimension, double dist_ex_count, double removalCoeff)
        {
            var innerPoints = points.Select(selector);
            double distAvr = removalCoeff * innerPoints.GetAverageDistance(dimension, dist_ex_count, out Dictionary<int, double> dict);
            if (discarded == null)
                discarded = new List<TSource>();
            //points.RemoveAll(p => p.DistToOthers > dist_avr);
            for (int i = points.Count - 1; i >= 0; i--)
            {
                var p = points[i];
                if (dict[i] <= distAvr)
                    continue;
                points.Remove(p);
                discarded.Add(p);
            }
            return distAvr;
        }

        /// <summary>
        /// 从一些点的集合中排除离散的点，判断依据是距其它点的平均距离，点的坐标以double数组的形式给出，同时需给定采样的比例（或数量）以及离群点的过滤系数
        /// </summary>
        /// <param name="points">待处理的点集合，每个元素代表点的坐标，以double数组的形式给出</param>
        /// <param name="dimension">点所在空间的维度，至少为1</param>
        /// <param name="dist_ex_count">计算每个点距其它所有点的平均距离时，所保留的样本点的数量，假如大于0小于1，则为比例（假如小于等于0则强制设置数量为1）</param>
        /// <param name="removalCoeff">离群点过滤系数，标准值为1，越小越严格，小于等于0将过滤掉所有点</param>
        /// <returns>返回计算出的各点间平均距离</returns>
        public static double RemoveDistantPoints(ref List<double[]> points, int dimension, double dist_ex_count, double removalCoeff)
        {
            var list = new List<double[]>();
            return RemoveDistantPoints(ref points, ref list, dimension, dist_ex_count, removalCoeff);
        }

        ///// <summary>
        ///// 从一些点的集合中排除离散的点，判断依据是距其它点的平均距离，点的坐标以double数组的形式给出，同时需给定采样的比例（或数量）以及离群点的过滤系数
        ///// </summary>
        ///// <param name="points">待处理的点集合</param>
        ///// <param name="discarded">存放被过滤掉的点的集合</param>
        ///// <param name="dimension">点所在空间的维度，至少为1</param>
        ///// <param name="dist_ex_count">计算每个点距其它所有点的平均距离时，所保留的样本点的数量，假如大于0小于1，则为比例（假如小于等于0则强制设置数量为1）</param>
        ///// <param name="removalCoeff">离群点过滤系数，标准值为1，越小越严格，小于等于0将过滤掉所有点</param>
        ///// <returns>返回计算出的各点间平均距离</returns>
        //public static double RemoveDistantPoints(ref List<double[]> points, ref List<double[]> discarded, int dimension, double dist_ex_count, double removalCoeff)
        //{
        //    double distAvr = 0;
        //    //至少需要3个点，而且维度至少为1，否则不需要排除
        //    int count;
        //    if (dimension < 1 || points == null || (count = points.Count()) < 3)
        //        goto END;
        //    if (discarded == null)
        //        discarded = new List<double[]>();
        //    //某一个点距离其它所有点的距离是样本池，计算从这个样本池中采样的数量，假如大于0小于1按比例计算，否则按绝对数量计算（至少为1）
        //    int ex_count = dist_ex_count > 0 && dist_ex_count < 1 ? (int)((count - 1) * dist_ex_count) : (int)dist_ex_count;
        //    ex_count = ex_count <= 0 ? 1 : ex_count;
        //    Dictionary<double[], double> dict = new Dictionary<double[], double>();
        //    List<int> dimensions = GetIntegerListByString("1~" + dimension);
        //    //List<double> distances = new List<double>();
        //    foreach (var pi in points)
        //    {
        //        if (pi == null || pi.Length < dimension) continue;
        //        List<double> listDists = new List<double>();
        //        foreach (var pj in points)
        //            //排除同一个点
        //            if (pj != null && pj.Length >= dimension && pi != pj)
        //                //计算与其它所有点的距离并储存
        //                listDists.Add(Math.Sqrt(dimensions.Select(d => Math.Pow(pj[d] - pi[d], 2)).Sum()));
        //        //找出排序靠前若干位的距离值并取平均值，记为单点距离平均值
        //        listDists.Sort();
        //        listDists = listDists.Take(ex_count).ToList();
        //        dict.Add(pi, listDists.Average());
        //    }
        //    //求所有点的单点距离平均值的平均值，记为全局平均值
        //    //double dist_avr = dict.Count == 0 ? 0 : dict.Values.Average() * removalCoeff;
        //    distAvr = dict.Count == 0 ? 0 : dict.Values.Average() * removalCoeff;
        //    //points.RemoveAll(p => p.DistToOthers > dist_avr);
        //    for (int i = points.Count - 1; i >= 0; i--)
        //    {
        //        var p = points[i];
        //        if (dict[p] <= distAvr)
        //            continue;
        //        points.Remove(p);
        //        discarded.Add(p);
        //    }
        //    END:
        //    return distAvr;
        //}

        /// <summary>
        /// 从一些点的集合中排除离散的点，判断依据是距其它点的平均距离，点的坐标以double数组的形式给出，同时需给定采样的比例（或数量）以及离群点的过滤系数
        /// </summary>
        /// <param name="points">待处理的点集合，每个元素代表点的坐标，以double数组的形式给出</param>
        /// <param name="discarded">存放被过滤掉的点的集合</param>
        /// <param name="dimension">点所在空间的维度，至少为1</param>
        /// <param name="dist_ex_count">计算每个点距其它所有点的平均距离时，所保留的样本点的数量，假如大于0小于1，则为比例（假如小于等于0则强制设置数量为1）</param>
        /// <param name="removalCoeff">离群点过滤系数，标准值为1，越小越严格，小于等于0将过滤掉所有点</param>
        /// <returns>返回计算出的各点间平均距离</returns>
        public static double RemoveDistantPoints(ref List<double[]> points, ref List<double[]> discarded, int dimension, double dist_ex_count, double removalCoeff)
        {
            double distAvr = removalCoeff * points.GetAverageDistance(dimension, dist_ex_count, out Dictionary<int, double> dict);
            if (discarded == null)
                discarded = new List<double[]>();
            //points.RemoveAll(p => p.DistToOthers > dist_avr);
            for (int i = points.Count - 1; i >= 0; i--)
            {
                var p = points[i];
                if (dict[i] <= distAvr)
                    continue;
                points.Remove(p);
                discarded.Add(p);
            }
            return distAvr;
        }
        #endregion

        /// <summary>
        /// 使输入的方位角保持在合理的范围之内（-180°到180°之间，包含-180°，不包含180°），假如超过180°则减360°，小于-180°则加360°，循环计算直到进入范围为止
        /// </summary>
        /// <param name="angle">待修改的输入方位角</param>
        public static void KeepAzimuthInRange(ref double angle)
        {
            //假如为180°，修改为-180°，确保同一个位置不会出现2种值
            if (angle == 180)
                angle = -180;
            //假如绝对值大于180，则向当前值符号相反的方向修正360°，直到在范围内为止
            while (Math.Abs(angle) > 180)
                angle -= 360 * Math.Sign(angle);
        }

        /// <summary>
        /// 根据给定的顶端以及回转处坐标计算（初步的）回转角度（y轴正向为回转0°方位）
        /// </summary>
        /// <param name="x1">顶端X坐标</param>
        /// <param name="y1">顶端Y坐标</param>
        /// <param name="xa">回转处X坐标</param>
        /// <param name="ya">回转处Y坐标</param>
        /// <returns></returns>
        public static double GetAngleByCoordinates(double x1, double y1, double xa, double ya)
        {
            //两点距离，加上一个极小值防止为0
            double xdiff = x1 - xa, ydiff = y1 - ya, dist = Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) + 1e-20;
            //初步角度，值区间为[-90, 90]
            double angle = Math.Asin(xdiff / dist) * 180 / Math.PI;
            //假如y坐标差为负值，则需要相对X轴做对称
            if (ydiff < 0)
                angle = 180 * (xdiff > 0 ? 1 : -1) - angle;
            return angle;
        }
    }
}
