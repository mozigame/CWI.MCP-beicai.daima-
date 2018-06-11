using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWI.MCP.Common
{
    /// <summary>
    ///  微信指定创建菜单结构
    /// </summary>
    [Serializable]
    public class Button
    {
        public string type { get; set; }

        public string name { get; set; }

        public string key { get; set; }

        public string url { get; set; }

        public List<Button> sub_button { get; set; }
    }
}
