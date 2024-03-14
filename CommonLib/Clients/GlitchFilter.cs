using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 数据尖刺（突变）过滤器
    /// </summary>
    public class GlitchFilter
    {
        //private readonly GenericStorage<double> _storage;
        //private readonly DataFilterClient _gaussianFilter = new DataFilterClient(5, FilterType.Gaussian, 1.5, false);
        private double _prevp = 0; //上一个处理过的值
        private int _longDistTimer = 0, _gapTimer = 0;

        public GlitchFilterParamModel Params { get; set; }

        //private bool _enabled = false;
        ///// <summary>
        ///// 是否启用过滤器
        ///// </summary>
        //public bool Enabled
        //{
        //    get { return _enabled; }
        //    set
        //    {
        //        _enabled = value;
        //        //假如设为不启用，清空队列内所有对象
        //        if (!_enabled && _storage != null && _storage.Count != 0)
        //            _storage.Clear();
        //    }
        //}

        ///// <summary>
        ///// 储存历史数据的容器
        ///// </summary>
        //public GenericStorage<double> ValueStorage { get { return _storage; } }

        #region 属性
        /// <summary>
        /// 是否启用过滤器
        /// </summary>
        public bool Enabled { get { return Params.Enabled; } }

        /// <summary>
        /// 储存历史数据的容器
        /// </summary>
        public GenericStorage<double> Storage { get { return Params.Storage; } }

        /// <summary>
        /// 当前观测值（observed value）
        /// </summary>
        public double ObsVal { get; private set; }

        /// <summary>
        /// 初步处理值（refined value）
        /// </summary>
        public double RefVal { get; private set; }

        /// <summary>
        /// 当前有效值
        /// </summary>
        public double CurrVal { get; private set; }
        #endregion

        //#region 默认距离检定
        ///// <summary>
        ///// 是否避免达到远距值（较大值）<para/>假如启用，将在达到此远距值时忽略并沿用之前的有效值，除非达到远距值的次数超过了检定限值（LongDistCountLimit），在这种情况下新值将被应用
        ///// </summary>
        //public bool LongDistAvoidEnabled { get; set; }

        ///// <summary>
        ///// 应避免达到的远距值（较大值）<para/>假如启用避免达到远距（LongDistAvoidEnabled）且达到此值，则用上一个输出值代替当前值，除非达到远距值的次数超过了检定限值（LongDistCountLimit），在这种情况下新值将被应用
        ///// </summary>
        //public double LongDist2Avoid { get; set; }

        ///// <summary>
        ///// 测距达到远距值（较大值）的检定次数限定值（超过此次数时新的测距值将被应用）
        ///// </summary>
        //public int LongDistCountLimit { get; set; }
        //#endregion

        //#region 变化平均值检定
        ///// <summary>
        ///// 变化差距平均值过滤是否启用
        ///// </summary>
        //public bool AverageGapEnabled { get; set; }

        //private int _length = 0;
        ///// <summary>
        ///// 存储的历史数值数量，最小长度为2，小于2则自动补正
        ///// </summary>
        //public int StepLength
        //{
        //    get { return _length; }
        //    set
        //    {
        //        _length = value > 1 ? value : 2;
        //        if (_storage != null)
        //            _storage.MaxCapacity = _length;
        //    }
        //}

        ///// <summary>
        ///// 过滤时允许的差异倍数，越小越严格，默认为5
        ///// </summary>
        //public double StepWidth { get; set; }
        //#endregion

        //#region 固定距离检定
        ///// <summary>
        ///// 是否启用固定差距过滤，启用后与上一次相比差异不超过固定差距阈值的将保留
        ///// </summary>
        //public bool FixedGapEnabled { get; set; }

        ///// <summary>
        ///// 固定差距过滤阈值
        ///// </summary>
        //public double FixedGapThres { get; set; }

        ///// <summary>
        ///// 达到固定差距检定次数限定值（超过此次数则以初步处理值替代当前值）
        ///// </summary>
        //public int FixedGapCountLimit { get; set; }
        //#endregion

        /////// <summary>
        /////// 是否使用高斯滤波
        /////// </summary>
        ////public bool GaussianFilterEnabled { get; set; }

        /// <summary>
        /// 用给定的是否启用过滤器标志、历史数据存储数量初始化
        /// </summary>
        /// <param name="enabled">是否启用过滤器</param>
        /// <param name="length">过滤器历史数据长度</param>
        public GlitchFilter(bool enabled, int length) : this(enabled, length, 5) { }

        ///// <summary>
        ///// 用给定的是否启用过滤器标志、历史数据存储数量、过滤差异倍数初始化
        ///// </summary>
        ///// <param name="enabled">是否启用过滤器</param>
        ///// <param name="length">过滤器历史数据长度</param>
        ///// <param name="width">过滤时允许的差异倍数，越小越严格，默认为5</param>
        //public GlitchFilter(bool enabled, int length, double width)
        //{
        //    Enabled = enabled;
        //    StepLength = length;
        //    StepWidth = width;
        //    _storage = new GenericStorage<double>(_length);
        //}

        /// <summary>
        /// 用给定的是否启用过滤器标志、历史数据存储数量、过滤差异倍数初始化
        /// </summary>
        /// <param name="enabled">是否启用过滤器</param>
        /// <param name="length">过滤器历史数据长度</param>
        /// <param name="width">过滤时允许的差异倍数，越小越严格，默认为5</param>
        public GlitchFilter(bool enabled, int length, double width) : this(new GlitchFilterParamModel(enabled, length, null, new AverageGapParamModel(true, width), null)) { }

        public GlitchFilter(GlitchFilterParamModel paramModel)
        {
            Params = paramModel ?? new GlitchFilterParamModel();
        }

        ///// <summary>
        ///// 将新值向队列内压入，假如过滤器未启用，用新值作当前值，否则进行尖刺过滤判断
        ///// </summary>
        ///// <param name="value">新值</param>
        //public void PushValue(ref double value)
        //{
        //    ObsVal = value; //当前观测值
        //    //假如过滤器未启用
        //    if (!Enabled)
        //    {
        //        CurrVal = ObsVal;
        //        return;
        //    }

        //    double temp = RefVal = ObsVal; //用于输出的临时变量，保证value值不变
        //    #region 默认距离检定
        //    //假如避免达到远距值
        //    if (LongDistAvoidEnabled)
        //    {
        //        //bool isLongDist = ObsVal == LongDist2Avoid;
        //        bool isLongDist = ObsVal >= LongDist2Avoid;
        //        _longDistTimer = isLongDist ? _longDistTimer + 1 : 0; //假如当前值与远距默认值相同，则计数器+1，否则重置为0
        //        //只在距离达到远距值的时候处理，假如达到远距值的次数未超过限定值，则以上一个输出值代替观测值与初步处理值，否则将应用打到远距值的新值
        //        if (isLongDist)
        //            temp = RefVal = ObsVal = _longDistTimer <= LongDistCountLimit ? _prevp : ObsVal;
        //        ////假如达到了远距值但次数未超过次数检定限定值，则修改为上一个输出值
        //        //if (isLongDist && _longDistTimer <= LongDistCountLimit)
        //        //    temp = _prevp;
        //    }
        //    #endregion
        //    ////假如使用高斯滤波
        //    //if (GaussianFilterEnabled)
        //    //{
        //    //    List<double> list = _storage.Queue.ToList();
        //    //    list.Add(temp);
        //    //    temp = _gaussianFilter.GetGaussianValue(list);
        //    //}
        //    #region 变化平均值检定
        //    if (AverageGapEnabled && _storage.Count == _storage.MaxCapacity)
        //    {
        //        double diff = RefVal - _prevp, sum = 0; //初步处理值与上一个输出值的差值，队列中所有值差值的和（下一个减上一个）
        //        for (int i = _storage.Count - 1; i >= 1; i--)
        //            sum += Math.Abs(_storage.ElementAt(i) - _storage.ElementAt(i - 1));
        //        //队列中所有值差值的平均值（下一个减上一个）
        //        double aver = sum / (_storage.Count - 1);
        //        //假如此次差值绝对值超过差值平均值，用平均值（在上一个输出值的基础上）修改这次的输出值，否则沿用当前值
        //        if (Math.Abs(diff) > StepWidth * aver)
        //            temp = _prevp + StepWidth * aver * Math.Sign(diff);
        //    }
        //    #endregion
        //    #region 固定距离检定
        //    if (FixedGapEnabled)
        //    {
        //        //double diff = ObsVal - _storage.First(); //当前值与上一个原始值的差值
        //        double diff = RefVal - _prevp; //初步处理值与上一个输出值的差值
        //        int sign = Math.Sign(diff);
        //        //temp = Math.Abs(diff) <= FixedGapThres ? temp : _prevp + FixedGapThres * Math.Sign(diff);
        //        bool isGapReached = Math.Abs(diff) > FixedGapThres; //变化差值的绝对值是否超过固定差距阈值
        //        //_gapTimer = isGapReached ? _gapTimer + sign : 0; //假如达到阈值则计数器±1（取决于变化差值的符号）
        //        _gapTimer = !isGapReached || _gapTimer * sign < 0 ? 0 : _gapTimer + sign; //假如未达到阈值，或新变化值与累计趋势相反，计数置零，否则计数器±1（取决于变化差值的符号）
        //        //假如变化差距达到阈值
        //        if (isGapReached)
        //        {
        //            //维持增加趋势或维持减小趋势的次数未超过限定值，则以上一次输出值增加或减少固定阈值来作为当前输出值
        //            if (Math.Abs(_gapTimer) <= FixedGapCountLimit)
        //                temp = _prevp + FixedGapThres * sign;
        //            //否则用初步处理值作为当前输出值，计数置零
        //            else
        //            {
        //                temp = RefVal;
        //                _gapTimer = 0;
        //            }
        //            ////维持增加趋势或维持减小趋势的次数未超过限定值，则以上一次输出值增加或减少固定阈值来作为当前输出值，否则用初步处理值作为当前输出值
        //            //temp = Math.Abs(_gapTimer) <= FixedGapCountLimit ? _prevp + FixedGapThres * sign : RefVal;
        //            //_gapTimer = 0;
        //        }
        //        //if (Math.Abs(diff) > FixedGapThres)
        //        //    temp = _prevp + FixedGapThres * Math.Sign(diff);
        //    }
        //    #endregion
        //    //value = CurrVal = temp;
        //    //_prevp = CurrVal;
        //    _prevp = value = CurrVal = temp;
        //    _storage.Push(ObsVal);
        //}

        /// <summary>
        /// 将新值向队列内压入，假如过滤器未启用，用新值作当前值，否则进行尖刺过滤判断
        /// </summary>
        /// <param name="value">新值</param>
        public void PushValue(ref double value)
        {
            ObsVal = value; //当前观测值
            //假如过滤器未启用
            if (!Enabled)
            {
                CurrVal = ObsVal;
                return;
            }

            double temp = RefVal = ObsVal; //用于输出的临时变量，保证value值不变
            var longDistParams = Params.LongDistAvoidParams;
            var averageGapParams = Params.AverageGapParams;
            var fixedGapParams = Params.FixedGapParams;
            #region 默认距离检定
            //假如避免达到远距值
            if (longDistParams.Enabled)
            {
                //bool isLongDist = ObsVal == LongDist2Avoid;
                bool isLongDist = Math.Abs(ObsVal) >= longDistParams.Threshold;
                _longDistTimer = isLongDist ? _longDistTimer + 1 : 0; //假如当前值与远距默认值相同，则计数器+1，否则重置为0
                //只在距离达到远距值的时候处理，假如达到远距值的次数未超过限定值，则以上一个输出值代替观测值与初步处理值，否则将应用打到远距值的新值
                if (isLongDist)
                    temp = RefVal = ObsVal = _longDistTimer <= longDistParams.CountLimit ? _prevp : ObsVal;
                ////假如达到了远距值但次数未超过次数检定限定值，则修改为上一个输出值
                //if (isLongDist && _longDistTimer <= LongDistCountLimit)
                //    temp = _prevp;
            }
            #endregion
            ////假如使用高斯滤波
            //if (GaussianFilterEnabled)
            //{
            //    List<double> list = _storage.Queue.ToList();
            //    list.Add(temp);
            //    temp = _gaussianFilter.GetGaussianValue(list);
            //}
            #region 变化平均值检定
            if (averageGapParams.Enabled && Storage.Count == Storage.MaxCapacity)
            {
                double diff = RefVal - _prevp, sum = 0; //初步处理值与上一个输出值的差值，队列中所有值差值的和（下一个减上一个）
                for (int i = Storage.Count - 1; i >= 1; i--)
                    sum += Math.Abs(Storage.ElementAt(i) - Storage.ElementAt(i - 1));
                //队列中所有值差值的平均值（下一个减上一个）
                double aver = sum / (Storage.Count - 1);
                //假如此次差值绝对值超过差值平均值，用平均值（在上一个输出值的基础上）修改这次的输出值，否则沿用当前值
                if (Math.Abs(diff) > averageGapParams.StepWidth * aver)
                    temp = _prevp + averageGapParams.StepWidth * aver * Math.Sign(diff);
            }
            #endregion
            #region 固定距离检定
            if (fixedGapParams.Enabled)
            {
                //double diff = ObsVal - _storage.First(); //当前值与上一个原始值的差值
                double diff = RefVal - _prevp; //初步处理值与上一个输出值的差值
                int sign = Math.Sign(diff);
                //temp = Math.Abs(diff) <= FixedGapThres ? temp : _prevp + FixedGapThres * Math.Sign(diff);
                bool isGapReached = Math.Abs(diff) > fixedGapParams.Threshold; //变化差值的绝对值是否超过固定差距阈值
                //_gapTimer = isGapReached ? _gapTimer + sign : 0; //假如达到阈值则计数器±1（取决于变化差值的符号）
                _gapTimer = !isGapReached || _gapTimer * sign < 0 ? 0 : _gapTimer + sign; //假如未达到阈值，或新变化值与累计趋势相反，计数置零，否则计数器±1（取决于变化差值的符号）
                //假如变化差距达到阈值
                if (isGapReached)
                {
                    //维持增加趋势或维持减小趋势的次数未超过限定值，则以上一次输出值增加或减少固定阈值来作为当前输出值
                    if (Math.Abs(_gapTimer) <= fixedGapParams.CountLimit)
                        temp = _prevp + fixedGapParams.Threshold * sign;
                    //否则用初步处理值作为当前输出值，计数置零
                    else
                    {
                        temp = RefVal;
                        _gapTimer = 0;
                    }
                    ////维持增加趋势或维持减小趋势的次数未超过限定值，则以上一次输出值增加或减少固定阈值来作为当前输出值，否则用初步处理值作为当前输出值
                    //temp = Math.Abs(_gapTimer) <= FixedGapCountLimit ? _prevp + FixedGapThres * sign : RefVal;
                    //_gapTimer = 0;
                }
                //if (Math.Abs(diff) > FixedGapThres)
                //    temp = _prevp + FixedGapThres * Math.Sign(diff);
            }
            #endregion
            //value = CurrVal = temp;
            //_prevp = CurrVal;
            _prevp = value = CurrVal = temp;
            Storage.Push(ObsVal);
        }

        /// <summary>
        /// 将新值向队列内压入，假如过滤器未启用，用新值作当前值，否则进行尖刺过滤判断
        /// </summary>
        /// <param name="value">新值</param>
        public void PushValue(double value)
        {
            double temp = value;
            PushValue(ref temp);
        }
    }

    /// <summary>
    /// 数据尖刺（突变）过滤器的参数实体类
    /// </summary>
    public class GlitchFilterParamModel
    {
        private readonly GenericStorage<double> _storage;

        private bool _enabled = false;
        /// <summary>
        /// 是否启用突变过滤器
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                //假如设为不启用，清空队列内所有对象
                if (!_enabled && _storage != null && _storage.Count != 0)
                    _storage.Clear();
            }
        }

        /// <summary>
        /// 储存历史数据的容器
        /// </summary>
        public GenericStorage<double> Storage { get { return _storage; } }

        private int _capacity = 5;
        /// <summary>
        /// 存储的历史数值容量，最小长度为2，小于2则自动补正（默认为5）
        /// </summary>
        public int StorageCapacity
        {
            get { return _capacity; }
            set
            {
                _capacity = value > 1 ? value : 2;
                if (_storage != null)
                    _storage.MaxCapacity = _capacity;
            }
        }

        /// <summary>
        /// 远距值（较大值）过滤参数
        /// </summary>
        public LongDistAvoidParamModel LongDistAvoidParams { get; set; }

        /// <summary>
        /// 变化平均值过滤参数
        /// </summary>
        public AverageGapParamModel AverageGapParams { get; set; }

        /// <summary>
        /// 固定差距过滤参数
        /// </summary>
        public FixedGapParamModel FixedGapParams { get; set; }

        /// <summary>
        /// 用给定参数初始化
        /// </summary>
        /// <param name="enabled">是否启用突变过滤器</param>
        /// <param name="capacity">存储的历史数值容量，最小长度为2</param>
        /// <param name="longDistAvoidParams">远距值（较大值）过滤参数的实体类对象</param>
        /// <param name="averageGapParams">变化平均值过滤参数的实体类对象</param>
        /// <param name="fixedGapParams">固定差距过滤参数的实体类对象</param>
        public GlitchFilterParamModel(bool enabled = false, int capacity = 5, LongDistAvoidParamModel longDistAvoidParams = null, AverageGapParamModel averageGapParams = null, FixedGapParamModel fixedGapParams = null)
        {
            Enabled = enabled;
            StorageCapacity = capacity;
            LongDistAvoidParams = longDistAvoidParams ?? new LongDistAvoidParamModel();
            AverageGapParams = averageGapParams ?? new AverageGapParamModel();
            FixedGapParams = fixedGapParams ?? new FixedGapParamModel();
            _storage = new GenericStorage<double>(_capacity);
        }
    }

    /// <summary>
    /// 避免输入值（的绝对值）达到远距值（较大值）的配置参数实体类，用于设置被赋予较大值时的更新方式
    /// </summary>
    public class LongDistAvoidParamModel
    {
        /// <summary>
        /// 是否避免达到远距值（较大值）<para/>假如启用，将在输入值（的绝对值）达到此远距值时忽略并沿用之前的有效值，除非达到远距值的次数超过了检定限值（CountLimit），在这种情况下新值将被应用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 应避免输入值（的绝对值）达到的远距值（较大值）<para/>假如启用避免达到远距（Enabled）且达到此值，则用上一个输出值代替当前值，除非达到远距值的次数超过了检定限值（CountLimit），在这种情况下新值将被应用
        /// </summary>
        public double Threshold { get; set; }

        /// <summary>
        /// 测距达到远距值（较大值）的检定次数限定值，超过此次数时新的测距值将被应用
        /// </summary>
        public int CountLimit { get; set; }

        /// <summary>
        /// 用给定参数初始化
        /// </summary>
        /// <param name="enabled">是否避免达到远距值（较大值）<para/>假如启用，将在达到此远距值时忽略并沿用之前的有效值，除非达到远距值的次数超过了检定限值（CountLimit），在这种情况下新值将被应用</param>
        /// <param name="threshold">应避免达到的远距值（较大值）<para/>假如启用避免达到远距（Enabled）且达到此值，则用上一个输出值代替当前值，除非达到远距值的次数超过了检定限值（CountLimit），在这种情况下新值将被应用</param>
        /// <param name="countLimit">测距达到远距值（较大值）的检定次数限定值（超过此次数时新的测距值将被应用）</param>
        public LongDistAvoidParamModel(bool enabled = false, double threshold = 0, int countLimit = 0)
        {
            Enabled = enabled;
            Threshold = threshold;
            CountLimit = countLimit;
        }
    }

    /// <summary>
    /// 变化平均值检定的配置参数实体类，用于设置当新值变化量大于历史值变化量时的更新方式
    /// </summary>
    public class AverageGapParamModel
    {
        /// <summary>
        /// 变化差距平均值过滤是否启用<para/>启用后当新值减最新输出值的变化量大于历史变化量时，新值的变化量将被抑制到历史变化量的范围内
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 过滤时允许的差异倍数，越小越严格，默认为5<para/>计算历史数据内每2个相邻值的差（绝对值）并求均值，假如新值与最新输出值相减取绝对值后的结果大于差异倍数乘以此均值，则用均值代替新值的变化量，否则新值将被应用
        /// </summary>
        public double StepWidth { get; set; }

        /// <summary>
        /// 用给定参数初始化
        /// </summary>
        /// <param name="enabled">变化差距平均值过滤是否启用，启用后当新值减最新输出值的变化量大于历史变化量时，新值的变化量将被抑制到历史变化量的范围内</param>
        /// <param name="stepWidth">过滤时允许的差异倍数，越小越严格，默认为5<para/>计算历史数据内每2个相邻值的差（绝对值）并求均值，假如新值与最新输出值相减取绝对值后的结果大于差异倍数乘以此均值，则用均值代替新值的变化量，否则新值将被应用</param>
        public AverageGapParamModel(bool enabled = false, /*int stepLength = 5, */double stepWidth = 5)
        {
            Enabled = enabled;
            //StepLength = stepLength;
            StepWidth = stepWidth;
        }
    }

    /// <summary>
    /// 固定差距检定的配置参数实体类，用于设置当新值变化量大于某固定值时的更新方式
    /// </summary>
    public class FixedGapParamModel
    {
        /// <summary>
        /// 是否启用固定差距过滤<para/>启用后与上一次相比差异不超过固定差距阈值的将保留
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 固定差距过滤阈值<para/>假如新值变化量超过此阈值并达到一定次数，则新值将被应用，否则将用最新输出值加上阈值作为新一轮的输出值
        /// </summary>
        public double Threshold { get; set; }

        /// <summary>
        /// 达到固定差距检定次数限定值<para/>超过此次数则以初步处理值（新值）替代当前值，否则将用最新输出值加上阈值作为新一轮的输出值
        /// </summary>
        public int CountLimit { get; set; }

        /// <summary>
        /// 用给定参数初始化
        /// </summary>
        /// <param name="enabled">是否启用固定差距过滤<para/>启用后与上一次相比差异不超过固定差距阈值的将保留</param>
        /// <param name="threshold">固定差距过滤阈值<para/>假如新值变化量超过此阈值并达到一定次数，则新值将被应用，否则将用最新输出值加上阈值作为新一轮的输出值</param>
        /// <param name="countLimit">达到固定差距检定次数限定值<para/>超过此次数则以初步处理值（新值）替代当前值，否则将用最新输出值加上阈值作为新一轮的输出值</param>
        public FixedGapParamModel(bool enabled = false, double threshold = 0, int countLimit = 0)
        {
            Enabled = enabled;
            Threshold = threshold;
            CountLimit = countLimit;
        }
    }
}
