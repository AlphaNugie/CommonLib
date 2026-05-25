using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonLib.Extensions
{
    /// <summary>
    /// 类功能扩展类
    /// </summary>
    public static class ExtensionClass
    {
        #region 序列、数组、List
        /// <summary>
        /// 将源集合进行拷贝并返回拷贝后的List，假如谓词不为空，则将先以谓词进行筛选
        /// </summary>
        /// <typeparam name="T">元素的类型</typeparam>
        /// <param name="values">用于拷贝的源集合</param>
        /// <param name="predicate">用于测试每个元素是否满足条件的函数</param>
        /// <returns></returns>
        public static List<T> Copy<T>(this IEnumerable<T> values, Func<T, bool>
            //.net 9框架下使返回对象可为空
#if NET9_0_OR_GREATER
            ?
#endif
            predicate = null)
        {
            //if (values == null) return null;
            //if (predicate != null) values = values.Where(predicate);
            //return values.ToList();

            List<T> result =
#if NET45_OR_GREATER
                new List<T>();
#elif NET9_0_OR_GREATER
                [];
#endif
            if (values == null) goto END;
            if (predicate != null) values = values.Where(predicate);
            result =
#if NET45_OR_GREATER
                values.ToList();
#elif NET9_0_OR_GREATER
                [.. values];
#endif
        END:
            return result;
        }

        /// <summary>
        /// 将一组泛型序列从任意索引位置截取出4个元素
        /// </summary>
        /// <param name="numbers">提供的泛型序列</param>
        /// <param name="startIndex">计算的起始索引，从此索引开始找出4个元素</param>
        /// <typeparam name="T">提取的泛型序列的类型</typeparam>
        /// <returns></returns>
        internal static IEnumerable<T> Take4AfterSkip<T>(this IEnumerable<T> numbers, int startIndex = 0)
        {
            return numbers.TakeAfterSkip(startIndex, 4);
        }

        /// <summary>
        /// 将一组泛型序列从任意索引位置截取出8个元素
        /// </summary>
        /// <param name="numbers">提供的泛型序列</param>
        /// <param name="startIndex">计算的起始索引，从此索引开始找出8个元素</param>
        /// <typeparam name="T">提取的泛型序列的类型</typeparam>
        /// <returns></returns>
        internal static IEnumerable<T> Take8AfterSkip<T>(this IEnumerable<T> numbers, int startIndex = 0)
        {
            return numbers.TakeAfterSkip(startIndex, 8);
        }

        /// <summary>
        /// 将一组泛型序列从任意索引位置截取出任意个元素
        /// </summary>
        /// <param name="numbers">提供的泛型序列</param>
        /// <param name="startIndex">计算的起始索引，从此索引开始取出任意个元素</param>
        /// <param name="count">取出元素的数量</param>
        /// <typeparam name="T">提取的泛型序列的类型</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static IEnumerable<T> TakeAfterSkip<T>(this IEnumerable<T> numbers, int startIndex, int count)
        {
            //int len = numbers == null ? 0 : numbers.Count();
#if NET45_OR_GREATER
            if (numbers == null) numbers = new T[0];
#elif NET9_0_OR_GREATER
            numbers ??= [];
#endif
            int len = numbers.Count();
            if (len < startIndex + count)
                throw new ArgumentOutOfRangeException(nameof(numbers), $"提供的byte序列长度{numbers.Count()}不足以提供从{startIndex}开始的连续{count}个字节");
            //截取从起始索引开始的4个字节
            return numbers.Skip(startIndex).Take(count);
        }

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
#if NET45_OR_GREATER
            if (list == null)
                throw new ArgumentNullException(paramName: nameof(list));
            if (prop < 0)
                throw new ArgumentOutOfRangeException(paramName: nameof(prop));
#elif NET9_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(list);
            ArgumentOutOfRangeException.ThrowIfNegative(prop);
#endif
            int length = list.Count();
            int half = prop < 1 ? (int)Math.Floor((1 - prop) * length) : (int)Math.Floor(prop); //将被削减的元素的数量
            int most = length < half ? 0 : length - half; //最终保留的元素的数量（最终数量小于0时补为0）
            if (both_end)
                half /= 2;
            //int half = (int)Math.Floor((1 - prop) / 2 * length), most = (int)Math.Floor(prop * length);
            return list.Skip(half).Take(most);
        }
        #endregion

        #region 字符串
        /// <summary>
        /// 获取转换后的Unicode编码字符串
        /// <para/>假如源字符串为空引用或空白字符串，则返回string.Empty
        /// <para/>假如不存在Unicode编码则返回源字符串，否则返回将Unicode转义字符转换后的字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetUnicodeEncodedString(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            //Match match = Regex.Match(input, RegexMatcher.RegexPattern_UnicodeString);
            Match match = Regex.Match(input, RegexMatcher.RegexPattern_UnicodeString_VariableDigits);
            if (match == null || !match.Success)
                return input;
            //假如存在有2位16进制数，补充为4位
            string[] parts = match.Value.Split(
#if NET45_OR_GREATER
                new string[] { @"\u" },
#elif NET9_0_OR_GREATER
                [@"\u"],
#endif
                StringSplitOptions.RemoveEmptyEntries);
            string unicodeString = @"\u" + string.Join(@"\u", parts.Select(part => part.PadLeft(4, '0')));
            // 将Unicode转义序列转换为实际的Unicode字符
            string chineseString = Regex.Unescape(unicodeString);
            return chineseString;
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
        #endregion

        #region Socket
        /// <summary>
        /// 更新并返回TcpClient的连接状态
        /// </summary>
        /// <returns>假如处于连接状态，返回true，否则返回false</returns>
        public static bool IsSocketConnected(this TcpClient client)
        {
            try { return client != null && client.Client != null && client.Client.IsSocketConnected(); }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// 更新并返回UdpClient的连接状态
        /// </summary>
        /// <returns>假如处于连接状态，返回true，否则返回false</returns>
        public static bool IsSocketConnected(this UdpClient client)
        {
            try { return client != null && client.Client != null && client.Client.IsSocketConnected(); }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// 更新并返回TcpClient的连接状态
        /// </summary>
        /// <returns>假如处于连接状态，返回true，否则返回false</returns>
        public static bool IsSocketConnected(this Socket socket)
        {
            //try { return (!socket.Poll(1000, SelectMode.SelectRead) || socket.Available != 0) && socket.Connected; }
            try { return socket != null && socket.Connected && (socket.Available != 0 || !socket.Poll(1000, SelectMode.SelectRead)); }
            catch (Exception) { return false;}
        }

        /// <summary>
        /// 获取Socket连接名称，格式：(本地终结点不为空)本地IP:端口->服务端IP:端口(远程终结点不为空)
        /// </summary>
        /// <param name="socket">套接字接口对象</param>
        /// <param name="remote">远程IP终结点</param>
        /// <param name="local">本地IP终结点</param>
        /// <returns></returns>
#if NET45_OR_GREATER
        public static string GetName(this Socket socket, out IPEndPoint remote, out IPEndPoint local)
#elif NET9_0_OR_GREATER
        public static string GetName(this Socket socket, out IPEndPoint? remote, out IPEndPoint? local)
#endif
        {
            remote = local = null;
#if NET45_OR_GREATER
            try { remote = (IPEndPoint)socket.RemoteEndPoint; } catch (Exception) { }
            try { local = (IPEndPoint)socket.LocalEndPoint; } catch (Exception) { }
#elif NET9_0_OR_GREATER
            try { remote = (IPEndPoint?)socket.RemoteEndPoint; } catch (Exception) { }
            try { local = (IPEndPoint?)socket.LocalEndPoint; } catch (Exception) { }
#endif
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
        #endregion

        /// <summary>
        /// 泛型类的扩展方法，使用双缓存（适用于DataGridView / ListView等
        /// </summary>
        /// <typeparam name="T">欲扩展方法的类型</typeparam>
        /// <param name="obj">泛型对象，对泛型进行扩展</param>
        /// <param name="setting">是否启用双缓存</param>
#if NET45_OR_GREATER
        public static void SetDoubleBuffered<T>(this T obj, bool setting)
        {
            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            propertyInfo.SetValue(obj, setting, null);
        }
#elif NET9_0_OR_GREATER
        public static void SetDoubleBuffered<T>(this T obj, bool setting)
        {
            Type? type = obj?.GetType();
            PropertyInfo? propertyInfo = type?.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            propertyInfo?.SetValue(obj, setting, null);
        }
#endif
    }
}
