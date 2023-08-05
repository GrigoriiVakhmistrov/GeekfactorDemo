using Refit;
using TestApp.Models.ProviderOne;

namespace TestApp.ProviderOneClient;

public interface IProviderOneClient
{
    [Get("/ping")]
    Task<HttpResponseMessage> PingAsync(CancellationToken cancellationToken);

    [Post("/search")]
    Task<ProviderOneSearchResponse> SearchAsync(ProviderOneSearchRequest request, CancellationToken cancellationToken);
}