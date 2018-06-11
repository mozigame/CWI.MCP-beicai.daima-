//---------------------------------------------
// 版权信息：版权所有(C) 2014，PAIDUI.CN
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋     2014/3/18 13:26:13         创建
//---------------------------------------------

using System;
using System.Net;

using CWI.MCP.Common;
using CWI.MCP.Models;

namespace  CWI.MCP.Services.APICommon
{
    /// <summary>
    /// 当前服务上下文实现类
    /// </summary>
    public class SysServericeContext : IServiceContextUser
    {
        /// <summary>
        /// 当前会话ID私有变量
        /// </summary>
        private string SessionId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sessionID">sessionID</param>
        public SysServericeContext(string sessionID)
        {
            this.SessionId = sessionID;
        }

        /// <summary>
        /// 获取或设置当前用户信息
        /// </summary>
        public LoginInfo User
        {
            get
            {
                LoginInfo userInfo = new LoginInfo();
                Session session = SessionManager.GetSession(SessionId);
                if (session != null)
                {
                    if (session.ContainsKey(Consts.LOGIN_USER_SESSION_KEY))
                    {
                        var loginInfoSession = session[Consts.LOGIN_USER_SESSION_KEY];
                        if (loginInfoSession != null)
                        {
                            userInfo = session[Consts.LOGIN_USER_SESSION_KEY] as LoginInfo;
                        }
                    }
                }
                return userInfo;
            }
            set
            {
                if (value == null)
                {
                    SessionManager.RemoveSession(SessionId);
                }
                else
                {
                    Session session = SessionManager.GetSession(SessionId);
                    if (session != null)
                    {
                        session[Consts.LOGIN_USER_SESSION_KEY] = value;

                    }
                }
            }
        }
    }

    /// <summary>
    /// 默认实现的IRequestTerminal,通常用作PC终端
    /// </summary>
    public class PrdRequestTerminal : IRequestTerminal
    {
        /// <summary>
        /// 当前会话ID私有变量
        /// </summary>
        private string SessionId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sessionID">sessionID</param>
        public PrdRequestTerminal(string sessionID)
        {
            this.SessionId = sessionID;
        }

        /// <summary>
        /// 获取客户端信息
        /// </summary>
        public RequestClientInfoModel ClientInfo
        {
            get
            {
                RequestClientInfoModel client = new RequestClientInfoModel();
                Session session = SessionManager.GetSession(SessionId);
                if (session != null)
                {
                    if (session.ContainsKey(Consts.CLIENT_INFO_SESSION_KEY))
                    {
                        var clientSeesion = session[Consts.CLIENT_INFO_SESSION_KEY];
                        if (clientSeesion != null)
                        {
                            client = session[Consts.CLIENT_INFO_SESSION_KEY] as RequestClientInfoModel;
                        }
                    }
                }
                return client;
            }
        }

        #region 基本属性

        /// <summary>
        /// 应用标识
        /// </summary>
        public string AppSign
        {
            get
            {
                string appSign = string.Empty;
                if (ClientInfo != null)
                {
                    appSign = ClientInfo.AppSign;
                }
                return appSign;
            }
        }

        /// <summary>
        /// 请求IP
        /// </summary>
        public string IP
        {
            get
            {
                string ip = string.Empty;
                if (ClientInfo != null)
                {
                    ip = ClientInfo.ClientIP;
                }

                string[] temps = ip.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (temps.Length != 4)
                {
                    ip = CommonUtil.GetRemoteIPAddress();
                }
                return ip;
            }
        }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name
        {
            get
            {
                string name = string.Empty;
                if (string.IsNullOrWhiteSpace(name))
                {
                    try
                    {
                        name = Dns.GetHostName();
                    }
                    catch
                    {
                    }
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    name = this.IP;
                }
                return name;
            }
        }

        /// <summary>
        /// 设备编号,此处获取设备Mac地址
        /// </summary>
        public string Code
        {
            get
            {
                string macCode = string.Empty;
                if (ClientInfo != null)
                {
                    macCode = ClientInfo.DeviceId;
                }
                if (string.IsNullOrWhiteSpace(macCode))
                {
                    macCode = this.IP;
                }
                return macCode;
            }
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType
        {
            get
            {
                DeviceType deviceType = DeviceType.Other;
                if (ClientInfo != null)
                {
                    return (DeviceType)TryConvertUtil.ToInt(ClientInfo.DeviceType, 0);
                }

                return deviceType;
            }
        }

        /// <summary>
        /// 客户端Token
        /// </summary>
        public string ClientToken
        {
            get
            {
                string clientToken = string.Empty;
                if (ClientInfo != null)
                {
                    return ClientInfo.ClientToken;
                }

                return clientToken;
            }
        }

        #endregion
    }
}
