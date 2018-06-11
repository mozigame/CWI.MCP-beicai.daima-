// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期          说明
// --------------------------------------------
//      陈钦       2013/07/10       创建
// --------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 页面SEO设置工厂
    /// </summary>
    public class PageSeoFactory
    {
        #region 私有字段定义

        /// <summary>
        /// 可空标签正则
        /// </summary>
        private static Regex regexNullable = new Regex(@"\(\?(?<value>((?!\?\)).(?!\(\?))*)\?\)", RegexOptions.Singleline);
        
        #endregion

        #region 公共字段定义

        /// <summary>
        /// SEO META信息-首页
        /// </summary>
        public const string SEO_META_INDEX = "INDEX";

        /// <summary>
        /// SEO META信息-公司简介
        /// </summary>
        public const string SEO_META_ABOUT = "ABOUT";

        /// <summary>
        /// SEO META信息-企业文化
        /// </summary>
        public const string SEO_META_ABOUT_CULTURE = "ABOUT-CULTURE";

        /// <summary>
        /// SEO META信息-排队之家
        /// </summary>
        public const string SEO_META_ABOUT_HOME = "ABOUT-HOME";

        /// <summary>
        /// SEO META信息-联系我们
        /// </summary>
        public const string SEO_META_ABOUT_CONTACT = "ABOUT-CONTACT";

        /// <summary>
        /// SEO META信息-人才招聘
        /// </summary>
        public const string SEO_META_JOBS = "JOBS";

        /// <summary>
        /// SEO META信息-新闻中心
        /// </summary>
        public const string SEO_META_NEWS = "NEWS";

        /// <summary>
        /// SEO META信息-新闻详情
        /// </summary>
        public const string SEO_META_NEWS_DETAIL = "NEWS-DETAIL";

        /// <summary>
        /// SEO META信息-专题
        /// </summary>
        public const string SEO_META_TOPIC = "TOPIC";

        /// <summary>
        /// SEO META信息-专题详情
        /// </summary>
        public const string SEO_META_TOPIC_DETAIL = "TOPIC-DETAIL";

        /// <summary>
        /// SEO META信息-产品-餐饮管理系统
        /// </summary>
        public const string SEO_META_PRODUCT_RMS = "PRODUCT-RMS";

        /// <summary>
        /// SEO META信息-产品-CRM系统
        /// </summary>
        public const string SEO_META_PRODUCT_CRM = "PRODUCT-CRM";

        /// <summary>
        /// SEO META信息-产品-点餐
        /// </summary>
        public const string SEO_META_PRODUCT_DC = "PRODUCT-DC";

        /// <summary>
        /// SEO META信息-产品-点菜宝
        /// </summary>
        public const string SEO_META_PRODUCT_DCB = "PRODUCT-DCB";

        /// <summary>
        /// SEO META信息-渠道合作
        /// </summary>
        public const string SEO_META_COOP_CHANNEL = "COOP-CHANNEL";

        /// <summary>
        /// SEO META信息-商务合作
        /// </summary>
        public const string SEO_META_COOP_BUSINESS = "COOP-BUSINESS";

        /// <summary>
        /// SEO META信息-常见问题
        /// </summary>
        public const string SEO_META_FAQ = "FAQ";

        /// <summary>
        /// SEO META信息-常见问题详情
        /// </summary>
        public const string SEO_META_FAQ_DETAIL = "FAQ-DETAIL";

        /// <summary>
        /// SEO META信息-服务中心
        /// </summary>
        public const string SEO_META_SERVICE = "SERVICE";

        /// <summary>
        /// SEO META信息-客户案例
        /// </summary>
        public const string SEO_META_CASE = "CASE";

        #endregion

        /// <summary>
        /// 获取标签值
        /// </summary>
        /// <param name="value">原值</param>
        /// <param name="tag">占位标签值</param>
        /// <returns>替换占位符后的值</returns>
        public static string GetMetaValue(string value, PageSeoTag tag)
        {
            if (string.IsNullOrEmpty(value)) return value;

            tag = tag ?? new PageSeoTag();
            bool isTitleEmpty = string.IsNullOrWhiteSpace(tag.Title);
            bool isSummaryEmpty = string.IsNullOrWhiteSpace(tag.Summary);
            bool isPageIndexEmpty = tag.PageIndex == null || tag.PageIndex < 2;
            
            //可空标签处理: (?...?)
            value = NullableHandle(value, isTitleEmpty, isSummaryEmpty, isPageIndexEmpty);

            //替换标签: {...}
            value = value.Replace(PageSeoTag.TitleTag, tag.Title);
            value = value.Replace(PageSeoTag.SummaryTag, tag.Summary);
            value = value.Replace(PageSeoTag.PageIndexTag, tag.PageIndex == null ? string.Empty : tag.PageIndex.ToString());            

            return value;
        }

        /// <summary>
        /// 可空标签处理
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="isTitleEmpty">标题是否为空</param>
        /// <param name="isSummaryEmpty">摘要是否为空</param>
        /// <param name="isPageIndexEmpty">页码是否为空</param>
        /// <returns>处理完后字符串</returns>
        private static string NullableHandle(string input, bool isTitleEmpty, bool isSummaryEmpty, bool isPageIndexEmpty)
        {
            MatchCollection matchNullables = regexNullable.Matches(input);

            if (matchNullables.Count == 0)
            {
                return input;
            }

            foreach (Match match in matchNullables)
            {
                Group valueGroup = match.Groups["value"];
                string groupValue = valueGroup.Value;
                string matchValue = match.Value;

                if (string.IsNullOrWhiteSpace(groupValue))
                {
                    input = input.Replace(match.Value, string.Empty);
                    continue;
                }

                if ((input.IndexOf(PageSeoTag.TitleTag) > -1 && isTitleEmpty) ||
                    (input.IndexOf(PageSeoTag.SummaryTag) > -1 && isSummaryEmpty) ||
                    (input.IndexOf(PageSeoTag.PageIndexTag) > -1 && isPageIndexEmpty))
                {
                    input = input.Replace(match.Value, string.Empty);
                    continue;
                }

                input = input.Replace(match.Value, groupValue);
            }

            return NullableHandle(input, isTitleEmpty, isSummaryEmpty, isPageIndexEmpty);
        }
    }

    /// <summary>
    /// SEO信息占位标签
    /// </summary>
    public class PageSeoTag
    {
        /// <summary>
        /// 标题标签名
        /// </summary>
        public const string TitleTag = "{标题}";

        /// <summary>
        /// 页码标签名
        /// </summary>
        public const string PageIndexTag = "{页码}";

        /// <summary>
        /// 摘要标签名
        /// </summary>
        public const string SummaryTag = "{摘要}";

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 分页页码
        /// </summary>
        public int? PageIndex { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }
    }
}
