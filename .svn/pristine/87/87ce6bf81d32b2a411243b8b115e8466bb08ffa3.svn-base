using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;

namespace  CWI.MCP.Common
{
    public class MD5CryptionUtil
    {
        #region 公有方法

        /// <summary>
        /// MD5 签名
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="inputCharset">编码格式, 默认utf-8 格式</param>
        /// <returns>签名后字符串</returns>
        public static string Sign(string prestr, string key, string inputCharset = "utf-8")
        {
            StringBuilder sb = new StringBuilder(32);
            MD5 md5 = new MD5CryptoServiceProvider();

            prestr = prestr + key;
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(inputCharset).GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }


        /// <summary>
        /// MD5 签名
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="inputCharset">编码格式, 默认utf-8 格式</param>
        /// <returns>签名后字符串</returns>
        public static string Sign(string prestr,  string inputCharset = "utf-8")
        {
            StringBuilder sb = new StringBuilder(32);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(inputCharset).GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }
        /// <summary>
        /// MD5 验证签名
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="sign">MD5签名字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="inputCharset">编码格式，默认utf-8</param>
        /// <returns>验证成功 or 失败</returns>
        public static bool Verify(string prestr, string sign, string key, string inputCharset = "utf-8")
        {
            string mysign = Sign(prestr, key, inputCharset).ToUpper();
            return mysign == sign;
        }


        /// <summary>
        /// MD5 验证签名
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="sign">MD5签名字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="inputCharset">编码格式，默认utf-8</param>
        /// <returns>验证成功 or 失败</returns>
        public static bool Verify(string prestr, string sign, string inputCharset = "utf-8")
        {
            string mysign = Sign(prestr, inputCharset).ToUpper();
            LogUtil.Info(string.Format("签名验证：生成的签名：{0},传过来的签名{1},用于生成签名的数据：{2}",mysign,sign,prestr));
            return mysign == sign;
        }

        /// <summary>
        /// 获取文件的md5摘要
        /// </summary>
        /// <param name="sFile">文件流</param>
        /// <returns>MD5摘要结果</returns>
        public static string GetAbstractToMD5(Stream sFile)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(sFile);
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取文件的md5摘要
        /// </summary>
        /// <param name="dataFile">文件流</param>
        /// <returns>MD5摘要结果</returns>
        public static string GetAbstractToMD5(byte[] dataFile)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(dataFile);
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        #endregion
    }
}
