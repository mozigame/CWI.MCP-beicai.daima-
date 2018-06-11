using System;
using System.IO;
using System.Web.Mvc;
using CWI.MCP.Common;
using CWI.MCP.Models.Common;
using CWI.MCP.Models.ViewModels.MCP.OPEN;
using CWI.MCP.Services;
using Evt.Framework.Common;

namespace CWI.MCP.Controllers.OPEN
{
    /// <summary>
    /// 内容摘要：用户控制器
    /// 编码作者：ZLP
    /// 编码时间：2016-7-5
    /// </summary>
    public class UserController : WebBaseController
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns>ActionResult</returns>
        [NonAuthorized]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 执行登录
        /// </summary>
        /// <param name="dev">管理员登录信息</param>
        /// <returns>ActionResult</returns>
        [NonAuthorized]
        [HttpPost]
        public ActionResult DoLogin(DevLoginViewModel dev)
        {
            SingleInstance<DeveloperService>.Instance.Login(dev);
            return OK();
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns>ActionResult</returns>
        [NonAuthorized]
        public ActionResult Logout()
        {
            SessionUtil.ClearSession();
            return OK();
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        /// <summary>
        /// 执行忘记密码方法
        /// </summary>
        /// <param name="model">邮件地址</param>
        /// <returns></returns>
        [NonAuthorized]
        public ActionResult DoForgetPassword(DevRegisterViewMode model)
        {
            try
            {
                SessionUtil.CheckValidateCode(model.PicCode.Trim());
                SingleInstance<DeveloperService>.Instance.ForgetPassword(model);
                return OK();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 注册页面
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 执行注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [NonAuthorized]
        public ActionResult DoRegister(DevRegisterViewMode model)
        {
            try
            {
                SessionUtil.CheckValidateCode(model.PicCode.Trim());
                SingleInstance<DeveloperService>.Instance.Register(model, DomailUrl);
                return OK();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 跳转注册成功界面
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [NonAuthorized]
        public ActionResult RegisterSuccess(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns>ActionResult</returns>
        [NonAuthorized]
        public ActionResult AuthenticationCode()
        {
            var strCode = VerifyCodeUtil.Intance.GenCode(Consts.WEB_VALIDATECODE_LENGTH);
            SessionUtil.AuthenticationCode = SecurityUtil.ConvertToMD5(strCode.ToLower());
            MemoryStream ms = VerifyCodeUtil.Intance.CreateCheckCodeImage(strCode);
            Response.ClearContent();
            Response.ContentType = "image/gif";
            Response.BinaryWrite(ms.ToArray());
            Response.End();
            return View("~/Views/User/AuthenticationCode.cshtml");
        }

        /// <summary>
        /// 获取短信验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        [HttpPost]
        [NonAuthorized]
        public ActionResult GetSmsVerifyCode(string mobile, string picCode, int codeType)
        {
            try
            {
                SessionUtil.CheckValidateCode(picCode);
                SingleInstance<DeveloperService>.Instance.GetSmsCode(mobile, codeType);
                return OK();
            }
            catch(Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 协议页面
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]
        public ActionResult Agreement()
        {
            return View();
        }

        /// <summary>
        /// 渲染激活视图
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]
        public ActionResult ActiveSuccess()
        {
            return View();
        }

        /// <summary>
        /// 激活成功页面
        /// </summary>
        /// <param name="key">参数串</param>
        /// <returns></returns>
        [HttpPost]
        [NonAuthorized]
        public ActionResult DoActiveSuccess(string key)
        {
            var email = SingleInstance<DeveloperService>.Instance.ActiveUserInfo(key);
            ViewBag.Email = email;
            return OK();
        }

    }
}
