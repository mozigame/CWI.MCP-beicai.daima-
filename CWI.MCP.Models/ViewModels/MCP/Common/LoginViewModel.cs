//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2012/02/21         创建
//---------------------------------------------
using CWI.MCP.Common;
using Evt.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CWI.MCP.Models
{
    /// <summary>
    /// 后台管理员登录信息
    /// </summary>
    public class LoginViewModel : ViewModel
    {
        /// <summary>
        /// 管理员帐号
        /// </summary>
        [Required(ErrorMessage = "帐号不能为空。")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "帐号由 4-20位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        [RegularExpression(RegexConsts.USERACCOUNT_PATTERN, ErrorMessage = "帐号由 4-20位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        public string Account { get; set; }

        /// <summary>
        /// 管理员密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空。")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "密码由 6-16位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        [RegularExpression(RegexConsts.USERPASSWORD_PATTERN, ErrorMessage = "密码由6-16位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        public string Pwd { get; set; }

    }
}