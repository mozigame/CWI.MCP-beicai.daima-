//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名         日期                说明
// --------------------------------------------
//      王军锋     2013/9/3 19:49:45   创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Configuration;
using Evt.Framework.Common;
using System.Text.RegularExpressions;

namespace CWI.MCP.Common
{
    /// <summary>
    /// 错误码解析器
    /// </summary>
    public static class ErrorCodeUtil
    {
        /// <summary>
        /// 异常信息集合
        /// </summary>
        private static Dictionary<string, string> _errorMessages = new Dictionary<string, string>();

        /// <summary>
        /// 错误码文件相对路径
        /// </summary>
        public static string FilePath 
        {
            get
            { 
                string path = ConfigurationManager.AppSettings["errorCode:FilePath"];
                if (string.IsNullOrWhiteSpace(path))
                {
                    return "resources\\ErrorMessages.config";
                }

                return path.TrimStart('\\');
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        static ErrorCodeUtil()
        {
            //加载资源
            LoadErrorMessage();
        }

        /// <summary>
        /// 载错误资源
        /// </summary>
        private static void LoadErrorMessage()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilePath);
            if (!File.Exists(path))
            {
                return;
            }
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                var items = xmlDoc.SelectNodes("//Error");
                foreach (XmlNode node in items)
                {
                    try
                    {
                        string code = node.Attributes["code"].Value;
                        string value = node.InnerText;
                        if (!_errorMessages.ContainsKey(code))
                        {
                            _errorMessages.Add(code, value);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        /// <summary>
        /// 根据错误编号获取错误信息
        /// </summary>
        /// <param name="code">错误编号</param>
        /// <returns>返回错误提醒</returns>
        public static string GetMessage(string code)
        {
            if (!_errorMessages.ContainsKey(code))
            {
                return code;
            }

            return _errorMessages[code];
        }

        /// <summary>
        /// 根据异常获取错误信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>返回错误提醒</returns>
        public static string GetMessage(Exception ex)
        {
            string code = string.Empty;
            if (ex is MessageException)
            {
                MessageException messageEx = ex as MessageException;
                code = string.IsNullOrWhiteSpace(messageEx.Code) ? messageEx.Message : IsNumeric(messageEx.Code) ? messageEx.Code : messageEx.Message;
            }
            else
            {
                code = ex.Message;
            }

            return GetMessage(code);
        }

        /// <summary>
        /// 是否为数字
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsNumeric(string val)
        {
            Regex rex = new Regex(@"^\d+$");
            return rex.IsMatch(val);
        }
    }
}
