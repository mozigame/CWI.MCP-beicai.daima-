using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWI.MCP.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ApiDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// 支付描述
        /// </summary>
        /// <param name="desc">描述内容</param>
        public ApiDescriptionAttribute(string desc)
        {
            this.Description = desc;
        }

        public SortedDictionary<string, string> ConvertToSortedDictionary()
        {
            return null;
        }
    }
}
