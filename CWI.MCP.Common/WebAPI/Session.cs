//-------------------------------------------------
//版本信息:版权所有(C) 2014,PAIDUI.CN
//变更历史:
//    姓名            日期                  说明
//-------------------------------------------------
//   王军锋     2014/12/11 13:46:11           创建
//-------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// Session类
    /// </summary>
    public class Session : Dictionary<string, object>
    {
        /// <summary>
        /// SessionID
        /// </summary>
        public string SessionID
        {
            get;
            set;
        }
    }
}
