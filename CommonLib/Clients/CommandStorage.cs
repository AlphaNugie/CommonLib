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
        //private const int _default_max_capacity = 10; //默认最大容量
        //private int _index = -1;

        //#region 属性
        ///// <summary>
        ///// 指令队列
        ///// 先进先出，新加入的指令在队列后方，索引靠后
        ///// </summary>
        //public Queue<string> CommandQueue { get; set; }

        ///// <summary>
        ///// 当前指令索引，为-1时代表未在队列中
        ///// 与队列索引相反，新加入指令索引靠前（最新指令索引为0），相当于从队列尾部查找
        ///// </summary>
        //public int Index
        //{
        //    get { return this._index; }
        //    set
        //    {
        //        int max_index = this.CommandQueue.Count - 1;
        //        this._index = value.Between(-1, max_index) ? value : this._index;
        //        this.CurrentCommand = this._index == -1 ? string.Empty : this.CommandQueue.ElementAt(max_index - this._index);
        //    }
        //}

        ///// <summary>
        ///// 当前指令，未在队列中时为空
        ///// </summary>
        //public string CurrentCommand { get; set; }

        ///// <summary>
        ///// 最大容量
        ///// </summary>
        //public int MaxCapacity { get; set; }
        //#endregion

        /// <summary>
        /// 以指定最大容量初始化指令存储器
        /// </summary>
        /// <param name="max">最大容量，大于0，否则使用默认容量</param>
        public CommandStorage(int max) : base(max) { }
        //{
        //    this.MaxCapacity = max > 0 ? max : _default_max_capacity;
        //    this.CommandQueue = new Queue<string>(this.MaxCapacity);
        //    this.Index = -1;
        //    //this.CurrentCommand = string.Empty;
        //}

        /// <summary>
        /// 以默认最大容量初始化指令存储器
        /// </summary>
        public CommandStorage() : base(/*_default_max_capacity*/) { }

        ///// <summary>
        ///// 压入新指令
        ///// </summary>
        ///// <param name="command">待压入指令</param>
        //public void PushCommand(string command)
        //{
        //    this.CommandQueue.Enqueue(command);
        //    if (this.CommandQueue.Count > this.MaxCapacity)
        //        this.CommandQueue.Dequeue();
        //    this.Index = -1;
        //}

        ///// <summary>
        ///// 转到上一条指令并返回指令内容，假如已是末尾则无变化
        ///// </summary>
        ///// <returns></returns>
        //public string LastCommand()
        //{
        //    this.Index++;
        //    return this.CurrentCommand;
        //}

        ///// <summary>
        ///// 转到下一条指令并返回指令内容，假如已在第一条指令则跳出队列
        ///// </summary>
        ///// <returns></returns>
        //public string NextCommand()
        //{
        //    this.Index--;
        //    return this.CurrentCommand;
        //}
    }
}
