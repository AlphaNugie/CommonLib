using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 颜色渐变器，具有6个节点，分别是红（RGB 255 0 0），黄（RGB 255 255 0），绿（RGB 0 255 0），青（RGB 0 255 255），蓝（RGB 0 0 255），紫（RGB 255 0 255）
    /// R G B三个通道值随着“距离”变化轮流周期性振动的过程，每两个相邻节点之间的分段长度为256（当实数落在[0,256)区间内时向下取整），6个节点存在5个分段，总距离为256x5=1280
    /// </summary>
    public class ColorSmoother
    {
        //private int _red = 255, _green = 0, _blue = 255;

        /// <summary>
        /// 最低数值，对应紫色，RGB(255, 0, 255)
        /// </summary>
        public double Bottom { get; private set; }

        /// <summary>
        /// 最高数值，对应红色，
        /// </summary>
        public double Top { get; private set; }

        private double _value;
        /// <summary>
        /// 当前数值
        /// </summary>
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Color = GetColor(_value);
            }
        }

        /// <summary>
        /// 当前颜色
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public ColorSmoother() { }

        /// <summary>
        /// 用一个最低数值与最高数值初始化颜色渐变器
        /// </summary>
        /// <param name="bottom">最低数值</param>
        /// <param name="top">最高数值</param>
        public ColorSmoother(double bottom, double top)
        {
            SetBottomTop(bottom, top);
        }

        /// <summary>
        /// 设置极值
        /// </summary>
        /// <param name="bottom">最低数值</param>
        /// <param name="top">最高数值</param>
        public void SetBottomTop(double bottom, double top)
        {
            if (bottom >= top)
                throw new ArgumentException("最低数值应低于最高数值");

            Bottom = bottom;
            Top = top;
        }

        /// <summary>
        /// 根据给定值获取对应颜色
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Color GetColor(double value)
        {
            //假如给定值不在区间范围内，返回透明
            if (!value.Between(Bottom, Top))
                return Color.Transparent;

            int _red = 255, _green = 0, _blue = 255; //R G B的初始设定值（紫色）
            int progInt = (int)Math.Floor((value - Bottom) / (Top - Bottom) * 1280); //按比例计算当前值在起始与终止节点间的距离（0代表紫色节点位置，1280代表红色节点位置），将这个距离向下取整
            int part = Math.DivRem(progInt, 256, out int remained); //距离除以步长256得到所在分段的索引，值范围从0到4（代表5个分段：/紫到蓝/到青/到绿/到黄/到红/），余数为剩余的振动步数
            //从当前值所在分段开始向前数2个分段（索引小于0则忽略），在3个分段内分别计算RGB通道的值
            for (int i = part - 2; i <= part; i++)
            {
                if (i < 0)
                    continue;
                int idx = i % 3; //代表RGB三个通道的索引：0 R，1 G，2 B
                int cycle = Math.DivRem(part - idx, 3, out int left); //已完成的循环次数（每次循环3个通道），余数不为0则代表不是一种通道
                //假如当前循环中的分段通道索引与值所在的分段通道索引不同（共计3个通道），则增加步长为256（因为振动已经完成），否则为剩余振动步数补上已完成的循环周期
                int step = (left > 0 ? 256 : remained) + cycle * 256;
                switch (idx)
                {
                    case 0:
                        _red = GetBounced(_red, step);
                        break;
                    case 1:
                        _green = GetBounced(_green, step);
                        break;
                    case 2:
                        _blue = GetBounced(_blue, step);
                        break;
                }
            }
            return Color.FromArgb(128, _red, _green, _blue);
        }

        /// <summary>
        /// 根据给定的步数返回在振动周期中的位置
        /// </summary>
        /// <param name="basic">基础步数</param>
        /// <param name="step">在基础步数之后再继续振动的步数</param>
        /// <returns></returns>
        private int GetBounced(int basic, int step)
        {
            if (step <= 0)
                return 0;

            int times = Math.DivRem(basic + step, 256, out int remained); //以步数除以256，返回商，并输出余数
            bool even = times % 2 == 0; //商是否为偶数
            int result = (even ? 1 : -1) * remained + (even ? 0 : 255); //假如商为偶数，则为0+余数，否则为255-余数
            return result;
        }
    }
}
