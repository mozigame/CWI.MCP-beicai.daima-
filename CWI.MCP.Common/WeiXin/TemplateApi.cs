using System;
using System.Web;
using System.Text;
using System.IO;
using System.Linq;
using System.Data;
using System.Security.Cryptography;
using Evt.Framework.Common;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CWI.MCP.Common.Extensions;

namespace CWI.MCP.Common
{
    public class TemplateApi
    {
        public static string SendTemplateMessage<T>(string accessToken, string openId, string topcolor, string url, T data) where T : TemplateMessageModel
        {
            string urlFormat = string.Format(WeChatConsts.WECHAT_SEND_MSG, accessToken);
            var msgData = new TempleteModel()
            {
                touser = openId,
                template_id = data.TemplateId,
                topcolor = topcolor,
                url = url,
                data = data
            };
            return NetUtil.WechatSendPostRequest(urlFormat, JsonUtil.Serialize(msgData));
        }
    }
}
