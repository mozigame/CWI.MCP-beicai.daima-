//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/06        创建
//---------------------------------------------
using System;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using CWI.MCP.Common.Extensions;

namespace  CWI.MCP.Common
{
    public static class TryConvertUtil
    {
        /// <summary>
        /// 转换成int类型
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defValue">默认值</param>
        /// <returns>int类型值</returns>
        public static int ToInt(object value, int defValue)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return defValue;
            }

            int def = 0;
            if (value is decimal)
            {
                return (int)((decimal)value);
            }
            if (value is float)
            {
                return (int)((float)value);
            }
            return int.TryParse(value.ToString(), out def) ? def : defValue;
        }

        /// <summary>
        /// 转换成int类型
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <returns>int类型值</returns>
        public static int ToInt(object obj)
        {
            return ToInt(obj, int.MinValue);
        }

        /// <summary>
        /// 转换成byte
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defValue">失败后默认值</param>
        /// <returns>byte类型值</returns>
        public static byte ToTinyInt(object value, byte defValue)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return defValue;
            }

            byte def = 0;
            return byte.TryParse(value.ToString(), out def) ? def : defValue;
        }

        /// <summary>
        /// 转换成short
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defValue">失败后默认值</param>
        /// <returns>short类型值</returns>
        public static short ToSmallInt(object value, short defValue)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return defValue;
            }

            short def = 0;
            return short.TryParse(value.ToString(), out def) ? def : defValue;
        }

        /// <summary>
        /// 转换成decimal
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defValue">失败后默认值</param>
        /// <returns>decimal类型值</returns>
        public static decimal ToDecimal(object value, decimal defValue)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return defValue;
            }

            decimal def = 0;
            return decimal.TryParse(value.ToString(), out def) ? def : defValue;
        }



        /// <summary>
        /// 转换成float
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defValue">失败后默认值</param>
        /// <returns>float类型值</returns>
        public static float ToFloat(object value, float defValue)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return defValue;
            }

            float def = 0;
            return float.TryParse(value.ToString(), out def) ? def : defValue;
        }

        /// <summary>
        /// 转为short类型
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defValue">失败后默认值</param>
        /// <returns></returns>
        public static short ToShort(object value, short defValue)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return defValue;
            }

            short def = 0;
            return short.TryParse(value.ToString(), out def) ? def : defValue;
        }

        /// <summary>
        /// 转换成Int64
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defValue">失败后默认值</param>
        /// <returns>Int64类型值</returns>
        public static Int64 ToBigInt(object value, Int64 defValue)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return defValue;
            }

            Int64 def = 0;
            return Int64.TryParse(value.ToString(), out def) ? def : defValue;
        }

        /// <summary>
        /// 转换成bool
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defValue">失败后默认值</param>
        /// <returns>bool类型值</returns>
        public static bool ToBool(object value, bool defValue)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return defValue;
            }

            if (value != null)
            {
                if (string.Compare(value.ToString(), "true", true) == 0) return true;
                if (string.Compare(value.ToString(), "false", true) == 0) return false;
                if (string.Compare(value.ToString(), "1", true) == 0) return true;
                if (string.Compare(value.ToString(), "0", true) == 0) return false;
            }
            return defValue;
        }

        /// <summary>
        /// 转换成byte
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <returns>返回byte，失败返回0</returns>
        public static byte ToTinyInt(object value)
        {
            return ToTinyInt(value, 0);
        }

        /// <summary>
        /// 转换成short
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <returns>返回short，失败返回0</returns>
        public static short ToSmallInt(object value)
        {
            return ToSmallInt(value, 0);
        }

        /// <summary>
        /// 转换成decimal
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <returns>返回decimal，失败返回0</returns>
        public static decimal ToDecimal(object value)
        {
            return ToDecimal(value, 0.00m);
        }

        /// <summary>
        /// 转换成float
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <returns>返回float，失败返回0</returns>
        public static float ToFloat(object value)
        {
            return ToFloat(value, 0);
        }

        /// <summary>
        /// 转换成Int64
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <returns>返回Int64，失败返回0</returns>
        public static Int64 ToBigInt(object value)
        {
            return ToBigInt(value, 0);
        }

        /// <summary>
        /// 转换成bool
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <returns>返回bool，失败返回false</returns>
        public static bool ToBool(object value)
        {
            return ToBool(value, false);
        }

        /// <summary>
        /// 转换成DateTime
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <returns>返回DateTime，失败返回DateTime.MinValue</returns>
        public static DateTime ToDateTime(object value)
        {
            return ToDateTime(value, DateTime.MinValue);
        }

        /// <summary>
        /// 转换成String
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <returns>string类型值</returns>
        public static string ToString(object value)
        {
            return ToString(value, string.Empty);
        }

        /// <summary>
        /// 转换成DateTime
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defValue">失败后默认值</param>
        /// <returns>DateTime类型值</returns>
        public static DateTime ToDateTime(object value, DateTime defValue)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return defValue;
            }

            if (value is Dictionary<string, object>)
            {
                Dictionary<string, object> temp = value as Dictionary<string, object>;
                return new DateTime((int)temp["Year"], (int)temp["Month"], (int)temp["Day"],
                    (int)temp["Hour"], (int)temp["Minute"], (int)temp["Second"], (int)temp["Millisecond"]);
            }

            DateTime def = DateTime.MinValue;
            return DateTime.TryParse(value.ToString(), out def) ? def : defValue;
        }

        /// <summary>
        /// 转换成String
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defValue">失败后默认值</param>
        /// <returns>string类型值</returns>
        public static string ToString(object value, string defValue)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return defValue;
            }

            return value.ToString().Trim();
        }

        /// <summary>
        /// 转换成颜色
        /// </summary>
        /// <param name="color">颜色值，如#fffffff</param>
        /// <returns></returns>
        public static Color ToColor(string color)
        {
            color = color.Replace("#", string.Empty);

            byte a = System.Convert.ToByte("ff", 16);

            byte pos = 0;

            if (color.Length == 8)
            {
                a = System.Convert.ToByte(color.Substring(pos, 2), 16);
                pos = 2;
            }

            byte r = System.Convert.ToByte(color.Substring(pos, 2), 16);

            pos += 2;

            byte g = System.Convert.ToByte(color.Substring(pos, 2), 16);

            pos += 2;

            byte b = System.Convert.ToByte(color.Substring(pos, 2), 16);

            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// 把DataTable转成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToModelList<T>(DataTable dt) where T : class, new()
        {
            return dt.ToList<T>();
        }

        /// <summary>
        /// 把数据行转为实体
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToModel<T>(DataRow dr) where T : class,new()
        {
            return dr.ToModel<T>();
        }
    }
}
