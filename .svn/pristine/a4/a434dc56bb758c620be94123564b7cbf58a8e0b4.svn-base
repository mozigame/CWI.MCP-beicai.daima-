//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/16        创建
//---------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Evt.Framework.DataAccess;
using Evt.Framework.Common;
using CWI.MCP.Models;
using CWI.MCP.Common;
using CWI.MCP.Common.ORM;
using System.Runtime.Caching;

namespace CWI.MCP.Services
{
    /// <summary>
    /// Service基类
    /// </summary>
    public class BaseService
    {
        #region DataAccessExtend

        /// <summary>
        /// 数据操作字典
        /// </summary>
        private static Dictionary<string, object> dic = new Dictionary<string, object>();

        /// <summary>
        /// 锁
        /// </summary>
        private static object locker = new object();

        /// <summary>
        /// 获取数据操作实体
        /// </summary>
        /// <typeparam name="T">DataModel实体</typeparam>
        /// <returns>返回实体的数据库操作</returns>
        internal DataAccessExtend<T> GetRepository<T>() where T : Model, new()
        {
            var tempK = typeof(T).ToString();
            if (!dic.ContainsKey(tempK))
            {
                lock (locker)
                {
                    if (!dic.ContainsKey(tempK))
                    {
                        try
                        {
                            var tempRet = new DataAccessExtend<T>(DbUtil.DataManager.Current);
                            dic.Add(tempK, tempRet);
                        }
                        catch { }
                    }
                }
            }

            return dic[tempK] as DataAccessExtend<T>;
        }

        /// <summary>
        /// 获取数据操作实体
        /// </summary>
        /// <typeparam name="T">DataModel实体</typeparam>
        /// <returns>返回实体的数据库操作</returns>
        internal DataAccessExtend<T> GetRepository<T>(DataManager dm) where T : Model, new()
        {
            var tempK = typeof(T).ToString();
            if (!dic.ContainsKey(tempK))
            {
                lock (locker)
                {
                    if (!dic.ContainsKey(tempK))
                    {
                        try
                        {
                            var tempRet = new DataAccessExtend<T>(dm);
                            dic.Add(tempK, tempRet);
                        }
                        catch { }
                    }
                }
            }

            return dic[tempK] as DataAccessExtend<T>;
        }

        #endregion

        #region 公用校验

        //public void CheckDataIsRefrenced<T>(T m) where T : Model, new()
        //{
        //    var attr = Attribute.GetCustomAttribute(m.GetType(), typeof(TableMappingAttribute)) as TableMappingAttribute;
        //    if (attr == null)
        //    {
        //        throw new Exception("对象未添加指定特性");
        //    }
        //    var tableName = attr.TableName;
        //    ConditionCollection cc = new ConditionCollection();
        //    cc.Add(new Condition("source_table", tableName));

        //    var modelList = this.GetRepository<OptTableRefrenceRelationsInfo>().ListModel(cc);
        //    var fieldMappingDic = this.GetRepository<T>().ModelPropertyFieldsMapping;
        //    modelList.ForEach(o =>
        //    {
        //        var sourceTableFieldList = new List<string>();
        //        var targetTableFieldList = new List<string>();
        //        if (o.SourceTableField.Contains(Consts.SPLIT_CHAR_COMMA))//联合主键
        //        {
        //            sourceTableFieldList = o.SourceTableField.Split(Consts.SPLIT_CHAR_COMMA).ToList();
        //        }
        //        else
        //        {
        //            sourceTableFieldList.Add(o.SourceTableField);
        //        }

        //        if (o.TargetTableField.Contains(Consts.SPLIT_CHAR_COMMA))//联合主键
        //        {
        //            targetTableFieldList = o.TargetTableField.Split(Consts.SPLIT_CHAR_COMMA).ToList();
        //        }
        //        else
        //        {
        //            targetTableFieldList.Add(o.TargetTableField);
        //        }

        //        var fieldList = new List<KeyValuePair<string, string>>();
        //        sourceTableFieldList.ForEach(s =>
        //        {
        //            var field = fieldMappingDic.FirstOrDefault(d => d.Value == s);
        //            if (string.IsNullOrWhiteSpace(field.Key))
        //            {
        //                return;
        //            }
        //            fieldList.Add(field);
        //        });

        //        if (sourceTableFieldList.Count != targetTableFieldList.Count || fieldList.Count != targetTableFieldList.Count)
        //        {
        //            throw new BusinessException("基础表引用关系错误，请联系管理员！");
        //        }

        //        string sql = string.Empty;
        //        var where = new List<string>();
        //        for (int k = 0; k < targetTableFieldList.Count; k++)
        //        {
        //            where.Add(string.Format(@"{0} = '{1}'", targetTableFieldList[k], m.GetValue(fieldList[k].Key)));
        //        }
        //        sql = string.Format(@"SELECT 1 FROM {0} WHERE {1} {2};", o.TargetTable, string.Join(" AND ", where), o.IsIncludeStatus == (int)LogicType.Yes ? "AND status_code = 1" : string.Empty);
        //        var obj = DbUtil.DataManager.Current.IData.ExecuteScalar(sql);
        //        if (obj != DBNull.Value && obj != null && obj.ToString() == "1")
        //        {
        //            throw new BusinessException(string.Format(@"数据已被{0}使用，不能进行当前操作！", o.TargetTableDescription));
        //        }
        //    });
        //}

        #endregion

        #region 缓存管理

        protected ObjectCache _cache = MemoryCache.Default;
        private static object _lockCache = new object();

        /// <summary>
        /// 缓存键值
        /// </summary>
        protected virtual string CacheKey
        {
            private set;
            get;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        protected void RemoveCache()
        {
            CheckCacheKey();
            RemoveCache(CacheKey);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        protected void RemoveCache(string key)
        {
            RemoveCache(key, false);
        }

        /// <summary>
        /// 移除所有缓存
        /// </summary>
        protected void RemoveAllCache()
        {
            var keys = _cache.ToList();
            foreach (var k in keys)
            {
                _cache.Remove(k.Key);
            }
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="isStartKey">是否只是起始值</param>
        protected void RemoveCache(string key, bool isStartKey)
        {
            CheckCacheKey(key);
            lock (_lockCache)
            {
                if (!isStartKey)
                {
                    _cache.Remove(key);
                }
                else
                {
                    var keys = _cache.ToList();
                    foreach (var k in keys)
                    {
                        if (k.Key.StartsWith(key))
                        {
                            _cache.Remove(k.Key);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 从缓存中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetFromCache<T>(string key) where T : new()
        {
            CheckCacheKey(key);
            if (!_cache.Contains(key))
            {
                return default(T);
            }

            return (T)_cache[key];
        }

        /// <summary>
        /// 从缓存中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetFromCache<T>() where T : new()
        {
            CheckCacheKey();
            return GetFromCache<T>(CacheKey);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="value"></param>
        protected void SetCacheValue(object value)
        {
            CheckCacheKey();
            lock (_lockCache)
            {
                _cache[CacheKey] = value;
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        protected void SetCacheValue(string key, object value)
        {
            CheckCacheKey(key);
            lock (_lockCache)
            {
                _cache[key] = value;
            }
        }

        /// <summary>
        /// 检查缓存键是否有效
        /// </summary>
        private void CheckCacheKey()
        {
            CheckCacheKey(CacheKey);
        }

        /// <summary>
        ///  检查缓存键是否有效
        /// </summary>
        /// <param name="key"></param>
        private void CheckCacheKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception("未定义缓存键值");
            }
        }

        #endregion
    }
}