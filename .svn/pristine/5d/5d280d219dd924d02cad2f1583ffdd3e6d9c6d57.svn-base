// 版权信息：版权所有(C) 2012，Evervictory Tech
// 变更历史：
// 姓名         日期          说明
// --------------------------------------------------------
// 王军锋     2012/5/28       创建
// --------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Util;

namespace  CWI.MCP.Common.Extensions
{
    /// <summary>
    /// 自定义请求校验类型类，不校验传入的值
    /// </summary>
    public class CustomRequestValidation : RequestValidator
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public CustomRequestValidation()
        {
        }

        /// <summary>
        /// 重写是否为有效的请求字符串
        /// </summary>
        /// <param name="context">HttpContext对象</param>
        /// <param name="value">当前传入的字符串</param>
        /// <param name="requestValidationSource">枚举RequestValidationSource对象</param>
        /// <param name="collectionKey">键值</param>
        /// <param name="validationFailureIndex">校验失败时的索引值</param>
        /// <returns>true=请求字符串有效，false=请求字符串无效</returns>
        protected override bool IsValidRequestString(System.Web.HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
        {
            validationFailureIndex = 0;
            return true;
        }
    }
}
