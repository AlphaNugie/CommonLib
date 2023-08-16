using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;

namespace CommonLib.Clients
{
    /// <summary>
    /// HTTP发送客户端
    /// </summary>
    public class DerivedHttpClient
    {
        private/* static*/ readonly HttpClient client = new HttpClient();
        
        /// <summary>
        /// 执行任务时的等待时间（毫秒，超过此时间报Timeout异常）
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public DerivedHttpClient() : this(5000) { }

        /// <summary>
        /// 用给定的等待时间（毫秒）初始化
        /// </summary>
        /// <param name="timeout">执行任务时的等待时间（毫秒，超过此时间报Timeout异常）</param>
        public DerivedHttpClient(int timeout)
        {
            Timeout = timeout;
        }

        /// <summary>
        /// 验证授权
        /// </summary>
        /// <param name="token">用于验证授权的token</param>
        public/* static*/ void OAuth(string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public/* static*/ string Get(string url)
        {
            url = FixUrl(url);
            Task<HttpResponseMessage> GetTask = client.GetAsync(url);
            try
            {
                return RunTask(GetTask);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 发送POST请求，并在HTTP内容标头中指定Content-Type
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public string Post(string url, string content, ContentType contentType)
        {
            url = FixUrl(url);
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            MemoryStream memory = new MemoryStream(byteArray);
            StreamContent contentStream = new StreamContent(memory);
            //contentStream.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string type = string.Empty;
            switch (contentType)
            {
                case ContentType.FormUrlEncoded:
                    type = "application/x-www-form-urlencoded";
                    break;
                case ContentType.Xml:
                    type = "application/xml";
                    break;
                case ContentType.Json:
                    type = "application/json";
                    break;
                case ContentType.FormData:
                    type = "multipart/form-data";
                    break;
            }
            contentStream.Headers.Add("Content-Type", type);
            Task<HttpResponseMessage> PostTask = client.PostAsync(url, contentStream);
            try
            {
                return RunTask(PostTask);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 发送POST请求，HTTP内容标头的Content-Type指定为FormUrlEncoded
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string Post(string url, string content)
        {
            return Post(url, content, ContentType.FormUrlEncoded);
        }

        //public/* static*/ string Post(string url, string content)
        //{
        //    url = FixUrl(url);
        //    byte[] byteArray = Encoding.UTF8.GetBytes(content);
        //    MemoryStream memory = new MemoryStream(byteArray);
        //    StreamContent contentStream = new StreamContent(memory);
        //    contentStream.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        //    Task<HttpResponseMessage> PostTask = client.PostAsync(url, contentStream);
        //    try
        //    {
        //        return RunTask(PostTask);
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="url">待向其发送请求的URL地址</param>
        /// <param name="content"></param>
        /// <returns></returns>
        public/* static*/ string Post(string url, Dictionary<string, string> content)
        {
            url = FixUrl(url);
            Task<HttpResponseMessage> PostTask = client.PostAsync(url, new FormUrlEncodedContent(content));
            try
            {
                return RunTask(PostTask);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 发送PUT请求
        /// </summary>
        /// <param name="url">待向其发送请求的URL地址</param>
        /// <returns></returns>
        public/* static*/ string Put(string url)
        {
            url = FixUrl(url);
            Task<HttpResponseMessage> PutTask = client.PutAsync(url, null);
            try
            {
                return RunTask(PutTask);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 发送PUT请求
        /// </summary>
        /// <param name="url">待向其发送请求的URL地址</param>
        /// <param name="content">发送的请求内容</param>
        /// <returns></returns>
        public/* static*/ string Put(string url, Dictionary<string, string> content)
        {
            url = FixUrl(url);
            Task<HttpResponseMessage> PutTask = client.PutAsync(url, new FormUrlEncodedContent(content));
            try
            {
                return RunTask(PutTask);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 发送删除请求
        /// </summary>
        /// <param name="url">待向其发送请求的URL地址</param>
        /// <returns></returns>
        public/* static*/ string Delete(string url)
        {
            url = FixUrl(url);
            Task<HttpResponseMessage> DeleteTask = client.DeleteAsync(url);
            try
            {
                return RunTask(DeleteTask);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 异步执行发送消息并获取响应的任务，响应时间为5秒
        /// </summary>
        /// <param name="task">待执行的任务</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        private/* static*/ string RunTask(Task<HttpResponseMessage> task)
        {
            bool intime; //是否在响应时间内完成
            //try { intime = task.Wait(5000); }
            try { intime = task.Wait(Timeout); }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                throw e;
            }
            if (!intime)
                throw new TimeoutException("Timeout");
            else
            {
                HttpContent result = task.Result.Content;
                return result.ReadAsStringAsync().Result;
            }
        }

        /// <summary>
        /// 假如给出的URL没有http开头，则添加在URL前添加http://
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private/* static*/ string FixUrl(string url)
        {
            return !url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? "http://" + url : url;
            //if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            //{
            //    return "http://" + url;
            //}
            //else
            //{
            //    return url;
            //}
        }
    }

    /// <summary>
    /// POST请求的Content-Type
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// application/x-www-form-urlencoded
        /// </summary>
        FormUrlEncoded,

        /// <summary>
        /// application/xml
        /// </summary>
        Xml,

        /// <summary>
        /// application/json
        /// </summary>
        Json,

        /// <summary>
        /// multipart/form-data
        /// </summary>
        FormData,

        ///// <summary>
        ///// application/x-www-form-urlencoded
        ///// </summary>
        //FormUrlEncoded = "application/x-www-form-urlencoded";

        ///// <summary>
        ///// application/xml
        ///// </summary>
        //Xml = "application/xml";

        ///// <summary>
        ///// application/json
        ///// </summary>
        //Json = "application/json";

        ///// <summary>
        ///// multipart/form-data
        ///// </summary>
        //FormData = "multipart/form-data";
    }
}