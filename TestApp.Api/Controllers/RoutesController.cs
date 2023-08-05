using System.Net;
using Microsoft.AspNetCore.Mvc;
using TestApp.Application.Services;
using TestApp.Contracts.Services;
using TestApp.Models.Search;

namespace TestApp.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/routes")]
[Consumes("application/json")]
[Produces("application/json")]
public class RoutesController : ControllerBase
{
    private readonly ISearchService _searchService;

    public RoutesController(ISearchService searchService)
    {
        _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
    }

    [HttpGet("ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> PingAsync(CancellationToken cancellationToken)
    {
        return await _searchService.IsAvailableAsync(cancellationToken)
            ? Ok()
            : Problem(statusCode: (int) HttpStatusCode.ServiceUnavailable);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<SearchResponse>> SearchAsync([FromBody] SearchRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _searchService.SearchAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (ServiceUnavailableException)
        {
            return Problem(statusCode: (int) HttpStatusCode.ServiceUnavailable,
                title: "Both providers are unavailable");
        }
    }
}