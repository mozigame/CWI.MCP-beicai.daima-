//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/18        创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 内容模版配置助手
    /// </summary>
    public class TemplateConfigUtil
    {
        static string filepath = "";

        /// <summary>
        /// 邮件内容
        /// </summary>
        public static EmlContentCollection EmlContentConfig
        {
            get;
            private set;
        }

        /// <summary>
        /// 邮件内容
        /// </summary>
        public static Dictionary<string, string> EmlDic
        {
            get;
            private set;
        }

        static TemplateConfigUtil()
        {
            var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace(@"file:\","");

            if (dir != null) filepath = Path.Combine(dir, @"Resources\MsgContent.xml");

            EmlDic = new Dictionary<string, string>();

            Load();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private static void Load()
        {
            if (File.Exists(filepath))
            {
                try
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(EmlContentCollection), new Type[] { });
                        EmlContentConfig = ((EmlContentCollection)ser.Deserialize(fs));
                    }

                    if (EmlContentConfig != null)

                        foreach (var item in EmlContentConfig.EmlDescriptions)
                        {
                            EmlDic.Add(item.EmlKey, item.EmlFormat);
                        }
                }
                catch (Exception e)
                {
                    LogUtil.Error("加载邮件配置文件错误." + e.ToString());
                }
            }
            else
            {
                LogUtil.Error("加载邮件配置文件错误");
            }
        }
    }
}
