using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evt.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CWI.MCP.Models
{
    public class TcpQueryModel
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public int Tp { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string Did { get; set; }

        /// <summary>
        /// 打印单号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 状态编码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 连接ID
        /// </summary>
        public long ConnId { get; set; }

        /// <summary>
        /// IP及端口地址
        /// </summary>
        public string IpPort { get; set; }

        /// <summary>
        /// 201指令类型：1-修改域名，2-关机重启，3-上传日志
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 201指令内容
        /// </summary>
        public string orderContent { get; set; }

        /// <summary>
        /// 101指令 pType = 1时，需要对打印机进行校验
        /// </summary>
        public string pType { get; set; }

        /// <summary>
        /// 101指令 打印机传入需要加密的key
        /// 106指令 打印机传入加密后的密钥，TCP进行解密对比
        /// </summary>
        public string pKey { get; set; }
    }
}
