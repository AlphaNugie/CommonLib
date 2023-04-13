using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLibExample.Tasks
{
    public static class ExampleTaskTest
    {
        public static void Run()
        {
            ExampleTask task = new ExampleTask() { AutoRestart = true, RestartInterval = 3 * 1000 }; //每隔5秒自动重启
            task.Initialize();
            task.Run();
            int i = 0;
            while (++i <= 50)
            {
                Thread.Sleep(1000);
            }
            //task.Dispose(); //释放资源
            //task.Restart(); //重启任务
            //while (++i <= 15)
            //{
            //    Thread.Sleep(1000);
            //}
            return;
        }
    }
}
