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
    public class ChangePwdViewModel : ViewModel
    {
        /// <summary>
        /// 店铺帐号
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空！")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "帐号由 4-20位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        [RegularExpression(RegexConsts.USERACCOUNT_PATTERN, ErrorMessage = "帐号由 4-20位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空！")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "密码由 6-16位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        [RegularExpression(RegexConsts.USERPASSWORD_PATTERN, ErrorMessage = "密码由6-16位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        public string Pwd { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "短信验证码不能为空！")]
        public string ValidCode { get; set; }

        /// <summary>
        /// 图片验证码
        /// </summary>
        [Required(ErrorMessage = "图片验证码不能为空！")]
        public string ImageCode { get; set; }

    }
}
