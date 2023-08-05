namespace TestApp.Models.ProviderTwo;

public class ProviderTwoRoute
{
    /// <summary>
    /// Mandatory
    /// Start point of route
    /// </summary>
    public ProviderTwoPoint Departure { get; init; } = null!;

    /// <summary>
    /// Mandatory
    /// End point of route
    /// </summary>
    public ProviderTwoPoint Arrival { get; init; } = null!;

    /// <summary>
    /// Mandatory
    /// Price of route
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    /// Mandatory
    /// Time limit. After it expires, route became not actual
    /// </summary>
    public DateTime TimeLimit { get; init; }
}