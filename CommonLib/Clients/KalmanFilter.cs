using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 卡尔曼滤波
    /// </summary>
    public class KalmanFilter
    {
        private bool _firstCycle = true; //是否是第一次赋值循环

        /// <summary>
        /// 预测值偏差度
        /// </summary>
        public double Q { get; private set; }

        /// <summary>
        /// 观测值偏差度
        /// </summary>
        public double R { get; private set; }

        /// <summary>
        /// 加速度（默认为0）
        /// </summary>
        public double Acce { get; private set; }

        /// <summary>
        /// 速度（默认为0）
        /// </summary>
        public double Velocity { get; private set; }

        /// <summary>
        /// 估计值相关
        /// </summary>
        public ValueSet Values { get; private set; }

        /// <summary>
        /// 估计协方差相关
        /// </summary>
        public ValueSet Covars { get; private set; }

        /// <summary>
        /// 当前时刻观测值
        /// </summary>
        public double CurrVal { get; private set; }

        /// <summary>
        /// 卡尔曼系数
        /// </summary>
        public double Coeff { get; private set; }

        /// <summary>
        /// 以给定的预测值偏差度、观测值偏差度初始化
        /// </summary>
        /// <param name="Q">预测值偏差度</param>
        /// <param name="R">观测值偏差度</param>
        public KalmanFilter(double Q, double R)
        {
            this.Q = Q;
            this.R = R;
            Values = new ValueSet();
            Covars = new ValueSet();
            _firstCycle = true;
        }

        /// <summary>
        /// 设置当前观测值，同时提供速度，加速度默认为0
        /// </summary>
        /// <param name="value">当前观测值</param>
        /// <param name="v">速度</param>
        public void SetValue(ref double value, double v)
        {
            SetValue(ref value, 0, v);
        }

        /// <summary>
        /// 设置当前观测值，同时提供加速度与速度
        /// </summary>
        /// <param name="value">当前观测值</param>
        /// <param name="a">加速度</param>
        /// <param name="v">速度</param>
        public void SetValue(ref double value, double a, double v)
        {
            CurrVal = value;
            Acce = a;
            Velocity = v;
            //第一次循环给初始值
            if (_firstCycle)
            {
                Values.PrevBest = CurrVal;
                Covars.PrevBest = 0;
                _firstCycle = false;
            }
            Values.InitEval = Values.PrevBest + Velocity;
            Covars.InitEval = Covars.PrevBest + Q;
            Coeff = Covars.InitEval / (Covars.InitEval + R);
            value = Values.BestEval = Values.InitEval + Coeff * (CurrVal - Values.InitEval);
            Covars.BestEval = (1 - Coeff) * Covars.InitEval;
            Values.PrevBest = Values.BestEval;
            Covars.PrevBest = Covars.BestEval;
        }
    }

    /// <summary>
    /// 3种值的组合：上一时刻最优估计，初估计，当前时刻最优估计
    /// </summary>
    public class ValueSet
    {
        /// <summary>
        /// 上一时刻最优估计
        /// </summary>
        public double PrevBest { get; set; }

        /// <summary>
        /// 初估计
        /// </summary>
        public double InitEval { get; set; }

        /// <summary>
        /// 当前时刻最优估计
        /// </summary>
        public double BestEval { get; set; }
    }
}
