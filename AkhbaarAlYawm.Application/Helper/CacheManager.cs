using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AkhbaarAlYawm.Application.Helper.CacheManager
{
    public class CacheManager
    {
        private static CacheType m_CachingEngine = CacheType.NotSpecified;

        public static CacheType CachingEngine { get { return m_CachingEngine; } set { m_CachingEngine = value; } }

        private static void checkCacheEngine()
        {
            if (m_CachingEngine == CacheType.NotSpecified)
            {
                string cacheEngine = System.Configuration.ConfigurationManager.AppSettings["cacheEngine"];
                if (string.IsNullOrEmpty(cacheEngine))
                {
                    m_CachingEngine = CacheType.TypedDictionaryCache;
                }
                else
                {
                    cacheEngine = cacheEngine.ToLower();
                    switch (cacheEngine)
                    {
                        case "aspnethttpruntimecache":
                            m_CachingEngine = CacheType.AspNetHttpRuntimeCache;
                            break;
                        case "aspnetsystemruntimecache":
                            m_CachingEngine = CacheType.AspNetSystemRuntimeCache;
                            break;
                        case "none":
                            m_CachingEngine = CacheType.None;
                            break;
                        case "typeddictionarycache":
                            m_CachingEngine = CacheType.TypedDictionaryCache;
                            break;
                        default:
                            m_CachingEngine = CacheType.AspNetSystemRuntimeCache;
                            break;
                    }
                }
            }
        }

        public static object Get(string aKey)
        {
            try
            {
                checkCacheEngine();

                switch (m_CachingEngine)
                {
                    case CacheType.None:
                        return null;
                    case CacheType.AspNetHttpRuntimeCache:
                        return HttpRuntime.Cache.Get(aKey);
                    case CacheType.TypedDictionaryCache:
                        System.Diagnostics.Debug.WriteLine("use GetFromTDCache");
                        // use GetFromTDCache ;
                        return null;
                    case CacheType.AspNetSystemRuntimeCache:
                        ObjectCache cache = MemoryCache.Default;
                        if (cache != null)
                        {
                            return cache.Get(aKey);
                        }
                        else
                        {
                            return null;
                        }
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static void Remove(string aKey)
        {
            try
            {
                checkCacheEngine();

                switch (m_CachingEngine)
                {
                    case CacheType.AspNetHttpRuntimeCache:
                        HttpRuntime.Cache.Remove(aKey);
                        break;
                    case CacheType.AspNetSystemRuntimeCache:
                        ObjectCache cache = MemoryCache.Default;
                        if (cache != null)
                        {
                            cache.Remove(aKey);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public static void Set(string aKey, object aValue)
        {
            checkCacheEngine();

            switch (m_CachingEngine)
            {
                case CacheType.None:
                    break;
                case CacheType.AspNetHttpRuntimeCache:
                    HttpRuntime.Cache.Insert(aKey, aValue);
                    break;
                case CacheType.TypedDictionaryCache:
                    System.Diagnostics.Debug.WriteLine("use AddToTDCache");
                    break;

                case CacheType.AspNetSystemRuntimeCache:
                    CacheItemPolicy policy = new CacheItemPolicy();
                    //policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(10.0);
                    ObjectCache cache = MemoryCache.Default;
                    if (cache != null)
                    {
                        cache.Set(aKey, aValue, policy);
                    }
                    break;
            }
        }

        public static void Set(string aKey, object aValue, DateTime aWhenToExpire)
        {
            checkCacheEngine();

            switch (m_CachingEngine)
            {
                case CacheType.None:
                    break;
                case CacheType.AspNetHttpRuntimeCache:
                    HttpRuntime.Cache.Insert(aKey, aValue, null,
                        aWhenToExpire,
                        System.Web.Caching.Cache.NoSlidingExpiration,
                        System.Web.Caching.CacheItemPriority.Normal,
                        null);
                    break;
                case CacheType.TypedDictionaryCache:
                    System.Diagnostics.Debug.WriteLine("use AddToTDCache");
                    break;
                case CacheType.AspNetSystemRuntimeCache:
                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.Priority = CacheItemPriority.Default;
                    policy.AbsoluteExpiration = aWhenToExpire;
                    ObjectCache cache = MemoryCache.Default;

                    if (cache != null)
                    {
                        cache.Set(aKey, aValue, policy);
                    }
                    break;
            }
        }

        public static void Set(string aKey, object aValue, TimeSpan aExpiryDuration)
        {
            checkCacheEngine();

            switch (m_CachingEngine)
            {
                case CacheType.None:
                    break;
                case CacheType.AspNetHttpRuntimeCache:
                    HttpRuntime.Cache.Insert(aKey, aValue, null,
                        DateTime.Now.Add(aExpiryDuration),
                        System.Web.Caching.Cache.NoSlidingExpiration,
                        System.Web.Caching.CacheItemPriority.Normal,
                        null);
                    break;
                case CacheType.TypedDictionaryCache:
                    System.Diagnostics.Debug.WriteLine("use AddToTDCache");
                    break;
                case CacheType.AspNetSystemRuntimeCache:
                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.Priority = CacheItemPriority.Default;
                    policy.AbsoluteExpiration = DateTime.Now.Add(aExpiryDuration);
                    ObjectCache cache = MemoryCache.Default;

                    if (cache != null)
                    {
                        cache.Set(aKey, aValue, policy);
                    }
                    break;
            }
        }

        #region Typed_Typed_Dictionary_section
        //    // Typed Typed Dictionary section
        //    private static Object thisLock = new Object();

        //    public static T Get<T>(string aKey)
        //    {
        //        var retVal = default(T);
        //        checkCacheEngine();
        //        if (m_CachingEngine == CacheType.TypedDictionaryCache)
        //        {
        //            if (TDCacheManager<T>.cache.ContainsKey(aKey))
        //            {
        //                TypedDictionaryValue<T> tdv = TDCacheManager<T>.cache[aKey];
        //                if (tdv.expiryTime >= DateTime.Now)
        //                {
        //                    return tdv.ObjValue;
        //                }
        //            }
        //        }

        //        return retVal;
        //    }

        //    public static void Remove<T>(string aKey)
        //    {
        //        checkCacheEngine();
        //        if (m_CachingEngine == CacheType.TypedDictionaryCache)
        //        {
        //            if (TDCacheManager<T>.cache.ContainsKey(aKey))
        //            {
        //                TDCacheManager<T>.cache.Remove(aKey);
        //            }
        //        }
        //    }

        //    public static void Set<T>(string aKey, T aValue)
        //    {
        //        TypedDictionaryValue<T> tdv = new TypedDictionaryValue<T>();
        //        tdv.ObjValue = aValue;
        //        tdv.expiryTime = DateTime.MaxValue;
        //        AddToCache(aKey, tdv);
        //    }

        //    public static void Set<T>(string aKey, T aValue, TimeSpan aExpiryDuration)
        //    {
        //        TypedDictionaryValue<T> tdv = new TypedDictionaryValue<T>();
        //        tdv.ObjValue = aValue;
        //        tdv.expiryTime = DateTime.Now.Add(aExpiryDuration);
        //        AddToCache(aKey, tdv);
        //    }

        //    public static void Set<T>(string aKey, T aValue, DateTime aWhenToExpire)
        //    {
        //        TypedDictionaryValue<T> tdv = new TypedDictionaryValue<T>();
        //        tdv.ObjValue = aValue;
        //        tdv.expiryTime = aWhenToExpire;
        //        AddToCache(aKey, tdv);
        //    }

        //    private static void AddToCache<T>(string aKey, TypedDictionaryValue<T> aValue)
        //    {
        //        checkCacheEngine();
        //        if (m_CachingEngine == CacheType.TypedDictionaryCache)
        //        {
        //            lock (thisLock)
        //            {
        //                TDCacheManager<T>.cache[aKey] = aValue;
        //            }
        //        }
        //    }

        #endregion

    }

    public enum CacheType
    {
        NotSpecified,
        None,
        AspNetHttpRuntimeCache,
        AspNetSystemRuntimeCache,
        TypedDictionaryCache
    }

    //struct TypedDictionaryValue<T>
    //{
    //    public T ObjValue { get; set; }
    //    public DateTime expiryTime { get; set; }
    //}

    //static class TDCacheManager<T>
    //{
    //    internal static readonly Dictionary<string, TypedDictionaryValue<T>> cache = new Dictionary<string, TypedDictionaryValue<T>>();
    //}
}
