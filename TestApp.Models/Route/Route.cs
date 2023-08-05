namespace TestApp.Models.Route;

public class Route
{
    /// <summary>
    /// Mandatory
    /// Identifier of the whole route
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Mandatory
    /// Start point of route
    /// </summary>
    public string Origin { get; init; } = null!;

    /// <summary>
    /// Mandatory
    /// End point of route
    /// </summary>
    public string Destination { get; init; } = null!;

    /// <summary>
    /// Mandatory
    /// Start date of route
    /// </summary>
    public DateTime OriginDateTime { get; init; }

    /// <summary>
    /// Mandatory
    /// End date of route
    /// </summary>
    public DateTime DestinationDateTime { get; init; }

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