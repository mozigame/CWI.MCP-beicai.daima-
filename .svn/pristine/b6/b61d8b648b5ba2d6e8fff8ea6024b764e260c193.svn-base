//---------------------------------------------
// 版权信息：版权所有(C) 2015，Coolwi
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2012-03-31        创建
//---------------------------------------------

using System;
using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Http;
using Evt.Framework.Common;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using CWI.MCP.Common.Extensions;
using System.Runtime.InteropServices;
using System.ServiceModel.Channels;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 公共工具类。
    /// </summary>
    public static class CommonUtil
    {
        #region 获取IP地址

        /// <summary>
        /// 获取本机IPV4地址【多个IP时只获取第一个】
        /// </summary>
        /// <returns>第一个IPV4地址</returns>
        public static string GetLocalIpAddress()
        {
            IPAddress[] addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

            string ip = string.Empty;

            foreach (var item in addresses)
            {
                if (item.AddressFamily.ToString().Equals("InterNetwork", StringComparison.CurrentCultureIgnoreCase))
                {
                    ip = item.ToString();
                    break;
                }
            }

            return ip;
        }

        /// <summary>
        /// 获取客户端IP地址。
        /// </summary>
        /// <returns>客户端的IP地址。</returns>
        public static string GetRemoteIPAddress()
        {
            string remoteIP = String.Empty;
            if (HttpContext.Current != null)
            {
                remoteIP = HttpContext.Current.Request.ServerVariables.Get("HTTP_Cdn_Src_Ip");
                if (string.IsNullOrEmpty(remoteIP))
                {
                    remoteIP = HttpContext.Current.Request.ServerVariables.Get("HTTP_X_FORWARDED_FOR");
                    if (String.IsNullOrEmpty(remoteIP))
                    {
                        remoteIP = HttpContext.Current.Request.ServerVariables.Get("REMOTE_ADDR");
                    }
                    else
                    {
                        remoteIP = remoteIP.Split(",".ToCharArray())[0];
                    }
                }
            }

            return remoteIP;
        }

        /// <summary>
        /// MVC API　取客户端IP
        /// </summary>
        /// <param name="request">HttpRequestMessage类实例</param>
        /// <returns>客户端IP</returns>
        public static string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContext)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 将IP地址转换成数字形式；如果不能获得IP地址，则返回0
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>将IP地址转换成数字形式；如果不能获得IP地址，则返回0</returns>
        public static long GetIPData(string ip)
        {
            if (String.IsNullOrEmpty(ip))
                return 0;

            IPAddress ipAdress = null;
            if (!IPAddress.TryParse(ip, out ipAdress))
            {
                return 0;
            }

            long lngIP = 0;
            byte[] bytIP = ipAdress.GetAddressBytes();
            lngIP = (((long)bytIP[0]) << 24) |
                (((long)bytIP[1]) << 16) | (((long)bytIP[2]) << 8) | ((long)bytIP[3]);

            return lngIP;
        }

        #endregion

        #region 获取经纬度或距离

        private const double EARTH_RADIUS = 6378.137;
        private static double Degrees(double d)
        {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// 根据经纬度计算距离
        /// </summary>
        /// <param name="lat1">起点经度</param>
        /// <param name="lng1">起点纬度</param>
        /// <param name="lat2">终点经度</param>
        /// <param name="lng2">终点纬度</param>
        /// <returns>起点终点距离</returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Degrees(lat1);
            double radLat2 = Degrees(lat2);
            double a = radLat1 - radLat2;
            double b = Degrees(lng1) - Degrees(lng2);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s * 1000;
        }

        /// <summary>
        /// 以一个经纬度为中心计算出四个顶点 对角线 left-top,right-bottom
        /// </summary>
        /// <param name="distance">半径(米)</param>
        /// <returns></returns>
        public static Degree[] GetDegreeCoordinates(Degree degree, double distance = 10)
        {
            double dlng = 2 * Math.Asin(Math.Sin(distance / (2 * EARTH_RADIUS)) / Math.Cos(degree.X));
            dlng = Degrees(dlng);//一定转换成角度数  原PHP文章这个地方说的不清楚根本不正确 后来lz又查了很多资料终于搞定了

            double dlat = distance / EARTH_RADIUS;
            dlat = Degrees(dlat);//一定转换成角度数

            return new Degree[] { new Degree(Math.Round(degree.X + dlat,6), Math.Round(degree.Y - dlng,6)),//left-top
                                  //new Degree(Math.Round(degree.X - dlat,6), Math.Round(degree.Y - dlng,6)),//left-bottom
                                  //new Degree(Math.Round(degree.X + dlat,6), Math.Round(degree.Y + dlng,6)),//right-top
                                  new Degree(Math.Round(degree.X - dlat,6), Math.Round(degree.Y + dlng,6)) //right-bottom
            };

        }

        #endregion

        #region 获取字符串信息

        /// <summary>
        /// 日期格式化，转换失败则返回当前日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string DateTimeFormat(string date)
        {
            return TryConvertUtil.ToDateTime(date, CommonUtil.GetDBDateTime()).ToString("yyyy年MM月dd日");
        }

        public static string DateTimeFormatLine(string date)
        {
            return TryConvertUtil.ToDateTime(date, CommonUtil.GetDBDateTime()).ToString("yyyy-MM-dd");
        }

        public static string DateTimeFormatStrLine(string date)
        {
            DateTime dbNow = CommonUtil.GetDBDateTime();
            string year = dbNow.Year.ToString();
            string month = dbNow.Month.ToString();
            string day = dbNow.Day.ToString();

            if (date.Trim().Length == RegexConsts.DATE_FORMAT_DAY.Length && (date.IndexOf("-") < 0 && date.IndexOf("/") < 0))
            {
                year = TryConvertUtil.ToInt(date.Substring(0, 4), dbNow.Year).ToString();
                month = TryConvertUtil.ToInt(date.Substring(4, 2), dbNow.Month).ToString().PadLeft(2, '0');
                day = TryConvertUtil.ToInt(date.Substring(6, 2), dbNow.Day).ToString().PadLeft(2, '0');

                return string.Format("{0}-{1}-{2} 00:00:00", year, month, day);
            }
            else if (date.Trim().Length == RegexConsts.TIME_FORMAT_SECOND.Length && (date.IndexOf("-") < 0 && date.IndexOf("/") < 0 && date.IndexOf(":") < 0))
            {
                year = TryConvertUtil.ToInt(date.Substring(0, 4), dbNow.Year).ToString();
                month = TryConvertUtil.ToInt(date.Substring(4, 2), dbNow.Month).ToString().PadLeft(2, '0');
                day = TryConvertUtil.ToInt(date.Substring(6, 2), dbNow.Day).ToString().PadLeft(2, '0');
                string hour = TryConvertUtil.ToInt(date.Substring(8, 2), dbNow.Hour).ToString().PadLeft(2, '0');
                string minute = TryConvertUtil.ToInt(date.Substring(10, 2), dbNow.Minute).ToString().PadLeft(2, '0');
                string second = TryConvertUtil.ToInt(date.Substring(12, 2), dbNow.Second).ToString().PadLeft(2, '0');

                return string.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, minute, second);
            }
            else
            {
                return TryConvertUtil.ToDateTime(date, dbNow).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 输出金额格式
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string DecimalFormat(decimal src)
        {
            return string.Format("{0:F2}", src);
        }

        /// <summary>
        /// 输出数量格式(小数后面的零自动舍去)
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string NumFormat(decimal src)
        {
            var temp = src.ToString();
            if (temp.Contains("."))
            {
                return src.ToString().TrimEnd('0').TrimEnd('.');
            }
            else
            {
                return temp;
            }
        }

        /// <summary>
        /// 隐藏手机号中间四位
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string HideMoblie(string phone)
        {
            if (phone.Length >= 7)
            {
                return phone.Substring(0, 3) + "****" + phone.Substring(7);
            }
            else
            {
                return phone;
            }
        }

        /// <summary>
        /// 隐藏票券号前四位
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HideSerai(string text)
        {
            if (text.Length >= 4)
            {
                return "****" + text.Substring(4);
            }
            else
            {
                return "****";
            }
        }

        /// <summary>
        /// 输出金额格式
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string AmountFormat(decimal src, int len = 2)
        {
            if (len == 2)
            {
                return string.Format("{0:C2}", src);
            }

            return string.Format("{0:C4}", src);
        }

        /// <summary>
        /// 输出金额格式
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string DecimalFormatCutDot(decimal src)
        {
            if (src == Math.Truncate(src))
            {
                return string.Format("{0:F0}", src);
            }
            else if (src * 10 == Math.Truncate(src * 10))
            {
                return string.Format("{0:F1}", src);
            }
            return string.Format("{0:F2}", src);
        }

        /// <summary>
        /// 截断小数位数
        /// </summary>
        /// <param name="d">要截断的数字</param>
        /// <param name="s">要保留的小数位数</param>
        /// <returns>已经截断的数字</returns>
        public static decimal ToFixed(this decimal d, int s = 2)
        {
            /*
            decimal sp = Convert.ToDecimal(Math.Pow(10, s));

            if (d < 0)
                return Math.Truncate(d) + Math.Ceiling((d - Math.Truncate(d)) * sp) / sp;
            else
                return Math.Truncate(d) + Math.Floor((d - Math.Truncate(d)) * sp) / sp;*/

            //以下使用四啥五入
            var str = d.ToString();
            if (str.IndexOf('.') < 0)
            {
                return TryConvertUtil.ToDecimal(string.Concat(str, ".").PadRight(s + 1 + str.Length, '0'));
            }
            else
            {
                var strList = str.Split('.');
                if (strList.Length > 1)
                {
                    string lastStr = strList[1];
                    if (lastStr.Length == 1)
                    {
                        return TryConvertUtil.ToDecimal(str.PadRight(1 + str.Length, '0'));
                    }
                }
                return Math.Round(d, s, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// 截断小数位数
        /// </summary>
        /// <param name="d">要截断的数字</param>
        /// <param name="s">要保留的小数位数</param>
        /// <returns>已经截断的数字</returns>
        public static string ToFixedStr(this decimal d, int s = 2)
        {
            return ToFixed(d, s).ToString("n");
        }

        /// <summary>
        /// 补零
        /// </summary>
        /// <param name="str">要补零的字符串</param>
        /// <param name="zeroLength">零的长度</param>
        /// <param name="lr">1：左补零，2：右补零</param>
        /// <returns>补零后的字符串</returns>
        public static string ToFixedZero(this string str, int zeroLength, int lr = 1)
        {
            if (lr == 1)
            {
                return str.PadLeft(zeroLength, '0');
            }
            else
            {
                return str.PadRight(zeroLength, '0');
            }
        }

        /// <summary>
        /// 获取字符串长度（中文算两个字符）
        /// </summary>
        /// <param name="content">字符串</param>
        /// <returns>字符串长度</returns>
        public static int GetStringLen(string content)
        {
            return content.CnLength();
        }

        /// <summary>
        /// 获取字符串的字节数
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <returns>字符串字节长度</returns>
        public static int GetStringByteLength(string str)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            int length = 0; // 表示当前的字节数

            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                // 偶数位置，如0、2、4等，为UCS2编码中两个字节的第一个字节
                if (i % 2 == 0)
                {
                    length++;   // 在UCS2第一个字节时n加1
                }
                else
                {
                    // 当UCS2编码的第二个字节大于0时，该UCS2字符为汉字，一个汉字算两个字节
                    if (bytes[i] > 0)
                    {
                        length++;
                    }
                }
            }
            return length;
        }

        /// <summary>
        /// 获取加星号的字符串(邮箱地址/手机号均使用)
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="startIndex">开始位数</param>
        /// <param name="length">加星号位数</param>
        /// <returns>加星号的字符串</returns>
        public static string GetStarString(string str, int startIndex, int length)
        {
            if (str.Length < (startIndex + length))
            {
                return str;
            }
            else
            {
                string retStr = string.Empty;
                if (str.IndexOf("@") > 0)
                {
                    string[] mail = str.Split('@');
                    if (mail[0].Length < (startIndex + length))
                    {
                        return str;
                    }
                    else
                    {
                        retStr = mail[0].Substring(0, startIndex) + string.Empty.PadLeft(length, '*') + mail[0].Substring(startIndex + length, mail[0].Length - (startIndex + length)) + "@" + mail[1];
                    }
                }
                else
                {
                    retStr = str.Substring(0, startIndex) + string.Empty.PadLeft(length, '*') + str.Substring(startIndex + length, str.Length - (startIndex + length));
                }
                return retStr;
            }
        }

        /// <summary>
        /// 获取字符串省略(当超出最大字节数时截取字符串，根据isEllipsis参数判断是否添加省略号)
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="maxByteCount">字节数最大值</param>
        /// <param name="isEllipsis">是否添加省略号</param>
        /// <returns>处理后的字符串</returns>
        public static string GetPrifxString(string str, int maxByteCount, bool isEllipsis = true)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(str);   //Unicode编码，产生的字节数组
            int length = str.Length;                         //字符串的字符数量
            int lenBytes = length;                              //字符串真正的字符串
            int[] byteArray = new int[length];                  //字节位数组，汉字为2,非汉字为1

            for (int i = 0; i < length; i++)
            {
                if (bytes[i * 2 + 1] > 0)
                {
                    byteArray[i] = 2;
                    lenBytes++;
                }
                else
                {
                    byteArray[i] = 1;
                }
            }

            if (maxByteCount >= lenBytes)
            {
                return str;
            }
            else
            {
                //int iByteCutOut = lenBytes - maxByteCount + (isEllipsis ? 3 : 0);   //需要截取字节数，省略号在maxByteCount内
                int iByteCutOut = lenBytes - maxByteCount;   //需要截取字节数，省略号不在maxByteCount内
                int iCur = 0;                       //当前截取字节数
                int iCharCutOut = 0;                      //截取字节数
                for (int j = byteArray.Length - 1; j >= 0; j--)
                {
                    iCharCutOut++;
                    iCur += byteArray[j];
                    if (iCur >= iByteCutOut)
                    {
                        break;
                    }
                }
                string strDest = str.Substring(0, length - iCharCutOut) + (isEllipsis ? "..." : string.Empty);
                return strDest;
            }
        }

        /// <summary>
        /// 获取首字母大写字符串
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        public static string GetTitleCaseString(string sourceStr)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(sourceStr);
        }

        /// <summary>
        /// 获取GUID
        /// </summary>
        /// <returns>GUID字符串</returns>
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 获取去分隔符GUID串
        /// </summary>
        /// <returns></returns>
        public static string GetGuidNoSeparator()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        /// <summary>
        /// 获取字符串Base64位编码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="inputCharset"></param>
        /// <returns></returns>
        public static string GetBase64String(string str, string inputCharset = "utf-8")
        {
            Encoding code = Encoding.GetEncoding(inputCharset);
            byte[] data = code.GetBytes(str);
            return Convert.ToBase64String(data);
        }

        #region SQL String

        private static string stringOfBackslashChars = "\u005c\u00a5\u0160\u20a9\u2216\ufe68\uff3c";
        private static string stringOfQuoteChars =
            "\u0022\u0027\u0060\u00b4\u02b9\u02ba\u02bb\u02bc\u02c8\u02ca\u02cb\u02d9\u0300\u0301\u2018\u2019\u201a\u2032\u2035\u275b\u275c\uff07";
        private static CharClass[] charClassArray = makeCharClassArray();

        private static CharClass[] makeCharClassArray()
        {

            CharClass[] a = new CharClass[65536];
            foreach (char c in stringOfBackslashChars)
            {
                a[c] = CharClass.Backslash;
            }
            foreach (char c in stringOfQuoteChars)
            {
                a[c] = CharClass.Quote;
            }
            return a;
        }

        private static bool needsQuoting(string s)
        {
            foreach (char c in s)
            {
                if (charClassArray[c] != CharClass.None)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Escapes the string.
        /// </summary>
        /// <param name="value">The string to escape</param>
        /// <returns>The string with all quotes escaped.</returns>
        public static string EscapeString(string value)
        {
            if (!needsQuoting(value))
                return value;

            StringBuilder sb = new StringBuilder();

            foreach (char c in value)
            {
                CharClass charClass = charClassArray[c];
                if (charClass != CharClass.None)
                {
                    sb.Append("\\");
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static string DoubleQuoteString(string value)
        {
            if (!needsQuoting(value))
                return value;

            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                CharClass charClass = charClassArray[c];
                if (charClass == CharClass.Quote)
                    sb.Append(c);
                else if (charClass == CharClass.Backslash)
                    sb.Append("\\");
                sb.Append(c);
            }
            return sb.ToString();
        }

        enum CharClass : byte
        {
            None,
            Quote,
            Backslash
        }

        #endregion

        #endregion

        #region 获取签名相关

        /// <summary>
        /// 获取MD5签名
        /// </summary>
        /// <param name="key">签名密钥</param>
        /// <param name="str">签名字符串</param>
        /// <returns>MD5签名</returns>
        public static string GetMD5Sign(string key, string str)
        {
            str += "&key=" + key;
            return MD5CryptionUtil.Sign(str).ToUpper();
        }

        /// <summary>
        /// 获取签名字符串
        /// </summary>
        /// <param name="dic">待签名参数</param>
        /// <param name="key">签名密钥</param>
        /// <param name="signType">签名加密方法</param>
        /// <returns></returns>
        public static string GetSign(SortedDictionary<string, object> dic, string key, SignType signType)
        {
            var strSign = string.Empty;
            var strRequestData = string.Empty;
            StringBuilder sbRequest = new StringBuilder();
            foreach (var item in dic)
            {
                if (item.Key.ToLower() != "sign" && item.Key.ToLower() != "sign_type" && item.Value != "" && item.Value != null)
                {
                    sbRequest.AppendFormat("{0}={1}&", item.Key, item.Value);
                }
            }

            if (sbRequest.Length > 0)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    strRequestData = string.Format("{0}&key={1}", sbRequest.ToString(), key.Trim());
                }
                else
                {
                    strRequestData = sbRequest.ToString().TrimEnd('&');
                }
            }

            switch (signType)
            {
                case SignType.MD5:
                    {
                        strSign = MD5CryptionUtil.Sign(strRequestData, "").ToUpper();
                        break;
                    };
                case SignType.SHA1:
                    {
                        strSign = SecurityUtil.SHA1Encrypt(strRequestData);
                        break;
                    };
                case SignType.SHA256:
                    {
                        strSign = SecurityUtil.SHA256Encrypt(strRequestData);
                        break;
                    }
                default:
                    break;
            }

            return strSign;
        }

        #endregion

        #region 日期处理

        /// <summary>
        /// 将字符串转换成日期类型,若字符串没有包括HH:mm:ss格式，则时间返回23:59:59,否则照常返回
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime? GetDate(string date)
        {
            if (!string.IsNullOrEmpty(date) && date.Trim() != string.Empty)
            {
                DateTime dt;
                if (DateTime.TryParse(date, out dt))
                {
                    if (!date.Contains(":"))
                    {
                        dt = dt.AddDays(1).AddSeconds(-1);
                    }
                }
                else
                {
                    throw new MessageException(string.Format("{0}不是日期格式", date));
                }

                return dt;
            }
            {
                return null;
            }
        }

        /// <summary>
        /// Unix时间戳（Unix timestamp）是一种时间表示方法，
        /// 定义为从格林威治时间1970年01月01日00时00分00秒起至现在的总秒数
        /// </summary>
        /// <returns></returns>
        public static long GetUnixTimestamp()
        {
            return (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).Ticks / 10000000;
        }

        /// <summary>
        /// Unix时间戳（Unix timestamp）是一种时间表示方法，
        /// 定义为从格林威治时间1970年01月01日00时00分00秒起至现在的总秒数
        /// </summary>
        /// <returns></returns>
        public static long GetUnixTimestamp(object date)
        {
            return (ConvertUtil.ToDateTime(date, new DateTime(1970, 1, 1)).ToUniversalTime() - new DateTime(1970, 1, 1)).Ticks / 10000000;
        }

        /// <summary>
        /// 获取日期，从格林威治时间
        /// </summary>
        /// <param name="unixTimestamp">格林威治时间1970年01月01日00时00分00秒起至现在的总秒数</param>
        /// <returns></returns>
        public static DateTime FromUnixTime(long unixTimestamp)
        {
            return new DateTime(1970, 1, 1).AddSeconds(unixTimestamp).ToLocalTime();
        }

        #endregion

        #region 获取计算机硬件信息

        /// <summary>
        /// 获取当前操作系统的名称、版本、位数
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentOSInfo()
        {
            var info = new Microsoft.VisualBasic.Devices.ComputerInfo();
            string osBit = Environment.Is64BitOperatingSystem ? "64 bit" : "32bit";
            return string.Format("{0} - {1} - {2}", info.OSFullName, info.OSVersion, osBit);
        }

        /// <summary>
        /// 通过NetBios获取MAC地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddressByNetBios()
        {
            string macAddress = "";
            try
            {
                string addr = "";
                int cb;
                ASTAT adapter;
                NCB Ncb = new NCB();
                char uRetCode;
                LANA_ENUM lenum;

                Ncb.ncb_command = (byte)NCBCONST.NCBENUM;
                cb = Marshal.SizeOf(typeof(LANA_ENUM));
                Ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                Ncb.ncb_length = (ushort)cb;
                uRetCode = Netbios(ref   Ncb);
                lenum = (LANA_ENUM)Marshal.PtrToStructure(Ncb.ncb_buffer, typeof(LANA_ENUM));
                Marshal.FreeHGlobal(Ncb.ncb_buffer);
                if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                    return "";

                for (int i = 0; i < lenum.length; i++)
                {
                    Ncb.ncb_command = (byte)NCBCONST.NCBRESET;
                    Ncb.ncb_lana_num = lenum.lana[i];
                    uRetCode = Netbios(ref   Ncb);
                    if (uRetCode != (short)NCBCONST.NRC_GOODRET)
                        return "";

                    Ncb.ncb_command = (byte)NCBCONST.NCBASTAT;
                    Ncb.ncb_lana_num = lenum.lana[i];
                    Ncb.ncb_callname[0] = (byte)'*';
                    cb = Marshal.SizeOf(typeof(ADAPTER_STATUS)) + Marshal.SizeOf(typeof(NAME_BUFFER)) * (int)NCBCONST.NUM_NAMEBUF;
                    Ncb.ncb_buffer = Marshal.AllocHGlobal(cb);
                    Ncb.ncb_length = (ushort)cb;
                    uRetCode = Netbios(ref   Ncb);
                    adapter.adapt = (ADAPTER_STATUS)Marshal.PtrToStructure(Ncb.ncb_buffer, typeof(ADAPTER_STATUS));
                    Marshal.FreeHGlobal(Ncb.ncb_buffer);

                    if (uRetCode == (short)NCBCONST.NRC_GOODRET)
                    {
                        if (i > 0)
                            addr += ":";
                        addr = string.Format("{0,2:X}:{1,2:X}:{2,2:X}:{3,2:X}:{4,2:X}:{5,2:X}",
                              adapter.adapt.adapter_address[0],
                              adapter.adapt.adapter_address[1],
                              adapter.adapt.adapter_address[2],
                              adapter.adapt.adapter_address[3],
                              adapter.adapt.adapter_address[4],
                              adapter.adapt.adapter_address[5]);
                    }
                }
                macAddress = addr.Replace(' ', '0').Replace(":", string.Empty);

            }
            catch
            {
            }
            return macAddress;
        }

        #region 获取底层硬件信息

        internal enum NCBCONST
        {
            NCBNAMSZ = 16,
            MAX_LANA = 254,
            NCBENUM = 0x37,
            NRC_GOODRET = 0x00,
            NCBRESET = 0x32,
            NCBASTAT = 0x33,
            NUM_NAMEBUF = 30,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ADAPTER_STATUS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] adapter_address;
            public byte rev_major;
            public byte reserved0;
            public byte adapter_type;
            public byte rev_minor;
            public ushort duration;
            public ushort frmr_recv;
            public ushort frmr_xmit;
            public ushort iframe_recv_err;
            public ushort xmit_aborts;
            public uint xmit_success;
            public uint recv_success;
            public ushort iframe_xmit_err;
            public ushort recv_buff_unavail;
            public ushort t1_timeouts;
            public ushort ti_timeouts;
            public uint reserved1;
            public ushort free_ncbs;
            public ushort max_cfg_ncbs;
            public ushort max_ncbs;
            public ushort xmit_buf_unavail;
            public ushort max_dgram_size;
            public ushort pending_sess;
            public ushort max_cfg_sess;
            public ushort max_sess;
            public ushort max_sess_pkt_size;
            public ushort name_count;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NAME_BUFFER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] name;
            public byte name_num;
            public byte name_flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NCB
        {
            public byte ncb_command;
            public byte ncb_retcode;
            public byte ncb_lsn;
            public byte ncb_num;
            public IntPtr ncb_buffer;
            public ushort ncb_length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] ncb_callname;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NCBNAMSZ)]
            public byte[] ncb_name;
            public byte ncb_rto;
            public byte ncb_sto;
            public IntPtr ncb_post;
            public byte ncb_lana_num;
            public byte ncb_cmd_cplt;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] ncb_reserve;
            public IntPtr ncb_event;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LANA_ENUM
        {
            public byte length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.MAX_LANA)]
            public byte[] lana;
        }

        [StructLayout(LayoutKind.Auto)]
        internal struct ASTAT
        {
            public ADAPTER_STATUS adapt;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NCBCONST.NUM_NAMEBUF)]
            public NAME_BUFFER[] NameBuff;
        }

        [DllImport("NETAPI32.DLL")]
        internal static extern char Netbios(ref   NCB ncb);

        #endregion

        #endregion

        #region 上传文件操作

        /// <summary>
        /// 图片文件扩展名集合
        /// </summary>
        private static string[] imgExtName = new string[3] { ".gif", ".jpg", ".png" };

        /// <summary>
        /// 传入图片名检查后缀是否成立。
        /// </summary>
        /// <param name="extName">上传的图片名</param>
        /// <returns>真或假</returns>
        public static bool CheckIsImgFile(string extName)
        {
            return imgExtName.Contains(extName.ToLower());
        }

        /// <summary>
        /// 校验上传文件大小
        /// </summary>
        /// <param name="fileUploader">上传文件</param>
        /// <returns>true:文件大小合法，false：文件大小超过限制</returns>
        public static bool CheckFileSize(HttpPostedFileBase fileUploader)
        {
            int fileSize = fileUploader.ContentLength;
            int maxSize = ConvertUtil.ToInt(ConfigUtil.GetConfig("UploadFileMaxSize"));
            if (maxSize <= 0) maxSize = 2 * 1024 * 1024; //2M
            return fileSize <= maxSize;
        }

        /// <summary>
        /// 获得格式的文件大小
        /// </summary>
        /// <param name="fileSizeBytes">用字节表示的文件的大小</param>
        /// <returns>格式化后的文件大小文本（单位：M）</returns>
        public static string GetFileSizeFormatMB(long fileSizeBytes)
        {
            return Math.Round((decimal)fileSizeBytes / (1024 * 1024), 3).ToString();
        }

        /// <summary>
        /// 获取文件虚拟路径
        /// </summary>
        /// <param name="fileName">客户端上传文件的完全限定名</param>
        /// <returns>文件虚拟路径</returns>
        public static string GetVirtualPath(string fileName)
        {
            string extName = Path.GetExtension(fileName).ToLower();
            DateTime now = DateTime.Now;
            string guid = Guid.NewGuid().ToString();
            return String.Format("/Content/Images/{0}/{1}/{2}/{3}/{4}{5}", now.Year, now.Month, now.Day, now.Hour, guid, extName);
        }

        /// <summary>
        /// 获取物理虚拟路径
        /// </summary>
        /// <param name="virtualPath">虚拟地址</param>
        /// <returns>文件绝对路径</returns>
        public static string GetPhysicalPath(string virtualPath)
        {
            //物理地址
            string physicalPath = HttpContext.Current.Server.MapPath(virtualPath);

            string[] path = physicalPath.Split('\\');
            string filePath = physicalPath.Replace("\\" + path[path.Length - 1], string.Empty);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            return physicalPath;
        }

        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件名称</returns>
        public static string GetFileName(string filePath)
        {
            try
            {
                filePath = Regex.Replace(filePath, @"\s+", "", RegexOptions.Singleline);
                string extName = Path.GetExtension(filePath).ToLower();
                string[] file = filePath.ToString().Split('/');
                return file[file.Length - 1].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

        #region 其他杂项

        /// <summary>
        /// 检查是否是Debug模式
        /// </summary>
        /// <returns>布尔</returns>
        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// 获取敬语
        /// </summary>
        /// <returns>敬语</returns>
        public static string GetHonorific()
        {
            DateTime dt = DateTime.Now;
            int hour = dt.Hour;

            string honorific = string.Empty;
            switch (hour)
            {
                case 5:
                case 6:
                case 7:
                    honorific = "早上好";
                    break;
                case 8:
                case 9:
                case 10:
                    honorific = "上午好";
                    break;
                case 11:
                case 12:
                case 13:
                    honorific = "中午好";
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                    honorific = "下午好";
                    break;
                default:
                    honorific = "晚上好";
                    break;
            }
            return honorific + "，";
        }

        /// <summary>
        /// 评分星星
        /// </summary>
        /// <param name="score">分数</param>
        /// <returns>Dom</returns>
        public static string GetScoreStar(decimal score)
        {
            decimal total = 5;
            decimal light = Math.Floor(score);
            decimal dark = total - (int)Math.Ceiling(score);
            decimal digit = (score - light) * 10;

            string starLight = "<s></s>";
            string starDark = "<s class='star0'></s>";
            string star = "<s class='star{0}'></s>";

            starLight = starLight.Repeat((int)light);
            starDark = starDark.Repeat((int)dark);

            string starHalf = string.Empty;
            if (digit > 0)
            {
                starHalf = string.Format(star, (int)digit);
            }
            return string.Format("{0}{1}{2}", starLight, starHalf, starDark);
        }

        /// <summary>
        /// 获得Javascript的重定向地址，在重定向之前会用先弹出提示文本
        /// </summary>
        /// <param name="promptText">提示文本</param>
        /// <param name="redirectUrl">要重定向的URL</param>
        /// <returns>JS重定向内容</returns>
        public static string GetRedirectJs(string promptText, string redirectUrl)
        {
            string result = String.Format(
              @"<script type=""text/javascript"">
                        alert(""{0}"");
                        location.href=""{1}"";
                </script> ",
                redirectUrl
            );
            return result;
        }

        /// <summary>
        /// 根据设备类型返回设备大类名
        /// </summary>
        /// <param name="mobileType">设备类型</param>
        /// <returns>设备大类名：1.x——ANDORID；2.x——IOS</returns>
        public static string GetMobileName(string mobileType)
        {
            string mobileName = string.Empty;
            if (mobileType.Substring(0, 1) == "1")
            {
                mobileName = "ANDROID";
            }
            else if (mobileType.Substring(0, 1) == "2")
            {
                mobileName = "IOS";
            }
            return mobileName;
        }

        /// <summary>
        /// 获取绝对路径
        /// </summary>
        /// <param name="relativelyUrl">相对路径</param>
        /// <returns>绝对路径</returns>
        public static string GetAbsoluteUrl(string relativelyUrl)
        {
            return "http://" + HttpContext.Current.Request.Url.Authority + relativelyUrl;
        }

        public static DateTime DbNow
        {
            get
            {
                return GetDBDateTime();
            }
        }

        /// <summary>
        /// 获得数据库服务器当前的日期和时间
        /// </summary>
        /// <returns>数据库服务器当前的日期和时间</returns>
        public static DateTime GetDBDateTime()
        {
            return TryConvertUtil.ToDateTime(DbUtil.DataManager.Current.IData.ExecuteScalar("SELECT NOW()"));
        }

        /// <summary>
        /// 获取上周一、周日日期
        /// </summary>
        /// <returns></returns>
        public static Tuple<DateTime, DateTime> GetPreWeekZone()
        {
            //当前数据库时间
            DateTime dbNow = CommonUtil.GetDBDateTime();

            //上周一
            DateTime preMonday = dbNow.AddDays(1 - TryConvertUtil.ToInt(dbNow.DayOfWeek.ToString("d"))).AddDays(-7).Date;

            //上周日
            DateTime preSunday = preMonday.AddDays(7).Date.AddSeconds(-1);

            return Tuple.Create(preMonday, preSunday);
        }

        /// <summary>
        /// 获取本周几
        /// </summary>
        /// <param name="day">周几</param>
        /// <returns>日期</returns>
        public static DateTime GetCurDayOfWeek(int day)
        {
            //当前数据库时间
            DateTime dbNow = CommonUtil.GetDBDateTime();
            if (day <= 0)
                day = 1;
            else if (day >= 8)
                day = 7;

            return dbNow.AddDays(day - TryConvertUtil.ToInt(dbNow.DayOfWeek.ToString("d"))).Date;
        }

        public class Degree
        {
            public double X { get; set; }

            public double Y { get; set; }

            public Degree(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        #endregion
    }
}
