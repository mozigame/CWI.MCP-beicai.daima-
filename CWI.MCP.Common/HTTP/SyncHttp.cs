// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期          说明
// --------------------------------------------
//      王军锋     2014/08/08       创建
// --------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Web;

namespace CWI.MCP.Common
{
    public class SyncHttp
    {
        public static CookieContainer myCookie;

        /// <summary>
        /// 绑定Cookie、获取验证码
        /// </summary>
        /// <param name="strUrl">链接地址（验证码获取地址）</param>
        /// <param name="server">Referer地址</param>
        /// <returns>Stream:验证码的数据流</returns>
        public static Stream GetModelStream(string strUrl, string server)//, string proxy, bool isProxy)
        {
            System.IO.Stream resStream = null;
            string cookie;
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                httpRequest.Timeout = 2000;
                httpRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                httpRequest.Referer = server;
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; Maxthon; .NET CLR 1.1.4322)";
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                cookie = httpResponse.Headers.Get("Set-Cookie");
                myCookie = new CookieContainer();
                myCookie.SetCookies(new Uri(server), cookie);
                resStream = httpResponse.GetResponseStream();
            }
            catch { }
            return resStream;
        }
        /// <summary>
        /// Post方法提交
        /// </summary>
        /// <param name="strUrl">目的资源链接</param>
        /// <param name="strParm">传递的参数和值</param>
        /// <returns>String:Response返回值</returns>
        public static string PostModel(string strUrl, string server, string strParm)
        {
            Encoding encode = System.Text.Encoding.UTF8;
            byte[] arrB = encode.GetBytes(strParm);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(strUrl);
            myReq.Method = "POST";
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.Timeout = 2000;
            myReq.Referer = server;
            myReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; Maxthon; .NET CLR 1.1.4322)";
            myReq.ContentLength = arrB.Length;
            myReq.CookieContainer = myCookie;
            Stream outStream = myReq.GetRequestStream();
            outStream.Write(arrB, 0, arrB.Length);
            outStream.Close();
            WebResponse myResp = null;
            try
            {
                //接收HTTP做出的响应       
                myResp = myReq.GetResponse();
            }
            catch (Exception e)
            {
            }
            Stream ReceiveStream = myResp.GetResponseStream();
            StreamReader readStream = new StreamReader(ReceiveStream, encode);
            Char[] read = new Char[256];
            int count = readStream.Read(read, 0, 256);
            string str = null;
            while (count > 0)
            {
                str += new String(read, 0, count);
                count = readStream.Read(read, 0, 256);
            }
            readStream.Close();
            myResp.Close();
            return str;
        }

        /// <summary>
        /// 同步方式发起http get请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="queryString">参数字符串</param>
        /// <returns>请求返回值</returns>
        public string HttpGet(string url, string queryString)
        {
            string strResponse = string.Empty;
            if (!string.IsNullOrEmpty(queryString))
            {
                url += "?" + queryString;
            }

            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Proxy = null;
                request.Method = "GET";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                request.ContentType = "application/x-www-form-urlencoded";
               // request.Headers.Add(string.Format("Coolwi-Header:AppSign=MMS;AppVersion=1.0;DeviceType=3;DeviceId={0};ClientToken={1}", CommonUtil.GetMacAddressByNetBios(), SessionUtil.ClientToken));
                request.Timeout = 20000;

                using (System.IO.Stream responseStream = request.GetResponse().GetResponseStream())
                {
                    using (System.IO.StreamReader responseReader = new System.IO.StreamReader(responseStream, Encoding.UTF8))
                    {
                        strResponse = responseReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("请求失败，参考信息：{0}", ex.Message));
                strResponse = ex.Message;
            }
            return strResponse;
        }

        /// <summary>
        /// 同步方式发起http get请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="paras">请求参数列表</param>
        /// <returns>请求返回值</returns>
        public string HttpGet(string url,List<APIParameter> paras)
        {
            string querystring = HttpUtil.GetQueryFromParas(paras);
            return HttpGet(url, querystring);
        }

        /// <summary>
        /// 同步方式发起http post请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="queryString">参数字符串</param>
        /// <returns>请求返回值</returns>
        public string HttpPost(string url, string queryString)
        {
            string strResponse = string.Empty;
            try
            {
                Encoding encoding = Encoding.GetEncoding("utf-8");
                byte[] bytesToPost = encoding.GetBytes(queryString);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Proxy = null;
                request.Method = "POST";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                request.ContentType = "application/x-www-form-urlencoded";
                //request.Headers.Add(string.Format("Coolwi-Header:AppSign=MMS;AppVersion={0};DeviceType=3;DeviceId={1};ClientToken={2}",Consts.APP_VER, CommonUtil.GetMacAddressByNetBios(), SessionUtil.ClientToken));
                request.ContentLength = bytesToPost.Length;
                System.IO.Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytesToPost, 0, bytesToPost.Length);
                requestStream.Close();

                using (System.IO.Stream responseStream = request.GetResponse().GetResponseStream())
                {
                    using (System.IO.StreamReader responseReader = new System.IO.StreamReader(responseStream, Encoding.UTF8))
                    {
                        strResponse = responseReader.ReadToEnd();
                    }
                }
            }
            catch(Exception ex)
            {
                LogUtil.Error(string.Format("请求API接口失败，参加信息：{0}。",ex.ToString()));
                strResponse = ex.Message;
            }
            return strResponse;
        }

        /// <summary>
        /// 同步方式发起http post请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="paras">请求参数列表</param>
        /// <returns>请求返回值</returns>
        public string HttpPost(string url, List<APIParameter> paras)
        {
            string querystring = HttpUtil.GetQueryFromParas(paras);
            return HttpPost(url, querystring);
        }

        /// <summary>
        /// 同步方式发起http post请求，可以同时上传文件
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="queryString">请求参数字符串</param>
        /// <param name="files">上传文件列表</param>
        /// <returns>请求返回值</returns>
        public string HttpPostWithFile(string url, string queryString, List<APIParameter> files)
        {
            Stream requestStream = null;
            StreamReader responseReader = null;
            string responseData = null;
            string boundary = DateTime.Now.Ticks.ToString("x");

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;
            webRequest.Credentials = CredentialCache.DefaultCredentials;

            Stream responseStream = null; 

            try
            {
                Stream memStream = new MemoryStream();

                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                List<APIParameter> listParams = HttpUtil.GetQueryParameters(queryString);

                foreach (APIParameter param in listParams)
                {
                    string formitem = string.Format(formdataTemplate, param.Name, param.Value);
                    byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }

                memStream.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: \"{2}\"\r\n\r\n";

                foreach (APIParameter param in files)
                {
                    string name = param.Name;
                    string filePath = param.Value;
                    string file = Path.GetFileName(filePath);
                    string contentType = HttpUtil.GetContentType(file);

                    string header = string.Format(headerTemplate, name, file, contentType);
                    byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                    memStream.Write(headerbytes, 0, headerbytes.Length);

                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;

                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }

                    memStream.Write(boundarybytes, 0, boundarybytes.Length);
                    fileStream.Close();
                }

                webRequest.ContentLength = memStream.Length;

                requestStream = webRequest.GetRequestStream();

                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();
                requestStream = null;

                responseStream = webRequest.GetResponse().GetResponseStream();
                responseReader = new StreamReader(responseStream);
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                    requestStream = null;
                }

                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream = null;
                }

                if (responseReader != null)
                {
                    responseReader.Close();
                    responseReader = null;
                }

                webRequest = null;
            }

            return responseData;
        }

        /// <summary>
        /// 同步方式发起http post请求，可以同时上传文件
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="paras">请求参数列表</param>
        /// <param name="files">上传文件列表</param>
        /// <returns>请求返回值</returns>
        public string HttpPostWithFile(string url, List<APIParameter> paras, List<APIParameter> files)
        {
            string querystring = HttpUtil.GetQueryFromParas(paras);
            return HttpPostWithFile(url, querystring, files);
        }
    }
}
