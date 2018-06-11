using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CWI.MCP.Common;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models
{
    /// <summary>
    /// 忘记密码VM
    /// </summary>
    public class ChangeMobileViewModel : ViewModel
    {
        /// <summary>
        /// 旧手机号
        /// </summary>
        [Required(ErrorMessage = "旧手机号不能空！")]
        [RegularExpression(RegexConsts.MOBILE_PATTERN, ErrorMessage = "旧手机号格式不正确，请重新输入！")]
        public string OldMobile { get; set; }

        /// <summary>
        /// 新手机号
        /// </summary>
        [Required(ErrorMessage = "新手机号不能空！")]
        [RegularExpression(RegexConsts.MOBILE_PATTERN, ErrorMessage = "新手机号格式不正确，请重新输入！")]
        public string NewMobile { get; set; }


        /// <summary>
        /// 旧手机号验证码
        /// </summary>
        [Required(ErrorMessage = "旧手机号短信验证码不能为空！")]
        public string OldMobileValidCode { get; set; }

        /// <summary>
        /// 新手机号验证码
        /// </summary>
        [Required(ErrorMessage = "新手机号短信验证码不能为空！")]
        public string NewMobileValidCode { get; set; }

        /// <summary>
        /// 图形验证码
        /// </summary>
        [Required(ErrorMessage = "图片验证码不能为空！")]
        public string ImageCode { get; set; }
    }
}
