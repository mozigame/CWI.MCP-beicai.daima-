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
using System.IO;
using log4net.Core;
using System.Reflection;

namespace  CWI.MCP.Common.Extensions.Log4net
{
    /// <summary>
    /// 扩展内部工具类
    /// </summary>
    public class ExtensionInternalUtils
    {
        /// <summary>
        /// 在流对象中写入指定属性的的值
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="loggingEvent">loggingEvent</param>
        /// <param name="propertyName">propertyName</param>
        internal static void Write(TextWriter writer, LoggingEvent loggingEvent,string propertyName) 
        {
            object msgObj = loggingEvent.MessageObject;
            if (msgObj != null)
            {
                PropertyInfo pInfo = msgObj.GetType().GetProperty(propertyName);
                if (pInfo != null)
                {
                    object value = pInfo.GetValue(msgObj, null);
                    writer.Write(value);
                }
            }
        }
    }
}
