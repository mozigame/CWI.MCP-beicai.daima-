//版权信息：版权所有(C) 2015，COOLWI.COM
//变更历史：
//    姓名         日期          说明
//--------------------------------------------------------
//   王军锋    2012/03/28       创建

using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using Evt.Framework.Common;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// SecurityUtil
    /// </summary>
    public static class SecurityUtil
    {
        #region MD5加密

        /// <summary>
        /// 获取MD5加密后Hash字符串
        /// </summary>
        /// <param name="strOriginal">初始字符串</param>
        /// <returns>MD5加密后Hash字符串</returns>
        public static string GetMd5Hash(string strOriginal)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(strOriginal));
            StringBuilder sbList = new StringBuilder();
            foreach (byte d in data)
            {
                sbList.Append(d.ToString("x2"));
            }
            return sbList.ToString();
        }

        /// <summary>
        /// 获取MD5及Salt加密后Hash字符串
        /// </summary>
        /// <param name="strOriginal">初始字符串</param>
        /// <param name="strSalt">Salte种子字符串</param>
        /// <returns>MD5加密后Hash字符串</returns>
        public static string GetMd5Hash(string strOriginal, string strSalt)
        {
            //如果调用未给Salt值,则默认
            if (string.IsNullOrEmpty(strSalt))
            {
                strSalt = Consts.ENCRYPT_SALT;
            }
            strOriginal = strOriginal + strSalt;

            return GetMd5Hash(strOriginal);
        }

        /// <summary>
        /// 使用 MD5 对输入字符串进行加密
        /// </summary>
        /// <param name="inputString">需要加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public static string ConvertToMD5(string inputString)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(inputString, "MD5");
        }

        #endregion

        #region SHA 加解密

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string SHA1Encrypt(string inputString)
        {
            SHA1Managed sha1 = new SHA1Managed();
            return ByteToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(inputString))).ToLower();
        }

        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string SHA256Encrypt(string inputString)
        {
            SHA256Managed sha256 = new SHA256Managed();
            return ByteToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString))).ToLower();
        }

        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2");
            /* hex format */
            return sbinary;
        }

        #endregion

        #region DES 加解密

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="encryptString">待加密的明文</param>
        /// <returns>加密后的密文</returns>
        public static string DESEncrypt(string encryptString)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(encryptString);
            des.Key = ASCIIEncoding.ASCII.GetBytes(Consts.DES_KEY);
            des.IV = ASCIIEncoding.ASCII.GetBytes(Consts.DES_KEY);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="decryptString">待解密的密文</param>
        /// <returns>解密后的明文</returns>
        public static string DESDecrypt(string decryptString)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[decryptString.Length / 2];
            for (int x = 0; x < decryptString.Length / 2; x++)
            {
                int i = Convert.ToInt32(decryptString.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(Consts.DES_KEY);
            des.IV = ASCIIEncoding.ASCII.GetBytes(Consts.DES_KEY);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            return Encoding.ASCII.GetString(ms.ToArray());
        }

        /// <summary>
        /// 数据做DES加密
        /// </summary>
        /// <param name="data">输入数据</param>
        /// <returns>输出数据</returns>
        public static string DESEncode(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            byte[] byKey = ASCIIEncoding.ASCII.GetBytes(Consts.DES_KEY);
            byte[] byIV = ASCIIEncoding.ASCII.GetBytes(Consts.DES_IV);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();

            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cst);

            sw.Write(data);
            sw.Flush();

            cst.FlushFinalBlock();
            sw.Flush();

            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        /// 数据做DES解密
        /// </summary>
        /// <param name="data">输入数据</param>
        /// <returns>输出数据</returns>
        public static string DESDecode(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            byte[] byKey = ASCIIEncoding.ASCII.GetBytes(Consts.DES_KEY);
            byte[] byIV = ASCIIEncoding.ASCII.GetBytes(Consts.DES_IV);

            byte[] byEnc;

            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return string.Empty;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);

            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);

            return sr.ReadToEnd();
        }

        #endregion

        #region Base64 加解密

        /// <summary>
        /// Base64编码，使用指定的编码
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <param name="encoding">编码对象</param>
        /// <returns>Base64编码后的字符串</returns>
        public static string Base64Encode(string sourceString, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(sourceString));
        }

        /// <summary>
        /// Base64编码，使用默认的编码
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <returns>Base64编码后的字符串</returns>
        public static string Base64Encode(string sourceString)
        {
            return Base64Encode(sourceString, Encoding.Default);
        }

        /// <summary>
        /// Base64解码，使用指定的编码
        /// </summary>
        /// <param name="base64Encoded">已编码的Base64字符串</param>
        /// <param name="encoding">编码对象</param>
        /// <returns>原字符串</returns>
        public static string Base64Decode(string base64Encoded, Encoding encoding)
        {
            return encoding.GetString(Convert.FromBase64String(base64Encoded));
        }

        /// <summary>
        /// Base64解码,使用默认的编码
        /// </summary>
        /// <param name="base64Encoded">已编码的Base64字符串</param>
        /// <returns>原字符串</returns>
        public static string Base64Decode(string base64Encoded)
        {
            return Base64Decode(base64Encoded, Encoding.Default);
        }

        #endregion

        #region 算术16进制 加解密

        /// <summary>
        /// 算术16进制加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        public static void MathHexEncoder(ref string data, string key)
        {
            string result = string.Empty;

            byte[] tmpSource = ASCIIEncoding.ASCII.GetBytes(key);
            byte[] tempHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            string hash = System.BitConverter.ToString(tempHash).Replace("-", string.Empty);

            char[] ck = hash.ToCharArray();
            char[] cd = data.ToCharArray();
            for (int i = 0; i < cd.Count(); i++)
            {
                int id = Convert.ToInt16(cd[i].ToString(), 16);
                if (i < ck.Count())
                {
                    int ik = Convert.ToInt16(ck[i].ToString(), 16);
                    int ie = id + ik;
                    if (ie > 15)
                    {
                        ie -= 16;
                    }

                    result += Convert.ToString(ie, 16).ToUpper();
                }
                else
                {
                    result += cd[i].ToString();
                }
            }

            data = result;
        }

        /// <summary>
        /// 算术16进制解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        public static void MathHexDecoder(ref string data, string key)
        {
            string result = string.Empty;

            byte[] tmpSource = ASCIIEncoding.ASCII.GetBytes(key);
            byte[] tempHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            string hash = System.BitConverter.ToString(tempHash).Replace("-", string.Empty);

            char[] ck = hash.ToCharArray();
            char[] cd = data.ToCharArray();
            for (int i = 0; i < cd.Count(); i++)
            {
                int id = Convert.ToInt16(cd[i].ToString(), 16);
                if (i < ck.Count())
                {
                    int ik = Convert.ToInt16(ck[i].ToString(), 16);
                    int ie = id - ik;
                    if (ie < 0)
                    {
                        ie += 16;
                    }

                    result += Convert.ToString(ie, 16).ToUpper();
                }
                else
                {
                    result += cd[i].ToString();
                }
            }

            data = result;
        }

        #endregion

        #region 接口请求参数验证签名

        /// <summary>
        /// 接口请求参数验证签名
        /// </summary>
        /// <param name="data">请求Json格式参数</param>
        /// <param name="coolwiKey">酷外科技私钥</param>
        /// <param name="inputCharset">编码</param>
        /// <returns>签名成功 True，签名失败 False</returns>
        public static bool ValidateRequestSign(string data, string coolwiKey, string inputCharset = "utf-8")
        {
            try
            {
                string sign = string.Empty;
                SortedDictionary<string, string> rdata = JsonUtil.Deserialize<SortedDictionary<string, string>>(data);
                StringBuilder strQuest = new StringBuilder();
                foreach (var item in rdata)
                {
                    if (!item.Key.ToLower().Equals("sign"))
                    {
                        strQuest.AppendFormat("{0}={1}&", item.Key, item.Value);
                    }
                    else
                    {
                        sign = item.Value;
                    }
                }

                StringBuilder sb = new StringBuilder(32);
                string orignStr = strQuest.ToString().TrimEnd('&') + coolwiKey;
                return CompareMD5Key(orignStr, sign, inputCharset);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 接口请求参数验证签名
        /// </summary>
        /// <param name="questStr">请求Quest对象</param>
        /// <param name="coolwiKey">酷外科技私钥</param>
        /// <param name="inputCharset">字符编码</param>
        /// <returns>签名成功 True，签名失败 False</returns>
        public static bool ValidateRequestSign(NameValueCollection questStr, string coolwiKey, string inputCharset = "utf-8")
        {
            try
            {
                if (questStr == null || questStr.Count == 0) return true;
                SortedDictionary<string, string> rdata = new SortedDictionary<string, string>();
                string sign = string.Empty;
                foreach (string key in questStr.Keys)
                {
                    if (!key.ToLower().Equals("sign"))
                    {
                        rdata.Add(key, questStr[key]);
                    }
                    else
                    {
                        sign = questStr[key];
                    }
                }
                StringBuilder strQuest = new StringBuilder();

                foreach (var item in rdata)
                {
                    strQuest.AppendFormat("{0}={1}&", item.Key, item.Value);
                }
                string orignStr = strQuest.ToString().TrimEnd('&') + coolwiKey;
                return CompareMD5Key(orignStr, sign, inputCharset);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// MD5加密串比较
        /// </summary>
        /// <param name="orign">原字符串</param>
        /// <param name="input">输入字符串</param>
        /// <param name="inputCharset">编码</param>
        /// <returns>验证成功 True，验证失败 False</returns>
        public static bool CompareMD5Key(string orign, string input, string inputCharset)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(inputCharset).GetBytes(orign));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString().ToLower().Equals(input.ToLower());
        }

        #endregion
    }
}
