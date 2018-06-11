using System;
using System.Collections.Generic;
using CWI.MCP.Common;
using CWI.MCP.Common.ORM;
using CWI.MCP.Models;
using Evt.Framework.Common;
using System.Transactions;

namespace CWI.MCP.Services
{
    /// <summary>
    /// 微信相关服务类
    /// </summary>
    public class WeChatService : BaseService
    {
        /// <summary>
        /// 绑定微信
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool BindWeiXinUser(McpWeiXinUserInfo model)
        {
            model.CreatedOn = CommonUtil.GetDBDateTime();
            model.ModifiedOn = model.CreatedOn;
            return GetRepository<McpWeiXinUserInfo>().Create(model) > 0;
        }

        /// <summary>
        /// 更新微信绑定用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateWeiXinUser(McpWeiXinUserInfo model)
        {
            model.ModifiedOn = CommonUtil.GetDBDateTime();
            return GetRepository<McpWeiXinUserInfo>().Update(model) > 0;
        }

        /// <summary>
        /// 解绑微信
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UnbindWeiXinUser(McpWeiXinUserInfo model)
        {
            model.StatusCode = StatusCodeType.Disabled.GetHashCode();
            model.ModifiedOn = CommonUtil.GetDBDateTime();
            return GetRepository<McpWeiXinUserInfo>().Update(model) > 0;
        }

        /// <summary>
        /// 校验是否存在微信用户
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="wxType"></param>
        /// <returns></returns>
        public bool IsExistsWeiXinUser(string openId, int wxType)
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("open_id", openId));
            cc.Add(new Condition("wx_type", wxType));
            return GetRepository<McpWeiXinUserInfo>().IsExists(cc);
        }

        /// <summary>
        /// 获取用户微信信息
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="wxType"></param>
        /// <returns></returns>
        public McpWeiXinUserInfo GetWeiXinUser(string openId, int wxType)
        {
            ConditionCollection cc = new ConditionCollection();
            cc.Add(new Condition("open_id", openId));
            cc.Add(new Condition("wx_type", wxType));
            return GetRepository<McpWeiXinUserInfo>().GetModel(cc);
        }

        /// <summary>
        ///  获取用户openID
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetWechatOpenId(string code)
        {
            string openId = String.Empty;
            string wechatAppId = ConfigUtil.WechatAppIdForIot;
            string wechatAppSecret = ConfigUtil.WechatAppSecretForIot;

            string url = String.Format(WeChatConsts.WECHAT_GET_OPENID, wechatAppId, wechatAppSecret, code);
            string resultText = NetUtil.WechatSendPostRequest(url, string.Empty);
            LogUtil.Debug(string.Format("GetWechatOpenId: code: {0}, res: {1}", code, resultText));
            Dictionary<string, object> dicWechat = JsonUtil.Deserialize<Dictionary<string, object>>(resultText);
            if (dicWechat.ContainsKey("openid"))
            {
                openId = TryConvertUtil.ToString(dicWechat["openid"], string.Empty);
            }
            return openId;
        }

        /// <summary>
        /// 更新微信用户访问令牌
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="token"></param>
        /// <param name="expiresIn"></param>
        private static void UpdateWeiXinToken(string appId, string token, int expiresIn)
        {
            string sql = "UPDATE mcp_weixin_token SET  expires_time=DATE_ADD(NOW(),INTERVAL @expiresIn SECOND),update_time=now(),access_token=@access_token WHERE app_id=@app_id";
            ParameterCollection pc = new ParameterCollection();
            pc.Add("@app_id", appId);
            pc.Add("@access_token", token);
            pc.Add("@expiresIn", expiresIn);
           var cnt = DbUtil.DataManager.Current.IData.ExecuteNonQuery(sql, pc);
        }

        /// <summary>
        /// Client 0, Merchant 1.
        /// </summary>
        /// <param name="wxType"></param>
        /// <returns></returns>
        public static string GetWeiXinToken(int wxType)
        {
            string wechatAppId = ConfigUtil.WechatAppIdForIot;
            string wechatAppSecret = ConfigUtil.WechatAppSecretForIot;
            string sql = "SELECT access_token FROM mcp_weixin_token WHERE app_id=@app_id AND expires_time>NOW()";
            ParameterCollection pc = new ParameterCollection();
            pc.Add("@app_id", wechatAppId);
            string token = ConvertUtil.ToString(DbUtil.DataManager.Current.IData.ExecuteScalar(sql, pc), "");

            if (string.IsNullOrWhiteSpace(token) || CheckTokenIsExpired(token))
            {
                string tokenUrl = String.Format(WeChatConsts.WECHAT_TOKEN, wechatAppId, wechatAppSecret);
                string tokenResult = NetUtil.ResponseByGet(tokenUrl, null);
                Dictionary<string, object> dicWechat = JsonUtil.Deserialize<Dictionary<string, object>>(tokenResult);
                if (dicWechat != null && dicWechat.ContainsKey("access_token"))
                {
                    token = dicWechat["access_token"].ToString();
                    UpdateWeiXinToken(wechatAppId, token, ConvertUtil.ToInt(dicWechat["expires_in"], 0));
                }
            }

            return token;
        }

        /// <summary>
        /// 校验accessToken是否过期
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static bool CheckTokenIsExpired(string accessToken)
        {
            string jsonStr = NetUtil.ResponseByGet(string.Format(WeChatConsts.WECHAT_TOKEN_ISEXPIRED, accessToken), null);
            Dictionary<string, object> dicWechat = JsonUtil.Deserialize<Dictionary<string, object>>(jsonStr);
            if (dicWechat != null && dicWechat.ContainsKey("errcode"))
            {
                return dicWechat["errcode"].ToString().Length > 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取微信Token
        /// </summary>
        /// <returns></returns>
        public string GetWxJsApiTicket(int wxType)
        {
            string wechatAppId = ConfigUtil.WechatAppIdForIot;
            string sql = "SELECT jsapi_ticket FROM mcp_weixin_jsapiticket WHERE app_id=@app_id AND expires_time>NOW()";
            ParameterCollection pc = new ParameterCollection();
            pc.Add("@app_id", wechatAppId);
            string jsapiTicket = ConvertUtil.ToString(DbUtil.DataManager.Current.IData.ExecuteScalar(sql, pc), "");
            if (string.IsNullOrWhiteSpace(jsapiTicket))
            {
                string ticketUrl = string.Format(WeChatConsts.WECHAT_JSSDK_TICKET, GetWeiXinToken(wxType));
                LogUtil.Debug(string.Format("ticketUrl：{0}", ticketUrl));
                string ticketResult = NetUtil.WeChatSendGetRequest(ticketUrl);
                LogUtil.Debug(string.Format("ticketResult：{0}", ticketResult));
                Dictionary<string, object> dicWechat = JsonUtil.Deserialize<Dictionary<string, object>>(ticketResult);
                if (dicWechat != null && dicWechat.ContainsKey("ticket"))
                {
                    jsapiTicket = dicWechat["ticket"].ToString();
                    UpdateWeiXinJsApiTicket(wechatAppId, jsapiTicket, ConvertUtil.ToInt(dicWechat["expires_in"], 0));
                }
            }
            return jsapiTicket;
        }

        #region JS-SDK相关

        /// <summary>
        /// 更新JS-SDK，API-Ticket
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="jsApiTicket"></param>
        /// <param name="expiresIn"></param>
        private void UpdateWeiXinJsApiTicket(string appId, string jsApiTicket, int expiresIn)
        {
            string sql = "UPDATE mcp_weixin_jsapiticket SET  expires_time=DATE_ADD(NOW(),INTERVAL @expiresIn SECOND),update_time=now(),jsapi_ticket=@jsapi_ticket WHERE app_id=@app_id";
            ParameterCollection pc = new ParameterCollection();
            pc.Add("@app_id", appId);
            pc.Add("@jsapi_ticket", jsApiTicket);
            pc.Add("@expiresIn", expiresIn);
            DbUtil.DataManager.Current.IData.ExecuteNonQuery(sql, pc);
        }

        /// <summary>
        /// JS-SDK使用权限签名算法
        /// </summary>
        /// <param name="jsapiTicket">The jsapi_ticket.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static WxJsApiConfigViewModel GetSign(int wxType, string url, string jsapiTicket)
        {
            //构建配置参数
            var wxConfigModel = new WxJsApiConfigViewModel();
            wxConfigModel.appId = ConfigUtil.WechatAppIdForIot;
            wxConfigModel.timestamp = SysDateTime.SecondTicks_1970;
            wxConfigModel.nonceStr = CommonUtil.GetGuidNoSeparator();
            string str = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsapiTicket, wxConfigModel.nonceStr, wxConfigModel.timestamp, url);
            wxConfigModel.signature = SecurityUtil.SHA1Encrypt(str);//SHA1加密
            return wxConfigModel;
        }

        /// <summary>
        /// 获取配置接口参数
        /// </summary>
        /// <param name="apiList">接口列表</param>
        /// <returns></returns>
        public WxJsApiConfigViewModel GetJsApiParamsModel(int wxType, string url)
        {
            string jsapiTicket = GetWxJsApiTicket(wxType);
            return WeChatService.GetSign(wxType, url, jsapiTicket);
        }

        #endregion

        #region 模版消息处理【待废弃】

        /// <summary>
        /// 上传提醒消息
        /// </summary>
        /// <param name="subject">文件主题</param>
        /// <returns>消息实体</returns>
        public UpSuccTemplateModel GetUploadMsgTemplateModel(string subject)
        {
            var temp = new UpSuccTemplateModel();
            temp.first = new TemplateDataItem("您好，您的文件已上传成功。");
            temp.keyword1 = new TemplateDataItem(DateTime.Now.ToString("yyyy年MM月dd日 HH:mm")); //上传时间
            temp.keyword2 = new TemplateDataItem(subject); //文件主题
            temp.remark = new TemplateDataItem("感谢您的使用。");
            return temp;
        }

        /// <summary>
        /// 打印提醒消息
        /// </summary>
        /// <param name="orderNo">订单编号</param>
        /// <returns>消息实体</returns>
        public PrintSuccTemplateModel GetPrintMsgTemplateModel(string orderNo)
        {
            var temp = new PrintSuccTemplateModel();
            temp.first = new TemplateDataItem("您好，您已打印成功。");
            temp.keyword1 = new TemplateDataItem(orderNo); //订单号
            temp.keyword2 = new TemplateDataItem(DateTime.Now.ToString("yyyy年MM月dd日 HH:mm")); //时间
            temp.remark = new TemplateDataItem("感谢您的使用。");
            return temp;
        }

        /// <summary>
        /// 支付提醒消息
        /// </summary>
        /// <param name="orderNo">订单编号</param>
        /// <param name="amount">订单金额</param>
        /// <returns>消息实体</returns>
        public PaySuccTemplateModel GetPayMsgTemplateModel(string orderNo, string amount, int payType)
        {
            var temp = new PaySuccTemplateModel();
            temp.first = new TemplateDataItem("您好，您已支付成功。");
            temp.keyword1 = new TemplateDataItem(orderNo); //订单号
            temp.keyword2 = new TemplateDataItem(amount); //金额
            temp.keyword3 = new TemplateDataItem(((PaymentType)payType).GetRemark()); //支付方式
            temp.remark = new TemplateDataItem("感谢您的使用。");
            return temp;
        }

        /// <summary>
        /// 退款提醒消息
        /// </summary>
        /// <param name="orderNo">退款单编号</param>
        /// <param name="amount">退款金额</param>
        /// <returns>消息实体</returns>
        public RefundSuccTemplateModel GetRefundMsgTemplateModel(string orderNo, string amount)
        {
            var temp = new RefundSuccTemplateModel();
            temp.first = new TemplateDataItem("尊敬的用户，您的退款正在处理中。");
            temp.keyword1 = new TemplateDataItem(orderNo); //退单编号
            temp.keyword2 = new TemplateDataItem(amount); //退款金额
            temp.remark = new TemplateDataItem("请您耐心等待，我们将在七个工作日内原路退还到您的账户，感谢您的使用。");
            return temp;
        }

        #endregion
    }
}
