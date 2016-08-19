using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PFTSTools
{
    public class MD5Tool
    {
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="InStr"></param>
        /// <returns></returns>
        public static string GetEncryptCode(string InStr)
        {
            //MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
            //byte[] btEncrypt = Encoding.Unicode.GetBytes(InStr);
            ////加密数组
            //byte[] resultEncrypt = md5CSP.ComputeHash(btEncrypt);
            //string strEncrypt = Encoding.Unicode.GetString(resultEncrypt);
            ////作为密码方式加密
            //return FormsAuthentication.HashPasswordForStoringInConfigFile(strEncrypt, "MD5");

            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(InStr));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
