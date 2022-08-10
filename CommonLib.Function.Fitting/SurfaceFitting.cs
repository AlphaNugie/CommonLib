using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Function.Fitting
{
    /// <summary>
    /// 平面拟合类
    /// </summary>
    public static class SurfaceFitting
    {
        ///<summary>
        ///用最小二乘法拟合三元一次曲面（也就是平面），获取曲面拟合函数的系数
        ///表达式形如ax + by + d = z，方法将返回包含a, b, d的的数组
        ///</summary>
        ///<param name="arrX">已知点的x坐标集合</param>
        ///<param name="arrY">已知点的y坐标集合</param>
        ///<param name="arrZ">已知点的z坐标集合</param>
        ///<param name="message">返回的错误消息</param>
        ///<returns>返回一个数组，数组中包括x, y的系数以及常数</returns>
        public static double[] GetSurceCoefficients(IEnumerable<double> arrX, IEnumerable<double> arrY, IEnumerable<double> arrZ, out string message)
        {
            message = string.Empty;
            int lengthx = arrX == null ? 0 : arrX.Count(), lengthy = arrY == null ? 0 : arrY.Count(), lengthz = arrZ == null ? 0 : arrZ.Count(), length;
            //xyz数量一致且不为0
            if (lengthx != lengthy || lengthx != lengthz || lengthy != lengthz)
            {
                message = "未知数长度不一致";
                return null;
            }
            if ((length = lengthx) == 0)
            {
                message = "未知数数量为0";
                return null;
            }
            var M = Matrix<double>.Build;
            //var M = FittingVariables.MatrixBuilder;
            double[] arrayc = new double[length];
            for (int i = 0; i < arrayc.Length; i++)
                arrayc[i] = 1;
            //假如xyz有n组，矩阵公式形如AX=B，其中：A为nx3(n行3列)的矩阵，3列从左到右分别为x列向量、y列向量、1；X为3x1的未知数矩阵，包括表达式系数a、b、d；B为nx1的矩阵，包括z列向量
            //公式左右同时左乘矩阵A的转置AT，有ATA * X = ATB，其中ATA、ATB分别为3x3、3x1的矩阵
            Matrix<double> A = M.DenseOfColumnArrays(arrX.ToArray(), arrY.ToArray(), arrayc), B = M.DenseOfColumnArrays(arrZ.ToArray()), AT = A.Transpose(), ATA = AT * A, ATB = AT * B;
            Matrix<double> X = ATA.LU().Solve(ATB);
            //Matrix<double> X = ATA.QR().Solve(ATB);
            return X.ToColumnMajorArray();
        }
    }
}
