using Microsoft.Extensions.Logging;
using TestApp.Contracts.Caching;
using TestApp.Contracts.Services;
using TestApp.Models.ProviderOne;
using TestApp.Models.ProviderTwo;
using TestApp.Models.Search;
using Route = TestApp.Models.Route.Route;

namespace TestApp.Application.Services;

public partial class SearchService : ISearchService
{
    private readonly IProviderOneSearchService _providerOneSearchService;
    private readonly IProviderTwoSearchService _providerTwoSearchService;
    private readonly IRouteCache _routeCache;
    private readonly ILogger<SearchService> _logger;

    public SearchService(IProviderOneSearchService providerOneSearchService,
        IProviderTwoSearchService providerTwoSearchService,
        IRouteCache routeCache, ILogger<SearchService> logger)
    {
        _providerOneSearchService = providerOneSearchService;
        _providerTwoSearchService = providerTwoSearchService;
        _routeCache = routeCache;
        _logger = logger;
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        // Check if both providers are available
        var providerOneAvailabilityTask = _providerOneSearchService.IsAvailableAsync(cancellationToken);
        var providerTwoAvailabilityTask = _providerTwoSearchService.IsAvailableAsync(cancellationToken);

        await Task.WhenAll(providerOneAvailabilityTask, providerTwoAvailabilityTask);

        var providerOneIsAvailable = providerOneAvailabilityTask.Result;
        var providerTwoIsAvailable = providerTwoAvailabilityTask.Result;

        if (!providerOneIsAvailable && !providerTwoIsAvailable && request.Filters?.OnlyCached != true)
        {
            Log.BothProvidersAreUnavailable(_logger);
            throw new ServiceUnavailableException("Both providers are unavailable");
        }

        ProviderOneSearchResponse? providerOneResponse = null;
        ProviderTwoSearchResponse? providerTwoResponse = null;

        var providerOneSearchTask = Task.FromResult<ProviderOneSearchResponse>(null!);
        var providerTwoSearchTask = Task.FromResult<ProviderTwoSearchResponse>(null!);

        var shouldSearch = request.Filters?.OnlyCached is null or false;

        // Search routes from provider one
        if (providerOneIsAvailable && shouldSearch)
        {
            var providerOneRequest = ProviderOneSearchRequest.FromSearchRequest(request);
            providerOneSearchTask = _providerOneSearchService.SearchAsync(providerOneRequest, cancellationToken);
        }

        // Search routes from provider two
        if (providerTwoIsAvailable && shouldSearch)
        {
            var providerTwoRequest = ProviderTwoSearchRequest.FromSearchRequest(request);
            providerTwoSearchTask = _providerTwoSearchService.SearchAsync(providerTwoRequest, cancellationToken);
        }

        await Task.WhenAll(providerOneSearchTask, providerTwoSearchTask);

        if (providerOneIsAvailable)
        {
            providerOneResponse = await providerOneSearchTask;
        }

        if (providerTwoIsAvailable)
        {
            providerTwoResponse = await providerTwoSearchTask;
        }

        // Merge and filter routes from both providers
        var mergedRoutes = MergeRoutes(providerOneResponse?.Routes ?? Array.Empty<ProviderOneRoute>(),
            providerTwoResponse?.Routes ?? Array.Empty<ProviderTwoRoute>(), request);
        var filteredRoutes = FilterRoutes(mergedRoutes, request.Filters);

        // Add routes to cache
        _routeCache.AddRoutes(request, filteredRoutes);

        // Get cheapest and fastest routes
        var cheapestRoute = filteredRoutes.MinBy(r => r.Price);
        var fastestRoute = filteredRoutes.MinBy(r => (r.DestinationDateTime - r.OriginDateTime).TotalMinutes);

        return new SearchResponse
        {
            Routes = filteredRoutes.ToArray(),
            MinPrice = cheapestRoute?.Price ?? 0,
            MaxPrice = fastestRoute?.Price ?? 0,
            MinTravelTime = filteredRoutes.Count > 0
                ? (int) filteredRoutes.Min(r => (r.DestinationDateTime - r.OriginDateTime).TotalMinutes)
                : int.MaxValue,
            MaxTravelTime = filteredRoutes.Count > 0
                ? (int) filteredRoutes.Max(r => (r.DestinationDateTime - r.OriginDateTime).TotalMinutes)
                : int.MinValue
        };
    }

    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(true); // Always return true, because the service is always available
    }

    private List<Route> MergeRoutes(ProviderOneRoute[]? providerOneRoutes,
        ProviderTwoRoute[]? providerTwoRoutes, SearchRequest searchRequest)
    {
        var mergedRoutes = new List<Route>();

        if (searchRequest.Filters?.OnlyCached ?? false)
            return _routeCache.GetRoutes(searchRequest);

        // Add routes from provider one
        if (providerOneRoutes != null)
            mergedRoutes.AddRange(providerOneRoutes.Select(providerOneRoute => new Route
            {
                Id = Guid.NewGuid(),
                Origin = providerOneRoute.From,
                Destination = providerOneRoute.To,
                OriginDateTime = providerOneRoute.DateFrom,
                DestinationDateTime = providerOneRoute.DateTo,
                Price = providerOneRoute.Price,
                TimeLimit = providerOneRoute.TimeLimit
            }));

        // Add routes from provider two
        if (providerTwoRoutes != null)
            mergedRoutes.AddRange(providerTwoRoutes.Select(providerTwoRoute => new Route
            {
                Id = Guid.NewGuid(),
                Origin = providerTwoRoute.Departure.Point,
                Destination = providerTwoRoute.Arrival.Point,
                OriginDateTime = providerTwoRoute.Departure.Date,
                DestinationDateTime = providerTwoRoute.Arrival.Date,
                Price = providerTwoRoute.Price,
                TimeLimit = providerTwoRoute.TimeLimit
            }));

        // Sort routes by price
        mergedRoutes.Sort((a, b) => a.Price.CompareTo(b.Price));

        return mergedRoutes;
    }

    private static List<Route> FilterRoutes(List<Route> routes, SearchFilters? filters)
    {
        if (filters == null)
        {
            return routes;
        }

        var filteredRoutes = routes.Where(route =>
            (filters.DestinationDateTime == null || route.DestinationDateTime <= filters.DestinationDateTime) &&
            (filters.MaxPrice == null || route.Price <= filters.MaxPrice) &&
            (filters.MinTimeLimit == null || route.TimeLimit >= filters.MinTimeLimit));

        return filteredRoutes.ToList();
    }

    private static partial class Log
    {
        [LoggerMessage(1, LogLevel.Error, "Both providers are unavailable")]
        public static partial void BothProvidersAreUnavailable(ILogger logger);
    }
}