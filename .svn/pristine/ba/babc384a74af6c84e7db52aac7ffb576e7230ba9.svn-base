using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evt.Framework.Mvc;
using System.ComponentModel.DataAnnotations;
using CWI.MCP.Common;
using System.Web.Routing;
using CWI.MCP.Common.Extensions.MVC;

namespace  CWI.MCP.Models
{
    public class SearchViewModel : ViewModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SearchViewModel()
        {
            this.PageIndex = RouteParameter.Index;
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWords { get; set; }

        /// <summary>
        /// 当前页索引
        /// </summary>
        [RegularExpression(RegexConsts.INT_FOR_GREAT_ZERO, ErrorMessage = "页码必须为大于零的正整数！")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 获取路由参数
        /// </summary>
        /// <returns>路由参数字典</returns>
        public RouteValueDictionary GetValues()
        {
            RouteValueDictionary values = new RouteValueDictionary();
            values.Add("PageIndex", this.PageIndex);
            if (!string.IsNullOrWhiteSpace(this.KeyWords))
            {
                values.Add("KeyWords", this.KeyWords);
            }
            return values;
        }
    }
}
