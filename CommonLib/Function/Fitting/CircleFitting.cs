using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Function.MathUtils;

namespace CommonLib.Function.Fitting
{
    /// <summary>
    /// 圆弧拟合结果类，包含圆心、半径、法向量等拟合信息
    /// </summary>
    public class CircleFitResult
    {
        /// <summary>拟合得到的圆心三维坐标</summary>
        public Point3D Center { get; set; }

        /// <summary>拟合得到的圆弧半径</summary>
        public double Radius { get; set; }

        /// <summary>圆弧所在平面的法向量</summary>
        public Point3D Normal { get; set; }

        /// <summary>内点数量（符合拟合模型的点数）</summary>
        public int InlierCount { get; set; }

        /// <summary>拟合误差</summary>
        public double Error { get; set; }
    }

    /// <summary>
    /// 基于RANSAC的三维圆弧拟合器，能够处理带噪声的数据
    /// RANSAC（Random Sample Consensus）是一种鲁棒的参数估计方法
    /// </summary>
    public class RansacCircleFitter3D
    {
        // RANSAC算法参数
        private int _maxIterations;                 // 最大迭代次数
        private readonly double _inlierThreshold;   // 内点距离阈值
        private readonly double _confidence;        // 置信度
        private readonly Random _random;            // 随机数生成器

        /// <summary>
        /// 构造函数，初始化RANSAC参数
        /// </summary>
        /// <param name="maxIterations">最大迭代次数，默认1000</param>
        /// <param name="inlierThreshold">内点距离阈值，默认0.01</param>
        /// <param name="confidence">置信度，默认0.99</param>
        public RansacCircleFitter3D(int maxIterations = 1000,
                                   double inlierThreshold = 0.01,
                                   double confidence = 0.99)
        {
            _maxIterations = maxIterations;
            _inlierThreshold = inlierThreshold;
            _confidence = confidence;
            _random = new Random();
        }

        /// <summary>
        /// 使用RANSAC算法拟合三维圆弧
        /// </summary>
        /// <param name="points">三维点集合，包含可能的噪声点</param>
        /// <returns>拟合结果，包含圆心、半径、法向量等信息</returns>
        /// <exception cref="ArgumentException">当点数不足3个时抛出异常</exception>
        public CircleFitResult FitCircleRansac(IEnumerable<Point3D> points)
        {
            // 将输入点转换为列表以便处理
            var pointList = points.ToList();

            // 验证输入数据有效性
            if (pointList.Count < 3)
                throw new ArgumentException("至少需要3个点来拟合圆");

            CircleFitResult bestResult = null;  // 最佳拟合结果
            int bestInlierCount = 0;            // 最佳内点数量

            // RANSAC主循环：多次迭代寻找最佳模型
            for (int i = 0; i < _maxIterations; i++)
            {
                // 1. 随机选择3个点（最小样本集）
                var samplePoints = GetRandomSample(pointList, 3);

                // 2. 用这3个点拟合初始圆模型
                var circle = FitCircleFromThreePoints(samplePoints);
                if (circle == null) continue;  // 如果拟合失败则跳过本次迭代

                // 3. 找出内点（符合当前圆模型的点）
                var inliers = FindInliers(pointList, circle);

                // 4. 如果内点数量足够多，用所有内点重新拟合圆
                if (inliers.Count >= pointList.Count * 0.3)  // 至少30%的点是内点
                {
                    // 使用所有内点进行精确拟合
                    var refinedCircle = RefineCircleWithAllPoints(inliers);

                    // 检查拟合是否成功
                    if (refinedCircle != null)
                    {
                        int inlierCount = CountInliers(pointList, refinedCircle);

                        // 5. 更新最佳模型
                        if (inlierCount > bestInlierCount)
                        {
                            bestInlierCount = inlierCount;
                            bestResult = refinedCircle;
                            bestResult.InlierCount = inlierCount;

                            // 动态调整迭代次数以提高效率
                            _maxIterations = Math.Min(_maxIterations,
                                CalculateOptimalIterations(inlierCount, pointList.Count, 3));
                        }
                    }
                }
            }

            // 如果RANSAC未找到合适模型，则使用所有点进行拟合作为备选方案
            var fallbackResult = RefineCircleWithAllPoints(pointList);
            return bestResult ?? fallbackResult;
        }

        /// <summary>
        /// 从点集中随机抽取指定数量的样本点
        /// </summary>
        /// <param name="points">原始点集</param>
        /// <param name="sampleSize">需要抽取的样本数量</param>
        /// <returns>随机抽取的样本点列表</returns>
        private List<Point3D> GetRandomSample(List<Point3D> points, int sampleSize)
        {
            // 使用随机排序后取前N个点的方法实现随机抽样
            return points.OrderBy(x => _random.Next()).Take(sampleSize).ToList();
        }

        /// <summary>
        /// 根据三个点拟合圆弧（三点确定一个圆）
        /// </summary>
        /// <param name="points">包含三个Point3D的列表</param>
        /// <returns>拟合的圆模型，如果三点共线则返回null</returns>
        private CircleFitResult FitCircleFromThreePoints(List<Point3D> points)
        {
            if (points.Count != 3) return null;

            try
            {
                // 计算三点形成的两个向量
                var vector1 = new Point3D(
                    points[1].X - points[0].X,  // 点1到点0的X分量
                    points[1].Y - points[0].Y,  // 点1到点0的Y分量  
                    points[1].Z - points[0].Z); // 点1到点0的Z分量

                var vector2 = new Point3D(
                    points[2].X - points[0].X,  // 点2到点0的X分量
                    points[2].Y - points[0].Y,  // 点2到点0的Y分量
                    points[2].Z - points[0].Z); // 点2到点0的Z分量

                // 计算法向量（通过叉积得到平面的法向量）
                var normal = new Point3D(
                    vector1.Y * vector2.Z - vector1.Z * vector2.Y,  // 叉积的X分量
                    vector1.Z * vector2.X - vector1.X * vector2.Z,  // 叉积的Y分量
                    vector1.X * vector2.Y - vector1.Y * vector2.X); // 叉积的Z分量

                // 计算法向量长度并归一化
                double normalLength = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
                if (normalLength < 1e-10) return null; // 三点共线，无法确定平面

                normal = new Point3D(normal.X / normalLength, normal.Y / normalLength, normal.Z / normalLength);

                // 建立局部坐标系以便进行二维圆拟合
                var pp = CreateLocalCoordinateSystem(normal);
                var uAxis = pp.UAxis;
                var vAxis = pp.VAxis;

                // 将三维点投影到二维平面
                var projectedPoints = ProjectPointsToPlane(points, points[0], uAxis, vAxis);

                // 在二维平面中拟合圆
                var circle2D = FitCircle2D(projectedPoints);

                // 检查二维圆拟合是否成功
                if (circle2D == null || double.IsNaN(circle2D.Radius) || circle2D.Radius <= 0)
                {
                    return null; // 圆半径无效，无法拟合
                }

                // 检查半径是否合理（避免三点共线导致的过大半径）
                const double maxReasonableRadius = 1e8;
                if (circle2D.Radius > maxReasonableRadius)
                {
                    return null; // 半径过大，可能是共线点
                }

                // 将二维圆心坐标转换回三维空间坐标
                var center3D = new Point3D(
                    points[0].X + circle2D.Center.X * uAxis.X + circle2D.Center.Y * vAxis.X,  // X坐标转换
                    points[0].Y + circle2D.Center.X * uAxis.Y + circle2D.Center.Y * vAxis.Y,  // Y坐标转换
                    points[0].Z + circle2D.Center.X * uAxis.Z + circle2D.Center.Y * vAxis.Z); // Z坐标转换

                return new CircleFitResult
                {
                    Center = center3D,
                    Radius = circle2D.Radius,
                    Normal = normal,
                    InlierCount = 3  // 初始模型只有3个内点
                };
            }
            catch
            {
                return null;  // 拟合过程中发生错误
            }
        }

        /// <summary>
        /// 创建局部坐标系，用于将三维点投影到二维平面
        /// </summary>
        /// <param name="normal">平面法向量</param>
        /// <returns>局部坐标系的u轴和v轴</returns>
        //private (Point3D uAxis, Point3D vAxis) CreateLocalCoordinateSystem(Point3D normal)
        private AxisPair CreateLocalCoordinateSystem(Point3D normal)
        {
            // 选择与法向量不平行的参考向量
            Point3D reference;
            if (Math.Abs(normal.X) > 0.9)  // 如果法向量主要指向X方向
                reference = new Point3D(0, 1, 0);  // 使用Y轴作为参考
            else
                reference = new Point3D(1, 0, 0);  // 使用X轴作为参考

            // 计算u轴（与法向量垂直）
            var uAxis = new Point3D(
                reference.Y * normal.Z - reference.Z * normal.Y,  // 叉积X分量
                reference.Z * normal.X - reference.X * normal.Z,  // 叉积Y分量
                reference.X * normal.Y - reference.Y * normal.X); // 叉积Z分量

            // 归一化u轴
            double uLength = Math.Sqrt(uAxis.X * uAxis.X + uAxis.Y * uAxis.Y + uAxis.Z * uAxis.Z);
            if (uLength < 1e-10)  // 如果参考向量与法向量平行
            {
                uAxis = new Point3D(0, 1, 0);  // 使用默认的Y轴方向
                uLength = 1;
            }

            uAxis = new Point3D(uAxis.X / uLength, uAxis.Y / uLength, uAxis.Z / uLength);

            // 计算v轴（与u轴和法向量都垂直）
            var vAxis = new Point3D(
                normal.Y * uAxis.Z - normal.Z * uAxis.Y,  // 法向量与u轴的叉积
                normal.Z * uAxis.X - normal.X * uAxis.Z,
                normal.X * uAxis.Y - normal.Y * uAxis.X);

            //return (uAxis, vAxis);
            return new AxisPair(uAxis, vAxis);
        }

        /// <summary>
        /// 将三维点投影到二维平面
        /// </summary>
        /// <param name="points">待投影的三维点集</param>
        /// <param name="planePoint">平面上的一个参考点</param>
        /// <param name="uAxis">局部坐标系的u轴</param>
        /// <param name="vAxis">局部坐标系的v轴</param>
        /// <returns>投影后的二维点坐标列表</returns>
        //private List<(double X, double Y)> ProjectPointsToPlane(List<Point3D> points, Point3D planePoint, Point3D uAxis, Point3D vAxis)
        private List<DoublePair> ProjectPointsToPlane(List<Point3D> points, Point3D planePoint, Point3D uAxis, Point3D vAxis)
        {
            //var result = new List<(double, double)>();
            var result = new List<DoublePair>();

            foreach (var point in points)
            {
                // 计算点相对于参考点的偏移量
                var relative = new Point3D(
                    point.X - planePoint.X,  // X方向偏移
                    point.Y - planePoint.Y,  // Y方向偏移
                    point.Z - planePoint.Z); // Z方向偏移

                // 计算点在局部坐标系中的坐标（点积投影）
                double u = relative.X * uAxis.X + relative.Y * uAxis.Y + relative.Z * uAxis.Z;  // u坐标
                double v = relative.X * vAxis.X + relative.Y * vAxis.Y + relative.Z * vAxis.Z;  // v坐标

                //result.Add((u, v));
                result.Add(new DoublePair(u, v));
            }

            return result;
        }

        /// <summary>
        /// 使用最小二乘法拟合二维圆
        /// </summary>
        /// <param name="points">二维点集</param>
        /// <returns>拟合的二维圆模型</returns>
        //private CircleFitResult FitCircle2D(List<(double X, double Y)> points)
        private CircleFitResult FitCircle2D(List<DoublePair> points)
        {
            int n = points.Count;  // 点数

            // 初始化各种统计量
            double sumX = 0, sumY = 0, sumX2 = 0, sumY2 = 0, sumX3 = 0, sumY3 = 0;
            double sumXY = 0, sumX2Y = 0, sumXY2 = 0;

            // 计算各项统计量
            //foreach (var (x, y) in points)
            foreach (var dp in points)
            {
                var x = dp.A;
                var y = dp.B;
                double x2 = x * x;    // x的平方
                double y2 = y * y;    // y的平方
                double x3 = x2 * x;   // x的立方
                double y3 = y2 * y;   // y的立方
                double xy = x * y;    // x和y的乘积
                double x2y = x2 * y;  // x²y
                double xy2 = x * y2;  // xy²

                // 累加各统计量
                sumX += x;
                sumY += y;
                sumX2 += x2;
                sumY2 += y2;
                sumX3 += x3;
                sumY3 += y3;
                sumXY += xy;
                sumX2Y += x2y;
                sumXY2 += xy2;
            }

            // 构建线性方程组的系数矩阵A和常数向量B
            // 方程组形式：A * [a; b; c] = B
            double[,] A = {
            { sumX2, sumXY, sumX },
            { sumXY, sumY2, sumY },
            { sumX, sumY, n }
        };

            double[] B = {
            -sumX3 - sumXY2,    // 方程1的常数项
            -sumX2Y - sumY3,     // 方程2的常数项
            -sumX2 - sumY2       // 方程3的常数项
        };

            // 解线性方程组
            var solution = SolveLinearSystem(A, B);
            double a = solution[0], b = solution[1], c = solution[2];

            // 计算圆心坐标和半径
            double centerX = -a / 2;      // 圆心X坐标
            double centerY = -b / 2;      // 圆心Y坐标
            double radiusSquared = centerX * centerX + centerY * centerY - c;  // 半径平方

            // 检查半径平方是否有效
            if (radiusSquared < 0 || double.IsNaN(radiusSquared))
            {
                // 点共线或退化情况，无法拟合圆
                return null;
            }

            double radius = Math.Sqrt(radiusSquared);  // 圆半径

            // 检查半径是否合理（避免过大的半径）
            const double maxValidRadius = 1e10;
            if (radius > maxValidRadius || double.IsInfinity(radius))
            {
                return null;
            }

            return new CircleFitResult
            {
                Center = new Point3D(centerX, centerY, 0),  // 二维圆心（Z=0）
                Radius = radius
            };
        }

        /// <summary>
        /// 根据当前圆模型找出内点
        /// </summary>
        /// <param name="points">所有待检测的点</param>
        /// <param name="circle">当前圆模型</param>
        /// <returns>符合圆模型的点列表</returns>
        private List<Point3D> FindInliers(List<Point3D> points, CircleFitResult circle)
        {
            var inliers = new List<Point3D>();

            foreach (var point in points)
            {
                // 计算点到圆的距离
                double distance = PointToCircleDistance(point, circle);
                // 如果距离小于阈值，则认为是内点
                if (distance < _inlierThreshold)
                {
                    inliers.Add(point);
                }
            }

            return inliers;
        }

        /// <summary>
        /// 统计符合圆模型的点数量
        /// </summary>
        /// <param name="points">所有待统计的点</param>
        /// <param name="circle">圆模型</param>
        /// <returns>内点数量</returns>
        private int CountInliers(List<Point3D> points, CircleFitResult circle)
        {
            int count = 0;

            foreach (var point in points)
            {
                double distance = PointToCircleDistance(point, circle);
                if (distance < _inlierThreshold)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 计算三维点到圆弧的距离
        /// </summary>
        /// <param name="point">待计算的点</param>
        /// <param name="circle">圆模型</param>
        /// <returns>点到圆的距离</returns>
        private double PointToCircleDistance(Point3D point, CircleFitResult circle)
        {
            // 计算点到圆心的向量
            var toCenter = new Point3D(
                point.X - circle.Center.X,  // X方向分量
                point.Y - circle.Center.Y,  // Y方向分量
                point.Z - circle.Center.Z); // Z方向分量

            // 计算点到圆心的欧氏距离
            double distanceToCenter = Math.Sqrt(
                toCenter.X * toCenter.X + toCenter.Y * toCenter.Y + toCenter.Z * toCenter.Z);

            // 计算点到圆平面的距离（法向量方向投影）
            double planeDistance = Math.Abs(
                toCenter.X * circle.Normal.X +
                toCenter.Y * circle.Normal.Y +
                toCenter.Z * circle.Normal.Z);

            // 计算点在圆平面上的投影到圆心的距离
            double inPlaneDistance = Math.Sqrt(Math.Max(0, distanceToCenter * distanceToCenter - planeDistance * planeDistance));

            // 点到圆的距离是平面内距离与半径差值的绝对值
            return Math.Abs(inPlaneDistance - circle.Radius);
        }

        /// <summary>
        /// 使用所有内点重新拟合更精确的圆模型
        /// </summary>
        /// <param name="inliers">内点集合</param>
        /// <returns>精确拟合的圆模型</returns>
        private CircleFitResult RefineCircleWithAllPoints(List<Point3D> inliers)
        {
            if (inliers.Count < 3) return null;

            try
            {
                // 1. 拟合内点所在的平面
                var plane = FitPlane(inliers);

                // 2. 建立局部坐标系
                //var (uAxis, vAxis) = CreateLocalCoordinateSystem(plane.Normal);
                var pp = CreateLocalCoordinateSystem(plane.Normal);
                var uAxis = pp.UAxis;
                var vAxis = pp.VAxis;

                // 3. 将内点投影到二维平面
                var projectedPoints = ProjectPointsToPlane(inliers, plane.Center, uAxis, vAxis);

                // 4. 在二维平面中拟合圆
                var circle2D = FitCircle2D(projectedPoints);

                // 5. 将二维圆转换回三维空间
                var center3D = new Point3D(
                    plane.Center.X + circle2D.Center.X * uAxis.X + circle2D.Center.Y * vAxis.X,
                    plane.Center.Y + circle2D.Center.X * uAxis.Y + circle2D.Center.Y * vAxis.Y,
                    plane.Center.Z + circle2D.Center.X * uAxis.Z + circle2D.Center.Y * vAxis.Z);

                return new CircleFitResult
                {
                    Center = center3D,
                    Radius = circle2D.Radius,
                    Normal = plane.Normal,
                    InlierCount = inliers.Count
                };
            }
            catch
            {
                return null;  // 拟合失败
            }
        }

        /// <summary>
        /// 使用最小二乘法拟合平面
        /// </summary>
        /// <param name="points">用于拟合平面的点集</param>
        /// <returns>拟合的平面（包含质心和法向量）</returns>
        //private (Point3D Center, Point3D Normal) FitPlane(List<Point3D> points)
        private CenterNormalPair FitPlane(List<Point3D> points)
        {
            // 计算点集的质心
            var center = new Point3D(
                points.Average(p => p.X),  // X坐标平均值
                points.Average(p => p.Y),  // Y坐标平均值
                points.Average(p => p.Z)); // Z坐标平均值

            // 初始化协方差矩阵元素
            double xx = 0, xy = 0, xz = 0, yy = 0, yz = 0, zz = 0;

            // 计算协方差矩阵
            foreach (var point in points)
            {
                double dx = point.X - center.X;  // X方向偏差
                double dy = point.Y - center.Y;  // Y方向偏差
                double dz = point.Z - center.Z;  // Z方向偏差

                // 累加协方差矩阵各元素
                xx += dx * dx; xy += dx * dy; xz += dx * dz;
                yy += dy * dy; yz += dy * dz; zz += dz * dz;
            }

            // 构建协方差矩阵
            var covarianceMatrix = new double[3, 3] {
            { xx, xy, xz },
            { xy, yy, yz },
            { xz, yz, zz }
        };

            // 使用幂迭代法求最小特征值对应的特征向量（平面法向量）
            var normal = InversePowerIteration(covarianceMatrix);

            //return (center, normal);
            return new CenterNormalPair(center, normal);
        }

        /// <summary>
        /// 逆幂迭代法求矩阵的最小特征值对应的特征向量
        /// 通过对协方差矩阵的逆进行幂迭代，收敛到原矩阵最小特征值对应的特征向量
        /// </summary>
        /// <param name="matrix">输入矩阵（对称正定矩阵）</param>
        /// <returns>最小特征值对应的特征向量</returns>
        private Point3D InversePowerIteration(double[,] matrix)
        {
            int n = 3;
            var invMatrix = new double[n, n];

            // 使用高斯消元法计算矩阵的逆
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    invMatrix[i, j] = matrix[i, j];
                }
            }

            // 构造增广矩阵 [A|I]
            var augmented = new double[n, 2 * n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    augmented[i, j] = matrix[i, j];
                }
                augmented[i, n + i] = 1.0;
            }

            // 高斯消元
            for (int i = 0; i < n; i++)
            {
                // 找主元
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(augmented[k, i]) > Math.Abs(augmented[maxRow, i]))
                        maxRow = k;
                }

                // 交换行
                for (int k = i; k < 2 * n; k++)
                {
                    double temp = augmented[i, k];
                    augmented[i, k] = augmented[maxRow, k];
                    augmented[maxRow, k] = temp;
                }

                // 归一化主元行
                double pivot = augmented[i, i];
                if (Math.Abs(pivot) < 1e-10)
                {
                    // 矩阵接近奇异，使用相对扰动
                    // 计算当前行中非零元素的最大值作为扰动基准
                    double rowMax = 0;
                    for (int k = i + 1; k < 2 * n; k++)
                    {
                        rowMax = Math.Max(rowMax, Math.Abs(augmented[i, k]));
                    }
                    double perturbation = Math.Max(1e-10, rowMax * 1e-6);
                    augmented[i, i] += perturbation;
                    pivot = augmented[i, i];
                }

                for (int k = i; k < 2 * n; k++)
                {
                    augmented[i, k] /= pivot;
                }

                // 消元
                for (int k = i + 1; k < n; k++)
                {
                    double factor = augmented[k, i];
                    for (int j = i; j < 2 * n; j++)
                    {
                        augmented[k, j] -= factor * augmented[i, j];
                    }
                }
            }

            // 回代求逆
            for (int i = n - 1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    double factor = augmented[j, i];
                    for (int k = i; k < 2 * n; k++)
                    {
                        augmented[j, k] -= factor * augmented[i, k];
                    }
                }

                // 提取逆矩阵
                for (int j = 0; j < n; j++)
                {
                    invMatrix[i, j] = augmented[i, n + j];
                }
            }

            // 对逆矩阵使用幂迭代（收敛到最小特征值对应的特征向量）
            double[] v = { _random.NextDouble(), _random.NextDouble(), _random.NextDouble() };
            double length = Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
            v[0] /= length; v[1] /= length; v[2] /= length;

            // 幂迭代
            for (int iter = 0; iter < 100; iter++)
            {
                double[] newV = new double[3];

                // 矩阵乘法：newV = invMatrix * v
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        newV[i] += invMatrix[i, j] * v[j];
                    }
                }

                // 归一化新向量
                length = Math.Sqrt(newV[0] * newV[0] + newV[1] * newV[1] + newV[2] * newV[2]);
                if (length < 1e-10) break;

                newV[0] /= length; newV[1] /= length; newV[2] /= length;

                // 检查收敛
                double diff = Math.Abs(newV[0] - v[0]) + Math.Abs(newV[1] - v[1]) + Math.Abs(newV[2] - v[2]);
                if (diff < 1e-10)
                {
                    v = newV;
                    break;
                }

                v = newV;
            }

            return new Point3D(v[0], v[1], v[2]);
        }

        /// <summary>
        /// 解线性方程组 Ax = B
        /// </summary>
        /// <param name="A">系数矩阵</param>
        /// <param name="B">常数向量</param>
        /// <returns>方程组的解向量</returns>
        private double[] SolveLinearSystem(double[,] A, double[] B)
        {
            int n = B.Length;  // 方程组维度
            double[,] matrix = new double[n, n + 1];  // 增广矩阵

            // 构建增广矩阵 [A|B]
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = A[i, j];  // 系数部分
                }
                matrix[i, n] = B[i];  // 常数部分
            }

            // 高斯消元法：前向消元
            for (int i = 0; i < n; i++)
            {
                // 寻找主元（列主元法提高数值稳定性）
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(matrix[k, i]) > Math.Abs(matrix[maxRow, i]))
                        maxRow = k;
                }

                // 交换行（行交换）
                for (int k = i; k <= n; k++)
                {
                    double temp = matrix[i, k];
                    matrix[i, k] = matrix[maxRow, k];
                    matrix[maxRow, k] = temp;
                }

                // 检查主元是否接近零（矩阵可能奇异）
                const double singularThreshold = 1e-10;
                if (Math.Abs(matrix[i, i]) < singularThreshold)
                {
                    // 矩阵奇异，无法求解
                    throw new ArgumentException("系数矩阵奇异，无法求解线性方程组");
                }

                // 消元
                for (int k = i + 1; k < n; k++)
                {
                    double factor = matrix[k, i] / matrix[i, i];  // 消元因子
                    for (int j = i; j <= n; j++)
                    {
                        matrix[k, j] -= factor * matrix[i, j];  // 消元操作
                    }
                }
            }

            // 回代求解
            double[] solution = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                solution[i] = matrix[i, n];  // 初始值为常数项
                for (int j = i + 1; j < n; j++)
                {
                    solution[i] -= matrix[i, j] * solution[j];  // 减去已知解的影响
                }
                solution[i] /= matrix[i, i];  // 除以系数
            }

            return solution;
        }

        /// <summary>
        /// 幂迭代法求矩阵的最小特征值对应的特征向量
        /// </summary>
        /// <param name="matrix">输入矩阵</param>
        /// <returns>最小特征值对应的特征向量</returns>
        private Point3D PowerIteration(double[,] matrix)
        {
            // 初始化随机向量
            double[] v = { _random.NextDouble(), _random.NextDouble(), _random.NextDouble() };
            double length = Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
            v[0] /= length; v[1] /= length; v[2] /= length;  // 归一化

            // 幂迭代
            for (int iter = 0; iter < 100; iter++)
            {
                double[] newV = new double[3];

                // 矩阵乘法：newV = matrix * v
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        newV[i] += matrix[i, j] * v[j];
                    }
                }

                // 归一化新向量
                length = Math.Sqrt(newV[0] * newV[0] + newV[1] * newV[1] + newV[2] * newV[2]);
                if (length < 1e-10) break;  // 防止除零

                newV[0] /= length; newV[1] /= length; newV[2] /= length;

                // 检查收敛
                double diff = Math.Abs(newV[0] - v[0]) + Math.Abs(newV[1] - v[1]) + Math.Abs(newV[2] - v[2]);
                if (diff < 1e-10)  // 收敛条件
                {
                    v = newV;
                    break;
                }

                v = newV;
            }

            return new Point3D(v[0], v[1], v[2]);
        }

        /// <summary>
        /// 根据当前内点比例计算最优迭代次数
        /// </summary>
        /// <param name="inlierCount">内点数量</param>
        /// <param name="totalPoints">总点数</param>
        /// <param name="sampleSize">样本大小</param>
        /// <returns>最优迭代次数</returns>
        private int CalculateOptimalIterations(int inlierCount, int totalPoints, int sampleSize)
        {
            double inlierRatio = (double)inlierCount / totalPoints;  // 内点比例
                                                                     // 根据RANSAC理论公式计算迭代次数
            return (int)Math.Ceiling(Math.Log(1 - _confidence) /
                   Math.Log(1 - Math.Pow(inlierRatio, sampleSize)));
        }
    }

    /// <summary>
    /// u轴与v轴的组合
    /// </summary>
    public class AxisPair
    {
        /// <summary>
        /// 
        /// </summary>
        public Point3D UAxis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Point3D VAxis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AxisPair(Point3D a, Point3D b)
        {
            UAxis = a;
            VAxis = b;
        }
    }

    /// <summary>
    /// 圆心与法向量的组合
    /// </summary>
    public class CenterNormalPair
    {
        /// <summary>
        /// 
        /// </summary>
        public Point3D Center { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Point3D Normal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CenterNormalPair(Point3D a, Point3D b)
        {
            Center = a;
            Normal = b;
        }
    }

    /// <summary>
    /// 浮点值的组合
    /// </summary>
    public class DoublePair
    {
        /// <summary>
        /// 
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DoublePair(double a, double b)
        {
            A = a;
            B = b;
        }
    }
}
