//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2012/12/03        创建
//---------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  CWI.MCP.Common.Attributes
{
    /// <summary>
    /// 当接收参数中有为0的字符串时，全部重置为string.Empty;
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ZeroToEmptyAttribute : Attribute
    {
        public ZeroToEmptyAttribute(params string[] keys) 
        {
            Keys = keys;
        }

        /// <summary>
        /// 需要处理的参数列表
        /// </summary>
        public string[] Keys
        {
            get;
            private set;
        }
    }
}
