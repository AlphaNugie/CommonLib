using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 指令存储器
    /// </summary>
    public class CommandStorage : GenericStorage<string>
    {
        /// <summary>
        /// 以指定最大容量初始化指令存储器
        /// </summary>
        /// <param name="max">最大容量，大于0，否则使用默认容量</param>
        public CommandStorage(int max) : base(max) { }

        /// <summary>
        /// 以默认最大容量初始化指令存储器
        /// </summary>
        public CommandStorage() : base() { }
    }
}
