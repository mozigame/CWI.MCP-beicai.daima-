using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evt.Framework.Common;

namespace  CWI.MCP.Common.ORM
{

    /// <summary>
    /// 条件查询,复杂的条件请用TermCondition类
    /// </summary>
    public class Condition : ICondition
    {
        private const string ParamPreFix = "CWI";
        private string _QueryFieldName;
        private object _Value = null;
        private string _ParamName;
        private OperationType _OpType = OperationType.Equal;
        /// <summary>
        /// 要查询的字段
        /// </summary>
        public string QueryFieldName { set { this._QueryFieldName = value; } get { return this._QueryFieldName; } }
        /// <summary>
        /// 值
        /// </summary>
        public object Value { set { this._Value = value; } get { return this._Value; } }
        /// <summary>
        /// 参数名，如@Name,如果为空表示为@QueryFiledName
        /// </summary>
        public string ParamName { set { this._ParamName = value; } get { return this._ParamName; } }

        /// <summary>
        /// 操作
        /// </summary>
        public OperationType OpType
        {
            set { this._OpType = value; }
            get { return this._OpType; }
        }

        /// <summary>
        /// 条件查询构造函数
        /// </summary>
        /// <param name="qname">要查询的字段名（如：Test）</param>
        /// <param name="pname">要查询参数名（如：@Test）</param>
        /// <param name="value">值</param>
        /// <param name="ot">条件类型</param>
        public Condition(string qname, string pname, object value, OperationType ot)
        {
            this.QueryFieldName = qname;
            this.ParamName = pname;
            this.OpType = ot;
            this.Value = value;
        }

        /// <summary>
        /// 条件查询构造函数
        /// </summary>
        /// <param name="qname">要查询的字段名（如：Test）</param>
        /// <param name="value">值</param>
        /// <param name="ot">条件类型</param>
        public Condition(string qname, object value, OperationType ot)
            : this(qname, null, value, ot)
        {
        }

        /// <summary>
        /// 条件查询构造函数
        /// </summary>
        /// <param name="qname">要查询的字段名（如：Test）</param>
        /// <param name="value">值</param>
        public Condition(string qname, object value)
            : this(qname, null, value, OperationType.Equal)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public ParameterCollection OutParam(DatabaseTypeEnum dataType)
        {
            ParameterCollection pc = new ParameterCollection();
            if (dataType == DatabaseTypeEnum.SqlServer)
                pc.Add(GetParamString(), Value);
            else if (dataType == DatabaseTypeEnum.MySql)
                pc.Add(GetParamString(), Value);
            else
                throw new Exception("无指定数据处理类");
            return pc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public string ToString(DatabaseTypeEnum dataType)
        {
            return ToString(dataType, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public string ToString(DatabaseTypeEnum dataType, Func<string, string> findField)
        {
            string fieldName = QueryFieldName;
            if (findField != null)
            {
                fieldName = findField(fieldName); 
            }

            string strPName = GetParamString();

            switch (_OpType)
            {
                case OperationType.Equal:
                    return fieldName + "=" + strPName;
                case OperationType.GreaterThan:
                    return fieldName + ">" + strPName;
                case OperationType.GreaterThanOrEqual:
                    return fieldName + ">=" + strPName;
                case OperationType.LeftLike:
                    {
                        if (dataType == DatabaseTypeEnum.SqlServer)
                            return fieldName + " LIKE '%'+" + strPName;
                        else if (dataType == DatabaseTypeEnum.MySql)
                            return fieldName + " LIKE CONCAT('%'," + strPName + ")";
                        else
                            throw new Exception("无指定数据处理类");
                    }
                case OperationType.LessThan:
                    return fieldName + "<" + strPName;
                case OperationType.LessThanOrEqual:
                    return fieldName + "<=" + strPName;
                case OperationType.Like:
                    {
                        if (dataType == DatabaseTypeEnum.SqlServer)
                            return fieldName + " LIKE '%'+" + strPName + "+'%'";
                        else if (dataType == DatabaseTypeEnum.MySql)
                            return fieldName + " LIKE CONCAT('%'," + strPName + ",'%')";
                        else
                            throw new Exception("无指定数据处理类");
                    }
                case OperationType.RightLike:
                    {
                        if (dataType == DatabaseTypeEnum.SqlServer)
                            return fieldName + " LIKE " + strPName + "+'%'";
                        else if (dataType == DatabaseTypeEnum.MySql)
                            return fieldName + " LIKE CONCAT(" + strPName + ",'%')";
                        else
                            throw new Exception("无指定数据处理类");
                    }
                case OperationType.In:
                    return fieldName + " IN (" + Value.ToString() + ")";
                case OperationType.NotIn:
                    return fieldName + " NOT IN (" + Value.ToString() + ")";
                case OperationType.NotEqual:
                    return fieldName + "<>" + strPName;
                case OperationType.NotLike:
                    {
                        if (dataType == DatabaseTypeEnum.SqlServer)
                            return fieldName + " NOT LIKE '%'+" + strPName + "+'%'";
                        else if (dataType == DatabaseTypeEnum.MySql)
                            return fieldName + " NOT LIKE CONCAT('%'," + strPName + ",'%')";
                        else
                            throw new Exception("无指定数据处理类");
                    }

                case OperationType.IsNull:
                    return fieldName + " IS NULL";
                case OperationType.IsNotNull:
                    return fieldName + " IS NOT NULL";
                case OperationType.NullOrEmpty:
                    {
                        if (dataType == DatabaseTypeEnum.SqlServer)
                            return " ISNULL(" + fieldName + ",'')=''";
                        else if (dataType == DatabaseTypeEnum.MySql)
                            return " IFNULL(" + fieldName + ",'')=''";
                        else
                            throw new Exception("无指定数据处理类");
                    }
                case OperationType.NotNullOrEmpty:
                    {
                        if (dataType == DatabaseTypeEnum.SqlServer)
                            return " ISNULL(" + fieldName + ",'')<>''";
                        else if (dataType == DatabaseTypeEnum.MySql)
                            return " IFNULL(" + fieldName + ",'')<>''";
                        else
                            throw new Exception("无指定数据处理类");
                    }
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetParamString()
        {
            string strPName = ParamName;
            if (string.IsNullOrEmpty(strPName))
                strPName = "@" + ParamPreFix + QueryFieldName.Trim();
            else
            {
                if (!strPName.Contains("@"))
                    strPName = "@" + ParamPreFix + strPName.Trim();
            }
            return strPName.Replace(".", string.Empty);
        }
    }
}
