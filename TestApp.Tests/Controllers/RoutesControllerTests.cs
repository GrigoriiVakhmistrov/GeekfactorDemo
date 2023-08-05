using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Language.Flow;
using TestApp.Api.Controllers;
using TestApp.Application.Services;
using TestApp.Contracts.Services;
using TestApp.Models.Route;
using TestApp.Models.Search;

namespace TestApp.Tests.Controllers;

public class RoutesControllerTests
{
    private readonly Mock<ISearchService> _searchServiceMock;
    private readonly RoutesController _controller;

    public RoutesControllerTests()
    {
        _searchServiceMock = new Mock<ISearchService>();
        _controller = new RoutesController(_searchServiceMock.Object);
    }

    [Fact]
    public async Task SearchAsyncReturnsOkObjectResultWithSearchResponse()
    {
        // Arrange
        var request = new SearchRequest();

        var searchResponse = new SearchResponse
        {
            Routes = Array.Empty<Route>(),
            MinPrice = 0,
            MaxPrice = 0,
            MinTravelTime = 0,
            MaxTravelTime = 0
        };

        SearchService_Setup()
            .ReturnsAsync(searchResponse);

        // Act
        var response = await _controller.SearchAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<ActionResult<SearchResponse>>(response);
        var okObjectResult = Assert.IsType<OkObjectResult>(response.Result);
        var result = Assert.IsType<SearchResponse>(okObjectResult.Value);
        Assert.Equal(searchResponse, result);
    }

    [Fact]
    public async Task SearchAsyncReturnsStatus500WhenServiceUnavailableExceptionIsThrown()
    {
        // Arrange
        const string expectedMessage = "Both providers are unavailable";
        var searchRequest = new SearchRequest();
        SearchService_Setup()
            .ThrowsAsync(new ServiceUnavailableException(""));

        // Act
        var response = await _controller.SearchAsync(searchRequest, default);

        // Assert
        Assert.IsType<ActionResult<SearchResponse>>(response);
        var objectResult = Assert.IsType<ObjectResult>(response.Result);
        Assert.Equal((int) HttpStatusCode.ServiceUnavailable, objectResult.StatusCode);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(expectedMessage, problemDetails.Title);
    }

    private ISetup<ISearchService, Task<SearchResponse>> SearchService_Setup()
    {
        return _searchServiceMock.Setup(x => x.SearchAsync(It.IsAny<SearchRequest>(), default));
    }
}