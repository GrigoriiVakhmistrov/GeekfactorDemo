using Refit;
using TestApp.Models.ProviderTwo;

namespace TestApp.ProviderTwoClient;

public interface IProviderTwoClient
{
    [Get("/ping")]
    Task<HttpResponseMessage> PingAsync(CancellationToken cancellationToken);

    [Post("/search")]
    Task<ProviderTwoSearchResponse> SearchAsync(ProviderTwoSearchRequest request, CancellationToken cancellationToken);
}