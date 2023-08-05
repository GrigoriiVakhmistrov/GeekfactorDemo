using System.Runtime.Caching;
using Microsoft.Extensions.Options;
using TestApp.Contracts.Caching;
using TestApp.Models.Search;
using Route = TestApp.Models.Route.Route;

namespace TestApp.Application.Cache;

public class MemoryRouteCache : IRouteCache
{
    private readonly ObjectCache _cache;
    private readonly CacheItemPolicy _policy;

    public MemoryRouteCache(IOptions<CacheConfiguration> options)
    {
        _cache = MemoryCache.Default;

        _policy = new CacheItemPolicy
        {
            SlidingExpiration = options.Value.Ttl
        };
    }

    public List<Route> GetRoutes(SearchRequest request)
    {
        var cacheKey = GetCacheKey(request);
        return _cache.Get(cacheKey) as List<Route> ?? new List<Route>();
    }

    public void AddRoutes(SearchRequest request, List<Route> filteredRoutes)
    {
        var cacheKey = GetCacheKey(request);
        _cache.Add(cacheKey, filteredRoutes, _policy);
    }

    private static string GetCacheKey(SearchRequest request)
    {
        return $"{request.Origin}-{request.Destination}-{request.OriginDateTime:yyyy-MM-dd}";
    }
}