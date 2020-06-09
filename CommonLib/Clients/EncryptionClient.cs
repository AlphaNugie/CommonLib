using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Clients
{
    /// <summary>
    /// 加密功能类
    /// </summary>
    public class EncryptionClient
    {
        #region 属性
        /// <summary>
        /// DES加密密钥
        /// </summary>
        public string DesKey_64 { get; private set; } = "Lolipops";

        /// <summary>
        /// DES加密向量
        /// </summary>
        public string DesIV_64 { get; private set; } = "HotCandy";

        /// <summary>
        /// AES加密密钥
        /// </summary>
        public string AesKey_64 { get; private set; } = "LolipopHardCandyIceCreamJamCrack";

        /// <summary>
        /// AES加密向量
        /// </summary>
        public string AesIV_64 { get; private set; } = "SweetsCookieDoll";
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        public EncryptionClient() { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="desKey">DES加密密钥</param>
        /// <param name="desIv">DES加密向量</param>
        /// <param name="aesKey">AES加密密钥</param>
        /// <param name="aesIv">AES加密向量</param>
        public EncryptionClient(string desKey, string desIv, string aesKey, string aesIv)
        {
            this.DesKey_64 = desKey;
            this.DesIV_64 = desIv;
            this.AesKey_64 = aesKey;
            this.AesIV_64 = aesIv;
        }

        /// <summary>
        /// 将字符串转换为MD5哈希值
        /// </summary>
        /// <param name="inputString">待转换字符串</param>
        /// <returns>返回字符串的MD5哈希值</returns>
        public static string StringToMD5Hah(string inputString)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < encryptedBytes.Length; i++)
                    stringBuilder.AppendFormat("{0:X2}", encryptedBytes[i]);
                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// AES加密字符串，按默认密钥与加密向量
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public string EncryptAES(string encryptString)
        {
            return EncryptAES(encryptString, this.AesKey_64, this.AesIV_64);
        }

        /// <summary>
        /// AES解密字符串，按默认密钥与加密向量
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <returns></returns>
        public string DecryptAES(string decryptString)
        {
            return DecryptAES(decryptString, this.AesKey_64, this.AesIV_64);
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public string EncryptDES(string encryptString)
        {
            return EncryptDES(encryptString, this.DesKey_64, this.DesIV_64);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public string DecryptDES(string decryptString)
        {
            return DecryptDES(decryptString, this.DesKey_64, this.DesIV_64);
        }

        #region static
        /// <summary>
        /// AES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥，要求为32位</param>
        /// <param name="encryptIV">加密向量，要求为16位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptAES(string encryptString, string encryptKey, string encryptIV)
        {
            try
            {
                byte[] byKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32));
                byte[] byIV = Encoding.UTF8.GetBytes(encryptIV.Substring(0, 16));
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = byKey;
                    aesAlg.IV = byIV;
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    MemoryStream mStream = new MemoryStream();
                    using (CryptoStream cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                        string s = Convert.ToBase64String(mStream.ToArray());
                        return Convert.ToBase64String(mStream.ToArray());
                    }
                }
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// AES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥，要求为32位，和加密密钥相同</param>
        /// <param name="decryptIV">解密向量，要求为16位，和加密向量相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptAES(string decryptString, string decryptKey, string decryptIV)
        {
            try
            {
                byte[] byKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 32));
                byte[] byIV = Encoding.UTF8.GetBytes(decryptIV.Substring(0, 16));
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = byKey;
                    aesAlg.IV = byIV;
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    MemoryStream mStream = new MemoryStream(inputByteArray);
                    CryptoStream cStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read);
                    using (StreamReader sReader = new StreamReader(cStream))
                    {
                        string decryptedText = sReader.ReadToEnd();
                        return decryptedText;
                    }
                }
            }
            catch
            {
                return decryptString;
            }
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <param name="encryptIV">加密向量，要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey, string encryptIV)
        {
            try
            {
                byte[] byKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] byIV = Encoding.UTF8.GetBytes(encryptIV.Substring(0, 8));
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                using (DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider())
                {
                    MemoryStream mStream = new MemoryStream();
                    using (CryptoStream cStream = new CryptoStream(mStream, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                        return Convert.ToBase64String(mStream.ToArray());
                    }
                }
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥，要求为8位，和加密密钥相同</param>
        /// <param name="decryptIV">解密向量，要求为8位，和加密向量相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey, string decryptIV)
        {
            try
            {
                byte[] byKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                byte[] byIV = Encoding.UTF8.GetBytes(decryptIV.Substring(0, 8));
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                using (DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider())
                {
                    MemoryStream mStream = new MemoryStream();
                    using (CryptoStream cStream = new CryptoStream(mStream, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                        return Encoding.UTF8.GetString(mStream.ToArray());
                    }
                }
            }
            catch
            {
                return decryptString;
            }
        }
        #endregion
    }
}
