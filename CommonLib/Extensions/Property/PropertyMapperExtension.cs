using CommonLib.Extensions.Reflection;
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
        /// <param name="lowerLevelEntity">指定属性所对应的实体，假如属性名称内带有索引，则为索引处元素的实体；假如属性名形如“Class.Student”，则对应实体就是Student，假如属性名形如“Class.Desk[0]”，则对应实体就是Desk列表的第一个元素</param>
        /// <param name="indices">假如最底层属性名称中带有枚举数的索引，则将这些索引以uint数组的形式输出出来，否则输出null</param>
        /// <returns></returns>
        public static PropertyInfo GetEntityProperty_InConstruction(this object entity, string propMapper, bool initProp, out object upperLevelEntity, out object lowerLevelEntity, out int[] indices)
        {
            return GetEntityProperty_InConstruction(entity, propMapper, initProp, out upperLevelEntity, out _, out lowerLevelEntity, out indices);
        }

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
        public static PropertyInfo GetEntityProperty_InConstruction(this object entity, string propMapper, bool initProp, out object upperLevelEntity, out object midLevelEntity, out object lowerLevelEntity, out int[] indices)
        {
            PropertyInfo targetProperty = null;
            //upperLevelEntity = lowerLevelEntity = entity;
            upperLevelEntity = midLevelEntity = lowerLevelEntity = entity;
            //lowerLevelEntity = entity;
            List<int> listIndexes = new List<int>();
            if (entity == null || string.IsNullOrWhiteSpace(propMapper))
                goto ENDING;

            string[] parts = propMapper.Split('.'); //根据'.'拆分，以寻找子属性
            Type targetPropertyType = entity.GetType(); //目标属性的类型
            //上层目标实体与下层目标实体：当只有一层时前者为参数entity，后者为第一层属性值；当有多层时则依次递进
            //upperLevelEntity = entity;
            //object lowerLevelTarget = entity;
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
                //string brackets = _regexBrackets.Match(fullPart).Value;
                //string brackets, part = fullPart.Replace(brackets = _regexBrackets.Match(fullPart).Value, string.Empty); //报错
                string brackets = _regexBrackets.Match(fullPart).Value, part = string.IsNullOrWhiteSpace(brackets) ? fullPart : fullPart.Replace(brackets, string.Empty);
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
                lowerLevelEntity = targetProperty.GetValue(upperLevelEntity); //假如有下个循环，则当前层实体将成为下个循环的上层实体
                #region 根据方括号内的索引逐层获取列表内或数组内的元素
                #region previous crap
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
                #endregion
                //假如没有方括号索引，直接进入下一次循环
                MatchCollection coll = _regexIndexes.Matches(brackets);
                if (coll == null || coll.Count == 0)
                    continue;
                listIndexes = coll.Cast<Match>().Select(match => int.Parse(match.Value.Trim('[', ']'))).ToList();
                //GetEntityByBracketIndexes(ref lowerLevelEntity, listIndexes, ref targetPropertyType);
                GetEntityByBracketIndexes(ref lowerLevelEntity, out midLevelEntity, listIndexes, ref targetPropertyType);
                #endregion
            }
        ENDING:
            indices = listIndexes == null || listIndexes.Count == 0 ? null : listIndexes.ToArray();
            return targetProperty;
        }

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

        /// <summary>
        /// 从指定的实体中根据给定的方括号索引来获取实体对应索引位置的元素值，同时返回最终的元素类型
        /// </summary>
        /// <param name="currentEntity">从中获取索引元素的指定实体</param>
        /// <param name="indices">所有方括号索引</param>
        /// <param name="entityType">最终的元素类型</param>
        public static void GetEntityByBracketIndexes(ref object currentEntity, IEnumerable<int> indices, ref Type entityType)
        {
            GetEntityByBracketIndexes(ref currentEntity, out _, indices, ref entityType);
        }

        /// <summary>
        /// 从指定的实体中根据给定的方括号索引来获取实体对应索引位置的元素值，同时返回最终的元素类型
        /// </summary>
        /// <param name="currentEntity">从中获取索引元素的指定实体，方法执行完毕后将成为索引处元素的实体</param>
        /// <param name="midLevelEntity">从中获取索引元素的指定实体，输入时与currentEntity相同，方法执行完毕后保持不变</param>
        /// <param name="indices">所有方括号索引</param>
        /// <param name="entityType">最终的元素类型</param>
        public static void GetEntityByBracketIndexes(ref object currentEntity, out object midLevelEntity, IEnumerable<int> indices, ref Type entityType)
        {
            Type genericType = null;
            midLevelEntity = currentEntity;
            if (indices == null || indices.Count() == 0 || currentEntity == null)
                goto ENDING;

            entityType = currentEntity.GetType(); //在初次进入方法时确认实体类型
            foreach (int index in indices)
            {
                ////获取泛型的类型参数，假如没有或有但不止一个则检查是否为数组，否则终止循环（无法继续处理）
                //Type[] genericTypes = entityType.GenericTypeArguments;
                ////假如是具有泛型参数的泛型类
                //if (genericTypes != null && genericTypes.Length > 0)
                //    genericType = genericTypes[0];
                ////假如是数组
                //else if (entityType.FullName.EndsWith("[]"))
                //    //else if (targetPropertyType.IsArray)
                //    genericType = Type.GetType(entityType.FullName.TrimEnd('[', ']'));
                //else
                //    break;
                //获取泛型或数组的类型参数，假如为空则终止循环（无法继续处理）
                if ((genericType = entityType.GetGenericType()) == null)
                    break;

                //将Emumerable.Element<T>方法通过获取到的类型参数转化为泛型方法
                MethodInfo genericMethod = ReflectionUtil.ElementAtMethod.MakeGenericMethod(genericType);
                //执行静态的Element<T>方法，并迭代当前对象值
                //可能目标对象的索引长度不够，导致反射调用产生异常，此种情况直接捕捉
                try
                {
                    currentEntity = genericMethod.Invoke(null, new object[] { currentEntity, index });
                    //entityType = currentEntity.GetType(); //在刷新实体之后再次确认实体类型，为下一次循环做准备
                    entityType = currentEntity == null ? genericType : currentEntity.GetType(); //在刷新实体之后再次确认实体类型，为下一次循环做准备（假如实体为空则沿用推断出的类型）
                }
                //catch (TargetInvocationException) { }
                catch (TargetInvocationException)
                {
                    //假如出现异常则将目标元素对象设置为null，并退出循环
                    currentEntity = null;
                    goto ENDING;
                }
                //listIndexes.Add(index);
            }
        ENDING:
            //（此步不可缺少，因有可能通过标签直接跳到这一行）刷新当前索引所在对象的类型，假如所在对象为空则使用泛型类型
            entityType = currentEntity != null ? currentEntity.GetType() : genericType;
        }

        ///// <summary>
        ///// 从指定的实体中根据给定的方括号索引来获取实体对应索引位置的元素值，同时返回最终的元素类型
        ///// </summary>
        ///// <param name="currentEntity">从中获取索引元素的指定实体</param>
        ///// <param name="indexes">所有方括号索引</param>
        ///// <param name="entityType">最终的元素类型</param>
        //public static void GetEntityByBracketIndexes(ref object currentEntity, IEnumerable<int> indexes, ref Type entityType)
        //{
        //    Type genericType = null;
        //    if (indexes == null || indexes.Count() == 0 || currentEntity == null)
        //        goto ENDING;

        //    entityType = currentEntity.GetType(); //在初次进入方法时确认实体类型
        //    foreach (int index in indexes)
        //    {
        //        //获取泛型的类型参数，假如没有或有但不止一个则检查是否为数组，否则终止循环（无法继续处理）
        //        //Type genericType;
        //        Type[] genericTypes = entityType.GenericTypeArguments;
        //        //假如是具有泛型参数的泛型类
        //        if (genericTypes != null && genericTypes.Length > 0)
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
        //            entityType = currentEntity.GetType(); //在刷新实体之后再次确认实体类型，为下一次循环做准备
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
        //    ////刷新当前索引所在对象的类型，假如所在对象为空则类型同样为空
        //    //entityType = currentEntity?.GetType();
        //    //刷新当前索引所在对象的类型，假如所在对象为空则使用泛型类型
        //    entityType = currentEntity != null ? currentEntity.GetType() : genericType;
        //}

        #region 旧GetEntityProperty方法
        ///// <summary>
        ///// 根据给定的属性名称在实体中查找指定属性的值，可查找子属性
        ///// </summary>
        ///// <param name="entity">待查询的实体</param>
        ///// <param name="propMapper">指定属性的名称，形如“Class.Student.Name”</param>
        ///// <param name="initProp">假如查找指定属性的过程中有任意一层属性为空，决定是否初始化</param>
        ///// <param name="upperLevelEntity">指定属性所属的实体：假如不查找子属性，则该实体就是当前待查询的实体；假如属性名形如“Class.Student.Name”，则所属实体就是Student</param>
        ///// <returns></returns>
        //public static PropertyInfo GetEntityProperty(this object entity, string propMapper, bool initProp, out object upperLevelEntity)
        //{
        //    PropertyInfo targetProperty = null;
        //    //upperLevelEntity = null;
        //    upperLevelEntity = entity;
        //    if (entity == null || string.IsNullOrWhiteSpace(propMapper))
        //        goto ENDING;

        //    string[] parts = propMapper.Split('.'); //根据'.'拆分，以寻找子属性
        //    Type targetPropertyType = entity.GetType(); //目标属性的类型
        //    //上层目标实体与下层目标实体：当只有一层时前者为参数entity，后者为第一层属性值；当有多层时则依次递进
        //    //upperLevelEntity = entity;
        //    object lowerLevelTarget = entity;
        //    //遍历PropertyMapper中指定的每一层属性
        //    foreach (var part in parts)
        //    {
        //        //假如不初始化空目标实体，且上层实体值为空，则当前层属性必然找不到，跳出循环
        //        if (!initProp && lowerLevelTarget == null)
        //        {
        //            targetProperty = null;
        //            break;
        //        }
        //        upperLevelEntity = lowerLevelTarget;
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
        //        lowerLevelTarget = targetProperty.GetValue(upperLevelEntity); //假如有下个循环，则当前层实体将成为下个循环的上层实体
        //    }

        //    ENDING:
        //    return targetProperty;
        //}
        #endregion

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
                    //PropertyInfo targetProperty = target.GetEntityProperty_InConstruction(attr.PropertyMapper, true, out object upperLevelTarget, out object midLevelTarget, out object lowerLevelTarget, out int[] indices);
                    PropertyInfo targetProperty = target.GetEntityProperty_InConstruction(attr.PropertyMapper, true, out object upperLevelTarget, out object midLevelTarget, out _, out int[] indices);
                    //假如未找到该属性，进入下一次循环
                    if (targetProperty == null)
                        continue;
                    //Type targetPropertyType = targetProperty.PropertyType;
                    #endregion

                    #region 旧属性赋值方法
                    ////假如最后一层实体不带索引，则直接赋值
                    //if (indexes == null || indexes.Length == 0)
                    //    targetProperty.SetValue(upperLevelTarget, sourceProperty.PropertyType == targetPropertyType ? sourceValue : Converter.Convert(targetPropertyType, sourceValue));
                    //else
                    //{
                    //    //TODO 反射给集合元素赋值需完善，在给集合赋值时需要判断是继承了IList或IDictionary，然后使用Array.SetValue方法
                    //    //targetPropertyType = lowerLevelTarget.GetType();
                    //    //targetProperty.SetValue(lowerLevelTarget, sourceProperty.PropertyType == targetPropertyType ? sourceValue : Converter.Convert(targetPropertyType, sourceValue));
                    //}
                    #endregion

                    #region 新属性赋值方法
                    Type targetPropertyType;
                    //目标属性是否不带索引
                    bool noIndices = indices == null || indices.Length == 0;
                    //不带索引直接判断属性类型，否则判断数组的类型（目前仅支持数组暂不支持List或集合等）
                    if (noIndices)
                        targetPropertyType = targetProperty.PropertyType;
                    else
                    {
                        //判断中间实体（数组、List或集合）的类型
                        Type midType = midLevelTarget?.GetType();
                        //获取泛型List或数组的类型参数，假如为空则进入下一个循环（当前循环的Attribute无法继续处理）
                        targetPropertyType = midType?.GetGenericType();
                        if (targetPropertyType == null)
                            continue;
                    }
                    //获取源属性的值，假如类型与目标类型不同则转换
                    object targetValue = sourceProperty.PropertyType == targetPropertyType ? sourceValue : Converter.Convert(targetPropertyType, sourceValue);
                    //不带索引直接判断为属性赋值，否则判断数组的类型（目前仅支持数组暂不支持List或集合等）
                    if (noIndices)
                        targetProperty.SetValue(upperLevelTarget, targetValue);
                    else
                        //TODO 在泛型List、数组或集合之中，CopyPropertyValueTo方法目前仅支持向数组中的元素赋值
                        ReflectionUtil.SetValueMethod.Invoke(midLevelTarget, new object[] { targetValue, indices[0] });
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
                    //PropertyInfo targetProperty = target.GetEntityProperty(attr.PropertyMapper, false, out object upperLevelTarget);
                    PropertyInfo targetProperty = target.GetEntityProperty_InConstruction(attr.PropertyMapper, false, out object upperLevelTarget, out object lowerLevelTarget, out int[] indices);
                    //Type targetPropertyType = targetProperty.PropertyType;
                    Type targetPropertyType = null;
                    #endregion

                    #region 旧属性复制方法
                    ////假如获取到了目标属性且目标属性具有set访问器
                    //if (targetProperty != null)
                    //    targetValue = targetProperty.GetValue(upperLevelTarget);
                    //if (indexes == null || indexes.Length == 0)
                    //    sourceProperty.SetValue(source, sourcePropertyType == targetPropertyType ? targetValue : Converter.Convert(sourcePropertyType, targetValue));
                    //else
                    //{
                    //    targetValue = lowerLevelTarget;
                    //    targetPropertyType = lowerLevelTarget.GetType();
                    //    sourceProperty.SetValue(source, sourcePropertyType == targetPropertyType ? targetValue : Converter.Convert(sourcePropertyType, targetValue));
                    //}
                    #endregion

                    #region 新属性复制方法
                    //假如目标属性为集合或数组中的元素，则将复制的对象更改为已拿到的目标元素对象
                    if (indices != null && indices.Length > 0)
                    {
                        targetValue = lowerLevelTarget;
                        if (lowerLevelTarget != null)
                            targetPropertyType = lowerLevelTarget.GetType();
                    }
                    //假如目标属性不属于集合或数组，而且获取到了目标属性且目标属性具有set访问器
                    else if (targetProperty != null)
                    {
                        targetValue = targetProperty.GetValue(upperLevelTarget);
                        targetPropertyType = targetProperty.PropertyType;
                    }
                    ////尝试进行赋值或转换，假如失败则进行操作选项的判断
                    //try { targetValue = sourcePropertyType == targetPropertyType ? targetValue : Converter.Convert(sourcePropertyType, targetValue); }
                    //catch (Exception)
                    //{
                    //    //跳过当前目标属性
                    //    if (attr.Option == CopyFromOptions.Skip) continue;
                    //    //使用默认初始化的值（值类型），或null（引用类型）
                    //    else if (attr.Option == CopyFromOptions.Ignore) targetValue = sourcePropertyType.IsValueType ? Activator.CreateInstance(sourcePropertyType) : null;
                    //}
                    //尝试进行赋值或转换，假如目标属性值为空或操作失败则进行操作选项的判断
                    try
                    {
                        targetValue = sourcePropertyType == targetPropertyType ? targetValue : Converter.Convert(sourcePropertyType, targetValue);
                        if (targetValue != null)
                            goto SET_VALUE;
                    }
                    catch (Exception) { }
                    //目标属性为空的操作
                    //跳过当前目标属性
                    if (attr.NullValueHandling == NullValueHandling.Skip) continue;
                    //使用默认初始化的值（值类型），或null（引用类型）
                    //else if (attr.Option == CopyFromOptions.Ignore) targetValue = sourcePropertyType.IsValueType ? Activator.CreateInstance(sourcePropertyType) : null;
                    else if (attr.NullValueHandling == NullValueHandling.Ignore) targetValue = sourcePropertyType.CreateDefValue();
                    SET_VALUE:
                    sourceProperty.SetValue(source, targetValue);
                    #endregion
                }
            }
        }
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
        /// <param name="nullValueHandling">当目标属性值为空时的处理方法</param>
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
        /// 从目标属性复制时假如目标属性为空（找不到或超出索引）的操作选项
        /// </summary>
        public NullValueHandling NullValueHandling { get; }
    }

    /// <summary>
    /// 从目标属性复制/向目标属性粘贴时假如目标属性为空（找不到或超出索引）的处理方法
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
