//-------------------------------------------------------
//  版权信息：版权所有(C) 2011，Evervictory Tech
//  变更历史：
//      姓名           日期              说明
// -------------------------------------------------------
//     王军锋          2011/12/28       创建

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CWI.MCP.Common;
using Evt.Framework.Common;
using Evt.Framework.Mvc;

namespace CWI.MCP.Controllers
{
     /// <summary>
    /// 后台控制器基类，后台的所有控制器需要继续自它
    /// </summary>
    public class WebBaseController : AbstractController
    {
        /// <summary>
        /// 重写父类的扩展授权方法，用来统一处理后台控制器的授权
        /// </summary>
        /// <param name="filterContext">授权上下文对象</param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            object[] auth = filterContext.ActionDescriptor.GetCustomAttributes(typeof(NonAuthorizedAttribute), false);
            if (auth.Length <= 0)
            {
                if (SessionUtil.Current == null && Request.RawUrl != "/favicon.ico")
                {
                    LogUtil.Warn(Request.RawUrl);
                    throw new AuthorizationException("会话超时，请重新登录！");
                }
            }
        }

        /// <summary>
        /// 处理进行Ajax请求时产生的异常
        /// </summary>
        /// <param name="filterContext">异常上下文对象</param>
        protected override void HandleAjaxException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is MessageException)
            {
                LogUtil.Warn(filterContext.Exception.ToString());
                ResponseUtil.ResponseTextHtml("{\"status\":0,\"data\":\"" + filterContext.Exception.Message + "\"}");
            }
            else if (filterContext.Exception is AuthorizationException)
            {
                ResponseUtil.ResponseTextHtml("{\"status\":3,\"data\":\"/User/Login\"}");
            }
            else if (filterContext.Exception is BinderException)
            {
                LogUtil.Info(filterContext.Exception.ToString());
                BinderException be = (BinderException)filterContext.Exception;
                if (be.Code == "0")
                {
                    ResponseUtil.ResponseTextHtml("<script>parent.P.Set.TempCallBack('{\"status\":0,\"data\":\"" + filterContext.Exception.Message + "\"}');</script>");
                }
            }
            else
            {
                LogUtil.Error(filterContext.Exception.ToString());
                ResponseUtil.ResponseTextHtml("{\"status\":4,\"data\":\"" + filterContext.Exception.Message + "\"}");
            }

            filterContext.Result = null;
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = 200;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        /// <summary>
        /// 处理非Ajax请求时产生的异常
        /// </summary>
        /// <param name="filterContext">异常上下文对象</param>
        protected override void HandleException(ExceptionContext filterContext)
        {
            // 下面是临时演示，以后需按异常类型完善
            if (filterContext.Exception is MessageException)
            {
                LogUtil.Warn(filterContext.Exception.ToString());
                //Response.Write("<div style=\"padding-top:100px; margin:0 auto; text-align:center;\">");
                //Response.Write("抱歉，发生了错误，参考消息：<span style='color:red;'>" + filterContext.Exception.Message + "</span>");
                //Response.Write("<br /><br /><a href=\"javascript:history.back();\">单击此处返回</a>");
                //Response.Write("</div>");
            }
            else if (filterContext.Exception is AuthorizationException)
            {
               // Response.Redirect("/User/Login", true);
                Response.Redirect("/Apply/ApplyPrompt", true);
            }
            else if (filterContext.Exception is BinderException)
            {
                LogUtil.Info(filterContext.Exception.ToString());
                BinderException be = (BinderException)filterContext.Exception;
                if (be.Code == "0")
                {
                    ResponseUtil.ResponseTextHtml("<script>parent.P.Set.TempCallBack('{\"status\":0,\"data\":\"" + filterContext.Exception.Message + "\"}');</script>");
                }
            }
            else
            {
                //此处日志由父类记录，避免重复记录
                //LogUtil.Error(filterContext.Exception.ToString());
                // 注要：如果想通过url传递任何参数，一定要防止注入攻击
                base.HandleException(filterContext);
            }

            ViewBag.Message = filterContext.Exception.Message;
            ViewBag.StackTrace = filterContext.Exception.StackTrace;
            filterContext.Result = View("Error");
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}