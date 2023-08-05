namespace TestApp.Models.Search;

public class SearchRequest
{
    /// <summary>
    /// Mandatory
    /// Start point of route, e.g. Moscow 
    /// </summary>
    public string Origin { get; init; } = null!;

    /// <summary>
    /// Mandatory
    /// End point of route, e.g. Sochi
    /// </summary>
    public string Destination { get; init; } = null!;

    /// <summary>
    /// Mandatory
    /// Start date of route
    /// </summary>
    public DateTime OriginDateTime { get; init; }

    /// <summary>
    /// Optional
    /// </summary>
    public SearchFilters? Filters { get; init; }
}