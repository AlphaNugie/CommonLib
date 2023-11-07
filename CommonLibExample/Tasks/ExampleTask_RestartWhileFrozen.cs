using CommonLib.Clients.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CommonLibExample.Tasks
{
    public class ExampleTask_RestartWhileFrozen : Task
    {
        public ExampleTask_RestartWhileFrozen(int interval = 1000, bool autoRestart = false, bool restartWhileFrozen = false) : base(interval, autoRestart, restartWhileFrozen)
        {
        }

        public override void Init()
        {
        }

        public override void LoopContent()
        {
            Thread.Sleep(int.MaxValue);
        }

        protected override Task GetNewInstance()
        {
            var task = new ExampleTask_RestartWhileFrozen();
            task.StateChanged += new TaskStateChangedEventHandler(Task_StateChanged);
            return task;
        }

        private void Task_StateChanged(object sender, TaskStateChangedEventArgs e)
        {
            if (e.State == CommonLib.Enums.ServiceState.Stopped)
                return;
            string message = string.Format("{0}任务已启动，任务重启次数：{1}", GetType().Name, e.RestartCounter);
            Console.WriteLine(message);
        }
    }
}
