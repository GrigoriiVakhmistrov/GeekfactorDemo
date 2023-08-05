using TestApp.Models.ProviderTwo;

namespace TestApp.Contracts.Services;

/// <summary>
/// Interface for Provider Two search service
/// </summary>
public interface IProviderTwoSearchService
{
    /// <summary>
    /// Searches for routes using the specified search request and cancellation token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns a ProviderTwoSearchResponse object containing the search results</returns>
    Task<ProviderTwoSearchResponse> SearchAsync(ProviderTwoSearchRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Checks if the Provider Two search service is available.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns true if the service is available, false otherwise.</returns>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
}