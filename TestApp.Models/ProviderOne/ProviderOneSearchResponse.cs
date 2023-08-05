namespace TestApp.Models.ProviderOne;

public class ProviderOneSearchResponse
{
    /// <summary>
    /// Mandatory
    /// Array of routes
    /// </summary>
    public ProviderOneRoute[] Routes { get; init; } = null!;
}