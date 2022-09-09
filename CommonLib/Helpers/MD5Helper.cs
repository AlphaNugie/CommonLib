using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Helpers
{
    /// <summary>
    /// MD5操作工具类
    /// </summary>
    public static class MD5Helper
    {
        /// <summary>
        /// 计算指定文件的MD5值
        /// </summary>
        /// <param name="fileName">指定文件的路径</param>
        /// <returns></returns>
        public static string CreateMD5(string fileName)
        {
            string hashStr;
            try
            {
                FileStream stream = new FileStream(
                    fileName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);
                byte[] hash;
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider()) { hash = md5.ComputeHash(stream); }
                hashStr = HexHelper.ByteArray2HexString(hash);
                stream.Close();
                stream.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return hashStr;
        }

        /// <summary>
        /// 根据流计算MD5值
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string CreateMD5(Stream stream)
        {
            byte[] hash;
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider()) { hash = md5.ComputeHash(stream); }
            return HexHelper.ByteArray2HexString(hash);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string CreateMD5(byte[] buffer, int offset, int count)
        {
            byte[] hash;
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider()) { hash = md5.ComputeHash(buffer, offset, count); }
            //MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //byte[] hash = md5.ComputeHash(buffer, offset, count);
            return HexHelper.ByteArray2HexString(hash);
        }

        //private static string ByteArrayToHexString(byte[] values)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (byte value in values)
        //    {
        //        sb.AppendFormat("{0:X2}", value);
        //    }
        //    return sb.ToString();
        //}
    }
}
