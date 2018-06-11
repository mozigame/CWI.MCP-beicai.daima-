using System.Web.Mvc;
using CWI.MCP.Common;
using CWI.MCP.Controllers;
using CWI.MCP.Services.MCP;
using Evt.Framework.Common;

namespace CWI.MCP.Controllers.OPEN
{
    public class ExpressController : WebBaseController
    {
        #region 快递单

        /// <summary>
        /// 快递单
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        [NonAuthorized]   
        public ActionResult ExpressInfo(string templateId)
        {
            return View();
        }


        /// <summary>
        /// 快递商
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]   
        [HttpPost]
        public string GetExpressList()
        {
            var expressList = SingleInstance<UserAccountService>.Instance.GetExpressList();
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { rows = expressList });
        }

        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <param name="expressId">快递商Id</param>
        /// <returns></returns>
        [NonAuthorized]   
        [HttpPost]
        public string GetExpressTemplateList(string expressId)
        {
            var templateList = SingleInstance<UserAccountService>.Instance.GetExpressTemplateList(expressId);
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { rows = templateList });
        }

        /// <summary>
        /// 根据模板Id获取模板实体
        /// </summary>
        /// <param name="templateId">模板Id</param>
        /// <returns></returns>
        [NonAuthorized]   
        public string GetExpressTemplate(string templateId)
        {
            return
                Newtonsoft.Json.JsonConvert.SerializeObject(
                    SingleInstance<UserAccountService>.Instance.GetExpressTemplate(templateId));
        }

        /// <summary>
        /// 获取模板参数
        /// </summary>
        /// <param name="templateId">模板Id</param>
        /// <returns></returns>
        [NonAuthorized]   
        [HttpPost]
        public string GetTemplateParams(string templateId)
        {
            var paramsList =
            SingleInstance<UserAccountService>.Instance.GetExpressTemplateParamsList(templateId);
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { rows = paramsList });
        }

        #endregion
    }
}