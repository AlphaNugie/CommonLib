using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufNetLibrary
{
    /// <summary>
    /// protobuf-net操作类
    /// </summary>
    public static class ProtobufNetWrapper
    {
        //private static readonly MemoryStream stream = new MemoryStream(0); //用于二进制数据读取与写入的流
        private const int HEADER_SIZE = 8;

        /// <summary>
        /// 将Int32整型数字转换为byte数组并写入另一指定byte数组内（从指定的目标索引开始）
        /// </summary>
        /// <param name="buffer">待写入值的目标byte数组</param>
        /// <param name="offset">开始写入的目标索引位置</param>
        /// <param name="value">待写入的值</param>
        public static void WriteValueToByteArray(byte[] buffer, int offset, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
        }

        /// <summary>
        /// 在一指定byte数组内，从指定的目标索引开始将4个字节转换为Int32整型数字
        /// </summary>
        /// <param name="data">待提取整型数字的源byte数组</param>
        /// <param name="offset">开始读取的索引位置</param>
        /// <returns></returns>
        public static int ReadValueFromByteArray(byte[] data, int offset)
        {
            //int最大可能需要4个字节表示，同时考虑索引位置，数组长度不得小于offset+4
            if (data == null || data.Length < offset + 4)
                return -1;
            return BitConverter.ToInt32(data, offset);
        }

        /// <summary>
        /// 将实体类对象序列化为byte数组
        /// </summary>
        /// <typeparam name="T">源实体类类型</typeparam>
        /// <param name="instance">待序列化的实体类对象</param>
        /// <returns></returns>
        public static byte[] SerializeToBytes<T>(T instance)
        {
            return SerializeToBytes(instance, 0);
            #region Original
            //stream.SetLength(0);
            //if (instance != null)
            //    Serializer.Serialize(stream, instance);
            //return stream.ToArray();
            #endregion
        }

        /// <summary>
        /// 将实体类对象序列化为byte数组，放在由Int32整型数字转换而来的byte数组后方
        /// </summary>
        /// <typeparam name="T">源实体类类型</typeparam>
        /// <param name="instance">待序列化的实体类对象</param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static byte[] SerializeToBytes<T>(T instance, int header)
        {
            //stream.SetLength(0);
            using (MemoryStream stream = new MemoryStream())
            {
                if (instance != null)
                    Serializer.Serialize(stream, instance);
                byte[] buffer = stream.ToArray();
                byte[] bytes = new byte[buffer.Length + HEADER_SIZE];
                WriteValueToByteArray(bytes, 0, header);
                Array.Copy(buffer, 0, bytes, HEADER_SIZE, buffer.Length);
                return bytes;
            }
        }

        /// <summary>
        /// 将实体类序列化为Base64数字编码字符串
        /// </summary>
        /// <typeparam name="T">源实体类类型</typeparam>
        /// <param name="instance">待序列化的实体类</param>
        /// <returns></returns>
        public static string SerializeToString<T>(T instance)
        {
            return SerializeToString<T>(instance, string.Empty);
        }

        /// <summary>
        /// 将实体类序列化为Base64数字编码字符串，在最前面添加特定字符串
        /// </summary>
        /// <typeparam name="T">源实体类类型</typeparam>
        /// <param name="instance">待序列化的实体类</param>
        /// <param name="header">待添加在最前面的字符串</param>
        /// <returns></returns>
        public static string SerializeToString<T>(T instance, string header)
        {
            return header + Convert.ToBase64String(SerializeToBytes(instance));
        }

        /// <summary>
        /// 从开头开始跳过消息头等长数量的元素，再将byte数组全部反序列化为实体类对象
        /// </summary>
        /// <typeparam name="T">目标实体类类型</typeparam>
        /// <param name="bytes">待反序列化的byte数组</param>
        /// <returns></returns>
        public static T DeserializeFromBytes<T>(byte[] bytes)
        {
            return DeserializeFromBytes<T>(bytes, HEADER_SIZE);
            //return DeserializeFromBytes<T>(bytes, 0);
        }

        /// <summary>
        /// 从开头开始跳过byte数组中指定数量的元素，再将byte数组反序列化为实体类对象
        /// </summary>
        /// <typeparam name="T">目标实体类类型</typeparam>
        /// <param name="bytes">待反序列化的byte数组</param>
        /// <param name="length">byte数组的跳过长度</param>
        /// <returns></returns>
        public static T DeserializeFromBytes<T>(byte[] bytes, int length)
        {
            if (bytes == null)
                return default;
            if (length > 0)
                bytes = bytes.Skip(length).ToArray();
            using (MemoryStream stream = new MemoryStream())
            {
                //stream.SetLength(0);
                stream.Write(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return Serializer.Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// 从开头开始跳过byte数组中指定数量的元素，再尝试将byte数组反序列化为实体类对象，返回序列化是否成功
        /// </summary>
        /// <typeparam name="T">目标实体类类型</typeparam>
        /// <param name="bytes">待反序列化的byte数组</param>
        /// <param name="length">byte数组的跳过长度</param>
        /// <param name="target">待转换的目标实体类对象</param>
        /// <returns></returns>
        public static bool TryDeserializeFromBytes<T>(byte[] bytes, int length, out T target)
        {
            #region Original
            //target = default;
            //bool result = false;
            //if (bytes == null)
            //    return result;
            //if (length > 0)
            //    bytes = bytes.Skip(length).ToArray();
            //stream.SetLength(0);
            //stream.Write(bytes, 0, bytes.Length);
            //stream.Seek(0, SeekOrigin.Begin);
            //try
            //{
            //    target = Serializer.Deserialize<T>(stream);
            //    result = true;
            //}
            //catch (Exception) { target = default; }
            #endregion
            try { target = DeserializeFromBytes<T>(bytes, length); }
            catch (Exception) { target = default; }
            return target != null;
        }

        /// <summary>
        /// 从开头开始跳过消息头等长数量的元素，再尝试将byte数组反序列化为实体类对象，返回序列化是否成功
        /// </summary>
        /// <typeparam name="T">目标实体类类型</typeparam>
        /// <param name="bytes">待反序列化的byte数组</param>
        /// <param name="target">待转换的目标实体类对象</param>
        /// <returns></returns>
        public static bool TryDeserializeFromBytes<T>(byte[] bytes, out T target)
        {
            return TryDeserializeFromBytes<T>(bytes, HEADER_SIZE, out target);
        }

        /// <summary>
        /// 将字符串反序列化为实体类对象
        /// </summary>
        /// <typeparam name="T">目标实体类类型</typeparam>
        /// <param name="str">待反序列化的字符串</param>
        /// <returns></returns>
        public static T DeserializeFromString<T>(string str)
        {
            return DeserializeFromString<T>(str, null);
        }

        /// <summary>
        /// 从字符串开头跳过特定的子字符串，将字符串剩余部分反序列化为实体类对象
        /// </summary>
        /// <typeparam name="T">目标实体类类型</typeparam>
        /// <param name="str">待反序列化的字符串</param>
        /// <param name="header">跳过的子字符串</param>
        /// <returns></returns>
        public static T DeserializeFromString<T>(string str, string header)
        {
            if (string.IsNullOrWhiteSpace(str))
                return default;
            header = header ?? string.Empty;
            str = str.StartsWith(header, StringComparison.Ordinal) ? str.Substring(header.Length) : str;
            return DeserializeFromBytes<T>(Convert.FromBase64String(str));
        }
    }
}
