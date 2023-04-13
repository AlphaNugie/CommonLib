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
    public partial class FormSineWaves : Form
    {
        public FormSineWaves()
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
            int y0 = Height / 2, halfHeight = y0;
            double angle = 0;
            double angleStep = Math.PI / 50;

            for (int x = 0; x < Width; x++)
            {
                double y = halfHeight - 100 * Math.Sin(angle);
                g.DrawLine(pen, x0, y0, x, (int)y);
                x0 = x;
                y0 = (int)y;
                angle += angleStep;
            }
        }
    }
}
