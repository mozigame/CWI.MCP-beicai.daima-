using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWI.MCP.Common;

namespace  CWI.MCP.Models
{
    /// <summary>
    /// 登陆信息
    /// </summary>
    public class LoginInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 用户手机
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 用户登陆令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public int Status { get; set; }

        //public static OptShopInfo GetShopInfo(LoginInfo login)
        //{
        //    return new OptShopInfo
        //    {
        //        ShopId = login.Id,
        //        ShopAccount = login.Account,
        //        Mobile = login.Mobile,
        //        AccessToken = login.AccessToken,
        //        StatusCode = login.Status,
        //    };
        //}

        //public static OptCustomerInfo GetCustomerInfo(LoginInfo login)
        //{
        //    return new OptCustomerInfo
        //    {
        //        CustomerId = TryConvertUtil.ToBigInt(login.Id, 0),
        //        CustomerAccount = login.Account,
        //        CustomerEmail = login.Email,
        //        AccessToken = login.AccessToken,
        //        StatusCode = login.Status,
        //    };
        //}

        //public static LoginInfo GetInfo(object login)
        //{
        //    LoginInfo loginInfo = null;
        //    if (login.GetType().ToString().IndexOf("OptCustomerInfo") > 0)
        //    {
        //        var obj = login as OptCustomerInfo;
        //        loginInfo = new LoginInfo
        //        {
        //            Id = obj.CustomerId.ToString(),
        //            Account = obj.CustomerEmail,
        //            Email = obj.CustomerEmail,
        //            Status = obj.StatusCode,
        //            AccessToken = obj.AccessToken
        //        };
        //    }
        //    else if (login.GetType().ToString().IndexOf("OptShopInfo") > 0)
        //    {
        //        var obj = login as OptShopInfo;
        //        loginInfo = new LoginInfo
        //        {
        //            Id = obj.ShopId.ToString(),
        //            Account = obj.ShopAccount,
        //            Mobile = obj.Mobile,
        //            Status = obj.StatusCode,
        //            AccessToken = obj.AccessToken
        //        };
        //    }
        //    return loginInfo;
        //}
    }
}
