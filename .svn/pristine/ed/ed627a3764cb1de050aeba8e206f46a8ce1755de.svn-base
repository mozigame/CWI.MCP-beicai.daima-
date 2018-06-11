//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/18        创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Evt.Framework.Common;
using CWI.MCP.Models;
using CWI.MCP.Common;
using CWI.MCP.Services;

namespace CWI.MCP.Services
{
    public class VerifyCodeService : BaseService
    {
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>已有的验证码</returns>
        public void GetVerifyCode(string appSign, VerifyCodeType verifyCodeType, SmsFuncType tempType, string terminalSign, string email, string mobile)
        {
            // 1.校验请求参数
            CheckGetUserVerifyCode(appSign, verifyCodeType, terminalSign, email, mobile);

            // 2.获取新验证码
            string verifyCode = SingleInstance<VerifyCodeService>.Instance.BuildVerifyCode(appSign, terminalSign, email, mobile);

            // 3.调用发送邮件接口,发送验证码
            string title = string.Empty;
            string content = string.Empty;
            EmailType emlType = EmailType.UnKnown;
            SMSType smsType = SMSType.UnKnown;
            decimal validateCodeExpire = ConfigUtil.ValidatecodeExpire / 60;
            switch (verifyCodeType)
            {
                case VerifyCodeType.BindEmail:
                    {
                        emlType = EmailType.Bind;
                        title = "yingmei.me邮箱验证";
                        content = string.Format(TemplateConfigUtil.EmlDic["BindEmail"], verifyCode, validateCodeExpire);
                        break;
                    }
                case VerifyCodeType.BindMobile:
                    {
                        smsType = SMSType.Bind;
                        content = string.Format(TemplateConfigUtil.EmlDic["SmsValidCode"], verifyCode, validateCodeExpire);
                        break;
                    }
                case VerifyCodeType.ModifyPwdByMobile:
                    {
                        smsType = SMSType.Bind;
                        content = string.Format(TemplateConfigUtil.EmlDic["SmsValidCode"], verifyCode, validateCodeExpire);
                        break;
                    }
                case VerifyCodeType.GetPwdByMobile:
                    {
                        smsType = SMSType.Bind;
                        content = string.Format(TemplateConfigUtil.EmlDic["SmsValidCode"], verifyCode, validateCodeExpire);
                        break;
                    }
                case VerifyCodeType.UpdateBindMobile:
                    {
                        smsType = SMSType.Bind;
                        content = string.Format(TemplateConfigUtil.EmlDic["SmsValidCode"], verifyCode, validateCodeExpire);
                        break;
                    }
                default:
                    {
                        throw new MessageException("未知的验证码类型！");
                    }
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                int codeType = (int)verifyCodeType;
                if (codeType == (int)VerifyCodeType.BindEmail)
                {
                    //发送邮件（暂时没有邮件发送）
                    //SingleInstance<EmailService>.Instance.SubmitEmail(email, title, content.ToString(), emlType);
                }

                //发送短信
                if (verifyCodeType == VerifyCodeType.BindMobile
                  || verifyCodeType == VerifyCodeType.ModifyPwdByMobile
                  || verifyCodeType == VerifyCodeType.GetPwdByMobile
                  || verifyCodeType == VerifyCodeType.UpdateBindMobile
                  )
                {
                    Dictionary<string, object> parms = new Dictionary<string, object>();
                    parms.Add("code", verifyCode);
                    parms.Add("code_expire", validateCodeExpire);
                    SingleInstance<SysService>.Instance.SendSmsMessage(appSign, mobile, parms, content, smsType, tempType);
                }
            }
        }

        /// <summary>
        /// 校验验证码
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="verifyCode">验证码</param>
        /// <returns>验证码ID</returns>
        public long CheckUserVerifyCode(string appSign, string verifyCode, string email, string mobile = "")
        {
            string sql = @" SELECT verify_code_id,verified,expire_date
                            FROM mcp_sys_verifycode 
                            WHERE app_sign=$app_sign$ AND {0} AND verify_code = $verify_code$
                            ORDER BY time_stamp DESC 
                            LIMIT 1";
            ParameterCollection pc = new ParameterCollection();
            pc.Add("verify_code", verifyCode);
            pc.Add("app_sign", appSign);
            if (!string.IsNullOrWhiteSpace(email))
            {
                sql = string.Format(sql, "mobile = $mobile$");
                pc.Add("mobile", email);
            }
            else
            {
                sql = string.Format(sql, "mobile = $mobile$");
                pc.Add("mobile", mobile);
            }
            var verifyCodeModel = this.GetRepository<McpSysVerifycodeInfo>().ListModelBySql(sql, pc).FirstOrDefault();
            if (verifyCodeModel == null)
            {
                throw new BusinessException(VerifyCodeStatusType.Fail.GetRemark());
            }
            else
            {
                if (verifyCodeModel.Verified == 1)
                {
                    throw new BusinessException(VerifyCodeStatusType.Used.GetRemark());
                }

                if (verifyCodeModel.ExpireDate < CommonUtil.GetDBDateTime())
                {
                    throw new BusinessException(VerifyCodeStatusType.Expired.GetRemark());
                }

                return verifyCodeModel.VerifyCodeId;
            }
        }

        /// <summary>
        /// 使用验证码
        /// </summary>
        /// <param name="verifyCodeId">验证码Id</param>
        public void UpdateVerifyCodeStatus(string appSign, long verifyCodeId)
        {
            McpSysVerifycodeInfo verifyCodeModel = new McpSysVerifycodeInfo();
            verifyCodeModel.VerifyCodeId = verifyCodeId;
            verifyCodeModel.AppSign = appSign;
            verifyCodeModel.Verified = 1;
            verifyCodeModel.VerifiedDate = CommonUtil.GetDBDateTime();
            long vid = this.GetRepository<McpSysVerifycodeInfo>().Update(verifyCodeModel, "verified,verified_date");
            if (vid <= 0)
            {
                throw new MessageException("系统错误，更新验证码状态失败！");
            }
        }

        #region 私有

        /// <summary>
        /// 校验获取验证码请求参数
        /// </summary>
        /// <param name="verifyCodeType">验证码类型</param>
        /// <param name="email">邮箱地址</param>
        /// <param name="mobile">手机号码</param>
        private void CheckGetUserVerifyCode(string appSign, VerifyCodeType verifyCodeType, string terminalSign, string email, string mobile = "")
        {
            // 1.校验验证码类型
            int codeType = (int)verifyCodeType;
            if (codeType <= 0)
            {
                throw new MessageException("验证码类型格式不正确！");
            }

            // 2.校验获取验证码方式
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(mobile))
            {
                throw new MessageException("手机号码和邮箱地址不可同时为空！");
            }

            //验证是否频繁发送
            bool isExist = this.IsExistVerifycodeBlackList(appSign, terminalSign, ConfigUtil.VarifyCodeFailCount);
            if (isExist)
            {
                throw new MessageException("您操作太频繁了，请稍候再试！");
            }
        }

        /// <summary>
        /// 获取当前用户已有的验证码（有效期内）
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>已有的验证码</returns>
        private string GetUserVerifyCode(string appSign, string email, string mobile = "")
        {
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(mobile))
            {
                throw new MessageException("手机号码和邮箱地址不可同时为空！");
            }
            string sql = @" SELECT * FROM mcp_sys_verifycode WHERE app_sign = $app_sign$  AND {0} AND verified = 0 AND expire_date > $expire_date$ ORDER BY time_stamp DESC  LIMIT 1";

            ParameterCollection pc = new ParameterCollection();
            pc.Add("app_sign", appSign);
            pc.Add("expire_date", CommonUtil.GetDBDateTime());
            if (!string.IsNullOrWhiteSpace(email))
            {
                sql = string.Format(sql, "email = email");
                pc.Add("email", email);
            }
            else
            {
                sql = string.Format(sql, "mobile = $mobile$");
                pc.Add("mobile", mobile);
            }

            var dbNow = CommonUtil.GetDBDateTime();
            var verifyCodeModel = this.GetRepository<McpSysVerifycodeInfo>().ListModelBySql(sql, pc).FirstOrDefault();
            if (verifyCodeModel != null)
            {
                //强制验证码过期
                CancelVerifyCode(appSign, email, mobile);

                //产生新的验证码记录，验证码相同
                verifyCodeModel.CreatedOn = dbNow;
                verifyCodeModel.ExpireDate = dbNow.AddSeconds(ConfigUtil.ValidatecodeExpire);
                this.GetRepository<McpSysVerifycodeInfo>().Create(verifyCodeModel);

                //返回验证码
                return verifyCodeModel.VerifyCode;
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>验证码</returns>
        private string BuildVerifyCode(string appSign, string terminalSign, string email, string mobile = "")
        {
            try
            {
                //1.强制该邮件或手机号对应验证码失效
                CancelVerifyCode(appSign, email, mobile);

                //1.生成验证码及失效日期
                var dbNow = CommonUtil.GetDBDateTime();
                string verifyCode = VerifyCodeUtil.Intance.GenNumCode(Consts.APP_VALIDATE_CODE_LENGTH);
                DateTime expireDate = dbNow.AddSeconds(ConfigUtil.ValidatecodeExpire);

                //2.登记用户验证码信息
                McpSysVerifycodeInfo verifyCodeModel = new McpSysVerifycodeInfo();
                verifyCodeModel.AppSign = appSign;
                verifyCodeModel.Mobile = mobile;
                verifyCodeModel.Email = email;
                verifyCodeModel.VerifyCode = verifyCode;
                verifyCodeModel.CreatedBy = mobile;
                verifyCodeModel.ExpireDate = expireDate;
                verifyCodeModel.Verified = 0;
                verifyCodeModel.CreatedOn = dbNow;
                verifyCodeModel.TerminalCode = terminalSign;
                this.GetRepository<McpSysVerifycodeInfo>().Create(verifyCodeModel);

                //3.返回产生的验证码
                return verifyCode;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex.StackTrace);
                throw new MessageException("系统错误，生成验证码失败！");
            }
        }

        /// <summary>
        /// 强制验证码过期
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <param name="mobile">手机号码</param>
        private void CancelVerifyCode(string appSign, string email, string mobile = "")
        {
            string sql = @"UPDATE mcp_sys_verifycode SET expire_date=NOW() WHERE app_sign=$app_sign$ AND {0} AND expire_date>NOW()";
            ParameterCollection pc = new ParameterCollection();
            pc.Add("app_sign", appSign);
            if (!string.IsNullOrWhiteSpace(email))
            {
                sql = string.Format(sql, "email = email");
                pc.Add("email", email);
            }
            else
            {
                sql = string.Format(sql, "mobile = $mobile$");
                pc.Add("mobile", mobile);
            }

            DbUtil.DataManager.Current.IData.ExecuteNonQuery(sql, pc);
        }

        /// <summary>
        /// 累计统计用户验证验证码次数，如果失败 limitcount，设置验证码为黑名单
        /// </summary>
        /// <param name="terminalCode">终端标识</param>
        /// <param name="limitCount">获取验证码次数</param>
        private void UpdateVerifycodeBlackList(string appSign, string terminalCode, int limitCount)
        {
            ParameterCollection pc = new ParameterCollection();
            pc.Add("appSign", appSign, ParameterDirectionEnum.Input);
            pc.Add("terminalcode", terminalCode, ParameterDirectionEnum.Input);
            pc.Add("totalcount", limitCount, ParameterDirectionEnum.Input);

            DbUtil.DataManager.Current.IData.ExecuteNonQuery("proc_sys_blacklist", CommandTypeEnum.StoredProcedure, pc);
        }

        /// <summary>
        /// 校验终端是否存在于黑名单中
        /// </summary>
        /// <param name="terminalCode"></param>
        /// <returns></returns>
        private bool IsExistVerifycodeBlackList(string appSign, string terminalCode)
        {
            string sql = "SELECT COUNT(black_id) FROM mcp_sys_blacklist WHERE app_sign=$app_sign$ AND terminal_code=$terminal_code$ AND lock_expire_date>CURRENT_TIMESTAMP() LIMIT 1;";
            ParameterCollection pc = new ParameterCollection();
            pc.Add("app_sign", appSign);
            pc.Add("terminal_code", terminalCode);
            object exist = DbUtil.DataManager.Current.IData.ExecuteScalar(sql, pc);
            return TryConvertUtil.ToInt(exist, 0) > 0;
        }

        /// <summary>
        /// 验证码错误limitcount内，判定为黑名单用户
        /// </summary>
        /// <param name="terminalCode">硬件标示符</param>
        /// <param name="limitCount">失败次数</param>
        /// <returns></returns>
        private bool IsExistVerifycodeBlackList(string appSign, string terminalCode, int limitCount)
        {
            UpdateVerifycodeBlackList(appSign, terminalCode, limitCount);

            return IsExistVerifycodeBlackList(appSign, terminalCode);
        }

        #endregion
    }
}
