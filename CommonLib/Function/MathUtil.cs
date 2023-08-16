using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function
{
    /// <summary>
    /// 数学计算功能类
    /// </summary>
    public static class MathUtil
    {
        /// <summary>
        /// 根据圆上3个点的XY坐标确定圆心的XY坐标和半径长度
        /// </summary>
        /// <param name="x1">第1点的X坐标</param>
        /// <param name="y1">第1点的Y坐标</param>
        /// <param name="x2">第2点的X坐标</param>
        /// <param name="y2">第2点的Y坐标</param>
        /// <param name="x3">第3点的X坐标</param>
        /// <param name="y3">第3点的Y坐标</param>
        /// <param name="centrex">圆心X坐标</param>
        /// <param name="centrey">圆心Y坐标</param>
        /// <param name="radius">圆周半径长度，单位与坐标轴单位相同</param>
        public static void GetCircleNumbers(double x1, double y1, double x2, double y2, double x3, double y3, out double centrex, out double centrey, out double radius)
        {
            double slope1 = (x2 - x1) / (y1 - y2);
            double slope2 = (x3 - x2) / (y2 - y3);

            double mid_x1 = (x1 + x2) / 2;
            double mid_y1 = (y1 + y2) / 2;
            double mid_x2 = (x2 + x3) / 2;
            double mid_y2 = (y2 + y3) / 2;

            centrex = (slope1 * mid_x1 - slope2 * mid_x2 + mid_y2 - mid_y1) / (slope1 - slope2);
            centrey = slope1 * (centrex - mid_x1) + mid_y1;

            radius = Math.Sqrt(Math.Pow(centrex - x1, 2) + Math.Pow(centrey - y1, 2));
        }

        /// <summary>
        /// 使输入的方位角保持在合理的范围之内（-180°到180°之间，包含-180°，不包含180°），假如超过180°则减360°，小于-180°则加360°，循环计算直到进入范围为止
        /// </summary>
        /// <param name="angle">待修改的输入方位角</param>
        public static void KeepAzimuthInRange(ref double angle)
        {
            //假如为180°，修改为-180°，确保同一个位置不会出现2种值
            if (angle == 180)
                angle = -180;
            //假如绝对值大于180，则向当前值符号相反的方向修正360°，直到在范围内为止
            while (Math.Abs(angle) > 180)
                angle -= 360 * Math.Sign(angle);
        }
    }
}
