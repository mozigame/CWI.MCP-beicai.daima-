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

namespace  CWI.MCP.Common.Extensions
{
    /// <summary>
    /// Double扩展
    /// </summary>
    public static class DoubleExtensions
    {
        public static decimal PercentageOf(this double number, int percent)
        {
            return (decimal)(number * percent / 100);
        }

        public static decimal PercentageOf(this double number, float percent)
        {
            return (decimal)(number * percent / 100);
        }

        public static decimal PercentageOf(this double number, double percent)
        {
            return (decimal)(number * percent / 100);
        }

        public static decimal PercentageOf(this double number, long percent)
        {
            return (decimal)(number * percent / 100);
        }

        public static decimal PercentOf(this double position, int total)
        {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }

        public static decimal PercentOf(this double position, float total)
        {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }

        public static decimal PercentOf(this double position, double total)
        {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }

        public static decimal PercentOf(this double position, long total)
        {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }

        public static long Round(this double value)
        {
            if (value >= 0) return (long)Math.Floor(value);
            return (long)Math.Ceiling(value);
        }

        public static string FormatMoney(this double money)
        {
            return money.ToString("#,###.00");
        }
    }
}
