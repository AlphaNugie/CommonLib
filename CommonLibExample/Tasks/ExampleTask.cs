using CommonLib.Clients.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibExample.Tasks
{
    public class ExampleTask : Task
    {
        public override void Init()
        {
        }

        public override void LoopContent()
        {
            //Restart();
        }

        protected override Task GetNewInstance(/*int interval, bool autoRestart, long restartInterval*/)
        {
            var task = new ExampleTask()/* { Interval = interval, AutoRestart = autoRestart, RestartInterval = restartInterval }*/;
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
