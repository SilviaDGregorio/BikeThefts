using BikeThefts.DataAccess.Interfaces;
using BikeThefts.DataAccess.Settings;
using BikeThefts.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;

namespace BikeThefts.DataAccess
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        public CacheService(IMemoryCache cache, IOptions<CacheSettings> options)
        {
            _cache = cache;
            _cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(options.Value.ExpirationMinutes));
        }

        public T GetCache<T>(Filters key)
        {
            _cache.TryGetValue(GetCacheKey(key), out T cacheEntry);
            return cacheEntry;
        }

        public void SetCache<T>(Filters key, T cacheEntry)
        {
            _cache.Set(GetCacheKey(key), cacheEntry, _cacheEntryOptions);
        }

        private string GetCacheKey(Filters key)
        {
            return key.Location + "." + key.Distance;
        }
    }
}
