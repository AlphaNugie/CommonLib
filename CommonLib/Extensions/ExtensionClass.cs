using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Extensions
{
    /// <summary>
    /// 类功能扩展类
    /// </summary>
    public static class ExtensionClass
    {
        /// <summary>
        /// 按比例或按数量缩小列表内元素数量（从两侧或从起始处缩减）
        /// </summary>
        /// <typeparam name="T">列表元素的泛型类型</typeparam>
        /// <param name="list">待缩减的列表</param>
        /// <param name="prop">比例或数量，小于1则缩减到此比例，否则为需要缩减的元素的数量（不可为负数）</param>
        /// <param name="both_end">是否从两侧缩减，假如为false，则仅从起始处缩减</param>
        /// <returns></returns>
        public static IEnumerable<T> Shrink<T>(this IEnumerable<T> list, double prop, bool both_end)
        {
            if (list == null)
                throw new ArgumentNullException(paramName: nameof(list));
            if (prop < 0)
                throw new ArgumentOutOfRangeException(paramName: nameof(prop));
            int length = list.Count();
            int half = prop < 1 ? (int)Math.Floor((1 - prop) * length) : (int)Math.Floor(prop); //将被削减的元素的数量
            int most = length < half ? 0 : length - half; //最终保留的元素的数量（最终数量小于0时补为0）
            if (both_end)
                half /= 2;
            //int half = (int)Math.Floor((1 - prop) / 2 * length), most = (int)Math.Floor(prop * length);
            return list.Skip(half).Take(most);
        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="input">待反转的源字符串</param>
        /// <returns></returns>
        public static string Reverse(this string input)
        {
            return new string(input.ToCharArray().Reverse().ToArray());
        }

        ///// <summary>
        ///// 更新并返回TcpClient的连接状态
        ///// </summary>
        ///// <returns>假如处于连接状态，返回true，否则返回false</returns>
        //public static bool IsSocketConnected(this TcpClient client)
        //{
        //    //假如TcpClient对象为空
        //    if (client == null || client.Client == null)
        //        return false;

        //    Socket socket = client.Client;
        //    return (!socket.Poll(1000, SelectMode.SelectRead) || socket.Available != 0) && socket.Connected;
        //}

        ///// <summary>
        ///// 更新并返回UdpClient的连接状态
        ///// </summary>
        ///// <returns>假如处于连接状态，返回true，否则返回false</returns>
        //public static bool IsSocketConnected(this UdpClient client)
        //{
        //    //假如UdpClient对象为空
        //    if (client == null || client.Client == null)
        //        return false;

        //    Socket socket = client.Client;
        //    return (!socket.Poll(1000, SelectMode.SelectRead) || socket.Available != 0) && socket.Connected;
        //}

        /// <summary>
        /// 更新并返回TcpClient的连接状态
        /// </summary>
        /// <returns>假如处于连接状态，返回true，否则返回false</returns>
        public static bool IsSocketConnected(this TcpClient client)
        {
            return client != null && client.Client != null && client.Client.IsSocketConnected();
        }

        /// <summary>
        /// 更新并返回UdpClient的连接状态
        /// </summary>
        /// <returns>假如处于连接状态，返回true，否则返回false</returns>
        public static bool IsSocketConnected(this UdpClient client)
        {
            return client != null && client.Client != null && client.Client.IsSocketConnected();
        }

        /// <summary>
        /// 更新并返回TcpClient的连接状态
        /// </summary>
        /// <returns>假如处于连接状态，返回true，否则返回false</returns>
        public static bool IsSocketConnected(this Socket socket)
        {
            return (!socket.Poll(1000, SelectMode.SelectRead) || socket.Available != 0) && socket.Connected;
        }

        /// <summary>
        /// 获取Socket连接名称，格式：(本地终结点不为空)本地IP:端口->服务端IP:端口(远程终结点不为空)
        /// </summary>
        /// <param name="socket">套接字接口对象</param>
        /// <param name="remote">远程IP终结点</param>
        /// <param name="local">本地IP终结点</param>
        /// <returns></returns>
        public static string GetName(this Socket socket, out IPEndPoint remote, out IPEndPoint local)
        {
            remote = local = null;
            try { remote = (IPEndPoint)socket.RemoteEndPoint; } catch (Exception) { }
            try { local = (IPEndPoint)socket.LocalEndPoint; } catch (Exception) { }
            string name = (local == null ? string.Empty : local.ToString()) + (remote == null ? string.Empty : ("->" + remote.ToString()));
            return name;
        }

        /// <summary>
        /// 获取Socket连接名称，格式：(本地终结点不为空)本地IP:端口->服务端IP:端口(远程终结点不为空)
        /// </summary>
        /// <param name="socket">套接字接口对象</param>
        /// <returns></returns>
        public static string GetName(this Socket socket)
        {
            //IPEndPoint remote, local;
            return GetName(socket, out _, out _);
        }

        /// <summary>
        /// 泛型类的扩展方法，使用双缓存（适用于DataGridView / ListView等
        /// </summary>
        /// <typeparam name="T">欲扩展方法的类型</typeparam>
        /// <param name="obj">泛型对象，对泛型进行扩展</param>
        /// <param name="setting">是否启用双缓存</param>
        public static void SetDoubleBuffered<T>(this T obj, bool setting)
        {
            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            propertyInfo.SetValue(obj, setting, null);
        }
    }
}
