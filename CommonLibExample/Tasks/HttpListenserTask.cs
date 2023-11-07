using CommonLib.Clients;
using CommonLib.Clients.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibExample.Tasks
{
    public class HttpListenserTask : Task
    {
        private readonly DerivedHttpListener _listener = new DerivedHttpListener();

        public override void Init()
        {
            _listener.IpAddress = "127.0.0.1";
            _listener.Port = 44332;
            _listener.Suffix = "test";
            _listener.Start();
        }

        public override void LoopContent()
        {
            Interval = 100;
            _listener.WebExplorerMessage = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        protected override Task GetNewInstance()
        {
            throw new NotImplementedException();
        }
    }
}
