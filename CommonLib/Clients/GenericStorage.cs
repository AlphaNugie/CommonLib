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
            get { return _index; }
            set
            {
                int max_index = Count - 1;
                _index = value.Between(-1, max_index) ? value : _index;
                CurrentContent = _index == -1 ? default : Queue.ElementAt(max_index - _index);
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
            MaxCapacity = max > 0 ? max : DEFAULT_MAX_CAPACITY;
            Queue = new Queue<T>(MaxCapacity);
            Index = -1;
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
            Queue.Enqueue(instance);
            if (Queue.Count > MaxCapacity)
                Queue.Dequeue();
            Index = -1;
        }

        /// <summary>
        /// 以默认项填满存储器
        /// </summary>
        public void FillEmptyShells()
        {
            for (int i = 0; i < MaxCapacity; i++)
                Push(default);
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
            return CurrentContent;
        }

        /// <summary>
        /// 转到上一条实体并返回，假如已是末尾则无变化
        /// </summary>
        /// <returns></returns>
        public T Previous()
        {
            Index++;
            return Current();
        }

        /// <summary>
        /// 转到下一条实体并返回，假如已在第一条实体则跳出队列
        /// </summary>
        /// <returns></returns>
        public T Next()
        {
            Index--;
            return Current();
        }

        /// <summary>
        /// 转到队列中最靠前（最晚加入的）实体并返回，假如已在第一条实体则跳出队列
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            Index = 0;
            return Current();
        }

        /// <summary>
        /// 转到队列中最靠前（最早加入的）实体并返回，假如已在第一条实体则跳出队列
        /// </summary>
        /// <returns></returns>
        public T Last()
        {
            Index = Count - 1;
            return Current();
        }

        /// <summary>
        /// 根据队列内部索引获取元素
        /// </summary>
        /// <param name="index">Queue内部索引</param>
        /// <returns></returns>
        public T ElementAt(int index)
        {
            return Queue == null ? default : Queue.ElementAtOrDefault(index);
        }
    }
}
