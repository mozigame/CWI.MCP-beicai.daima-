//---------------------------------------------
// 版权信息：版权所有(C) 2015，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2012/10/24        创建
//---------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  CWI.MCP.Common.Extensions.ViewModel
{
    public class EUrlStatViewModel
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        public string RequestUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string ParamData
        {
            get;
            set;
        }

        /// <summary>
        /// 控制器
        /// </summary>
        public string Controller
        {
            get;
            set;
        }

        /// <summary>
        /// Action
        /// </summary>
        public string Action
        {
            get;
            set;
        }

        /// <summary>
        /// SessionID
        /// </summary>
        public string SessionID
        {
            get;
            set;
        }

        /// <summary>
        /// 请求类型
        /// </summary>
        public string RequestType
        {
            get;
            set;
        }

        /// <summary>
        /// 项目标识
        /// </summary>
        public string ProSign
        {
            get;
            set;
        }

        /// <summary>
        /// 请求头部参数
        /// </summary>
        public string HeaderParamData
        {
            get;
            set;
        }

        /// <summary>
        /// 用户ID;
        /// </summary>
        public string UserId 
        {
            get; set; 
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string DeviceId
        {
            get;
            set;
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType
        {
            get;
            set;
        }
    }
}
