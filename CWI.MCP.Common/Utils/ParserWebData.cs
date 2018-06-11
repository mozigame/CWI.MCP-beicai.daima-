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
    /// ParserWebData
    /// </summary>
    public class ParserWebData
    {
        /// <summary>
        /// pattern1,pattern2
        /// </summary>
        private string pattern1, pattern2;

        /// <summary>
        /// ParserWebData
        /// </summary>
        /// <param name="pattern1">pattern1</param>
        /// <param name="pattern2">pattern2</param>
        public ParserWebData(string pattern1, string pattern2)
        {
            this.pattern1 = pattern1;
            this.pattern2 = pattern2;
        }

        /// <summary>
        /// SetPattern
        /// </summary>
        /// <param name="pattern1">pattern1</param>
        /// <param name="pattern2">pattern2</param>
        public void SetPattern(string pattern1, string pattern2)
        {
            this.pattern1 = pattern1;
            this.pattern2 = pattern2;
        }

        /// <summary>
        /// 解析网页上分析的数据
        /// </summary>
        /// <param name="htmlText">数据源</param>
        /// <param name="keyName">正则表达式里面的分组key</param>
        /// <param name="valueName">正则表达式里面的分组value</param>
        /// <returns>IList列表</returns>
        public IList<IList<string>> Parser(ref string htmlText, string keyName, string valueName)
        {
            IDictionary<string, string> dict = RegexUtil.GetDictionary(ref htmlText, pattern1, keyName, valueName);
            IList<IList<string>> list = new List<IList<string>>();
            string temp;
            foreach (KeyValuePair<string, string> obj in dict)
            {
                temp = string.Format("{0},", obj.Value);
                IList<string> list1 = RegexUtil.GetRegexArray(ref temp, pattern2, valueName);
                list.Add(list1);
            }
            return list;
        }

        /// <summary>
        /// 将Unicode转换成字符串
        /// </summary>
        /// <param name="unicode">Unicode，示例：&#22823;&#23567;&#30436;</param>
        /// <returns>返回中文字符串，示例：大小盤</returns>
        public static string TransToString(string unicode)
        {
            Regex objRegex = new Regex("&#(?<UnicodeCode>[\\d]{5});", RegexOptions.IgnoreCase);
            Match objMatch = objRegex.Match(unicode);
            StringBuilder sb = new StringBuilder(unicode);

            while (objMatch.Success)
            {
                string code = Convert.ToString(Convert.ToInt32(objMatch.Result("${UnicodeCode}")), 16);
                byte[] array = new byte[2];

                array[0] = (byte)Convert.ToInt32(code.Substring(2), 16);
                array[1] = (byte)Convert.ToInt32(code.Substring(0, 2), 16);

                sb.Replace(objMatch.Value, Encoding.Unicode.GetString(array));
                objMatch = objMatch.NextMatch();
            }
            return sb.ToString();
        }
    }
}
