
//---------------------------------------------
// 版权信息：版权所有(C) 2011，EVT Tech
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      徐树林        2011/11/03        创建
//---------------------------------------------

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
using CWI.MCP.Common.ORM;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

namespace CWI.MCP.Common
{
    public class SmsUtil
    {
        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="validateCode">验证码</param>
        public static void SendValidCode(string mobile, string validateCode, decimal validateCodeExpire, SmsFuncType type)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("code", validateCode);
            dic.Add("code_expire", validateCodeExpire);
            string[] paramArray = new string[] { validateCode, validateCodeExpire.ToString() };

            //发送渠道
            var sendWay = ConfigUtil.SmsWay;

            switch (type)
            {
                case SmsFuncType.OpenRegister:
                    switch (sendWay)
                    {
                        case "0":
                            var smsParam = "{\"code\":\"" + validateCode + "\",\"second\":\"" + validateCodeExpire + "\"}";
                            SendSmsByAli(mobile, smsParam, SmsTemplateType.OPEN_DY_ValidCode.GetHashCode());
                            break;
                        case "1":
                            SendSmsByRly(mobile, paramArray, SmsTemplateType.OPEN_RLY_ValidCode.GetHashCode());
                            break;
                        default:
                            break;
                    }
                    break;
                case SmsFuncType.OpenModifyPwd:
                    switch (sendWay)
                    {
                        case "0":
                            var smsParam = "{\"code\":\"" + validateCode + "\",\"second\":\"" + validateCodeExpire + "\"}";
                            SendSmsByAli(mobile, smsParam, SmsTemplateType.OPEN_DY_ValidCode.GetHashCode());
                            break;
                        case "1":
                            SendSmsByRly(mobile, paramArray, SmsTemplateType.OPEN_RLY_ValidCodeForMpwd.GetHashCode());
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        #region 0-【阿里大鱼】私有

        /// <summary>
        /// 【阿里大鱼】执行发送阿里大鱼短信方法
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="smsParam">短信模板变量</param>
        /// <param name="smsTemplateCode">短信模板ID</param>
        /// <returns>通过判断调用借口返回的错误信息来判断是否成功</returns>
        public static string SendSmsByAli(string mobile, string smsParam, int smsTemplateCode)
        {
            var serverUrl = ConfigUtil.ServerUrl_ali;
            var appKey = ConfigUtil.Appkey_ali;
            var appSecret = ConfigUtil.AppSecret_ali;
            ITopClient client = new DefaultTopClient(serverUrl, appKey, appSecret);
            // 构建阿里大鱼短信实体对象
            var req = new AlibabaAliqinFcSmsNumSendRequest
            {
                // 公共回传参数，在“消息返回”中会透传回该参数
                Extend = "1234",
                // 短信类型，传入值写入normal 
                SmsType = "normal",
                // 短信签名
                SmsFreeSignName = ConfigUtil.SmsFreeSignName_ali,
                // 短信模板变量
                SmsParam = smsParam,
                // 短信接收号码
                RecNum = mobile,
                // 短信模板ID
                SmsTemplateCode = string.Format("SMS_{0}", smsTemplateCode.ToString())
            };

            var rsp = client.Execute(req);
            return rsp.ErrMsg;
        }

        #endregion

        #region 1-【电信】私有

        /// <summary>
        /// 获取访问令牌API接口
        /// </summary>
        private static readonly string GetDxSmsTokenApi = "https://oauth.api.189.cn/emp/oauth2/v3/access_token";

        /// <summary>
        /// 发送短信API接口
        /// </summary>
        private static readonly string SendDxSmsApi = "http://api.189.cn/v2/emp/templateSms/sendSms";

        /// <summary>
        /// Token字典
        /// </summary>
        private static Dictionary<string, Tuple<string, DateTime>> _dXSmsToken = new Dictionary<string, Tuple<string, DateTime>>();

        /// <summary>
        /// 获取登录Token
        /// </summary>
        /// <param name="tryNum"></param>
        /// <returns></returns>
        private static string GetDxSmsAccessToken(int tryNum)
        {
            string key = "smstoken";
            if (_dXSmsToken.ContainsKey(key))
            {
                var kv = _dXSmsToken[key];
                if (kv.Item2 > DateTime.Now)
                {
                    return kv.Item1;
                }
            }
            Dictionary<string, string> paras = new Dictionary<string, string>();
            paras.Add("grant_type", "client_credentials");
            paras.Add("app_id", ConfigUtil.SmsAppId_dx);
            paras.Add("app_secret", ConfigUtil.SmsAppSecret_dx);
            try
            {
                string res = NetUtil.SendPostRequest(GetDxSmsTokenApi, paras);
                Dictionary<string, string> resDic = JsonUtil.Deserialize<Dictionary<string, string>>(res);
                if (resDic.ContainsKey("access_token"))
                {
                    _dXSmsToken.Clear();
                    _dXSmsToken.Add(key, new Tuple<string, DateTime>(resDic["access_token"], DateTime.Now.AddSeconds(ConvertUtil.ToInt(resDic["expires_in"], 0))));
                    return resDic["access_token"];
                }
                else
                {
                    LogUtil.Debug(resDic["res_message"]);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex.ToString());
                System.Threading.Thread.Sleep(200);
                if (tryNum < 3)
                {
                    return GetDxSmsAccessToken(++tryNum);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 【电信】发送短信
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="templateParam"></param>
        /// <param name="tempateId"></param>
        private static void SendSmsByDx(string mobile, Dictionary<string, object> templateParam, int tempateId)
        {
            string token = GetDxSmsAccessToken(0);
            if (string.IsNullOrEmpty(token))
            {
                return;
            }
            Dictionary<string, string> paras = new Dictionary<string, string>();

            paras.Add("acceptor_tel", mobile);
            paras.Add("access_token", token);
            paras.Add("app_id", ConfigUtil.SmsAppId_dx);
            paras.Add("template_id", tempateId.ToString());
            paras.Add("template_param", JsonUtil.Serialize(templateParam));
            paras.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            try
            {
                string res = NetUtil.SendPostRequest(SendDxSmsApi, paras);
                LogUtil.Debug(string.Format("发送手机短信：{0},{1}", mobile, JsonUtil.Serialize(templateParam)));
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex.ToString());
            }
        }

        #endregion

        #region 2-【荣联云】私有

        /// <summary>
        /// 【荣联云】短信平台域名
        /// </summary>
        private static readonly string RlySmsIpDomain = "sandboxapp.cloopen.com";

        /// <summary>
        /// 【荣联云】短信平台端口
        /// </summary>
        private static readonly string RlySmsPort = "8883";

        /// <summary>
        /// 【荣联云】发送短信
        /// </summary>
        private static void SendSmsByRly(string mobile, string[] templateParam, int tempateId)
        {
            CCPRestSDK api = new CCPRestSDK();
            bool isInit = api.init(RlySmsIpDomain, RlySmsPort);
            api.setAccount(ConfigUtil.SmsAccountSid_rly, ConfigUtil.SmsAuthToken_rly);
            api.setAppId(ConfigUtil.SmsAppId_rly);

            try
            {
                if (isInit)
                {
                    Dictionary<string, object> retData = api.SendTemplateSMS(mobile, tempateId.ToString(), templateParam);
                    string ret = GetRlySmsDictData(retData);
                    LogUtil.Info(ret);
                }
                else
                {
                    LogUtil.Error("短信模版初始化失败！");
                }
            }
            catch (Exception exc)
            {
                LogUtil.Error(exc.ToString());
            }
        }

        /// <summary>
        /// 获取短信数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetRlySmsDictData(Dictionary<string, object> data)
        {
            string ret = null;
            foreach (KeyValuePair<string, object> item in data)
            {
                if (item.Value != null && item.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    ret += item.Key.ToString() + "={";
                    ret += GetRlySmsDictData((Dictionary<string, object>)item.Value);
                    ret += "};";
                }
                else
                {
                    ret += item.Key.ToString() + "=" + (item.Value == null ? "null" : item.Value.ToString()) + ";";
                }
            }
            return ret;
        }

        #endregion
    }
}

