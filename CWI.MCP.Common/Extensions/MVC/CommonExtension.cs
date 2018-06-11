// 版权信息：版权所有(C) 2013，Paidui Tech
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//              2013/07/02        创建
//---------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace  CWI.MCP.Common.Extensions.MVC
{
    /// <summary>
    /// 方法扩展
    /// </summary>
    public static class CommonExtension
    {
        /// <summary>
        /// 返回Http请求参数格式
        /// </summary>
        /// <param name="self">self</param>
        /// <returns>参数字符串</returns>
        public static string ToParamString(this Dictionary<string, string> self)
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> kv in self)
            {
                list.Add(string.Format("{0}={1}", kv.Key, HttpUtility.UrlEncode(kv.Value)));
            }

            string param = string.Join("&", list);

            return param;
        }
    }
}