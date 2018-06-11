using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text;
using CWI.MCP.Common;
using CWI.MCP.Common.ORM;
using CWI.MCP.Models;
using CWI.MCP.Models.ViewModels.MCP.OPEN;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using Evt.Framework.Common;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;
using System.Transactions;

namespace CWI.MCP.Services
{
    /// <summary>
    /// 内容摘要：开放平台注册登录
    /// 编码作者：ZLP
    /// 编码时间：2016-7-5
    /// </summary>
    public class DeveloperService : BaseService
    {
        #region 方法区

        #region 登录

        /// <summary>
        /// 开放平台登录方法
        /// </summary>
        /// <param name="model"></param>
        public void Login(DevLoginViewModel model)
        {
            if (model == null)
            {
                throw new MessageException("登录参数不能为空");
            }

            var username = model.UserName.Trim();
            var userpwd = model.UserPwd.Trim();

            if (string.IsNullOrEmpty(username))
            {
                throw new MessageException("帐号不能为空");
            }

            if (string.IsNullOrEmpty(userpwd))
            {
                throw new MessageException("密码不能为空");
            }

            var cc = new ConditionCollection()
            {
                new Condition("mobile",username)
            };
            var developer = this.GetRepository<McpDeveloperInfo>().GetModel(cc);
            if (developer == null || !developer.UserPwd.Equals(SecurityUtil.ConvertToMD5(userpwd)))
            {
                throw new MessageException("帐号或密码错误");
            }

            //if (developer.IsActived == (int) IsActived.NoActive)
            //{
            //    throw new MessageException("您的帐号未激活，请于注册后24小时内到您的注册邮箱激活帐号");
            //}

            if (developer.StatusCode == StatusCodeType.Disabled.GetHashCode() || developer.StatusCode == StatusCodeType.Disabled.GetHashCode())
            {
                throw new MessageException("您的帐号异常，请于管理员联系");
            }

            // 如果登录成功，更新业务表的最后登陆时间和登录IP
            developer.LastLoginDate = CommonUtil.GetDBDateTime();
            developer.LastLoginIp = CommonUtil.GetLocalIpAddress();

            this.GetRepository<McpDeveloperInfo>().Update(developer);

            SessionUtil.Current = new SessionData()
            {
                Account = developer.DeveloperId,
                Mobile = developer.Mobile
            };
        }

        #endregion

        #region 注册

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="dev">视图模型</param>
        /// <returns></returns>
        public void Register(DevRegisterViewMode dev, string domain)
        {
            // 基础数据校验
            CheckViewModel(dev);

            //将数据插入开发者业务数据表
            var model = new McpDeveloperInfo
            {
                DeveloperId = CommonUtil.GetGuidNoSeparator(),
                Mobile = dev.Mobile,
                Email = dev.Email,
                UserPwd = SecurityUtil.ConvertToMD5(dev.UserPwd),
                IsActived = 1,
                StatusCode = StatusCodeType.Valid.GetHashCode(),
                CreatedOn = CommonUtil.GetDBDateTime(),
                Memo = dev.Memo
            };

            this.GetRepository<McpDeveloperInfo>().Create(model);
        }

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        private void CheckViewModel(DevRegisterViewMode dev)
        {
            // 邮箱验证
            //CheckEmail(dev.Email);

            CheckMobile(dev.Mobile);

            // 密码和确认密码验证
            CheckPwd(dev.UserPwd, dev.ReUserPwd);

            // 手机号码和手机验证码
            var verifyCodeId = SingleInstance<VerifyCodeService>.Instance.CheckUserVerifyCode(Consts.OPEN_APP_SIGN.ToLower(),
                dev.MobileCode, "", dev.Mobile);

            //3.更新验证码状态
            SingleInstance<VerifyCodeService>.Instance.UpdateVerifyCodeStatus(Consts.OPEN_APP_SIGN.ToLower(), verifyCodeId);

        }

        /// <summary>
        /// 邮箱校验
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private void CheckEmail(string email)
        {
            // 正则表达式字符串
            var emailReg = new Regex(RegexConsts.EMAIL_PATTERN);
            if (string.IsNullOrEmpty(email))
            {
                throw new MessageException("邮箱不能为空");
            }

            if(email.Length <6 || email.Length > 40)
            {
                throw new MessageException("Email的长度不小于6和不大于60个字符，请重新输入");
            }

            if (!emailReg.IsMatch(email))
            {
                throw new MessageException("邮箱格式不正确，请重新输入");
            }

            ConditionCollection cc = new ConditionCollection(){
                new Condition("mobile",email)
            };

            if(this.GetRepository<McpDeveloperInfo>().IsExists(cc))
            {
                throw new MessageException("该邮箱已经存在，请重新输入有效邮箱地址");
            }
        }

        /// <summary>
        /// 密码验证
        /// </summary>
        /// <param name="password">开发者密码</param>
        /// <param name="rePassword">确认开发者密码</param>
        /// <returns></returns>
        private void CheckPwd(string password, string rePassword)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new MessageException("密码不能为空");
            }

            if (string.IsNullOrEmpty(rePassword))
            {
                throw new MessageException("确认密码不能为空");
            }

            if (password.Length < 6 || password.Length > 16)
            {
                throw new MessageException("密码长度不能小于6或者大于16个字符，请重新输入");
            }

            if (password != rePassword)
            {
                throw new MessageException("密码和确认密码不一致，请重新输入");
            }
        }

        /// <summary>
        /// 手机号码验证
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="codeType">验证码类型：0-注册，1-忘记密码</param>
        /// <returns></returns>
        private void CheckMobile(string mobile, int codeType = 0)
        {
            var mobileReg = new Regex(RegexConsts.MOBILE_PATTERN);

            if (string.IsNullOrEmpty(mobile))
            {
                throw new MessageException("手机号码不能为空，请重新输入");
            }

            if (!mobileReg.IsMatch(mobile))
            {
                throw new MessageException("手机号码格式不正确");
            }

            var cc = new ConditionCollection();
            cc.Add(new Condition("mobile", mobile));
            var isExists = this.GetRepository<McpDeveloperInfo>().IsExists(cc);

            if (codeType == 0 && isExists)
            {
                throw new MessageException("该手机号码已经存在，请重新输入");
            }
            else if (codeType == 1 && !isExists)
            {
                throw new MessageException("该手机号码不存在，请重新输入");
            }
        }

        /// <summary>
        /// 获取短信验证码
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="codeType">验证码类型：0-注册，1-忘记密码</param>
        public void GetSmsCode(string mobile, int codeType)
        {
            CheckMobile(mobile, codeType);

            try
            {
                SingleInstance<VerifyCodeService>.Instance.GetVerifyCode(Consts.OPEN_APP_SIGN.ToLower(), VerifyCodeType.BindMobile, SmsFuncType.OpenRegister, mobile, string.Empty, mobile);
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("获取注册验证码失败，手机号码：{0}，参考信息：{1}", mobile, ex.StackTrace));
                throw new MessageException(ex.Message);
            }
        }

        #endregion

        #region 忘记密码

        #region 忘记密码之获取短信验证码【待废弃】

        /// <summary>
        /// 根据用户帐号，获取该用户的手机信息，用于发送短信验证码
        /// </summary>
        /// <param name="email"></param>
        public void GetSmsCode(string email)
        {
            #region 根据正确的账户，查找账户有关信息，并且发送短信验证码

            CheckModifyData(email);

            ConditionCollection cc = new ConditionCollection(){
                new Condition("email",email)
            };

            // 获取该用户的实体
            var developerModel = GetRepository<McpDeveloperInfo>().GetModel(cc);

            try
            {
                SingleInstance<VerifyCodeService>.Instance.GetVerifyCode(Consts.OPEN_APP_SIGN.ToLower(), VerifyCodeType.BindMobile, SmsFuncType.OpenRegister, CommonUtil.GetRemoteIPAddress(), string.Empty, developerModel.Mobile);
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("获取重置密码验证码失败，手机号码：{0}，参考信息：{1}", developerModel.Mobile, ex.StackTrace));
                throw new MessageException(ex.Message);
            }

            #endregion
        }

        #endregion

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="model">有关用户信息实体</param>
        public void ForgetPassword(DevRegisterViewMode model)
        {
            var ccWhere = new ConditionCollection(){
                new Condition("mobile",model.Mobile)
            };

            using (TransactionScope tran = new TransactionScope())
            {

                CheckModifyData(model.Mobile);

                // 获取该用户的实体
                var developerModel = GetRepository<McpDeveloperInfo>().GetModel(ccWhere);

                #region 短信验证码的校验和密码的校验
                // 手机号码和手机验证码
                var verifyCodeId = SingleInstance<VerifyCodeService>.Instance.CheckUserVerifyCode(Consts.OPEN_APP_SIGN.ToLower(),
                    model.MobileCode, "", developerModel.Mobile);

                //3.更新验证码状态
                SingleInstance<VerifyCodeService>.Instance.UpdateVerifyCodeStatus(Consts.OPEN_APP_SIGN.ToLower(), verifyCodeId);

                // 执行密码校验
                CheckPwd(model.UserPwd, model.ReUserPwd);

                #endregion

                #region 执行密码修改操作

                developerModel.UserPwd = SecurityUtil.ConvertToMD5(model.UserPwd);

                this.GetRepository<McpDeveloperInfo>().Update(developerModel);

                #endregion

                tran.Complete();
            }

        }

        /// <summary>
        /// 校验密码重置的条件是否满足【待废弃】
        /// </summary>
        /// <param name="key">加密字符串</param>
        /// <param name="password">密码</param>
        /// <param name="rePassword">重新输入密码</param>
        public void DoModifyPass(string key,string password,string rePassword)
        {
            var message = "密码修改";

            var paramInfo = HttpUtility.UrlDecode(SecurityUtil.DESDecode(key));
            if (!string.IsNullOrEmpty(paramInfo))
            {
                paramInfo = paramInfo.Substring(paramInfo.IndexOf('=') + 1);
                var verifyId = paramInfo.Substring(0, paramInfo.IndexOf('&'));
                paramInfo = paramInfo.Substring(paramInfo.IndexOf('=') + 1);
                var modifyCode = paramInfo.Substring(0, paramInfo.IndexOf('&'));
                var email = paramInfo.Substring(paramInfo.IndexOf('=') + 1);

                // 执行校验邮件连接参数
                CheckVerifyCode(modifyCode, email, verifyId, message);
                // 执行密码校验
                CheckPwd(password, rePassword);

                // 执行修改密码操作
                var cc = new ConditionCollection(){
                    new Condition("mobile",email)
                };

                var developer = this.GetRepository<McpDeveloperInfo>().GetModel(cc);
                developer.UserPwd = SecurityUtil.ConvertToMD5(password);

                this. GetRepository<McpDeveloperInfo>().Update(developer);
            }
            else
            {
                throw new MessageException("连接地址有误，请于管理员联系");
            }
        }

        #endregion

        #region 激活成功 【待废弃】

        /// <summary>
        /// 激活用户信息验证
        /// </summary>
        /// <param name="key">激活参数串</param>
        public string ActiveUserInfo(string key)
        {
            var email = "";
            var paramInfo = HttpUtility.UrlDecode(SecurityUtil.DESDecode(key));
            if (!string.IsNullOrEmpty(paramInfo))
            {
                paramInfo = paramInfo.Substring(paramInfo.IndexOf('=') + 1);
                var verifyId = paramInfo.Substring(0, paramInfo.IndexOf('&'));
                paramInfo = paramInfo.Substring(paramInfo.IndexOf('=') + 1);
                var activeCode = paramInfo.Substring(0, paramInfo.IndexOf('&'));
                email = paramInfo.Substring(paramInfo.IndexOf('=') + 1);

                var message = "激活用户";
                CheckVerifyCode(activeCode, email, verifyId, message);

                // 更新开发者数据表数据
                var condit = new ConditionCollection()
                {
                    new Condition("mobile", email)
                };

                var developer = GetRepository<McpDeveloperInfo>().GetModel(condit);

                developer.IsActived = (int) IsActived.Active;
                developer.ActivedDatetime = CommonUtil.GetDBDateTime();

                this.GetRepository<McpDeveloperInfo>().Update(developer);

                return email;
            }
            else
            {
                throw new MessageException("连接地址有误，请于管理员联系");
            }
        }

        #endregion

        #region 有关校验邮件验证码异常

        /// <summary>
        /// 校验邮件验证码
        /// </summary>
        /// <param name="code">激活码</param>
        /// <param name="email">邮件</param>
        /// <param name="verifyId">验证码ID</param>
        /// <param name="message">操作信息</param>
        private void CheckVerifyCode(string code, string email, string verifyId,string message)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(verifyId))
            {
                throw new MessageException(message +"参数有误，请重新提交");
            }

            var conditions = new ConditionCollection()
            {
                new Condition("verify_code_id", verifyId)
            };
            var activeInfo = this.GetRepository<McpSysVerifycodeInfo>().GetModel(conditions);
            if (activeInfo != null)
            {
                if ((activeInfo.VerifiedDate - CommonUtil.GetDBDateTime()).Days >= 1)
                {
                    throw new MessageException(message + "已经失效，请重新提交");
                }

                if (activeInfo.VerifyCode != HttpUtility.UrlDecode(code))
                {
                    throw new MessageException(message + "验证码不正确，请重新提交");
                }

                #region 更新验证码业务表数据

                activeInfo.Verified = 1;
                activeInfo.ModifiedOn = CommonUtil.GetDBDateTime();

                this.GetRepository<McpSysVerifycodeInfo>().Update(activeInfo);

                #endregion
            }
            else
            {
                throw new MessageException(message + "参数不存在，请重新提交");
            }
        }

        #endregion

        #region 私有方法

        private void CheckModifyData(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                throw new MessageException("手机号码不能为空");
            }

            var mobileReg = new Regex(RegexConsts.MOBILE_PATTERN);
            if (!mobileReg.IsMatch(mobile))
            {
                throw new MessageException("手机号码格式不正确，请重新输入");
            }

            ConditionCollection cc = new ConditionCollection(){
                new Condition("mobile",mobile)
            };

            if (!this.GetRepository<McpDeveloperInfo>().IsExists(cc))
            {
                throw new MessageException("该用户不存在，请重新输入");
            }

            var verifyModel = GetRepository<McpDeveloperInfo>().GetModel(cc);

            if (verifyModel.IsActived == (int)IsActived.NoActive)
            {
                throw new MessageException("该帐号未进行激活，请激活后进行重置密码");
            }
        }

        #endregion

        #endregion
    }
}
