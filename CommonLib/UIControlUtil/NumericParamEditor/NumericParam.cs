using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.UIControlUtil.NumericParamEditor
{
    /// <summary>
    /// 描述一个可编辑的数值参数，与具体业务实体解耦。
    /// 通过 getter/setter 委托读写宿主对象 T 上的某个 double 字段/属性。
    /// </summary>
    /// <typeparam name="T">承载该参数的宿主对象类型</typeparam>
    public sealed class NumericParam<T>
    {
        /// <summary>
        /// 参数名（唯一标识），也用于控件 Tag 匹配
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 单次步进调整量（正数）
        /// </summary>
        public double Step { get; }

        /// <summary>
        /// 参数值下限（含）
        /// </summary>
        public double Min { get; }

        /// <summary>
        /// 参数值上限（含）
        /// </summary>
        public double Max { get; }

        /// <summary>
        /// 显示时保留的小数位数
        /// </summary>
        public int DecimalPlaces { get; }

        /// <summary>
        /// 读取宿主对象 T 上的参数值
        /// </summary>
        public Func<T, double> Getter { get; }

        /// <summary>
        /// 向宿主对象 T 写入参数值
        /// </summary>
        public Action<T, double> Setter { get; }

        /// <summary>
        /// 构造一个数值参数描述
        /// </summary>
        /// <param name="name">参数名（唯一标识）</param>
        /// <param name="step">单次步进量（正数）</param>
        /// <param name="min">下限（含）</param>
        /// <param name="max">上限（含）</param>
        /// <param name="decimalPlaces">显示小数位数</param>
        /// <param name="getter">读取委托</param>
        /// <param name="setter">写入委托</param>
        /// <exception cref="ArgumentNullException">name/getter/setter 为空</exception>
        /// <exception cref="ArgumentOutOfRangeException">step 非正数，或 min &gt; max</exception>
        public NumericParam(string name, double step, double min, double max, int decimalPlaces,
            Func<T, double> getter, Action<T, double> setter)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (step <= 0)
                throw new ArgumentOutOfRangeException(nameof(step), "步长必须为正数");
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), "下限不能大于上限");

            Name = name;
            Step = step;
            Min = min;
            Max = max;
            DecimalPlaces = decimalPlaces;
            Getter = getter ?? throw new ArgumentNullException(nameof(getter));
            Setter = setter ?? throw new ArgumentNullException(nameof(setter));
        }

        /// <summary>
        /// 读取宿主对象 T 上的当前值
        /// </summary>
        public double Get(T host) => Getter(host);

        /// <summary>
        /// 向宿主对象 T 写入值
        /// </summary>
        public void Set(T host, double value) => Setter(host, value);

        /// <summary>
        /// 将任意值钳制到 [Min, Max] 区间
        /// </summary>
        public double Clamp(double value)
        {
            if (value < Min) return Min;
            if (value > Max) return Max;
            return value;
        }

        /// <summary>
        /// 在宿主当前值基础上按 direction 方向步进一次，返回钳制后的新值（不写回）。
        /// </summary>
        /// <param name="host">宿主对象</param>
        /// <param name="direction">方向，正数增大、负数减小</param>
        public double StepValue(T host, double direction)
        {
            return Clamp(Get(host) + direction * Step);
        }

        /// <summary>
        /// 将数值格式化为显示文本（按 DecimalPlaces 保留小数）
        /// </summary>
        public string Format(double value)
        {
            return value.ToString("F" + DecimalPlaces);
        }
    }
}
