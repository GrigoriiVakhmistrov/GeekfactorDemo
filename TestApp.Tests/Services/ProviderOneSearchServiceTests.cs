using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using TestApp.Application.Providers.One;
using TestApp.Models.ProviderOne;
using TestApp.ProviderOneClient;

namespace TestApp.Tests.Services;

public class ProviderOneSearchServiceTests
{
    private readonly Mock<IProviderOneClient> _clientMock;
    private readonly ProviderOneSearchService _searchService;

    public ProviderOneSearchServiceTests()
    {
        _clientMock = new Mock<IProviderOneClient>();
        Mock<ILogger<ProviderOneSearchService>> loggerMock = new();
        _searchService = new ProviderOneSearchService(_clientMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task SearchAsyncReturnsCorrectResponse()
    {
        // Arrange
        var expectedResponse = new ProviderOneSearchResponse
        {
            Routes = new ProviderOneRoute[]
            {
                new()
                {
                    From = "City A",
                    To = "City B",
                    DateFrom = DateTime.UtcNow,
                    DateTo = DateTime.UtcNow.AddHours(6),
                    Price = 150,
                    TimeLimit = DateTime.UtcNow.AddDays(7)
                }
            }
        };

        IProviderOneClient_SetupSearch()
            .ReturnsAsync(expectedResponse);

        var actualResponse =
            await _searchService.SearchAsync(new ProviderOneSearchRequest(), CancellationToken.None);
        Assert.Equal(expectedResponse.Routes.Length, actualResponse.Routes.Length);
        Assert.Equivalent(expectedResponse.Routes[0], actualResponse.Routes[0]);
    }

    [Fact]
    public async Task IsAvailableAsyncReturnsTrueWhenApiClientIsAvailable()
    {
        // Arrange
#pragma warning disable IDISP001
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
#pragma warning restore IDISP001

        IProviderOneClient_SetupPing()
            .ReturnsAsync(expectedResponse);

        // Act
        var isAvailable = await _searchService.IsAvailableAsync(default);

        // Assert
        Assert.True(isAvailable);
    }

    [Fact]
    public async Task IsAvailableAsyncReturnsFalseWhenApiClientIsNotAvailable()
    {
        // Arrange
#pragma warning disable IDISP001
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.Forbidden);
#pragma warning restore IDISP001

        IProviderOneClient_SetupPing()
            .ReturnsAsync(expectedResponse);

        // Act
        var isAvailable = await _searchService.IsAvailableAsync(default);

        // Assert
        Assert.False(isAvailable);
    }

    private ISetup<IProviderOneClient, Task<HttpResponseMessage>> IProviderOneClient_SetupPing()
    {
        return _clientMock.Setup(c => c.PingAsync(
            It.IsAny<CancellationToken>()
        ));
    }

    private ISetup<IProviderOneClient, Task<ProviderOneSearchResponse>> IProviderOneClient_SetupSearch()
    {
        return _clientMock.Setup(c => c.SearchAsync(
            It.IsAny<ProviderOneSearchRequest>(),
            It.IsAny<CancellationToken>()
        ));
    }
}