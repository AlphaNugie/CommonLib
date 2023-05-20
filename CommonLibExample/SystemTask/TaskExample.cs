using CommonLib.Clients.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibExample.SystemTask
{
    internal class TaskExample
    {
        /// <summary>
        /// 进行异步写入操作
        /// </summary>
        /// <returns></returns>
        private Task<string> OpcWriteAsync()
        {
            TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
            try
            {
                //进行写入操作
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
    }
}
