//版权信息：版权所有(C) 2015，COOLWI.COM
//变更历史：
//    姓名         日期          说明
//--------------------------------------------------------
//   王军锋     2012/03/31       创建

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 前台会话对象
    /// </summary>
    [Serializable]
    public class SessionData
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SessionData()
        {
            AdminIsSupper = false;
        }

        /// <summary>
        /// 用户标识【店铺ID、平台ID、管理员ID】
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 账号【店铺账号、客户账号、管理员账号】
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机【店铺】
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱【客户】
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 用户类型【店铺、客户、管理员】
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// 【后台系统】管理员姓名
        /// </summary>
        public string AdminName { get; set; }

        /// <summary>
        /// 【后台系统】是否为超级管理员
        /// </summary>
        public bool AdminIsSupper { get; set; }

        /// <summary>
        /// 用户角色列表
        /// </summary>
        public List<string> UserRoleList { get; set; }
    }
}
