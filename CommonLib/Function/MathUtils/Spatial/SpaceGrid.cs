using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.MathUtils.Spatial
{
    /// <summary>
    /// 管理所有立方块的立方块综合信息对象，提供添加点、获取某立方块的平均点、更新某立方块的点等功能
    /// </summary>
    public class SpaceGrid
    {
        private readonly Dictionary<SpaceIndex, GridCube> _gridCubes = new Dictionary<SpaceIndex, GridCube>();

        /// <summary>
        /// 立方体的边长（米）
        /// </summary>
        //public const double UnitSize = 0.05;
        public const double UnitSize = 0.15;

        /// <summary>
        /// 用给定的Point3D序列初始化立方块综合信息对象
        /// </summary>
        /// <param name="points2Add"></param>
        public SpaceGrid(IEnumerable<Point3D> points2Add = null)
        {
            if (points2Add == null || points2Add.Count() == 0) return;
            AddPoints(points2Add);
        }

        /// <summary>
        /// 用给定的Point3D数组初始化立方块综合信息对象
        /// </summary>
        /// <param name="points"></param>
        public SpaceGrid(params Point3D[] points)
        {
            if (points == null || points.Length == 0) return;
            AddPoints(points);
        }

        /// <summary>
        /// 清除所有的立方块及其内部包含的所有坐标点
        /// </summary>
        public void Clear()
        {
            foreach (var key in _gridCubes.Keys)
            {
                _gridCubes[key].Clear();
                _gridCubes.Remove(key);
            }
            _gridCubes.Clear();
        }

        /// <summary>
        /// 添加空间中的坐标点，按坐标值计算索引以划分到正确的立方体内
        /// </summary>
        /// <param name="point"></param>
        public void AddPoint(Point3D point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point), "提供的坐标点对象为空引用");
            var key = GetGridKey(point);
            if (!_gridCubes.ContainsKey(key))
                _gridCubes.Add(key, new GridCube());
            _gridCubes[key].AddPoint(point);
        }

        /// <summary>
        /// 批量添加坐标点
        /// </summary>
        /// <param name="points"></param>
        public void AddPoints(IEnumerable<Point3D> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points), "提供的坐标点列表对象为空引用");
            foreach (var point in points)
                AddPoint(point);
        }

        /// <summary>
        /// 按索引找出对应位置立方体的坐标点（内部所有点取均值，没有任何点返回null）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Point3D GetAveragePoint(SpaceIndex key)
        {
            if (_gridCubes.ContainsKey(key))
                return _gridCubes[key].GetAveragePoint();
            return null;
        }

        ///// <summary>
        ///// 用新坐标对象更新索引所在位置的立方体坐标点列表，之前所有点清除
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="newPoint"></param>
        //public void UpdatePoint(SpaceIndex key, Point3D newPoint)
        //{
        //    if (gridCubes.ContainsKey(key))
        //    {
        //        gridCubes[key].Points.Clear();
        //        gridCubes[key].Points.Add(newPoint);
        //    }
        //}

        /// <summary>
        /// 给定一个空间中坐标点，计算这个坐标点所应划分到的立方块的位置索引
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private SpaceIndex GetGridKey(Point3D point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point), "提供的坐标点对象为空引用");
            int x = (int)(point.X / UnitSize);
            int y = (int)(point.Y / UnitSize);
            int z = (int)(point.Z / UnitSize);
            return new SpaceIndex(x, y, z);
        }

        /// <summary>
        /// 列出所有立方块的均值坐标点，可提供额外的bool参数用于判断是否仅保留最上层的点（默认为false）
        /// </summary>
        /// <param name="upperPoints">只要最上层的点</param>
        /// <returns></returns>
        public List<Point3D> GetAllAveragePoints(bool upperPoints = false)
        {
            //return _gridCubes.Values.Select(cube => cube.GetAveragePoint()).ToList();
            List<GridCube> cubes = new List<GridCube>();
            //假如只取最上层点，则按X索引和Y索引分组，再取出每组中最大的索引（在XY相同的情况下取出Z最大的SpaceIndex）
            if (upperPoints)
            {
                var indexGroups = _gridCubes.Keys.GroupBy(index => new { index.XIndex, index.YIndex });
                cubes = indexGroups.Select(group => _gridCubes[group.Max()]).ToList();
            }
            else
                cubes = _gridCubes.Values.ToList();
            return cubes.Select(cube => cube.GetAveragePoint()).ToList();
        }
    }
}
