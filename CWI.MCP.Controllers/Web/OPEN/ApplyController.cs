using System.Web.Mvc;
using CWI.MCP.Common;
using CWI.MCP.Controllers;
using CWI.MCP.Models.ViewModels.MCP.OPEN;
using CWI.MCP.Services.MCP;
using Evt.Framework.Common;

namespace CWI.MCP.Controllers.OPEN
{
    public class ApplyController : WebBaseController
    {
        #region 有关应用管理

        /// <summary>
        /// 应用管理
        /// </summary>
        /// <returns></returns>
        public ActionResult ApplyManagement()
        {
            return View(SingleInstance<UserAccountService>.Instance.GetApplyModelList());
        }

        /// <summary>
        /// 新增应用界面
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult AddApplyInfo(string appId)
        {
            if (string.IsNullOrEmpty(appId))
            {
                return View();
            }
            else
            {
                return View(SingleInstance<UserAccountService>.Instance.GetApplyViewModel(appId));
            }
        }

        /// <summary>
        /// 新增应用界面
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DoApplyInfo(DevApplyViewModel dev)
        {
            var result = SingleInstance<UserAccountService>.Instance.AddApplyInfo(dev);
            if (result.Result)
            {
                var message = @"<script>window.location.href='/apply/applymanagement';</script>";
                return Content(message);
            }
            else
            {
                var msg = getAlertDailog(result.Msg);
                return Content(msg);
            }
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <returns></returns>
        public ActionResult SettingApply(string appId)
        {
            return View(SingleInstance<UserAccountService>.Instance.GetApplyViewModel(appId));
        }


        /// <summary>
        /// 平台首页
        /// </summary>
        /// <returns>ActionResult</returns>
        [NonAuthorized]

        public ActionResult ApplyPrompt()
        {
            return View();
        }


        /// <summary>
        /// 删除应用
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <returns></returns>
        public ActionResult DropApply(string appId)
        {
            SingleInstance<UserAccountService>.Instance.DropApplyInfo(appId);
            return OK();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 构造模式窗口信息
        /// </summary>
        /// <param name="msg">需要弹出的信息</param>
        /// <returns></returns>
        private string getAlertDailog(string msg)
        {
            return @"<style>
		               body{width: 100%;padding: 0;margin: 0;box-sizing: border-box;font-size: 14px;color: #555;}
		               .dialog{position: fixed;width: 100%;height: 100%;}
		               .dialog-mask-transparent{position: absolute;width: 100%;height: 100%;background: #eee;z-index:1;}
		               .dialog-content{width: 20%;margin: 150px auto;z-index: 2;position: relative;}
		               .dialog-base .dialog-content{background: #31C7A0;border-radius: 5px;color: #fff;border:1px solid #2BAD8A;}
		               .dialog-heading{border:1px solid #ccc;padding: 10px 15px;border-radius: 5px 5px 0px 0px;}
		               .dialog-body{padding: 10px 15px;border-left: 1px solid #ccc;border-right:1px solid #ccc;}
		               .dialog-footer{border-top: 1px solid #2bad8a;overflow: hidden;}
		               .dialog-btn{height: 35px;line-height: 35px;width: 50%;float: left;text-align: center;display: block;color: #fff;}
		               .dialog-btn:first-child{border-right: 1px solid #2bad8a;}
		               .dialog-btn:hover{text-decoration: none;background: #2bad8a;}
		               .dialog-open .dialog-content{border:1px solid #ccc;padding: 10px;border-radius: 5px;background: #fff;}
		               .dialog-open .dialog-heading{color: #555;text-align: center;border: 0 none;border-radius: 0;}
		               .dialog-open .dialog-body{border-left: 0 none; border-right: 0 none;}
		               .dialog-open .dialog-footer{border-top: 0 none;margin:20px -11px -11px;}
		               .dialog-open .dialog-btn{width: 100%;color: #555;border:1px solid #ccc;border-radius: 0px 0px 5px 5px;text-decoration: none;}
		               .dialog-open .dialog-btn:hover{background: #eee;color: #000;}
	               </style>
                   <div class='dialog dialog-open'>
	                  <div class='dialog-mask-transparent'></div>
	                    <div class='dialog-content'>
		                   <div class='dialog-heading'>提示</div>
		                      <div class='dialog-body'>" + msg + @"</div>
		                   <div class='dialog-footer'>
			                 <a href='javascript:void(0);' onclick='history.go(-1);' class='dialog-btn'>确定</a>
		                   </div>
	                    </div>
                  </div>";
        }

        #endregion
    }
}