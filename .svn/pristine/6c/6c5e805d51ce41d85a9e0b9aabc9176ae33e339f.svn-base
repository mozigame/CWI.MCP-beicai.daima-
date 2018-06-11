//---------------------------------------------
// 版权信息：版权所有(C) 2015，Coolwi
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//            2012/09/13          创建
//---------------------------------------------
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CWI.MCP.Common;
using CWI.MCP.Controllers.IOT;
using CWI.MCP.Common.Extensions.MVC;

namespace CWI.MCP.IOT
{
    /// <summary>
    /// MvcApplication
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 注册全局过滤器
        /// </summary>
        /// <param name="filters">全局过滤器集合</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            //404
            routes.MapRoute("Error404"
                , "Error404"
                , new { controller = "Error", action = "Error404" }
                , null
                , new string[] { "CWI.MCP.Controllers.IOT" });

            //Error
            routes.MapRoute("Error500"
                , "Error500"
                , new { controller = "Error", action = "Error500" }
                , null
                , new string[] { "CWI.MCP.Controllers.IOT" });

            //通用列表【页码】
            routes.MapRoute("Page-List"
                , "{controller}/{action}/{pageIndex}"
                , null
                , new { pageIndex = RegexConsts.INT_FOR_GREAT_ZERO }
                , new string[] { "CWI.MCP.Controllers.IOT" });

            //默认及登录页
            routes.MapRoute(
                "Default", 
                "{controller}/{action}/{id}",
                new { controller = "System", action = "Message", id = UrlParameter.Optional }, 
                new string[] { "CWI.MCP.Controllers.IOT" });
        }

        /// <summary>
        /// 网站启动后第一个请求到达时执行
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// 捕获发生在应用程序中的错误
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        protected void Application_Error(object sender, EventArgs e)
        {
#if DEBUG
            Exception ex = Server.GetLastError();
            string message = string.Empty;
            if (ex != null)
            {
                if (ex is HttpException && ((HttpException)ex).GetHttpCode() == 404)
                {
                    LogUtil.Info(ex.ToString());
                }
                else
                {
                    LogUtil.Error("Application_Error:" + ex.ToString());
                }
                message = ex.ToString();
            }
            Server.ClearError();
            ResponseUtil.ResponseTextHtml("{\"status\":4,\"data\":\"" + message + "\"}");
            HttpContext.Current.Response.StatusCode = 500;
            HttpContext.Current.Response.TrySkipIisCustomErrors = true;

#else
            Exception exception = Server.GetLastError();
            Response.Clear();

            if (exception is HttpException && ((HttpException)exception).GetHttpCode() == 404)
            {
                Redirect404();
                LogUtil.Info(exception.ToString());
            }
            else
            {
                Redirect500();
                LogUtil.Error(exception.ToString());
            }

            //清除Exception,避免继续传递给上一级处理
            Server.ClearError();
            Response.TrySkipIisCustomErrors = true;
#endif
        }

        /// <summary>
        /// 创建每一个新会话时执行
        /// </summary>
        protected void Session_Start()
        {
            Response.Cookies["MCP_IOT_SessionId"].Value = HttpContext.Current.Session.SessionID;
            SessionUtil.AppSign = "MCP_IOT";
        }

        #region 私有

        /// <summary>
        /// 跳转404
        /// </summary>
        private void Redirect404()
        {
            //Response.StatusCode = 404;
            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "Error404");
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }

        /// <summary>
        /// 跳转500
        /// </summary>
        private void Redirect500()
        {
            RouteData routeData = new RouteData();
            UrlHelper urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(Context), routeData));
            string url = urlHelper.FullUrl("Error500", "Error");
            Response.Redirect(url);
        }

        #endregion
    }
}