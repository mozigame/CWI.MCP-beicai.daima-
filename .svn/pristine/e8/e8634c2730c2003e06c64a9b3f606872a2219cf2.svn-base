//版权信息：版权所有(C) 2015，COOLWI.COM
//变更历史：
//    姓名         日期          说明
//--------------------------------------------------------
//   王军锋     2012/03/31       创建

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 正则表达式公共类
    /// </summary>
    public class RegexUtil
    {
        /// <summary>
        /// 匹配组数据
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="pattern">模式匹配</param>
        /// <param name="groupName">组名</param>
        /// <returns>IList列表</returns>
        public static IList<string> GetList(ref string source, string pattern, string groupName)
        {
            return GetList(ref source, pattern, groupName, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 匹配组数据
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="pattern">模式匹配</param>
        /// <param name="groupName">组名</param>
        /// <param name="options">正则参数</param>
        /// <returns>IList列表</returns>
        public static IList<string> GetList(ref string source, string pattern, string groupName, RegexOptions options)
        {
            IList<string> matchNoList = new List<string>();
            Regex reg = new Regex(pattern, options);
            MatchCollection matches = reg.Matches(source);
            foreach (Match g in matches)
            {
                matchNoList.Add(g.Groups[groupName].Value);
            }
            return matchNoList;
        }

        /// <summary>
        /// 匹配组数据
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="pattern">模式匹配</param>
        /// <param name="groupName">组名</param>
        /// <returns>IList列表</returns>
        public static IList<string> GetRegexArray(ref string source, string pattern, string groupName)
        {
            IList<string> matchNoList = new List<string>();
            Regex reg = new Regex(pattern);
            Match m = reg.Match(source);

            // int matchCount = 0;
            while (m.Success)
            {
                //  ++matchCount;
                matchNoList.Add(m.Groups[groupName].Value);
                m = m.NextMatch();
            }
            return matchNoList;
        }

        /// <summary>
        /// Get Dictionary
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pattern">pattern</param>
        /// <param name="keyName">keyName</param>
        /// <param name="valueName">valueName</param>
        /// <returns>IDictionary</returns>
        public static IDictionary<string, string> GetDictionary(ref string source, string pattern, string keyName, string valueName)
        {
            IDictionary<string, string> matchNoList = new Dictionary<string, string>();
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            MatchCollection matches = reg.Matches(source);
            foreach (Match g in matches)
            {
                matchNoList[g.Groups[keyName].Value] = g.Groups[valueName].Value;
            }
            return matchNoList;
        }

        /// <summary>
        /// Get String
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pattern">pattern</param>
        /// <param name="groupName">groupName</param>
        /// <returns>string</returns>
        public static string GetString(ref string source, string pattern, string groupName)
        {
            return GetString(ref source, pattern, groupName, RegexOptions.None);
        }

        /// <summary>
        /// Get String
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pattern">pattern</param>
        /// <param name="groupName">groupName</param>
        /// <param name="options">options</param>
        /// <returns>string</returns>
        public static string GetString(ref string source, string pattern, string groupName, RegexOptions options)
        {
            Regex reg = new Regex(pattern, options);
            Match matches = reg.Match(source);
            if (matches.Success)
                return matches.Groups[groupName].Value;
            return string.Empty;
        }

        /// <summary>
        /// Is Match
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="regexString">RegexString</param>
        /// <returns>bool</returns>
        public static bool IsMatch(ref string source, string regexString)
        {
            if (source == null)
                return false;
            System.Text.RegularExpressions.Match match = Regex.Match(source, regexString, System.Text.RegularExpressions.RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            if (match.Success)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Is Match
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="regexString">RegexString</param>
        /// <param name="groupName">groupName</param>
        /// <param name="groupValue">groupValue</param>
        /// <returns>bool</returns>
        public static bool IsMatch(ref string source, string regexString, string[] groupName, ref string[] groupValue)
        {
            if (source == null)
                return false;
            System.Text.RegularExpressions.Match match = Regex.Match(source, regexString, System.Text.RegularExpressions.RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            if (match.Success)
            {
                for (int i = 0; i < groupName.Length; i++)
                {
                    groupValue[i] = match.Groups[groupName[i]].Value;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Macth Replace
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="regexString">RegexString</param>
        /// <param name="resultString">ResultString</param>
        /// <returns>string</returns>
        public static string MacthReplace(string source, string regexString, string resultString)
        {
            if (source == null || source.Length == 0)
                return string.Empty;
            System.Text.RegularExpressions.Regex regex = new Regex(regexString, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            return regex.Replace(source, resultString);

        }

        /// <summary>
        /// 校验日期
        /// </summary>
        /// <param name="strSource">strSource</param>
        /// <returns>bool</returns>
        public static bool IsDate(string strSource)
        {
            return Regex.IsMatch(strSource, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
        }

        /// <summary>          
        /// 是否为时间型字符串          
        /// </summary>
        /// <param name="strSource">时间字符串(15:00)</param>
        /// <returns>bool</returns>          
        public static bool IsTime(string strSource)
        {
            return Regex.IsMatch(strSource, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d)$");
        }

        /// <summary>          
        /// 是否为日期+时间型(15:00)字符串          
        /// </summary>
        /// <param name="strSource">strSource</param>          
        /// <returns>bool</returns>          
        public static bool IsDateTime(string strSource)
        {
            return Regex.IsMatch(strSource, @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d)$");
        }

        /// <summary>
        /// 校验是否为年月
        /// </summary>
        /// <param name="strSource">输入字符串</param>
        /// <returns>是：true,否：false</returns>
        public static bool IsYearMonth(string strSource)
        {
            return Regex.IsMatch(strSource, RegexConsts.YEAR_MONTH);
        }

        /// <summary>
        /// 校验是否为手机号码
        /// </summary>
        /// <param name="strSource">输入字符串</param>
        /// <returns>是：true,否：false</returns>
        public static bool IsMobile(string strSource)
        {
            return Regex.IsMatch(strSource, RegexConsts.MOBILE_PATTERN);
        }

        /// <summary>
        /// 校验是否为邮箱地址
        /// </summary>
        /// <param name="strSource">输入字符串</param>
        /// <returns>是：true,否：false</returns>
        public static bool IsEmail(string strSource)
        {
            return Regex.IsMatch(strSource, RegexConsts.EMAIL_PATTERN);
        }
    }
}