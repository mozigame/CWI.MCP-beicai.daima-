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

namespace  CWI.MCP.Common.Exceptions
{
    /// <summary>
    /// 404错误
    /// </summary>
    public class Error404Exception : Exception
    {
        /// <summary>
        /// 构告函数
        /// </summary>
        /// <param name="message"></param>
        public Error404Exception(string message) : base(message)
        {
            
        }
    }
}
