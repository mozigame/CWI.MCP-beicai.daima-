using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  CWI.MCP.Models
{
    /// <summary>
    /// 分页结果集
    /// </summary>
    [Serializable]
    public class PagingResult<T>
    {
        /// <summary>
        /// 总行数
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 记录集
        /// </summary>
        public T Data { get; set; }
    }
}
