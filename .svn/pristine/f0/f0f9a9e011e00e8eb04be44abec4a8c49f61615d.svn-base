//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2012/10/24        创建
//---------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Collections.Specialized;
using Evt.Framework.Common;
using CWI.MCP.Common.Extensions.ViewModel;

namespace  CWI.MCP.Common.Extensions.MVC
{
    /// <summary>
    /// Url统计过虑器
    /// </summary>
    public class UrlStatFilter : IAuthorizationFilter
    {
        /// <summary>
        /// 实现接口中方法
        /// </summary>
        /// <param name="filterContext">授权过滤器对象</param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;

            EUrlStatViewModel stat = new EUrlStatViewModel();
            stat.RequestUrl = request.Url.Authority + request.Url.AbsolutePath;
            stat.RequestType = request.RequestType;
            stat.SessionID = filterContext.HttpContext.Session.SessionID;
            stat.ProSign = "API";
            stat.ParamData = GetParamData(request);
            stat.HeaderParamData = GetHeadersParam(request);
            stat.UserId = SessionUtil.Current != null ? SessionUtil.Current.UserId : null;
            RequestClientInfoQueryModel mode = GetClientInfo(request);
            stat.DeviceId =mode.DeviceId;
            stat.DeviceType = mode.DeviceType;
            stat.Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            stat.Action = filterContext.ActionDescriptor.ActionName;

            LogUtil.WriteDB(stat);
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
    }
}
