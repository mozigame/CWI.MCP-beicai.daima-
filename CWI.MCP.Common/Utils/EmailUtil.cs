//---------------------------------------------
// 版权信息：版权所有(C) 2015，Coolwi
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2013/3/20        创建
//      王军锋        2013/7/12        PWZ迁移
//---------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWI.MCP.Common;
using System.Net.Mail;
using System.Net;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 邮件发送类【待整合】
    /// </summary>
    public class EmailUtil
    {
        /// <summary>
        /// 初始化邮件发送参数
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="toEmails">接收邮箱　可设置多个逗号隔开 例："aa@bb.com,cc@bb.com"</param>
        /// <param name="fromEmail">发送邮箱</param>
        /// <param name="displayName">显示发送者名称</param>
        /// <returns>返回邮件发送成功或失败信息</returns>
        public static bool SendEmail(string title, string body, string toEmails, string fromEmail, string displayName, bool isBodyHtml = false)
        {
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(fromEmail, displayName, Encoding.UTF8),
                Subject = title,
                Body = body,
                IsBodyHtml = isBodyHtml,
                Priority = MailPriority.High,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8,
            };
            var aToEmails = toEmails.Split(',');
            if (aToEmails.Count() > 0)
            {
                foreach (var toEmail in aToEmails)
                {
                    mailMessage.To.Add(new MailAddress(toEmail));
                }
            }
            else
            {
                mailMessage.To.Add(new MailAddress(toEmails));
            }

            var host = ConfigUtil.SenderServerHost;
            var port = ConfigUtil.SenderServerHostPort;
            var userName = ConfigUtil.SenderEmailAccount;
            var password = ConfigUtil.SenderEmailAccountPwd;
            var timeOut = ConfigUtil.SenderEmailTimeOut;
            return SendEmail(host, port, userName, password, timeOut, mailMessage);
        }

        /// <summary>
        /// Smtp邮件发送
        /// </summary>
        /// <param name="host">邮件服务器IP或地址</param>
        /// <param name="port">邮件服务器端口，pop3端口:110, smtp端口是:25 </param>
        /// <param name="userName">邮箱用户名</param>
        /// <param name="password">邮箱密码</param>
        /// <param name="timeOut">邮件发送超时时间</param>
        /// <param name="mailMessage">邮件对象</param>
        /// <returns>返回邮件发送成功或失败信息</returns>
        public static bool SendEmail(string host, int port, string userName, string password, int timeOut, MailMessage mailMessage)
        {
            bool bl = false;
            try
            {
                var client = new SmtpClient(host)
                {
                    Port = port,
                    Timeout = timeOut,
                    Credentials = new NetworkCredential(userName, password),
                    EnableSsl = ConfigUtil.SenderServerEnableSSL
                };
                client.Send(mailMessage);
                bl = true;
            }
            catch (Exception ex)
            {
                LogUtil.Info("FromEmail" + mailMessage.From.Address + "  ToEmail" + String.Join(",", mailMessage.To) + "   " + ex.ToString());
            }
            return bl;
        }
    }
}
