//-------------------------------------------------
//版本信息:版权所有(C) 2014,COOLWI.COM
//变更历史:
//    姓名            日期                    说明
//-------------------------------------------------
//   王军锋          2014/11/10 18:13:00      创建
//-------------------------------------------------
using System;
using System.Web.Routing;
using CWI.MCP.Common;
using System.Collections.Generic;

namespace  CWI.MCP.Models
{
    /// <summary>
    /// 分页器参数类
    /// </summary>
    [Serializable]
    public class PagerModel
    {
        private static int PAGE_SIZE = ConfigUtil.PageSize;
        private const int PAGE_INDEX = 1;

        /// <summary>
        ///  翻页
        /// </summary>
        private PagerModel()
        {
        }

        /// <summary>
        /// 创建分页对象
        /// </summary>
        /// <param name="action">action</param>
        /// <param name="controller">controller</param>
        /// <param name="values">生成Url参数</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="recordCount">count</param>
        /// <param name="pageIndexKey">pageindex键(不区分大小写)</param>
        /// <returns>PagerModel</returns>
        public static PagerModel New(string action, string controller, int pageIndex,int recordCount, Dictionary<string,string> parameters = null)
        {
            PagerModel pager = new PagerModel();
            pager.PageSize = ConfigUtil.PageSize;
            pager.RecordCount = recordCount;
            pager.Action = CommonUtil.GetTitleCaseString(action);
            pager.Controller = controller;
            pager.PageIndexKey = "PageIndex";
            pager.PageIndex = pageIndex;

            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add(pager.PageIndexKey, pageIndex);
            if (parameters!=null)
            {
                foreach (string key in parameters.Keys)
                {
                    rvd.Add(key, parameters[key]);
                }
            }
            pager.Values = rvd;
            return pager;
        }

        public int PageIndex { get; set; }

        /// <summary>
        /// 分页Action
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 分页Controller
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Page Index 键(如：pageIndex,PageIndex)
        /// </summary>
        public string PageIndexKey { get; set; }

        public RouteValueDictionary Values
        {
            get; set;
        }
        
        /// <summary>
        /// 总记录数
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                if (RecordCount <= 0 || PageSize <= 0)
                {
                    return 1;
                }

                return (int)Math.Ceiling(RecordCount / (decimal)PageSize);
            }
        }
    }
}