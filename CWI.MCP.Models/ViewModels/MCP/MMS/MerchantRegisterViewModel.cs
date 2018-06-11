using CWI.MCP.Common;
using Evt.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CWI.MCP.Models
{
    /// <summary>
    /// 店铺注册参数视图模型
    /// </summary>
    public class MerchantRegisterViewModel : ViewModel
    {
        /// <summary>
        /// 帐号
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空！")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "帐号由 4-20位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        [RegularExpression(RegexConsts.USERACCOUNT_PATTERN, ErrorMessage = "帐号由 4-20位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        public string Account { get; set; }
    
        /// <summary>
        /// 设置密码
        /// </summary>
        [Required(ErrorMessage = "密码为空，请重新输入！")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "密码由 6-16位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        [RegularExpression(RegexConsts.USERPASSWORD_PATTERN, ErrorMessage = "密码由6-16位字符，可由英文、数字及“_”、“-”组成，请重新输入！")]
        public string Pwd { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Required(ErrorMessage = "手机号不能空！")]
        [RegularExpression(RegexConsts.MOBILE_PATTERN, ErrorMessage = "手机号格式不正确，请重新输入！")]
        public string Mobile { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "短信验证码不能为空！")]
        public string Validcode { get; set; }

        /// <summary>
        /// 图形验证码
        /// </summary>
        [Required(ErrorMessage = "图片验证码不能为空！")]
        public string ImageCode { get; set; }

    }
}