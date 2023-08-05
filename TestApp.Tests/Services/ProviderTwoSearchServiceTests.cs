using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using TestApp.Application.Providers.Two;
using TestApp.Models.ProviderTwo;
using TestApp.ProviderTwoClient;

namespace TestApp.Tests.Services;

public class ProviderTwoSearchServiceTests
{
    private readonly Mock<IProviderTwoClient> _clientMock;
    private readonly ProviderTwoSearchService _searchService;

    public ProviderTwoSearchServiceTests()
    {
        _clientMock = new Mock<IProviderTwoClient>();
        Mock<ILogger<ProviderTwoSearchService>> loggerMock = new();
        _searchService = new ProviderTwoSearchService(_clientMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task SearchAsyncReturnsCorrectResponse()
    {
        // Arrange
        var expectedResponse = new ProviderTwoSearchResponse
        {
            Routes = new ProviderTwoRoute[]
            {
                new()
                {
                    Arrival = new ProviderTwoPoint() {Point = "City A", Date = DateTime.UtcNow},
                    Departure = new ProviderTwoPoint() {Point = "City B", Date = DateTime.UtcNow},
                    Price = 150,
                    TimeLimit = DateTime.UtcNow.AddDays(7)
                }
            }
        };

        IProviderTwoClient_SetupSearch()
            .ReturnsAsync(expectedResponse);

        var loggerMock = new Mock<ILogger<ProviderTwoSearchService>>();

        // Act
        var actualResponse =
            await _searchService.SearchAsync(new ProviderTwoSearchRequest(), CancellationToken.None);

        // Assert
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

        IProviderTwoClient_SetupPing()
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

        IProviderTwoClient_SetupPing()
            .ReturnsAsync(expectedResponse);

        // Act
        var isAvailable = await _searchService.IsAvailableAsync(default);

        // Assert
        Assert.False(isAvailable);
    }

    private ISetup<IProviderTwoClient, Task<HttpResponseMessage>> IProviderTwoClient_SetupPing()
    {
        return _clientMock.Setup(c => c.PingAsync(
            It.IsAny<CancellationToken>()
        ));
    }

    private ISetup<IProviderTwoClient, Task<ProviderTwoSearchResponse>> IProviderTwoClient_SetupSearch()
    {
        return _clientMock.Setup(c => c.SearchAsync(
            It.IsAny<ProviderTwoSearchRequest>(),
            It.IsAny<CancellationToken>()
        ));
    }
}