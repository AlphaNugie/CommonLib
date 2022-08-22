using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients.Tasks
{
    /// <summary>
    /// 空白任务
    /// </summary>
    public class BlankTask : Task
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public BlankTask() : base() { }

        public override void Init()
        {
            //throw new NotImplementedException();
        }

        public override void LoopContent()
        {
            //throw new NotImplementedException();
        }
    }
}
