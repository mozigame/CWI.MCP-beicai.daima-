//-------------------------------------------------
//版本信息:版权所有(C) 2014,COOLWI.COM
//变更历史:
//    姓名            日期             说明
//-------------------------------------------------
//   王军锋     2014/08/01 13:46:11    创建
//-------------------------------------------------

using System;
using System.Data;
using System.Web;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.Web.Http;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net.Http.Headers;

using Evt.Framework.Common;
using CWI.MCP.Common;
using CWI.MCP.Models;
using CWI.MCP.API.Handels;
using CWI.MCP.Common.Extensions.ViewModel;
using CWI.MCP.Services.APICommon;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using System.Text;

namespace CWI.MCP.API.Controllers
{
 /// <summary>
    /// 控制器基类
    /// </summary>
    [WebAPIFilter]
    [UnhandledExceptionFilter]
    [ModelValidationFilter]
    public class BaseController : ApiController
    {
        /// <summary>
        /// 会话
        /// </summary>
        public Session Session
        {
            get;
            set;
        }

        /// <summary>
        /// 获取当前会话状态用户
        /// </summary>
        public string CurrentSessionUserAccount
        {
            get
            {
                LoginInfo loginInfo = Session[Consts.LOGIN_USER_SESSION_KEY] as LoginInfo;
                return loginInfo == null ? string.Empty : loginInfo.Account;
            }
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        private JavaScriptSerializer _js = null;

        /// <summary>
        /// 获取序列化对象
        /// </summary>
        private JavaScriptSerializer JS
        {
            get
            {
                if (_js == null)
                {
                    _js = new JavaScriptSerializer();
                }
                return _js;
            }
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns>object</returns>
        [CWI.MCP.API.Handels.SkipAuthorize]
        [AcceptVerbs("GET")]
        public object Index()
        {
            return OK();
        }

        #region 受保护的辅助方法

        /// <summary>
        /// 返回状态为成功的JsonResult。
        /// </summary>
        /// <returns>JsonResult</returns>
        protected object OK()
        {
            return OK(null);
        }

        /// <summary>
        /// 返回状态为成功的JsonResult。
        /// </summary>
        /// <param name="data">需返回给客户端的Json数据。</param>
        /// <returns>JsonResult</returns>
        protected object OK(object data)
        {
            object result = null;
            if (data is DataTable)
            {
                var dataTable = ConvertUtil.ConvertDataTableToList(data as DataTable);
                result = new { status = ActionResultCode.Success, data = dataTable };
            }
            else if (data is DataSet)
            {
                var dataSet = ConvertUtil.ConvertDataSetToDictionary(data as DataSet);
                result = new { status = ActionResultCode.Success, data = dataSet };
            }
            else if (data == null)
            {
                result = new { status = ActionResultCode.Success, data = "ok" };
            }
            else
            {
                result = new { status = ActionResultCode.Success, data = data };
            }
            
            return result;
        }

        /// <summary>
        /// 返回状态为错误的JsonResult。
        /// </summary>
        /// <param name="message">错误信息。</param>
        /// <returns>JsonResult</returns>
        protected object Failed(string message)
        {
            object result = null;
            result = new { status = ActionResultCode.Failed, data = message };
            return result;
        }

        /// <summary>
        /// 返回状态为重复提交的JsonResult。
        /// </summary>
        /// <param name="message">重复提交信息。</param>
        /// <returns>JsonResult</returns>
        protected object RepeatSubmit(string message)
        {
            object result = null;
            result = new { status = ActionResultCode.RepeatSubmit, data = message };
            return result;
        }

        /// <summary>
        /// 返回确认提示，需等待客户端确定下一步操作
        /// </summary>
        /// <param name="message">确认消息</param>
        /// <returns>响应</returns>
        protected object Confirm(string message)
        {
            return new { status = ActionResultCode.Confirm, data = message };
        }

        /// <summary>
        /// 获取请求参数列表
        /// </summary>
        /// <param name="actionDesc">接口描述</param>
        /// <returns></returns>
        protected NameValueCollection GetRequestParams(string actionDesc = "")
        {
            var queryStrings = Request.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(queryStrings))
            {
                queryStrings = HttpUtility.UrlDecode(Request.RequestUri.Query.TrimStart('?'), Encoding.UTF8);
            }
            else
            {
                queryStrings = HttpUtility.UrlDecode(queryStrings, Encoding.UTF8);
            }
            LogUtil.Info(string.Format("{0}请求参数：{1}", actionDesc, queryStrings));

            var sortDics = new SortedDictionary<string, object>();
            var requestForms = HttpRequestUtility.GetNameValueCollection(queryStrings, out sortDics);
            if (requestForms.Keys.Count <= 0)
            {
                var msg = string.Format("{0}缺少参数！", actionDesc);
                LogUtil.Warn(msg);
                throw new MessageException(msg);
            }
            return requestForms;
        }

        #endregion

    }

    /// <summary>
    /// 过滤器属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WebAPIFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        #region 公共方法

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="actionContext">当前异常</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            var returnType = "application/json";
            //HttpRequestMessage request = actionContext.Request;
            //NameValueCollection requestForm = HttpUtility.ParseQueryString(request.RequestUri.Query);
            //if (requestForm.AllKeys.Contains("rt"))
            //{
            //    returnType = requestForm["rt"].ToString() == "0" ? "application/xml" : "application/json";
            //}

            actionContext.Request.Headers.Accept.Clear();
            actionContext.Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(returnType));
            actionContext.Request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("zh-cn", 0.5));
            actionContext.Request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("en-us", 0.5));
            actionContext.Request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            actionContext.Request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            System.Collections.ObjectModel.Collection<CookieHeaderValue> cookies = actionContext.Request.Headers.GetCookies("SessionID");

            BaseController bc = (BaseController)actionContext.ControllerContext.Controller;

            //初始化Cookie和Session以及当前登录用户
            InitiSession(cookies, actionContext, bc);

            //写入日志          
            //WriteLogInfo(actionContext, request, bc);

            //检查当前Action是否需要身份验证和授权才能执行
            CheckLoginAndOperation(actionContext, bc);
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="actionExecutedContext">异常类型</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            BaseController bc = (BaseController)actionExecutedContext.ActionContext.ControllerContext.Controller;
            string actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName ?? string.Empty;

            if (actionExecutedContext.Response != null)
            {
                actionExecutedContext.Response.Headers.AddCookies(new CookieHeaderValue[] { new CookieHeaderValue("SessionID", bc.Session.SessionID) { Path = "/" } });

                //如果请求不带版本号，则添加响应头以指示浏览器不缓存当前请求结果
                if (actionExecutedContext.ActionContext.Request.Properties.ContainsKey("v"))
                {
                    actionExecutedContext.ActionContext.Response.Headers.Add("Pragma", "no-cache");
                    actionExecutedContext.ActionContext.Response.Headers.Add("Expires", "0");
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }

        #endregion

        #region 私有方法

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
                dic.Add(key, collection[key]);
            }

            return JsonUtil.Serialize(dic);
        }

        /// <summary>
        /// 检查当前Action是否需要身份验证和授权才能执行
        /// </summary>
        /// <param name="actionContext">HttpActionContext</param>
        /// <param name="bc">BaseController</param>
        private void CheckLoginAndOperation(HttpActionContext actionContext, BaseController bc)
        {
            var attributes = actionContext.ActionDescriptor.GetCustomAttributes<CWI.MCP.API.Handels.SkipAuthorizeAttribute>();

            //验证是否需要登录
            if (attributes != null && attributes.Count > 0)
            {
                return;
            }

            //验证是否已经登录
            if (bc.Session.ContainsKey(Consts.LOGIN_USER_SESSION_KEY))
            {
                LoginInfo info = bc.Session[Consts.LOGIN_USER_SESSION_KEY] as LoginInfo;
                if (info == null)
                {
                    throw new AuthenticationException("会话超时，请重新登录！");
                }
            }
            else
            {
                throw new AuthenticationException("会话超时，请重新登录！");
            }
        }

        /// <summary>
        /// 初始化Session和当前用户
        /// </summary>
        /// <param name="cookies">Collection</param>
        /// <param name="bc">BaseController</param>
        private void InitiSession(Collection<CookieHeaderValue> cookies, HttpActionContext actionContext, BaseController bc)
        {
            if (cookies == null || cookies.Count == 0)
            {
                bc.Session = SessionManager.CreateSession();
            }
            else
            {
                string sessionID = string.Empty;
                foreach (CookieState cookieState in cookies[0].Cookies)
                {
                    if (cookieState.Name == "SessionID")
                    {
                        sessionID = cookieState.Value;
                        break;
                    }
                }
                Session session = SessionManager.GetSession(sessionID);

                if (session == null)
                {
                    bc.Session = SessionManager.CreateSession();
                }
                else
                {
                    bc.Session = session;
                }
            }

            //获取客户端信息
            RequestClientInfoModel clientModel = new RequestClientInfoModel();
            if (bc.Session.ContainsKey(Consts.CLIENT_INFO_SESSION_KEY))
            {
                var clientSeesion = bc.Session[Consts.CLIENT_INFO_SESSION_KEY];
                if (clientSeesion != null)
                {
                    clientModel = bc.Session[Consts.CLIENT_INFO_SESSION_KEY] as RequestClientInfoModel;
                }
            }
            else
            {
                clientModel = GetClientInfo(actionContext.Request);
                clientModel.ClientIP = CommonUtil.GetClientIp(actionContext.Request);
                bc.Session[Consts.CLIENT_INFO_SESSION_KEY] = clientModel;
            }

            //设置Service中的ContextUser         
            ServiceContext.Current.ContextUser = new SysServericeContext(bc.Session.SessionID);

            //设置Service中的RequestTerminal
            ServiceContext.Current.RequestTerminal = new PrdRequestTerminal(bc.Session.SessionID);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="actionContext">HttpActionContext</param>
        /// <param name="request">HttpRequestMessage</param>
        /// <param name="bc">BaseController</param>
        private void WriteLogInfo(HttpActionContext actionContext, HttpRequestMessage request, BaseController bc)
        {
            EUrlStatViewModel stat = new EUrlStatViewModel();
            stat.RequestUrl = request.RequestUri.Authority + request.RequestUri.AbsolutePath;
            stat.RequestType = request.Method.Method;
            stat.SessionID = string.Empty;
            stat.ProSign = "MCP";
            if (!actionContext.ActionArguments.ContainsKey("IP"))
            {
                actionContext.ActionArguments.Add("IP", CommonUtil.GetClientIp(actionContext.Request));
            }
            else if (actionContext.Request.Headers.Contains("Coolwi-Header"))
            {
                actionContext.ActionArguments.Add("Client-Info", actionContext.Request.Headers.GetValues("Coolwi-Header")); //新版：保存请求头中自定义的客户端信息
            }

            stat.ParamData = JsonUtil.Serialize(actionContext.ActionArguments);
            stat.Controller = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            stat.Action = actionContext.ActionDescriptor.ActionName;
            stat.SessionID = bc.Session.SessionID;
            if (ConfigUtil.IsLogToDb)
            {
                LogUtil.WriteDB(stat);
            }
            else
            {
                LogUtil.Info(JsonUtil.Serialize(stat));
            }
        }


        /// <summary>
        /// 解析Coolwi-Header内容
        /// </summary>
        /// <param name="valueString">Coolwi-Header值</param>
        /// <returns>解析Coolwi-Header内容</returns>
        private RequestClientInfoModel ResolveClientInfo(string valueString)
        {
            RequestClientInfoModel model = new RequestClientInfoModel();
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

            return JsonUtil.Deserialize<RequestClientInfoModel>(JsonUtil.Serialize(dic));
        }

        /// <summary>
        /// 从Request对象中获取Headers请求的Client-Info参数DeviceID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private RequestClientInfoModel GetClientInfo(HttpRequestMessage request)
        {
            RequestClientInfoModel model = new RequestClientInfoModel();

            string clientInfo = string.Empty;
            if (!string.IsNullOrEmpty(clientInfo))
            {
                try
                {
                    model = JsonUtil.Deserialize<RequestClientInfoModel>(clientInfo);
                }
                catch (Exception ex)
                {
                    LogUtil.Warn(string.Format("Coolwi-Header is invalid:{0}.", ex.Message));
                    throw new MessageException("Http请求头信息格式不正确。");
                }
            }
            else
            {
                if (request.Headers.Contains("Coolwi-Header"))
                {
                    clientInfo = request.Headers.GetValues("Coolwi-Header").FirstOrDefault();
                }
                model = ResolveClientInfo(clientInfo);
            }

            return model;
        }

        #endregion
    }

    /// <summary>
    /// 未处理异常
    /// </summary>
    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        private JavaScriptSerializer _js = null;

        /// <summary>
        /// 获取序列化对象
        /// </summary>
        private JavaScriptSerializer JS
        {
            get
            {
                if (_js == null)
                {
                    _js = new JavaScriptSerializer();
                }
                return _js;
            }
        }

        /// <summary>
        /// 异常抓获事件
        /// </summary>
        /// <param name="context">当前异常</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            object result = null;
            Exception exception = context.Exception;

            //把错误号转化为实际的错误信息 
            string message = ErrorHandler.GetErrorMessage(exception.Message);
            if (exception is AuthenticationException)
            {
                //未登录
                Exception ex = exception.GetBaseException();
                result = new { status = ActionResultCode.Unauthorized, data = ex.Message };
                LogUtil.Info(GetExceptionMessage(context, ex));
            }
            else if (exception is InnerException)
            {
                //内部错误
                Exception ex = exception.GetBaseException();
                result = new { status = ActionResultCode.InnerError, data = message };
                LogUtil.Warn(GetExceptionMessage(context, ex));
            }
            else if (exception is MessageException)
            {
                //数据异常
                Exception ex = exception.GetBaseException();
                result = new { status = ActionResultCode.Failed, data = message };
                LogUtil.Warn(GetExceptionMessage(context, ex));
            }
            else if (exception is BusinessException)
            {
                //业务异常
                BusinessException businessEx = exception as BusinessException;
                message = string.Format(ErrorHandler.GetErrorMessage(businessEx.Message));

                Exception ex = exception.GetBaseException();
                result = new { status = ActionResultCode.Failed, data = message };
                LogUtil.Warn(GetExceptionMessage(context, ex));
            }
            else if (exception is HttpRequestValidationException)
            {
                //验证异常
                Exception ex = exception.GetBaseException();
                result = new { status = ActionResultCode.Failed, data = "请输入有效字符！" };
                LogUtil.Info(GetExceptionMessage(context, ex));
            }
            else
            {
                //未知异常：全部写入Error级别的日志
                Exception ex = exception.GetBaseException();
                LogUtil.Error(GetExceptionMessage(context, ex));

                result = new { status = ActionResultCode.UnknownError, data = "出故障啦，请您稍后重新尝试！" };
            }
            context.Response = context.Request.CreateResponse();
            context.Response.Headers.Clear();
            context.Response.Content = new StringContent(JS.Serialize(result));
            context.Response.StatusCode = System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// 拼装异常信息与环境信息
        /// </summary>
        /// <param name="context">当前上下文</param>
        /// <param name="ex">异常</param>
        /// <returns>异常信息与环境信息文本</returns>
        private static string GetExceptionMessage(HttpActionExecutedContext context, Exception ex)
        {
            string paramData = JsonUtil.Serialize(context.ActionContext.ActionArguments);
            BaseController bc = (BaseController)context.ActionContext.ControllerContext.Controller;
            string sessionID = bc.Session.SessionID;
            return ex.ToString() + "\r\n  ParamData:" + paramData + "  SessionID:" + sessionID + "\r\n  URL:" + context.Request.RequestUri + "\r\n\r\n";
        }
    }

    /// <summary>
    /// Model校验特性过滤器
    /// </summary>
    public class ModelValidationFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        private JavaScriptSerializer _js = null;

        /// <summary>
        /// 获取序列化对象
        /// </summary>
        private JavaScriptSerializer JS
        {
            get
            {
                if (_js == null)
                {
                    _js = new JavaScriptSerializer();
                }
                return _js;
            }
        }

        /// <summary>
        /// Action执行时触发
        /// </summary>
        /// <param name="actionContext">actionContext</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                // Return the validation errors in the response body.
                string errors = string.Empty;
                foreach (KeyValuePair<string, ModelState> keyValue in actionContext.ModelState)
                {
                    if (keyValue.Value.Errors.Count > 0)
                    {
                        errors = keyValue.Value.Errors[0].ErrorMessage;
                        break;
                    }
                }

                //actionContext.Response =  actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, errors);
                object result = new { status = ActionResultCode.Failed, data = errors };
                actionContext.Response = actionContext.Request.CreateResponse();
                actionContext.Response.Headers.Clear();
                actionContext.Response.Content = new StringContent(JS.Serialize(result));
                actionContext.Response.StatusCode = System.Net.HttpStatusCode.OK;
            }
        }
    }
}
