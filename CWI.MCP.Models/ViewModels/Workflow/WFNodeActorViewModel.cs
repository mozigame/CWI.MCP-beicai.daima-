using Evt.Framework.Mvc;

namespace  CWI.MCP.Models
{
    public class WfNodeActorViewModel : ViewModel
    {
        /// <summary>
        /// 工作流节点处理人ID
        /// </summary>
        public string ActorId { get; set; }

        /// <summary>
        /// 工作流节点处理人姓名
        /// </summary>
        public string ActorName { get; set; }
    }
}