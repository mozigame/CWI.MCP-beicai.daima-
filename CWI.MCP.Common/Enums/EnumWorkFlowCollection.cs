using CWI.MCP.Common.Attributes;

namespace CWI.MCP.Common
{
    /// <summary>
    /// 工作流业务类型
    /// </summary>
    public enum WFBusinessType
    {
        /// <summary>
        /// 未指定
        /// </summary>
        [EnumDescription("未指定")]
        All = 0,

        /// <summary>
        /// 商家结算
        /// </summary>
        [EnumDescription("商家结算")]
        Selttlement 
    }

    /// <summary>
    /// 工作流记录是否通过
    /// </summary>
    public enum WFIsPass
    {
        /// <summary>
        /// 退回（未通过）
        /// </summary>
        [EnumDescription("退回")]
        Back = 0,

        /// <summary>
        /// 通过
        /// </summary>
        [EnumDescription("通过")]
        Pass
    }

    /// <summary>
    /// 工作流记录是否通过
    /// </summary>
    public enum WFActionType
    {
        /// <summary>
        /// 新增暂存
        /// </summary>
        NewSave = 0,

        /// <summary>
        /// 新增提交
        /// </summary>
        NewSubmit,

        /// <summary>
        /// 编辑暂存
        /// </summary>
        EditSave,

        /// <summary>
        /// 编辑提交
        /// </summary>
        EditSubmit,

        /// <summary>
        /// 重走流程提交
        /// </summary>
        FinishedSubmit
    }
}
