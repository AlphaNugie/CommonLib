using System;
using System.Collections.Generic;

namespace CommonLib.DataUtil
{
    /// <summary>
    /// SQL SET子句构建器，支持链式调用，减少重复的 string.Format 代码
    /// </summary>
    public class SetClauseBuilder
    {
        private readonly List<string> _list;

        /// <summary>
        /// 无参构造器，内部自动创建列表
        /// </summary>
        public SetClauseBuilder()
        {
            _list =
#if NET45_OR_GREATER
                new List<string>();
#elif NET9_0_OR_GREATER
                [];
#endif
        }

        /// <summary>
        /// 构造器（传入外部列表）
        /// </summary>
        /// <param name="list">目标列表</param>
        public SetClauseBuilder(List<string> list)
        {
            _list = list ?? throw new ArgumentNullException(nameof(list));
        }

        /// <summary>
        /// 添加一个不带格式化的SET子句，如 "column = value"
        /// </summary>
        /// <param name="column">列名</param>
        /// <param name="value">值</param>
        public SetClauseBuilder Add(string column, object value)
        {
            _list.Add(string.Format("{0} = {1}", column, value));
            return this;
        }

        /// <summary>
        /// 添加一个带格式化的SET子句，如 "column = 123.456"（f3格式）
        /// </summary>
        /// <param name="column">列名</param>
        /// <param name="value">值</param>
        /// <param name="format">格式化字符串，如 "f3"</param>
        public SetClauseBuilder AddFormatted(string column, object value, string format)
        {
            _list.Add(string.Format("{0} = {1:" + format + "}", column, value));
            return this;
        }

        /// <summary>
        /// 添加一个带引号字符串值的SET子句，如 "quality = 'good'"
        /// </summary>
        /// <param name="column">列名</param>
        /// <param name="value">字符串值</param>
        public SetClauseBuilder AddQuoted(string column, string value)
        {
            _list.Add(string.Format("{0} = '{1}'", column, value));
            return this;
        }

        /// <summary>
        /// 添加一个原始的SET子句（不进行任何格式化处理）
        /// </summary>
        /// <param name="rawClause">原始子句</param>
        public SetClauseBuilder AddRaw(string rawClause)
        {
            _list.Add(rawClause);
            return this;
        }

        /// <summary>
        /// 用逗号+换行拼接所有SET子句并返回
        /// </summary>
        public string Build()
        {
            return string.Join(",\n  ", _list);
        }
    }
}
