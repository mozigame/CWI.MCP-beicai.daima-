// --------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期          说明
// --------------------------------------------
//      王军锋     2012/12/03       创建
// --------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using CWI.MCP.Common.Attributes;
using Evt.Framework.Common;
using System.Web.Routing;

namespace  CWI.MCP.Common.Extensions.MVC
{
    /// <summary>
    /// UrlHelper方法扩展
    /// </summary>
    public static class MethodExtension
    {
        #region UrlHelper

        public static string FullUrl(this UrlHelper urlHelper, string actionName)
        {
            //相对地址
            string relativelyUrl = urlHelper.Action(actionName);

            string controllerName = GetControllerName(urlHelper);

            //绝对地址
            string absoluteUrl = GetAbsoluteUrl(urlHelper, actionName, controllerName, relativelyUrl);
            return absoluteUrl;
        }

        public static string FullUrl(this UrlHelper urlHelper, string actionName, object routeValues)
        {
            //相对地址
            string relativelyUrl = urlHelper.Action(actionName, routeValues);

            string controllerName = GetControllerName(urlHelper);

            //绝对地址
            string absoluteUrl = GetAbsoluteUrl(urlHelper, actionName, controllerName, relativelyUrl);
            return absoluteUrl;
        }

        public static string FullUrl(this UrlHelper urlHelper, string actionName, RouteValueDictionary routeValues)
        {
            //相对地址
            string relativelyUrl = urlHelper.Action(actionName, routeValues);

            string controllerName = GetControllerName(urlHelper);

            //绝对地址
            string absoluteUrl = GetAbsoluteUrl(urlHelper, actionName, controllerName, relativelyUrl);
            return absoluteUrl;
        }

        public static string FullUrl(this UrlHelper urlHelper, string actionName, string controllerName)
        {
            //相对地址
            string relativelyUrl = urlHelper.Action(actionName, controllerName);

            //绝对地址
            string absoluteUrl = GetAbsoluteUrl(urlHelper, actionName, controllerName, relativelyUrl);
            return absoluteUrl;
        }

        public static string FullUrl(this UrlHelper urlHelper, string actionName, string controllerName, object routeValues)
        {
            //相对地址
            string relativelyUrl = urlHelper.Action(actionName, controllerName, routeValues);

            //绝对地址
            string absoluteUrl = GetAbsoluteUrl(urlHelper, actionName, controllerName, relativelyUrl);
            return absoluteUrl;
        }

        public static string FullUrl(this UrlHelper urlHelper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            //相对地址
            string relativelyUrl = urlHelper.Action(actionName, controllerName, routeValues);

            //绝对地址
            string absoluteUrl = GetAbsoluteUrl(urlHelper, actionName, controllerName, relativelyUrl);
            return absoluteUrl;
        }

        public static string FullUrl(this UrlHelper urlHelper, string actionName, string controllerName, object routeValues, string protocol)
        {
            //相对地址
            string relativelyUrl = urlHelper.Action(actionName, controllerName, routeValues, protocol);

            //绝对地址
            string absoluteUrl = GetAbsoluteUrl(urlHelper, actionName, controllerName, relativelyUrl);
            return absoluteUrl;
        }

        public static string FullUrl(this UrlHelper urlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, string protocol, string hostName)
        {
            //相对地址
            string relativelyUrl = urlHelper.Action(actionName, controllerName, routeValues, protocol, hostName);

            //绝对地址
            string absoluteUrl = GetAbsoluteUrl(urlHelper, actionName, controllerName, relativelyUrl);
            return absoluteUrl;
        }


        /// <summary>
        /// 获取跳转绝对地址
        /// </summary>
        /// <param name="actionName">Action名</param>
        /// <param name="controllerName">Controller名</param>
        /// <param name="relativelyUrl">相对地址</param>
        /// <returns>Url</returns>
        private static string GetAbsoluteUrl(UrlHelper urlHelper, string actionName, string controllerName, string relativelyUrl, string protocol = "", string hostName = "")
        {
            LogUtil.Debug(string.Format("Controller:{0};Action:{1}", controllerName, actionName));

            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            IController controller = factory.CreateController(urlHelper.RequestContext, controllerName);
            Type type = controller.GetType();
            MethodInfo method = type.GetMethod(actionName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (method == null)
            {
                throw new Exception(string.Format("Controller:{0}中不存在Action:{1}", controllerName, actionName));
            }

            return relativelyUrl.ToLower();
        }

        /// <summary>
        /// 获取控制器名称
        /// </summary>
        /// <param name="urlHelper">UrlHelper</param>
        /// <returns>controller name</returns>
        private static string GetControllerName(UrlHelper urlHelper)
        {
            object value = null;
            string controllerName = string.Empty;
            RouteValueDictionary routeValue = urlHelper.RequestContext.RouteData.Values;
            if (routeValue != null && routeValue.TryGetValue("controller", out value))
            {
                controllerName = ConvertUtil.ToString(value);
            }

            if (string.IsNullOrEmpty(controllerName))
            {
                throw new MessageException("无法获取当前控制器名称");
            }

            return controllerName;
        }

        #endregion
    }
}
