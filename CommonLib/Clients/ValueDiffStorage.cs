using CommonLib.Enums;
using CommonLib.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
//using System.Web.UI.Design.WebControls;

namespace CommonLib.Clients
{
    /// <summary>
    /// 数值存储器，存储当前值、上一个值以及差异值
    /// </summary>
    /// <typeparam name="T">数值的类型（只能是可比较类型）</typeparam>
    public class ValueDiffStorage<T> where T : IComparable
    {
        #region 事件
        /// <summary>
        /// 变化趋势改变事件
        /// </summary>
        public event ValueTrendChangedEventHandler<T>
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            ValueTrendChanged;

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event ValueChangedEventHandler<T>
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            ValueChangedEvent;
        #endregion

        #region 属性
        /// <summary>
        /// 差异值计数器，差异值符号改变时重置，否则每次数值改变都自增一次
        /// </summary>
        public uint DiffTicker { get; private set; }

        /// <summary>
        /// 差异计数器阈值，当差异计数等于这个值时触发事件（默认为0）
        /// </summary>
        public uint DiffTickerThreshold { get; set; }

        /// <summary>
        /// 量化变化量差异度的阈值，当差异度超过这个值时 <see cref="ValueChanged"/> 属性为true，同时 <see cref="ValueChangedEvent"/> 事件被触发
        /// <para/>假如泛型 T 不支持直接的数值转换或减法运算符，则忽略此阈值、直接将当前值与上一个值比较
        /// <para/>假如泛型 T 为数值类型，则此阈值默认值为0，此时只要当前值与上一个值不同，<see cref="ValueChanged"/> 属性就将为true
        /// <para/>假如泛型 T 为数值类型且阈值设置为负数，这种情况下不论新值与旧值的比较结果如何，<see cref="ValueChanged"/> 属性都将为true
        /// </summary>
        public T
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            ValueChangedThreshold
        { get; set; }

        /// <summary>
        /// 上一个值
        /// </summary>
        public T
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            PrevValue
        { get; private set; } = default;

        private T
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            _currValue = default, _prevValueSecAgo = default, _currValueSecLater = default; //当前值

        private DateTime /*_prevTime = DateTime.Now, */_currTime = DateTime.Now;

        /// <summary>
        /// 当前值，改变后将历史值存入FormerValue字段
        /// </summary>
        public T
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
             CurrentValue
        {
            get { return _currValue; }
            set
            {
                //尝试计算差值并与给定的差异量化阈值比较（数值类型）
                // 假如泛型 T 不支持直接的数值转换或减法运算符，则忽略此阈值、直接将当前值与上一个值比较
                ValueChanged = CompareValues(value, _currValue, ValueChangedThreshold,
                    out T
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif

            diff,
                    out int compareResult);
                if (ValueChanged)
                {
                    //更新值
                    PrevValue = _currValue;
                    _currValue = value;
                    ValueChangedEvent?.BeginInvoke(this, new ValueChangedEventArgs<T>(PrevValue, _currValue), null, null);
                }
                //判断变化趋势时，每两次比较之间的间隔不可小于1秒，否则不进行下一步
                var now = DateTime.Now;
                var seconds = (now - _currTime).TotalSeconds;
                if (seconds < 1)
                    return;
                //更新时间
                //_prevTime = _currTime;
                _currTime = now;
                //更新每秒变化值
                _prevValueSecAgo = _currValueSecLater;
                _currValueSecLater = value;
                double scale;
                //尝试比较数值的变化度，假如泛型为非数值类型将报错、并继续向下尝试用CompareTo方法计算变化度，否则直接跳到末尾计算速度部分
                try
                {
                    //scale = (double)(object)_currValueSecLater - (double)(object)_prevValueSecAgo;
                    scale = Convert.ToDouble(_currValueSecLater) - Convert.ToDouble(_prevValueSecAgo);
                    goto END;
                }
                catch (Exception) { }
                ////备用的趋势比较方式，可能泛型不支持比较方法，因此需捕捉可能出现的异常
                //try { CurrentTrend = _currValue.CompareTo(PrevValue); } catch (Exception) { }
                //备用的数值变化度计算方式，使用CompareTo方法
                //scale = _currValueSecLater.CompareTo(_prevValueSecAgo);
                scale = _currValueSecLater == null ? 0 : _currValueSecLater.CompareTo(_prevValueSecAgo);
            END:
                //var seconds = (_currTime - _prevTime).TotalSeconds;
                CurrentSpeed = scale / seconds;
            }
        }

        /// <summary>
        /// 当前值是否有改变
        /// </summary>
        public bool ValueChanged { get; private set; }

        /// <summary>
        /// 上一个不同的变化趋势
        /// </summary>
        public ValueTrend PrevTrend { get; private set; }

        private double _prevSpeed, _currSpeed;
        /// <summary>
        /// 当前速度值（值的变化速度，只有当泛型为数值类型或为可比较类型时有效，单位为xxx每秒）<para/>当速度变化的绝对值不超过阈值时趋势为0，变化绝对值超过阈值且为正值时趋势为1，否则为-1
        /// </summary>
        public double CurrentSpeed
        {
            get { return _currSpeed; }
            private set
            {
                _prevSpeed = _currSpeed;
                _currSpeed = value;
                //判断当前趋势为不变（静止）的条件：之前和当前的速度值均为0，或者仅当前速度值小于速度变化阈值
                //假如当前趋势非静止，根据速度值符号判断趋势为增大或减小
                int trend = (_prevSpeed == 0 && _currSpeed == 0) || Math.Abs(_currSpeed) < SpeedThreshold ? 0 : Math.Sign(_currSpeed);
                CurrentTrend = (ValueTrend)trend;
                ////判断当前趋势为不变（静止）的条件：之前和当前的速度值均为0，或者当前速度值小于速度变化阈值
                //if ((_prevSpeed == 0 && _currSpeed == 0) || Math.Abs(_currSpeed) < SpeedThreshold) CurrentTrend = ValueTrend.Unchanged;
                ////假如当前趋势非静止，根据速度值符号判断趋势为增大或减小
                //else CurrentTrend = _currSpeed > 0 ? 1 : -1;
            }
        }

        /// <summary>
        /// 速度值（值的变化速度）的阈值（绝对值），默认为0，当为非正数时将不对速度值进行任何筛选或限制<para/>当速度变化量的绝对值不超过此值时趋势为0，变化量绝对值超过此值且为正值时趋势为1，否则为-1
        /// </summary>
        public double SpeedThreshold { get; set; }

        private ValueTrend _currTrendCache = ValueTrend.Unchanged, _prevTrendCache = ValueTrend.Unchanged; //当前及之前缓存的变化趋势（-1,0,1）
        private ValueTrend _currTrend = ValueTrend.Unchanged; //当前变化趋势（-1,0,1）
        /// <summary>
        /// 当前变化趋势（-1,0,1）
        /// </summary>
        public ValueTrend CurrentTrend
        {
            get { return _currTrend; }
            private set
            {
                //PrevTrend = _currTrend;
                //_currTrend = value;
                ////假如变化趋势前后有差别则重置变化趋势计数，否则计数+1
                //DiffTicker = !_currTrend.Equals(PrevTrend) ? 0 : DiffTicker + 1;
                ////当变化趋势计数达到临界值时触发事件
                //if (DiffTicker == DiffTickerThreshold)
                //    ValueTrendChanged?.BeginInvoke(this, new ValueTrendChangedEventArgs<T>(PrevValue, _currValue, _currSpeed, _currTrend), null, null);
                _prevTrendCache = _currTrendCache;
                _currTrendCache = value;
                //假如变化趋势前后有差别则重置变化趋势计数，否则计数+1
                DiffTicker = !_currTrendCache.Equals(_prevTrendCache) ? 0 : DiffTicker + 1;
                //当变化趋势计数达到临界值时更新当前趋势
                if (DiffTicker == DiffTickerThreshold)
                {
                    PrevTrend = _currTrend;
                    _currTrend = _currTrendCache;
                    //假如确定下来的趋势有变化，则触发事件
                    if (!PrevTrend.Equals(_currTrend))
                        ValueTrendChanged?.BeginInvoke(this, new ValueTrendChangedEventArgs<T>(PrevValue, _currValue, _currSpeed, _currTrend, PrevTrend), null, null);
                }
            }
        }
        #endregion

        #region 构造器
        /// <summary>
        /// 使用默认的参数初始化，差异计数器阈值为4，速度阈值为0
        /// </summary>
        public ValueDiffStorage() : this(4) { }

        /// <summary>
        /// 使用给定的差异计数器阈值和速度阈值初始化
        /// </summary>
        /// <param name="diffTickerThreshold"></param>
        /// <param name="speedThreshold"></param>
        /// <param name="valueChangedThreshold">量化变化量差异度的阈值，当差异度超过这个值时 <see cref="ValueChanged"/> 属性为true，同时 <see cref="ValueChangedEvent"/> 事件被触发</param>
        public ValueDiffStorage(uint diffTickerThreshold, double speedThreshold = 0,
            T
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            valueChangedThreshold = default)
        {
            DiffTickerThreshold = diffTickerThreshold;
            SpeedThreshold = speedThreshold;
            ValueChangedThreshold = valueChangedThreshold;
        }
        #endregion

        /// <summary>
        /// 设置当前值
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(T value)
        {
            CurrentValue = value;
        }

        //        /// <summary>
        //        /// 比较两个值，并返回变化量的绝对值和比较结果
        //        /// <para/>假如 T 不能转换为 double 或者 T 不能从 double 转换回来，则使用 CompareTo 方法
        //        /// <para/>这里的逻辑假设你需要自己实现从 a 和 b 计算变化量的绝对值的逻辑
        //        /// <para/>因为 IComparable 只提供了比较方法，并没有直接提供数值计算方法
        //        /// </summary>
        //        /// <param name="newValue">变量a</param>
        //        /// <param name="currValue">变量b</param>
        //        /// <param name="diffThres">变化量的阈值，将变化量（变量a与b之差）的绝对值与此阈值进行比较</param>
        //        /// <param name="diffT">（通过参数输出）变化量（变量a与b之差）的绝对值</param>
        //        /// <param name="comparisonResult">（通过参数输出）变化量（的绝对值）与阈值的比较结果，大于0表示变化量大于阈值，小于0表示变化量小于阈值，等于0表示变化量等于阈值</param>
        //        /// <returns>返回一个布尔值，表示进行的比较过程是否成功，假如为false通常代表 T 类型不支持直接的数值转换或减法运算符，需要自定义变化量计算逻辑</returns>
        //#if NET45_OR_GREATER
        //        public bool CompareValues(T newValue, T currValue, T diffThres, out T diffT, out int comparisonResult)
        //#elif NET9_0_OR_GREATER
        //        public bool CompareValues(T? newValue, T? currValue, T? diffThres, out T? diffT, out int comparisonResult)
        //#endif
        //        {
        //            bool success = false;
        //            diffT = default;
        //            comparisonResult = 0;
        //            try
        //            {
        //                // 计算 a 和 b 之间变化量的绝对值
        //                double diff = Math.Abs(Convert.ToDouble(newValue) - Convert.ToDouble(currValue));

        //                // 将 diff 转换回 T 类型以与 变化阈值 进行比较
        //                // 这里假设 T 有一个构造函数接受 double 类型
        //                diffT = (T)Convert.ChangeType(diff, typeof(T));

        //                // 比较 变化量 和 变化阈值
        //                comparisonResult = diffT.CompareTo(diffThres);

        //                //if (comparisonResult > 0)
        //                //    Console.WriteLine("变化量的绝对值大于 阈值");
        //                //else if (comparisonResult < 0)
        //                //    Console.WriteLine("变化量的绝对值小于 阈值");
        //                //else
        //                //    Console.WriteLine("变化量的绝对值等于 阈值");
        //                success = true;
        //            }
        //            //catch (InvalidCastException)
        //            catch (Exception)
        //            {
        //                // 捕捉到类型转换异常，说明 T 不能直接转换为 double 或者 T 不能从 double 转换回来
        //                // 
        //                success = false;
        //                // 如果 T 不能转换为 double 或者 T 不能从 double 转换回来，则使用 CompareTo 方法
        //                // 这里的逻辑假设你需要自己实现从 a 和 b 计算变化量的绝对值的逻辑
        //                // 因为 IComparable 只提供了比较方法，并没有直接提供数值计算方法
        //                //Console.WriteLine("T 类型不支持直接的数值转换或减法运算符，需要自定义变化量计算逻辑");
        //            }
        //            return success;
        //        }

        /// <summary>
        /// 比较两个值，并返回变化量的绝对值和比较结果
        /// <para/>假如 T 不能转换为 double 或者 T 不能从 double 转换回来，则使用 CompareTo 方法
        /// <para/>这里的逻辑假设你需要自己实现从 a 和 b 计算变化量的绝对值的逻辑
        /// <para/>因为 IComparable 只提供了比较方法，并没有直接提供数值计算方法
        /// </summary>
        /// <param name="newValue">变量a</param>
        /// <param name="currValue">变量b</param>
        /// <param name="diffThres">变化量的阈值，将变化量（变量a与b之差）的绝对值与此阈值进行比较</param>
        /// <param name="diffT">（通过参数输出）变化量（变量a与b之差）的绝对值</param>
        /// <param name="comparisonResult">（通过参数输出）变化量（的绝对值）与阈值的比较结果，大于0表示变化量大于阈值，小于0表示变化量小于阈值，等于0表示变化量等于阈值</param>
        /// <returns>返回一个布尔值，表示值是否发生变化</returns>
#if NET45_OR_GREATER
        public bool CompareValues(T newValue, T currValue, T diffThres, out T diffT, out int comparisonResult)
#elif NET9_0_OR_GREATER
        public bool CompareValues(T? newValue, T? currValue, T? diffThres, out T? diffT, out int comparisonResult)
#endif
        {
            bool valueChanged = false;
            diffT = default;
            comparisonResult = 0;
            try
            {
                // 计算 a 和 b 之间变化量的绝对值
                double diff = Math.Abs(Convert.ToDouble(newValue) - Convert.ToDouble(currValue));

                // 将 diff 转换回 T 类型以与 变化阈值 进行比较
                // 这里假设 T 有一个构造函数接受 double 类型
                diffT = (T)Convert.ChangeType(diff, typeof(T));

                // 比较 变化量 和 变化阈值
                comparisonResult = diffT.CompareTo(diffThres);

                valueChanged = comparisonResult > 0;
            }
            //catch (InvalidCastException)
            catch (Exception)
            {
                // 捕捉到类型转换异常，说明 T 不能直接转换为 double 或者 T 不能从 double 转换回来
                // 此时仅比较新值与旧值是否不同来判断值是否改变
                valueChanged = newValue != null && !newValue.Equals(_currValue);
                // 如果 T 不能转换为 double 或者 T 不能从 double 转换回来，则使用 CompareTo 方法
                // 这里的逻辑假设你需要自己实现从 a 和 b 计算变化量的绝对值的逻辑
                // 因为 IComparable 只提供了比较方法，并没有直接提供数值计算方法
                //Console.WriteLine("T 类型不支持直接的数值转换或减法运算符，需要自定义变化量计算逻辑");
            }
            return valueChanged;
        }
    }
}
