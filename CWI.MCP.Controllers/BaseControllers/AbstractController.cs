using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using System.Collections.Specialized;
using Evt.Framework.Common;

using CWI.MCP.Common;
using CWI.MCP.Common.Extensions.ViewModel;
using CWI.MCP.Services;
using CWI.MCP.Models;

namespace CWI.MCP.Controllers
{
    // 创建一个异步委托
    public delegate void WriteStatCaller(EUrlStatViewModel stat);

    public abstract class AbstractController : AsyncController  //由于长连接为异步模式，故需从AsyncController继承 Modified by 王军锋 2012/08/29
    {
        #region 验证与异常

        /// <summary>
        /// 检查未标记NonAuthorizedAttribute属性的Action，如果是未登录状态，则抛出AuthorizationException异常
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            WriteRequestToDB(filterContext);

            base.OnAuthorization(filterContext);

            //object[] nonAuth = filterContext.ActionDescriptor.GetCustomAttributes(typeof(NonAuthorizedAttribute), false);
            //if (nonAuth.Length == 0)
            //{
            //    if (SessionUtil.Current == null)
            //    {
            //        throw new AuthorizationException("请先登录");
            //    }
            //}
        }

        #region 将请求写入数据库

        /// <summary>
        /// 写请求到数据库
        /// </summary>
        /// <param name="filterContext">filterContext</param>
        private void WriteRequestToDB(AuthorizationContext filterContext) 
        {
            try
            {
                //EUrlStatViewModel stat = GetUrlStatViewModel(filterContext);
                //WriteStatCaller writeStatCaller = new WriteStatCaller(CommonService.Instance.WriteUrlStat);
                //writeStatCaller.BeginInvoke(stat, null, null);
            }
            catch (Exception ex) 
            {
                LogUtil.Warn(ex.Message);
            }
        }

        /// <summary>
        /// 获取EUrlStatViewModel对象
        /// </summary>
        /// <param name="filterContext">AuthorizationContext</param>
        /// <returns></returns>
        private EUrlStatViewModel GetUrlStatViewModel(AuthorizationContext filterContext) 
        {
            HttpRequestBase request = filterContext.HttpContext.Request;

            EUrlStatViewModel stat = new EUrlStatViewModel();
            stat.RequestUrl = request.Url.Authority + request.Url.AbsolutePath;
            stat.RequestType = request.RequestType;
            stat.SessionID = filterContext.HttpContext.Session.SessionID;
            stat.ProSign = "MCP";
            stat.ParamData = GetParamData(request);
            stat.HeaderParamData = GetHeadersParam(request);
            stat.UserId = SessionUtil.Current != null ? SessionUtil.Current.UserId : null;
            RequestClientInfoQueryModel mode = GetClientInfo(request);
            stat.DeviceId = mode.DeviceId;
            stat.DeviceType = mode.DeviceType;
            stat.Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            stat.Action = filterContext.ActionDescriptor.ActionName;

            return stat;
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
        /// 从Request对象中获取Headers请求的Client-Info参数
        /// </summary>
        /// <param name="request">Request对象</param>
        /// <returns>参数Json格式</returns>
        private string GetHeadersParam(HttpRequestBase request)
        {
            string json = string.Empty;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Dictionary<string, string> userInfo = new Dictionary<string, string>();

            string headInfo = request.Headers["Client-Info"];

            if (headInfo != null)
            {
                dic.Add("ClientInfo", JsonUtil.Deserialize<RequestClientInfoQueryModel>(request.Headers["Client-Info"]));
            }
            else
            {
                headInfo = request.Headers.Get("Coolwi-Header");
                if (!string.IsNullOrEmpty(headInfo))
                {
                    dic.Add("CoolwiHeader", headInfo);
                }
            }

            if (SessionUtil.Current != null)
            {
                userInfo.Add("UserId", SessionUtil.Current.UserId);
            }
            dic.Add("UserInfo", userInfo);
            return JsonUtil.Serialize(dic);

        }

        /// <summary>
        /// 从Request对象中获取Headers请求的Client-Info参数DeviceID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private RequestClientInfoQueryModel GetClientInfo(HttpRequestBase request)
        {
            RequestClientInfoQueryModel model = new RequestClientInfoQueryModel();

            string clientInfo = request.Headers.Get("Client-Info"); //旧版格式
            if (!string.IsNullOrEmpty(clientInfo))
            {
                model = JsonUtil.Deserialize<RequestClientInfoQueryModel>(clientInfo);
            }
            else
            {
                clientInfo = request.Headers.Get("Coolwi-Header");
                model = ResolveClientInfo(clientInfo);
            }

            return model;
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

        /// <summary>
        /// 解析Coolwi-Header内容
        /// </summary>
        /// <param name="valueString">Coolwi-Header值</param>
        /// <returns>解析Coolwi-Header内容</returns>
        private RequestClientInfoQueryModel ResolveClientInfo(string valueString)
        {
            RequestClientInfoQueryModel model = new RequestClientInfoQueryModel();
            if (string.IsNullOrWhiteSpace(valueString))
            {
                return model;
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] values = valueString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var v in values)
            {
                string[] item = v.Split(new char[] { '=' });
                if (item.Length != 2)
                {
                    LogUtil.Warn(string.Format("Coolwi-Header is invalid:{0}.", valueString));
                    continue;
                }

                if (!dic.ContainsKey(item[0]))
                {
                    dic.Add(item[0], item[1]);
                }
            }

            return JsonUtil.Deserialize<RequestClientInfoQueryModel>(JsonUtil.Serialize(dic));
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

            if (string.IsNullOrEmpty(Request.Headers["X-Requested-With"]))
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
                LogUtil.Warn(filterContext.Exception.ToString());
                ViewBag.Type = "MessageException";
            }
            else if (filterContext.Exception is AuthorizationException)
            {
                ViewBag.Type = "AuthorizationException";
            }
            else
            {
                LogUtil.Error(filterContext.Exception.ToString());
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
                LogUtil.Info("测试是否进入这里来");
                LogUtil.Warn(filterContext.Exception.ToString());
                ResponseUtil.ResponseTextHtml("{\"status\":2,\"data\":\"" + filterContext.Exception.Message + "\"}");
            }
            else if (filterContext.Exception is AuthorizationException)
            {
                ResponseUtil.ResponseTextHtml("{\"status\":3,\"data\":\"" + filterContext.Exception.Message + "\"}");
            }
            else
            {
                LogUtil.Error(filterContext.Exception.ToString());
                ResponseUtil.ResponseTextHtml("{\"status\":4,\"data\":\"" + filterContext.Exception.Message + "\"}");
            }

            filterContext.Result = null;
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        #endregion

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
        /// 返回格式如下：{"status":1,"data":参数}
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
        /// 返回格式如下：{"status":0,"data":参数}
        /// </summary>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Error(string data, string code)
        {
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/html";
            cr.Content = GetOutputJson(ResponseStatusType.Failed, data, code);
            return cr;
        }

        /// <summary>
        /// 表示需要客户端确认后续操作。
        /// </summary>
        /// <param name="data">要输出的数据</param>
        /// <returns>content输出</returns>
        public ActionResult Confirm(string data)
        {
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/html";
            cr.Content = GetOutputJson(ResponseStatusType.Confirm, data);
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
                if (dataString != null && dataString.IndexOf("{") >= 0 && dataString.IndexOf("}") >= 0)
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

        /// <summary>
        /// 重定向到指定Controller的指定Action。
        /// </summary>
        /// <param name="actionName">Action名称。</param>
        /// <param name="controllerName">Controller名称。</param>
        /// <returns>RedirectToRouteResult</returns>
        protected RedirectToRouteResult RedirectToNewAction(string actionName, string controllerName)
        {
            string companyName = RoutingUtil.CompanyTag;
            return RedirectToRoute("Default", new { company = companyName, controller = controllerName, action = actionName });
        }

        /// <summary>
        /// 获取请求域名
        /// </summary>
        protected string DomailUrl
        {
            get
            {
                var protocol = Request.Url.Port == 443 ? "https" : "http";
                var port = (Request.Url.Port != 80 && Request.Url.Port != 443) ? ":" + Request.Url.Port.ToString() : string.Empty;
                return string.Format("{0}://{1}{2}", protocol, Request.Url.Host, port);
            }
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

        #endregion
    }
}