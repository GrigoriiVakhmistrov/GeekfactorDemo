using System.Net;
using Microsoft.Extensions.Logging;
using TestApp.Contracts.Services;
using TestApp.Models.ProviderTwo;
using TestApp.ProviderTwoClient;

namespace TestApp.Application.Providers.Two;

public partial class ProviderTwoSearchService : IProviderTwoSearchService
{
    private readonly IProviderTwoClient _providerTwoClient;
    private readonly ILogger<ProviderTwoSearchService> _logger;

    public ProviderTwoSearchService(IProviderTwoClient providerTwoClient, ILogger<ProviderTwoSearchService> logger)
    {
        _providerTwoClient = providerTwoClient ?? throw new ArgumentNullException(nameof(providerTwoClient));
        _logger = logger;
    }

    public async Task<ProviderTwoSearchResponse> SearchAsync(ProviderTwoSearchRequest request,
        CancellationToken cancellationToken)
    {
        return await _providerTwoClient.SearchAsync(request, cancellationToken);
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var httpResponse = await _providerTwoClient.PingAsync(cancellationToken);

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
        [LoggerMessage(1, LogLevel.Error, "Provider two error")]
        public static partial void Exception(ILogger logger, Exception exception);
    }
}