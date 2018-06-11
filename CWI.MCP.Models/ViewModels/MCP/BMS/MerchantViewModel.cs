using System.ComponentModel.DataAnnotations;
using Evt.Framework.Mvc;
using System;

namespace CWI.MCP.Models
{
    public class MerchantViewModel : ViewModel
    {
        /// <summary>
        /// 账户
        /// </summary>
        public string Account
        {
            get; set;
        }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile
        {
            get; set;
        }
        /// <summary>
        /// 余额
        /// </summary>
        public string Balance
        {
            get; set;
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn
        {
            get; set;
        }

    }
}
