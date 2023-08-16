using CommonLib.Clients.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibExample.SystemTask
{
    /// <summary>
    /// System.Threading.Tasks.Task任务的例子
    /// </summary>
    internal class TaskExample
    {
        /// <summary>
        /// 进行异步操作
        /// </summary>
        /// <returns></returns>
        private Task<string> DoSomethingAsync()
        {
            TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
            try
            {
                //进行一些操作
                string result = "成功";
                taskCompletionSource.TrySetResult(result);
            }
            catch (Exception e)
            {
                taskCompletionSource.TrySetException(e);
            }
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public string RunTask(Task<string> task)
        {
            bool intime; //是否在响应时间内完成
            try { intime = task.Wait(5000); }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                throw e;
            }
            if (!intime)
                throw new TimeoutException("Timeout");
            else
            {
                return task.Result;
            }
        }

        /// <summary>
        /// 执行例子
        /// </summary>
        public static void RunExample()
        {
            var example = new TaskExample();
            var task = example.DoSomethingAsync();
            string result = example.RunTask(task);
            Console.WriteLine(result);
        }
    }
}
