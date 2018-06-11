//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名         日期             说明
// --------------------------------------------
//      王军锋   2014/11/21 11:44:25     创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWI.MCP.Common.Attributes;

namespace CWI.MCP.Common
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtend
    {
        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="type">枚举值</param>
        /// <returns>枚举描述</returns>
        public static string GetRemark<T>(this T type) where T : struct
        {
            if (type is Enum)
            {
                var t = type as Enum;
                return EnumDescriptionAttribute.GetDescription(t);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取对应枚举的所有值
        /// </summary>
        /// <param name="type">枚举值</param>
        /// <returns>枚举的所有值</returns>
        public static List<T> List<T>(this T type) where T : struct
        {
            if (type is Enum)
            {
                return Enum.GetValues(type.GetType()).Cast<T>().ToList();
            }
            return new List<T>();
        }
    }
}
