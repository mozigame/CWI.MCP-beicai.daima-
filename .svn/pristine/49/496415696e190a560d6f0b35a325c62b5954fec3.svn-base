// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期          说明
// --------------------------------------------
//      王军锋     2012/12/03       创建
// --------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace  CWI.MCP.Common.Extensions.MVC
{
    /// <summary>
    /// 路由参数
    /// </summary>
    public sealed class RouteParameter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static RouteParameter() 
        {
            Optional = UrlParameter.Optional;
            Default = "0";
            Index = 1;
            Numeral = 0;
        }

        /// <summary>
        /// 包含可选参数的只读值
        /// </summary>
        public static UrlParameter Optional { get; private set; }

        /// <summary>
        /// 默认值("0")
        /// </summary>
        public static string Default { get; private set; }

        /// <summary>
        /// 默认索引值(1)
        /// </summary>
        public static int Index { get; private set; }

        /// <summary>
        /// 默认整数
        /// </summary>
        public static int Numeral { get; private set; }
        
    }
}
