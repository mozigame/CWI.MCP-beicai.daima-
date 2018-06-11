using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Collections.Specialized;
using System.Web;

using Evt.Framework.Common;
using Evt.Framework.DataAccess;
using CWI.MCP.Common;
using System.Web.Routing;
using CWI.MCP.Common.Attributes;
using CWI.MCP.Services;

namespace CWI.MCP.Controllers
{
    public abstract class WeiXinBaseController : Controller
    {
        #region 验证与异常

        /// <summary>
        /// 重写父类的扩展授权方法，用来统一处理后台控制器的授权
        /// </summary>
        /// <param name="filterContext">授权上下文对象</param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            object[] auth = filterContext.ActionDescriptor.GetCustomAttributes(typeof(NonAuthorizedAttribute), false);
            if (auth.Length <= 0)
            {
                //if (SessionUtil.Current == null && Request.RawUrl != "/favicon.ico")
                //{
                //    if (SessionUtil.Current == null)
                //    {
                //        LogUtil.Warn(string.Format("Session丢失，请求地址：{0}", Request.RawUrl));
                //        throw new AuthorizationException("会话超时，请重新登录！");
                //    }
                //}
            }
        }

        /// <summary>
        /// 从Request对象中获取请求参数Json格式
        /// </summary>
        /// <param name="request">Request对象</param>
        /// <returns>参数Json格式</returns>
        private string GetParamData(HttpRequestBase request)
        {
            string json = string.Empty;
            string reuqestType = request.RequestType.ToLower();

            switch (reuqestType)
            {
                case "get":
                    json = GetJson(request.QueryString);
                    break;
                case "post":
                    json = GetJson(request.Form);
                    break;
                default:
                    json = GetJson(request.Params);
                    break;
            }

            return json;
        }

        /// <summary>
        /// 获取集合中的键值对,并序列号
        /// </summary>
        /// <param name="collection">集合</param>
        /// <returns>json对象</returns>
        private string GetJson(NameValueCollection collection)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (string key in collection.AllKeys)
            {
                if (!String.IsNullOrEmpty(key))
                {
                    dic.Add(key, collection[key]);
                }
            }

            return JsonUtil.Serialize(dic);
        }

        #endregion

        /// <summary>
        /// 返回的消息格式：
        /// status:0=失败，1=成功，2=提示消息，3=未登录，4=未知的异常
        /// data:返回的业务数据
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            if (string.IsNullOrEmpty(Request.Headers["ajax"]))
            {
                HandleException(filterContext);
            }
            else
            {
                HandleAjaxException(filterContext);
            }
        }

        protected virtual void HandleException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is MessageException)
            {
                LogUtil.Warn(string.Format("消息异常：{0}",filterContext.Exception.ToString()));
                ViewBag.Type = "MessageException";
            }
            else if (filterContext.Exception is AuthorizationException)
            {
                ViewBag.Type = "AuthorizationException";
                LogUtil.Warn(string.Format("权限异常：{0}",filterContext.Exception.ToString()));
                Response.Redirect(string.Format("/User/Login?toUrl={0}", Request.RawUrl));
            }
            else
            {
                LogUtil.Error(string.Format("未知异常：{0}",filterContext.Exception.ToString()));
                ViewBag.Type = "Exception";
            }
            ViewBag.Message = filterContext.Exception.Message;
            ViewBag.StackTrace = filterContext.Exception.StackTrace;
            filterContext.Result = View("Error");
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        protected virtual void HandleAjaxException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is MessageException)
            {
                LogUtil.Warn(string.Format("Ajax消息异常：{0}",filterContext.Exception.ToString()));
                ResponseUtil.ResponseTextHtml("{\"status\":2,\"data\":\"" + filterContext.Exception.Message + "\"}");
            }
            else if (filterContext.Exception is AuthorizationException)
            {
                LogUtil.Warn(string.Format("Ajax权限异常：{0}", filterContext.Exception.ToString()));
                ResponseUtil.ResponseTextHtml("{\"status\":3,\"data\":\"" + filterContext.Exception.Message + "\"}");
            }
            else
            {
                LogUtil.Error(string.Format("Ajax未知异常：{0}", filterContext.Exception.ToString()));
                ResponseUtil.ResponseTextHtml("{\"status\":4,\"data\":\"" + filterContext.Exception.Message + "\"}");
            }

            filterContext.Result = null;
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = 200;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        #region 成功或失败的消息返回

        /// <summary>
        /// 返回格式如下：{"status":1,data:"ok"}
        /// </summary>
        /// <returns></returns>
        public ActionResult OK()
        {
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/html";
            cr.Content = GetOutputJson(ResponseStatusType.OK, "ok");
            return cr;
        }

        /// <summary>
        /// 返回格式如下：{"status":"1","data":参数}
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult OK(string data)
        {
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/html";
            cr.Content = GetOutputJson(ResponseStatusType.OK, data);
            return cr;
        }

        /// <summary>
        /// 返回格式如下：{"status":1,"data":参数}
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult OK(int data)
        {
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/html";
            cr.Content = GetOutputJson(ResponseStatusType.OK, data);
            return cr;
        }

        /// <summary>
        /// 返回格式如下：{"status":1,"data":参数}
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult OK(object data)
        {
            if (data is string)
            {
                return OK(data as string);
            }
            else
            {
                return OK(JsonUtil.Serialize(data));
            }
        }

        /// <summary>
        /// 返回格式如下：{"status":0,"data":参数}
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult Error(string data)
        {
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/html";
            cr.Content = GetOutputJson(ResponseStatusType.Failed, data);
            return cr;
        }

        /// <summary>
        /// 执行失败，以错误码返回
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult ErrorUsingCode(string code)
        {
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/html";
            cr.Content = GetOutputJsonUsingCode(ResponseStatusType.Failed, code);
            return cr;
        }

        #region 构造输出JSON
        /// <summary>
        /// 构造输出的JSON
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="data">数据</param>
        /// <param name="code">错误码</param>
        /// <returns>输出的JSON</returns>
        protected string GetOutputJson(ResponseStatusType status, object data, string code)
        {
            string jsonData;
            if (!(data is string))
            {
                jsonData = JsonUtil.Serialize(data);
            }
            else
            {
                string dataString = data as string;
                if (dataString != null && ((dataString.IndexOf("{") >= 0 && dataString.IndexOf("}") >= 0) || dataString == "[]"))
                {
                    jsonData = dataString;
                }
                else
                {
                    jsonData = string.Format("\"{0}\"", data);
                }
            }

            return string.Format("{{\"status\":{0},\"code\":\"{1}\",\"data\":{2}}}", (int)status, code ?? string.Empty, jsonData);
        }

        /// <summary>
        /// 构造输出的JSON
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="code">错误码</param>
        /// <param name="data">数据</param>
        /// <returns>输出的JSON</returns>
        protected string GetOutputJson(ResponseStatusType status, object data)
        {
            return GetOutputJson(status, data, string.Empty);
        }

        /// <summary>
        /// 使用错误码构造输出
        /// </summary>
        /// <param name="status">响应状态</param>
        /// <param name="code">错误码</param>
        /// <returns>响应</returns>
        protected string GetOutputJsonUsingCode(ResponseStatusType status, string code)
        {
            return GetOutputJson(status, ErrorCodeUtil.GetMessage(code), code);
        }
        #endregion

        #endregion

        #region 常用方法

        /// <summary>
        /// 把指定的text输出到响应流
        /// </summary>
        /// <param name="text">将要输出的text</param>
        public void ResponseTextHtml(string text)
        {
            HttpContext.Response.ContentType = "text/html";
            HttpContext.Response.Write(text);
        }

        #endregion

        #region MockResponse

        protected override void Execute(RequestContext requestContext)
        {
            //if (!MockResponse(requestContext))
            //{
            base.Execute(requestContext);
            //}
        }

        private bool MockResponse(RequestContext requestContext)
        {
            if (requestContext.HttpContext.Request.Url.Segments.Length < 4)
            {
                return false;
            }

            string controllerName = requestContext.HttpContext.Request.Url.Segments[2].Replace("/", "");
            string actionName = requestContext.HttpContext.Request.Url.Segments[3].Replace("/", "");
            string fileName = requestContext.HttpContext.Server.MapPath("~/Xml/" + controllerName + "_" + actionName + ".xml");

            if (!System.IO.File.Exists(fileName))
            {
                return false;
            }

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(fileName);

            if (xmldoc.ChildNodes[1].Attributes[0].Value != "true")
            {
                return false;
            }

            Dictionary<string, string> keys = new Dictionary<string, string>();
            foreach (string k in requestContext.HttpContext.Request.QueryString.AllKeys)
            {
                keys.Add(k, requestContext.HttpContext.Request.QueryString[k]);
            }

            XmlNode response = null;
            XmlNode jsons = xmldoc.ChildNodes[1];
            foreach (XmlNode json in jsons)
            {
                if (IsMatchedNode(json, keys))
                {
                    response = json;
                    break;
                }
            }

            requestContext.HttpContext.Response.ContentType = "text/html";

            if (response == null)
            {
                requestContext.HttpContext.Response.Write("在XML文件中找不到对应的节点");
            }
            else
            {
                requestContext.HttpContext.Response.Write(response.InnerText.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", ""));
            }

            return true;
        }

        private bool IsMatchedNode(XmlNode json, Dictionary<string, string> keyvalues)
        {
            foreach (KeyValuePair<string, string> kv in keyvalues)
            {
                if (json.Attributes[kv.Key] == null || json.Attributes[kv.Key].Value != kv.Value)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 初始化当前会话
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userAccount"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        protected SessionData InitSession(long userId, string userAccount, string email)
        {
            //初始化会话信息
            var user = new SessionData()
            {
                UserId = userId.ToString(),
                Account = userAccount == null ? string.Empty : userAccount.Trim(),
                Email = email == null ? string.Empty : email.Trim()
            };
            SessionUtil.Current = user;
            return SessionUtil.Current;
        }

        #endregion
    }
}
