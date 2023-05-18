using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Function;

namespace CommonLib.Clients
{
    /// <summary>
    /// 滤波功能对象
    /// </summary>
    public class DataFilterClient
    {
        private ushort wing = 3;

        /// <summary>
        /// 邻域翼展（半径），默认为3
        /// </summary>
        public ushort Wing
        {
            get { return wing; }
            set
            {
                wing = value;
                Neighbours = (short)((DoubleWinged ? 2 : 1) * wing + 1);
            }
        }

        /// <summary>
        /// 是否为双翼（从中心向两侧延伸），假如为false则为单翼（从中心向左侧延伸），默认为true
        /// </summary>
        public bool DoubleWinged { get; set; } = true;

        /// <summary>
        /// 邻域大小，假如为双翼，则应为奇数
        /// </summary>
        public short Neighbours { get; private set; }

        /// <summary>
        /// 滤波类型
        /// </summary>
        public FilterType Type { get; private set; }

        /// <summary>
        /// 高斯分布计算器
        /// </summary>
        public GaussianCalculator GausCalc { get; set; }

        /// <summary>
        /// 最新的错误信息
        /// </summary>
        public string LastErrorMessage { get; set; }

        ///// <summary>
        ///// 构造器
        ///// </summary>
        ///// <param name="wing">邻域翼展（半径），假如邻域长度为7，则半径应为3</param>
        ///// <param name="type">滤波类型</param>
        //public DataFilterClient(byte wing, FilterType type) : this(wing, type, 0) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="wing">邻域翼展（半径），假如半径为3，当领域为双翼时邻域长度为7，否则邻域长度为4</param>
        /// <param name="type">滤波类型</param>
        /// <param name="sigma">高斯分布标准差，标准差越大，数据分布越分散，反之数据分布越集中</param>
        /// <param name="double_winged">样本中心两侧是双翼还是单翼</param>
        public DataFilterClient(ushort wing, FilterType type, double sigma = 4.5, bool double_winged = true)
        {
            Wing = wing;
            Type = type;
            DoubleWinged = double_winged;
            GausCalc = new GaussianCalculator(1, 0, sigma);
        }

        /// <summary>
        /// 获取滤波后的数据
        /// </summary>
        /// <param name="samples">待滤波的样本</param>
        /// <returns></returns>
        public List<double> GetFilteredSamples(IEnumerable<double> samples)
        {
            if (samples == null)
                LastErrorMessage = "样本点为空";
            //样本数若小于领域大小，退出
            else if (samples.Count() < Neighbours)
                LastErrorMessage = "样本数小于邻域大小";

            if (!string.IsNullOrWhiteSpace(LastErrorMessage))
                throw new ArgumentException(LastErrorMessage, nameof(samples));
            
            List<double> result = new List<double>(); //储存结果的List
            var count = samples.Count();
            double element, element_new = 0;

            for (var i = 0; i < count; i++)
            {
                element = samples.ElementAt(i);
                //边缘的样本不处理，直接返回
                if (i < wing || i >= count - (DoubleWinged ? wing : 0))
                {
                    result.Add(element);
                    continue;
                }
                List<double> medianSamples = GetNeighbourSamples(samples, i);

                if (Type == FilterType.Average) //均值
                    element_new = medianSamples.Average();
                else if (Type == FilterType.Median) //中值
                    element_new = GetMedianNumber(medianSamples);
                else if (Type == FilterType.Gaussian)
                    element_new = GetGaussianValue(medianSamples);
                result.Add(element_new);
            }

            return result;
        }

        /// <summary>
        /// 在样本中根据样本索引找出邻域样本
        /// </summary>
        /// <param name="samples">样本集合</param>
        /// <param name="index">中心样本索引</param>
        /// <returns></returns>
        public List<double> GetNeighbourSamples(IEnumerable<double> samples, int index)
        {
            if (samples == null || samples.Count() < Neighbours)
                LastErrorMessage = "样本点为空";
            //样本数若小于领域大小，退出
            else if (samples.Count() < Neighbours)
                LastErrorMessage = "样本数小于邻域大小";
            else if (/*index < 0 || */index >= samples.Count())
                LastErrorMessage = "索引大小超出范围";

            if (!string.IsNullOrWhiteSpace(LastErrorMessage))
                throw new ArgumentException(LastErrorMessage);

            var count = samples.Count();
            List<double> array = new List<double>();
            for (var i = index - Wing; i <= index + (DoubleWinged ? Wing : 0); i++)
            {
                //当邻域范围超出左侧或右侧时，改变索引值（向右或向左找）
                var temp = (i < 0 || i >= count) ? (i < 0 ? count + i : i - count) : i;
                array.Add(samples.ElementAt(temp));
            }

            return array;
        }

        /// <summary>
        /// 获取样本的中值
        /// </summary>
        /// <param name="samples">样本</param>
        /// <returns></returns>
        public double GetMedianNumber(IEnumerable<double> samples)
        {
            if (samples == null || samples.Count() == 0)
                LastErrorMessage = "样本中没有任何元素";

            if (!string.IsNullOrWhiteSpace(LastErrorMessage))
                throw new ArgumentException(LastErrorMessage, "samples");

            samples = samples.OrderBy(sample => sample);
            int count = samples.Count();

            //奇数个样本取中值，否则取中间两值的平均值
            if (count % 2 == 0)
                return (samples.ElementAt(count / 2 - 1) + samples.ElementAt(count / 2)) / 2;
            else
                return samples.ElementAt((count - 1) / 2);
        }

        /// <summary>
        /// 根据高斯分布（中心样本为中心点）求各位置样本系数并加权平均
        /// </summary>
        /// <param name="samples">样本</param>
        /// <returns></returns>
        public double GetGaussianValue(IEnumerable<double> samples)
        {
            return GetGaussianValue(samples, out _);
        }

        /// <summary>
        /// 根据高斯分布（中心样本为中心点）求各位置样本系数并加权平均
        /// </summary>
        /// <param name="samples">样本</param>
        /// <param name="ratios">计算结束后返回的每个样本根据高斯分布得出的系数</param>
        /// <returns></returns>
        public double GetGaussianValue(IEnumerable<double> samples, out List<double> ratios)
        {
            ratios = null;
            if (samples == null || samples.Count() == 0)
                LastErrorMessage = "样本中没有任何元素";

            if (!string.IsNullOrWhiteSpace(LastErrorMessage))
                throw new ArgumentException(LastErrorMessage, "samples");

            double center = (samples.Count() - 1) / (DoubleWinged ? 2 : 1), ratio_sum = 0, value_sum = 0;
            ratios = new List<double>();
            for (int i = 0; i < samples.Count(); i++)
            {
                double ratio = GausCalc.Calc(i - center); //高斯分布在某坐标的值（center处的值为最高点）
                ratio_sum += ratio;
                value_sum += ratio * samples.ElementAt(i);
                ratios.Add(ratio);
            }
            return value_sum / ratio_sum;
        }
    }

    /// <summary>
    /// 滤波类型
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// 均值滤波
        /// </summary>
        Average = 1,

        /// <summary>
        /// 中值滤波
        /// </summary>
        Median = 2,

        /// <summary>
        /// 高斯滤波
        /// </summary>
        Gaussian = 3
    }
}
