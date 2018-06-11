using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evt.Framework.Mvc;
using System.ComponentModel.DataAnnotations;
using CWI.MCP.Common;

namespace  CWI.MCP.Models
{
    public class SmsVerifyCodeByAccountViewModel : ViewModel
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        [Required(ErrorMessage = "请输入帐号！")]
        public string Account { get; set; }

        /// <summary>
        /// 图片验证码
        /// </summary>
        [Required(ErrorMessage = "请输入图片验证码！")]
        [RegularExpression(RegexConsts.IMAGE_CODE_PATTERN, ErrorMessage = "图片验证码输入有误!")]
        public string ImageCode { get; set; }

        /// <summary>
        /// 验证码类型
        /// </summary>
        public int VerifyCodeType { get; set; }

        /// <summary>
        /// 短信码类型
        /// </summary>
        public int SmsFuncType { get; set; }
    }
}
