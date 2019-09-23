using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
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
        public string LastErrorMessage { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="sample_length"></param>
        public SampleAnalyzer(int sample_length)
        {
            this.SectionLength = sample_length <= 0 ? 1 : sample_length;
        }

        /// <summary>
        /// 获取样本区间内的极值
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<double> GetSectionExtremeValues(IEnumerable<double> list)
        {
            if (list == null || list.Count() == 0)
                this.LastErrorMessage = "样本为空";

            if (!string.IsNullOrWhiteSpace(this.LastErrorMessage))
                throw new ArgumentException(this.LastErrorMessage, "list");

            int groupNumber = (int)Math.Ceiling((double)list.Count() / this.SectionLength);
            List<double> result = new List<double>();
            for (int i = 0; i < groupNumber; i++)
            {
                double extreme = 0;
                for (int j = 0; j < this.SectionLength; j++)
                {
                    int index = i * this.SectionLength + j;
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
