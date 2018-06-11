//-------------------------------------------------
//版本信息:版权所有(C) 2014,PAIDUI.CN
//变更历史:
//    姓名            日期                  说明
//-------------------------------------------------
//   王军锋     2014/03/18 13:46:11           创建
//-------------------------------------------------
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;

using CWI.MCP.Common;

namespace CWI.MCP.API.Handels
{
    /// <summary>
    /// 控制器选择器类
    /// </summary>
    public class AppSignHttpControllerSelector : IHttpControllerSelector
    {
        /// <summary>
        /// httpConfiguration
        /// </summary>
        private readonly HttpConfiguration _configuration;

        /// <summary>
        /// APP应用标识
        /// </summary>
        public const string AppSignRouteVariableName = "app_sign";

        /// <summary>
        /// 控制器类后缀
        /// </summary>
        private const string ControllerKey = "controller";

        /// <summary>
        /// 延迟加载的所有控制器类型
        /// </summary>
        private readonly Lazy<ConcurrentDictionary<string, Type>> _apiControllerTypes;

        /// <summary>
        /// 控制器完整类名前缀
        /// </summary>
        public const string ControllerPrefix = "CWI.MCP.API.Controllers";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration">configuration</param>
        public AppSignHttpControllerSelector(HttpConfiguration configuration)
        {
            _configuration = configuration;

            _apiControllerTypes = new Lazy<ConcurrentDictionary<string, Type>>(GetControllerTypes);
        }

        /// <summary>
        /// 控制器选择
        /// </summary>
        /// <param name="request">HttpRequestMessage</param>
        /// <returns>选择的控制器</returns>
        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            return this.GetApiController(request);
        }

        /// <summary>
        /// 取AppSign
        /// </summary>
        /// <param name="request">HttpRequestMessage</param>
        /// <returns>appSign</returns>
        private static string GetAppSign(HttpRequestMessage request)
        {
            var data = request.GetRouteData();
            if (data.Route.DataTokens == null)
            {
                return null;
            }
            else
            {
                object appSign = null;
                data.Values.TryGetValue(AppSignRouteVariableName, out appSign);
                return appSign == null ? string.Empty : appSign.ToString();
            }
        }

        /// <summary>
        /// 取控制器类型
        /// </summary>
        /// <returns>控制器类型</returns>
        private static ConcurrentDictionary<string, Type> GetControllerTypes()
        {
           // var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly[] assemblies = new Assembly[] { AppDomain.CurrentDomain.Load("CWI.MCP.API") }; //指定程序集

            var types = assemblies
                .SelectMany(a => a
                    .GetTypes().Where(t =>
                        !t.IsAbstract && t.Namespace != null &&
                        t.Namespace.StartsWith(ControllerPrefix, StringComparison.OrdinalIgnoreCase) &&
                        typeof(IHttpController).IsAssignableFrom(t)))
                .ToDictionary(t => t.FullName, t => t);

            return new ConcurrentDictionary<string, Type>(types);
        }

        /// <summary>
        /// 取控制器
        /// </summary>
        /// <param name="request">HttpRequestMessage</param>
        /// <returns>控制器</returns>
        private HttpControllerDescriptor GetApiController(HttpRequestMessage request)
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            var appSign = GetAppSign(request);
            var controllerName = GetControllerName(request);

            //忽略浏览器发送的icon请求
            if (controllerName.Equals("favicon.ico"))
            {
                return null;
            }

            Type type;
            try
            {
                type = GetControllerType(appSign, controllerName);
            }
            catch (Exception ex)
            {
                LogUtil.Warn(ex.ToString());
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, new HttpError("控制器" + controllerName + "未找到")));
            }

            TimeSpan te = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ta = ts.Subtract(te).Duration();
            LogUtil.Info(string.Format("\r\n\r\n路由选择控制器 {0} 耗时：{1}秒{2}毫秒", controllerName, ta.Seconds, ta.Milliseconds));

            return new HttpControllerDescriptor(_configuration, controllerName, type);
        }

        /// <summary>
        /// 根据HttpRequestMessage取控制器名
        /// </summary>
        /// <param name="request">HttpRequestMessage</param>
        /// <returns>控制器名</returns>
        public virtual string GetControllerName(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("Instance of HttpRequestMessage is null");
            }
            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
            {
                return null;
            }
            object str = null;
            routeData.Values.TryGetValue(ControllerKey, out str);
            return str.ToString();
        }

        /// <summary>
        /// 取控制器类型
        /// </summary>
        /// <param name="appSign">appSign</param>
        /// <param name="controllerName">控制器名</param>
        /// <returns>控制器类型</returns>
        private Type GetControllerType(string appSign, string controllerName)
        {
            var query = _apiControllerTypes.Value.AsEnumerable();

            if (string.IsNullOrEmpty(appSign))
            {
                query = query.WithoutAppSign();
            }
            else
            {
                query = query.ByAppSign(appSign);
            }

            return query.ByControllerName(appSign, controllerName).Select(x => x.Value).Single();
        }

        /// <summary>
        /// 未实现的控制器映射关系
        /// </summary>
        /// <returns>控制器映射关系字典</returns>
        public virtual IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            throw new NotImplementedException();
        }
    }   
}
