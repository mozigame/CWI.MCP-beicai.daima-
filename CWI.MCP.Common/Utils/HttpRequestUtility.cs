//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/06        创建
//---------------------------------------------

using System;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// HttpRequest模拟请求辅助类
    /// </summary>
    public class HttpRequestUtility
    {
        #region 声明及构造函数
        /// <summary>
        /// 私有实例
        /// </summary>
        private static HttpRequestUtility _instance = new HttpRequestUtility();

        /// <summary>
        /// 唯一实例
        /// </summary>
        public static HttpRequestUtility Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private HttpRequestUtility()
        {
        }
        #endregion

        /// <summary>
        /// Cookie容器
        /// </summary>
        private static CookieContainer _cookies = new CookieContainer();

        /// <summary>
        /// Post数据到指定的URL
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="parameters">参数集合</param>
        /// <param name="isSetNewSessionID">是否生成新的SessionID</param>
        /// <returns>数据Post的返回结果</returns>
        public string PostDataToUrl(string url, Dictionary<string, string> parameters, bool isSetNewSessionID)
        {
            string responseData = string.Empty;

            //Init http发送
            HttpWebRequest request = null;
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            request.UserAgent = string.Empty;
            request.Timeout = 200000;
            if (isSetNewSessionID)
            {
                request.CookieContainer = new CookieContainer();
            }
            else
            {
                request.CookieContainer = _cookies;
            }
            Encoding encoding = Encoding.GetEncoding("utf-8");

            StreamReader responseReader = null;
            Stream responseStream = null;

            //POST数据
            string buffer = string.Empty;
            if (parameters != null)
            {
                buffer = parameters.Aggregate(buffer, (current, keyValuePair) => current + (HttpUtility.UrlEncode(keyValuePair.Key, encoding) + "=" + HttpUtility.UrlEncode(keyValuePair.Value, encoding) + "&"));
            }

            buffer = buffer.TrimEnd('&');

            byte[] data = encoding.GetBytes(buffer);

            request.ContentLength = data.Length;

            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                responseStream = request.GetResponse().GetResponseStream();
                responseReader = new StreamReader(responseStream);
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("URL :" + url + "，请检查是否可以正常访问!" + ex.ToString());
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (responseReader != null)
                {
                    responseReader.Close();
                }
                if (isSetNewSessionID)
                {
                    _cookies = request.CookieContainer;
                }
            }

            return responseData;
        }

        /// <summary>
        /// 以GET方式请求URL，并获取结果
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="parameters">参数集合</param>
        /// <param name="isSetNewSessionID">是否生成新的SessionID</param>
        /// <returns>返回结果</returns>
        public string GetUrl(string url, Dictionary<string, string> parameters, bool isSetNewSessionID)
        {
            string responseData = string.Empty;

            //GET参数
            string getParams = string.Empty;
            if (parameters != null)
            {
                getParams = parameters.Aggregate(getParams, (current, keyValuePair) => current + (keyValuePair.Key + "=" + keyValuePair.Value + "&"));
            }
            getParams = getParams.TrimEnd('&');

            //Init http发送
            HttpWebRequest request = null;
            request = WebRequest.Create(url + "?" + getParams) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = string.Empty;
            request.Timeout = 200000;
            if (isSetNewSessionID)
            {
                request.CookieContainer = new CookieContainer();
            }
            else
            {
                request.CookieContainer = _cookies;
            }

            StreamReader responseReader = null;
            Stream responseStream = null;

            try
            {
                WebResponse response = request.GetResponse();
                responseStream = response.GetResponseStream();
                responseReader = new StreamReader(responseStream);
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("URL :" + url + "，请检查是否可以正常访问!" + ex.ToString());
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (responseReader != null)
                {
                    responseReader.Close();
                }
                if (isSetNewSessionID)
                {
                    _cookies = request.CookieContainer;
                }
            }

            return responseData;
        }

        /// <summary>
        /// 获取请求参数键值对
        /// </summary>
        /// <param name="request">请求参数字符传</param>
        /// <returns>请求参数键值对</returns>
        public static NameValueCollection GetNameValueCollection(string request)
        {
            NameValueCollection nv = new NameValueCollection();
            if (!string.IsNullOrWhiteSpace(request))
            {
                List<string> kvs = request.Trim().Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var kv in kvs)
                {
                    string[] values = kv.Trim().Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    string key = values[0];
                    string value = values.Length >= 2 ? values[1] : string.Empty;

                    if (!string.IsNullOrWhiteSpace(key) && !nv.AllKeys.Contains(key))
                    {
                        nv.Add(key, HttpUtility.UrlDecode(value));
                    }
                }
            }
            return nv;
        }

        /// <summary>
        /// 获取请求参数键值对
        /// </summary>
        /// <param name="request">请求参数字符传</param>
        /// <returns>请求参数键值对</returns>
        public static NameValueCollection GetNameValueCollection(string request, out SortedDictionary<string, object> sortDics, string signKey = "sign")
        {
            var nv = new NameValueCollection();
            sortDics = new SortedDictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(request))
            {
                List<string> kvs = request.Trim().Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var kv in kvs)
                {
                    string[] values = kv.Trim().Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    string key = values[0];
                    string value = values.Length >= 2 ? values[1] : string.Empty;

                    if (!string.IsNullOrWhiteSpace(key) && !nv.AllKeys.Contains(key))
                    {
                        nv.Add(key, HttpUtility.UrlDecode(value));

                        if (key.ToLower() != signKey.ToLower())
                        {
                            sortDics.Add(key, value);
                        }
                    }
                }
            }
            return nv;
        }
    }
}
