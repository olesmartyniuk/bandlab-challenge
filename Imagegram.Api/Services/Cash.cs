using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Imagegram.Api.Services
{
    public class Cash<TItem>
    {
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions()
        {
            SizeLimit = 1024
        });

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
