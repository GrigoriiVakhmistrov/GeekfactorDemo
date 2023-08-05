using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace TestApp.ProviderTwoClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProviderTwoClient(this IServiceCollection serviceCollection,
        IConfiguration configuration, string section = "ProviderTwo:Url")
    {
        var configurationSection = configuration.GetSection(section);

        serviceCollection.AddRefitClient<IProviderTwoClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(configurationSection.Value!));

        return serviceCollection;
    }
}