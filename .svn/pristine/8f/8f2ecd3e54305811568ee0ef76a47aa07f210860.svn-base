//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋       2014/08/01         创建
//---------------------------------------------

using CWI.MCP.Common;
using CWI.MCP.Models;
using CWI.MCP.Services;
using Evt.Framework.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace CWI.MCP.API.Controllers.MCP
{
    public class DeviceController : BaseController
    {
        /// <summary>
        /// 设备授权
        /// </summary>
        /// <param name="model">设备信息</param>
        /// <returns></returns>
        [CWI.MCP.API.Handels.SkipAuthorize]
        [HttpGet]
        public object Auth([FromUri]AuthViewModel model)
        {
            try
            {
                SingleInstance<DeviceService>.Instance.AuthDevice(model.sn, model.ck, true);
                return OK();
            }
            catch (Exception ex)
            {
                return Failed(ex.Message);
            }
        }

        /// <summary>
        /// 设备授权
        /// </summary>
        /// <param name="authJson">设备信息</param>
        /// <returns></returns>
        [CWI.MCP.API.Handels.SkipAuthorize]
        [HttpPost]
        public object BatchAuth([FromBody]BatchAuthViewModel authJson)
        {
            try
            {
                var devices = JsonUtil.Deserialize<List<AuthViewModel>>(authJson.data);
                SingleInstance<DeviceService>.Instance.AuthDevice(devices, true);
                return OK();
            }
            catch (Exception ex)
            {
                return Failed(ex.Message);
            }
        }
    }
}
