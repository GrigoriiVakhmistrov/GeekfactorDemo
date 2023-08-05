using Microsoft.Extensions.Options;
using TestApp.Application.Cache;
using TestApp.Models.Route;
using TestApp.Models.Search;

namespace TestApp.Tests.Caching;

public class MemoryRouteCacheTests
{
    [Fact]
    public void AddRoutesAddsRoutesToCache()
    {
        // Arrange
        var cache = new MemoryRouteCache(Options.Create(new CacheConfiguration
        {
            Ttl = TimeSpan.FromHours(1)
        }));
        var routes = new List<Route>
        {
            new()
            {
                Id = Guid.NewGuid(), Origin = "A", Destination = "B", OriginDateTime = DateTime.Now,
                DestinationDateTime = DateTime.Now.AddHours(1), Price = 100, TimeLimit = DateTime.Now.AddHours(2)
            },
            new()
            {
                Id = Guid.NewGuid(), Origin = "B", Destination = "C", OriginDateTime = DateTime.Now,
                DestinationDateTime = DateTime.Now.AddHours(2), Price = 200, TimeLimit = DateTime.Now.AddHours(3)
            }
        };
        var request = new SearchRequest {Origin = "A", Destination = "C", OriginDateTime = DateTime.Now};

        // Act
        cache.AddRoutes(request, routes);

        // Assert
        Assert.Equal(routes.Count, cache.GetRoutes(request).Count);
        Assert.Equal(routes[0], cache.GetRoutes(request).First(t => t.Id == routes[0].Id));
        Assert.Equal(routes[1], cache.GetRoutes(request).First(t => t.Id == routes[1].Id));
    }

    [Fact]
    public void TryGetReturnsTrueIfRouteIsInCache()
    {
        // Arrange
        var cache = new MemoryRouteCache(Options.Create(new CacheConfiguration
        {
            Ttl = TimeSpan.FromHours(1)
        }));
        var route = new Route
        {
            Id = Guid.NewGuid(), Origin = "A", Destination = "B", OriginDateTime = DateTime.Now,
            DestinationDateTime = DateTime.Now.AddHours(1), Price = 100, TimeLimit = DateTime.Now.AddHours(2)
        };
        var request = new SearchRequest {Origin = "A", Destination = "B", OriginDateTime = DateTime.Now};

        cache.AddRoutes(request, new List<Route> {route});

        // Act
        var result = cache.GetRoutes(request);

        // Assert
        Assert.True(result.Count > 0);
        Assert.Equal(route.Destination, result[0].Destination);
        Assert.Equal(route.Origin, result[0].Origin);
        Assert.Equal(route.DestinationDateTime, result[0].DestinationDateTime);
        Assert.Equal(route.OriginDateTime, result[0].OriginDateTime);
        Assert.Equal(route.Price, result[0].Price);
        Assert.Equal(route.TimeLimit, result[0].TimeLimit);
        Assert.Equal(route.Id, result[0].Id);
    }

    [Fact]
    public void TryGetReturnsFalseIfRouteIsNotInCache()
    {
        // Arrange
        var cache = new MemoryRouteCache(Options.Create(new CacheConfiguration
        {
            Ttl = TimeSpan.FromHours(1)
        }));
        var request = new SearchRequest {Origin = "A", Destination = "B", OriginDateTime = DateTime.Now};

        // Act
        var result = cache.GetRoutes(request);

        // Assert
        Assert.False(result.Count > 0);
        Assert.NotNull(result);
    }
}