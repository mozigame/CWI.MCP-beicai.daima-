//---------------------------------------------
// 版权信息：版权所有(C) 2014，PAIDUI.CN
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋     2014/3/25 13:39:07         创建
//---------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace CWI.MCP.API.Handels
{
    /// <summary>
    /// 所有控制器类型的过虑规则
    /// </summary>
    public static class ControllerTypeSpecifications
    {
        /// <summary>
        /// 根据AppSign过滤
        /// </summary>
        /// <param name="query">类型集合</param>
        /// <param name="appSign">AppSign</param>
        /// <returns>过滤后的类型集合</returns>
        public static IEnumerable<KeyValuePair<string, Type>> ByAppSign(this IEnumerable<KeyValuePair<string, Type>> query, string appSign)
        {
            var appSignToFind = string.Format(CultureInfo.InvariantCulture, ".{0}.", appSign);

            return query.Where(x => x.Key.IndexOf(appSignToFind, StringComparison.OrdinalIgnoreCase) != -1);
        }

        /// <summary>
        /// 直接返回所有类型
        /// </summary>
        /// <param name="query">类型集合</param>
        /// <returns>所有类型集合</returns>
        public static IEnumerable<KeyValuePair<string, Type>> WithoutAppSign(this IEnumerable<KeyValuePair<string, Type>> query)
        {
            return query;
        }

        /// <summary>
        /// 根据appSign和控制器名找到控制器类型
        /// </summary>
        /// <param name="query">控制器类型集合</param>
        /// <param name="appSign">AppSign</param>
        /// <param name="controllerName">控制器名</param>
        /// <returns>控制器集合类</returns>
        public static IEnumerable<KeyValuePair<string, Type>> ByControllerName(this IEnumerable<KeyValuePair<string, Type>> query, string appSign, string controllerName)
        {
            var controllerNameToFind = string.Empty;
            if (!string.IsNullOrEmpty(appSign))
            {
                controllerNameToFind = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", AppSignHttpControllerSelector.ControllerPrefix, appSign, controllerName);
                var result = query.Where(x => x.Key.StartsWith(controllerNameToFind, StringComparison.OrdinalIgnoreCase));
                return result;
            }
            else
            {
                var result = query.Where(x => x.Key.EndsWith("." + controllerName + "Controller"));
                return result;
            }
        }
    }
}
