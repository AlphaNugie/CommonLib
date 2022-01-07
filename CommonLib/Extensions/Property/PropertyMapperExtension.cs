using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonLib.Extensions.Property
{
    /// <summary>
    /// PropertyMapper扩展类
    /// </summary>
    public static class PropertyMapperExtension
    {
        #region 私有成员
        /// <summary>
        /// 匹配包含任意内容的一对方括号的正则表达式
        /// </summary>
        private static readonly Regex _regexBrackets = new Regex(RegexMatcher.RegexPattern_Brackets, RegexOptions.Compiled);

        /// <summary>
        /// 匹配枚举数方括号索引的正则表达式
        /// </summary>
        private static readonly Regex _regexIndexes = new Regex(RegexMatcher.RegexPattern_EnumIndexes, RegexOptions.Compiled);

        ///// <summary>
        ///// System.Linq.Enumerable的类型对象
        ///// </summary>
        //private static readonly Type _typeOfEnumerable = typeof(Enumerable); //从System.Core程序集中的System.Linq.Lookup类获取程序集
        #endregion

        /// <summary>
        /// 根据给定的属性名称在实体中查找指定属性的值，可查找子属性
        /// </summary>
        /// <param name="entity">待查询的实体</param>
        /// <param name="propMapper">指定属性的名称，形如“Class.Student.Name”</param>
        /// <param name="initProp">假如查找指定属性的过程中有任意一层属性为空，决定是否初始化</param>
        /// <param name="upperLevelEntity">指定属性所属的实体：假如不查找子属性，则该实体就是当前待查询的实体；假如属性名形如“Class.Student.Name”，则所属实体就是Student</param>
        /// <param name="indexes">假如最底层属性名称中带有枚举数的索引，则将这些索引以uint数组的形式输出出来，否则输出null</param>
        /// <returns></returns>
        public static PropertyInfo GetEntityProperty_InConstruction(this object entity, string propMapper, bool initProp, out object upperLevelEntity, out int[] indexes)
        {
            PropertyInfo targetProperty = null;
            //upperLevelEntity = null;
            upperLevelEntity = entity;
            List<int> listIndexes = new List<int>();
            if (entity == null || string.IsNullOrWhiteSpace(propMapper))
                goto ENDING;

            string[] parts = propMapper.Split('.'); //根据'.'拆分，以寻找子属性
            Type targetPropertyType = entity.GetType(); //目标属性的类型
            //上层目标实体与下层目标实体：当只有一层时前者为参数entity，后者为第一层属性值；当有多层时则依次递进
            //upperLevelEntity = entity;
            object lowerLevelTarget = entity;
            //遍历PropertyMapper中指定的每一层属性
            foreach (var fullPart in parts)
            {
                listIndexes.Clear();
                //假如不初始化空目标实体，且上层实体值为空，则当前层属性必然找不到，跳出循环
                if (!initProp && lowerLevelTarget == null)
                {
                    targetProperty = null;
                    break;
                }
                upperLevelEntity = lowerLevelTarget;
                //把当前层属性名称中的方括号内容提取出来，并从当前层属性名称中剔除
                //string brackets = _regexBrackets.Match(fullPart).Value;
                string brackets, part = fullPart.Replace(brackets = _regexBrackets.Match(fullPart).Value, string.Empty);
                //根据上层实体类型以及当前层属性名称获取目标属性，假如目标属性为空，则跳出循环
                targetProperty = targetPropertyType.GetProperty(part);
                if (targetProperty == null)
                    break;
                targetPropertyType = targetProperty.PropertyType; //获取当前层属性类型
                try
                {
                    //假如初始化空目标实体，且目标属性值为空，则初始化并为当前层实体赋值（假如有下个循环，则当前层实体将成为下个循环的上层实体）
                    if (initProp && targetProperty.GetValue(upperLevelEntity) == null)
                        targetProperty.SetValue(upperLevelEntity, Activator.CreateInstance(targetPropertyType));
                }
                catch (Exception) { break; }
                lowerLevelTarget = targetProperty.GetValue(upperLevelEntity); //假如有下个循环，则当前层实体将成为下个循环的上层实体
                #region 根据方括号内的索引逐层获取列表内或数组内的元素
                //foreach (Match match in _regexIndexes.Matches(brackets))
                //{
                //    int index = int.Parse(match.Value.Trim('[', ']')); //获取索引
                //    targetPropertyType = lowerLevelTarget.GetType(); //刷新当前索引所在对象的类型
                //    //获取泛型的类型参数，假如没有或有但不止一个则检查是否为数组，否则终止循环（无法继续处理）
                //    Type genericType;
                //    Type[] genericTypes = targetPropertyType.GenericTypeArguments;
                //    //假如是具有泛型参数的泛型类
                //    if (genericTypes != null || genericTypes.Length == 1)
                //        genericType = genericTypes[0];
                //    //假如是数组
                //    else if (targetPropertyType.FullName.EndsWith("[]"))
                //    //else if (targetPropertyType.IsArray)
                //        genericType = Type.GetType(targetPropertyType.FullName.TrimEnd('[', ']'));
                //    else
                //        break;
                //    //将Emumerable.Element<T>方法通过获取到的类型参数转化为泛型方法
                //    MethodInfo genericMethod = ReflectionUtil.ElementAtMethod.MakeGenericMethod(genericType);
                //    //执行静态的Element<T>方法，并迭代当前对象值
                //    lowerLevelTarget = genericMethod.Invoke(null, new object[] { lowerLevelTarget, index });
                //    listIndexes.Add(index);
                //}
                //假如没有方括号索引，直接进入下一次循环
                MatchCollection coll = _regexIndexes.Matches(brackets);
                if (coll == null || coll.Count == 0)
                    continue;
                listIndexes = coll.Cast<Match>().Select(match => int.Parse(match.Value.Trim('[', ']'))).ToList();
                GetEntityByBracketIndexes(ref lowerLevelTarget, listIndexes, ref targetPropertyType);
                #endregion
            }
        ENDING:
            indexes = listIndexes == null || listIndexes.Count == 0 ? null : listIndexes.ToArray();
            return targetProperty;
        }

        /// <summary>
        /// 从指定的实体中根据给定的方括号索引来获取实体对应索引位置的元素值，同时返回最终的元素类型
        /// </summary>
        /// <param name="currentEntity">从中获取索引元素的指定实体</param>
        /// <param name="indexes">所有方括号索引</param>
        /// <param name="entityType">最终的元素类型</param>
        public static void GetEntityByBracketIndexes(ref object currentEntity, IEnumerable<int> indexes, ref Type entityType)
        {
            if (indexes == null || indexes.Count() == 0)
                return;

            foreach (int index in indexes)
            {
                //int index = int.Parse(match.Value.Trim('[', ']')); //获取索引
                entityType = currentEntity.GetType(); //刷新当前索引所在对象的类型
                //获取泛型的类型参数，假如没有或有但不止一个则检查是否为数组，否则终止循环（无法继续处理）
                Type genericType;
                Type[] genericTypes = entityType.GenericTypeArguments;
                //假如是具有泛型参数的泛型类
                if (genericTypes != null || genericTypes.Length == 1)
                    genericType = genericTypes[0];
                //假如是数组
                else if (entityType.FullName.EndsWith("[]"))
                    //else if (targetPropertyType.IsArray)
                    genericType = Type.GetType(entityType.FullName.TrimEnd('[', ']'));
                else
                    break;
                //将Emumerable.Element<T>方法通过获取到的类型参数转化为泛型方法
                MethodInfo genericMethod = ReflectionUtil.ElementAtMethod.MakeGenericMethod(genericType);
                //执行静态的Element<T>方法，并迭代当前对象值
                currentEntity = genericMethod.Invoke(null, new object[] { currentEntity, index });
                //listIndexes.Add(index);
            }
        }

        /// <summary>
        /// 根据给定的属性名称在实体中查找指定属性的值，可查找子属性
        /// </summary>
        /// <param name="entity">待查询的实体</param>
        /// <param name="propMapper">指定属性的名称，形如“Class.Student.Name”</param>
        /// <param name="initProp">假如查找指定属性的过程中有任意一层属性为空，决定是否初始化</param>
        /// <param name="upperLevelEntity">指定属性所属的实体：假如不查找子属性，则该实体就是当前待查询的实体；假如属性名形如“Class.Student.Name”，则所属实体就是Student</param>
        /// <returns></returns>
        public static PropertyInfo GetEntityProperty(this object entity, string propMapper, bool initProp, out object upperLevelEntity)
        {
            PropertyInfo targetProperty = null;
            //upperLevelEntity = null;
            upperLevelEntity = entity;
            if (entity == null || string.IsNullOrWhiteSpace(propMapper))
                goto ENDING;

            string[] parts = propMapper.Split('.'); //根据'.'拆分，以寻找子属性
            Type targetPropertyType = entity.GetType(); //目标属性的类型
            //上层目标实体与下层目标实体：当只有一层时前者为参数entity，后者为第一层属性值；当有多层时则依次递进
            //upperLevelEntity = entity;
            object lowerLevelTarget = entity;
            //遍历PropertyMapper中指定的每一层属性
            foreach (var part in parts)
            {
                //假如不初始化空目标实体，且上层实体值为空，则当前层属性必然找不到，跳出循环
                if (!initProp && lowerLevelTarget == null)
                {
                    targetProperty = null;
                    break;
                }
                upperLevelEntity = lowerLevelTarget;
                //根据上层实体类型以及当前层属性名称获取目标属性，假如目标属性为空，则跳出循环
                targetProperty = targetPropertyType.GetProperty(part);
                if (targetProperty == null)
                    break;
                targetPropertyType = targetProperty.PropertyType; //获取当前层属性类型
                try
                {
                    //假如初始化空目标实体，且目标属性值为空，则初始化并为当前层实体赋值（假如有下个循环，则当前层实体将成为下个循环的上层实体）
                    if (initProp && targetProperty.GetValue(upperLevelEntity) == null)
                        targetProperty.SetValue(upperLevelEntity, Activator.CreateInstance(targetPropertyType));
                }
                catch (Exception) { break; }
                lowerLevelTarget = targetProperty.GetValue(upperLevelEntity); //假如有下个循环，则当前层实体将成为下个循环的上层实体
            }

            ENDING:
            return targetProperty;
        }

        /// <summary>
        /// 获取源实体中每个属性的PropertyMapper特性，根据特性值在目标实体中寻找对应属性并为该属性赋值（可由'.'符号指定子属性）
        /// </summary>
        /// <typeparam name="Target">目标实体类型参数</typeparam>
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
            //假如目标实体为空，则初始化
            if (target == null)
                target = (Target)Activator.CreateInstance(targetType);
            //遍历每个属性，找到PropertyMapper特性并根据特性值为目标实体属性赋值
            foreach (var sourceProperty in sourceProperties)
            {
                //假如有PropertyMapper特性且属性有get访问器
                IEnumerable<PropertyMapperToAttribute> attrs = sourceProperty.GetCustomAttributes<PropertyMapperToAttribute>(false);
                if (attrs == null || attrs.Count() == 0 || sourceProperty.GetGetMethod() == null)
                    continue;
                object sourceValue = sourceProperty.GetValue(source);
                foreach (var attr in attrs)
                {
                    if (string.IsNullOrWhiteSpace(attr.PropertyMapper))
                        continue;

                    #region 旧判断方式
                    //string[] parts = attr.PropertyMapper.Split('.'); //根据'.'拆分，以寻找子属性
                    //PropertyInfo targetProperty = null;
                    //Type targetPropertyType = targetType; //目标属性的类型
                    //object upperLevelTarget = target, lowerLevelTarget = target; //上层目标实体与下层目标实体：当只有一层时前者为参数target，后者为第一层属性值；当有多层时则依次递进
                    ////遍历PropertyMapper中指定的每一层属性
                    //foreach (var part in parts)
                    //{
                    //    upperLevelTarget = lowerLevelTarget;
                    //    //根据上层实体类型以及当前层属性名称获取目标属性，假如目标属性为空，则跳出循环
                    //    targetProperty = targetPropertyType.GetProperty(part);
                    //    if (targetProperty == null)
                    //        break;
                    //    targetPropertyType = targetProperty.PropertyType; //获取当前层属性类型
                    //    //假如目标属性值为空，则初始化并为当前层实体赋值（假如有下个循环，则当前层实体将成为下个循环的上层实体）
                    //    try
                    //    {
                    //        if (targetProperty.GetValue(upperLevelTarget) == null)
                    //            targetProperty.SetValue(upperLevelTarget, Activator.CreateInstance(targetPropertyType));
                    //        lowerLevelTarget = targetProperty.GetValue(upperLevelTarget); //假如有下个循环，则当前层实体将成为下个循环的上层实体
                    //    }
                    //    catch (Exception) { break; }
                    //}
                    #endregion

                    #region 新判断方式
                    PropertyInfo targetProperty = target.GetEntityProperty(attr.PropertyMapper, true, out object upperLevelTarget);
                    //假如未找到该属性，进入下一次循环
                    if (targetProperty == null)
                        continue;
                    Type targetPropertyType = targetProperty.PropertyType;
                    #endregion

                    //假如目标属性具有set访问器
                    //if (targetProperty != null)
                        targetProperty.SetValue(upperLevelTarget, sourceProperty.PropertyType == targetPropertyType ? sourceValue : Converter.Convert(targetPropertyType, sourceValue));
                }
            }
        }

        /// <summary>
        /// 获取源实体中每个属性的PropertyMapper特性，根据特性值在目标实体中获取对应属性的值并赋给源实体中的属性（可由'.'符号指定子属性）
        /// </summary>
        /// <typeparam name="Target">目标实体类型参数</typeparam>
        /// <param name="source">源实体</param>
        /// <param name="target">目标实体</param>
        public static void CopyPropertyValueFrom<Target>(this object source, Target target)
        {
            Type sourceType = source.GetType()/*, targetType = typeof(Target)*/; //获取源类型与目标类型
            //获取所有属性，假如目标实体为空或类型不包含任何属性则退出
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            if (target == null || sourceProperties == null || sourceProperties.Length == 0)
                return;
            //假如源实体为空，则初始化
            if (source == null)
                source = Activator.CreateInstance(sourceType);
            //遍历每个属性，找到PropertyMapper特性并根据特性值从目标实体属性获取值并赋给当前属性
            foreach (var sourceProperty in sourceProperties)
            {
                //假如有PropertyMapper特性且属性有set访问器
                IEnumerable<PropertyMapperFromAttribute> attrs = sourceProperty.GetCustomAttributes<PropertyMapperFromAttribute>(false);
                if (attrs == null || attrs.Count() == 0 || sourceProperty.GetSetMethod() == null)
                    continue;
                Type sourcePropertyType = sourceProperty.PropertyType;
                object targetValue = default;
                foreach (var attr in attrs)
                {
                    if (string.IsNullOrWhiteSpace(attr.PropertyMapper))
                        continue;

                    #region 旧判断方法
                    //string[] parts = attr.PropertyMapper.Split('.'); //根据'.'拆分，以寻找子属性
                    //PropertyInfo targetProperty = null;
                    //Type targetPropertyType = targetType; //目标属性的类型
                    //object upperLevelTarget = target, lowerLevelTarget = target; //上层目标实体与下层目标实体：当只有一层时前者为参数target，后者为第一层属性值；当有多层时则依次递进
                    ////遍历PropertyMapper中指定的每一层属性
                    //foreach (var part in parts)
                    //{
                    //    //假如上层实体值为空，则当前层属性必然找不到，跳出循环
                    //    if (lowerLevelTarget == null)
                    //    {
                    //        targetProperty = null;
                    //        break;
                    //    }
                    //    upperLevelTarget = lowerLevelTarget;
                    //    //根据上层实体类型以及当前层属性名称获取目标属性，假如目标属性为空，则跳出循环
                    //    targetProperty = targetPropertyType.GetProperty(part);
                    //    if (targetProperty == null)
                    //        break;
                    //    targetPropertyType = targetProperty.PropertyType; //获取当前层属性类型
                    //    lowerLevelTarget = targetProperty.GetValue(upperLevelTarget); //假如有下个循环，则当前层实体将成为下个循环的上层实体
                    //}
                    #endregion

                    #region 新判断方法
                    PropertyInfo targetProperty = target.GetEntityProperty(attr.PropertyMapper, false, out object upperLevelTarget);
                    Type targetPropertyType = targetProperty.PropertyType;
                    #endregion

                    //假如获取到了目标属性且目标属性具有set访问器
                    if (targetProperty != null)
                        targetValue = targetProperty.GetValue(upperLevelTarget);
                    sourceProperty.SetValue(source, sourcePropertyType == targetPropertyType ? targetValue : Converter.Convert(sourcePropertyType, targetValue));
                }
            }
        }
    }

    ///// <summary>
    ///// 为添加此特性的属性在其它实体中寻找符合指定名称的属性，以向其进行赋值
    ///// </summary>
    //public sealed class PropertyMapperAttribute : Attribute
    //{
    //    /// <summary>
    //    /// 以指定Mapper名称初始化特性
    //    /// </summary>
    //    /// <param name="mapper">目标属性的名称，可由'.'符号指定子属性</param>
    //    public PropertyMapperAttribute(string mapper)
    //    {
    //        PropertyMapper = mapper;
    //    }

    //    /// <summary>
    //    /// 特性的目标属性名称，可由'.'符号指定子属性
    //    /// </summary>
    //    public string PropertyMapper { get; }
    //}

    /// <summary>
    /// 为添加此特性的属性在其它实体中寻找符合指定名称的属性，以向其进行赋值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyMapperToAttribute : Attribute
    {
        /// <summary>
        /// 以指定Mapper名称初始化特性
        /// </summary>
        /// <param name="mapper">目标属性的名称，可由'.'符号指定子属性</param>
        public PropertyMapperToAttribute(string mapper)
        {
            PropertyMapper = mapper;
        }

        /// <summary>
        /// 特性的目标属性名称，可由'.'符号指定子属性
        /// </summary>
        public string PropertyMapper { get; }
    }

    /// <summary>
    /// 为添加此特性的属性在其它实体中寻找符合指定名称的属性，以从该属性获取值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyMapperFromAttribute : Attribute
    {
        /// <summary>
        /// 以指定Mapper名称初始化特性
        /// </summary>
        /// <param name="mapper">目标属性的名称，可由'.'符号指定子属性</param>
        public PropertyMapperFromAttribute(string mapper)
        {
            PropertyMapper = mapper;
        }

        /// <summary>
        /// 特性的目标属性名称，可由'.'符号指定子属性
        /// </summary>
        public string PropertyMapper { get; }
    }
}
