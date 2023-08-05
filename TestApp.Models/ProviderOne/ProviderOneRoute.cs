namespace TestApp.Models.ProviderOne;

public class ProviderOneRoute
{
    /// <summary>
    /// Mandatory
    /// Start point of route
    /// </summary>
    public string From { get; init; } = null!;

    /// <summary>
    /// Mandatory
    /// End point of route
    /// </summary>
    public string To { get; init; } = null!;

    /// <summary>
    /// Mandatory
    /// Start date of route
    /// </summary>
    public DateTime DateFrom { get; init; }

    /// <summary>
    /// Mandatory
    /// End date of route
    /// </summary>
    public DateTime DateTo { get; init; }

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