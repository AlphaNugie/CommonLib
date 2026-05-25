using CommonLib.Extensions.Reflection;
using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace CommonLib.Extensions.Property
{
    /// <summary>
    /// PropertyMapper扩展类
    /// </summary>
#if NET45_OR_GREATER
    public static class PropertyMapperExtension
#elif NET9_0_OR_GREATER
    //.net 9 框架下将此类声明为partial，以兼容正则表达式
    public static partial class PropertyMapperExtension
#endif
    {
        #region 私有成员
#if NET45_OR_GREATER
        /// <summary>
        /// 匹配包含任意内容的一对方括号的正则表达式
        /// </summary>
        private static readonly Regex _regexBrackets = 
            new Regex(RegexMatcher.RegexPattern_Brackets, RegexOptions.Compiled);

        /// <summary>
        /// 匹配枚举数方括号索引的正则表达式
        /// </summary>
        private static readonly Regex _regexIndexes = new Regex(RegexMatcher.RegexPattern_EnumIndexes, RegexOptions.Compiled);
#elif NET9_0_OR_GREATER
        /// <summary>
        /// 匹配包含任意内容的一对方括号的正则表达式
        /// </summary>
        [GeneratedRegex(RegexMatcher.RegexPattern_Brackets, RegexOptions.Compiled)]
        private static partial Regex GeneratedRegex_Brackets();

        /// <summary>
        /// 匹配枚举数方括号索引的正则表达式
        /// </summary>
        [GeneratedRegex(RegexMatcher.RegexPattern_EnumIndexes, RegexOptions.Compiled)]
        private static partial Regex GeneratedRegex_EnumIndexes();

        private static readonly Regex _regexBrackets = GeneratedRegex_Brackets();
        private static readonly Regex _regexIndexes = GeneratedRegex_EnumIndexes();
#endif


        ///// <summary>
        ///// System.Linq.Enumerable的类型对象
        ///// </summary>
        //private static readonly Type _typeOfEnumerable = typeof(Enumerable); //从System.Core程序集中的System.Linq.Lookup类获取程序集
        #endregion

        #region GetEntityProperty
        /// <summary>
        /// 根据给定的属性名称在实体中查找指定属性的值，可查找子属性
        /// </summary>
        /// <param name="entity">待查询的实体</param>
        /// <param name="propMapper">指定属性的名称，形如“Class.Student.Name”</param>
        /// <param name="initProp">假如查找指定属性的过程中有任意一层属性为空，决定是否初始化</param>
        /// <param name="upperLevelEntity">指定属性所属的实体：假如不查找子属性，则该实体就是当前待查询的实体；假如属性名形如“Class.Student.Name”，则所属实体就是Student</param>
        /// <param name="lowerLevelEntity">指定属性所对应的实体，假如属性名称内带有索引，则为索引处元素的实体；假如属性名形如“Class.Student”，则对应实体就是Student，假如属性名形如“Class.Desk[0]”，则对应实体就是Desk列表的第一个元素</param>
        /// <param name="indices">假如最底层属性名称中带有枚举数的索引，则将这些索引以uint数组的形式输出出来，否则输出null</param>
        /// <returns></returns>
#if NET45_OR_GREATER
        public static PropertyInfo GetEntityProperty_InConstruction(this object entity, string propMapper, bool initProp, out object upperLevelEntity, out object lowerLevelEntity, out int[] indices)
#elif NET9_0_OR_GREATER
        public static PropertyInfo? GetEntityProperty_InConstruction(this object? entity, string propMapper, bool initProp, out object? upperLevelEntity, out object? lowerLevelEntity, out int[]? indices)
#endif
        {
            return GetEntityProperty_InConstruction(entity, propMapper, initProp, out upperLevelEntity, out _, out lowerLevelEntity, out indices);
        }

        #region 旧方法V2：GetEntityProperty_InConstruction
        ///// <summary>
        ///// 根据给定的属性名称在实体中查找指定属性的值，可查找子属性
        ///// </summary>
        ///// <param name="entity">待查询的实体</param>
        ///// <param name="propMapper">指定属性的名称，形如“Class.Student.Name”</param>
        ///// <param name="initProp">假如查找指定属性的过程中有任意一层属性为空，决定是否初始化</param>
        ///// <param name="upperLevelEntity">指定属性所属的实体，假如不查找子属性，则该实体就是当前待查询的实体；假如属性名形如“Class.Student.Name”，则所属实体就是Student</param>
        ///// <param name="midLevelEntity">指定属性所对应或所属的中间实体，假如属性名称内带有索引，则为索引处元素所属的集合、数组或List；假如属性名形如“Class.Student”，则对应实体就是Student，假如属性名形如“Class.Desk[0]”，则对应实体就是Desk</param>
        ///// <param name="lowerLevelEntity">指定属性所对应的实体，假如属性名称内带有索引，则为索引处元素的实体；假如属性名形如“Class.Student”，则对应实体就是Student，假如属性名形如“Class.Desk[0]”，则对应实体就是Desk列表的第一个元素</param>
        ///// <param name="indices">假如最底层属性名称中带有枚举数的索引，则将这些索引以uint数组的形式输出出来，否则输出null</param>
        ///// <returns></returns>
        //public static PropertyInfo GetEntityProperty_InConstruction(this object entity, string propMapper, bool initProp, out object upperLevelEntity, out object midLevelEntity, out object lowerLevelEntity, out int[] indices)
        //{
        //    PropertyInfo targetProperty = null;
        //    //upperLevelEntity = lowerLevelEntity = entity;
        //    upperLevelEntity = midLevelEntity = lowerLevelEntity = entity;
        //    //lowerLevelEntity = entity;
        //    List<int> listIndexes = new List<int>();
        //    if (entity == null || string.IsNullOrWhiteSpace(propMapper))
        //        goto ENDING;

        //    string[] parts = propMapper.Split('.'); //根据'.'拆分，以寻找子属性
        //    Type targetPropertyType = entity.GetType(); //目标属性的类型
        //    //上层目标实体与下层目标实体：当只有一层时前者为参数entity，后者为第一层属性值；当有多层时则依次递进
        //    //upperLevelEntity = entity;
        //    //object lowerLevelTarget = entity;
        //    //遍历PropertyMapper中指定的每一层属性
        //    foreach (var fullPart in parts)
        //    {
        //        listIndexes.Clear();
        //        //假如不初始化空目标实体，且上层实体值为空，则当前层属性必然找不到，跳出循环
        //        if (!initProp && lowerLevelEntity == null)
        //        {
        //            targetProperty = null;
        //            break;
        //        }
        //        upperLevelEntity = lowerLevelEntity;
        //        //把当前层属性名称中的方括号内容提取出来，并从当前层属性名称中剔除
        //        //string brackets = _regexBrackets.Match(fullPart).Value;
        //        //string brackets, part = fullPart.Replace(brackets = _regexBrackets.Match(fullPart).Value, string.Empty); //报错
        //        string brackets = _regexBrackets.Match(fullPart).Value, part = string.IsNullOrWhiteSpace(brackets) ? fullPart : fullPart.Replace(brackets, string.Empty);
        //        //根据上层实体类型以及当前层属性名称获取目标属性，假如目标属性为空，则跳出循环
        //        targetProperty = targetPropertyType.GetProperty(part);
        //        if (targetProperty == null)
        //            break;
        //        targetPropertyType = targetProperty.PropertyType; //获取当前层属性类型
        //        try
        //        {
        //            //假如初始化空目标实体，且目标属性值为空，则初始化并为当前层实体赋值（假如有下个循环，则当前层实体将成为下个循环的上层实体）
        //            if (initProp && targetProperty.GetValue(upperLevelEntity) == null)
        //                targetProperty.SetValue(upperLevelEntity, Activator.CreateInstance(targetPropertyType));
        //            ////假如初始化空目标实体，且可读写的同时目标属性值为空，则初始化并为当前层实体赋值（假如有下个循环，则当前层实体将成为下个循环的上层实体）
        //            //if (initProp && targetProperty.CanRead && targetProperty.CanWrite && targetProperty.GetValue(upperLevelEntity) == null)
        //            //    targetProperty.SetValue(upperLevelEntity, Activator.CreateInstance(targetPropertyType));
        //        }
        //        catch (Exception) { break; }
        //        lowerLevelEntity = targetProperty.GetValue(upperLevelEntity); //假如有下个循环，则当前层实体将成为下个循环的上层实体
        //        #region 根据方括号内的索引逐层获取列表内或数组内的元素
        //        #region previous crap
        //        //foreach (Match match in _regexIndexes.Matches(brackets))
        //        //{
        //        //    int index = int.Parse(match.Value.Trim('[', ']')); //获取索引
        //        //    targetPropertyType = lowerLevelTarget.GetType(); //刷新当前索引所在对象的类型
        //        //    //获取泛型的类型参数，假如没有或有但不止一个则检查是否为数组，否则终止循环（无法继续处理）
        //        //    Type genericType;
        //        //    Type[] genericTypes = targetPropertyType.GenericTypeArguments;
        //        //    //假如是具有泛型参数的泛型类
        //        //    if (genericTypes != null || genericTypes.Length == 1)
        //        //        genericType = genericTypes[0];
        //        //    //假如是数组
        //        //    else if (targetPropertyType.FullName.EndsWith("[]"))
        //        //    //else if (targetPropertyType.IsArray)
        //        //        genericType = Type.GetType(targetPropertyType.FullName.TrimEnd('[', ']'));
        //        //    else
        //        //        break;
        //        //    //将Emumerable.Element<T>方法通过获取到的类型参数转化为泛型方法
        //        //    MethodInfo genericMethod = ReflectionUtil.ElementAtMethod.MakeGenericMethod(genericType);
        //        //    //执行静态的Element<T>方法，并迭代当前对象值
        //        //    lowerLevelTarget = genericMethod.Invoke(null, new object[] { lowerLevelTarget, index });
        //        //    listIndexes.Add(index);
        //        //}
        //        #endregion
        //        //假如没有方括号索引，直接进入下一次循环
        //        MatchCollection coll = _regexIndexes.Matches(brackets);
        //        if (coll == null || coll.Count == 0)
        //            continue;
        //        listIndexes = coll.Cast<Match>().Select(match => int.Parse(match.Value.Trim('[', ']'))).ToList();
        //        //GetEntityByBracketIndexes(ref lowerLevelEntity, listIndexes, ref targetPropertyType);
        //        GetEntityByBracketIndexes(ref lowerLevelEntity, out midLevelEntity, listIndexes, ref targetPropertyType);
        //        #endregion
        //    }
        //ENDING:
        //    indices = listIndexes == null || listIndexes.Count == 0 ? null : listIndexes.ToArray();
        //    return targetProperty;
        //}
        #endregion

        /// <summary>
        /// 根据给定的属性名称在实体中查找指定属性的值，可查找子属性
        /// </summary>
        /// <param name="entity">待查询的实体</param>
        /// <param name="propMapper">指定属性的名称，形如“Class.Student.Name”</param>
        /// <param name="initProp">假如查找指定属性的过程中有任意一层属性为空，决定是否初始化</param>
        /// <param name="upperLevelEntity">指定属性所属的实体，假如不查找子属性，则该实体就是当前待查询的实体；假如属性名形如“Class.Student.Name”，则所属实体就是Student</param>
        /// <param name="midLevelEntity">指定属性所对应或所属的中间实体，假如属性名称内带有索引，则为索引处元素所属的集合、数组或List；假如属性名形如“Class.Student”，则对应实体就是Student，假如属性名形如“Class.Desk[0]”，则对应实体就是Desk</param>
        /// <param name="lowerLevelEntity">指定属性所对应的实体，假如属性名称内带有索引，则为索引处元素的实体；假如属性名形如“Class.Student”，则对应实体就是Student，假如属性名形如“Class.Desk[0]”，则对应实体就是Desk列表的第一个元素</param>
        /// <param name="indices">假如最底层属性名称中带有枚举数的索引，则将这些索引以uint数组的形式输出出来，否则输出null</param>
        /// <returns></returns>
#if NET45_OR_GREATER
        public static PropertyInfo GetEntityProperty_InConstruction(this object entity, string propMapper, bool initProp, out object upperLevelEntity, out object midLevelEntity, out object lowerLevelEntity, out int[] indices)
#elif NET9_0_OR_GREATER
        public static PropertyInfo? GetEntityProperty_InConstruction(this object? entity, string propMapper, bool initProp, out object? upperLevelEntity, out object? midLevelEntity, out object? lowerLevelEntity, out int[]? indices)
#endif
        {
            PropertyInfo
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            targetProperty = null;
            upperLevelEntity = midLevelEntity = lowerLevelEntity = entity;
            List<int> listIndexes =
#if NET45_OR_GREATER
                new List<int>();
#elif NET9_0_OR_GREATER
                [];
#endif
            if (entity == null || string.IsNullOrWhiteSpace(propMapper))
                goto ENDING;

            string[] parts = propMapper.Split('.'); //根据'.'拆分，以寻找子属性
            Type
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            targetPropertyType = entity.GetType(); //目标属性的类型
            //遍历PropertyMapper中指定的每一层属性
            foreach (var fullPart in parts)
            {
                listIndexes.Clear();
                //假如不初始化空目标实体，且上层实体值为空，则当前层属性必然找不到，跳出循环
                if (!initProp && lowerLevelEntity == null)
                {
                    targetProperty = null;
                    break;
                }
                upperLevelEntity = lowerLevelEntity;
                //把当前层属性名称中的方括号内容提取出来，并从当前层属性名称中剔除
                string brackets = _regexBrackets.Match(fullPart).Value, part = string.IsNullOrWhiteSpace(brackets) ? fullPart : fullPart.Replace(brackets, string.Empty);
                //根据上层实体类型以及当前层属性名称获取目标属性，假如目标属性为空或目标属性不可读，则跳出循环
                //targetProperty = targetPropertyType.GetProperty(part);
                targetProperty = targetPropertyType?.GetProperty(part);
                if (targetProperty == null || !targetProperty.CanRead)
                    break;
                targetPropertyType = targetProperty.PropertyType; //获取当前层属性类型
                try
                {
                    //假如初始化空目标实体，且可写的同时目标属性值为空，则初始化并为当前层实体赋值（假如有下个循环，则当前层实体将成为下个循环的上层实体）
                    //【防御性处理】当 upperLevelEntity 为 null 时（通常由上一轮索引越界/列表为空导致），
                    //不再尝试调用 GetValue/SetValue，直接跳出循环避免抛出 TargetException
                    if (upperLevelEntity == null)
                    {
                        Debug.WriteLine($"[PropertyMapper] 属性路径解析中断：属性 '{part}' 的上层实体为 null，无法继续获取属性值。完整路径片段: '{fullPart}'");
                        targetProperty = null;
                        break;
                    }
                    if (initProp && targetProperty.CanWrite && targetProperty.GetValue(upperLevelEntity) == null)
                        targetProperty.SetValue(upperLevelEntity, Activator.CreateInstance(targetPropertyType));
                }
                catch (Exception) { break; }

                //【防御性处理】再次检查 upperLevelEntity 是否为 null，
                //防止因上述 initProp 分支中 SetValue 失败等边缘情况导致后续 GetValue 传入 null 实例
                if (upperLevelEntity == null)
                {
                    Debug.WriteLine($"[PropertyMapper] 属性路径解析中断：属性 '{part}' 的上层实体在初始化后仍为 null。完整路径片段: '{fullPart}'");
                    targetProperty = null;
                    break;
                }

                lowerLevelEntity = targetProperty.GetValue(upperLevelEntity); //假如有下个循环，则当前层实体将成为下个循环的上层实体
                #region 根据方括号内的索引逐层获取列表内或数组内的元素
                //假如没有方括号索引，直接进入下一次循环
                MatchCollection coll = _regexIndexes.Matches(brackets);
                if (coll == null || coll.Count == 0)
                    continue;
                listIndexes = coll.Cast<Match>().Select(match => int.Parse(match.Value.Trim('[', ']'))).ToList();
                GetEntityByBracketIndexes(ref lowerLevelEntity, out midLevelEntity, listIndexes, ref targetPropertyType);
                #endregion
            }
        ENDING:
            indices = listIndexes == null || listIndexes.Count == 0 ? null : listIndexes.ToArray();
            return targetProperty;
        }

        #region 旧方法：GetEntityProperty_InConstruction
        ///// <summary>
        ///// 根据给定的属性名称在实体中查找指定属性的值，可查找子属性
        ///// </summary>
        ///// <param name="entity">待查询的实体</param>
        ///// <param name="propMapper">指定属性的名称，形如“Class.Student.Name”</param>
        ///// <param name="initProp">假如查找指定属性的过程中有任意一层属性为空，决定是否初始化</param>
        ///// <param name="upperLevelEntity">指定属性所属的实体：假如不查找子属性，则该实体就是当前待查询的实体；假如属性名形如“Class.Student.Name”，则所属实体就是Student</param>
        ///// <param name="lowerLevelEntity">指定属性所对应的实体，假如属性名称内带有索引，则为索引处元素的实体；假如属性名形如“Class.Student”，则对应实体就是Student，假如属性名形如“Class.Desk[0]”，则对应实体就是Desk列表的第一个元素</param>
        ///// <param name="indexes">假如最底层属性名称中带有枚举数的索引，则将这些索引以uint数组的形式输出出来，否则输出null</param>
        ///// <returns></returns>
        //public static PropertyInfo GetEntityProperty_InConstruction(this object entity, string propMapper, bool initProp, out object upperLevelEntity, out object lowerLevelEntity, out int[] indexes)
        //{
        //    PropertyInfo targetProperty = null;
        //    //upperLevelEntity = null;
        //    upperLevelEntity = lowerLevelEntity = entity;
        //    //lowerLevelEntity = entity;
        //    List<int> listIndexes = new List<int>();
        //    if (entity == null || string.IsNullOrWhiteSpace(propMapper))
        //        goto ENDING;

        //    string[] parts = propMapper.Split('.'); //根据'.'拆分，以寻找子属性
        //    Type targetPropertyType = entity.GetType(); //目标属性的类型
        //    //上层目标实体与下层目标实体：当只有一层时前者为参数entity，后者为第一层属性值；当有多层时则依次递进
        //    //upperLevelEntity = entity;
        //    //object lowerLevelTarget = entity;
        //    //遍历PropertyMapper中指定的每一层属性
        //    foreach (var fullPart in parts)
        //    {
        //        listIndexes.Clear();
        //        //假如不初始化空目标实体，且上层实体值为空，则当前层属性必然找不到，跳出循环
        //        if (!initProp && lowerLevelEntity == null)
        //        {
        //            targetProperty = null;
        //            break;
        //        }
        //        upperLevelEntity = lowerLevelEntity;
        //        //把当前层属性名称中的方括号内容提取出来，并从当前层属性名称中剔除
        //        //string brackets = _regexBrackets.Match(fullPart).Value;
        //        //string brackets, part = fullPart.Replace(brackets = _regexBrackets.Match(fullPart).Value, string.Empty); //报错
        //        string brackets = _regexBrackets.Match(fullPart).Value, part = string.IsNullOrWhiteSpace(brackets) ? fullPart : fullPart.Replace(brackets, string.Empty);
        //        //根据上层实体类型以及当前层属性名称获取目标属性，假如目标属性为空，则跳出循环
        //        targetProperty = targetPropertyType.GetProperty(part);
        //        if (targetProperty == null)
        //            break;
        //        targetPropertyType = targetProperty.PropertyType; //获取当前层属性类型
        //        try
        //        {
        //            //假如初始化空目标实体，且目标属性值为空，则初始化并为当前层实体赋值（假如有下个循环，则当前层实体将成为下个循环的上层实体）
        //            if (initProp && targetProperty.GetValue(upperLevelEntity) == null)
        //                targetProperty.SetValue(upperLevelEntity, Activator.CreateInstance(targetPropertyType));
        //        }
        //        catch (Exception) { break; }
        //        lowerLevelEntity = targetProperty.GetValue(upperLevelEntity); //假如有下个循环，则当前层实体将成为下个循环的上层实体
        //        #region 根据方括号内的索引逐层获取列表内或数组内的元素
        //        #region previous crap
        //        //foreach (Match match in _regexIndexes.Matches(brackets))
        //        //{
        //        //    int index = int.Parse(match.Value.Trim('[', ']')); //获取索引
        //        //    targetPropertyType = lowerLevelTarget.GetType(); //刷新当前索引所在对象的类型
        //        //    //获取泛型的类型参数，假如没有或有但不止一个则检查是否为数组，否则终止循环（无法继续处理）
        //        //    Type genericType;
        //        //    Type[] genericTypes = targetPropertyType.GenericTypeArguments;
        //        //    //假如是具有泛型参数的泛型类
        //        //    if (genericTypes != null || genericTypes.Length == 1)
        //        //        genericType = genericTypes[0];
        //        //    //假如是数组
        //        //    else if (targetPropertyType.FullName.EndsWith("[]"))
        //        //    //else if (targetPropertyType.IsArray)
        //        //        genericType = Type.GetType(targetPropertyType.FullName.TrimEnd('[', ']'));
        //        //    else
        //        //        break;
        //        //    //将Emumerable.Element<T>方法通过获取到的类型参数转化为泛型方法
        //        //    MethodInfo genericMethod = ReflectionUtil.ElementAtMethod.MakeGenericMethod(genericType);
        //        //    //执行静态的Element<T>方法，并迭代当前对象值
        //        //    lowerLevelTarget = genericMethod.Invoke(null, new object[] { lowerLevelTarget, index });
        //        //    listIndexes.Add(index);
        //        //}
        //        #endregion
        //        //假如没有方括号索引，直接进入下一次循环
        //        MatchCollection coll = _regexIndexes.Matches(brackets);
        //        if (coll == null || coll.Count == 0)
        //            continue;
        //        listIndexes = coll.Cast<Match>().Select(match => int.Parse(match.Value.Trim('[', ']'))).ToList();
        //        GetEntityByBracketIndexes(ref lowerLevelEntity, listIndexes, ref targetPropertyType);
        //        #endregion
        //    }
        //ENDING:
        //    indexes = listIndexes == null || listIndexes.Count == 0 ? null : listIndexes.ToArray();
        //    return targetProperty;
        //}

        #endregion
        #endregion

        #region GetEntityByBracketIndexes
        #region 旧方法：GetEntityByBracketIndexes
        ///// <summary>
        ///// 从指定的实体中根据给定的方括号索引来获取实体对应索引位置的元素值，同时返回最终的元素类型
        ///// </summary>
        ///// <param name="currentEntity">从中获取索引元素的指定实体</param>
        ///// <param name="indexes">所有方括号索引</param>
        ///// <param name="entityType">最终的元素类型</param>
        //public static void GetEntityByBracketIndexes(ref object currentEntity, IEnumerable<int> indexes, ref Type entityType)
        //{
        //    if (indexes == null || indexes.Count() == 0)
        //        return;

        //    foreach (int index in indexes)
        //    {
        //        //int index = int.Parse(match.Value.Trim('[', ']')); //获取索引
        //        entityType = currentEntity.GetType(); //刷新当前索引所在对象的类型
        //        //获取泛型的类型参数，假如没有或有但不止一个则检查是否为数组，否则终止循环（无法继续处理）
        //        Type genericType;
        //        Type[] genericTypes = entityType.GenericTypeArguments;
        //        //假如是具有泛型参数的泛型类
        //        if (genericTypes != null || genericTypes.Length == 1)
        //            genericType = genericTypes[0];
        //        //假如是数组
        //        else if (entityType.FullName.EndsWith("[]"))
        //            //else if (targetPropertyType.IsArray)
        //            genericType = Type.GetType(entityType.FullName.TrimEnd('[', ']'));
        //        else
        //            break;
        //        //将Emumerable.Element<T>方法通过获取到的类型参数转化为泛型方法
        //        MethodInfo genericMethod = ReflectionUtil.ElementAtMethod.MakeGenericMethod(genericType);
        //        //执行静态的Element<T>方法，并迭代当前对象值
        //        //可能目标对象的索引长度不够，导致反射调用产生异常，此种情况直接捕捉
        //        try
        //        {
        //            currentEntity = genericMethod.Invoke(null, new object[] { currentEntity, index });
        //            entityType = currentEntity.GetType();
        //        }
        //        catch (TargetInvocationException) { }
        //        //listIndexes.Add(index);
        //    }
        //}
        #endregion

        /// <summary>
        /// 从指定的实体中根据给定的方括号索引来获取实体对应索引位置的元素值，同时返回最终的元素类型
        /// </summary>
        /// <param name="currentEntity">从中获取索引元素的指定实体</param>
        /// <param name="indices">所有方括号索引</param>
        /// <param name="entityType">最终的元素类型</param>
#if NET45_OR_GREATER
        public static void GetEntityByBracketIndexes(ref object currentEntity, IEnumerable<int> indices, ref Type entityType)
#elif NET9_0_OR_GREATER
        public static void GetEntityByBracketIndexes(ref object? currentEntity, IEnumerable<int> indices, ref Type? entityType)
#endif
        {
            GetEntityByBracketIndexes(ref currentEntity, out _, indices, ref entityType);
        }

        #region 旧方法V2：GetEntityByBracketIndexes
        ///// <summary>
        ///// 从指定的实体中根据给定的方括号索引来获取实体对应索引位置的元素值，同时返回最终的元素类型
        ///// </summary>
        ///// <param name="currentEntity">从中获取索引元素的指定实体，方法执行完毕后将成为索引处元素的实体</param>
        ///// <param name="midLevelEntity">从中获取索引元素的指定实体，输入时与currentEntity相同，方法执行完毕后保持不变</param>
        ///// <param name="indices">所有方括号索引</param>
        ///// <param name="entityType">最终的元素类型</param>
        //public static void GetEntityByBracketIndexes(ref object currentEntity, out object midLevelEntity, IEnumerable<int> indices, ref Type entityType)
        //{
        //    Type genericType = null;
        //    midLevelEntity = currentEntity;
        //    if (indices == null || indices.Count() == 0 || currentEntity == null)
        //        goto ENDING;

        //    entityType = currentEntity.GetType(); //在初次进入方法时确认实体类型
        //    foreach (int index in indices)
        //    {
        //        ////获取泛型的类型参数，假如没有或有但不止一个则检查是否为数组，否则终止循环（无法继续处理）
        //        //Type[] genericTypes = entityType.GenericTypeArguments;
        //        ////假如是具有泛型参数的泛型类
        //        //if (genericTypes != null && genericTypes.Length > 0)
        //        //    genericType = genericTypes[0];
        //        ////假如是数组
        //        //else if (entityType.FullName.EndsWith("[]"))
        //        //    //else if (targetPropertyType.IsArray)
        //        //    genericType = Type.GetType(entityType.FullName.TrimEnd('[', ']'));
        //        //else
        //        //    break;
        //        //获取泛型或数组的类型参数，假如为空则终止循环（无法继续处理）
        //        if ((genericType = entityType.GetGenericType()) == null)
        //            break;

        //        //将Emumerable.Element<T>方法通过获取到的类型参数转化为泛型方法
        //        MethodInfo genericMethod = ReflectionUtil.ElementAtMethod.MakeGenericMethod(genericType);
        //        //执行静态的Element<T>方法，并迭代当前对象值
        //        //可能目标对象的索引长度不够，导致反射调用产生异常，此种情况直接捕捉
        //        try
        //        {
        //            currentEntity = genericMethod.Invoke(null, new object[] { currentEntity, index });
        //            //entityType = currentEntity.GetType(); //在刷新实体之后再次确认实体类型，为下一次循环做准备
        //            entityType = currentEntity == null ? genericType : currentEntity.GetType(); //在刷新实体之后再次确认实体类型，为下一次循环做准备（假如实体为空则沿用推断出的类型）
        //        }
        //        //catch (TargetInvocationException) { }
        //        catch (TargetInvocationException)
        //        {
        //            //假如出现异常则将目标元素对象设置为null，并退出循环
        //            currentEntity = null;
        //            goto ENDING;
        //        }
        //        //listIndexes.Add(index);
        //    }
        //ENDING:
        //    //（此步不可缺少，因有可能通过标签直接跳到这一行）刷新当前索引所在对象的类型，假如所在对象为空则使用泛型类型
        //    entityType = currentEntity != null ? currentEntity.GetType() : genericType;
        //}
        #endregion

        /// <summary>
        /// 从指定的实体中根据给定的方括号索引来获取实体对应索引位置的元素值，同时返回最终的元素类型
        /// </summary>
        /// <param name="currentEntity">从中获取索引元素的指定实体，方法执行完毕后将成为索引处元素的实体</param>
        /// <param name="midLevelEntity">从中获取索引元素的指定实体，输入时与currentEntity相同，方法执行完毕后保持不变</param>
        /// <param name="indices">所有方括号索引</param>
        /// <param name="entityType">最终的元素类型</param>
#if NET45_OR_GREATER
        public static void GetEntityByBracketIndexes(ref object currentEntity, out object midLevelEntity, IEnumerable<int> indices, ref Type entityType)
#elif NET9_0_OR_GREATER
        public static void GetEntityByBracketIndexes(ref object? currentEntity, out object? midLevelEntity, IEnumerable<int> indices, ref Type? entityType)
#endif
        {
            Type
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            genericType = null;
            midLevelEntity = currentEntity;
            if (indices == null || !indices.Any() || currentEntity == null)
                goto ENDING;

            //【防御性处理】检查集合是否为空或索引是否越界，
            //提前拦截以避免后续反射调用抛出异常并将 null 向下传递
            if (currentEntity is System.Collections.IEnumerable enumerable)
            {
                int count = 0;
                foreach (var _ in enumerable) { count++; }
                int firstIndex = indices.First();
                if (count == 0)
                {
                    Debug.WriteLine($"[PropertyMapper] 索引访问失败：集合为空，无法获取索引 [{firstIndex}] 的元素。");
                    currentEntity = null;
                    goto ENDING;
                }
                if (firstIndex < 0 || firstIndex >= count)
                {
                    Debug.WriteLine($"[PropertyMapper] 索引越界：索引 [{firstIndex}] 超出范围（集合长度: {count}）。");
                    currentEntity = null;
                    goto ENDING;
                }
            }

            entityType = currentEntity.GetType(); //在初次进入方法时确认实体类型
            foreach (int index in indices)
            {
                //获取泛型或数组的类型参数，假如为空则终止循环（无法继续处理）
                if ((genericType = entityType.GetGenericType()) == null)
                    break;

                //将Emumerable.Element<T>方法通过获取到的类型参数转化为泛型方法
                MethodInfo
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            genericMethod = ReflectionUtil.ElementAtMethod?.MakeGenericMethod(genericType);
                //执行静态的Element<T>方法，并迭代当前对象值
                //可能目标对象的索引长度不够，导致反射调用产生异常，此种情况直接捕捉
                try
                {
#if NET45_OR_GREATER
                    currentEntity = genericMethod.Invoke(null, new object[] { currentEntity, index });
#elif NET9_0_OR_GREATER
                    currentEntity = genericMethod?.Invoke(null, [currentEntity, index]);
#endif
                    entityType = currentEntity == null ? genericType : currentEntity.GetType(); //在刷新实体之后再次确认实体类型，为下一次循环做准备（假如实体为空则沿用推断出的类型）
                }
                catch (TargetInvocationException)
                {
                    //假如出现异常则将目标元素对象设置为null，并退出循环
                    currentEntity = null;
                    goto ENDING;
                }
            }
        ENDING:
            //（此步不可缺少，因有可能通过标签直接跳到这一行）刷新当前索引所在对象的类型，假如所在对象为空则使用泛型类型
            entityType = currentEntity != null ? currentEntity.GetType() : genericType;
        }
        #endregion

        /// <summary>
        /// 输入参数中给定用来获取目标实体中属性的PropertyMapper描述字符串，根据PropertyMapper在目标实体中获取对应属性的值（可由'.'符号指定子属性）
        /// </summary>
        /// <typeparam name="Target">获取属性值的目标实体类型参数</typeparam>
        /// <param name="target">获取属性值的目标实体</param>
        /// <param name="propMapper">指定属性的名称，形如“Class.Student.Name”</param>
        /// <param name="nullValueHandling">假如读取到的属性值为空时的操作，<see cref="NullValueHandling.Skip"/>将直接忽略并输出null，<see cref="NullValueHandling.Ignore"/>将尝试初始化为对应类型的默认值</param>
        /// <param name="convertedToType">要转换到的类型，假如为null则不转换</param>
#if NET45_OR_GREATER
        public static object GetPropertyValue<Target>(this Target target, string propMapper, NullValueHandling nullValueHandling = NullValueHandling.Skip, Type convertedToType = null)
#elif NET9_0_OR_GREATER
        public static object? GetPropertyValue<Target>(this Target target, string propMapper, NullValueHandling nullValueHandling = NullValueHandling.Skip, Type? convertedToType = null)
#endif
        {
            return GetPropertyValue(target, propMapper, out _, nullValueHandling, convertedToType);
        }

        /// <summary>
        /// 输入参数中给定用来获取目标实体中属性的PropertyMapper描述字符串，根据PropertyMapper在目标实体中获取对应属性的值（可由'.'符号指定子属性）
        /// </summary>
        /// <typeparam name="Target">获取属性值的目标实体类型参数</typeparam>
        /// <param name="target">获取属性值的目标实体</param>
        /// <param name="propMapper">指定属性的名称，形如“Class.Student.Name”</param>
        /// <param name="targetPropertyType">指定属性的类型</param>
        /// <param name="nullValueHandling">假如读取到的属性值为空时的操作，<see cref="NullValueHandling.Skip"/>将直接忽略并输出null，<see cref="NullValueHandling.Ignore"/>将尝试初始化为对应类型的默认值</param>
        /// <param name="convertedToType">要转换到的类型，假如为null则不转换</param>
#if NET45_OR_GREATER
        public static object GetPropertyValue<Target>(this Target target, string propMapper, out Type targetPropertyType, NullValueHandling nullValueHandling = NullValueHandling.Skip,
            Type convertedToType = null)
#elif NET9_0_OR_GREATER
        public static object? GetPropertyValue<Target>(this Target target, string propMapper, out Type? targetPropertyType, NullValueHandling nullValueHandling = NullValueHandling.Skip,
            Type? convertedToType = null)
#endif
        {
            #region 获取属性
#if NET45_OR_GREATER
            PropertyInfo targetProperty = target.GetEntityProperty_InConstruction(propMapper, false, out object upperLevelTarget, out object lowerLevelTarget, out int[] indices);
#elif NET9_0_OR_GREATER
            PropertyInfo? targetProperty = target.GetEntityProperty_InConstruction(propMapper, false, out object? upperLevelTarget, out object? lowerLevelTarget, out int[]? indices);
#endif
            targetPropertyType = null;
            #endregion

            object
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            targetValue = null;
            //假如目标属性不存在或不可读
            if (targetProperty == null || !targetProperty.CanRead || upperLevelTarget == null)
                goto NULL_HANDLING;

            //假如目标属性为集合或数组中的元素，则将复制的对象更改为已拿到的目标元素对象
            if (indices != null && indices.Length > 0)
            {
                targetValue = lowerLevelTarget;
                if (lowerLevelTarget != null)
                    targetPropertyType = lowerLevelTarget.GetType();
            }
            else
            {
                targetValue = targetProperty.GetValue(upperLevelTarget);
                targetPropertyType = targetProperty.PropertyType;
            }
            //假如给定了要转换的类型，则尝试进行赋值或转换，假如目标属性值为空或操作失败则进行操作选项的判断
            try
            {
                if (convertedToType != null)
                {
                    targetValue = convertedToType == targetPropertyType ? targetValue : Converter.Convert(convertedToType, targetValue);
                    if (targetValue != null)
                        goto RETURN_VALUE;
                }
            }
            catch (Exception) { }
            //目标属性为空、不可读、目标属性值为空的操作
            NULL_HANDLING:
            //对应属性值为null时的操作（值为null或者未找到属性）
            if (targetValue == null)
            {
                //跳过当前目标属性
                if (nullValueHandling == NullValueHandling.Skip) goto RETURN_VALUE;
                //使用默认初始化的值（值类型），或使用默认构造器初始化（引用类型）
                //else if (nullValueHandling == NullValueHandling.Ignore)
                //    targetValue = convertedToType != null ? convertedToType.CreateDefValue() : targetPropertyType.CreateDefValue();
                else if (nullValueHandling == NullValueHandling.Ignore)
                {
                    if (convertedToType != null)
                        targetValue = convertedToType.CreateDefValue();
                    else if (targetPropertyType != null)
                        targetValue = targetPropertyType.CreateDefValue();

                    // 如果targetPropertyType为null，你需要决定如何处理这种情况
                    // 这里可以选择抛出异常、使用默认值或者其他逻辑
                    // 或者 throw new ArgumentNullException(nameof(targetPropertyType));
                    else
                        targetValue = default;
                }
            }
            RETURN_VALUE:
            return targetValue;
        }

        #region CopyPropertyValueTo/From 扩展方法
        /// <summary>
        /// 获取源实体中每个属性的PropertyMapper特性，根据特性值在目标实体中寻找对应属性并为该属性赋值（可由'.'符号指定子属性）
        /// </summary>
        /// <typeparam name="Target">目标实体类型参数</typeparam>
        /// <param name="source">源实体</param>
        /// <param name="target">目标实体</param>
#if NET45_OR_GREATER
        public static void CopyPropertyValueTo<Target>(this object source, ref Target target)
#elif NET9_0_OR_GREATER
        public static void CopyPropertyValueTo<Target>(this object source, ref Target? target)
#endif
        {
            Type sourceType = source.GetType(), targetType = typeof(Target); //获取源类型与目标类型
            //获取所有属性，假如源实体为空或类型不包含任何属性则退出
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            if (source == null || sourceProperties == null || sourceProperties.Length == 0)
                return;
            //假如目标实体为空，则初始化
#if NET45_OR_GREATER
            if (target == null)
                target = (Target)Activator.CreateInstance(targetType);
#elif NET9_0_OR_GREATER
            target ??= (Target?)Activator.CreateInstance(targetType);
#endif
            //遍历每个属性，找到PropertyMapper特性并根据特性值为目标实体属性赋值
            foreach (var sourceProperty in sourceProperties)
            {
                //假如有PropertyMapper特性且属性有get访问器
                IEnumerable<PropertyMapperToAttribute> attrs = sourceProperty.GetCustomAttributes<PropertyMapperToAttribute>(false);
                //if (attrs == null || attrs.Count() == 0 || sourceProperty.GetGetMethod() == null)
                if (attrs == null || !attrs.Any() || !sourceProperty.CanRead)
                    continue;
                object
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            sourceValue = sourceProperty.GetValue(source);
                foreach (var attr in attrs)
                {
                    if (string.IsNullOrWhiteSpace(attr.PropertyMapper))
                        continue;

                    #region 新判断方式
#if NET45_OR_GREATER
                    PropertyInfo targetProperty = target.GetEntityProperty_InConstruction(attr.PropertyMapper, true, out object upperLevelTarget, out object midLevelTarget, out _, out int[] indices);
#elif NET9_0_OR_GREATER
                    PropertyInfo? targetProperty = target.GetEntityProperty_InConstruction(attr.PropertyMapper, true, out object? upperLevelTarget, out object? midLevelTarget, out _, out int[]? indices);
#endif
                    //假如未找到该属性，进入下一次循环
                    if (targetProperty == null)
                        continue;
                    #endregion

                    #region 新属性赋值方法
#if NET45_OR_GREATER
                    Type targetPropertyType;
#elif NET9_0_OR_GREATER
                    Type? targetPropertyType;
#endif
                    //目标属性是否不带索引
                    bool noIndices = indices == null || indices.Length == 0;
                    //不带索引直接判断属性类型，否则判断数组的类型（目前仅支持数组暂不支持List或集合等）
                    if (noIndices)
                        targetPropertyType = targetProperty.PropertyType;
                    else
                    {
                        //判断中间实体（数组、List或集合）的类型
                        Type
                        //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
                        ?
#endif
                        midType = midLevelTarget?.GetType();
                        //获取泛型List或数组的类型参数，假如为空则进入下一个循环（当前循环的Attribute无法继续处理）
                        targetPropertyType = midType?.GetGenericType();
                        if (targetPropertyType == null)
                            continue;
                    }
                    //获取源属性的值，假如类型与目标类型不同则转换
                    object
                    //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
                    ?
#endif
                    targetValue = sourceProperty.PropertyType == targetPropertyType ? sourceValue : Converter.Convert(targetPropertyType, sourceValue);
                    //不带索引直接判断为属性赋值，否则判断数组的类型（目前仅支持数组暂不支持List或集合等）
                    if (noIndices)
                        //假如赋值失败，直接跳过当前循环
                        try
                        {
                            if (targetProperty.CanWrite)
                                targetProperty.SetValue(upperLevelTarget, targetValue);
                        }
                        catch (Exception) { continue; }
                    //else
                    else if(indices != null && indices.Length > 0) // 添加空值检查
                    {
#if NET45_OR_GREATER
                        ReflectionUtil.SetValueMethod?.Invoke(midLevelTarget, new object[] { targetValue, indices[0] });
#elif NET9_0_OR_GREATER
                        ReflectionUtil.SetValueMethod?.Invoke(midLevelTarget, [targetValue, indices[0]]);
#endif
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 获取源实体中每个属性的PropertyMapper特性，根据特性值在目标实体中获取对应属性的值并赋给源实体中的属性（可由'.'符号指定子属性）
        /// </summary>
        /// <typeparam name="Target">目标实体类型参数</typeparam>
        /// <param name="source">源实体</param>
        /// <param name="target">目标实体</param>
#if NET45_OR_GREATER
        public static void CopyPropertyValueFrom<Target>(this object source, Target target)
#elif NET9_0_OR_GREATER
        public static void CopyPropertyValueFrom<Target>(this object? source, Target target)
#endif
        {
            if (source == null)
                return;

            Type sourceType = source.GetType(); //获取源类型与目标类型
            //获取所有属性，假如目标实体为空或类型不包含任何属性则退出
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            if (target == null || sourceProperties == null || sourceProperties.Length == 0)
                return;
            //假如源实体为空，则初始化
#if NET45_OR_GREATER
            if (source == null)
                source = Activator.CreateInstance(sourceType);
#elif NET9_0_OR_GREATER
            source ??= Activator.CreateInstance(sourceType);
#endif
            //遍历每个属性，找到PropertyMapper特性并根据特性值从目标实体属性获取值并赋给当前属性
            foreach (var sourceProperty in sourceProperties)
            {
                //假如有PropertyMapper特性且属性有set访问器
                IEnumerable<PropertyMapperFromAttribute> attrs = sourceProperty.GetCustomAttributes<PropertyMapperFromAttribute>(false);
                //if (attrs == null || attrs.Count() == 0 || sourceProperty.GetSetMethod() == null)
                if (attrs == null || !attrs.Any() || !sourceProperty.CanWrite)
                    continue;

                Type sourcePropertyType = sourceProperty.PropertyType;
                //object类型默认值为null
#if NET45_OR_GREATER
                object targetValue = null;
#elif NET9_0_OR_GREATER
                object? targetValue = null;
#endif

                foreach (var attr in attrs)
                {
                    if (string.IsNullOrWhiteSpace(attr.PropertyMapper))
                        continue;

                    #region 新取值方法
                    targetValue = target.GetPropertyValue(attr.PropertyMapper, attr.NullValueHandling, sourcePropertyType);
                    //对应属性值为null时的操作，选择跳过时将不进行赋值
                    if (targetValue == null && attr.NullValueHandling == NullValueHandling.Skip) continue;
                    #endregion

                    #region 旧取值方法
                    //#region 新判断方法
                    //PropertyInfo targetProperty = target.GetEntityProperty_InConstruction(attr.PropertyMapper, false, out object upperLevelTarget, out object lowerLevelTarget, out int[] indices);
                    //Type targetPropertyType = null;
                    //#endregion

                    //#region 新属性复制方法
                    ////假如目标属性不存在或不可读
                    //if (targetProperty == null || !targetProperty.CanRead || upperLevelTarget == null)
                    //    goto NULL_HANDLING;

                    ////假如目标属性为集合或数组中的元素，则将复制的对象更改为已拿到的目标元素对象
                    //if (indices != null && indices.Length > 0)
                    //{
                    //    targetValue = lowerLevelTarget;
                    //    if (lowerLevelTarget != null)
                    //        targetPropertyType = lowerLevelTarget.GetType();
                    //}
                    //else
                    //{
                    //    ////假如目标属性不存在或不可读
                    //    //if (targetProperty == null || !targetProperty.CanRead || upperLevelTarget == null)
                    //    //    goto NULL_HANDLING;
                    //    targetValue = targetProperty.GetValue(upperLevelTarget);
                    //    targetPropertyType = targetProperty.PropertyType;
                    //}
                    ////尝试进行赋值或转换，假如目标属性值为空或操作失败则进行操作选项的判断
                    //try
                    //{
                    //    targetValue = sourcePropertyType == targetPropertyType ? targetValue : Converter.Convert(sourcePropertyType, targetValue);
                    //    if (targetValue != null)
                    //        goto SET_VALUE;
                    //}
                    //catch (Exception) { }
                    ////目标属性为空、不可读、目标属性值为空的操作
                    //NULL_HANDLING:
                    ////对应属性值为null时的操作（值为null或者未找到属性）
                    //if (targetValue == null)
                    //{
                    //    //跳过当前目标属性
                    //    if (attr.NullValueHandling == NullValueHandling.Skip) continue;
                    //    //使用默认初始化的值（值类型），或使用默认构造器初始化（引用类型）
                    //    else if (attr.NullValueHandling == NullValueHandling.Ignore)
                    //        targetValue = sourcePropertyType.CreateDefValue();
                    //}
                    //SET_VALUE:
                    #endregion

                    sourceProperty.SetValue(source, targetValue);
                    //#endregion
                }
            }
        }

        //public static void CopyPropertyValueFrom<Target>(this object source, Target target)
        ///// <summary>
        ///// 获取源实体中每个属性的PropertyMapper特性，根据特性值在目标实体中获取对应属性的值并赋给源实体中的属性（可由'.'符号指定子属性）
        ///// </summary>
        ///// <typeparam name="Target">目标实体类型参数</typeparam>
        ///// <param name="source">源实体</param>
        ///// <param name="target">目标实体</param>
        //public static void CopyPropertyValueFrom<Target>(this object source, Target target)
        //{
        //    Type sourceType = source.GetType()/*, targetType = typeof(Target)*/; //获取源类型与目标类型
        //    //获取所有属性，假如目标实体为空或类型不包含任何属性则退出
        //    PropertyInfo[] sourceProperties = sourceType.GetProperties();
        //    if (target == null || sourceProperties == null || sourceProperties.Length == 0)
        //        return;
        //    //假如源实体为空，则初始化
        //    if (source == null)
        //        source = Activator.CreateInstance(sourceType);
        //    //遍历每个属性，找到PropertyMapper特性并根据特性值从目标实体属性获取值并赋给当前属性
        //    foreach (var sourceProperty in sourceProperties)
        //    {
        //        //假如有PropertyMapper特性且属性有set访问器
        //        IEnumerable<PropertyMapperFromAttribute> attrs = sourceProperty.GetCustomAttributes<PropertyMapperFromAttribute>(false);
        //        //if (attrs == null || attrs.Count() == 0 || sourceProperty.GetSetMethod() == null)
        //        if (attrs == null || attrs.Count() == 0 || !sourceProperty.CanWrite)
        //            continue;

        //        Type sourcePropertyType = sourceProperty.PropertyType;
        //        //object targetValue = default;
        //        object targetValue = null; //object类型默认值为null

        //        foreach (var attr in attrs)
        //        {
        //            if (string.IsNullOrWhiteSpace(attr.PropertyMapper))
        //                continue;

        //            #region 新判断方法
        //            PropertyInfo targetProperty = target.GetEntityProperty_InConstruction(attr.PropertyMapper, false, out object upperLevelTarget, out object lowerLevelTarget, out int[] indices);
        //            Type targetPropertyType = null;
        //            #endregion

        //            #region 新属性复制方法
        //            //假如目标属性不存在或不可读
        //            if (targetProperty == null || !targetProperty.CanRead || upperLevelTarget == null)
        //                goto NULL_HANDLING;

        //            //假如目标属性为集合或数组中的元素，则将复制的对象更改为已拿到的目标元素对象
        //            if (indices != null && indices.Length > 0)
        //            {
        //                targetValue = lowerLevelTarget;
        //                if (lowerLevelTarget != null)
        //                    targetPropertyType = lowerLevelTarget.GetType();
        //            }
        //            else
        //            {
        //                ////假如目标属性不存在或不可读
        //                //if (targetProperty == null || !targetProperty.CanRead || upperLevelTarget == null)
        //                //    goto NULL_HANDLING;
        //                targetValue = targetProperty.GetValue(upperLevelTarget);
        //                targetPropertyType = targetProperty.PropertyType;
        //            }
        //            //尝试进行赋值或转换，假如目标属性值为空或操作失败则进行操作选项的判断
        //            try
        //            {
        //                targetValue = sourcePropertyType == targetPropertyType ? targetValue : Converter.Convert(sourcePropertyType, targetValue);
        //                if (targetValue != null)
        //                    goto SET_VALUE;
        //            }
        //            catch (Exception) { }
        //            //目标属性为空、不可读、目标属性值为空的操作
        //            NULL_HANDLING:
        //            //对应属性值为null时的操作（值为null或者未找到属性）
        //            if (targetValue == null)
        //            {
        //                //跳过当前目标属性
        //                if (attr.NullValueHandling == NullValueHandling.Skip) continue;
        //                //使用默认初始化的值（值类型），或null（引用类型）
        //                else if (attr.NullValueHandling == NullValueHandling.Ignore)
        //                    targetValue = sourcePropertyType.CreateDefValue();
        //            }
        //            SET_VALUE:
        //            sourceProperty.SetValue(source, targetValue);
        //            #endregion
        //        }
        //    }
        //}
        #endregion
    }

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
        /// <param name="nullValueHandling">当目标属性值为空时的处理方法</param>
        public PropertyMapperToAttribute(string mapper, NullValueHandling nullValueHandling/* = NullValueHandling.Ignore*/)
        {
            PropertyMapper = mapper;
            NullValueHandling = nullValueHandling;
        }

        /// <summary>
        /// 以指定Mapper名称初始化特性
        /// </summary>
        /// <param name="mapper">目标属性的名称，可由'.'符号指定子属性</param>
        public PropertyMapperToAttribute(string mapper) : this(mapper, NullValueHandling.Ignore) { }

        /// <summary>
        /// 特性的目标属性名称，可由'.'符号指定子属性
        /// </summary>
        public string PropertyMapper { get; }

        /// <summary>
        /// 向目标属性粘贴时假如目标属性为空（找不到或超出索引）的操作选项
        /// </summary>
        public NullValueHandling NullValueHandling { get; }
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
        /// <param name="nullValueHandling">当目标属性不存在、目标属性不可读或目标属性值为空时的处理方法</param>
        public PropertyMapperFromAttribute(string mapper, NullValueHandling nullValueHandling/* = NullValueHandling.Ignore*/)
        {
            PropertyMapper = mapper;
            NullValueHandling = nullValueHandling;
        }

        /// <summary>
        /// 以指定Mapper名称初始化特性
        /// </summary>
        /// <param name="mapper">目标属性的名称，可由'.'符号指定子属性</param>
        public PropertyMapperFromAttribute(string mapper) : this(mapper, NullValueHandling.Ignore) { }

        /// <summary>
        /// 特性的目标属性名称，可由'.'符号指定子属性
        /// </summary>
        public string PropertyMapper { get; }

        /// <summary>
        /// 从目标属性复制时假如目标属性不存在（找不到或超出索引）、目标属性不可读或目标属性值为空的操作选项
        /// </summary>
        public NullValueHandling NullValueHandling { get; }
    }

    /// <summary>
    /// 从目标属性复制/向目标属性粘贴时假如目标属性不存在（找不到或超出索引）、目标属性不可读或目标属性值为空的处理方法
    /// </summary>
    public enum NullValueHandling
    {
        /// <summary>
        /// 无视错误，生成默认的初始化值继续进行复制
        /// </summary>
        Ignore,

        /// <summary>
        /// 跳过当前目标属性的复制操作
        /// </summary>
        Skip,

        ///// <summary>
        ///// 停止所有从目标属性复制的操作
        ///// </summary>
        //Stop,
    }
}
