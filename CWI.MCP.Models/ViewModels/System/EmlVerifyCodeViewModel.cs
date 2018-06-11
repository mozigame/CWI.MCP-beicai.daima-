//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/21        创建
//---------------------------------------------
using CWI.MCP.Common;
using Evt.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace  CWI.MCP.Models
{
    /// <summary>
    /// 获取邮件验证码参数
    /// </summary>
    public class EmlVerifyCodeViewModel : ViewModel
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required(ErrorMessage = "请输入邮箱地址！")]
        [RegularExpression(RegexConsts.EMAIL_PATTERN, ErrorMessage = "请输入正确的邮箱地址！")]
        public string Email { get; set; }

        /// <summary>
        /// 验证码类型
        /// </summary>
        public int VerifyCodeType { get; set; }

        /// <summary>
        /// 终端设备唯一标识
        /// </summary>
        public string TerminalSign { get; set; }
    }
}