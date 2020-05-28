using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Extensions.Property
{
    /// <summary>
    /// PropertyMapper扩展类
    /// </summary>
    public static class PropertyMapperExtension
    {
        /// <summary>
        /// 获取源实体中每个属性的PropertyMapper特性，根据特性值在目标实体中寻找对应属性并为该属性赋值（可由'.'符号指定子属性）
        /// </summary>
        /// <param name="source">源实体</param>
        /// <param name="target">目标实体</param>
        public static void CopyPropertyValueTo<Target>(this object source, ref Target target)
        {
            //Type sourceType = source.GetType(), targetType = target.GetType(); //获取源类型与目标类型
            Type sourceType = source.GetType(), targetType = typeof(Target); //获取源类型与目标类型
            //获取所有属性，假如源实体为空或类型不包含任何属性则退出
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            if (source == null || sourceProperties == null || sourceProperties.Length == 0)
                return;
            ////假如目标实体为空，则初始化
            if (target == null)
                target = (Target)Activator.CreateInstance(targetType);
            //遍历每个属性，找到PropertyMapper特性并根据特性值为目标实体属性赋值
            foreach (var sourceProperty in sourceProperties)
            {
                //假如有PropertyMapper特性且属性有get访问器
                object[] attrs = sourceProperty.GetCustomAttributes(typeof(PropertyMapperAttribute), false);
                if (attrs == null || attrs.Length == 0 || sourceProperty.GetGetMethod() == null)
                    continue;
                object sourceValue = sourceProperty.GetValue(source);
                foreach (PropertyMapperAttribute attr in attrs)
                {
                    if (string.IsNullOrWhiteSpace(attr.PropertyMapper))
                        continue;
                    string[] parts = attr.PropertyMapper.Split('.'); //根据'.'拆分，以寻找子属性
                    PropertyInfo targetProperty = null;
                    Type targetPropertyType = targetType; //目标属性的类型
                    object upperLevelTarget = target, lowerLevelTarget = target; //上层目标实体与下层目标实体：当只有一层时前者为参数target，后者为第一层属性值；当有多层时则依次递进
                    //遍历PropertyMapper中指定的每一层属性
                    foreach (var part in parts)
                    {
                        upperLevelTarget = lowerLevelTarget;
                        //根据上层实体类型以及当前层属性名称获取目标属性，假如目标属性为空，则跳出循环
                        targetProperty = targetPropertyType.GetProperty(part);
                        if (targetProperty == null)
                            break;
                        targetPropertyType = targetProperty.PropertyType; //获取当前层属性类型
                        //假如目标属性值为空，则初始化并为当前层实体赋值（假如有下个循环，则当前层实体将成为下个循环的上层实体）
                        try
                        {
                            if (targetProperty.GetValue(upperLevelTarget) == null)
                                targetProperty.SetValue(upperLevelTarget, Activator.CreateInstance(targetPropertyType));
                            lowerLevelTarget = targetProperty.GetValue(upperLevelTarget);
                        }
                        catch (Exception) { break; }
                    }
                    //假如获取到了目标属性且目标属性具有set访问器
                    if (targetProperty != null)
                        targetProperty.SetValue(upperLevelTarget, sourceProperty.PropertyType == targetPropertyType ? sourceValue : Converter.Convert(targetPropertyType, sourceValue));
                }
            }
        }
        ///// <summary>
        ///// 获取源实体中每个属性的PropertyMapper特性，根据特性值在目标实体中寻找对应属性并为该属性赋值（可由'.'符号指定子属性）
        ///// </summary>
        ///// <typeparam name="Source">源实体类型</typeparam>
        ///// <typeparam name="Target">目标实体类型</typeparam>
        ///// <param name="source">源实体</param>
        ///// <param name="target">目标实体</param>
        //public static void CopyPropertyValueTo<Source, Target>(this Source source, ref Target target) where Target : new()
        //{
        //    Type sourceType = typeof(Source), targetType = typeof(Target); //获取源类型与目标类型
        //    //获取所有属性，假如源实体为空或类型不包含任何属性则退出
        //    PropertyInfo[] sourceProperties = sourceType.GetProperties();
        //    if (source == null || sourceProperties == null || sourceProperties.Length == 0)
        //        return;
        //    //假如目标实体为空，则初始化
        //    if (target == null)
        //        target = new Target();
        //    //遍历每个属性，找到PropertyMapper特性并根据特性值为目标实体属性赋值
        //    foreach (var sourceProperty in sourceProperties)
        //    {
        //        //假如有PropertyMapper特性且属性有get访问器
        //        object[] attrs = sourceProperty.GetCustomAttributes(typeof(PropertyMapperAttribute), false);
        //        if (attrs == null || attrs.Length == 0 || sourceProperty.GetGetMethod() == null)
        //            continue;
        //        object sourceValue = sourceProperty.GetValue(source);
        //        foreach (PropertyMapperAttribute attr in attrs)
        //        {
        //            if (string.IsNullOrWhiteSpace(attr.PropertyMapper))
        //                continue;
        //            string[] parts = attr.PropertyMapper.Split('.'); //根据'.'拆分，以寻找子属性
        //            PropertyInfo targetProperty = null;
        //            Type targetPropertyType = targetType; //目标属性的类型
        //            //遍历每一层属性名称
        //            foreach (var part in parts)
        //            {
        //                targetProperty = targetPropertyType.GetProperty(part); //根据名称获取目标属性
        //                if (targetProperty == null)
        //                    break;
        //                targetPropertyType = targetProperty.PropertyType; //获取当前属性类型
        //            }
        //            //假如获取到了目标属性且目标属性具有set访问器
        //            if (targetProperty != null && targetProperty.GetSetMethod() != null)
        //                targetProperty.SetValue(target, sourceProperty.PropertyType == targetPropertyType ? sourceValue : Converter.Convert(targetPropertyType, sourceValue));
        //            //targetProperty.SetValue(target, sourceValue);
        //            //targetField.SetValue(target, Converter.Convert(fieldType, sourceValue));
        //        }
        //    }
        //}
    }

    /// <summary>
    /// 为添加此特性的属性在其它实体中寻找符合指定名称的属性
    /// </summary>
    public sealed class PropertyMapperAttribute : Attribute
    {
        /// <summary>
        /// 以指定Mapper名称初始化特性
        /// </summary>
        /// <param name="mapper">目标属性的名称，可由'.'符号指定子属性</param>
        public PropertyMapperAttribute(string mapper)
        {
            this.PropertyMapper = mapper;
        }

        /// <summary>
        /// 特性的目标属性名称，可由'.'符号指定子属性
        /// </summary>
        public string PropertyMapper { get; }
    }
}
