namespace TestApp.Models.ProviderTwo;

public class ProviderTwoSearchResponse
{
    /// <summary>
    /// Mandatory
    /// Array of routes
    /// </summary>
    public ProviderTwoRoute[] Routes { get; init; } = null!;
}