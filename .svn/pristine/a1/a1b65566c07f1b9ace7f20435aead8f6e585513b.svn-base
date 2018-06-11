using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evt.Framework.Common;
using System.Collections;



namespace  CWI.MCP.Common.ORM
{
    /// <summary>
    /// 条件集合
    /// </summary>
    public class ConditionCollection : IEnumerable
    {
        private List<ConditionsClause> _Conditions = null;

        /// <summary>
        /// 条件
        /// </summary>
        internal List<ConditionsClause> Conditions { set { this._Conditions = value; } get { return this._Conditions; } }

        /// <summary>
        /// 条件个数
        /// </summary>
        public int ConditionCount
        {
            get
            {
                return GetConditionsCounts(this);
            }
        }

        private List<SqlQueryClause> _Query = null;
        /// <summary>
        /// 条件
        /// </summary>
        internal List<SqlQueryClause> Query { set { this._Query = value; } get { return this._Query; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="jt"></param>
        public ConditionCollection(ICondition condition, Clause jt)
        {
            if (this.Conditions == null)
                this.Conditions = new List<ConditionsClause>();
            this.Conditions.Add(new ConditionsClause(condition, jt));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        public ConditionCollection(ICondition condition)
        {
            if (this.Conditions == null)
                this.Conditions = new List<ConditionsClause>();
            this.Conditions.Add(new ConditionsClause(condition, Clause.And));
        }

        /// <summary>
        /// 
        /// </summary>
        public ConditionCollection() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <param name="jt"></param>
        public ConditionCollection(ConditionCollection q, Clause jt)
        {
            if (this.Query == null)
                this.Query = new List<SqlQueryClause>();
            this.Query.Add(new SqlQueryClause(q, jt));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <param name="jt"></param>
        public void Add(ConditionCollection q, Clause jt)
        {
            if (this.Query == null)
                this.Query = new List<SqlQueryClause>();
            this.Query.Add(new SqlQueryClause(q, jt));
        }

        /// <summary>
        /// 默认逻辑为and运算
        /// </summary>
        /// <param name="q">条件</param>
        public void Add(ConditionCollection q)
        {
            Add(q, Clause.And);
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="condition">Condition,TermCondition类</param>
        /// <param name="jt">条件符</param>
        public void Add(ICondition condition, Clause jt)
        {
            if (this.Conditions == null)
                this.Conditions = new List<ConditionsClause>();
            this.Conditions.Add(new ConditionsClause(condition, jt));
        }

        /// <summary>
        /// 默认逻辑为and运算
        /// </summary>
        /// <param name="condition">Condition,TermCondition类</param>
        public void Add(ICondition condition)
        {
            Add(condition, Clause.And);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public ParameterCollection GetParams(DatabaseTypeEnum dataType)
        {
            ParameterCollection list = new ParameterCollection();
            List<ConditionsClause> listConditions = GetConditions(this);
            foreach (ConditionsClause p in listConditions)
            {
                if (p.Condition is Condition)
                {
                    Condition c = (Condition)p.Condition;
                    OperationType ot = c.OpType;
                    if (ot != OperationType.IsNull && ot != OperationType.In && ot != OperationType.NotIn && ot != OperationType.IsNotNull && ot != OperationType.NullOrEmpty && ot != OperationType.NotNullOrEmpty) //不需要参数的过滤掉
                        list.AddRange(c.OutParam(dataType));
                }
                else if (p.Condition is TermCondition)
                {
                    TermCondition t = (TermCondition)p.Condition;
                    if (t.DataParam != null)
                        list.AddRange(t.DataParam);
                }
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public string QueryToString(DatabaseTypeEnum dataType, Func<string, string> findField)
        {
            string strWhere = GetConditionsString(this, dataType, findField);
            string strNewWhere = "";
            //过滤前后多余的条件符
            if (strWhere.Trim().StartsWith("AND") || strWhere.Trim().StartsWith("OR"))
            {
                if (strWhere.Trim().StartsWith("AND"))
                    strNewWhere = strWhere.Remove(0, " AND".Length);
                if (strWhere.Trim().StartsWith("OR"))
                    strNewWhere = strWhere.Remove(0, " OR".Length);
                strWhere = strNewWhere;
            }
            if (strWhere.Trim().EndsWith("AND") || strWhere.Trim().EndsWith("OR"))
            {
                if (strWhere.Trim().EndsWith("AND"))
                    strNewWhere = strWhere.Remove(strWhere.Length - "AND ".Length);
                if (strWhere.Trim().EndsWith("OR"))
                    strNewWhere = strWhere.Remove(strWhere.Length - "OR ".Length);
                strWhere = strNewWhere;
            }
            return strWhere;
        }

        /// <summary>
        /// 
        /// </summary>
        internal class ConditionsClause
        {
            private ICondition _Condition = null;
            private Clause jt = Clause.And;

            /// <summary>
            /// 条件
            /// </summary>
            public ICondition Condition { set { this._Condition = value; } get { return this._Condition; } }

            /// <summary>
            /// 条件拼接
            /// </summary>
            public Clause Jt { set { this.jt = value; } get { return this.jt; } }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="c">条件</param>
            /// <param name="j">条件拼接符</param>
            public ConditionsClause(ICondition c, Clause j)
            {
                this.Condition = c;
                this.Jt = j;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal class SqlQueryClause
        {
            private ConditionCollection _Query = null;
            private Clause jt = Clause.And;

            /// <summary>
            /// 条件
            /// </summary>
            public ConditionCollection Query { set { this._Query = value; } get { return this._Query; } }

            /// <summary>
            /// 条件拼接
            /// </summary>
            public Clause Jt { set { this.jt = value; } get { return this.jt; } }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="q">条件</param>
            /// <param name="j">条件拼接符</param>
            public SqlQueryClause(ConditionCollection q, Clause j)
            {
                this.Query = q;
                this.Jt = j;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private static int GetConditionsCounts(ConditionCollection q)
        {
            if (q == null)
                return 0;
            int counter = 0;
            counter = (q.Conditions == null ? 0 : q.Conditions.Count);
            List<SqlQueryClause> qlist = q.Query;
            if (qlist != null)
            {
                foreach (SqlQueryClause _q in qlist)
                {
                    counter += GetConditionsCounts(_q.Query);
                }
            }
            return counter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private static List<ConditionsClause> GetConditions(ConditionCollection q)
        {
            if (q == null)
                return new List<ConditionsClause>();
            List<SqlQueryClause> qlist = q.Query;
            List<ConditionsClause> list = new List<ConditionsClause>();
            if (q.Conditions != null)
            {
                list.AddRange(q.Conditions);
            }
            if (qlist != null)
            {
                foreach (SqlQueryClause _q in qlist)
                {
                    list.AddRange(GetConditions(_q.Query));
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private static string GetConditionsString(ConditionCollection q, DatabaseTypeEnum dataType, Func<string, string> findField)
        {
            if (q == null)
                return String.Empty;
            StringBuilder sbSqlWhere = new StringBuilder();
            if (q.Conditions != null)
            {
                sbSqlWhere.Append(" (");
                for (int i = 0; i < q.Conditions.Count; i++)
                {
                    if (q.Conditions[i].Jt == Clause.And)
                    {
                        sbSqlWhere.Append(" AND ");
                        if (q.Conditions[i].Condition is Condition)
                        {
                            sbSqlWhere.Append(((Condition)q.Conditions[i].Condition).ToString(dataType, findField));
                        }
                        else
                        {
                            sbSqlWhere.Append(q.Conditions[i].Condition.ToString(dataType));
                        }
                    }
                }
                for (int i = 0; i < q.Conditions.Count; i++)
                {
                    if (q.Conditions[i].Jt == Clause.Or)
                    {
                        sbSqlWhere.Append(" OR ");
                        if (q.Conditions[i].Condition is Condition)
                        {
                            sbSqlWhere.Append(((Condition)q.Conditions[i].Condition).ToString(dataType, findField));
                        }
                        else
                        {
                            sbSqlWhere.Append(q.Conditions[i].Condition.ToString(dataType));
                        }
                    }
                }
                sbSqlWhere.Append(") ");
                sbSqlWhere.Replace("( AND ", "(").Replace("( OR ", "(");
            }
            List<SqlQueryClause> qlist = q.Query;
            if (qlist != null)
            {
                foreach (SqlQueryClause _q in qlist)
                {
                    if (_q.Query != null)
                    {
                        if (_q.Query.Conditions != null && _q.Query.Conditions.Count > 0)
                        {
                            if (_q.Jt == Clause.And)
                                sbSqlWhere.Append(" AND ");
                            else if (_q.Jt == Clause.Or)
                                sbSqlWhere.Append(" OR ");

                        }
                        sbSqlWhere.Append(GetConditionsString(_q.Query, dataType, findField));
                    }

                }
            }

            return sbSqlWhere.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return GetEnumerator(this);
        }

        private static IEnumerator GetEnumerator(ConditionCollection q)
        {
            if (q == null)
            {
                yield break;
            }

            if (q.Conditions != null)
            {
                foreach (var c in q.Conditions)
                {
                    yield return c;
                }
            }

            List<SqlQueryClause> qlist = q.Query;
            if (qlist != null)
            {
                foreach (SqlQueryClause _q in qlist)
                {
                    GetEnumerator(_q.Query);
                }
            }
        }
    }

    /// <summary>
    /// 条件符枚举
    /// </summary>
    public enum Clause
    {
        /// <summary>
        /// and
        /// </summary>
        And,
        /// <summary>
        /// or
        /// </summary>
        Or
    }

    /// <summary>
    /// 条件类型枚举
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// like　%str%
        /// </summary>
        Like,
        /// <summary>
        /// like %str
        /// </summary>
        LeftLike,
        /// <summary>
        /// like str%
        /// </summary>
        RightLike,
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 大于等于
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// In
        /// </summary>
        In,
        /// <summary>
        /// Not In
        /// </summary>
        NotIn,
        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual,
        /// <summary>
        /// not like
        /// </summary>
        NotLike,
        /// <summary>
        /// 为空
        /// </summary>
        IsNull,
        /// <summary>
        /// 不为空
        /// </summary>
        IsNotNull,
        /// <summary>
        /// null OR 空
        /// </summary>
        NullOrEmpty,
        /// <summary>
        /// 不为空
        /// </summary>
        NotNullOrEmpty
    }
}
