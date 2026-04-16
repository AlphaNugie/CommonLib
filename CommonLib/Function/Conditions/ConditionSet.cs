using CommonLib.Extensions.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonLib.Function.Conditions
{
    /// <summary>
    /// 条件集合，在数据源 <see cref="DataSource"/> 内寻找 <see cref="FieldName"/>，并与给定值 <see cref="Value"/> 进行比较，比较方式由比较符号 <see cref="ComparisonSymbol"/> 决定
    /// </summary>
    public class ConditionSet
    {
        #region 属性
        /// <summary>
        /// 数据源 <see cref="DataSource"/> 的实体类内指定属性的名称，形如“Class.Student.Name”
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 数据源，会在此数据源内寻找 <see cref="FieldName"/>，并与给定值 <see cref="Value"/> 进行比较，比较方式由比较符号 <see cref="ComparisonSymbol"/> 决定
        /// <para/>可以是任何类型，如OpcDatasource实体类、OpcDaItem实体类、OpcDaGroup实体类等
        /// </summary>
        public object DataSource { get; set; }

        /// <summary>
        /// 比较符号，如“=”、“>”、“&lt;”、“>=”、“&lt;=”、“!=”
        /// </summary>
        public string ComparisonSymbol { get; set; }

        /// <summary>
        /// 用于和指定属性值进行比较的值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 条件命中后需要展示的描述性字符串
        /// </summary>
        public string ConditionHitDescription { get; set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="comparisonSymbol"></param>
        /// <param name="value"></param>
        /// <param name="condHitDescr"></param>
        /// <param name="dataSource"></param>
        public ConditionSet(string fieldName, string comparisonSymbol, object value, string condHitDescr, object dataSource = null)
        {
            FieldName = fieldName;
            ComparisonSymbol = comparisonSymbol;
            Value = value;
            ConditionHitDescription = condHitDescr;
            DataSource = dataSource;
        }

        /// <summary>
        /// 构造函数
        /// 条件字符串格式：[属性名][比较符号][值],[条件命中字符串]
        /// </summary>
        /// <param name="conditionString"></param>
        /// <param name="dataSource"></param>
        /// <exception cref="ArgumentException"></exception>
        public ConditionSet(string conditionString, object dataSource = null)
        {
            conditionString = conditionString.Trim();
            var nameParts = conditionString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length < 2)
                goto INVALID_FORMAT;
            ConditionHitDescription = nameParts[1].Trim();
            var comparisonPart = nameParts[0].Trim();
            //var match = Regex.Match(comparisonPart, @"^(\w+)(=|>|<|>=|<=)(.*)$");
            // 这里使用正则表达式来匹配比较字符串，以提高匹配效率
            var match = Regex.Match(comparisonPart, @"(?:=>|>=|=<|<=|<|>|!=|=)");
            if (!match.Success)
                goto INVALID_FORMAT;
            FieldName = comparisonPart.Substring(0, match.Index).Trim();
            ComparisonSymbol = match.Value;
            Value = comparisonPart.Substring(match.Index + match.Length).Trim();
            DataSource = dataSource;
            return;

        INVALID_FORMAT:
            throw new ArgumentException("Invalid condition string format.");
        }

        /// <summary>
        /// 根据属性值反向构建格式化字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}, {3}", FieldName, ComparisonSymbol, Value, ConditionHitDescription);
        }

        /// <summary>
        /// 数据源 <see cref="DataSource"/> 内的属性 <see cref="FieldName"/> 的值与给定值 <see cref="Value"/> 进行比较并返回比较结果，比较方式由比较符号 <see cref="ComparisonSymbol"/> 决定
        /// </summary>
        /// <returns></returns>
        public bool IsMatch()
        {
            return IsMatch(out _);
        }

        /// <summary>
        /// 数据源 <see cref="DataSource"/> 内的属性 <see cref="FieldName"/> 的值与给定值 <see cref="Value"/> 进行比较并返回比较结果，比较方式由比较符号 <see cref="ComparisonSymbol"/> 决定
        /// </summary>
        /// <param name="targetType">数据源 <see cref="DataSource"/> 内的属性 <see cref="FieldName"/> 的类型</param>
        /// <returns></returns>
        public bool IsMatch(out Type targetType)
        {
            targetType = null;
            if (DataSource == null) return false;
            object targetValue = DataSource.GetPropertyValue(FieldName, out targetType);
            return IsMatch(targetValue);
        }

        /// <summary>
        /// 传入的值是否满足条件（进行对应符号的比较）
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public bool IsMatch(object targetValue)
        {
            if (targetValue == null || Value == null)
                return false;

            // 获取目标值的实际类型
            Type targetType = targetValue.GetType();

            //// 双重类型检查：确认值对象实现了 IComparable
            //if (!(targetValue is IComparable comparable) ||
            //    !Value.GetType().IsAssignableFrom(targetType))
            IComparable comparable;
            try { comparable = (IComparable)targetValue; }
            catch (Exception) { return false; }
            //// 双重类型检查：确认值对象实现了 IComparable
            //if (!Value.GetType().IsAssignableFrom(targetType))
            //    return false;

            try
            {
                // 尝试将比较值转换为目标类型
                object convertedValue = Convert.ChangeType(Value, targetType);

                // 执行实际比较（这里以 ">" 为例）
                switch (ComparisonSymbol)
                {
                    case ">":
                        return comparable.CompareTo(convertedValue) > 0;
                    case "<":
                        return comparable.CompareTo(convertedValue) < 0;
                    case ">=":
                    case "=>":
                        return comparable.CompareTo(convertedValue) >= 0;
                    case "<=":
                    case "=<":
                        return comparable.CompareTo(convertedValue) <= 0;
                    case "=":
                        return comparable.CompareTo(convertedValue) == 0;
                    case "!=":
                        return comparable.CompareTo(convertedValue) != 0;
                    default:
                        return false;
                }
            }
            catch (InvalidCastException)
            {
                // 类型转换失败
                return false;
            }
            catch (FormatException)
            {
                // 值格式不匹配
                return false;
            }
            catch (OverflowException)
            {
                // 数值溢出
                return false;
            }
        }
    }
}
