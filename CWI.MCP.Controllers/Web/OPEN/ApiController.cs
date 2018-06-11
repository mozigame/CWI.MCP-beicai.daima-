using System.Web.Mvc;
using CWI.MCP.Controllers;
using Evt.Framework.Common;

namespace CWI.MCP.Controllers.OPEN
{
    /// <summary>
    /// 内容摘要：API说明控制器
    /// 编码作者：ZLP
    /// 编码时间：2016-7-15
    /// </summary>
    public class ApiController : WebBaseController
    {
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]        
        public ActionResult AccessToken()
        {
            return View();
        }

        /// <summary>
        /// 关联打印设备
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]        
        public ActionResult BindPrinters()
        {
            return View();
        }

        /// <summary>
        /// 解绑打印设备
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]        
        public ActionResult UnBindPrinters()
        {
            return View();
        }

        /// <summary>
        /// 打印小票
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]        
        public ActionResult PrintTicket()
        {
            ViewBag.DomanUrl = DomailUrl;
            return View();
        }

        /// <summary>
        /// 打印快递面单
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]        
        public ActionResult PrintExpress()
        {
            return View();
        }

        /// <summary>
        /// 检验打印设备是否可关联
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]        
        public ActionResult ChkPrintersEnableBind()
        {
            return View();
        }
    }
}