using BikeThefts.Domain.Entities;
using BikeThefts.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace BikeThefts.DataAccess
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T GetCache<T>(Filters key)
        {
            _cache.TryGetValue(GetCacheKey(key), out T cacheEntry);
            return cacheEntry;
        }

        public void SetCache<T>(Filters key, T cacheEntry)
        {
            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            // Save data in cache.
            _cache.Set(GetCacheKey(key), cacheEntry, cacheEntryOptions);
        }
        private string GetCacheKey(Filters key)
        {
            return key.Location + key.Distance;
        }
    }
}
