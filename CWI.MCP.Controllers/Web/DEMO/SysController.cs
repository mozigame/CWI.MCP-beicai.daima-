using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CWI.MCP.Common;
using CWI.MCP.Models;
using CWI.MCP.Models.Common;
using CWI.MCP.Models.ViewModels.MCP.OPEN;
using CWI.MCP.Services;
using Evt.Framework.Common;
using Newtonsoft.Json;

namespace CWI.MCP.Controllers.DEMO
{
    public class SysController : WebBaseController
    {
        #region 页面信息

        private static string apiurl = ConfigUtil.GetConfig("ApiDomainUrl");

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns>ActionResult</returns>
        [NonAuthorized]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 基础设置
        /// </summary>
        /// <returns>ActionResult</returns>
        [NonAuthorized]
        public ActionResult AccessToken()
        {
            return View();
        }

        /// <summary>
        /// 关联打印设备
        /// </summary>
        /// <returns>ActionResult</returns>
        [NonAuthorized]
        public ActionResult BindPrinter()
        {
            return View();
        }

        /// <summary>
        /// 解绑打印设备
        /// </summary>
        /// <returns>ActionResult</returns>
        [NonAuthorized]
        public ActionResult UnBindPrinter()
        {
            return View();
        }

        /// <summary>
        /// 打印小票
        /// </summary>
        /// <returns>ActionResult</returns>
        [NonAuthorized]
        public ActionResult PrintBill()
        {
            return View();
        }

        /// <summary>
        /// 检验打印设备是否可关联
        /// </summary>
        /// <returns>ActionResult</returns>
        [NonAuthorized]
        public ActionResult ChkPrintersEnableBind()
        {
            return View();
        }

        #endregion

        #region 执行动作

        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        [NonAuthorized]
        [HttpGet]
        public object GetAccessToken(AppViewModel appModel)
        {
            var url = string.Format("{0}/mcp/sys/getAccessToken", apiurl);
            var parms = string.Format("app_id={0}&app_key={1}", appModel.app_id.Trim(), appModel.app_key.Trim());
            var result = NetUtil.ResponseByGet(url, parms);
            ViewBag.Result = result;
            return View("AccessToken");
        }

        /// <summary>
        /// 关联打印设备
        /// </summary>
        /// <param name="bindModel"></param>
        /// <returns></returns>
        [NonAuthorized]
        [HttpPost]
        public object DoBindPrinter(PrintViewModel bindModel)
        {
            var url = string.Format("{0}/mcp/sys/bindPrinters", apiurl);
            var parms = string.Format("app_id={0}&access_token={1}&merchant_code={2}&printer_codes={3}",
                bindModel.app_id.Trim(), bindModel.access_token.Trim(), bindModel.merchant_code.Trim(), bindModel.printer_codes.Trim());
            var result = NetUtil.ResponseByPost(url, parms);
            ViewBag.Result = result;
            return View("BindPrinter");
        }

        /// <summary>
        /// 解绑打印设备
        /// </summary>
        /// <param name="unBindModel"></param>
        /// <returns></returns>
        [NonAuthorized]
        [HttpPost]
        public object DoUnBindPrinter(PrinterBaseViewModel unBindModel)
        {
            var url = string.Format("{0}/mcp/sys/unBindPrinters", apiurl);
            var parms = string.Format("access_token={0}&merchant_code={1}&printer_codes={2}",
                unBindModel.access_token.Trim(), unBindModel.merchant_code.Trim(), unBindModel.printer_codes.Trim());
            var result = NetUtil.ResponseByPost(url, parms);
            ViewBag.Result = result;
            return View("UnBindPrinter");
        }

        /// <summary>
        /// 打印票据
        /// </summary>
        /// <param name="billModel"></param>
        /// <returns></returns>
        [NonAuthorized]
        [HttpPost]
        public object DoPrint(BillViewModel billModel)
        {
            var url = string.Format("{0}/mcp/sys/print", apiurl);
            var parms = string.Format("app_id={0}&access_token={1}&merchant_code={2}&printer_codes={3}&copies={4}&bill_no={5}&bill_type={6}&template_id={7}&bill_content={8}",
                billModel.app_id.Trim(), billModel.access_token.Trim(), billModel.merchant_code.Trim(), billModel.printer_codes.Trim(),
                billModel.copies.Trim(), billModel.bill_no.Trim(), billModel.bill_type.Trim(),
                billModel.template_id.Trim(), HttpUtility.UrlEncode(billModel.bill_content.Trim(), Encoding.UTF8));
            var result = NetUtil.ResponseByPost(url, parms);
            ViewBag.Result = result;
            return View("PrintBill");
        }

        /// <summary>
        /// 检验打印设备是否可关联
        /// </summary>
        /// <param name="chkModel"></param>
        /// <returns></returns>
        [NonAuthorized]
        [HttpPost]
        public object DoChkPrintersEnableBind(PrinterCheckViewModel chkModel)
        {
            var url = string.Format("{0}/mcp/sys/chkPrintersEnableBind", apiurl);
            var parms = string.Format("access_token={0}&printer_codes={1}",
                chkModel.access_token.Trim(), chkModel.printer_codes.Trim());
            var result = NetUtil.ResponseByPost(url, parms);
            ViewBag.Result = result;
            return View("ChkPrintersEnableBind");
        }

        /// <summary>
        /// 业务回调
        /// </summary>
        /// <param name="callbackModel"></param>
        /// <returns></returns>
        [NonAuthorized]
        [HttpPost]
        public object CallBack()
        {
            var sign = Request.Form["sign"] as string;
            var sbQuest = new StringBuilder();
            var billNo = Request.Form["bill_no"] as string;
            var printerCode = Request.Form["printer_code"] as string;
            var resultCode = Request.Form["result_code"] as string;
            var nonceStr = Request.Form["nonce_str"] as string;
            var signType = Request.Form["sign_type"] as string;
            var timeStamp = Request.Form["time_stamp"] as string;

            sbQuest.AppendFormat("{0}={1}&", "bill_no", billNo);
            sbQuest.AppendFormat("{0}={1}&", "nonce_str", nonceStr);
            sbQuest.AppendFormat("{0}={1}&", "printer_code", printerCode);
            sbQuest.AppendFormat("{0}={1}&", "result_code", resultCode);
            sbQuest.AppendFormat("{0}={1}&", "sign_type", signType);
            sbQuest.AppendFormat("{0}={1}&", "time_stamp", timeStamp);
            var parms = sbQuest.ToString().TrimEnd('&');
            LogUtil.Debug(string.Format("请求参数：{0}", parms));

            var key = ConfigUtil.GetConfig("SignKey");
            var mySign = CommonUtil.GetMD5Sign(key, parms);
            LogUtil.Debug(string.Format("传入签名：{0}，计算得到签名：{1}", sign, mySign));
            var msg = string.Empty;
            if (sign.Equals(mySign, System.StringComparison.CurrentCultureIgnoreCase))
            {
                string sql = "UPDATE print_order SET result_code=$result_code$,modfied_on=NOW() WHERE bill_no=$bill_no$ AND printer_code=$printer_code$";
                var pc = new ParameterCollection();
                pc.Add("bill_no", billNo);
                pc.Add("printer_code", printerCode);
                pc.Add("result_code", resultCode);

                try
                {
                    msg = "更新业务数据成功！";
                    DbUtil.DataManager.DataManagerBud.IData.ExecuteNonQuery(sql, pc);
                    LogUtil.Debug(msg);
                    return OK(msg);
                }
                catch (Exception ex)
                {
                    msg = string.Format("更新业务数据失败，参考信息：{0}！", ex.ToString());
                    LogUtil.Debug(msg);
                    return Error(msg);
                }
            }
            else
            {
                msg = "非法请求！";
                LogUtil.Debug(msg);
                return Error(msg);
            }
        }

        #endregion
    }
}
