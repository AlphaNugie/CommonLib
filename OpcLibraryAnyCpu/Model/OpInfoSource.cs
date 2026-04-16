using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DA
namespace OpcLibrary.Model
#elif UA
namespace OpcLibrary.Ua.Model
#endif
{
    /// <summary>
    /// 连接OPC服务的成功消息的实体类
    /// </summary>
    public class OpInfoSource
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 操作返回的消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 操作是否取消（连接超时）
        /// </summary>
        public bool OperationCancelled { get; set; }
    }
}
