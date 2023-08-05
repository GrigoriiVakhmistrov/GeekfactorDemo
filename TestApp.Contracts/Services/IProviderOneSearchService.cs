using TestApp.Models.ProviderOne;

namespace TestApp.Contracts.Services;

/// <summary>
/// Interface for Provider One search service
/// </summary>
public interface IProviderOneSearchService
{
    /// <summary>
    /// Searches for routes using the specified search request and cancellation token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns a ProviderOneSearchResponse object containing the search results</returns>
    Task<ProviderOneSearchResponse> SearchAsync(ProviderOneSearchRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Checks if the Provider One search service is available.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns true if the service is available, false otherwise.</returns>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
}