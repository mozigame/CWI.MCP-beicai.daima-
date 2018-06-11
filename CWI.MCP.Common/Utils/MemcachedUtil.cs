//版权信息：版权所有(C) 2015，COOLWI.COM
//变更历史：
//    姓名         日期          说明
//--------------------------------------------------------
//   技术部     2012/11/14       创建

//第三方组件版本：Enyim.Caching         v1.2.11.0
//                MemcachedProviders    v1.2.4416.26484

using System;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using MemcachedProviders.Cache;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 缓存工具类。
    /// </summary>
    public class MemCacheUtil
    {
        /// <summary>
        /// ObjectCache
        /// </summary>
        private static ObjectCache _cache = null;

        /// <summary>
        /// CacheUtil
        /// </summary>
        static MemCacheUtil()
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
                LogUtil.Error("CacheUtil: " + ex.Message, ex);
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
                LogUtil.Error("CacheUtil: " + ex.Message, ex);
            }

            return default(T);
        }

        /// <summary>
        /// 异步保存数据到缓存。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <returns>保存是否成功。</returns>
        public static bool Set(string key, object value)
        {
            return Set(key, value, false);
        }

        /// <summary>
        /// 使用指定方式（同步或异步）保存数据到缓存。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="sync">是否使用同步操作缓存数据。</param>
        /// <returns>保存是否成功。</returns>
        public static bool Set(string key, object value, bool sync)
        {
            try
            {
                SetCacheValue(key, value, sync);
            }
            catch (Exception ex)
            {
                LogUtil.Error("CacheUtil: " + ex.Message, ex);
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
            DistCache.Remove(key);
        }

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
            //e.g. key="180_nm"
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
                case "n": //"n"表示保存在Web Cache中
                    useMemcached = false;
                    useWebCache = true;
                    useBothCache = false;
                    break;
                case "m": //"m"表示保存在Memcached中
                    useMemcached = true;
                    useWebCache = false;
                    useBothCache = false;
                    break;
                case "nm": //"nm"或者"mn"表示同时保存标识在Web Cache和Memcached中
                case "mn":
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
            object value = null;
            bool useMemcached, useWebCache, useBothCache;
            int cacheTimeout;
            string versionKey;

            AnalyzeCacheKey(key, out useMemcached, out useWebCache, out useBothCache, out cacheTimeout, out versionKey);

            if (useBothCache) //使用双缓存
            {
                object dotNetCacheVersion = _cache.Get(versionKey);
                object memcachedVersion = DistCache.Get(versionKey);

                //如果Web Cache和Memcached中的缓存版本不相等，则表示缓存的数据已发生变化，需重新从数据库读取并更新缓存
                if (dotNetCacheVersion == null || memcachedVersion == null || dotNetCacheVersion.ToString() != memcachedVersion.ToString())
                {
                    value = null;
                }
                else
                {
                    value = _cache.Get(key);
                }
            }
            else if (useWebCache)
            {
                value = _cache.Get(key);
            }
            else if (useMemcached)
            {
                value = DistCache.Get(key);
            }

            return value;
        }

        /// <summary>
        /// 设置缓存数据。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">要缓存的数据。</param>
        /// <param name="sync">是否使用同步操作缓存数据。</param>
        /// <returns>缓存是否成功。</returns>
        private static void SetCacheValue(string key, object value, bool sync)
        {
            bool useMemcached, useWebCache, useBothCache;
            int cacheTimeout;
            string versionKey;
            string version = Guid.NewGuid().ToString();

            AnalyzeCacheKey(key, out useMemcached, out useWebCache, out useBothCache, out cacheTimeout, out versionKey);

            if (useBothCache) //使用双缓存
            {
                //保存缓存版本至Memcached
                if (sync)
                {
                    SetMemcachedValue(versionKey, version, cacheTimeout);
                }
                else
                {
                    SetMemcachedDataDelegate setVersionDelegate = new SetMemcachedDataDelegate(SetMemcachedValue);
                    setVersionDelegate.BeginInvoke(versionKey, version, cacheTimeout, null, null);
                }

                //保存缓存版本和数据至Web Cache
                CacheItemPolicy policy = new CacheItemPolicy();
                if (cacheTimeout <= 0)
                {
                    policy.AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration;
                }
                else
                {
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(cacheTimeout);
                }
                _cache.Set(versionKey, version, policy);
                _cache.Set(key, value, policy);
            }
            else if (useWebCache) //使用Web Cache
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                if (cacheTimeout <= 0)
                {
                    policy.AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration;
                }
                else
                {
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(cacheTimeout);
                }

                _cache.Set(key, value, policy);
            }
            else if (useMemcached) //使用Memcached
            {
                if (sync)
                {
                    SetMemcachedValue(key, value, cacheTimeout);
                }
                else
                {
                    SetMemcachedDataDelegate setVersionDelegate = new SetMemcachedDataDelegate(SetMemcachedValue);
                    setVersionDelegate.BeginInvoke(key, value, cacheTimeout, null, null);
                }
            }
        }

        /// <summary>
        /// 保存缓存数据至Memcached。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">要缓存的数据。</param>
        /// <param name="cacheTimeout">缓存过期时间。</param>
        /// <returns>缓存是否成功。</returns>
        private static bool SetMemcachedValue(string key, object value, int cacheTimeout)
        {
            bool success = false;
            int tryCount = ConfigUtil.MemcachedTryCount;

            //Memcached Set/Get数据超过2s的，系统记录警告信息
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    if (cacheTimeout <= 0)
                    {
                        success = DistCache.Add(key, value);
                    }
                    else
                    {
                        success = DistCache.Add(key, value, cacheTimeout * 1000);
                    }

                    if (success)
                    {
                        break;
                    }

                    LogUtil.Error(String.Format("第【{0}】次尝试向Memcached保存Key为【{1}】的数据失败！", i, key));
                }
                catch (Exception ex)
                {
                    LogUtil.Error(String.Format("第【{0}】次尝试向Memcached保存Key为【{1}】的数据失败，{2}", i, key, ex.Message), ex);
                }
            }

            //Memcached Set/Get数据超过2s的，系统记录警告信息
            if (sw != null)
            {
                sw.Stop();

                long duration = sw.ElapsedMilliseconds / 1000;

                if (duration > ConfigUtil.WarnDuration)
                {
                    LogUtil.Warn(String.Format("向Memcached保存Key为【{0}】的数据的时间超过【{1}】s！", key, ConfigUtil.WarnDuration));
                }

                sw = null;
            }


            return success;
        }

        /// <summary>
        /// 用于向Memcached异步保存数据的委托。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="cacheTimeout">缓存过期时间。</param>
        /// <returns>保存是否成功。</returns>
        internal delegate bool SetMemcachedDataDelegate(string key, object value, int cacheTimeout);
    }
}
