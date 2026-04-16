using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
#if NET45
    /// <summary>
    /// 样本分析
    /// </summary>
    public class SampleAnalyzer
    {
        /// <summary>
        /// 区间长度
        /// </summary>
        public int SectionLength { get; set; }

        /// <summary>
        /// 最新的错误信息
        /// </summary>
        public string LastErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="sample_length"></param>
        public SampleAnalyzer(int sample_length)
        {
            SectionLength = sample_length <= 0 ? 1 : sample_length;
        }
#elif NET9_0_OR_GREATER
    /// <summary>
    /// 样本分析
    /// </summary>
    /// <remarks>
    /// 构造器
    /// </remarks>
    /// <param name="sample_length"></param>
    public class SampleAnalyzer(int sample_length)
    {
        /// <summary>
        /// 区间长度
        /// </summary>
        public int SectionLength { get; set; } = sample_length <= 0 ? 1 : sample_length;

        /// <summary>
        /// 最新的错误信息
        /// </summary>
        public string LastErrorMessage { get; set; } = string.Empty;
#endif

        /// <summary>
        /// 获取样本区间内的极值
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<double> GetSectionExtremeValues(IEnumerable<double> list)
        {
            //if (list == null || !list.Any())
            //    LastErrorMessage = "样本为空";
#if NET45
            if (list == null) list = new List<double>();
#elif NET9_0_OR_GREATER
            list ??= [];
#endif
            if (!list.Any())
                LastErrorMessage = "不包含任何样本";

            if (!string.IsNullOrWhiteSpace(LastErrorMessage))
                throw new ArgumentException(LastErrorMessage, nameof(list));

            int groupNumber = (int)Math.Ceiling((double)list.Count() / SectionLength);
#if NET45
            List<double> result = new List<double>();
#elif NET9_0_OR_GREATER
            List<double> result = [];
#endif
            for (int i = 0; i < groupNumber; i++)
            {
                double extreme = 0;
                for (int j = 0; j < SectionLength; j++)
                {
                    int index = i * SectionLength + j;
                    //假如已遍历完所有元素，脱出循环
                    if (index >= list.Count())
                        break;
                    extreme = Math.Abs(list.ElementAt(index)) > extreme ? list.ElementAt(index) : extreme; //迭代极值
                }
                result.Add(extreme);
            }
            return result;
        }
    }
}
