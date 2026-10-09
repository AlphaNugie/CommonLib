using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.UIControlUtil.NumericParamEditor
{
    /// <summary>
    /// 一组 <see cref="NumericParam{T}"/> 的集合，提供按名字/Tag 查找与批量操作。
    /// 该层只做纯逻辑，不依赖任何 UI 框架，可由 WPF、WinForms、控制台等任意宿主复用。
    /// </summary>
    /// <typeparam name="T">承载参数的对象类型</typeparam>
    public sealed class NumericParamTable<T> : IEnumerable<NumericParam<T>>
    {
        private readonly List<NumericParam<T>> _params = new List<NumericParam<T>>();
        private readonly Dictionary<string, NumericParam<T>> _byName
            = new Dictionary<string, NumericParam<T>>(StringComparer.Ordinal);

        /// <summary>参数数量</summary>
        public int Count => _params.Count;

        /// <summary>
        /// 按索引获取参数
        /// </summary>
        public NumericParam<T> this[int index] => _params[index];

        /// <summary>
        /// 添加一个参数。参数名重复时抛出异常。
        /// </summary>
        public void Add(NumericParam<T> param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (_byName.ContainsKey(param.Name))
                throw new ArgumentException($"参数名 \"{param.Name}\" 已存在", nameof(param));

            _params.Add(param);
            _byName.Add(param.Name, param);
        }

        /// <summary>
        /// 按参数名精确查找，找不到返回 null
        /// </summary>
        public NumericParam<T> Find(string name)
        {
            if (name == null) return null;
            _byName.TryGetValue(name, out var p);
            return p;
        }

        /// <summary>
        /// 按控件 Tag 查找参数。
        /// Tag 可为纯参数名（"Roll"）或带方向（"Roll:1"），统一取冒号前的名字部分匹配。
        /// </summary>
        public NumericParam<T> FindByTag(object tag)
        {
            if (tag == null) return null;
            string raw = tag.ToString();
            int idx = raw.IndexOf(':');
            string name = idx >= 0 ? raw.Substring(0, idx) : raw;
            return Find(name);
        }

        /// <summary>
        /// 从 Tag 中解析方向（冒号后部分），"1" 或正数返回 +1，否则返回 -1。
        /// </summary>
        public static double ParseDirection(object tag, double positiveDirection = 1.0)
        {
            if (tag == null) return 1.0;
            string raw = tag.ToString();
            int idx = raw.IndexOf(':');
            if (idx < 0 || idx + 1 >= raw.Length) return 1.0;

            string dir = raw.Substring(idx + 1);
            if (double.TryParse(dir, out double d))
                return d > 0 ? positiveDirection : -positiveDirection;
            return -positiveDirection;
        }

        /// <summary>
        /// 获取参数集合的枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<NumericParam<T>> GetEnumerator() => _params.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _params.GetEnumerator();
    }
}
