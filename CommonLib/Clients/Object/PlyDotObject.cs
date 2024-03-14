using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients.Object
{
    /// <summary>
    /// PLY文件中顶点对象
    /// </summary>
    public class PlyDotObject
    {
        private Color color;
        private byte red = 0, green = 0, blue = 0;
        private string custominfo = string.Empty;

        /// <summary>
        /// X轴坐标，毫米
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y轴坐标，毫米
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Z轴坐标，毫米
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// 定制化信息
        /// </summary>
        public string CustomedInfo
        {
            get { return custominfo; }
            set { custominfo = value == null ? string.Empty : value.Trim(); }
        }

        /// <summary>
        /// RGB颜色
        /// </summary>
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                red = color.R;
                green = color.G;
                blue = color.B;
            }
        }

        /// <summary>
        /// 红
        /// </summary>
        public byte Red
        {
            get { return red; }
            set
            {
                red = value;
                color = Color.FromArgb(red, green, blue);
            }
        }

        /// <summary>
        /// 绿
        /// </summary>
        public byte Green
        {
            get { return green; }
            set
            {
                green = value;
                color = Color.FromArgb(red, green, blue);
            }
        }

        /// <summary>
        /// 蓝
        /// </summary>
        public byte Blue
        {
            get { return blue; }
            set
            {
                blue = value;
                color = Color.FromArgb(red, green, blue);
            }
        }

        /// <summary>
        /// 构造器，指定RGB颜色中各项值
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="y">Y轴坐标</param>
        /// <param name="z">Z轴坐标</param>
        /// <param name="red">红</param>
        /// <param name="green">绿</param>
        /// <param name="blue">蓝</param>
        public PlyDotObject (double x, double y, double z, byte red, byte green, byte blue)
        {
            X = x;
            Y = y;
            Z = z;
            Red = red;
            Green = green;
            Blue = blue;
        }

        /// <summary>
        /// 构造器，指定RGB颜色
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="y">Y轴坐标</param>
        /// <param name="z">Z轴坐标</param>
        /// <param name="color">RGB颜色</param>
        public PlyDotObject(double x, double y, double z, Color color) : this(x, y, z, color.R, color.G, color.B) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="y">Y轴坐标</param>
        /// <param name="z">Z轴坐标</param>
        public PlyDotObject (double x, double y, double z) : this(x, y, z, 0, 0, 0) { }
    }
}
