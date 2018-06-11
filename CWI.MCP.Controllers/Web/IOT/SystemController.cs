using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evt.Framework.Common;
using System.IO;
using CWI.MCP.Common;
using System.Xml;
using System.Web.Security;
using System.Web.Mvc;
using CWI.MCP.Services;
using CWI.MCP.Models;
using CWI.MCP.Common.Attributes;

namespace CWI.MCP.Controllers.IOT
{
    public class SystemController : WeiXinBaseController
    {
        /// <summary>
        /// 校验
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]
        public object Verify()
        {
            LogUtil.Debug(string.Format("request str:{0}", Request.QueryString));

            var echostr = Request["echostr"] as string;
            var retstr = echostr != null ? echostr.Trim() : string.Empty;

            bool isValid = CheckSignature(ConfigUtil.IotToken);
            return isValid ? retstr : string.Empty;
        }

        /// <summary>
        /// 消息页面
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]
        public object Message()
        {
            var code = Request["code"] as string;
            ViewBag.Code = code;

            return View();
        }

        /// <summary>
        /// 关于页面
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]
        public object About()
        {
            return View();
        }

        #region 基础

        /// <summary>
        /// 创建菜单
        /// </summary>
        [NonAuthorized]
        public string CreateMenu()
        {
            string backUrl = String.Format("http://{0}/{1}", Request.Url.Authority, "System/WechatCallBack");

            #region 构建自定义菜单

            List<Button> menuList = new List<Button>();

            //1.配置WiFi
            Button menu1 = new Button() { key = "config", name = "配置WiFi", type = "view", url = String.Format(WeChatConsts.WECHAT_AUTHORIZE, ConfigUtil.WechatAppIdForIot, backUrl, "/device/config"), sub_button = new List<Button>() };

            //2.关于
            Button menu2 = new Button() { key = "about", name = "  关 于  ", type = "view", url = String.Format(WeChatConsts.WECHAT_AUTHORIZE, ConfigUtil.WechatAppIdForIot, backUrl, "/system/about"), sub_button = new List<Button>() };

            menuList.Add(menu1);
            menuList.Add(menu2);

            #endregion

            string json = JsonUtil.Serialize("button", menuList);
            json = json.Replace("\\u0026", "&");
            string weiXinToken = WeChatService.GetWeiXinToken(EnumWeChatType.Client.GetHashCode());
            string tokenUrl = String.Format(WeChatConsts.WECHAT_MENU_ADD, weiXinToken);
            LogUtil.Debug(json);
            string result = NetUtil.WechatSendPostRequest(tokenUrl, json);
            return result;
        }

        /// <summary>
        /// 接收微信推送信息控制器接口
        /// </summary>
        /// <param name="merchantId"></param>
        [NonAuthorized]
        public void ReceiveMessage()
        {
            LogUtil.Debug(string.Format("原始消息：{0}", Request.QueryString.ToString()));

            // 判断是否是请求认证
            string echoStr = ConvertUtil.ToString(Request.QueryString["echostr"], String.Empty);
            if (CheckSignature(ConfigUtil.WechatToken))
            {
                if (!String.IsNullOrEmpty(echoStr))
                {
                    LogUtil.Debug(string.Format("自动回调：{0}",echoStr));
                    Response.Write(echoStr);
                    Response.End();
                }
                else
                {
                    Stream s = System.Web.HttpContext.Current.Request.InputStream;
                    byte[] b = new byte[s.Length];
                    s.Read(b, 0, (int)s.Length);
                    string postStr = Encoding.UTF8.GetString(b);
                    LogUtil.Debug(string.Format("接收信息：{0}",postStr));
                    if (!string.IsNullOrEmpty(postStr))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(postStr);
                        XmlNodeList list = doc.GetElementsByTagName("xml");
                        XmlNode xn = list[0];
                        WeChatMsgViewModel receiveMessageModel = new WeChatMsgViewModel();
                        receiveMessageModel.FromUserName = xn.SelectSingleNode("//FromUserName") != null ? xn.SelectSingleNode("//FromUserName").InnerText : String.Empty;
                        receiveMessageModel.ToUserName = xn.SelectSingleNode("//ToUserName") != null ? xn.SelectSingleNode("//ToUserName").InnerText : String.Empty;
                        receiveMessageModel.Content = xn.SelectSingleNode("//Content") != null ? xn.SelectSingleNode("//Content").InnerText : String.Empty;
                        receiveMessageModel.MsgType = xn.SelectSingleNode("//MsgType") != null ? xn.SelectSingleNode("//MsgType").InnerText : String.Empty;
                        receiveMessageModel.CreateTime = xn.SelectSingleNode("//CreateTime") != null ? xn.SelectSingleNode("//CreateTime").InnerText : String.Empty;
                        receiveMessageModel.Description = xn.SelectSingleNode("//Description") != null ? xn.SelectSingleNode("//Description").InnerText : String.Empty;
                        receiveMessageModel.Format = xn.SelectSingleNode("//Format") != null ? xn.SelectSingleNode("//Format").InnerText : String.Empty;
                        receiveMessageModel.MediaId = xn.SelectSingleNode("//MediaId") != null ? xn.SelectSingleNode("//MediaId").InnerText : String.Empty;
                        receiveMessageModel.PicUrl = xn.SelectSingleNode("//PicUrl") != null ? xn.SelectSingleNode("//PicUrl").InnerText : String.Empty;
                        receiveMessageModel.ThumbMediaId = xn.SelectSingleNode("//ThumbMediaId") != null ? xn.SelectSingleNode("//ThumbMediaId").InnerText : String.Empty;
                        receiveMessageModel.Title = xn.SelectSingleNode("//Title") != null ? xn.SelectSingleNode("//Title").InnerText : String.Empty;
                        receiveMessageModel.MsgId = xn.SelectSingleNode("//MsgId") != null ? xn.SelectSingleNode("//MsgId").InnerText : String.Empty;
                        receiveMessageModel.Event = xn.SelectSingleNode("//Event") != null ? xn.SelectSingleNode("//Event").InnerText : String.Empty;
                        receiveMessageModel.EventKey = xn.SelectSingleNode("//EventKey") != null ? xn.SelectSingleNode("//EventKey").InnerText : String.Empty;
                        receiveMessageModel.Latitude = xn.SelectSingleNode("//Latitude") != null ? ConvertUtil.ToDouble(xn.SelectSingleNode("//Latitude").InnerText) : 0;
                        receiveMessageModel.Longitude = xn.SelectSingleNode("//Longitude") != null ? ConvertUtil.ToDouble(xn.SelectSingleNode("//Longitude").InnerText) : 0;
                        receiveMessageModel.Precision = xn.SelectSingleNode("//Precision") != null ? ConvertUtil.ToDouble(xn.SelectSingleNode("//Precision").InnerText) : 0;

                        #region 硬件平台属性

                        receiveMessageModel.DeviceType = xn.SelectSingleNode("//DeviceType") != null ? xn.SelectSingleNode("//DeviceType").InnerText : String.Empty;
                        receiveMessageModel.DeviceID = xn.SelectSingleNode("//DeviceID") != null ? xn.SelectSingleNode("//DeviceID").InnerText : String.Empty;
                        receiveMessageModel.SessionID = xn.SelectSingleNode("//SessionID") != null ? xn.SelectSingleNode("//SessionID").InnerText : String.Empty;
                        receiveMessageModel.MsgId = xn.SelectSingleNode("//MsgId") != null ? xn.SelectSingleNode("//MsgId").InnerText : String.Empty;
                        receiveMessageModel.OpenID = xn.SelectSingleNode("//OpenID") != null ? xn.SelectSingleNode("//OpenID").InnerText : String.Empty;

                        #endregion

                        Response.Write(DisposeWechatMessage(receiveMessageModel));
                        Response.End();
                    }
                }
            }
        }

        /// <summary>
        /// 处理推送消息
        /// </summary>
        /// <param name="wechatMessageModel"></param>
        /// <param name="shopModel"></param>
        /// <returns></returns>
        [NonAuthorized]
        public string DisposeWechatMessage(WeChatMsgViewModel wechatMessageModel)
        {
            string xml = string.Empty;
            // 因为 枚举中不能使用event关键字  做特殊处理
            wechatMessageModel.MsgType = wechatMessageModel.MsgType != "event" ? wechatMessageModel.MsgType : "events";
            LogUtil.Debug("openId:" + wechatMessageModel.FromUserName);

            // 目前只处理图片 文字 和事件推送
            EnumWeChatMessageType wechatType = (EnumWeChatMessageType)Enum.Parse(typeof(EnumWeChatMessageType), wechatMessageModel.MsgType);
            switch (wechatType)
            {
                case EnumWeChatMessageType.events:
                    {
                        if (wechatMessageModel.Event == EnumWeChatEventType.subscribe.ToString())
                        {
                            //关注 
                            LogUtil.Debug("关注");
                            Subscribe(wechatMessageModel);

                            //发送关注消息
                            xml = @"<xml>
                                        <ToUserName><![CDATA[{0}]]></ToUserName>
                                        <FromUserName><![CDATA[{1}]]></FromUserName>
                                        <CreateTime>{3}</CreateTime>
                                        <MsgType><![CDATA[text]]></MsgType>
                                        <Content><![CDATA[{2}]]></Content>
                               </xml>";
                            xml = GetAubscribeAutoMessage(wechatMessageModel, xml);
                        }
                        if (wechatMessageModel.Event == EnumWeChatEventType.unsubscribe.ToString())
                        {
                            //取消关注
                            LogUtil.Debug("取消关注");
                            UnSubscribe(wechatMessageModel);
                        }
                        if (wechatMessageModel.Event == EnumWeChatEventType.bind.ToString())
                        {
                            //绑定设备
                            LogUtil.Debug("绑定设备");
                        }
                        if (wechatMessageModel.Event == EnumWeChatEventType.unbind.ToString())
                        {
                            //解除绑定
                            LogUtil.Debug("解除绑定");
                        }
                        break;
                    }
                default:
                    break;

            }
            return xml;
        }

        /// <summary>
        /// 微信回调
        /// </summary>
        [NonAuthorized]
        public void WechatCallBack()
        {
            try
            {
                var openId = string.Empty;
                var state = Request["state"] as string;
                var code = Request["code"] as string;
                LogUtil.Debug(string.Format("state:{0},code:{1}", state, code));
                if (!string.IsNullOrWhiteSpace(code))
                {
                    openId = WeChatService.GetWechatOpenId(code);
                    SessionUtil.WechatOpenId = openId;
                }

                string url = state.Contains('/') ? state : string.Format("/device/{0}", state);
                LogUtil.Debug(string.Format("url:{0}", url));
                Response.Redirect(url, false);
            }
            catch (Exception ex)
            {
                LogUtil.Debug(ex.Message);
            }
        }

        #endregion

        #region 方法

        /// <summary>
        ///  微信推送消息安全校验
        /// </summary>
        /// <param name="appToken">Token</param>
        /// <returns>是否通过验证</returns>
        private bool CheckSignature(string appToken)
        {
            string signature = ConvertUtil.ToString(Request.QueryString["signature"], String.Empty);
            string timestamp = ConvertUtil.ToString(Request.QueryString["timestamp"], String.Empty);
            string nonce = ConvertUtil.ToString(Request.QueryString["nonce"], String.Empty);
            string[] ArrTmp = { appToken, timestamp, nonce };
            Array.Sort(ArrTmp);//字典排序  
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            return signature == tmpStr.ToLower();
        }

        /// <summary>
        /// 关注后回复的消息内容
        /// </summary>
        /// <param name="wechatMessageModel"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        private string GetAubscribeAutoMessage(WeChatMsgViewModel wechatMessageModel, string xml)
        {
            string aubscribeAutoMessage = ConfigUtil.WechatSubscribeAutoMessage;
            xml = String.Format(xml, wechatMessageModel.FromUserName, wechatMessageModel.ToUserName, aubscribeAutoMessage, wechatMessageModel.CreateTime);
            return xml;
        }

        /// <summary>
        /// 关注
        /// </summary>
        /// <param name="wechatMessageModel"></param>
        /// <returns></returns>
        private void Subscribe(WeChatMsgViewModel wechatMessageModel)
        {
            LogUtil.Debug("openId:" + wechatMessageModel.FromUserName);
            var wxUser = SingleInstance<WeChatService>.Instance.GetWeiXinUser(wechatMessageModel.FromUserName, EnumWeChatType.Client.GetHashCode());
            if (wxUser == null)
            {
                //新关注用户
                wxUser = new McpWeiXinUserInfo();
                wxUser.UserId = 0;
                wxUser.OpenId = wechatMessageModel.FromUserName;
                wxUser.WxType = EnumWeChatType.Client.GetHashCode();
                wxUser.StatusCode = StatusCodeType.Valid.GetHashCode();
                SingleInstance<WeChatService>.Instance.BindWeiXinUser(wxUser);
            }
            else
            {
                //已取消关注用户重新关注
                if (wxUser.StatusCode != StatusCodeType.Valid.GetHashCode())
                {
                    wxUser.StatusCode = StatusCodeType.Valid.GetHashCode();
                    SingleInstance<WeChatService>.Instance.UpdateWeiXinUser(wxUser);
                }
            }
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="wechatMessageModel"></param>
        /// <returns></returns>
        private bool UnSubscribe(WeChatMsgViewModel wechatMessageModel)
        {
            LogUtil.Debug("openId:" + wechatMessageModel.FromUserName);
            var wxUser = SingleInstance<WeChatService>.Instance.GetWeiXinUser(wechatMessageModel.FromUserName, EnumWeChatType.Client.GetHashCode());
            LogUtil.Debug(string.Format("取消关注，微信用户openId：{0}:", wechatMessageModel.FromUserName));
            return SingleInstance<WeChatService>.Instance.UnbindWeiXinUser(wxUser);
        }

        #endregion
    }
}
