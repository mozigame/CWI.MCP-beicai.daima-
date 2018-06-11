using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWI.MCP.Common
{
    /// <summary>
    /// 消息模版基类
    /// </summary>
    public class TemplateMessageModel
    {
        public string TemplateId { get; set; }

        public TemplateDataItem first { get; set; }

        public TemplateDataItem remark { get; set; }
    }

    /// <summary>
    /// 上传文件成功
    /// </summary>
    public class UpSuccTemplateModel : TemplateMessageModel
    {
        public TemplateDataItem keyword1 { get; set; }

        public TemplateDataItem keyword2 { get; set; }

        public UpSuccTemplateModel()
            : base()
        {
            TemplateId = WeChatConsts.ClientUpSuccWxTempId;
        }
    }

    /// <summary>
    /// 支付成功
    /// </summary>
    public class PaySuccTemplateModel : TemplateMessageModel
    {
        public TemplateDataItem keyword1 { get; set; }

        public TemplateDataItem keyword2 { get; set; }

        public TemplateDataItem keyword3 { get; set; }


        public PaySuccTemplateModel()
            : base()
        {
            TemplateId = WeChatConsts.ClientPaySuccWxTempId;
        }
    }

    /// <summary>
    /// 退款成功
    /// </summary>
    public class RefundSuccTemplateModel : TemplateMessageModel
    {
        public TemplateDataItem keyword1 { get; set; }

        public TemplateDataItem keyword2 { get; set; }


        public RefundSuccTemplateModel()
            : base()
        {
            TemplateId = WeChatConsts.ClientRefundSuccWxTempId;
        }
    }

    /// <summary>
    /// 打印成功
    /// </summary>
    public class PrintSuccTemplateModel : TemplateMessageModel
    {
        public TemplateDataItem keyword1 { get; set; }

        public TemplateDataItem keyword2 { get; set; }

        public PrintSuccTemplateModel()
            : base()
        {
            TemplateId = WeChatConsts.ClientPrintSuccWxTempId;
        }
    }
}
