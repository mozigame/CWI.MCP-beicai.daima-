// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期          说明
// --------------------------------------------
//      王军锋     2014/08/08       创建
// --------------------------------------------
using Evt.Framework.Common;
using System;
using System.Collections.Generic;


namespace CWI.MCP.Common
{
    public class ApiClient
    {
        /// <summary>
        /// 实例化
        /// </summary>
        private static ApiClient _instance = new ApiClient();

        /// <summary>
        /// 构造函数
        /// </summary>
        private ApiClient()
        {
        }


        /// <summary>
        /// 类唯一实例
        /// </summary>
        public static ApiClient Instance
        {
            get { return _instance; }
        }


        /// <summary>
        /// 执行Post方式API接口
        /// </summary>
        /// <param name="apiUrl">接口地址</param>
        /// <param name="parms">参数列表</param>
        /// <returns>服务器响应数据</returns>
        public string CallPostApi(string apiUrl, List<APIParameter> parms)
        {
            if (NetUtil.IsConnectedInternet)
            {
                var result = new SyncHttp().HttpPost(apiUrl, HttpUtil.GetQueryFromParas(parms));
                return result;
            }
            else
            {
                LogUtil.Error(Consts.NET_EXCEPTION_MSG);
                return Consts.NET_EXCEPTION_MSG;
            }
        }

        /// <summary>
        /// 执行Get方式API接口
        /// </summary>
        /// <param name="apiUrl">接口地址</param>
        /// <param name="parms">参数列表</param>
        /// <returns>服务器响应数据</returns>
        public string CallGetApi(string apiUrl, List<APIParameter> parms)
        {
            if (NetUtil.IsConnectedInternet)
            {
                var result = new SyncHttp().HttpGet(apiUrl, HttpUtil.GetQueryFromParas(parms));
                return result;
            }
            else
            {
                LogUtil.Error(Consts.NET_EXCEPTION_MSG);
                throw new MessageException(Consts.NET_EXCEPTION_MSG);
            }
        }
    }
}