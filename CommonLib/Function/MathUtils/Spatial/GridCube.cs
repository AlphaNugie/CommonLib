using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.MathUtils.Spatial
{
    /// <summary>
    /// 空间中网格状排列的立方体对象，每个立方体中储存一些三维空间坐标点对象（数量有固定上限），提供计算平均点的方法
    /// </summary>
    public class GridCube
    {
        /// <summary>
        /// 立方体内储存的点的数量上限
        /// </summary>
        public const int MaxPoints = 30;

        /// <summary>
        /// 储存空间点对象的列表，存在储存上限，假如超过上限则先将旧点排出
        /// </summary>
        public List<Point3D> Points { get; private set; } = new List<Point3D>();

        /// <summary>
        /// 清除内部所有坐标点
        /// </summary>
        public void Clear() { Points.Clear(); }

        /// <summary>
        /// 添加新点，假如已包含此点则不继续进行（各属性相同），否则继续；假如超过上限则先将旧点排出
        /// </summary>
        /// <param name="point"></param>
        public void AddPoint(Point3D point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point), "提供的坐标点对象为空引用");
            if (Points.Contains(point))
                return;
            //假如数量达到或超过上限，则删除最早添加的点
            while (Points.Count >= MaxPoints)
                Points.RemoveAt(0);
            Points.Add(point);
        }

        /// <summary>
        /// 用所有点的坐标均值返回一个新坐标对象
        /// </summary>
        /// <returns></returns>
        public Point3D GetAveragePoint()
        {
            double avgX = Points.Average(p => p.X),
                avgY = Points.Average(p => p.Y),
                avgZ = Points.Average(p => p.Z),
                avgRfl = Points.Average(p => p.Reflectivity);
            return new Point3D(avgX, avgY, avgZ, avgRfl);
        }
    }
}
