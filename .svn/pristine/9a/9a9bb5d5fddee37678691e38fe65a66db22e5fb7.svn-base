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
using System.Text.RegularExpressions;
using System.Web.Routing;
using Evt.Framework.Common;
using CWI.MCP.Common.Extensions.MVC;

namespace  CWI.MCP.Common.Extensions
{
    public static class MethodExtension
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>布尔</returns>
        public static bool IsValid(this string str) 
        {
            return !string.IsNullOrEmpty(str) && str != RouteParameter.Default;
        }

        /// <summary>
        /// 文本重复多少次
        /// </summary>
        /// <param name="str">原始文本</param>
        /// <param name="count">重复次数</param>
        /// <returns>结果文本</returns>
        public static string Repeat(this string str, int count)
        {
            if (count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < count; i++)
                {
                    sb.Append(str);
                }
                return sb.ToString();
            }
            else 
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 更新字典中的值，若不存在则根据参数AddWhenNotExists定义是否添加
        /// </summary>
        /// <param name="values">字典</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="AddWhenNotExists">true:若键不存在，就执行添加操作;false:不执行添加操作</param>
        public static void Update(this RouteValueDictionary values, string key, object value, bool AddWhenNotExists = false) 
        {
            if (values.ContainsKey(key))
            {
                values[key] = value;
            }
            else
            {
                if (AddWhenNotExists)
                {
                    values.Add(key, value);
                }
            }
        }

        /// <summary>
        /// 字典中是否存在匹配项
        /// </summary>
        /// <param name="values">字典</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>布尔</returns>
        public static bool IsMatch(this RouteValueDictionary values, string key, object value) 
        {
            object retValue = null;
            bool isExists = values.TryGetValue(key, out retValue);
            if (isExists) 
            {
                if (string.Equals(retValue,value)) 
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 更新参数，将等于string.Empty的值修改默认值
        /// </summary>
        /// <param name="values">字典</param>
        public static void UpdateParameter(this RouteValueDictionary values) 
        {
            List<string> keys = values.Keys.ToList();
            foreach (string key in keys) 
            {
                string value = ConvertUtil.ToString(values[key], string.Empty);
                if (string.IsNullOrEmpty(value)) 
                {
                    values[key] = RouteParameter.Default;
                }
            }
        }

        /// <summary>
        /// 将textArea中输入的文本中的回车换行替换成网页格式显示
        /// </summary>
        /// <param name="str">str</param>
        /// <returns></returns>
        public static string ToHTML(this string str)
        {
            return str.Replace(" ", "&nbsp;").Replace("\r\n", "<br />");
        }

    }
}
