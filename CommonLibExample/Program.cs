﻿using CommonLib.Clients;
using CommonLib.DataUtil;
using CommonLib.Extensions.Property;
using CommonLib.Function;
using CommonLib.Helpers;
using CommonLibExample.MathNet;
using CommonLibExample.PropertyMapper;
using MathWorks.MATLAB.NET.Arrays;
using MatlabFunctions;
//using MatlabFunctionsNative;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLibExample
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region Matlab 曲线拟合API（需要以x64架构运行）
            //Polyfitting fitting = new Polyfitting();
            //int[,] listx = { { 0 }, { 1 }, { 2 } }, listy = { { 0 }, { 2 }, { 4 } };
            //MWArray x = new MWNumericArray(listx), y = new MWNumericArray(listy);
            //MWNumericArray result = fitting.GetPolyfit(x, y) as MWNumericArray;
            //double[] results = result.ToArray().Cast<double>().Select(d => Math.Round(d, 3)).ToArray(); //返回结果分为为斜率与截距
            #endregion

            #region test
            //GenericStorage<double> store = new GenericStorage<double>(5);
            //store.Push(1);
            //store.Push(3);
            //store.Push(5);
            //store.Push(7);
            //List<double> list = store.Queue.ToList();
            //return;

            //GlitchFilter filter = new GlitchFilter(true, 6);
            //double temp = 0;
            //List<double> list = new List<double>() { 1, 2, 1, 2, 1, 2, 1, 10, 4, -2 };
            //foreach (var value in list)
            //{
            //    filter.PushValue(value);
            //    temp = filter.CurrentValue;
            //}
            //return;

            //string temp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //string stamp = DateTimeHelper.GetTimeStamp();
            //return;

            //MathNetSample.RunSample();
            //List<double> listx = new List<double>() { 2, 0, 1 }, listy = new List<double>() { 9, 1, 5 };
            //double[] results = CurveFitting.GetCurveCoefficients(listx, listy, listx.Count, 1);
            //return;
            //string fileName = @"D:\煤二期\Documents\雷达数据\files-20200723-臂架右后雷达\20200723_155710_行走向北2_距离增加\arm_right_back";
            //string[] lines = File.ReadAllLines(fileName + ".txt");
            //foreach (string line in lines)
            //    if (!line.Contains('-'))
            //        File.AppendAllLines(fileName + "-modi.txt", new string[] { line });

            //PropertyMapperExample.CopyToMethodTest();
            //PropertyMapperExample.CopyFromMethodTest();

            //string path = @"Log", name = "test.txt";
            //FileSystemHelper.UpdateFilePath(ref path, name, out string fileNameDate, out string filePath, out string filePathDate);

            //int a = 1, b = 2;
            //Type type1 = a.GetType(), type2 = b.GetType();
            //bool flag = type1 == type2;

            //TempClass c = new TempClass() { Id = 1 };
            //Type type = typeof(Converter);
            //var method = type.GetMethod("ConvertType", new Type[] { typeof(object) });
            //PropertyInfo prop1 = c.GetType().GetProperty("Id"), prop2 = c.GetType().GetProperty("Value");
            //MethodInfo genericMethod1 = method.MakeGenericMethod(prop1.PropertyType), genericMethod2 = method.MakeGenericMethod(prop2.PropertyType);
            //string source = "44", source2 = "33.23";
            //prop1.SetValue(c, genericMethod1.Invoke(null, new object[] { source }));
            //prop2.SetValue(c, genericMethod2.Invoke(null, new object[] { source2 }));

            //double target = (double)"1.23".ConvertType(typeof(double));
            //Type type = target.GetType();
            //float target3 = Converter.ConvertType<float>("1.23");
            //int target2 = Converter.ConvertType<int>("44");
            //string result = Converter.ConvertType<string>(DateTime.Now);
            //DateTime time = Converter.ConvertType<DateTime>(result);
            //int i = "13".ConvertType<int>();
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }

    public class TempClass
    {
        public int Id { get; set; }

        public double Value { get; set; }
    }
}
