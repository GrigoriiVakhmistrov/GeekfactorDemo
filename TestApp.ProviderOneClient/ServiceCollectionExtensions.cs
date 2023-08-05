using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace TestApp.ProviderOneClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProviderOneClient(this IServiceCollection serviceCollection,
        IConfiguration configuration, string section = "ProviderOne:Url")
    {
        var configurationSection = configuration.GetSection(section);

        serviceCollection.AddRefitClient<IProviderOneClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(configurationSection.Value!));

        return serviceCollection;
    }
}