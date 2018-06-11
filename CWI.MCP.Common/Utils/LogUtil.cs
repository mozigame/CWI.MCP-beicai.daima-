//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/06        创建
//---------------------------------------------

using System;
using log4net;
using System.Text;
using System.Web;
using System.Collections.Specialized;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
[assembly: log4net.Config.Repository("log4net-tracemanager-repository")]

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 日志工具类。
    /// </summary>
    public static class LogUtil
    {
        private static ILog _log = LogManager.GetLogger(typeof(LogUtil));
        private static ILog _logDB = LogManager.GetLogger("dbLogger");

        /// <summary>
        /// 日志记录中的系统名称
        /// </summary>
        public static string _systemName = string.Empty;
        public static string SystemName { get; set; }

        /// <summary>
        /// CacheUtil
        /// </summary>
        static LogUtil()
        {
            if (string.IsNullOrWhiteSpace(_systemName))
            {
                _systemName = "MCP";
            }
        }

        /// <summary>
        /// 记录日志到数据库(用于手机端请求地址统计功能)
        /// </summary>
        /// <param name="message">消息对象</param>
        public static void WriteDB(object message)
        {
            if (_logDB.IsInfoEnabled)
            {
                _logDB.Info(message);
            }
        }

        /// <summary>
        /// 记录调试级别的日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public static void Debug(string message)
        {
            if (_log.IsDebugEnabled)
            {
                _log.Debug(FormatMessage(message, "Debug", String.Empty));
            }
        }

        /// <summary>
        /// 记录消息级别的日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public static void Info(string message)
        {
            if (_log.IsInfoEnabled)
            {
                _log.Info(FormatMessage(message, "Info", String.Empty));
            }
        }

        /// <summary>
        /// 记录警告级别的日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public static void Warn(string message)
        {
            if (_log.IsWarnEnabled)
            {
                _log.Warn(FormatMessage(message, "Warn", String.Empty));
            }
        }

        /// <summary>
        /// 记录错误级别的日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public static void Error(string message)
        {
            if (_log.IsErrorEnabled)
            {
                _log.Error(FormatMessage(message, "Error", String.Empty));
            }
        }


        /// <summary>
        /// 记录错误级别的日志。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="ex">异常对象。</param>
        public static void Error(string message, Exception ex)
        {
            if (_log.IsErrorEnabled)
            {
                _log.Error(FormatMessage(message, "Error", ex.ToString()));
            }
        }

        #region 私有方法

        /// <summary>
        /// 格式化日志信息。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="level">日志级别。</param>
        /// <param name="stackTrace">堆栈信息。</param>
        /// <returns>格式化后的日志信息。</returns>
        private static string FormatMessage(string message, string level, string stackTrace)
        {
            string msg = "{0}|{1}|{2}|{3}";
            string datetime = SysDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string systemName = SystemName ?? "MCP";

            return String.Format(msg, systemName, level, datetime, message + stackTrace);
        }

        /// <summary>
        /// 格式化精简日志信息<br />
        /// 信息格式为：系统名称|日志级别|发生时间|日志信息
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="level">日志级别。</param>
        /// <returns>格式化后的精简日志信息。</returns>
        private static string FormatSimpleMessage(string message, string level)
        {
            string msg = "{0}|{1}|{2}|{3}";
            string datetime = SysDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string systemName = SystemName ?? "MCP";

            return String.Format(msg, systemName, level, datetime, message);
        }


        /// <summary>
        /// 格式化详细日志信息<br />
        /// 信息格式为：系统名称|日志级别|发生时间|日志信息|所在机器|客户端IP|Session信息|浏览器信息|请求信息|堆栈信息
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="level">日志级别。</param>
        /// <param name="stackTrace">堆栈信息。</param>
        /// <returns>格式化后的详细日志信息。</returns>
        private static string FormatDetailMessage(string message, string level, string stackTrace)
        {
            string msg = @"{0}|{1}|{2}*RequestUrl:{3}*Message:{4}*Machine:{5}*ClientIP:{6}*Headers:{7}*Parameters:{8}*StackTrae:{9}*";
            msg = msg.Replace("*", Environment.NewLine);
            string systemName = "MCP";
            string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string requestUrl = string.Empty;
            string machine = Environment.MachineName;
            string clientIP = String.Empty;
            string browserInfo = String.Empty;
            string requestInfo = String.Empty;

            try
            {
                if (HttpContext.Current != null)
                {
                    clientIP = HttpContext.Current.Request.UserHostAddress;
                    requestUrl = HttpContext.Current.Request.Url.AbsoluteUri;

                    NameValueCollection headers = HttpContext.Current.Request.Headers;
                    if (headers != null && headers.Count > 0)
                    {
                        browserInfo = "[";
                        foreach (string header in headers)
                        {
                            foreach (string headervalue in headers.GetValues(header))
                            {
                                browserInfo += String.Format("{0}:{1};", header, headervalue);
                            }
                        }
                        browserInfo += "]";
                    }

                    string httpMethod = HttpContext.Current.Request.HttpMethod.ToUpper();
                    if (httpMethod == "GET")
                    {
                        requestInfo = GetGetRequestInfo();
                    }
                    else if (httpMethod == "POST")
                    {
                        requestInfo = GetPostRequestInfo();
                    }
                }
            }
            catch
            {
                //此处主要是为保证当Request不存在时，日志正常记录
            }

            return String.Format(msg, systemName, level, datetime, requestUrl, message, machine, clientIP, browserInfo, requestInfo, stackTrace);
        }

        private static string GetGetRequestInfo()
        {
            NameValueCollection querystring = HttpContext.Current.Request.QueryString;

            string requestInfo = "GET[";
            try
            {
                foreach (string key in querystring.AllKeys)
                {
                    foreach (string value in querystring.GetValues(key))
                    {
                        requestInfo += String.Format("{0}:{1};", key, value);
                    }
                }
            }
            catch
            {

            }
            requestInfo += "]";
            return requestInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetPostRequestInfo()
        {
            string requestInfo = "POST[";
            try
            {
                int count = 0;
                byte[] buffer = new byte[1024];
                StringBuilder builder = new StringBuilder();

                //读取请求输入流的内容
                System.IO.Stream requestStream = HttpContext.Current.Request.InputStream;
                while ((count = requestStream.Read(buffer, 0, 1024)) > 0)
                {
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
                }

                requestInfo += builder.ToString();
            }
            catch
            {
            }
            requestInfo += "]";

            return requestInfo;
        }

        #endregion
    }
}