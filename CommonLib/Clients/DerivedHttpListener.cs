using CommonLib.Enums;
using CommonLib.Events;
using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Function.TimerEventRaiser;

namespace CommonLib.Clients
{
    /// <summary>
    /// HTTP监听服务端
    /// </summary>
    public class DerivedHttpListener
    {
        #region 事件
        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event DataReceivedEventHandler DataReceived;

        /// <summary>
        /// 服务状态改变事件
        /// </summary>
        public event ServiceStateEventHandler ServiceStateChanged;
        #endregion
        #region 私有成员变量
        private readonly TimerEventRaiser _raiser = new TimerEventRaiser(1000);
        #endregion
        #region 属性
        /// <summary>
        /// HttpListener对象
        /// </summary>
        public HttpListener BaseListener { get; private set; }

        /// <summary>
        /// HTTP监听的地址
        /// </summary>
        public string IpAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// HTTP服务的访问端口（默认8080）
        /// </summary>
        public int Port { get; set; } = 8080;

        /// <summary>
        /// 地址后缀
        /// </summary>
        public string Suffix { get; set; } = "/";

        /// <summary>
        /// HTTP监听服务的名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 有新请求时生成的全局唯一标识符
        /// </summary>
        public string GUID { get; private set; }

        /// <summary>
        /// 网页浏览器访问的返回信息
        /// </summary>
        public string WebExplorerMessage { get; set; }

        /// <summary>
        /// 最新的错误消息
        /// </summary>
        public string LastErrorMessage { get; set; }

        /// <summary>
        /// 接收缓冲区大小（字节数）
        /// </summary>
        public int ReceiveBufferSize { get; set; } = 32768;
        #endregion

        /// <summary>
        /// 以默认端口号初始化HTTP监听服务端
        /// </summary>
        public DerivedHttpListener() : this(null, 0, null) { }

        /// <summary>
        /// 以指定端口号初始化HTTP监听服务端
        /// </summary>
        /// <param name="ip">监听服务的地址</param>
        /// <param name="port">假如端口号不大于0，以默认端口号启动</param>
        /// <param name="suffix">地址后缀</param>
        public DerivedHttpListener(string ip, int port, string suffix)
        {
            if (!string.IsNullOrWhiteSpace(ip))
                IpAddress = ip;
            if (port > 0)
                Port = port;
            if (!string.IsNullOrWhiteSpace(suffix))
                Suffix = suffix;
            WebExplorerMessage = string.Empty;
            LastErrorMessage = string.Empty;
            //BaseListener = new HttpListener(); //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
            //BaseListener.Prefixes.Add($"http://+:{Port}/"); //定义url及端口号，通常设置为配置文件

            _raiser.RaiseThreshold = 10000;
            _raiser.RaiseInterval = 5000;
            _raiser.ThresholdReached += new ThresholdReachedEventHandler(Raiser_ThresholdReached);
        }

        private void Raiser_ThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            //接收超时的处理方法
        }

        /// <summary>
        /// 启动监听器
        /// </summary>
        public int Start()
        {
            int result = 1;
            try
            {
                if (BaseListener == null)
                {
                    BaseListener = new HttpListener(); //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
                    //配置监听地址。+代表本机可能的IP，如localhost、127.0.0.1、192.168.199.X(本机IP)等，假如失败则以管理员方式启动
                    //string address = $"http://{IpAddress}:{Port}/web/";
                    string address = $"http://+:{Port}{Suffix}";
                    BaseListener.Prefixes.Clear();
                    BaseListener.Prefixes.Add(address); //定义url及端口号，通常设置为配置文件
                    BaseListener.Start();
                    Name = address;
                }
                //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
                //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
                BaseListener.BeginGetContext(Result, null);
                _raiser.Run();
                if (BaseListener.IsListening)
                    ServiceStateChanged?.BeginInvoke(this, new ServiceStateEventArgs($"HTTP监听服务{Name}已启动", ServiceState.Started), null, null);
                //Console.WriteLine($"服务端初始化完毕，正在等待客户端请求,时间：{DateTime.Now.ToString()}\r\n");
                //Console.ReadKey();
            }
            catch (Exception e)
            {
                LastErrorMessage = $"HTTP监听服务{Name}启动过程中出现错误：{e.Message}";
                result = 0;
                Stop();
                //throw;
            }
            return result;
        }

        /// <summary>
        /// 停止监听器
        /// </summary>
        public int Stop()
        {
            int result = 1;
            try
            {
                if (BaseListener != null)
                {
                    BaseListener.Stop();
                    BaseListener = null;
                }
                _raiser.Stop();
                //if (!BaseListener.IsListening)
                    ServiceStateChanged?.BeginInvoke(this, new ServiceStateEventArgs($"HTTP监听服务{Name}已停止", ServiceState.Stopped), null, null);
            }
            catch (ObjectDisposedException e)
            {
                LastErrorMessage = $"HTTP监听服务{Name}已被释放：{e.Message}";
                result = 0;
                BaseListener = null;
            }
            catch (Exception e)
            {
                LastErrorMessage = $"HTTP监听服务{Name}停止过程中出现错误：{e.Message}";
                result = 0;
                throw;
            }
            return result;
        }

        /// <summary>
        /// 异步监听回调
        /// </summary>
        /// <param name="ar"></param>
        private void Result(IAsyncResult ar)
        {
            if (BaseListener == null || !BaseListener.IsListening)
                return;
            //当接收到请求后程序流会走到这里

            //继续异步监听
            BaseListener.BeginGetContext(Result, null);
            GUID = Guid.NewGuid().ToString();
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine($"接到新的请求:{GUID},时间：{DateTime.Now.ToString()}");
            //获得context对象
            var context = BaseListener.EndGetContext(ar);
            var request = context.Request;
            var response = context.Response;
            ////如果是js的ajax请求，还可以设置跨域的ip地址与参数
            //context.Response.AppendHeader("Access-Control-Allow-Origin", "*"); //后台跨域请求，通常设置为配置文件
            //context.Response.AppendHeader("Access-Control-Allow-Headers", "ID,PW"); //后台跨域参数设置，通常设置为配置文件
            //context.Response.AppendHeader("Access-Control-Allow-Method", "post"); //后台跨域请求设置，通常设置为配置文件
            context.Response.ContentType = "text/plain;charset=UTF-8"; //告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
            context.Response.AddHeader("Content-type", "text/plain"); //添加响应头信息
            context.Response.ContentEncoding = Encoding.UTF8;
            //定义返回客户端的信息，当请求为POST方式时返回处理过的信息，否则判断可能为网页访问、返回指定消息
            string returnObj = request.HttpMethod == "POST" && request.InputStream != null ? HandleRequest(request, response) : WebExplorerMessage;
            //string returnObj = null; //定义返回客户端的信息
            //if (request.HttpMethod == "POST" && request.InputStream != null)
            //    returnObj = HandleRequest(request, response); //处理客户端发送的请求并返回处理信息
            //else
            //    returnObj = WebExplorerMessage;
            var returnByteArr = Encoding.UTF8.GetBytes(returnObj); //设置客户端返回信息的编码
            try
            {
                using (var stream = response.OutputStream)
                {
                    //把处理信息返回到客户端
                    stream.Write(returnByteArr, 0, returnByteArr.Length);
                }
            }
            catch (Exception ex)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"网络蹦了：{ex.ToString()}");
                LastErrorMessage = $"网络蹦了：{ex.ToString()}";
            }
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine($"请求处理完成：{GUID},时间：{ DateTime.Now.ToString()}\r\n");
        }

        /// <summary>
        /// 处理HTTP请求并设定对请求的响应
        /// </summary>
        /// <param name="request">HTTP请求的对象</param>
        /// <param name="response">对HTTP请求的响应的对象</param>
        /// <returns></returns>
        private string HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            //string data;
            try
            {
                var byteList = new List<byte>();
                var byteArr = new byte[ReceiveBufferSize];
                int readLen = 0;
                int len = 0;
                //接收客户端传过来的数据并转成字符串类型
                do
                {
                    readLen = request.InputStream.Read(byteArr, 0, byteArr.Length);
                    len += readLen;
                    byteList.AddRange(byteArr);
                } while (readLen != 0);
                //处理byte序列的长度
                if (byteList.Count > len)
                    byteList.RemoveRange(len, byteList.Count - len);
                DataReceived?.BeginInvoke(this, new DataReceivedEventArgs(byteList.ToArray()), null, null);
                _raiser.Click();
                //data = Encoding.UTF8.GetString(byteList.ToArray(), 0, len);
            }
            catch (Exception ex)
            {
                LastErrorMessage = $"处理请求的过程中出现错误：{ex.ToString()}";
                response.StatusDescription = LastErrorMessage;
                response.StatusCode = 404;
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"在接收数据时发生错误:{ex.ToString()}");
                return response.StatusDescription; //把服务端错误信息直接返回可能会导致信息不安全，此处仅供参考
            }
            response.StatusDescription = "OK"; //获取或设置返回给客户端的 HTTP 状态代码的文本说明。
            response.StatusCode = 200; // 获取或设置返回给客户端的 HTTP 状态代码。
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine($"接收数据完成:{data.Trim()},时间：{DateTime.Now.ToString()}");
            return response.StatusDescription;
        }

        ///// <summary>
        ///// 测试HTTP服务端
        ///// </summary>
        ///// <param name="args"></param>
        //static void Main(string[] args)
        //{
        //    string operation;
        //    do
        //    {
        //        Console.WriteLine("按任意键发送数据到服务端");
        //        Console.ReadLine();
        //        var wc = new WebClient();
        //        var url = "http://127.0.0.1:8080";
        //        Console.WriteLine($"请求服务地址:{url}，时间：{DateTime.Now.ToString()}");
        //        //模拟一个json数据发送到服务端
        //        var data = new Data(1, "张三");
        //        var jsonModel = JsonConvert.SerializeObject(data);
        //        //发送到服务端并获得返回值
        //        var returnInfo = wc.UploadData(url, Encoding.UTF8.GetBytes(jsonModel));
        //        //把服务端返回的信息转成字符串
        //        var str = Encoding.UTF8.GetString(returnInfo);
        //        Console.ForegroundColor = ConsoleColor.Cyan;
        //        Console.WriteLine($"服务端返回信息：{str},时间：{DateTime.Now.ToString()}");
        //        Console.ForegroundColor = ConsoleColor.White;
        //        Console.WriteLine($"请问是否继续：继续 【y】,退出【n】");
        //        operation = Console.ReadLine();
        //    } while (operation == "y");
        //}
    }

    //class Data
    //{
    //    public Data(int id, string name)
    //    {
    //        this.ID = id;
    //        this.Name = name;
    //    }
    //    public int ID { get; set; }

    //    public string Name { get; set; }
    //}
}
