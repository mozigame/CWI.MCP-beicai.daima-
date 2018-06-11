//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2011/11/26        创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  CWI.MCP.Common.Attributes
{
    /// <summary>
    /// 表示需要经过身份验证后才能访问
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SkipAuthorizationAttribute : Attribute
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public SkipAuthorizationAttribute()
        {
        }
    }
}
