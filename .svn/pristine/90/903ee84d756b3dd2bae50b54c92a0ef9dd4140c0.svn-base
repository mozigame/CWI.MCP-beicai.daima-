//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/06/04          创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Evt.Framework.Common;
using Evt.Framework.DataAccess;

namespace  CWI.MCP.Common.ORM
{
    /// <summary>
    /// DataAccess扩展,针对Mysql 
    /// </summary>
    /// <typeparam name="T">继承Model的DataModel</typeparam>
    public class DataAccessExtend<T> where T : Model, new()
    {
        public DataManager DBManager { get; private set; }

        private const int MAXFETCHCOUNT = 1000000;

        private Type type;
        private string TableName = String.Empty;
        /// <summary>
        /// 主键字段名
        /// </summary>
        private string PrimaryKeyName = String.Empty;
        /// <summary>
        /// 主键类的属性名
        /// </summary>
        private string PrimaryKeyProperty = string.Empty;
        private TableMappingAttribute Attr = null;
        private Dictionary<string, bool> DicFieldNullable = null;

        /// <summary>
        /// 字段与类型
        /// </summary>
        private Dictionary<string, string> DataFields = new Dictionary<string, string>();
        /// <summary>
        /// 属性与TableMappingAttribute
        /// </summary>
        private Dictionary<string, TableMappingAttribute> DataFieldsTableMapping = new Dictionary<string, TableMappingAttribute>();
        /// <summary>
        /// 属性与字段
        /// </summary>
        private Dictionary<string, string> PropertyFieldsMapping = new Dictionary<string, string>();

        /// <summary>
        /// 设置方法与字段的关系
        /// </summary>
        private Dictionary<string, Action<T, object>> SetMethodFucs = new Dictionary<string, Action<T, object>>();
        private Dictionary<string, Func<T, object>> GetMethodFucs = new Dictionary<string, Func<T, object>>();

        /// <summary>
        /// 表名
        /// </summary>
        public string ReceiveTableName
        {
            get
            {
                return TableName;
            }
        }

        /// <summary>
        /// 主键名
        /// </summary>
        public string ReceivePrimaryKeyName
        {
            get { return PrimaryKeyName; }
        }

        /// <summary>
        /// 得到数据类型
        /// </summary>
        public DatabaseTypeEnum DataProviderType
        {
            get
            {
                return DatabaseTypeEnum.MySql;
            }
        }

        /// <summary>
        /// 属性与字段映射
        /// </summary>
        public Dictionary<string, string> ModelPropertyFieldsMapping
        {
            get
            {
                return PropertyFieldsMapping;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataAccessExtend(DataManager dataManager)
        {
            this.DBManager = dataManager;
            type = typeof(T);
            Attr = Attribute.GetCustomAttribute(type, typeof(TableMappingAttribute)) as TableMappingAttribute;
            if (Attr == null)
                throw new Exception("对象未添加指定特性");
            this.InitFields();
            TableName = Attr.TableName;
        }

        #region "添加对象"
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="_model">对象</param>
        /// <returns></returns>
        public long Create(T _model)
        {
            string strSql = "INSERT INTO " + TableName + "({0}) VALUES({1})";
            List<string> NeedAddField = new List<string>();
            StringBuilder fields = new StringBuilder();
            StringBuilder insertValue = new StringBuilder();
            ParameterCollection listParam = new ParameterCollection();
            foreach (string obj in DataFields.Keys)
            {
                object objValue = GetMethodFucs[obj](_model);

                if (obj == PrimaryKeyName)
                {
                    if (this.GetFieldType(PrimaryKeyName).ToLower().IndexOf("int") > -1)
                    {
                        continue;
                    }
                    else if (this.GetFieldType(PrimaryKeyName).ToLower() == "string")
                    {
                        if (objValue == null)
                        {
                            objValue = Guid.NewGuid().ToString();

                            SetMethodFucs[obj](_model, objValue);
                        }
                    }
                }

                if (objValue is DateTime && ((DateTime)objValue) == DateTime.MinValue) continue;
                fields.AppendFormat(",`{0}`", obj);

                //如果为null且是string类型的,自动修正为string.Empty
                listParam.Add("@" + obj, GetFieldValue(objValue, DataFields[obj]));
                insertValue.AppendFormat(",@{0}", obj);
            }

            if (listParam.Count > 0)
            {
                long num = 0;
                fields.Remove(0, 1);
                insertValue.Remove(0, 1);
                strSql = String.Format(strSql, fields.ToString(), insertValue.ToString());
                if (!string.IsNullOrWhiteSpace(PrimaryKeyName) && this.GetFieldType(PrimaryKeyName).ToLower().IndexOf("int") > -1)
                {
                    strSql += ";SELECT IFNULL(LAST_INSERT_ID(),0);";
                    switch (this.GetFieldType(PrimaryKeyName).ToLower())
                    {
                        case "int64":
                            num = Convert.ToInt64(DBManager.IData.ExecuteScalar(strSql, listParam));
                            break;
                        case "int32":
                            num = Convert.ToInt32(DBManager.IData.ExecuteScalar(strSql, listParam));
                            break;
                        case "int16":
                            num = Convert.ToInt16(DBManager.IData.ExecuteScalar(strSql, listParam));
                            break;
                    }
                    //SetMethodFucs[PrimaryKeyName](_model, num);
                    return num;
                }
                else
                {
                    return DBManager.IData.ExecuteNonQuery(strSql, listParam);
                }
            }
            return 0;
        }

        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="_model">对象</param>
        /// <param name="collect">要添加的列组合</param>
        /// <returns>返回自增量</returns>
        public long Create(T _model, string strCollect)
        {
            string strSql = "INSERT INTO " + TableName + "({0}) VALUES({1})";
            List<string> NeedAddField = new List<string>();
            StringBuilder fields = new StringBuilder();
            StringBuilder insertValue = new StringBuilder();
            ParameterCollection listParam = new ParameterCollection();
            string[] strArrary = strCollect.Split(',');
            List<string> listStr = GetRightFields(strCollect);//过虑掉不存在的字段，不分区大小写

            foreach (string obj in listStr)
            {
                object objValue = type.InvokeMember(GetPropertyNameByField(obj), BindingFlags.GetProperty, null, _model, null);

                if (obj == PrimaryKeyName)
                {
                    if (this.GetFieldType(PrimaryKeyName).ToLower().IndexOf("int") > -1)
                    {
                        continue;
                    }
                    else if (this.GetFieldType(PrimaryKeyName).ToLower() == "string")
                    {
                        if (objValue == null)
                        {
                            objValue = Guid.NewGuid().ToString();
                            type.InvokeMember(GetPropertyNameByField(obj), BindingFlags.SetProperty, null, _model, new object[] { objValue });
                        }
                    }
                }

                if (objValue is DateTime && ((DateTime)objValue) == DateTime.MinValue) continue;
                fields.AppendFormat(",`{0}`", obj);

                //如果为null且是string类型的,自动修正为string.Empty
                listParam.Add("@" + obj, GetFieldValue(objValue, DataFields[obj]));
                insertValue.AppendFormat(",@{0}", obj);
            }

            if (listParam.Count > 0)
            {
                long num = 0;
                fields.Remove(0, 1);
                insertValue.Remove(0, 1);
                strSql = String.Format(strSql, fields.ToString(), insertValue.ToString());
                if (!string.IsNullOrWhiteSpace(PrimaryKeyName) && this.GetFieldType(PrimaryKeyName).ToLower().IndexOf("int") > -1)
                {
                    strSql += ";SELECT IFNULL(LAST_INSERT_ID(),0);";
                    switch (this.GetFieldType(PrimaryKeyName).ToLower())
                    {
                        case "int64":
                            num = Convert.ToInt64(DBManager.IData.ExecuteScalar(strSql, listParam));
                            break;
                        case "int32":
                            num = Convert.ToInt32(DBManager.IData.ExecuteScalar(strSql, listParam));
                            break;
                        case "int16":
                            num = Convert.ToInt16(DBManager.IData.ExecuteScalar(strSql, listParam));
                            break;
                    }

                    type.InvokeMember(GetPropertyNameByField(PrimaryKeyName), BindingFlags.SetProperty, null, _model, new object[] { num });
                    return num;
                }
                else
                {
                    return DBManager.IData.ExecuteNonQuery(strSql, listParam);
                }
            }
            return 0;
        }

        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="list">对象集合</param>
        /// <returns></returns>
        public void Create(List<T> list)
        {
            StringBuilder sb = new StringBuilder();
            ParameterCollection pc = new ParameterCollection();
            int counter = 0;
            foreach (T obj in list)
            {
                counter++;
                ConstructCreateSql(obj, ref sb, ref pc, counter);

                if (counter > 100)
                {
                    DBManager.IData.ExecuteNonQuery(sb.ToString(), pc);
                    sb.Clear();
                    pc.Clear();
                    counter = 0;
                }
            }

            if (sb.Length > 0)
            {
                DBManager.IData.ExecuteNonQuery(sb.ToString(), pc);
                sb.Clear();
                pc.Clear();
            }
        }

        /// <summary>
        /// 构建创建实体的SQL语句
        /// </summary>
        /// <param name="model">实体</param>
        /// <param name="sb">sql</param>
        /// <param name="pc">条件</param>
        /// <param name="flag">标识</param>
        private void ConstructCreateSql(T model, ref StringBuilder sb, ref ParameterCollection pc, int flag)
        {
            string strSql = "INSERT INTO " + TableName + "({0}) VALUES({1});";
            List<string> NeedAddField = new List<string>();
            StringBuilder fields = new StringBuilder();
            StringBuilder insertValue = new StringBuilder();
            ParameterCollection listParam = new ParameterCollection();
            foreach (string obj in DataFields.Keys)
            {
                object objValue = GetMethodFucs[obj](model);

                if (obj == PrimaryKeyName)
                {
                    if (this.GetFieldType(PrimaryKeyName).ToLower().IndexOf("int") > -1)
                    {
                        continue;
                    }
                    else if (this.GetFieldType(PrimaryKeyName).ToLower() == "string")
                    {
                        if (objValue == null)
                        {
                            objValue = Guid.NewGuid().ToString();
                            SetMethodFucs[obj](model, objValue);
                        }
                    }
                }

              //  if (objValue == null) continue;
                if (objValue is DateTime && ((DateTime)objValue) == DateTime.MinValue) continue;
                fields.AppendFormat(",`{0}`", obj);

                //xie.zl 如果为null且是string类型的,自动修正为string.Empty
                //listParam.Add("@" + obj + "_" + flag, objValue);
                listParam.Add("@" + obj + "_" + flag, GetFieldValue(objValue, DataFields[obj]));
                insertValue.AppendFormat(",@{0}_{1}", obj, flag);
            }
            if (listParam.Count > 0)
            {
                fields.Remove(0, 1);
                insertValue.Remove(0, 1);
                strSql = String.Format(strSql, fields.ToString(), insertValue.ToString());
                sb.Append(strSql);
                pc.AddRange(listParam);
            }
        }

        /// <summary>
        /// 得到SQL
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string GetCreateSQL(List<T> list)
        {
            if (list == null || list.Count == 0) return null;

            StringBuilder sb = new StringBuilder();
            ParameterCollection pc = new ParameterCollection();

            ConstructCreateSql(list, ref sb);

            return sb.ToString();
        }

        /// <summary>
        /// 创建SQL语句
        /// </summary>
        /// <param name="ltModel"></param>
        /// <param name="sb"></param>
        private void ConstructCreateSql(List<T> ltModel, ref StringBuilder sb)
        {
            string strSql = "INSERT INTO " + TableName + "({0}) VALUES {1};";
            List<string> NeedAddField = new List<string>();
            StringBuilder fields = new StringBuilder();
            StringBuilder insertValue = new StringBuilder();

            foreach (string obj in DataFields.Keys)
            {
                fields.AppendFormat(",`{0}`", obj);
            }
            fields.Remove(0, 1);

            foreach (var model in ltModel)
            {
                insertValue.Append("(");
                StringBuilder sbTemp = new StringBuilder();
                foreach (string obj in DataFields.Keys)
                {
                    object objValue = GetMethodFucs[obj](model);

                    if (obj == PrimaryKeyName)
                    {
                        if (this.GetFieldType(PrimaryKeyName).ToLower().IndexOf("int") > -1)
                        {
                            continue;
                        }
                        else if (this.GetFieldType(PrimaryKeyName).ToLower() == "string")
                        {
                            if (objValue == null)
                            {
                                objValue = Guid.NewGuid().ToString();
                                SetMethodFucs[obj](model, objValue);
                            }
                        }
                    }

                    if (objValue == null)
                    {
                        sbTemp.AppendFormat(",NULL");
                        continue;
                    }

                    if (objValue is string)
                    {
                        sbTemp.AppendFormat(",'{0}'", CommonUtil.EscapeString(objValue.ToString()));
                    }
                    else if (objValue is DateTime)
                    {
                        sbTemp.AppendFormat(",'{0}'", ((DateTime)objValue).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        sbTemp.AppendFormat(",'{0}'", objValue);
                    }

                }
                sbTemp.Remove(0, 1);
                insertValue.Append(sbTemp);
                insertValue.Append("),");
            }

            strSql = String.Format(strSql, fields.ToString(), insertValue.ToString().TrimEnd(','));
            sb.Append(strSql);
        }
        #endregion

        #region "获取对象"

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要获取的列("*，null,空"取全部)</param>
        /// <returns>返回一个对象</returns>
        public T GetModel(ConditionCollection conditions, string strCollect)
        {
            return GetModel(conditions, strCollect, (string)null);
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <returns>返回一个对象</returns>
        public T GetModel(ConditionCollection conditions)
        {
            return GetModel(conditions, "*", (string)null);
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBy">排序，如order by [字段] desc</param>
        /// <returns>返回一个对象</returns>
        [Obsolete("请使用带OrderByCollection或OrderBy排序参数的方法,此方法下个版本会移除")]
        public T GetModel(ConditionCollection conditions, string strCollect, string orderBy)
        {
            string strWhere = "";
            ParameterCollection listparam = null;
            strWhere = GetQueryWhere(conditions, ref listparam);
            string strFetch = string.IsNullOrEmpty(strCollect) ? "*" : strCollect;
            if (!strFetch.Equals("*"))
            {
                List<string> listFields = GetRightFields(strFetch);
                strFetch = string.Join(",", listFields.ToArray());
            }
            string strSql = "SELECT " + strFetch + " FROM " + TableName + (string.IsNullOrEmpty(strWhere) ? "" : (" WHERE " + strWhere)) + " " + (orderBy == null ? "" : orderBy) + " LIMIT 1 ";

            T _model = null;
            DataTable dt = DBManager.IData.ExecuteDataTable(strSql, listparam);
            if (dt != null && dt.Rows.Count == 1)
                _model = GetModel(dt.Rows[0], strCollect);
            return _model;
        }


        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBy">排序</param>
        /// <returns>返回一个对象</returns>
        public T GetModel(ConditionCollection conditions, string strCollect, OrderBy orderBy)
        {
            return GetModel(conditions, strCollect, CreateOrderByString(orderBy));
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBys">排序</param>
        /// <returns>返回一个对象</returns>
        public T GetModel(ConditionCollection conditions, string strCollect, OrderByCollection orderBys)
        {
            return GetModel(conditions, strCollect, CreateOrderByString(orderBys));
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="num">要取的数量</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBy">排序，如order by [字段] desc</param>
        /// <returns>返回对象列表</returns>
        [Obsolete("请使用带OrderByCollection或OrderBy排序参数的方法,此方法下个版本会移除")]
        private List<T> ListModel(int num, ConditionCollection conditions, string strCollect, string orderBy)
        {
            if (num <= 0) num = MAXFETCHCOUNT;
            string strWhere = "";
            ParameterCollection listparam = null;
            strWhere = GetQueryWhere(conditions, ref listparam);
            string strFetch = string.IsNullOrEmpty(strCollect) ? "*" : strCollect;
            if (!strFetch.Equals("*"))
            {
                List<string> listFields = GetRightFields(strFetch);
                strFetch = string.Join(",", listFields.ToArray());
            }
            string strSql = "SELECT " + strFetch + " FROM " + TableName + (string.IsNullOrEmpty(strWhere) ? "" : (" WHERE " + strWhere)) + " " + (orderBy == null ? "" : orderBy) + " LIMIT " + num.ToString();
            List<T> list = new List<T>();
            DataTable dt = DBManager.IData.ExecuteDataTable(strSql, listparam);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                    list.Add(GetModel(dr, strFetch));
            }
            return list;
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="num">要取的数量</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBy">排序</param>
        /// <returns>返回对象列表</returns>
        public List<T> ListModel(int num, ConditionCollection conditions, string strCollect, OrderBy orderBy)
        {
            return ListModel(num, conditions, strCollect, CreateOrderByString(orderBy));
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="num">要取的数量</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBys">排序</param>
        /// <returns>返回对象列表</returns>
        public List<T> ListModel(int num, ConditionCollection conditions, string strCollect, OrderByCollection orderBys)
        {
            return ListModel(num, conditions, strCollect, CreateOrderByString(orderBys));
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns>返回对象列表</returns>
        public List<T> ListModelBySql(string strSql)
        {
            return ListModelBySql(strSql, null);
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>返回对象列表</returns>
        public List<T> ListModelBySql(string strSql, ParameterCollection parameters)
        {
            return ListModelBySql(strSql, parameters, null);
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <returns>返回对象列表</returns>
        private List<T> ListModelBySql(string strSql, ParameterCollection parameters, string strCollect)
        {
            List<T> list = new List<T>();
            if (String.IsNullOrWhiteSpace(strCollect))
            {
                strCollect = strSql.ToLower().Trim();
                strCollect = Regex.Replace(strCollect, "\\s+", " "); //去掉多余的空格
                int intBegin = 0;
                int intEnd = 0;
                if (strCollect.IndexOf("select ") != -1)
                {
                    intBegin = "select ".Length;
                }
                else
                    intBegin = "select ".Length;
                intEnd = strCollect.IndexOf(" from ");
                strCollect = strCollect.Substring(intBegin, intEnd - intBegin);
                strCollect = Regex.Replace(strCollect, "\\s+", "");
            }

            DataTable dt = DBManager.IData.ExecuteDataTable(strSql, parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                    list.Add(GetModel(dr, strCollect));
            }
            return list;
        }

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="num">要取记录数量</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBy">排序，如order by [字段] desc</param>
        /// <returns>返回DataSet</returns>
        [Obsolete("请使用带OrderByCollection或OrderBy排序参数的方法,此方法下个版本会移除")]
        public DataSet QueryDateSet(int num, ConditionCollection conditions, string strCollect, string orderBy)
        {
            if (num <= 0) num = MAXFETCHCOUNT;
            string strWhere = "";
            ParameterCollection listparam = null;
            strWhere = GetQueryWhere(conditions, ref listparam);
            string strFetch = string.IsNullOrEmpty(strCollect) ? "*" : strCollect;
            if (!strFetch.Equals("*"))
            {
                List<string> listFields = GetRightFields(strFetch);
                strFetch = string.Join(",", listFields.ToArray());
            }

            string strSql = "SELECT " + strFetch + " FROM " + TableName + (string.IsNullOrEmpty(strWhere) ? "" : (" WHERE " + strWhere)) + " " + (orderBy == null ? "" : orderBy) + " LIMIT " + num.ToString();
            return DBManager.IData.ExecuteDataSet(strSql, listparam);
        }

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="num">要取记录数量</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBy">排序</param>
        /// <returns>返回DataSet</returns>
        public DataSet QueryDateSet(int num, ConditionCollection conditions, string strCollect, OrderBy orderBy)
        {
            return QueryDateSet(num, conditions, strCollect, CreateOrderByString(orderBy));
        }

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="num">要取记录数量</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBys">排序</param>
        /// <returns>返回DataSet</returns>
        public DataSet QueryDateSet(int num, ConditionCollection conditions, string strCollect, OrderByCollection orderBys)
        {
            return QueryDateSet(num, conditions, strCollect, CreateOrderByString(orderBys));
        }

        /// <summary>
        /// 分页获取对象列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="orderBy">排序，如order by [字段] desc</param>
        /// <returns>返回对象列表</returns>
        [Obsolete("请使用带OrderByCollection或OrderBy排序参数的方法,此方法下个版本会移除")]
        public List<T> ListModel(int pageIndex, int pageSize, string strCollect, ConditionCollection conditions, string orderBy)
        {
            string strWhere = "";
            ParameterCollection listparam = null;
            strWhere = GetQueryWhere(conditions, ref listparam);
            string strFetch = string.IsNullOrEmpty(strCollect) ? "*" : strCollect;
            if (!strFetch.Equals("*"))
            {
                List<string> listFields = GetRightFields(strFetch);
                strFetch = string.Join(",", listFields.ToArray());
            }
            string strSql = String.Empty;
            strSql = "SELECT {0} FROM {1} {2} {3} LIMIT {4},{5}";
            strSql = String.Format(strSql, strFetch, TableName, (string.IsNullOrEmpty(strWhere) ? "" : "WHERE " + strWhere), orderBy, (pageIndex - 1) * pageSize, pageSize);
            return ListModelBySql(strSql, listparam, strCollect);
        }

        /// <summary>
        /// 分页获取对象列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="orderBy">排序</param>
        /// <returns>返回对象列表</returns>
        public List<T> ListModel(int pageIndex, int pageSize, string strCollect, ConditionCollection conditions, OrderBy orderBy)
        {
            return ListModel(pageIndex, pageSize, strCollect, conditions, CreateOrderByString(orderBy));
        }

        /// <summary>
        /// 分页获取对象列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="orderBys">排序</param>
        /// <returns>返回对象列表</returns>
        public List<T> ListModel(int pageIndex, int pageSize, string strCollect, ConditionCollection conditions, OrderByCollection orderBys)
        {
            return ListModel(pageIndex, pageSize, strCollect, conditions, CreateOrderByString(orderBys));
        }

        /// <summary>
        /// 分页获取对象列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="conditions">查询条件</param>
        /// <returns>返回对象列表</returns>
        public List<T> ListModel(int pageIndex, int pageSize, string strCollect, ConditionCollection conditions)
        {
            return ListModel(pageIndex, pageSize, strCollect, conditions, (string)null);
        }

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBy">排序，如order by [字段] desc</param>
        /// <returns>返回DataSet</returns>
        [Obsolete("请使用带OrderByCollection或OrderBy排序参数的方法,此方法下个版本会移除")]
        public DataSet QueryDateSet(ConditionCollection conditions, string strCollect, string orderBy)
        {
            return QueryDateSet(MAXFETCHCOUNT, conditions, strCollect, orderBy);
        }

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBys">排序</param>
        /// <returns>返回DataSet</returns>
        public DataSet QueryDateSet(ConditionCollection conditions, string strCollect, OrderByCollection orderBys)
        {
            return QueryDateSet(MAXFETCHCOUNT, conditions, strCollect, CreateOrderByString(orderBys));
        }

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="OrderBy">排序</param>
        /// <returns>返回DataSet</returns>
        public DataSet QueryDateSet(ConditionCollection conditions, string strCollect, OrderBy orderBy)
        {
            return QueryDateSet(MAXFETCHCOUNT, conditions, strCollect, CreateOrderByString(orderBy));
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBy">排序，如order by [字段] desc</param>
        /// <returns>返回对象列表</returns>
        [Obsolete("请使用带OrderByCollection或OrderBy排序参数的方法,此方法下个版本会移除")]
        public List<T> ListModel(ConditionCollection conditions, string strCollect, string orderBy)
        {
            return ListModel(MAXFETCHCOUNT, conditions, strCollect, orderBy);
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBys">排序</param>
        /// <returns>返回对象列表</returns>
        public List<T> ListModel(ConditionCollection conditions, string strCollect, OrderByCollection orderBys)
        {
            return ListModel(MAXFETCHCOUNT, conditions, strCollect, CreateOrderByString(orderBys));
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <param name="orderBy">排序</param>
        /// <returns>返回对象列表</returns>
        public List<T> ListModel(ConditionCollection conditions, string strCollect, OrderBy orderBy)
        {
            return ListModel(MAXFETCHCOUNT, conditions, strCollect, CreateOrderByString(orderBy));
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="strCollect">要取的字段("*，null,空"取全部)</param>
        /// <returns>返回对象列表</returns>
        public List<T> ListModel(ConditionCollection conditions, string strCollect)
        {
            return ListModel(MAXFETCHCOUNT, conditions, strCollect, (string)null);
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <returns>返回对象列表</returns>
        public List<T> ListModel(ConditionCollection conditions)
        {
            return ListModel(MAXFETCHCOUNT, conditions, null, (string)null);
        }

        /// <summary>
        /// 列出对象
        /// </summary>
        /// <returns>返回对象列表</returns>
        public List<T> ListModel()
        {
            return ListModel((ConditionCollection)null);
        }

        /// <summary>
        /// 得到分页(可多表)
        /// </summary>
        /// <param name="currentPage">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="conditions">链接的表</param>
        /// <param name="where">条件（不带where）</param>
        /// <param name="orderBy">排序（ORDER By）</param>
        /// <param name="counts">等于0时返回记录数</param>
        /// <param name="fetchFields">字段</param>
        /// <returns>返回DataTable</returns>
        [Obsolete("请使用带OrderByCollection或OrderBy排序参数的方法,此方法下个版本会移除")]
        public DataTable QueryData(int currentPage, int pageSize, string fetchFields, string joinTable, ConditionCollection conditions, string orderBy, ref int counts)
        {
            string strSql = "";
            string strSqlCount = "";
            if (String.IsNullOrEmpty(fetchFields)) fetchFields = "*";
            string strWhere = "";
            ParameterCollection listparam = null;
            strWhere = GetQueryWhere(conditions, ref listparam);
            if (!String.IsNullOrEmpty(strWhere)) strWhere = " WHERE " + strWhere + " ";
            strSql = "SELECT  {0} FROM {1} " + joinTable + " {2} {3}  LIMIT {4},{5}";
            strSql = String.Format(strSql, fetchFields, TableName, strWhere, orderBy, (currentPage - 1) * pageSize, pageSize);
            if (counts != int.MinValue)
            {
                strSqlCount = "SELECT  COUNT(1) FROM " + this.TableName + " " + joinTable + strWhere;
                counts = Convert.ToInt32(DBManager.IData.ExecuteScalar(strSqlCount, listparam));
            }
            DataSet ds = DBManager.IData.ExecuteDataSet(strSql, listparam);
            if (ds != null && ds.Tables.Count == 1)
                return ds.Tables[0];
            return null;
        }

        /// <summary>
        /// 得到分页(可多表)
        /// </summary>
        /// <param name="currentPage">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="conditions">链接的表</param>
        /// <param name="where">条件（不带where）</param>
        /// <param name="orderBys">排序</param>
        /// <param name="counts">等于0时返回记录数</param>
        /// <param name="fetchFields">字段</param>
        /// <returns>返回DataTable</returns>
        public DataTable QueryData(int currentPage, int pageSize, string fetchFields, string joinTable, ConditionCollection conditions, OrderByCollection orderBys, ref int counts)
        {
            return QueryData(currentPage, pageSize, fetchFields, joinTable, conditions, CreateOrderByString(orderBys), ref counts);
        }

        /// <summary>
        /// 得到分页(可多表)
        /// </summary>
        /// <param name="currentPage">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="conditions">链接的表</param>
        /// <param name="where">条件（不带where）</param>
        /// <param name="orderBys">排序</param>
        /// <param name="counts">等于0时返回记录数</param>
        /// <param name="fetchFields">字段</param>
        /// <returns>返回DataTable</returns>
        public DataTable QueryData(int currentPage, int pageSize, string fetchFields, string joinTable, ConditionCollection conditions, OrderBy orderBy, ref int counts)
        {
            return QueryData(currentPage, pageSize, fetchFields, joinTable, conditions, CreateOrderByString(orderBy), ref counts);
        }

        /// <summary>
        /// 得到分页(可多表)
        /// </summary>
        /// <param name="conditions">链接的表</param>
        /// <param name="where">条件（不带where）</param>
        /// <param name="orderBy">排序（ORDER By）</param>
        /// <param name="counts">等于0时返回记录数</param>
        /// <param name="fetchFields">字段</param>
        /// <returns>返回DataTable</returns>
        [Obsolete("请使用带OrderByCollection或OrderBy排序参数的方法,此方法下个版本会移除")]
        public DataTable QueryData(string fetchFields, string joinTable, ConditionCollection conditions, string orderBy)
        {
            int counts = int.MinValue;
            return QueryData(1, MAXFETCHCOUNT, fetchFields, joinTable, conditions, orderBy, ref counts);
        }

        /// <summary>
        /// 得到分页(可多表)
        /// </summary>
        /// <param name="conditions">链接的表</param>
        /// <param name="where">条件（不带where）</param>
        /// <param name="orderBys">排序</param>
        /// <param name="counts">等于0时返回记录数</param>
        /// <param name="fetchFields">字段</param>
        /// <returns>返回DataTable</returns>
        public DataTable QueryData(string fetchFields, string joinTable, ConditionCollection conditions, OrderByCollection orderBys)
        {
            int counts = int.MinValue;
            return QueryData(1, MAXFETCHCOUNT, fetchFields, joinTable, conditions, CreateOrderByString(orderBys), ref counts);
        }

        /// <summary>
        /// 得到分页(可多表)
        /// </summary>
        /// <param name="conditions">链接的表</param>
        /// <param name="where">条件（不带where）</param>
        /// <param name="orderBy">排序</param>
        /// <param name="counts">等于0时返回记录数</param>
        /// <param name="fetchFields">字段</param>
        /// <returns>返回DataTable</returns>
        public DataTable QueryData(string fetchFields, string joinTable, ConditionCollection conditions, OrderBy orderBy)
        {
            int counts = int.MinValue;
            return QueryData(1, MAXFETCHCOUNT, fetchFields, joinTable, conditions, CreateOrderByString(orderBy), ref counts);
        }


        #endregion

        #region "删除操作"
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="conditions">删除条件</param>
        /// <returns>返回受影响的行</returns>
        public int Delete(ConditionCollection conditions)
        {
            string strWhere = "";
            ParameterCollection listparam = null;
            strWhere = GetQueryWhere(conditions, ref listparam);
            string strSql = "DELETE FROM " + TableName + (string.IsNullOrEmpty(strWhere) ? "" : (" WHERE " + strWhere));

            return DBManager.IData.ExecuteNonQuery(strSql, listparam);
        }
        /// <summary>
        /// 删除全部
        /// </summary>
        /// <returns>返回受影响的行</returns>
        public int Delete()
        {
            string strSql = "DELETE FROM " + TableName;
            return DBManager.IData.ExecuteNonQuery(strSql);
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="primaryKey">主键值</param>
        /// <returns>返回受影响的行</returns>
        public int Delete<TKey>(TKey primaryKey)
        {
            if (string.IsNullOrWhiteSpace(PrimaryKeyName))
            {
                throw new Exception(string.Format("表{0}没有定义主键！", TableName));
            }

            if (typeof(TKey).Name.ToLower() != this.GetFieldType(PrimaryKeyName).ToLower())
            {
                throw new ArgumentException("参数类型与主键类型不符合");
            }
            string sql = "DELETE FROM " + TableName + " WHERE " + PrimaryKeyName + "=@key";
            ParameterCollection pc = new ParameterCollection();
            pc.Add("@key", primaryKey);

            return DBManager.IData.ExecuteNonQuery(sql, pc);

        }
        #endregion

        #region "判断是否存在"
        /// <summary>
        /// 判断指定条件的记录是否存在 
        /// </summary>
        /// <param name="conditions">判断条件</param>
        /// <returns>true表示存在，false表示不存在</returns>
        public bool IsExists(ConditionCollection conditions)
        {
            if (conditions == null)
                return true;
            string strWhere = "";
            ParameterCollection listparam = null;
            strWhere = GetQueryWhere(conditions, ref listparam);
            if (string.IsNullOrEmpty(strWhere) || listparam == null)
                return true;
            string strSql = "SELECT COUNT(1) FROM " + TableName + " WHERE " + strWhere;
            return Convert.ToInt32(DBManager.IData.ExecuteScalar(strSql, listparam)) > 0;
        }
        #endregion

        #region "更新对象"
        /// <summary>
        /// 根据主键更新对象
        /// </summary>
        /// <param name="list">要更新对象集合</param>
        public void Update(List<T> list)
        {
            foreach (T obj in list)
            {
                Update(obj);
            }
        }

        /// <summary>
        /// 根据主键更新对象
        /// </summary>
        /// <param name="list">要更新对象集合</param>
        /// <param name="strCollect">必填，要更新的列(field1,fiedl2,field3)，支持实体属性</param>
        public void Update(List<T> list, string strCollect)
        {
            foreach (T obj in list)
            {
                Update(obj, strCollect);
            }
        }

        /// <summary>
        /// 根据主键更新对象
        /// </summary>
        /// <param name="_model">要更新的对象</param>
        /// <returns>返回受影响的行</returns>
        public int Update(T _model)
        {
            string strCollect = String.Empty;
            foreach (string obj in DataFields.Keys)
            {
                strCollect += (strCollect == String.Empty ? "" : ",") + obj;
            }
            return Update(_model, strCollect);
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="_model">要更新的对象</param>
        /// <param name="strCollect">必填，要更新的列(field1,fiedl2,field3)，支持实体属性</param>
        /// <returns>返回受影响的行</returns>
        public int Update(T _model, string strCollect)
        {
            if (string.IsNullOrEmpty(strCollect) || string.IsNullOrEmpty(PrimaryKeyProperty))
                return -1;

            object PkValue = type.InvokeMember(PrimaryKeyProperty, BindingFlags.GetProperty, null, _model, null);
            if (PkValue == null)
                return -1;
            string strWhere = "";
            if (this.GetFieldType(PrimaryKeyName).ToLower().IndexOf("int") > -1 || this.GetFieldType(PrimaryKeyName).ToLower() == "double" || this.GetFieldType(PrimaryKeyName).ToLower() == "decimal")
            {
                strWhere = PrimaryKeyName + "=" + PkValue.ToString();
            }
            else
            {
                strWhere = PrimaryKeyName + "='" + PkValue.ToString() + "'";
            }

            List<string> listStr = GetRightFields(strCollect);//过虑掉不存在的字段，不分区大小写

            string strSql = "UPDATE " + TableName + " {0} WHERE " + strWhere;
            StringBuilder sb = new StringBuilder("");
            ParameterCollection listParam = new ParameterCollection();
            foreach (string obj in listStr)
            {
                var key = GetPropertyNameByField(obj);
                object objValue = type.InvokeMember(key, BindingFlags.GetProperty, null, _model, null);
                if (DicFieldNullable.ContainsKey(key) && !DicFieldNullable[key])
                {
                    if (objValue == null) continue;
                    if (objValue is DateTime && ((DateTime)objValue) == DateTime.MinValue) continue;
                }
                sb.Append(",`" + obj + "`=@" + obj);
                //listParam.Add("@" + obj, objValue == null ? DBNull.Value : objValue);
                listParam.Add("@" + obj, GetFieldValue(objValue, DataFields[obj]));
            }
            sb.Remove(0, 1);
            strSql = String.Format(strSql, " SET " + sb.ToString());
            return DBManager.IData.ExecuteNonQuery(strSql, listParam);
        }

        /// <summary>
        /// 获取字段值
        /// </summary>
        /// <param name="v">值</param>
        /// <param name="vType">值类型</param>
        /// <returns></returns>
        private object GetFieldValue(object v, string vType)
        {
            if (v != null)
            {
                return v;
            }
            else    //为null 特殊处理
            {
                if (string.Equals(vType, "String", StringComparison.InvariantCultureIgnoreCase))
                {
                    return string.Empty;
                }
                else
                {
                    return DBNull.Value;
                }
            }
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="_model">要更新的对象</param>
        /// <param name="conditions">更新条件</param>
        /// <param name="strCollect">要更新的字段，支持实体属性</param>
        /// <returns>返回受影响的行</returns>
        public int Update(T _model, ConditionCollection conditions, string strCollect)
        {
            string[] strArrary = null;
            if (string.IsNullOrEmpty(strCollect))
            {
                strArrary = DataFields.Keys.ToArray();
            }
            else
                strArrary = strCollect.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> listStr = GetRightFields(strArrary);

            string strWhere = "";
            ParameterCollection listparam = null;
            strWhere = GetQueryWhere(conditions, ref listparam);
            string strSql = "UPDATE " + TableName + " {0} " + (string.IsNullOrEmpty(strWhere) ? " " : (" WHERE " + strWhere));
            StringBuilder sb = new StringBuilder("");
            ParameterCollection listparam1 = new ParameterCollection();
            foreach (string obj in listStr)
            {
                var key = GetPropertyNameByField(obj);
                object objValue = type.InvokeMember(key, BindingFlags.GetProperty, null, _model, null);
                if (DicFieldNullable.ContainsKey(key) && !DicFieldNullable[key])
                {
                    if (objValue == null) continue;
                    if (objValue is DateTime && ((DateTime)objValue) == DateTime.MinValue) continue;
                }
                sb.Append(",`" + obj + "`=@" + obj);
                listparam1.Add("@" + obj, GetFieldValue(objValue, DataFields[obj]));
            }
            sb.Remove(0, 1);
            strSql = String.Format(strSql, " SET " + sb.ToString());
            listparam1.AddRange(listparam);
            return DBManager.IData.ExecuteNonQuery(strSql, listparam1);
        }
        #endregion

        #region "转换"
        /// <summary>
        /// DataTable转换成实体类
        /// </summary>
        /// <param name="dt">要转换的DataTable</param>
        /// <returns>返回实体对象</returns>
        public List<T> ListModel(DataTable dt)
        {
            List<T> list = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(GetModel(dr));
            }
            return list;
        }

        /// <summary>
        /// DataRow转换成实体类
        /// </summary>
        /// <param name="dr">要换转的DataRow</param>
        /// <returns>返回实体对象</returns>
        public T GetModel(DataRow dr)
        {
            return GetModel(dr, null);
        }

        /// <summary>
        /// DataRow转换成实体类
        /// </summary>
        /// <param name="dr">要换转的DataRow</param>
        /// <param name="strCollect">要转换的字段</param>
        /// <returns>返回实体对象</returns>
        private T GetModel(DataRow dr, string strCollect)
        {
            T _model = new T();
            List<string> listStr = new List<string>();
            listStr = GetRightFields(strCollect);

            for (int i = listStr.Count - 1; i >= 0; i--)
            {
                if (!dr.Table.Columns.Contains(listStr[i]))
                    listStr.RemoveAt(i);
            }

            foreach (string obj in listStr)
            {
                string _modelPropertyName = GetPropertyNameByField(obj);
                if (!string.IsNullOrEmpty(_modelPropertyName))
                {
                    try
                    {
                        SetMethodFucs[obj](_model, ConvertType(obj, dr[obj], DataFields[obj]));
                        //type.InvokeMember(_modelPropertyName, BindingFlags.SetProperty, null, _model, new object[] { ConvertType(obj, dr[obj], DataFields[obj]) });
                    }
                    catch
                    {
                        SetMethodFucs[obj](_model, ConvertType(obj, DBNull.Value, DataFields[obj]));
                        //type.InvokeMember(_modelPropertyName, BindingFlags.SetProperty, null, _model, new object[] { ConvertType(obj, DBNull.Value, DataFields[obj]) });
                    }
                }
            }
            return _model;
        }
        #endregion

        #region "得到记录数"
        /// <summary>
        /// 得到记录
        /// </summary>
        /// <returns>返回记录数</returns>
        public int Count()
        {
            return Count(null);
        }

        /// <summary>
        /// 根据指定条件获取记录数
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <returns>返回记录数</returns>
        public int Count(ConditionCollection conditions)
        {
            string strWhere = "";
            ParameterCollection listparam = null;
            strWhere = GetQueryWhere(conditions, ref listparam);
            string strSql = "SELECT COUNT(1) FROM " + TableName + (string.IsNullOrEmpty(strWhere) ? "" : " WHERE " + strWhere);
            return Convert.ToInt32(DBManager.IData.ExecuteScalar(strSql, listparam));
        }

        /// <summary>
        /// 得到去掉指定重复列的记录
        /// </summary>
        /// <param name="strField">指定需要去掉重复的字段</param>
        /// <returns>返回记录数</returns>
        public int GetDistinctCount(string strField)
        {
            return GetDistinctCount(strField, null);
        }

        /// <summary>
        /// 得到去掉指定重复列的记录
        /// </summary>
        /// <param name="strField">指定需要去掉重复的字段</param>
        /// <param name="query">条件</param>
        /// <returns>返回记录数</returns>
        public int GetDistinctCount(string strField, ConditionCollection query)
        {
            string strWhere = "";
            ParameterCollection listparam = null;
            strWhere = GetQueryWhere(query, ref listparam);
            string strSql = "SELECT COUNT(DISTINCT " + strField + ") FROM " + TableName + (string.IsNullOrEmpty(strWhere) ? "" : " WHERE " + strWhere);
            return Convert.ToInt32(DBManager.IData.ExecuteScalar(strSql, listparam));

        }
        #endregion

        #region "其它操作"
        /// <summary>
        /// 聚合
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="q">查询条件</param>
        /// <returns>出错时返回-1，其它返回聚合结果</returns>
        public object Sum(string field, ConditionCollection q)
        {
            bool exists = false;
            List<string> list = GetRightFields(field);
            if (list.Count == 1)
            {
                exists = true;
                field = list[0];
            }
            if (exists)
            {
                ParameterCollection listparam = null;
                string strWhere = GetQueryWhere(q, ref listparam);
                string strSql = "SELECT SUM(" + field + ") FROM " + TableName + (string.IsNullOrEmpty(strWhere) ? "" : " WHERE " + strWhere);
                object obj = DBManager.IData.ExecuteScalar(strSql, listparam);
                if (obj == null || Convert.IsDBNull(obj))
                    return -1;
                return obj;
            }
            return -1;
        }

        /// <summary>
        /// 聚合
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>出错时返回-1，其它返回聚合结果</returns>
        public object Sum(string field)
        {
            bool exists = false;
            List<string> list = GetRightFields(field);
            if (list.Count == 1)
            {
                exists = true;
                field = list[0];
            }
            if (exists)
            {
                string strSql = "SELECT SUM(" + field + ") FROM " + TableName;
                object obj = DBManager.IData.ExecuteScalar(strSql);
                if (obj == null || Convert.IsDBNull(obj))
                    return -1;
                return obj;
            }
            return -1;
        }

        /// <summary>
        /// 最大值
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>出错时返回-1，其它返回最大值</returns>
        public object Max(string field)
        {
            bool exists = false;
            List<string> list = GetRightFields(field);
            if (list.Count == 1)
            {
                exists = true;
                field = list[0];
            }
            if (exists)
            {
                string strSql = "SELECT MAX(" + field + ") FROM " + TableName;
                object obj = DBManager.IData.ExecuteScalar(strSql);
                if (obj == null || Convert.IsDBNull(obj))
                    return -1;
                return obj;
            }
            return -1;
        }

        /// <summary>
        /// 最大值
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="q">查询条件</param>
        /// <returns>出错时返回-1，其它返回最大值</returns>
        public object Max(string field, ConditionCollection q)
        {
            bool exists = false;
            List<string> list = GetRightFields(field);
            if (list.Count == 1)
            {
                exists = true;
                field = list[0];
            }
            if (exists)
            {
                ParameterCollection listparam = null;
                string strWhere = GetQueryWhere(q, ref listparam);
                string strSql = "SELECT MAX(" + field + ") FROM " + TableName + (string.IsNullOrEmpty(strWhere) ? "" : " WHERE " + strWhere);
                object obj = DBManager.IData.ExecuteScalar(strSql, listparam);
                if (obj == null || Convert.IsDBNull(obj))
                    return -1;
                return obj;
            }
            return -1;
        }

        /// <summary>
        /// 最小值
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>出错时返回-1，其它返回最小值</returns>
        public object Min(string field)
        {
            bool exists = false;
            List<string> list = GetRightFields(field);
            if (list.Count == 1)
            {
                exists = true;
                field = list[0];
            }
            if (exists)
            {
                string strSql = "SELECT MIN(" + field + ") FROM " + TableName;
                object obj = DBManager.IData.ExecuteScalar(strSql);
                if (obj == null || Convert.IsDBNull(obj))
                    return -1;
                return obj;
            }
            return -1;
        }

        /// <summary>
        /// 最小值
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="q">查询条件</param>
        /// <returns>出错时返回-1，其它返回最小值</returns>
        public object Min(string field, ConditionCollection q)
        {
            bool exists = false;
            List<string> list = GetRightFields(field);
            if (list.Count == 1)
            {
                exists = true;
                field = list[0];
            }
            if (exists)
            {
                ParameterCollection listparam = null;
                string strWhere = GetQueryWhere(q, ref listparam);
                string strSql = "SELECT MIN(" + field + ") FROM " + TableName + (string.IsNullOrEmpty(strWhere) ? "" : " WHERE " + strWhere);
                object obj = DBManager.IData.ExecuteScalar(strSql, listparam);
                if (obj == null || Convert.IsDBNull(obj))
                    return -1;
                return obj;
            }
            return -1;
        }

        /// <summary>
        /// 平均值
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>出错时返回-1，其它返回平均值</returns>
        public object Avg(string field)
        {
            bool exists = false;
            List<string> list = GetRightFields(field);
            if (list.Count == 1)
            {
                exists = true;
                field = list[0];
            }
            if (exists)
            {
                string strSql = "SELECT AVG(" + field + ") FROM " + TableName;
                object obj = DBManager.IData.ExecuteScalar(strSql);
                if (obj == null || Convert.IsDBNull(obj))
                    return -1;
                return obj;
            }
            return -1;
        }

        /// <summary>
        /// 平均值
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="q">查询条件</param>
        /// <returns>出错时返回-1，其它返回平均值</returns>
        public object Avg(string field, ConditionCollection q)
        {
            bool exists = false;
            List<string> list = GetRightFields(field);
            if (list.Count == 1)
            {
                exists = true;
                field = list[0];
            }
            if (exists)
            {
                ParameterCollection listparam = null;
                string strWhere = GetQueryWhere(q, ref listparam);
                string strSql = "SELECT AVG(" + field + ") FROM " + TableName + (string.IsNullOrEmpty(strWhere) ? "" : " WHERE " + strWhere);
                object obj = DBManager.IData.ExecuteScalar(strSql);
                if (obj == null || Convert.IsDBNull(obj))
                    return -1;
                return obj;
            }
            return -1;
        }
        #endregion

        #region "私有方法"
        /// <summary>
        /// 获取字段和类型
        /// </summary>
        private void InitFields()
        {
            DicFieldNullable = new Dictionary<string, bool>();
            foreach (PropertyInfo obj in type.GetProperties())
            {
                TableMappingAttribute attr = Attribute.GetCustomAttribute(obj, typeof(TableMappingAttribute)) as TableMappingAttribute;
                if (attr != null)
                {
                    if (attr.PrimaryKey)
                    {
                        PrimaryKeyName = attr.FieldName;
                        PrimaryKeyProperty = obj.Name;
                    }
                    if (obj.PropertyType.IsGenericType && obj.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        DataFields.Add(attr.FieldName, obj.PropertyType.GetGenericArguments()[0].Name);
                        DataFieldsTableMapping.Add(obj.Name, attr);
                        PropertyFieldsMapping.Add(obj.Name, attr.FieldName);
                        DicFieldNullable.Add(obj.Name, true);
                    }
                    else
                    {
                        DataFields.Add(attr.FieldName, obj.PropertyType.Name);
                        DataFieldsTableMapping.Add(obj.Name, attr);
                        PropertyFieldsMapping.Add(obj.Name, attr.FieldName);
                        DicFieldNullable.Add(obj.Name, false);
                    }
                }
            }

            foreach (var key in PropertyFieldsMapping.Keys)
            {
                PropertyInfo propertyInfo = type.GetProperty(key);
                MethodInfo setName = propertyInfo.GetSetMethod();
                Action<T, object> setFunc = ReflectionHelper.CreateDelegate<T>(setName);
                SetMethodFucs.Add(PropertyFieldsMapping[key], setFunc);

                PropertyInfo propertyInfoGet = type.GetProperty(key);
                MethodInfo getName = propertyInfoGet.GetGetMethod();
                Func<T, object> getFunc = ReflectionHelper.CreateGetDelegate<T>(getName);
                GetMethodFucs.Add(PropertyFieldsMapping[key], getFunc);

            }
        }

        /// <summary>
        /// 把OrderByCollection 转成 SQL
        /// </summary>
        /// <param name="orderBys">排序</param>
        /// <returns>返回排序SQL</returns>
        private string CreateOrderByString(OrderByCollection orderBys)
        {
            string strOrderBy = string.Empty;
            if (orderBys != null && orderBys.Count > 0)
            {
                strOrderBy = " ORDER BY ";
                List<string> list = new List<string>();
                foreach (var item in orderBys)
                {
                    string fieldName = item.FieldName;
                    string sort = item.SortType == SortTypeEnum.Asc ? "ASC" : "DESC";
                    if (!fieldName.Contains(".") && !fieldName.Contains(","))
                    {
                        List<string> fields = GetRightFields(fieldName);
                        if (fields.Count == 1)
                        {
                            fieldName = string.Format("`{0}`", fields[0]);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    list.Add(string.Format("{0} {1}", fieldName, sort));
                }

                if (list.Count > 0)
                {
                    return strOrderBy += string.Join(",", list.ToArray());
                }
            }
            return string.Empty;
        }


        /// <summary>
        /// 把OrderBy 转成 SQL
        /// </summary>
        /// <param name="orderBys">排序</param>
        /// <returns>返回排序SQL</returns>
        private string CreateOrderByString(OrderBy orderBy)
        {
            if (orderBy != null)
            {
                return CreateOrderByString(new OrderByCollection { orderBy });
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取正确的字段
        /// </summary>
        /// <param name="fieldName">字段或属性名称</param>
        /// <returns>返回正确的字段</returns>
        private string GetRightField(string fieldName)
        {
            if (!fieldName.Contains(".") && !fieldName.Contains(","))
            {
                List<string> fields = GetRightFields(fieldName);
                if (fields.Count == 1)
                {
                    fieldName = string.Format("`{0}`", fields[0]);
                }
            }

            return fieldName;
        }

        /// <summary>
        /// 得到正确的字段
        /// </summary>
        /// <param name="strCollect">字段（支持实体属性）</param>
        /// <returns>返回数据库字段</returns>
        private List<string> GetRightFields(string strCollect)
        {
            if (string.IsNullOrWhiteSpace(strCollect) || strCollect.IndexOf("*") > -1)
            {
                List<string> list = new List<string>();
                foreach (string obj in DataFields.Keys)
                    list.Add(obj);
                return list;
            }

            string[] strArrary = strCollect.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> listFields = new List<string>();
            foreach (var str in strArrary)
            {
                listFields.Add(str.Trim());
            }
            return GetRightFields(listFields.ToArray());
        }

        /// <summary>
        /// 得到正确的字段
        /// </summary>
        /// <param name="strArrary">字段（支持实体属性）</param>
        /// <returns>返回数据库字段</returns>
        private List<string> GetRightFields(string[] strArrary)
        {
            if (strArrary != null && strArrary.Length == 1)
            {
                if (PropertyFieldsMapping.ContainsKey(strArrary[0]))
                {
                    return new List<string>() { PropertyFieldsMapping[strArrary[0]] };
                }

                return new List<string>() { strArrary[0] };
            }

            List<string> listStr = new List<string>();//过虑掉不存在的字段，不分区大小写
            //获取数据库的字段
            var list = from str in strArrary
                       from obj in DataFields.Keys
                       where str.ToLower() == obj.ToLower()
                       select obj;
            //根据实现属性获取数据库字段
            listStr = list.ToList();

            List<string> listO = strArrary.ToList();
            foreach (var str in listStr)
            {
                listO.Remove(str);
            }

            foreach (var str in listO)
            {
                if (PropertyFieldsMapping.ContainsKey(str))
                {
                    if (!listStr.Contains(PropertyFieldsMapping[str]))
                    {
                        listStr.Add(PropertyFieldsMapping[str]);
                    }
                }
            }

            return listStr;
        }

        /// <summary>
        /// 根据字段获取属性
        /// </summary>
        /// <param name="field">字段（支持实体属性）</param>
        /// <returns>返回数据库字段</returns>
        private string GetPropertyNameByField(string field)
        {
            KeyValuePair<string, string> keyValue = PropertyFieldsMapping.First(data => data.Value == field);
            if (!keyValue.Equals(null))
                return keyValue.Key;
            return String.Empty;
        }

        /// <summary>
        /// 得到字段类型
        /// </summary>
        /// <param name="strFieldName">字段名</param>
        /// <returns>返回字段名</returns>
        private string GetFieldType(string strFieldName)
        {
            return DataFields[strFieldName];
        }

        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="original">原始值</param>
        /// <param name="valueType">值类型</param>
        /// <returns>返回值</returns>
        private object ConvertType(string fieldName, object original, string valueType)
        {
            string strValueType = valueType.ToLower();
            if (Convert.IsDBNull(original))
            {
                if (DicFieldNullable.ContainsKey(fieldName) && DicFieldNullable[fieldName]) return null;
                switch (strValueType)
                {
                    case "string":
                        return String.Empty;
                    case "int16":
                        return 0;
                    case "int32":
                        return 0;
                    case "int64":
                        return 0;
                    case "datetime":
                        return DateTime.MinValue;
                    case "boolean":
                        return false;
                    case "double":
                        return (double)0.0;
                    case "decimal":
                        return (decimal)0.0;
                    case "single":
                        return 0;
                    default:
                        return null;
                }
            }
            switch (strValueType)
            {
                case "string":
                    return Convert.ToString(original);
                case "int32":
                    return Convert.ToInt32(original);
                case "int64":
                    return Convert.ToInt64(original);
                case "datetime":
                    return Convert.ToDateTime(original);
                case "boolean":
                    return Convert.ToBoolean(original);
                case "double":
                    return Convert.ToDouble(original);
                case "decimal":
                    return Convert.ToDecimal(original);
                case "single":
                    return Convert.ToSingle(original);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="listparams">参数</param>
        /// <returns>返回条件字符串</returns>
        private string GetQueryWhere(ConditionCollection conditions, ref ParameterCollection listparams)
        {
            if (listparams == null) listparams = new ParameterCollection();
            string strWhere = "";
            if (conditions != null)
            {
                ParameterCollection _list = conditions.GetParams(DataProviderType);
                string _strWhere = conditions.QueryToString(DataProviderType, GetRightField);
                if (_list != null && !string.IsNullOrEmpty(_strWhere))
                {
                    strWhere = _strWhere;
                    listparams.AddRange(_list);
                }
            }
            return strWhere;
        }
        #endregion

    }

    #region 委托
    internal static class ReflectionHelper
    {
        public static Action<T, object> CreateDelegate<T>(MethodInfo method) where T : class
        {
            MethodInfo genericHelper = typeof(ReflectionHelper).GetMethod("CreateDelegate",
            BindingFlags.Static | BindingFlags.NonPublic);
            //由于创建符合Set方法的委托需要获取Set方法的参数类型，因此通过反射实现
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod
            (typeof(T), method.GetParameters()[0].ParameterType);

            object ret = constructedHelper.Invoke(null, new object[] { method });

            return (Action<T, object>)ret;
        }

        static Action<TTarget, object> CreateDelegate<TTarget, TParam>(MethodInfo method) where TTarget : class
        {
            Action<TTarget, TParam> func = (Action<TTarget, TParam>)Delegate.CreateDelegate
                (typeof(Action<TTarget, TParam>), method);

            Action<TTarget, object> ret = (TTarget target, object param) => func(target, (TParam)param);
            return ret;
        }

        public static Func<TTarget, object> CreateGetDelegate<TTarget>(MethodInfo method) where TTarget : class
        {
            MethodInfo genericHelper = typeof(ReflectionHelper).GetMethod("CreateGetDelegate",
            BindingFlags.Static | BindingFlags.NonPublic);

            MethodInfo constructedHelper = genericHelper.MakeGenericMethod
            (typeof(TTarget), method.ReturnType);
            try
            {
                object ret = constructedHelper.Invoke(null, new object[] { method });
                return (Func<TTarget, object>)ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static Func<TTarget, object> CreateGetDelegate<TTarget, TResult>(MethodInfo method) where TTarget : class
        {
            Func<TTarget, TResult> func = (Func<TTarget, TResult>)Delegate.CreateDelegate
                (typeof(Func<TTarget, TResult>), method);

            Func<TTarget, object> ret = (TTarget target) => func(target);
            return ret;
        }
    }
    #endregion
}
