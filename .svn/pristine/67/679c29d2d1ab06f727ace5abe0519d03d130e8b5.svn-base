using System.ComponentModel.DataAnnotations;
using Evt.Framework.Mvc;

namespace CWI.MCP.Models
{
    /// <summary>
    /// 设备授权参数
    /// </summary>
    public class AuthViewModel : ViewModel
    {
        /// <summary>
        /// 制造编号
        /// </summary>
        [Required(ErrorMessage = "制造编号不能为空！")]
        public string sn { get; set; }

        /// <summary>
        /// 校验码
        /// </summary>
        [Required(ErrorMessage = "校验码不能为空！")]
        public string ck { get; set; }
    }
}