using System.Web.Mvc;
using Evt.Framework.Common;

namespace CWI.MCP.Controllers.BPS
{
    public class UserController : WebBaseController
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [NonAuthorized]
        public ActionResult Login()
        {
            return View();
        }
    }
}
