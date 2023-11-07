using CommonLib.Clients.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibExample.Tasks
{
    public class ExampleTask : Task
    {
        public ExampleTask() : base() { }

        public ExampleTask(int interval = 1000, bool autoRestart = false, long restartInterval = 0, bool restartWhileFrozen = false) : base(interval, autoRestart, restartInterval, restartWhileFrozen)
        {
        }

        public override void Init()
        {
        }

        public override void LoopContent()
        {
            //Restart();
        }

        protected override Task GetNewInstance()
        {
            var task = new ExampleTask();
            task.StateChanged += new TaskStateChangedEventHandler(Task_StateChanged);
            return task;
        }

        //protected override void RestartUrself()
        //{
        //    var task = new ExampleTask() { Interval = Interval };
        //    task.StateChanged += new TaskStateChangedEventHandler(Task_StateChanged);
        //    task.Initialize();
        //    task.Run();
        //}

        private void Task_StateChanged(object sender, TaskStateChangedEventArgs e)
        {
            var state = e.State;
        }
    }
}
