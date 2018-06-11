using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evt.Framework.Common;

namespace  CWI.MCP.Common.ORM
{
    /// <summary>
    /// 自定义条件
    /// </summary>
    public class TermCondition : ICondition
    {
        private ParameterCollection _DataParam;
        /// <summary>
        /// 参数，需赋值
        /// </summary>
        public ParameterCollection DataParam
        {
            get { return _DataParam; }
            set { this._DataParam = value; }
        }

        private string _SqlQueryString;
        /// <summary>
        /// 查询条件（如"Name=@Name"）
        /// </summary>
        public string SqlQueryString
        {
            get { return _SqlQueryString; }
            set { this._SqlQueryString = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlQueryString"></param>
        /// <param name="param"></param>
        public TermCondition(string sqlQueryString, ParameterCollection param)
        {
            this.SqlQueryString = sqlQueryString;
            this._DataParam = param;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlQueryString"></param>
        public TermCondition(string sqlQueryString)
        {
            this.SqlQueryString = sqlQueryString;
            this._DataParam = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlQueryString"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        public TermCondition(string sqlQueryString, string paramName, object paramValue)
        {
            if (paramValue != null)
            {
                if (DataParam == null) DataParam = new ParameterCollection();
                DataParam.Add(paramName, paramValue);
            }
            this.SqlQueryString = sqlQueryString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.SqlQueryString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public string ToString(DatabaseTypeEnum dataType)
        {
            return ToString();
        }
    }
}
