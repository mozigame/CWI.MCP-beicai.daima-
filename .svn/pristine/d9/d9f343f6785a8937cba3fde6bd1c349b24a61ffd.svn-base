// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期          说明
// --------------------------------------------
//      王军锋     2012/06/29       创建
// --------------------------------------------
//      王军锋   2012/07/14       优化，获得城市ID的算法
// --------------------------------------------
//      王军锋   2012/07/17       增加，城市子域名和城市ID双向互转系列方法
// --------------------------------------------
//      王军锋   2013/07/10       增加，获取Controller、Action、GetUrlData、GetFormData、GetRouteData方法
// --------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Evt.Framework.Common;
using System.Data;
using CWI.MCP.Common.Exceptions;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// Request请求
    /// </summary>
    public class RequestUtil
    {
        /// <summary>
        /// 取路由中的ID值
        /// </summary>
        public static string Id
        {
            get
            {
                string id = GetRouteData("id");
                return id == null ? string.Empty : id;
            }
        }

        /// <summary>
        /// 取路由中的pageIndex值
        /// </summary>
        public static string PageIndex
        {
            get
            {
                string id = GetRouteData("pageIndex");
                return id == null ? string.Empty : id;
            }
        }

        /// <summary>
        /// 取路由中的控制器名称
        /// </summary>
        public static string Controller
        {
            get
            {
                string controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"] as string;
                return controller == null ? string.Empty : controller;
            }
        }

        /// <summary>
        ///  取路由中的Action名称
        /// </summary>
        public static string Action
        {
            get
            {
                string action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"] as string;
                return action == null ? string.Empty : action;
            }
        }

        /// <summary>
        /// 取路由中参数值
        /// </summary>
        /// <param name="key">参数key</param>
        /// <returns>参数值</returns>
        public static string GetRouteData(string key)
        {
            string data = HttpContext.Current.Request.RequestContext.RouteData.Values[key] as string;
            return data;
        }

        /// <summary>
        /// 取URL地址中QueryString参数值
        /// </summary>
        /// <param name="key">参数key</param>
        /// <returns>参数值</returns>
        public static string GetUrlData(string key)
        {
            return HttpContext.Current.Request.QueryString[key];
        }

        /// <summary>
        /// 取URL地址中参数值
        /// </summary>
        /// <param name="key">参数key</param>
        /// <returns>参数值</returns>
        public static string GetFormData(string key)
        {
            return HttpContext.Current.Request.Form[key];
        }
    }
}
