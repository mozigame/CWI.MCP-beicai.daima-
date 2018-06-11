using CWI.MCP.Common;
using CWI.MCP.Services;
using CWI.MCP.Models;
using System.Collections.Generic;
using Evt.Framework.Common;
using System.Web.Mvc;
using System;

namespace CWI.MCP.Controllers.IOT
{
    public class DeviceController : WeiXinBaseController
    {
        #region View页面

        /// <summary>
        /// 设备列表
        /// </summary>
        /// <returns>View</returns>
        [NonAuthorized]
        public ActionResult List()
        {
            return View();
        }

        /// <summary>
        /// 设备详细
        /// </summary>
        /// <returns>View</returns>
        [NonAuthorized]
        public ActionResult Detail(string deviceId)
        {
            var device = SingleInstance<DeviceService>.Instance.GetWxDevice(deviceId);
            return View(device);
        }

        /// <summary>
        /// 配置WiFi页面
        /// </summary>
        /// <returns></returns>
        [NonAuthorized]
        public object Config()
        {
            ViewBag.ConfigModel = SingleInstance<WeChatService>.Instance.GetJsApiParamsModel(EnumWeChatType.Client.GetHashCode(), Request.Url.AbsoluteUri);
            ViewBag.WiFiKey = CommonUtil.GetBase64String(ConfigUtil.WiFiKey);
            return View();
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <param name="pageModel">分页参数</param>
        /// <returns></returns>
        [NonAuthorized]
        public object GetDevices(PageViewModel pageModel)
        {
            int pageCount = 0;
            var key = new Dictionary<string, object>();
            var devices = SingleInstance<DeviceService>.Instance.GetDevices(pageModel, out pageCount);
            key.Add("PageCount", pageCount);
            key.Add("Devices", devices);
            return OK(key);
        }

        #endregion

        #region 请求接口

        /// <summary>
        /// 设备授权
        /// </summary>
        /// <param name="macCode"></param>
        /// <param name="snCode"></param>
        /// <returns></returns>
        [NonAuthorized]
        public object Auth(string macCode, string snCode)
        {
            try
            {
                SingleInstance<DeviceService>.Instance.AuthDevice(macCode, snCode);
                return OK();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 开通WiFi
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [NonAuthorized]
        public object OpenWiFi(string deviceId)
        {
            try
            {
                SingleInstance<DeviceService>.Instance.OpenDeviceWiFi(deviceId);
                return OK();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        #endregion

        #region 集成OpenAPI

        /// <summary>
        /// 设备查询平台回调
        /// </summary>
        /// <param name="notifyModel"></param>
        /// <returns></returns>
        public object GetDeviceNotify(NotifyViewModel notifyModel)
        {
            var ret = SingleInstance<DeviceService>.Instance.GetDeviceCallBack(notifyModel);
            return OK(ret);
        }

        /// <summary>
        /// 设备控制平台回调
        /// </summary>
        /// <param name="notifyModel"></param>
        /// <returns></returns>
        public object SetDeviceNotify(NotifyViewModel notifyModel)
        {
            var ret = SingleInstance<DeviceService>.Instance.SetDeviceCallBack(notifyModel);
            return OK(ret);
        }

        /// <summary>
        /// 设备状态更新通知回调
        /// </summary>
        /// <param name="notifyModel"></param>
        /// <returns></returns>
        public object DeviceStatusNotify(StatusNotifyViewModel notifyModel)
        {
            var ret = SingleInstance<DeviceService>.Instance.DeviceStatusCallBack(notifyModel);
            return OK(ret);
        }

        #endregion
    }
}
