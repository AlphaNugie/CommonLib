using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufNetWrapper
{
    /// <summary>
    /// protobuf-net操作类
    /// </summary>
    public static class ProtobufNetWrapper
    {
        public static MemoryStream stream = new MemoryStream(0); //用于二进制数据读取与写入的流
        //public static TypeModel typeModel = RuntimeTypeModel.Create();
        //public static ProtoReader reader = ProtoReader.Create(stream, typeModel);

        /// <summary>
        /// 将实体类对象序列化为byte数组
        /// </summary>
        /// <typeparam name="T">源实体类类型</typeparam>
        /// <param name="instance">待序列化的实体类对象</param>
        /// <returns></returns>
        public static byte[] SerializeToBytes<T>(T instance)
        {
            stream.SetLength(0);
            Serializer.Serialize(stream, instance);
            return stream.ToArray();
        }

        /// <summary>
        /// 将实体类序列化为字符串
        /// </summary>
        /// <typeparam name="T">源实体类类型</typeparam>
        /// <param name="instance">待序列化的实体类</param>
        /// <returns></returns>
        public static string SerializeToString<T>(T instance)
        {
            return Encoding.Default.GetString(SerializeToBytes(instance));
        }

        /// <summary>
        /// 将byte数组反序列化为实体类对象
        /// </summary>
        /// <typeparam name="T">目标实体类类型</typeparam>
        /// <param name="bytes">待反序列化的byte数组</param>
        /// <returns></returns>
        public static T DeserializeFromBytes<T>(byte[] bytes)
        {
            if (bytes == null)
                return default(T);
            stream.SetLength(0);
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return Serializer.Deserialize<T>(stream);
        }

        /// <summary>
        /// 将字符串反序列化为实体类对象
        /// </summary>
        /// <typeparam name="T">目标实体类类型</typeparam>
        /// <param name="str">待反序列化的字符串</param>
        /// <returns></returns>
        public static T DeserializeFromString<T>(string str)
        {
            return DeserializeFromBytes<T>(Encoding.Default.GetBytes(str));
        }
    }
}
