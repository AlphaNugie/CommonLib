using CommonLib.Clients.Tasks;
using SocketHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibExample.Tasks
{
    public class TcpServerTask : Task
    {
        private readonly SocketTcpServer _server = new SocketTcpServer();

        public override void Init()
        {
            _server.ServerIp = "127.0.0.1";
            _server.ServerPort = 44333;
            _server.Start();
        }

        public override void LoopContent()
        {
            _server.SendData(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), StringType.Normal);
        }

        protected override Task GetNewInstance()
        {
            throw new NotImplementedException();
        }
    }
}
