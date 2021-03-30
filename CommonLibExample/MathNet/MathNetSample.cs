using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibExample.MathNet
{
    public static class MathNetSample
    {
        public static void RunSample()
        {
            var M = Matrix<double>.Build;
            //List<SampleClass> list = new List<SampleClass>() { new SampleClass(1, 1, 2), new SampleClass(2, 3, 5), new SampleClass(0, 4, 6), new SampleClass(7, 8, 11), new SampleClass(0.5, 7.5, 1.2) };
            List<SampleClass> list = new List<SampleClass>() { new SampleClass(1, 1, 3), new SampleClass(2, 3, 6), new SampleClass(0, 4, 5), new SampleClass(7, 8, 16), new SampleClass(0.5, 7.5, 9) };
            double[] arrayx = list.Select(s => s.X).ToArray(), arrayy = list.Select(s => s.Y).ToArray(), arrayz = list.Select(s => s.Z).ToArray(), arrayc = new double[list.Count];
            for (int i = 0; i < arrayc.Length; i++)
                arrayc[i] = 1;
            Matrix<double> A = M.DenseOfColumnArrays(arrayx, arrayy, arrayc), B = M.DenseOfColumnArrays(arrayz), AT = A.Transpose(), ATA = AT * A, ATB = AT * B;
            string asub = A.ToString(), bsub = B.ToString(), atsub = AT.ToString(), atasub = ATA.ToString(), atbsub = ATB.ToString();
            Matrix<double> X = ATA.LU().Solve(ATB);
            //X = ATA.Solve(ATB);
            double[] cs = X.ToColumnMajorArray();
            double arctan = Math.Atan(1 / cs[0]) * 180 / Math.PI;
            //List <List<double>> list2 = list1.Select(s => new List<double>() { s.X, s.Y, 1 }).ToList(), list3 = list1.Select(s => new List<double>() { s.Z }).ToList();
            //Matrix<double> A = M.DenseOfRows(list2), B = M.DenseOfRows(list3);
            double d = ATA.Determinant();
        }
    }

    public class SampleClass
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public SampleClass(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
