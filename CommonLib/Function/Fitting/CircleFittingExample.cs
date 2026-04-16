using CommonLib.Function.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.Fitting
{
    class Program
    {
        static void Main()
        {
            // 生成带噪声的测试数据
            var points = GenerateNoisyArcPoints();

            // 创建RANSAC拟合器
            var fitter = new RansacCircleFitter3D(
                maxIterations: 1000,
                inlierThreshold: 0.1,  // 根据实际噪声水平调整
                confidence: 0.99);

            // 拟合圆弧
            var result = fitter.FitCircleRansac(points);

            if (result != null)
            {
                Console.WriteLine($"拟合成功！");
                Console.WriteLine($"圆心坐标: ({result.Center.X:F3}, {result.Center.Y:F3}, {result.Center.Z:F3})");
                Console.WriteLine($"半径: {result.Radius:F3}");
                Console.WriteLine($"内点数量: {result.InlierCount}/{points.Count()}");
                Console.WriteLine($"法向量: ({result.Normal.X:F3}, {result.Normal.Y:F3}, {result.Normal.Z:F3})");
            }
            else
            {
                Console.WriteLine("拟合失败！");
            }
        }

        static IEnumerable<Point3D> GenerateNoisyArcPoints()
        {
            var points = new List<Point3D>();
            double radius = 2.0;
            var center = new Point3D(1, 2, 3);
            var normal = new Point3D(0.6, 0.8, 0); // 法向量

            var random = new Random();

            for (double angle = 0; angle <= Math.PI * 1.5; angle += Math.PI / 20)
            {
                // 生成圆弧上的点
                double x = center.X + radius * Math.Cos(angle);
                double y = center.Y + radius * Math.Sin(angle);
                double z = center.Z;

                // 添加噪声
                x += (random.NextDouble() - 0.5) * 0.1;
                y += (random.NextDouble() - 0.5) * 0.1;
                z += (random.NextDouble() - 0.5) * 0.1;

                points.Add(new Point3D(x, y, z));
            }

            // 添加一些异常值
            for (int i = 0; i < 5; i++)
            {
                points.Add(new Point3D(
                    center.X + (random.NextDouble() - 0.5) * 5,
                    center.Y + (random.NextDouble() - 0.5) * 5,
                    center.Z + (random.NextDouble() - 0.5) * 5));
            }

            return points;
        }
    }
}
