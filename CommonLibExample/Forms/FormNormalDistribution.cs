using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLibExample.Forms
{
    public partial class FormNormalDistribution : Form
    {
        public FormNormalDistribution()
        {
            InitializeComponent();
            Size = new Size(900, 600);
            //DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Red, 2);
            int x0 = 0;
            int y0 = Height * 3 / 4, halfHeight = y0; //窗口下1/4处作为x轴
            double scale = 400; //缩放倍数
            double mean = 0, stdDev = 0.72; //正态分布的均值与标准差

            for (int x = 0; x < Width; x++)
            {
                //窗口半宽处作为y轴，将像素点缩小到1/400计算，输出放大到400倍作为像素显示，正态分布平均值0，标准差0.68
                double y = halfHeight - MathExtension.NormalDistribution(((double)x - Width / 2) / scale, mean, stdDev) * scale;
                g.DrawLine(pen, x0, y0, x, (int)y);
                x0 = x;
                y0 = (int)y;
            }
        }
    }
}
