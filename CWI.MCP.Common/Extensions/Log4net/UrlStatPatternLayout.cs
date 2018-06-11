//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2012/10/24        创建
//---------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Layout;

namespace  CWI.MCP.Common.Extensions.Log4net
{
    public class UrlStatPatternLayout : PatternLayout
    {
        /// <summary>
        /// 请求统计 Pattern Layout
        /// </summary>
        public UrlStatPatternLayout() 
        {
            this.AddConverter("RequestUrl", typeof(RequestUrlPatternLayoutConverter));
            this.AddConverter("ParamData", typeof(ParamDataPatternLayoutConverter));
            this.AddConverter("Controller", typeof(ControllerPatternLayoutConverter));
            this.AddConverter("Action", typeof(ActionPatternLayoutConverter));
            this.AddConverter("SessionID", typeof(SessionIDPatternLayoutConverter));
            this.AddConverter("RequestType", typeof(RequestTypePatternLayoutConverter));
            this.AddConverter("ProSign", typeof(ProSignPatternLayoutConverter));
            this.AddConverter("HeaderParamData", typeof(HeaderParamDataPatternLayoutConverter));
            this.AddConverter("UserId", typeof(UserIdPatternLayoutConverter));
            this.AddConverter("DeviceId", typeof(DeviceIdPatternLayoutConverter));
            this.AddConverter("DeviceType", typeof(DeviceTypePatternLayoutConverter));
        }
    }
}
