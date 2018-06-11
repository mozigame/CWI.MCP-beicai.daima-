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
    /// 获取短信验证码参数
    /// </summary>
    public class SmsVerifyCodeViewModel : ViewModel
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        [Required(ErrorMessage = "请输入手机号码！")]
        [RegularExpression(RegexConsts.MOBILE_PATTERN, ErrorMessage = "手机号码格式不正确！")]
        public string Mobile { get; set; }

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
        /// 功能类型
        /// </summary>
        public int SmsFuncType { get; set; }
    }
}