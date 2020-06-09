using CommonLib.DataUtil;
using CommonLib.Extensions.Property;
using CommonLib.Function;
using CommonLib.Helpers;
using CommonLibExample.PropertyMapper;
using System;
using System.Collections.Generic;
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
