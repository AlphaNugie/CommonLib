using CommonLib.Extensions;
using CommonLib.Extensions.Property;
using CommonLib.Extensions.Reflection;
using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OpcLibrary
{
    /// <summary>
    /// OPC项信息实体类，每个OPC项信息实体对应单独的OPC项ID、服务端句柄、客户端句柄以及值（供读取或写入）
    /// 可根据这些信息为OPC组添加OPC项（OpcGroupInfo中的SetItems方法）
    /// </summary>
    public class OpcItemInfo
    {
        private object _upperLevelEntity; //对应属性所属的实体
        private object _midLevelEntity; //指定属性所对应或所属的中间实体，假如属性名称内带有索引，则为索引处元素所属的集合、数组或List
        //private object _lowerLevelEntity; //直接与对应属性相关联的当前层实体
        private object _currentEntity; //直接与对应属性相关联的当前层实体，实际使用中就是最终对应的值或实体
        private int[] _indices = null; //方括号索引

        #region 属性
        /// <summary>
        /// OPC项ID（名称）
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// OPC服务端句柄
        /// </summary>
        public int ServerHandle { get; set; }

        /// <summary>
        /// OPC客户端句柄
        /// </summary>
        public int ClientHandle { get; set; }

        /// <summary>
        /// 读取或待写入的值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 值的系数（默认为0，此时不起作用）
        /// </summary>
        public double Coeff { get; set; }

        /// <summary>
        /// 值的偏移量（默认为0，系数为0时不起作用）
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// 数据源字段名称
        /// </summary>
        public string FieldName { get; set; }

        ///// <summary>
        ///// 对应属性在枚举数中的索引
        ///// </summary>
        //public List<int> Indexes { get; private set; }

        private PropertyInfo _prop = null;
        /// <summary>
        /// 根据数据源字段名称获取的属性，假如OpcGroupInfo.DataSource属性为空，或找到该字段，则属性为空
        /// </summary>
        internal PropertyInfo Property
        {
            get { return _prop; }
            set
            {
                _prop = value;
                ConvertTypeMethod = _prop == null ? null : Converter.ConvertTypeMethod.MakeGenericMethod(_prop.PropertyType);
            }
        }

        /// <summary>
        /// 转换类型的静态泛型方法（类型参数为字段类型），假如属性为空则为空
        /// </summary>
        internal MethodInfo ConvertTypeMethod { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="itemId">OPC项ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        public OpcItemInfo(string itemId, int clientHandle) : this(itemId, clientHandle, null, 0) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="itemId">OPC项ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        /// <param name="fieldName">数据源中字段名称</param>
        public OpcItemInfo(string itemId, int clientHandle, string fieldName) : this(itemId, clientHandle, fieldName, 0) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="itemId">OPC项ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        /// <param name="fieldName">数据源中字段名称</param>
        /// <param name="coeff">值的系数，默认为0，此时不起作用</param>
        public OpcItemInfo(string itemId, int clientHandle, string fieldName, double coeff) : this(itemId, clientHandle, fieldName, coeff, 0) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="itemId">OPC项ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        /// <param name="fieldName">数据源中字段名称</param>
        /// <param name="coeff">值的系数，默认为0，此时不起作用</param>
        /// <param name="offset">值的偏移量，默认为0，系数为0时不起作用</param>
        public OpcItemInfo(string itemId, int clientHandle, string fieldName, double coeff, double offset)
        {
            ItemId = itemId;
            ClientHandle = clientHandle;
            Value = string.Empty;
            FieldName = fieldName;
            Coeff = coeff;
            Offset = offset;
        }

        /// <summary>
        /// 在给定的数据源中根据现有的参数获取数据源的目标属性，同时得出该属性的所属实体
        /// </summary>
        /// <param name="dataSource">数据源实体</param>
        public void InitTargetProperty(object dataSource)
        {
            if (dataSource == null)
                return;

            //Property = dataSource.GetEntityProperty_InConstruction(FieldName, true, out _upperLevelEntity, out _lowerLevelEntity, out _indexes);
            Property = dataSource.GetEntityProperty_InConstruction(FieldName, true, out _upperLevelEntity, out _, out _indices);
        }

        /// <summary>
        /// 根据给定索引更新当前实体的值以及类型转换泛型方法的对象
        /// </summary>
        private void UpdateCurrentEntityByIndexes()
        {
            UpdateCurrentEntityByIndexes(out _);
        }

        /// <summary>
        /// 根据给定索引更新当前实体的值以及类型转换泛型方法的对象
        /// </summary>
        /// <param name="type">输出该索引处元素的类型对象</param>
        private void UpdateCurrentEntityByIndexes(out Type type)
        {
            type = null;
            if (Property == null || _indices == null || _indices.Length == 0)
                return;

            //_currentEntity = Property.GetValue(_upperLevelEntity);
            _currentEntity = Property.CanRead ? Property.GetValue(_upperLevelEntity) : null;
            //Type type = Property.PropertyType;
            type = Property.PropertyType;
            //PropertyMapperExtension.GetEntityByBracketIndexes(ref _currentEntity, _indexes, ref type);
            PropertyMapperExtension.GetEntityByBracketIndexes(ref _currentEntity, out _midLevelEntity, _indices, ref type);
            ConvertTypeMethod = _currentEntity == null ? null : Converter.ConvertTypeMethod.MakeGenericMethod(type);
        }

        /// <summary>
        /// 获取经过转换方法转换后的数据源字段值，假如转换方法为空则返回空
        /// </summary>
        /// <returns></returns>
        public object GetPropertyValue()
        {
            //假如值的系数不为0，则试图乘以这个系数并加上偏移量
            object[] values = Coeff == 0 ? new object[] { Value } : new object[] { double.Parse(Value) * Coeff + Offset };
            return ConvertTypeMethod?.Invoke(null, values);
        }

        /// <summary>
        /// 假如属性或给定的数据源不为空，则为数据源设置经过转换方法转换后的数据源字段值
        /// </summary>
        /// <param name="dataSource"></param>
        public void SetPropertyValue(object dataSource)
        {
            InitTargetProperty(dataSource);
            //假如属性为空或属性所属实体为空，退出方法
            if (Property == null || _upperLevelEntity == null)
                return;
            ////假如没有提供任何方括号索引
            //if (_indexes == null || _indexes.Length == 0)
            //    Property.SetValue(_upperLevelEntity, GetPropertyValue());
            //是否没有提供任何方括号索引
            bool noIndices = _indices == null || _indices.Length == 0;
            //获取属性要赋的值，赋值之前先检查是否为方括号索引所指向的实体（没有索引将不进行特殊操作）
            if (!noIndices)
                UpdateCurrentEntityByIndexes();
            object value = GetPropertyValue();
            //假如没有提供任何方括号索引
            if (_indices == null || _indices.Length == 0)
            {
                if (Property.CanWrite)
                    Property.SetValue(_upperLevelEntity, value);
            }
            //假如提供了方括号索引，则去寻找对应位置的元素
            //TODO 在泛型List、数组或集合之中，SetPropertyValue方法目前仅支持向数组中的元素赋值
            else
                ReflectionUtil.SetValueMethod.Invoke(_midLevelEntity, new object[] { value, _indices[0] });
        }

        /// <summary>
        /// 假如属性或给定的数据源不为空，则从数据源向Item赋值
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="nullValueHandling">当获取到的标签值为空时的处理方法</param>
        public void SetItemValue(object dataSource, NullValueHandling nullValueHandling = NullValueHandling.Skip)
        {
            //假如数据源对象为空，则跳过赋值操作
            if (dataSource == null)
                return;
            InitTargetProperty(dataSource);
            //if (Property != null && _upperLevelEntity != null)
            //{
            //    object value = Property.GetValue(_upperLevelEntity);
            //    //假如值的系数不为0，则试图乘以这个系数
            //    if (Coeff != 0)
            //        value = double.Parse(value.ToString()) * Coeff;
            //    Value = value.ToString();
            //    //Value = Property.GetValue(_upperLevelEntity).ToString();
            //}

            ////假如属性为空或属性所属实体为空，退出方法
            //if (Property == null || _upperLevelEntity == null)
            //    return;
            //object value;
            //默认值为空，假如属性为空或属性所属实体为空，跳到最后赋值部分
            object value = null;
            //目标属性类型
            Type type = null;
            if (Property == null || !Property.CanRead || _upperLevelEntity == null)
                //goto SET_VALUE;
                goto NULL_HANDLING;
            ////假如没有提供方括号索引，则直接获取值，否则根据索引获取元素值
            //if (_indices == null || _indices.Length == 0)
            //{
            //    value = Property.GetValue(_upperLevelEntity);
            //    type = Property.PropertyType;
            //}
            //else
            //{
            //    UpdateCurrentEntityByIndexes(out type);
            //    value = _currentEntity;
            //}
            //假如没有提供方括号索引，则直接获取值，否则根据索引获取元素值
            if (_indices != null && _indices.Length > 0)
            {
                UpdateCurrentEntityByIndexes(out type);
                value = _currentEntity;
            }
            else
            {
                value = Property.GetValue(_upperLevelEntity);
                type = Property.PropertyType;
            }
            NULL_HANDLING:
            //对应属性值为null时的操作（值为null或者未找到属性）
            if (value == null)
            {
                //直接跳到最后赋值部分，值为null将不会写入
                if (nullValueHandling == NullValueHandling.Skip)
                    goto SET_VALUE;
                //无视空值，假如为值类型将初始化为新实例（引用类型依然为null）
                else if (nullValueHandling == NullValueHandling.Ignore)
                    value = type.CreateDefValue();
            }

            //假如值的系数不为0，则试图乘以这个系数
            if (Coeff != 0)
            {
                //舍入的小数点位数：将值与系数的小数点位数相加，相加的和与偏移量的小数点位数取较大值
                int places = Math.Max(value.GetDecimalPlaces() + Coeff.GetDecimalPlaces(), Offset.GetDecimalPlaces());
                value = Math.Round(double.Parse(value.ToString()) * Coeff + Offset, places);
            }
            SET_VALUE:
            Value = value?.ToString();
        }
    }
}
