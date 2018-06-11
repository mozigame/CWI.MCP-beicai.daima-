//---------------------------------------------
// 版权信息：版权所有(C) 2016，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2016/07/04        创建
//---------------------------------------------

using System;
using Evt.Framework.Common;
using Evt.Framework.DataAccess;

namespace CWI.MCP.Models
{
    /// <summary>
    /// McpEquipmentInfo
    /// </summary>
 	[Serializable]
    [TableMapping(TableName = "mcp_equipment")]
	public class McpEquipmentInfo : Model
    {
		/// <summary>
		/// 设备ID
		/// </summary>
		[TableMapping(FieldName = "equipment_id", PrimaryKey = true)]
		public  int EquipmentId { set; get; }

		/// <summary>
		/// 1:58mm热敏小票,2:80mm热敏小票,3:针式快递面单
		/// </summary>
		[TableMapping(FieldName = "equipment_type")]
		public  int EquipmentType { set; get; }

		/// <summary>
		/// 设备编码唯一编号
		/// </summary>
		[TableMapping(FieldName = "equipment_code")]
		public string EquipmentCode { set; get; }

        /// <summary>
        /// 校验码
        /// </summary>
        [TableMapping(FieldName = "check_code")]
        public string CheckCode { set; get; }

        /// <summary>
        /// 设备ID
        /// </summary>
        [TableMapping(FieldName = "device_id")]
        public string DeviceId { set; get; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { set; get; }

        /// <summary>
        /// 是否关联:0-未关联,1-已关联
        /// </summary>
        [TableMapping(FieldName = "is_bind")]
        public int IsBind { set; get; }

        /// <summary>
        /// 是否授权:0-未授权,1-已授权
        /// </summary>
        [TableMapping(FieldName = "is_auth")]
        public int IsAuth { set; get; }

        /// <summary>
        /// 是否开通WiFi:0-未开通,1-已开通
        /// </summary>
        [TableMapping(FieldName = "is_open_wifi")]
        public int IsOpenWifi { set; get; }

		/// <summary>
		/// 创建人
		/// </summary>
		[TableMapping(FieldName = "created_by")]
		public string CreatedBy { set; get; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[TableMapping(FieldName = "created_on")]
		public DateTime CreatedOn { set; get; }

        /// <summary>
        /// 更新人
        /// </summary>
        [TableMapping(FieldName = "modified_by")]
        public string ModifiedBy { set; get; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [TableMapping(FieldName = "modified_on")]
        public DateTime ModifiedOn { set; get; }

		/// <summary>
		/// 备注
		/// </summary>
		[TableMapping(FieldName = "memo")]
		public string Memo { set; get; }

        /// <summary>
		/// 软件版本
		/// </summary>
		[TableMapping(FieldName = "var_version")]
        public string Var_version { set; get; }

        /// <summary>
		/// 硬件版本
		/// </summary>
		[TableMapping(FieldName = "firmware_version")]
        public string Firmware_version { set; get; }

        /// <summary>
		/// 打印机目前域名
		/// </summary>
		[TableMapping(FieldName = "domain")]
        public string Domain { set; get; }
        
        /// <summary>
        /// 新域名
        /// </summary>
        [TableMapping(FieldName = "new_domain")]
        public string new_Domain { set; get; }
    }
}
