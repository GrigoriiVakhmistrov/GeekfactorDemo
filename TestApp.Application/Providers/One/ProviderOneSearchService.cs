using System.Net;
using Microsoft.Extensions.Logging;
using TestApp.Contracts.Services;
using TestApp.Models.ProviderOne;
using TestApp.ProviderOneClient;

namespace TestApp.Application.Providers.One;

public partial class ProviderOneSearchService : IProviderOneSearchService
{
    private readonly IProviderOneClient _providerOneClient;
    private readonly ILogger<ProviderOneSearchService> _logger;

    public ProviderOneSearchService(IProviderOneClient providerOneClient, ILogger<ProviderOneSearchService> logger)
    {
        _providerOneClient = providerOneClient ?? throw new ArgumentNullException(nameof(providerOneClient));
        _logger = logger;
    }

    public async Task<ProviderOneSearchResponse> SearchAsync(ProviderOneSearchRequest request,
        CancellationToken cancellationToken)
    {
        return await _providerOneClient.SearchAsync(request, cancellationToken);
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var httpResponse = await _providerOneClient.PingAsync(cancellationToken);

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            Log.Exception(_logger, ex);
        }

        return false;
    }

    private static partial class Log
    {
        [LoggerMessage(1, LogLevel.Error, "Provider one error")]
        public static partial void Exception(ILogger logger, Exception exception);
    }
}