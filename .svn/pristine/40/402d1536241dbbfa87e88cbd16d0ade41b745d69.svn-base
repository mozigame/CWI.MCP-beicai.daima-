//-------------------------------------------------
//版本信息:版权所有(C) 2014,PAIDUI.CN
//变更历史:
//    姓名            日期                  说明
//-------------------------------------------------
//   王军锋     2014/12/11 13:46:11           创建
//-------------------------------------------------
using System.Collections.Generic;
using CWI.MCP.Common;

namespace CWI.MCP.Models.WebApi
{
    /// <summary>
    /// session管理
    /// </summary>
    public class SessionManager
    {
        /// <summary>
        /// 保存Session的Dictionary
        /// </summary>
        private static Dictionary<string, Session> dcSession = new Dictionary<string, Session>();

        /// <summary>
        /// 获取所有Session
        /// </summary>
        /// <param name="sessionID">sessionID</param>
        /// <returns>Session</returns>
        public static Dictionary<string, Session> GetSessions()
        {
            return dcSession;
        }

        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="sessionID">sessionID</param>
        /// <returns>Session</returns>
        public static Session GetSession(string sessionID)
        {
            if (dcSession.ContainsKey(sessionID))
            {
                return dcSession[sessionID];
            }

            return null;
        }

        /// <summary>
        /// 通过 Session value 查找Session
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Session</returns>
        public static Session GetSessionByValue(string value)
        {
            foreach (Session session in dcSession.Values)
            {
                if (session.ContainsValue(value))
                {
                    return session;
                }
            }
            return null;
        }

        /// <summary>
        /// 创建Session
        /// </summary>
        /// <returns>Session</returns>
        public static Session CreateSession()
        {
            Session session = new Session();

            session.SessionID = CommonUtil.GetGuidNoSeparator();

            dcSession.Add(session.SessionID, session);

            return session;
        }

        /// <summary>
        /// 移除Session
        /// </summary>
        /// <param name="sessionID">sessionID</param>
        public static void RemoveSession(string sessionID)
        {
            dcSession.Remove(sessionID);
        }
    }
}
