using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NETCoreBase.Common.Helpers
{
    public static class CryptHelper
    {
        private static IConfiguration _configuration;

        private static List<int> AES_CRYPT_LENGTH = new List<int>() {
            128, 192, 256
        };

        static CryptHelper()
        {
            CryptHelper._configuration = JsonConfigurationExtensions.AddJsonFile(
                new ConfigurationBuilder(), "appsettings.json", true, true).Build();
        }


        public static string DecryptByAes(string base64String)
        {
            string str = String.Empty;
            if (!string.IsNullOrWhiteSpace(base64String))
            {
                string str1 = ConfigurationBinder.Get<string>(CryptHelper._configuration.GetSection("AesKey"));
                string str2 = ConfigurationBinder.Get<string>(CryptHelper._configuration.GetSection("AesIV"));
                CryptHelper.ValidateKeyIVLength(str1, str2);
                Aes aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cryptoTransform = aes.CreateDecryptor(Encoding.UTF8.GetBytes(str1), Encoding.UTF8.GetBytes(str2));
                byte[] numArray = null;
                byte[] numArray1 = null;
                try
                {
                    numArray = Convert.FromBase64String(base64String);
                    numArray1 = cryptoTransform.TransformFinalBlock(numArray, 0, (int)numArray.Length);
                }
                catch (Exception exception)
                {
                    throw new Exception(string.Concat("解密出錯:", exception.Message));
                }
                str = Encoding.UTF8.GetString(numArray1);
            }
            return str;
        }

        public static string EncryptByAes(string text)
        {
            string base64String = String.Empty;
            if (!string.IsNullOrWhiteSpace(text))
            {
                string str = ConfigurationBinder.Get<string>(CryptHelper._configuration.GetSection("AesKey"));
                string str1 = ConfigurationBinder.Get<string>(CryptHelper._configuration.GetSection("AesIV"));
                CryptHelper.ValidateKeyIVLength(str, str1);
                Aes aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cryptoTransform = aes.CreateEncryptor(Encoding.UTF8.GetBytes(str), Encoding.UTF8.GetBytes(str1));
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                byte[] numArray = cryptoTransform.TransformFinalBlock(bytes, 0, (int)bytes.Length);
                base64String = Convert.ToBase64String(numArray);
            }
            return base64String;
        }

        public static string HashAu4A83(string au4a83)
        {
            string base64String;
            if (!string.IsNullOrWhiteSpace(au4a83))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(au4a83);
                byte[] numArray = (new SHA1CryptoServiceProvider()).ComputeHash(bytes, 0, (int)bytes.Length);
                base64String = Convert.ToBase64String(numArray);
            }
            else
            {
                base64String = au4a83;
            }
            return base64String;
        }

        private static void ValidateKeyIVLength(string key, string iv)
        {
            int length = (int)Encoding.UTF8.GetBytes(key).Length * 8;
            int num = (int)Encoding.UTF8.GetBytes(iv).Length * 8;
            if (!AES_CRYPT_LENGTH.Contains(length) || !AES_CRYPT_LENGTH.Contains(num))
            {
                throw new Exception(string.Format("key或iv的長度不在128bits、192bits、256bits其中一個，輸入的key bits:{0},iv bits:{1}", length, num));
            }
        }
    }
}