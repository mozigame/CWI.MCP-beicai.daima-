//---------------------------------------------
// 版权信息：版权所有(C) 2014，PAIDUI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋       2014/11/14         创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 异常处理
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// 异常信息集合
        /// </summary>
        private static Dictionary<string, string> _errorMessages = new Dictionary<string, string>();

        /// <summary>
        /// 错误提醒XML文件名称
        /// </summary>
        private const string ERROR_FILE_NAME = "ErrorMessages.xml";

        /// <summary>
        /// 错误提醒XML文件夹名称
        /// </summary>
        private const string ERROR_FILE_DIR = "DATA";

        /// <summary>
        /// 构造函数
        /// </summary>
        static ErrorHandler()
        {
            //加载资源
            LoadErrorMessage();
        }

        /// <summary>
        /// 载错误资源
        /// </summary>
        private static void LoadErrorMessage()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ERROR_FILE_DIR, ERROR_FILE_NAME);
            if (!File.Exists(path))
            {
                return;
            }
            try
            {
                XmlDocument xmlDoc = XmlUtil.LoadXmlFile(path);
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
        public static string GetErrorMessage(string code)
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
        public static string GetErrorMessage(BusinessException ex)
        {
            return GetErrorMessage(ex.Message);
        }
    }
}
