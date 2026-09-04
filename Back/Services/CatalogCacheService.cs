using Back.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace Back.Services
{
    public class CatalogCacheService
    {
        private const string CacheKey = "public_catalog";
        private static readonly TimeSpan Duration = TimeSpan.FromSeconds(60);
        private readonly IMemoryCache _cache;

        public CatalogCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool TryGet(out CatalogDto? catalog)
        {
            return _cache.TryGetValue(CacheKey, out catalog);
        }

        public void Set(CatalogDto catalog)
        {
            _cache.Set(CacheKey, catalog, Duration);
        }

        public void Invalidate()
        {
            _cache.Remove(CacheKey);
        }
    }
}
