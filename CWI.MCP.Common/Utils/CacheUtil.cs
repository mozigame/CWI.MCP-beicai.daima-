//版权信息：版权所有(C) 2015，COOLWI.COM
//变更历史：
//    姓名         日期          说明
//--------------------------------------------------------
//   王军锋     2012/03/31       创建

using System;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 缓存工具类。
    /// </summary>
    public static class CacheUtil
    {
        /// <summary>
        /// ObjectCache
        /// </summary>
        private static ObjectCache _cache = null;

        /// <summary>
        /// CacheUtil
        /// </summary>
        static CacheUtil()
        {
            if (_cache == null)
            {
                _cache = MemoryCache.Default;
            }
        }

        /// <summary>
        /// 获取指定键的缓存数据。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回object类型的值，使用时需要显示转换。</returns>
        public static object Get(string key)
        {
            try
            {
                return GetCacheValue(key);
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 获取指定键的缓存数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="key">键。</param>
        /// <returns>类型为T的值。</returns>
        public static T Get<T>(string key)
        {
            try
            {
                return (T)GetCacheValue(key);
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex.Message);
            }

            return default(T);
        }

        /// <summary>
        /// 保存数据到缓存。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <returns>保存是否成功。</returns>
        public static bool Set(string key, object value)
        {
            try
            {
                SetCacheValue(key, value);
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 移除指定键的缓存数据。
        /// </summary>
        /// <param name="key">键。</param>
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }

        #region 私有方法

        /// <summary>
        /// 分析缓存Key。
        /// </summary>
        /// <param name="key">缓存Key。</param>
        /// <param name="useMemcached">是否使用Memcached。</param>
        /// <param name="useWebCache">是否使用Web Cache。</param>
        /// <param name="useBothCache">是否同时使用Memcached和Web Cache。</param>
        /// <param name="cacheTimeout">缓存过期时间。</param>
        /// <param name="versionKey">缓存版本Key。</param>
        private static void AnalyzeCacheKey(string key, out bool useMemcached, out bool useWebCache, out bool useBothCache, out int cacheTimeout, out string versionKey)
        {
            string[] keyParts = key.ToLower().Split('_');
            if (keyParts == null || keyParts.Length < 2)
            {
                throw new ArgumentException("传入的Cache Key格式不正确。", "key", null);
            }

            string cacheType;
            string lastPart = keyParts[keyParts.Length - 1];
            if (Regex.IsMatch(lastPart, @"^\d+$")) //如果Key中最后一段为数字，则表明设置了缓存的过期时间。
            {
                cacheTimeout = Convert.ToInt32(lastPart);
                cacheType = keyParts[keyParts.Length - 2];
            }
            else
            {
                cacheTimeout = ConfigUtil.CacheTimeout; //如果Key不包含过期时间，则从配置文件读取默认过期时间。
                cacheType = lastPart;
            }

            switch (cacheType)
            {
                case "w": //"w"表示保存在Web Cache中
                    useMemcached = false;
                    useWebCache = true;
                    useBothCache = false;
                    break;
                case "m": //"m"表示保存在Memcached中
                    useMemcached = true;
                    useWebCache = false;
                    useBothCache = false;
                    break;
                case "wm": //"wm"或者"mw"表示同时保存标识在Web Cache和Memcached中，并将数据保存在Web Cache中
                case "mw":
                    useMemcached = true;
                    useWebCache = true;
                    useBothCache = true;
                    break;
                default: //其他值则说明没有设置缓存类型，将其设置为保存在Memcached中
                    useMemcached = true;
                    useWebCache = false;
                    useBothCache = false;
                    break;
            }

            versionKey = key + "_id";
        }

        /// <summary>
        /// 获取缓存数据。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>缓存数据。</returns>
        private static object GetCacheValue(string key)
        {
            return _cache.Get(key);
        }

        /// <summary>
        /// 设置缓存数据。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">要缓存的数据。</param>
        /// <returns>缓存是否成功。</returns>
        private static bool SetCacheValue(string key, object value)
        {
            bool useMemcached, useWebCache, useBothCache;
            int cacheTimeout;
            string versionKey;

            AnalyzeCacheKey(key, out useMemcached, out useWebCache, out useBothCache, out cacheTimeout, out versionKey);
            _cache.Set(key, value, DateTime.Now.AddSeconds(cacheTimeout));

            return true;
        }

        #endregion
    }
}
