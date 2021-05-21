using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Imagegram.Api.Services
{
    public class Cache<TItem>
    {
        private readonly MemoryCache _cache;

        public Cache(IConfiguration configuration)
        {
            _cache = new MemoryCache(new MemoryCacheOptions()
            {
                SizeLimit = configuration.GetValue<int>("InMemoryCacheMaxItems")
            });
        }                

        public async ValueTask<TItem> GetOrCreate(object key, Func<Task<TItem>> createItem)
        {
            if (!_cache.TryGetValue(key, out TItem cacheEntry))
            {
                cacheEntry = await createItem();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetPriority(CacheItemPriority.High);
                
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}
