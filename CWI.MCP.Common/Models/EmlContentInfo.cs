//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/18        创建
//---------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace  CWI.MCP.Common
{
    [Serializable]
    [XmlRoot(ElementName = "EmlContentCollection")]
    public class EmlContentCollection
    {
        [XmlElement("EmlDescription")]
        public List<EmlDescription> EmlDescriptions
        {
            get;
            set;
        }

    }

    [Serializable]
    public class EmlDescription
    {
        [XmlElement("EmlKey")]
        public string EmlKey
        {
            get;
            set;
        }

        [XmlElement("EmlFormat")]
        public string EmlFormat
        {
            get;
            set;
        }

        [XmlElement("EmlStatus")]
        public string EmlStatus
        {
            get;
            set;
        }

        [XmlElement("EmlExamples")]
        public string EmlExamples
        {
            get;
            set;
        }

    }
}
