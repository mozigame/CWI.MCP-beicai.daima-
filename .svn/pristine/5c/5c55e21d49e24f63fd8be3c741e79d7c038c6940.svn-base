//版权信息：版权所有(C) 2014，PAIDUI.COM
//变更历史：
//     姓名         日期          说明
//--------------------------------------------------------
//   王军锋     2014/01/16       创建

using System;
using System.Net;
using CWI.MCP.Common;

namespace CWI.MCP.Models.WebApi
{
    #region ServiceContextUser

    /// <summary>
    /// 一个接口,用于约束如何设置获取服务会话用户信息
    /// </summary>
    public interface IServiceContextUser
    {
        /// <summary>
        /// 获取或设置用户信息
        /// </summary>
        LoginInfo CurrUser { get; set; }
    }

    /// <summary>
    /// 默认实现的IServiceContextUser接口
    /// </summary>
    public class DefaultServiceContextUser : IServiceContextUser
    {
        private LoginInfo _currUser;

        /// <summary>
        /// 获取或设置用户信息
        /// </summary>
        public LoginInfo CurrUser
        {
            get
            {
                if (_currUser == null)
                {
                    throw new Exception("未初始化ServiceContext的Current");
                }
                return _currUser;
            }
            set
            {
                _currUser = value;
            }
        }
    }
    #endregion

    #region RequestTerminal

    /// <summary>
    /// 一个接口,用于获取当前请求的设备信息
    /// </summary>
    public interface IRequestTerminal
    {
        /// <summary>
        /// 应用标识
        /// </summary>
        string AppSign { get; }

        /// <summary>
        /// 设备IP
        /// </summary>
        string IP { get; }

        /// <summary>
        /// 设备名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 设备编号
        /// </summary>
        string Code { get; }

        /// <summary>
        /// 设备类型
        /// </summary>
        DeviceType DeviceType { get; }

        /// <summary>
        /// 客户端Token
        /// </summary>
        string ClientToken { get; }
    }

    /// <summary>
    /// 默认实现的IRequestTerminal,通常用作PC终端
    /// </summary>
    public class DefaultRequestTerminal : IRequestTerminal
    {
        private string _appSign = string.Empty;
        private string _ip = string.Empty;
        private string _name = string.Empty;
        private string _code = string.Empty;
        private string _clientToken = string.Empty;

        /// <summary>
        /// 应用标识
        /// </summary>
        public string AppSign
        {
            get
            {
                return _appSign;
            }
        }

        /// <summary>
        /// 请求IP
        /// </summary>
        public string IP
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_ip))
                {
                    return _ip;
                }
                else
                {
                    return CommonUtil.GetRemoteIPAddress();
                }
            }
        }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_name))
                {
                    return _name;
                }
                var str = string.Empty;
                try
                {
                    str = Dns.GetHostName();
                    _name = str;
                }
                catch { }
                return str;
            }
        }

        /// <summary>
        /// 设备编号,此处获取设备Mac地址
        /// </summary>
        public string Code
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_code))
                {
                    return _code;
                }
                var str = string.Empty;
                try
                {
                    str = CommonUtil.GetMacAddressByNetBios();
                    _code = str;
                }
                catch { }
                return str;
            }
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType
        {
            get { return DeviceType.Other; }
        }

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string ClientToken
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_clientToken))
                {
                    return _clientToken;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
    #endregion
}
