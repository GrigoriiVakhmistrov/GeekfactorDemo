using TestApp.Models.Search;

namespace TestApp.Contracts.Services;

/// <summary>
/// Interface for main search service
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Searches for routes using the specified search request and cancellation token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns a SearchResponse object containing the search results</returns>
    Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if the search service is available.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns true if the service is available, false otherwise.</returns>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
}