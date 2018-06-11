//---------------------------------------------
// 版权信息：版权所有(C) 2015，Coolwi
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//     王军锋         2012/12/05       创建
//---------------------------------------------
using System.Web.Mvc;

namespace CWI.MCP.Controllers.DEMO
{
    /// <summary>
    /// 错误处理
    /// </summary>
    public class ErrorController : WebBaseController
    {
        /// <summary>
        /// 404页面
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Error404() 
        {
            Response.StatusCode = 404;
            return View();
        }

        /// <summary>
        /// 错误页面
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Error500()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}
