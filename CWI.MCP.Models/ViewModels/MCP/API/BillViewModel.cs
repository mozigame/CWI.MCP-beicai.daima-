using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CWI.MCP.Common;
using Evt.Framework.Mvc;
using CWI.MCP.Common.Attributes;

namespace CWI.MCP.Models
{
    /// <summary>
    /// 打印参数
    /// </summary>
    public class BillViewModel : PrintViewModel
    {
        /// <summary>
        /// 票据类型
        /// </summary>
        //[Required(ErrorMessage = "票据类型不能为空！")]
        [RegularExpression(RegexConsts.INT_FOR_GREAT_ZERO, ErrorMessage = "票据类型应为大于零的整数！")]
        public string bill_type { get; set; }

        /// <summary>
        /// 模版ID：交易小票时可为空，快递面单时不可空
        /// </summary>
        public string template_id { get; set; }

        /// <summary>
        /// 打印份数
        /// </summary>
        //[Required(ErrorMessage = "打印份数不能为空！")]
        [RegularExpression(RegexConsts.INT_FOR_GREAT_ZERO, ErrorMessage = "份数应为大于零的整数！")]
        public string copies { get; set; }

        /// <summary>
        /// 票据单号
        /// </summary>
        [Required(ErrorMessage = "票据单号不能为空！")]
        public string bill_no { get; set; }

        /// <summary>
        /// 票据内容,参数格式为(json对象字符串): {参数名：参数值，参数名：参数值}
        /// </summary>
        [Required(ErrorMessage = "票据内容不能为空！")]
        public string bill_content { set; get; }

        /// <summary>
        /// MD5加密的字符串
        /// </summary> 
        public string sign { get; set; }
    }
}


