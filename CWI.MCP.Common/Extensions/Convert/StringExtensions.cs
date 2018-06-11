//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋       2014/09/19         创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace  CWI.MCP.Common.Extensions
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 是否为Null Or Empty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 是否为Null Or Empty 不包含空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullEmptyWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 是否为Null Or Empty 不包含空格,并给予默认值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string IsNullEmptyWhiteSpace(this string str, string defaultValue)
        {
            return str.IsNullEmptyWhiteSpace() ? defaultValue : str;
        }

        /// <summary>
        /// 是否为Null Or Empty,如果为Null Or Empty，则用默认值代替
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string IsNullEmpty(this string str, string defaultValue)
        {
            return str.IsNullEmpty() ? defaultValue : str;
        }

        /// <summary>
        /// 把系统符转换成html
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        public static string UnHtml(this string htmlStr)
        {
            if (htmlStr.IsNullEmpty()) return string.Empty;
            return htmlStr.Replace("\"", "\\\"").ShowXmlHtml().Replace(" ", "&nbsp;").Replace("\n", "<br>");
        }

        /// <summary>
        ///  把系统符转换成html
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        public static string StrToHtml(this string htmlStr)
        {
            if (htmlStr.IsNullEmpty()) return string.Empty;
            return htmlStr.Replace(" ", "&nbsp;").Replace("\r\n", "<br>");
        }

        /// <summary>
        /// 把html转换成系统符
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        public static string HtmlToStr(this string htmlStr)
        {
            if (htmlStr.IsNullEmpty()) return string.Empty;
            return htmlStr.Replace("&nbsp;", " ").Replace("<br>", "\r\n");
        }

        /// <summary>
        /// 把html转成xml标签
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        public static string ShowXmlHtml(this string htmlStr)
        {
            if (htmlStr.IsNullEmpty()) return string.Empty;
            string str = htmlStr.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;");
            return str;
        }

        /// <summary>
        /// 过滤html和javascript标签
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        public static string ShowHtml(this string htmlStr)
        {
            if (htmlStr.IsNullEmpty()) return string.Empty;
            string str = htmlStr;

            str = Regex.Replace(str, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "_$1.$2", RegexOptions.IgnoreCase);
            str = new Regex("<script", RegexOptions.IgnoreCase).Replace(str, "<_script");
            str = new Regex("<object", RegexOptions.IgnoreCase).Replace(str, "<_object");
            str = new Regex("javascript:", RegexOptions.IgnoreCase).Replace(str, "_javascript:");
            str = new Regex("vbscript:", RegexOptions.IgnoreCase).Replace(str, "_vbscript:");
            str = new Regex("expression", RegexOptions.IgnoreCase).Replace(str, "_expression");
            str = new Regex("@import", RegexOptions.IgnoreCase).Replace(str, "_@import");
            str = new Regex("<iframe", RegexOptions.IgnoreCase).Replace(str, "<_iframe");
            str = new Regex("<frameset", RegexOptions.IgnoreCase).Replace(str, "<_frameset");
            str = Regex.Replace(str, @"(\<|\s+)o([a-z]+\s?=)", "$1_o$2", RegexOptions.IgnoreCase);
            str = new Regex(@" (on[a-zA-Z ]+)=", RegexOptions.IgnoreCase).Replace(str, " _$1=");
            return str;
        }

        /// <summary>
        /// 得到字符串长度（一个汉字为两位）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int CnLength(this string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        /// <summary>
        /// 截取字符串（一个汉字为两位）
        /// </summary>
        /// <param name="strInput"></param>
        /// <param name="len">截取多长</param>
        /// <param name="flg">截取后填充字符</param>
        /// <returns></returns>
        public static string SubString(this string strInput, int len, string flg)
        {
            string myResult = string.Empty;
            if (len >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(strInput);
                if (bsSrcString.Length > len)
                {
                    int nRealLength = len;
                    int[] anResultFlag = new int[len];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = 0; i < len; i++)
                    {
                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3) nFlag = 1;
                        }
                        else nFlag = 0;
                        anResultFlag[i] = nFlag;
                    }
                    if ((bsSrcString[len - 1] > 127) && (anResultFlag[len - 1] == 1))
                        nRealLength = len + 1;
                    bsResult = new byte[nRealLength];
                    Array.Copy(bsSrcString, bsResult, nRealLength);
                    myResult = Encoding.Default.GetString(bsResult);
                    myResult = myResult + (nRealLength >= strInput.CnLength() ? "" : flg);
                }
                else myResult = strInput;
            }
            return myResult;
        }

        /// <summary>
        /// 得到文件扩展名
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetFileExtends(this string filename)
        {
            string ext = null;
            if (filename.IndexOf('.') > 0)
            {
                string[] fs = filename.Split('.');
                ext = fs[fs.Length - 1];
            }
            return ext;
        }

        /// <summary>
        /// 得到url的文件名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlFileName(this string url)
        {
            if (url == null) return "";
            string[] strs1 = url.Split(new char[] { '/' });
            return strs1[strs1.Length - 1].Split(new char[] { '?' })[0];
        }

        /// <summary>
        /// 得到html代码中的所有Href
        /// </summary>
        /// <param name="HtmlCode"></param>
        /// <returns></returns>
        public static IList<string> GetHref(this string HtmlCode)
        {
            IList<string> MatchVale = new List<string>();
            string Reg = @"(h|H)(r|R)(e|E)(f|F) *= *('|"")?((\w|\\|\/|\.|:|-|_)+)('|""| *|>)?";
            foreach (Match m in Regex.Matches(HtmlCode, Reg))
            {
                MatchVale.Add((m.Value).ToLower().Replace("href=", "").Trim().TrimEnd('\'').TrimEnd('"').TrimStart('\'').TrimStart('"'));
            }
            return MatchVale;
        }

        /// <summary>
        /// 得到html中的所有SRC
        /// </summary>
        /// <param name="HtmlCode"></param>
        /// <returns></returns>
        public static IList<string> GetSrc(this string HtmlCode)
        {
            IList<string> MatchVale = new List<string>();
            string Reg = @"(s|S)(r|R)(c|C) *= *('|"")?((\w|\\|\/|\.|:|-|_)+)('|""| *|>)?";
            foreach (Match m in Regex.Matches(HtmlCode, Reg))
            {
                MatchVale.Add((m.Value).ToLower().Replace("src=", "").Trim().TrimEnd('\'').TrimEnd('"').TrimStart('\'').TrimStart('"'));
            }
            return MatchVale;
        }

        /// <summary>
        /// 获取Email主机名
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        public static string GetEmailHostName(this string strEmail)
        {
            if (strEmail.IndexOf("@") < 0) return "";
            return strEmail.Substring(strEmail.LastIndexOf("@")).ToLower();
        }

        /// <summary>
        /// 转成字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value)
        {
            return value.ToBytes(null);
        }

        /// <summary>
        /// 转成字节
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value, Encoding encoding)
        {
            encoding = (encoding ?? Encoding.Default);
            return encoding.GetBytes(value);
        }

        /// <summary>
        /// 移除所有html
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveAllHTML(this string content)
        {
            string pattern = "<[^>]*>";
            return Regex.Replace(content, pattern, string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Reverse(this string value)
        {
            if (value.IsNullEmpty()) return string.Empty;

            var chars = value.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        /// format格式化
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        /// <summary>
        /// format格式化
        /// </summary>
        /// <param name="text"></param>
        /// <param name="provider"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string text, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, text, args);
        }

        /// <summary>
        /// 替换（正则实现）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regexPattern"></param>
        /// <param name="replaceValue"></param>
        /// <returns></returns>
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue)
        {
            return ReplaceWith(value, regexPattern, replaceValue, RegexOptions.None);
        }

        /// <summary>
        ///  替换（正则实现）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regexPattern"></param>
        /// <param name="replaceValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue, RegexOptions options)
        {
            return Regex.Replace(value, regexPattern, replaceValue, options);
        }

        /// <summary>
        ///  替换（正则实现）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regexPattern"></param>
        /// <param name="evaluator"></param>
        /// <returns></returns>
        public static string ReplaceWith(this string value, string regexPattern, MatchEvaluator evaluator)
        {
            return ReplaceWith(value, regexPattern, RegexOptions.None, evaluator);
        }

        /// <summary>
        ///  替换（正则实现）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regexPattern"></param>
        /// <param name="options"></param>
        /// <param name="evaluator"></param>
        /// <returns></returns>
        public static string ReplaceWith(this string value, string regexPattern, RegexOptions options, MatchEvaluator evaluator)
        {
            return Regex.Replace(value, regexPattern, evaluator, options);
        }

        /// <summary>
        ///  替换（正则实现）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regexPattern"></param>
        /// <param name="ReplaceString"></param>
        /// <param name="IsCaseInsensetive"></param>
        /// <returns></returns>
        public static string ReplaceWith(this string value, string regexPattern, string ReplaceString, bool IsCaseInsensetive)
        {
            return Regex.Replace(value, regexPattern, ReplaceString, IsCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }
  
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regexPattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string[] Split(this string value, string regexPattern, RegexOptions options)
        {
            return Regex.Split(value, regexPattern, options);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regexPattern"></param>
        /// <returns></returns>
        public static string[] Split(this string value, string regexPattern)
        {
            return value.Split(regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// 检查是否包含指定数组的内容
        /// </summary>
        /// <param name="value"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public static bool ContainsArray(this string value, params string[] keywords)
        {
            return keywords.All((s) => value.Contains(s));
        }

        /// <summary>
        /// 用换行分割
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> GetLines(this string text)
        {
            return text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        }

        /// <summary>
        /// 是否与指定数据匹配
        /// </summary>
        /// <param name="str"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool IsMatch(this string str, string op)
        {
            if (str.Equals(String.Empty) || str == null) return false;
            Regex re = new Regex(op, RegexOptions.IgnoreCase);
            return re.IsMatch(str);
        }

        /// <summary>
        /// 是否为IP地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIP(this string input)
        {
            return input.IsMatch(@"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"); //@"^(([01]?[\d]{1,2})|(2[0-4][\d])|(25[0-5]))(\.(([01]?[\d]{1,2})|(2[0-4][\d])|(25[0-5]))){3}$";
        }

        /// <summary>
        /// 是否为数字
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsNumber(this string strNumber)
        {
            string pet = @"^([0-9])[0-9]*(\.\w*)?$";
            return strNumber.IsMatch(pet);
        }

        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsDouble(this string input)
        {
            string pet = @"^[0-9]*[1-9][0-9]*$";//@"^\d{1,}$"//整数校验常量//@"^-?(0|\d+)(\.\d+)?$"//数值校验常量 
            return input.IsMatch(pet);
        }

        /// <summary>
        /// 是否为整型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsInt(this string input)
        {
            string pet = @"^[0-9]*$"; //@"^([0-9])[0-9]*(\.\w*)?$";
            return input.IsMatch(pet);
        }

        /// <summary>
        /// 数组是否为数字
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsNumberArray(this string[] strNumber)
        {
            if (strNumber == null) return false;
            if (strNumber.Length < 1) return false;
            foreach (string id in strNumber)
                if (!id.IsNumber()) return false;
            return true;
        }

        /// <summary>
        /// 是否为Email
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmail(this string input)
        {
            string pet = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";//@"^\w+((-\w+)|(\.\w+))*\@\w+((\.|-)\w+)*\.\w+$";
            return input.IsMatch(pet);
        }

        /// <summary>
        /// 是否为URL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsUrl(this string input)
        {
            string pet = @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$";//@"^http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            return input.IsMatch(pet);
        }

        /// <summary>
        /// 是否为日期类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsDateTimeByRegex(this string input)
        {
            //string pet = @"^(?:(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(\/|-|\.)(?:0?2\1(?:29))$)|(?:(?:1[6-9]|[2-9]\d)?\d{2})(\/|-|\.)(?:(?:(?:0?[13578]|1[02])\2(?:31))|(?:(?:0?[1,3-9]|1[0-2])\2(29|30))|(?:(?:0?[1-9])|(?:1[0-2]))\2(?:0?[1-9]|1\d|2[0-8]))$";
            string pet = @"^(?=\d)(?:(?!(?:1582(?:\.|-|\/)10(?:\.|-|\/)(?:0?[5-9]|1[0-4]))|(?:1752(?:\.|-|\/)0?9(?:\.|-|\/)(?:0?[3-9]|1[0-3])))(?=(?:(?!000[04]|(?:(?:1[^0-6]|[2468][^048]|[3579][^26])00))(?:(?:\d\d)(?:[02468][048]|[13579][26]))\D0?2\D29)|(?:\d{4}\D(?!(?:0?[2469]|11)\D31)(?!0?2(?:\.|-|\/)(?:29|30))))(\d{4})([-\/.])(0?\d|1[012])\2((?!00)[012]?\d|3[01])(?:$|(?=\x20\d)\x20))?((?:(?:0?[1-9]|1[012])(?::[0-5]\d){0,2}(?:\x20[aApP][mM]))|(?:[01]?\d|2[0-3])(?::[0-5]\d){1,2})?$";
            return input.IsMatch(pet);
        }

        /// <summary>
        /// 是否为日期类型
        /// </summary>
        /// <param name="DateTimeStr"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string DateTimeStr)
        {
            try { DateTime _dt = DateTime.Parse(DateTimeStr); return true; }
            catch { return false; }
        }

        /// <summary>
        /// 是否为邮编
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsZip(this string input)
        {
            return input.IsMatch(@"\d{6}");
        }

        public static bool IsDate(this string DateStr)
        {
            try { DateTime _dt = DateTime.Parse(DateStr); return true; }
            catch { return false; }
        }


        public static bool IsTime(this string TimeStr)
        {
            return TimeStr.IsMatch(@"^([0-1]\\d|2[0-3]):[0-5]\\d:[0-5]\\d$");//^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$
        }

        public static bool IsAlphaNumeric(this string input)
        {
            return input.IsMatch(@"[^a-zA-Z0-9]");
        }

        public static bool IsTelepone(this string input)
        {
            return input.IsMatch(@"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");//："^(\(\d{3,4}-)|\d{3.4}-)?\d{7,8}$
        }

        public static bool IsMobile(this string input)
        {
            return input.IsMatch(@"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");
        }

        public static bool IsBase64String(this string str)
        {
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }

        public static bool IsYear(this string input)
        {
            return Regex.IsMatch(input, @"^(19\d\d)|(200\d)$");
        }

        public static bool IsImgFileName(this string filename)
        {
            filename = filename.Trim();
            if (filename.EndsWith(".") || (filename.IndexOf(".") == -1)) return false;
            string str = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
            if (((str != "jpg") && (str != "jpeg")) && ((str != "png") && (str != "bmp"))) return (str == "gif");
            return true;
        }

        public static bool IsGuid(this string s)
        {
            if (s.IsNullEmpty()) return false;
            Regex format = new Regex("^[A-Fa-f0-9]{32}$|" +
                "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2},{0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$");
            Match match = format.Match(s);
            return match.Success;
        }

        public static Guid ToGuid(this string target)
        {
            if ((!target.IsNullEmpty()) && (target.Trim().Length == 22))
            {
                string encoded = string.Concat(target.Trim().Replace("-", "+").Replace("_", "/"), "==");
                byte[] base64 = Convert.FromBase64String(encoded);
                return new Guid(base64);
            }
            return Guid.Empty;
        }
    }
}
