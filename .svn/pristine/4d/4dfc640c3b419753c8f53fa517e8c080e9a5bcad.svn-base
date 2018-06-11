using CWI.MCP.Common;
using CWI.MCP.Common.ORM;
using CWI.MCP.Models;
using Evt.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWI.MCP.Services
{
    public class AdminService : BaseService
    {
        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="info">管理员登录信息</param>
        public void Login(LoginViewModel model)
        {
            if (model == null)
            {
                throw new MessageException("参数不能为空");
            }

            // 1.获取登录信息
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("admin_account", model.Account.Trim()));
            var info = this.GetRepository<McpAdminInfo>().GetModel(cc);

            // 2.验证状态
            CheckLoginUser(info, model);

            // 3.更新数据
            info.LastLoginIp = CommonUtil.GetRemoteIPAddress();
            info.LastLoginDate = CommonUtil.GetDBDateTime();
            this.GetRepository<McpAdminInfo>().Update(info);

            // 4.验证成功后设置session
            SessionUtil.Current = new SessionData()
            {
                UserId = info.AdminId.ToString(),
                Account = info.AdminAccount.ToString(),
                AdminName = info.AdminName,
                AdminIsSupper = info.Issupper == 1
            };
        }

        /// <summary>
        /// 校验登录用户并记录信息
        /// </summary>
        /// <param name="info">登录用户信息</param>
        private void CheckLoginUser(McpAdminInfo info, LoginViewModel model)
        {
            if (info == null)
            {
                throw new MessageException("此帐号不存在！");
            }

            if (!info.AdminPwd.Equals(SecurityUtil.ConvertToMD5(model.Pwd.Trim())))
            {
                throw new MessageException("密码不正确！");
            }

            switch (info.StatusCode)
            {
                case (int)StatusCodeType.Disabled:
                    throw new MessageException(UserStatusType.Disabled.GetRemark());

                case (int)StatusCodeType.Deleted:
                    throw new MessageException(UserStatusType.Deleted.GetRemark());

                default: break;
            };
        }

    }
}
