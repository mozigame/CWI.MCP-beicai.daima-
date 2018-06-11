//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋       2014/11/14         创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Web.Script.Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Collections;

namespace  CWI.MCP.Common.Extensions
{
    /// <summary>
    /// IList扩展
    /// </summary>
    public static class IListExtensions
    {
        public static bool IsNullEmpty(this IList self) { return self == null || self.Count == 0; }

        /// <summary>
        /// 集合交换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Swap<T>(this IList<T> @this, int x, int y)
        {
            if (x != y)
            {
                T xValue = @this[x];
                @this[x] = @this[y];
                @this[y] = xValue;
            }
        }

        ///// <summary>
        ///// 转为分页
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="list"></param>
        ///// <param name="intPageIndex"></param>
        ///// <param name="intPageSize"></param>
        ///// <returns></returns>
        //public static PagedList<T> ToPagedList<T>(this List<T> list, int intPageIndex, int intPageSize)
        //{
        //    return new PagedList<T>(list, intPageIndex, intPageSize);
        //}

    }
}
