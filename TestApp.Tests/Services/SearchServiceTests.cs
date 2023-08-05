using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using TestApp.Application.Services;
using TestApp.Contracts.Caching;
using TestApp.Contracts.Services;
using TestApp.Models.ProviderOne;
using TestApp.Models.ProviderTwo;
using TestApp.Models.Route;
using TestApp.Models.Search;

namespace TestApp.Tests.Services;

public class SearchServiceTests
{
    private readonly SearchService _searchService;
    private readonly Mock<IRouteCache> _cacheMock;
    private readonly Mock<IProviderOneSearchService> _providerOneMock;
    private readonly Mock<IProviderTwoSearchService> _providerTwoMock;
    private const string CityA = "City A";
    private const string CityB = "City B";
    private const string CityC = "City C";

    public SearchServiceTests()
    {
        _cacheMock = new Mock<IRouteCache>();
        _providerOneMock = new Mock<IProviderOneSearchService>();
        _providerTwoMock = new Mock<IProviderTwoSearchService>();
        Mock<ILogger<SearchService>> loggerMock = new();
        _searchService = new SearchService(_providerOneMock.Object, _providerTwoMock.Object, _cacheMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task SearchRoutesReturnsRoutesFromBothProviders()
    {
        // Arrange
        var providerOneResponse = new ProviderOneSearchResponse
        {
            Routes = new ProviderOneRoute[]
            {
                new()
                {
                    From = CityA,
                    To = CityB,
                    DateFrom = DateTime.Today.AddDays(1),
                    DateTo = DateTime.Today.AddDays(2),
                    Price = 100,
                    TimeLimit = DateTime.Today.AddDays(3)
                }
            }
        };

        var providerTwoResponse = new ProviderTwoSearchResponse
        {
            Routes = new ProviderTwoRoute[]
            {
                new()
                {
                    Departure = new ProviderTwoPoint
                    {
                        Point = CityA,
                        Date = DateTime.Today.AddDays(1)
                    },
                    Arrival = new ProviderTwoPoint
                    {
                        Point = CityB,
                        Date = DateTime.Today.AddDays(2)
                    },
                    Price = 200,
                    TimeLimit = DateTime.Today.AddDays(3)
                }
            }
        };

        IProviderOneSearchService_SetupSearch().ReturnsAsync(providerOneResponse);
        IProviderOneSearchService_SetupIsAvailable().ReturnsAsync(true);
        IProviderTwoSearchService_SetupSearch().ReturnsAsync(providerTwoResponse);
        IProviderTwoSearchService_SetupIsAvailable().ReturnsAsync(true);

        var filters = new SearchFilters
        {
            DestinationDateTime = DateTime.Today.AddDays(2),
            MaxPrice = 250,
            MinTimeLimit = DateTime.Today.AddDays(1),
            OnlyCached = false
        };

        var searchRequest = new SearchRequest
        {
            Destination = CityB,
            Origin = CityA,
            Filters = filters,
            OriginDateTime = DateTime.Today.AddDays(1)
        };

        // Act
        var result = await _searchService.SearchAsync(searchRequest, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Routes);

        var expectedRoutes = new List<Route>
        {
            new()
            {
                Origin = CityA,
                Destination = CityB,
                OriginDateTime = DateTime.Today.AddDays(1),
                DestinationDateTime = DateTime.Today.AddDays(2),
                Price = 100
            },
            new()
            {
                Origin = CityA,
                Destination = CityB,
                OriginDateTime = DateTime.Today.AddDays(1),
                DestinationDateTime = DateTime.Today.AddDays(2),
                Price = 200
            }
        };

        Assert.Equal(expectedRoutes.Count, result.Routes.Length);
        for (var i = 0; i < result.Routes.Length; i++)
        {
            Assert.Equal(expectedRoutes[i].Origin, result.Routes[i].Origin);
            Assert.Equal(expectedRoutes[i].Destination, result.Routes[i].Destination);
            Assert.Equal(expectedRoutes[i].OriginDateTime, result.Routes[i].OriginDateTime);
            Assert.Equal(expectedRoutes[i].DestinationDateTime, result.Routes[i].DestinationDateTime);
            Assert.Equal(expectedRoutes[i].Price, result.Routes[i].Price);
        }

        _providerOneMock.Verify(p => p.SearchAsync(It.IsAny<ProviderOneSearchRequest>(), CancellationToken.None),
            Times.Once);
        _providerTwoMock.Verify(p => p.SearchAsync(It.IsAny<ProviderTwoSearchRequest>(), CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task SearchRoutesReturnsRoutesFromProviderOne()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            Origin = CityA,
            Destination = CityB,
            OriginDateTime = DateTime.Today.AddDays(7)
        };
        var providerOneResponse = new ProviderOneSearchResponse
        {
            Routes = new ProviderOneRoute[]
            {
                new()
                {
                    From = CityA,
                    To = CityB,
                    DateFrom = DateTime.Today,
                    DateTo = DateTime.Today.AddDays(7),
                    Price = 100,
                    TimeLimit = DateTime.Today.AddDays(1)
                },
                new()
                {
                    From = CityA,
                    To = CityB,
                    DateFrom = DateTime.Today.AddDays(1),
                    DateTo = DateTime.Today.AddDays(8),
                    Price = 150,
                    TimeLimit = DateTime.Today.AddDays(2)
                }
            }
        };
        var providerTwoResponse = new ProviderTwoSearchResponse
        {
            Routes = Array.Empty<ProviderTwoRoute>()
        };
        IProviderOneSearchService_SetupSearch().ReturnsAsync(providerOneResponse);
        IProviderOneSearchService_SetupIsAvailable().ReturnsAsync(true);
        IProviderTwoSearchService_SetupSearch().ReturnsAsync(providerTwoResponse);
        IProviderTwoSearchService_SetupIsAvailable().ReturnsAsync(false);

        // Act
        var result = await _searchService.SearchAsync(searchRequest, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Routes.Length);
        Assert.Equal(CityA, result.Routes[0].Origin);
        Assert.Equal(CityB, result.Routes[0].Destination);
        Assert.Equal(DateTime.Today, result.Routes[0].OriginDateTime);
        Assert.Equal(DateTime.Today.AddDays(7), result.Routes[0].DestinationDateTime);
        Assert.Equal(100, result.Routes[0].Price);
        Assert.Equal(DateTime.Today.AddDays(1), result.Routes[0].TimeLimit);
        Assert.Equal(CityA, result.Routes[1].Origin);
        Assert.Equal(CityB, result.Routes[1].Destination);
        Assert.Equal(DateTime.Today.AddDays(1), result.Routes[1].OriginDateTime);
        Assert.Equal(DateTime.Today.AddDays(8), result.Routes[1].DestinationDateTime);
        Assert.Equal(150, result.Routes[1].Price);
        Assert.Equal(DateTime.Today.AddDays(2), result.Routes[1].TimeLimit);
    }

    [Fact]
    public async Task SearchRoutesReturnsRoutesFromProviderTwo()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            Origin = CityA,
            Destination = CityB,
            OriginDateTime = DateTime.Today.AddDays(1)
        };

        var providerTwoResponse = new ProviderTwoSearchResponse
        {
            Routes = new ProviderTwoRoute[]
            {
                new()
                {
                    Departure = new ProviderTwoPoint {Point = CityA, Date = DateTime.UtcNow},
                    Arrival = new ProviderTwoPoint {Point = CityB, Date = DateTime.UtcNow.AddDays(1)},
                    Price = 100.0M,
                    TimeLimit = DateTime.UtcNow.AddDays(3)
                }
            }
        };

        var providerOneResponse = new ProviderOneSearchResponse
        {
            Routes = Array.Empty<ProviderOneRoute>()
        };

        IProviderOneSearchService_SetupSearch().Returns(Task.FromResult(providerOneResponse));
        IProviderOneSearchService_SetupIsAvailable().ReturnsAsync(false);
        IProviderTwoSearchService_SetupSearch().Returns(Task.FromResult(providerTwoResponse));
        IProviderTwoSearchService_SetupIsAvailable().ReturnsAsync(true);

        // Act
        var result = await _searchService.SearchAsync(searchRequest, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Routes);
        Assert.Single(result.Routes);
        Assert.Equal(CityA, result.Routes[0].Origin);
        Assert.Equal(CityB, result.Routes[0].Destination);
    }

    [Fact]
    public async Task SearchRoutesReturnsNoRoutesWhenBothProvidersFail()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            Origin = CityA,
            Destination = CityB,
            OriginDateTime = DateTime.Now.AddDays(3),
            Filters = new SearchFilters
            {
                MaxPrice = 1000
            }
        };

        IProviderOneSearchService_SetupIsAvailable().ReturnsAsync(false);
        IProviderTwoSearchService_SetupIsAvailable().ReturnsAsync(false);

        // Act
        try
        {
            // Act
            var result = await _searchService.SearchAsync(searchRequest, CancellationToken.None);

            // Assert
            Assert.Empty(result.Routes);
        }
        catch (Exception ex)
        {
            // Assert
            Assert.IsType<ServiceUnavailableException>(ex);
        }
    }

    [Fact]
    public async Task SearchRoutesReturnsNoRoutesWhenAllProvidersReturnNoRoutes()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            Origin = CityA,
            Destination = CityB,
            OriginDateTime = DateTime.Now.AddDays(1),
            Filters = new SearchFilters()
        };

        var providerOneResponse = new ProviderOneSearchResponse {Routes = Array.Empty<ProviderOneRoute>()};
        var providerTwoResponse = new ProviderTwoSearchResponse {Routes = Array.Empty<ProviderTwoRoute>()};

        IProviderOneSearchService_SetupSearch().ReturnsAsync(providerOneResponse);
        IProviderOneSearchService_SetupIsAvailable().ReturnsAsync(true);
        IProviderTwoSearchService_SetupSearch().ReturnsAsync(providerTwoResponse);
        IProviderTwoSearchService_SetupIsAvailable().ReturnsAsync(true);

        // Act
        var result = await _searchService.SearchAsync(searchRequest, CancellationToken.None);

        // Assert
        Assert.Empty(result.Routes);
    }

    [Fact]
    public async Task TestFilterRoutesReturnsExpectedNumberOfRoutes()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            Origin = CityA,
            Destination = CityB,
            OriginDateTime = new DateTime(2022, 01, 02),
            Filters = new SearchFilters() {MaxPrice = 50}
        };

        var providerOneRoute = new ProviderOneRoute
        {
            From = CityA,
            To = CityB,
            DateFrom = new DateTime(2022, 01, 01),
            DateTo = new DateTime(2022, 01, 02),
            Price = 50,
            TimeLimit = new DateTime(2021, 12, 31)
        };

        var providerTwoRoute = new ProviderTwoRoute
        {
            Departure = new ProviderTwoPoint {Point = CityA, Date = new DateTime(2022, 01, 01)},
            Arrival = new ProviderTwoPoint {Point = CityB, Date = new DateTime(2022, 01, 02)},
            Price = 75,
            TimeLimit = new DateTime(2021, 12, 31)
        };

        var providerOneResponse = new ProviderOneSearchResponse {Routes = new[] {providerOneRoute}};
        var providerTwoResponse = new ProviderTwoSearchResponse {Routes = new[] {providerTwoRoute}};
        IProviderOneSearchService_SetupSearch().ReturnsAsync(providerOneResponse);
        IProviderOneSearchService_SetupIsAvailable().ReturnsAsync(true);
        IProviderTwoSearchService_SetupSearch().ReturnsAsync(providerTwoResponse);
        IProviderTwoSearchService_SetupIsAvailable().ReturnsAsync(true);

        // Act
        var searchResult = await _searchService.SearchAsync(searchRequest, CancellationToken.None);

        // Assert
        Assert.Single(searchResult.Routes);
    }

    [Fact]
    public async Task TestFilterRoutesReturnsExpectedRoutesWhenFilteringByDestinationDateTime()
    {
        // Arrange
        var providerOneRoutes = new ProviderOneRoute[]
        {
            new()
            {
                From = CityA, To = CityB, DateFrom = DateTime.Now, DateTo = DateTime.Now.AddDays(6),
                Price = 500, TimeLimit = DateTime.Now.AddDays(6)
            },
            new()
            {
                From = CityB, To = CityA, DateFrom = DateTime.Now, DateTo = DateTime.Now.AddDays(5),
                Price = 400, TimeLimit = DateTime.Now.AddDays(5)
            },
            new()
            {
                From = CityA, To = CityC, DateFrom = DateTime.Now, DateTo = DateTime.Now.AddDays(6),
                Price = 300, TimeLimit = DateTime.Now.AddDays(6)
            }
        };
        var providerOneResponse = new ProviderOneSearchResponse {Routes = providerOneRoutes};

        var providerTwoRoutes = new ProviderTwoRoute[]
        {
            new()
            {
                Departure = new ProviderTwoPoint {Point = CityA, Date = DateTime.Now},
                Arrival = new ProviderTwoPoint {Point = CityB, Date = DateTime.Now.AddDays(6)}, Price = 700,
                TimeLimit = DateTime.Now.AddDays(6)
            },
            new()
            {
                Departure = new ProviderTwoPoint {Point = CityB, Date = DateTime.Now},
                Arrival = new ProviderTwoPoint {Point = CityA, Date = DateTime.Now.AddDays(5)}, Price = 800,
                TimeLimit = DateTime.Now.AddDays(5)
            },
            new()
            {
                Departure = new ProviderTwoPoint {Point = CityA, Date = DateTime.Now},
                Arrival = new ProviderTwoPoint {Point = CityC, Date = DateTime.Now.AddDays(6)}, Price = 900,
                TimeLimit = DateTime.Now.AddDays(6)
            }
        };
        var providerTwoResponse = new ProviderTwoSearchResponse {Routes = providerTwoRoutes};

        var searchFilters = new SearchFilters {DestinationDateTime = DateTime.Now.AddDays(5)};
        var searchRequest = new SearchRequest {Filters = searchFilters};

        IProviderOneSearchService_SetupSearch().ReturnsAsync(providerOneResponse);
        IProviderOneSearchService_SetupIsAvailable().ReturnsAsync(true);
        IProviderTwoSearchService_SetupSearch().ReturnsAsync(providerTwoResponse);
        IProviderTwoSearchService_SetupIsAvailable().ReturnsAsync(true);

        // Act
        var result = await _searchService.SearchAsync(searchRequest, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Routes.Length);
        Assert.Equal(CityA, result.Routes[0].Destination);
        Assert.Equal(CityB, result.Routes[1].Origin);
    }

    [Fact]
    public async Task TestFilterRoutesReturnsExpectedRoutesWhenFilteringByMinTimeLimit()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            Origin = CityA,
            Destination = CityB,
            OriginDateTime = DateTime.UtcNow,
            Filters = new SearchFilters() {MinTimeLimit = DateTime.UtcNow.AddHours(1)}
        };

        var providerOneRoute = new ProviderOneRoute
        {
            From = CityA,
            To = CityB,
            DateFrom = DateTime.UtcNow,
            DateTo = DateTime.UtcNow.AddDays(1),
            Price = 100,
            TimeLimit = DateTime.UtcNow
        };

        var providerTwoRoute = new ProviderTwoRoute
        {
            Departure = new ProviderTwoPoint {Point = CityA, Date = DateTime.UtcNow},
            Arrival = new ProviderTwoPoint {Point = CityB, Date = DateTime.UtcNow.AddDays(1)},
            Price = 200,
            TimeLimit = DateTime.UtcNow.AddHours(2)
        };

        var providerOneResponse = new ProviderOneSearchResponse
        {
            Routes = new[] {providerOneRoute}
        };

        var providerTwoResponse = new ProviderTwoSearchResponse
        {
            Routes = new[] {providerTwoRoute}
        };

        IProviderOneSearchService_SetupSearch().ReturnsAsync(providerOneResponse);
        IProviderOneSearchService_SetupIsAvailable().ReturnsAsync(true);
        IProviderTwoSearchService_SetupSearch().ReturnsAsync(providerTwoResponse);
        IProviderTwoSearchService_SetupIsAvailable().ReturnsAsync(true);

        // Act
        var result = await _searchService.SearchAsync(searchRequest, CancellationToken.None);

        // Assert
        Assert.Single(result.Routes);
        Assert.Equal(providerTwoRoute.Price, result.Routes.First().Price);
    }

    [Fact]
    public async Task TestFilterRoutesReturnsExpectedRoutesWhenFilteringByOnlyCached()
    {
        // Arrange
        var routes = new List<Route>
        {
            new()
            {
                Origin = CityA,
                Destination = CityB,
                OriginDateTime = new DateTime(2022, 6, 1, 8, 0, 0),
                DestinationDateTime = new DateTime(2022, 6, 1, 10, 0, 0),
                Price = 100,
                TimeLimit = new DateTime(2022, 5, 30)
            },
            new()
            {
                Origin = CityB,
                Destination = CityA,
                OriginDateTime = new DateTime(2022, 6, 1, 8, 0, 0),
                DestinationDateTime = new DateTime(2022, 6, 1, 10, 0, 0),
                Price = 200,
                TimeLimit = new DateTime(2022, 5, 30)
            }
        };

        var searchRequest = new SearchRequest
        {
            Origin = CityA,
            Destination = CityB,
            OriginDateTime = new DateTime(2022, 6, 1, 8, 0, 0),
            Filters = new SearchFilters {OnlyCached = true}
        };

        _cacheMock.Setup(c => c.GetRoutes(It.Is<SearchRequest>(r =>
                r.Origin == CityA && r.Destination == CityB &&
                r.OriginDateTime == new DateTime(2022, 6, 1, 8, 0, 0))))
            .Returns(routes);

        // Act
        var result = await _searchService.SearchAsync(searchRequest, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Routes.Length);
        Assert.Equal(CityA, result.Routes[0].Origin);
        Assert.Equal(CityB, result.Routes[0].Destination);
        Assert.Equal(CityB, result.Routes[1].Origin);
        Assert.Equal(CityA, result.Routes[1].Destination);
    }

    private ISetup<IProviderOneSearchService, Task<ProviderOneSearchResponse>> IProviderOneSearchService_SetupSearch()
    {
        return _providerOneMock.Setup(p => p.SearchAsync(It.IsAny<ProviderOneSearchRequest>(), CancellationToken.None));
    }

    private ISetup<IProviderOneSearchService, Task<bool>> IProviderOneSearchService_SetupIsAvailable()
    {
        return _providerOneMock.Setup(p => p.IsAvailableAsync(CancellationToken.None));
    }

    private ISetup<IProviderTwoSearchService, Task<ProviderTwoSearchResponse>> IProviderTwoSearchService_SetupSearch()
    {
        return _providerTwoMock.Setup(p => p.SearchAsync(It.IsAny<ProviderTwoSearchRequest>(), CancellationToken.None));
    }

    private ISetup<IProviderTwoSearchService, Task<bool>> IProviderTwoSearchService_SetupIsAvailable()
    {
        return _providerTwoMock.Setup(p => p.IsAvailableAsync(CancellationToken.None));
    }
}