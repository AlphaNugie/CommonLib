using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 卡尔曼滤波
    /// 卡尔曼滤波器用增益K是在状态预测值和观测误差值之间做了一个折中
    /// 如果K很小，比如等于0，则滤波结果更加接近由系统状态估计值给出的递归结果；如果K很大，比如等于1，则滤波结果更加接近于观测值所反算出来的状态变量
    /// K值的计算可大致示意为公式：K=Q/(Q+R)，因此K值与QR的比值有关系，它们的比值决定了滤波值应该更多来自于系统模型演化的信息，还是来自于观察信号信息
    /// 如果不确切知道Q、R、P0的准确先验信息，应适当增大Q的取值，以增大对实时量测值的利用权重，俗称调谐。但是调谐存在盲目性，无法知道Q要调到多大才行
    /// </summary>
    public class KalmanFilter
    {
        private bool _firstCycle = true; //是否是第一次赋值循环

        /// <summary>
        /// 预测值偏差度（噪声协方差），代表对预测值的置信度，越小跟随性越差（越平缓），可令Q+R=1，此时仅调节Q值就可修改跟随性
        /// </summary>
        public double Q { get; private set; } = 0.3;

        /// <summary>
        /// 观测值偏差度（噪声协方差），代表对测量值的置信度，R越大代表越不相信测量值，可令Q+R=1，此时仅调节Q值就可修改跟随性
        /// </summary>
        public double R { get; private set; } = 0.7;

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
        public KalmanFilter(double Q = 0.3, double R = 0.7)
        {
            this.Q = Q;
            this.R = R;
            Reset();
            //Values = new ValueSet();
            //Covars = new ValueSet();
            //_firstCycle = true;
        }

        ///// <summary>
        ///// 设置当前观测值，同时提供速度，加速度默认为0
        ///// </summary>
        ///// <param name="value">当前观测值</param>
        ///// <param name="v">速度</param>
        //public void SetValue(ref double value, double v)
        //{
        //    SetValue(ref value, 0, v);
        //}

        /// <summary>
        /// 设置当前观测值，同时提供加速度与速度
        /// </summary>
        /// <param name="value">当前观测值</param>
        /// <param name="v">速度</param>
        ///// <param name="a">加速度</param>
        public void SetValue(ref double value, /*double a, */double v = 0)
        {
            //假如输入值不是数字，不予处理
            if (double.IsNaN(value))
                return;

            CurrVal = value;
            //Acce = a;
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
            Values.BestEval = Values.InitEval + Coeff * (CurrVal - Values.InitEval);
            Covars.BestEval = (1 - Coeff) * Covars.InitEval;
            Values.PrevBest = Values.BestEval;
            Covars.PrevBest = Covars.BestEval;
            value = Values.BestEval;

            //假如值与方差的最优估计在处理之后变成非数字，则进行重置
            if (double.IsNaN(Values.PrevBest) || double.IsNaN(Covars.PrevBest))
                Reset();
        }

        /// <summary>
        /// 状态重置
        /// </summary>
        public void Reset()
        {
            Acce = 0;
            Velocity = 0;
            Values = new ValueSet();
            Covars = new ValueSet();
            CurrVal = 0;
            Coeff = 0;
            _firstCycle = true;
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
