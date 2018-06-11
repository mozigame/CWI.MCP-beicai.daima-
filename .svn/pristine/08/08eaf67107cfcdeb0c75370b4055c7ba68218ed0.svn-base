using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using CWI.MCP.Common;
using CWI.MCP.Common.ORM;
using CWI.MCP.Models;
using Evt.Framework.Common;
using Newtonsoft.Json;

namespace CWI.MCP.Services
{
    public class DeviceService : BaseService
    {
        /// <summary>
        /// 获取微信设备信息
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public McpWeiXinDeviceInfo GetWxDevice(string deviceId)
        {
            var cc = new ConditionCollection();
            cc.Add(new Condition("device_id", deviceId));
            return this.GetRepository<McpWeiXinDeviceInfo>().GetModel(cc);
        }

        /// <summary>
        /// 获取设备列表【授权或开通WiFi】
        /// </summary>
        /// <returns></returns>
        public object GetDevices(PageViewModel pageModel, out int pageCount)
        {
            var list = GetDeviceList(pageModel);
            int totalCount = this.GetRepository<McpEquipmentInfo>().Count();
            int pageSize = pageModel.PageSize > 0 ? pageModel.PageSize : 10;
            pageCount = (int)Math.Ceiling(TryConvertUtil.ToDecimal(totalCount) / pageSize);

            return (from o in list
                    select new
                    {
                        EquId = o.EquipmentId,
                        EquCode = o.EquipmentCode,
                        SnCode = o.CheckCode,
                        DeviceId = o.DeviceId,
                        IsAuth = o.IsAuth,
                        IsOpenWifi = o.IsOpenWifi
                    });
        }

        /// <summary>
        /// 设备授权操作
        /// </summary>
        /// <param name="printerCode">设备编号</param>
        /// <param name="checkCode">设备校验码</param>
        /// <param name="isOpenWiFi">是否开通Wi-Fi</param>
        /// <returns></returns>
        public void AuthDevice(string printerCode, string checkCode, bool isOpenWiFi = false)
        {
            var authModel = new List<AuthViewModel>();
            authModel.Add(new AuthViewModel() { sn = printerCode, ck = checkCode });
            AuthDevice(authModel, isOpenWiFi);
        }

        /// <summary>
        /// 设备授权操作
        /// </summary>
        /// <param name="printerCode">设备编号</param>
        /// <param name="checkCode">设备校验码</param>
        /// <param name="isOpenWiFi">是否开通Wi-Fi</param>
        /// <returns></returns>
        public void AuthDevice(List<AuthViewModel> authModel, bool isOpenWiFi = false)
        {
            try
            {
                //1.校验设备
                var mcpEqus = CheckDevice(authModel, 1);

                //2.调用获取设备ID及二维码接口
                var devices = GetDeviceList(ConfigUtil.ProductId, mcpEqus);

                //3.调用控制设备授权接口API&回调更新
                devices = AuthDeviceList(devices);

                //4.控制设备【开通Wi-Fi】
                if (isOpenWiFi)
                {
                    OpenDeviceWiFi(devices);
                }

                //5.更新对应关系
                SaveWxDeviceInfo(devices, mcpEqus, isOpenWiFi);
            }
            catch (Exception ex)
            {
                throw new MessageException(ex.Message);
            }
        }

        #region 私有方法

        /// <summary>
        /// 获取单个DeviceId
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <returns></returns>
        private GetDeviceRetViewModel GetDevice(string productId)
        {
            var list = GetDeviceList(productId);
            return list.FirstOrDefault();
        }

        /// <summary>
        /// 获取指定数量的设备列表
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <param name="mcpEqus">设备列表</param>
        /// <returns>设备Id列表</returns>
        private List<GetDeviceRetViewModel> GetDeviceList(string productId, List<McpEquipmentInfo> mcpEqus = null)
        {
            var deviceNum = mcpEqus != null ? mcpEqus.Count : 1;
            var deviceList = new List<GetDeviceRetViewModel>();
            for (var i = 0; i < deviceNum; i++)
            {
                var getModel = CallGetDeviceIdAPI(productId);
                if (getModel == null)
                {
                    throw new MessageException("请求微信硬件获取设备ID接口失败");
                }
                if (getModel.base_resp.errcode != 0)
                {
                    throw new MessageException(getModel.base_resp.errmsg);
                }

                deviceList.Add(new GetDeviceRetViewModel
                {
                    deviceid = getModel.deviceid,
                    devicelicence = getModel.devicelicence,
                    qrticket = getModel.qrticket,
                    devicemac = mcpEqus != null ? mcpEqus[i].EquipmentCode : string.Empty
                });
            }
            return deviceList;
        }

        /// <summary>
        /// 设备批量授权
        /// </summary>
        /// <param name="devices"></param>
        private List<GetDeviceRetViewModel> AuthDeviceList(List<GetDeviceRetViewModel> devices)
        {
            //1.构建请求接口参数列表
            var deviceList = new List<DeviceInfoViewModel>();
            foreach (var device in devices)
            {
                deviceList.Add(new DeviceInfoViewModel()
                {
                    id = device.deviceid,
                    mac = device.devicemac.PadRight(12,'0'),
                    connect_protocol = "4",
                    auth_key = string.Empty,//ConfigUtil.AuthKey,
                    close_strategy = "1",
                    conn_strategy = "1",
                    crypt_method = "0",
                    auth_ver = "0",
                    manu_mac_pos = "-1",
                    ser_mac_pos = "-2",
                    ble_simple_protocol = "0"
                });
            }

            //2.请求调用设备授权接口
            var configModel = new ConfigDeviceViewModel();
            configModel.device_list = deviceList;
            configModel.device_num = configModel.device_list.Count;
            configModel.op_type = 1;
            var setModel = CallAuthDeviceAPI(configModel);
            if (setModel == null)
            {
                throw new MessageException("请求微信硬件授权接口失败");
            }
            else
            {
                for (var i = 0; i < setModel.resp.Count; i++)
                {
                    if (setModel.resp[i].errcode == 0)
                    {
                        devices[i].devicetype = setModel.resp[i].base_info.device_type;
                    }
                    else
                    {
                        throw new MessageException(setModel.resp[i].errmsg);
                    }
                }
            }
            return devices;
        }

        /// <summary>
        /// 开通设备WiFi功能
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public void OpenDeviceWiFi(string deviceId)
        {
            try
            {
                //1.校验设备
                var mcpEqu = CheckDevice(string.Empty, string.Empty, deviceId, 2);

                //2. 控制设备
                OpenDeviceWiFi(deviceId, mcpEqu.DeviceType);
            }
            catch (Exception ex)
            {
                throw new MessageException(ex.Message);
            }
        }

        /// <summary>
        /// 开通设备WiFi功能
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceType"></param>
        private void OpenDeviceWiFi(string deviceId, string deviceType)
        {
            var servs = new Dictionary<string, object>();
            var status = new Dictionary<string, object>();
            status.Add("status", 1);
            servs.Add("operation_status", status);
            var deviceModel = new DeviceViewModel();
            deviceModel.user = "oskDA0TRIx9xNfRzoNLenXJ3bWFU";
            deviceModel.device_id = deviceId;
            deviceModel.device_type = deviceType;
            deviceModel.services = servs;
            deviceModel.data = string.Empty;
            var retModel = SingleInstance<DeviceService>.Instance.SetDevice(deviceModel);
            if (retModel == null)
            {
                throw new MessageException("请求微信硬件控制硬件接口失败");
            }
            if (!string.IsNullOrWhiteSpace(retModel.error_msg))
            {
                throw new MessageException(retModel.error_msg);
            }
        }

        /// <summary>
        /// 开通设备WiFi功能
        /// </summary>
        /// <param name="devices">设备列表</param>
        private void OpenDeviceWiFi(List<GetDeviceRetViewModel> devices)
        {
            if(devices!=null && devices.Count>0)
            {
                foreach(var device in devices)
                {
                    OpenDeviceWiFi(device.deviceid, device.devicetype);
                }
            }
        }

        /// <summary>
        /// 保存微信设备信息及授权、WiFi开通标识
        /// </summary>
        /// <param name="devices"></param>
        /// <param name="mcpEqus"></param>
        /// <param name="isOpenWiFi"></param>
        private void SaveWxDeviceInfo(List<GetDeviceRetViewModel> devices, List<McpEquipmentInfo> mcpEqus, bool isOpenWiFi)
        {
            var dbNow = CommonUtil.GetDBDateTime();
            using (TransactionScope tran = new TransactionScope())
            {
                for (var i = 0; i < devices.Count; i++)
                {
                    //5.1.创建设备号与微信deviceId关联
                    var mcpDevice = new McpWeiXinDeviceInfo();
                    mcpDevice.DeviceId = devices[i].deviceid;
                    mcpDevice.DeviceLicence = devices[i].devicelicence;
                    mcpDevice.Qrticket = devices[i].qrticket;
                    mcpDevice.DeviceType = devices[i].devicetype;
                    mcpDevice.CreatedOn = dbNow;
                    this.GetRepository<McpWeiXinDeviceInfo>().Create(mcpDevice);

                    //5.2.更新微云打设备授权、Wi-Fi开通标识
                    mcpEqus[i].DeviceId = devices[i].deviceid;
                    mcpEqus[i].IsAuth = 1;
                    mcpEqus[i].IsOpenWifi = isOpenWiFi ? 1 : 0;
                    mcpEqus[i].ModifiedOn = dbNow;
                    this.GetRepository<McpEquipmentInfo>().Update(mcpEqus[i]);
                }

                tran.Complete();
            }
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <param name="pageModel">分页参数</param>
        /// <returns>设备列表</returns>
        private List<McpEquipmentInfo> GetDeviceList(PageViewModel pageModel)
        {
            int pgStartIndex = 0;
            int pgEndIndex = int.MaxValue;
            if (pageModel != null)
            {
                pgStartIndex = (pageModel.PageIndex - 1) * pageModel.PageSize;
                pgEndIndex = pageModel.PageSize;
            }
            var pc = new ParameterCollection();
            pc.Add("start_index", pgStartIndex);
            pc.Add("end_index", pgEndIndex);

            string sql = "SELECT equipment_id,equipment_code,check_code,device_id,is_auth,is_open_wifi FROM mcp_equipment ORDER BY is_auth,is_open_wifi ASC LIMIT $start_index$, $end_index$";
            return this.GetRepository<McpEquipmentInfo>().ListModelBySql(sql, pc);
        }

        /// <summary>
        /// 获取微云打平台设备列表
        /// </summary>
        /// <param name="mcpEqus"></param>
        /// <returns></returns>
        private List<McpEquipmentInfo> GetMcpDeviceList(List<AuthViewModel> mcpEqus)
        {
            var equs = new StringBuilder();
            if (mcpEqus != null)
            {
                foreach (var equ in mcpEqus)
                {
                    equs.AppendFormat("'{0}-{1}',", equ.sn, equ.ck);
                }
            }

            var sql = string.Format(@"SELECT * FROM mcp_equipment WHERE CONCAT_WS('-',equipment_code,check_code) IN ({0})", equs.ToString().TrimEnd(','));
            return this.GetRepository<McpEquipmentInfo>().ListModelBySql(sql);
        }

        /// <summary>
        /// 获取微云打平台设备信息
        /// </summary>
        /// <param name="macCode"></param>
        /// <returns></returns>
        public McpEquipmentInfo GetMcpDevicInfo(string macCode)
        {
            var cc = new ConditionCollection();
            cc.Add(new Condition("equipment_code", macCode));
            return this.GetRepository<McpEquipmentInfo>().GetModel(cc);
        }

        /// <summary>
        /// 获取微云打平台设备信息
        /// </summary>
        /// <param name="macCode"></param>
        /// <returns></returns>
        private McpEquipmentInfo GetMcpDevicInfoByDeviceId(string deviceId)
        {
            var cc = new ConditionCollection();
            cc.Add(new Condition("device_id", deviceId));
            return this.GetRepository<McpEquipmentInfo>().GetModel(cc);
        }

        /// <summary>
        /// 校验设备是否有效
        /// </summary>
        /// <param name="macCode"></param>
        /// <param name="snCode"></param>
        private McpEquipmentInfo CheckDevice(string macCode, string snCode, string deviceId, int operateType)
        {
            McpEquipmentInfo mcpDevice = null;
            if (!string.IsNullOrWhiteSpace(macCode) && !string.IsNullOrWhiteSpace(snCode))
            {
                mcpDevice = GetMcpDevicInfo(macCode);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(deviceId))
                {
                    var cc = new ConditionCollection();
                    cc.Add(new Condition("device_id", deviceId));
                    var wxDevice = this.GetRepository<McpWeiXinDeviceInfo>().GetModel(cc);

                    mcpDevice = GetMcpDevicInfoByDeviceId(deviceId);
                    mcpDevice.DeviceType = wxDevice != null ? wxDevice.DeviceType : string.Empty;
                }
            }

            if (mcpDevice == null || (!mcpDevice.CheckCode.Equals(snCode, StringComparison.CurrentCultureIgnoreCase) && operateType == 1))
                throw new MessageException("未找到对应设备");
            if(operateType == 1 && mcpDevice.IsAuth == 1)
                throw new MessageException("该设备已授权，无需重新授权");
            if (operateType == 2 && mcpDevice.IsAuth == 0)
                throw new MessageException("请先对设备进行授权再开通WiFi");
            if (operateType == 2 && mcpDevice.IsOpenWifi == 1)
                throw new MessageException("该设备已开通WiFi，无需重复开通");
            return mcpDevice;
        }

        /// <summary>
        /// 校验设备是否有效
        /// </summary>
        /// <param name="mcpEqus">设备列表</param>
        /// <param name="operateType">操作类型</param>
        /// <returns></returns>
        private List<McpEquipmentInfo> CheckDevice(List<AuthViewModel> mcpEqus, int operateType)
        {
            var mcpDevices = GetMcpDeviceList(mcpEqus);
            if (mcpDevices == null)
            {
                throw new MessageException("未找到对应设备");
            }

            if (operateType == 1)
            {
                var authDevice = mcpDevices.Find(d => d.IsAuth == 1);
                if (authDevice != null)
                {
                    throw new MessageException(string.Format("设备：{0}已授权,无需重新授权", authDevice.EquipmentCode));
                }
            }
            else if (operateType == 2)
            {
                var authDevice = mcpDevices.Find(d => d.IsAuth == 0);
                if (authDevice != null)
                {
                    throw new MessageException(string.Format("设备：{0}请先授权,再开通WiFi", authDevice.EquipmentCode));
                }

                var wifiDevice = mcpDevices.Find(d => d.IsOpenWifi == 1);
                if (wifiDevice != null)
                {
                    throw new MessageException(string.Format("设备：{0}已开通WiFi,无需重复开通", wifiDevice.EquipmentCode));
                }
            }

            return mcpDevices;
        }

        #endregion

        #region IOT硬件平台集成

        /// <summary>
        /// 获取设备ID及二维码
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <returns></returns>
        private GetDeviceRetViewModel CallGetDeviceIdAPI(string productId)
        {
            string weiXinToken = WeChatService.GetWeiXinToken(EnumWeChatType.Client.GetHashCode());
            var url = string.Format(WeChatConsts.IOT_GET_DEVICE_INFO, weiXinToken);
            var parms = string.Format("&product_id={0}", productId);
            var result = NetUtil.ResponseByGet(url, parms);
            return JsonConvert.DeserializeObject<GetDeviceRetViewModel>(result.Trim());
        }

        /// <summary>
        /// 授权设备
        /// </summary>
        /// <param name="configModel">设备授权信息</param>
        /// <returns></returns>
        public AuthDeviceRetViewModel CallAuthDeviceAPI(ConfigDeviceViewModel configModel)
        {
            var obj = new AuthDeviceRetViewModel();
            string weiXinToken = WeChatService.GetWeiXinToken(EnumWeChatType.Client.GetHashCode());
            var url = string.Format(WeChatConsts.IOT_AUTH_DEVICE, weiXinToken);
            if (configModel != null)
            {
                var parms = JsonConvert.SerializeObject(configModel);
                var result = NetUtil.WechatSendPostRequest(url, parms);
                obj = JsonConvert.DeserializeObject<AuthDeviceRetViewModel>(result.Trim());
            }
            return obj;
        }

        /// <summary>
        /// 设备查询
        /// </summary>
        /// <returns></returns>
        public BaseRetViewModel GetDevice(DeviceViewModel deviceModel)
        {
            BaseRetViewModel retModel=null; 
            string weiXinToken = WeChatService.GetWeiXinToken(EnumWeChatType.Client.GetHashCode());
            var url = string.Format(WeChatConsts.IOT_GET_DEVICE, weiXinToken);
            if (deviceModel != null)
            {
                var parms = string.Format("&device_type={0}&device_id={1}&services={2}&user={3}&data={4}",
                    deviceModel.device_type, deviceModel.device_id, deviceModel.services.ToArray().ToString(), deviceModel.data);
                var result = NetUtil.ResponseByPost(url, parms);
                if (string.IsNullOrWhiteSpace(result))
                {
                    retModel = JsonConvert.DeserializeObject<BaseRetViewModel>(result.Trim());
                }
            }
            return retModel;
        }

        /// <summary>
        /// 设备控制
        /// </summary>
        /// <returns></returns>
        public BaseRetViewModel SetDevice(DeviceViewModel deviceModel)
        {
            BaseRetViewModel retModel = null; 
            string weiXinToken = WeChatService.GetWeiXinToken(EnumWeChatType.Client.GetHashCode());
            var url = string.Format(WeChatConsts.IOT_SET_DEVICE, weiXinToken);
            if (deviceModel != null)
            {
                var parms = JsonConvert.SerializeObject(deviceModel);
                var result = NetUtil.WechatSendPostRequest(url, parms);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    retModel = JsonConvert.DeserializeObject<BaseRetViewModel>(result.Trim());
                }
            }
            return retModel;
        }

        /// <summary>
        /// 设备查询回调处理
        /// </summary>
        /// <param name="notifyModel"></param>
        /// <returns></returns>
        public BaseRetViewModel GetDeviceCallBack(NotifyViewModel notifyModel)
        {
            var ret = new BaseRetViewModel();
            return ret;
        }

        /// <summary>
        /// 设备控制回调处理
        /// </summary>
        /// <param name="notifyModel"></param>
        /// <returns></returns>
        public BaseRetViewModel SetDeviceCallBack(NotifyViewModel notifyModel)
        {
            var ret = new BaseRetViewModel();
            return ret;
        }

        /// <summary>
        /// 状态通知回调处理
        /// </summary>
        /// <param name="notifyModel"></param>
        /// <returns></returns>
        public BaseRetViewModel DeviceStatusCallBack(StatusNotifyViewModel notifyModel)
        {
            var ret = new BaseRetViewModel();
            return ret;
        }

        #endregion
    }
}
