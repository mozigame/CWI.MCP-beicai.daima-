using System;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models.ViewModels.MCP.OPEN
{
    public class DevRegisterViewMode : ViewModel
    {
        /// <summary>
        /// 开发者ID
        /// </summary>
        public string DeveloperId { set; get; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { set; get; }

        /// <summary>
        /// 手机验证码
        /// </summary>
        public string MobileCode { set; get; }

        /// <summary>
        /// 图形验证码
        /// </summary>
        public string PicCode { set; get; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string Email { set; get; }

        /// <summary>
        /// 开发者密码
        /// </summary>
        public string UserPwd { set; get; }

        /// <summary>
        /// 确认开发者密码
        /// </summary>
        public string ReUserPwd { set; get; }

        /// <summary>
        /// 是否激活:0-未激活,1-已激活
        /// </summary>
        public int IsActived { set; get; }

        /// <summary>
        /// 激活日期
        /// </summary>
        public DateTime ActivedDatetime { set; get; }

        /// <summary>
        /// 最后一次登录IP
        /// </summary>
        public string LastLoginIp { set; get; }

        /// <summary>
        /// 最后一次登录时间
        /// </summary>
        public DateTime LastLoginDate { set; get; }

        /// <summary>
        /// 状态码:1-启用,2-停用,3-删除
        /// </summary>
        public int StatusCode { set; get; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedOn { set; get; }

        /// <summary>
        /// 最后变更日期
        /// </summary>
        public DateTime ModifiedOn { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { set; get; }
				
				
    }
}
