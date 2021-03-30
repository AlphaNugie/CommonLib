using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 泛式存储器
    /// </summary>
    public class GenericStorage<T>
    {
        /// <summary>
        /// 默认最大容量
        /// </summary>
        public const int DEFAULT_MAX_CAPACITY = 10;

        protected int _index = -1;

        #region 属性
        /// <summary>
        /// 队列
        /// 先进先出，新加入的指令在队列后方，索引靠后
        /// </summary>
        public Queue<T> Queue { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public int Count { get { return Queue == null ? 0 : Queue.Count; } }

        /// <summary>
        /// 当前指令索引，为-1时代表未在队列中
        /// 与队列索引相反，新加入指令索引靠前（最新指令索引为0），相当于从队列尾部查找
        /// </summary>
        public int Index
        {
            get { return this._index; }
            set
            {
                int max_index = this.Count - 1;
                this._index = value.Between(-1, max_index) ? value : this._index;
                this.CurrentContent = this._index == -1 ? default : this.Queue.ElementAt(max_index - this._index);
            }
        }

        /// <summary>
        /// 当前实体，未在队列中时为空
        /// </summary>
        public T CurrentContent { get; set; }

        /// <summary>
        /// 最大容量
        /// </summary>
        public int MaxCapacity { get; set; }
        #endregion

        /// <summary>
        /// 以指定最大容量初始化存储器
        /// </summary>
        /// <param name="max">最大容量，大于0，否则使用默认容量</param>
        public GenericStorage(int max)
        {
            this.MaxCapacity = max > 0 ? max : DEFAULT_MAX_CAPACITY;
            this.Queue = new Queue<T>(this.MaxCapacity);
            this.Index = -1;
        }

        /// <summary>
        /// 以默认最大容量初始化存储器
        /// </summary>
        public GenericStorage() : this(DEFAULT_MAX_CAPACITY) { }

        /// <summary>
        /// 压入新实体
        /// </summary>
        /// <param name="instance">待压入指令</param>
        public void Push(T instance)
        {
            this.Queue.Enqueue(instance);
            if (this.Queue.Count > this.MaxCapacity)
                this.Queue.Dequeue();
            this.Index = -1;
        }

        /// <summary>
        /// 以默认项填满存储器
        /// </summary>
        public void FillEmptyShells()
        {
            for (int i = 0; i < this.MaxCapacity; i++)
                this.Push(default);
        }

        /// <summary>
        /// 清空队列内所有对象
        /// </summary>
        public void Clear()
        {
            if (Count != 0)
                Queue.Clear();
        }

        /// <summary>
        /// 返回当前实体
        /// </summary>
        /// <returns></returns>
        public T Current()
        {
            return this.CurrentContent;
        }

        /// <summary>
        /// 转到上一条实体并返回，假如已是末尾则无变化
        /// </summary>
        /// <returns></returns>
        public T Previous()
        {
            this.Index++;
            return this.Current();
        }

        /// <summary>
        /// 转到下一条实体并返回，假如已在第一条实体则跳出队列
        /// </summary>
        /// <returns></returns>
        public T Next()
        {
            this.Index--;
            return this.Current();
        }

        /// <summary>
        /// 转到队列中最靠前（最晚加入的）实体并返回，假如已在第一条实体则跳出队列
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            this.Index = 0;
            return this.Current();
        }

        /// <summary>
        /// 转到队列中最靠前（最早加入的）实体并返回，假如已在第一条实体则跳出队列
        /// </summary>
        /// <returns></returns>
        public T Last()
        {
            this.Index = this.Count - 1;
            return this.Current();
        }

        /// <summary>
        /// 根据队列内部索引获取元素
        /// </summary>
        /// <param name="index">Queue内部索引</param>
        /// <returns></returns>
        public T ElementAt(int index)
        {
            return this.Queue == null ? default : this.Queue.ElementAtOrDefault(index);
        }
    }
}
