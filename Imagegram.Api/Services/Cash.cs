using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Imagegram.Api.Services
{
    public class Cash<TItem>
    {
        private readonly MemoryCache _cache;

        public Cash(IConfiguration configuration)
        {
            _cache = new MemoryCache(new MemoryCacheOptions()
            {
                SizeLimit = configuration.GetValue<int>("InMemoryCashMaxItems")
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
