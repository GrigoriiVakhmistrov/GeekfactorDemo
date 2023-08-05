namespace TestApp.Models.Search;

/// <summary>
/// Represents the response of a search request.
/// </summary>
public class SearchResponse
{
    /// <summary>
    /// Gets or sets the array of routes that match the search criteria.
    /// </summary>
    public Route.Route[] Routes { get; init; } = null!;

    /// <summary>
    /// Gets or sets the minimum price among the found routes.
    /// </summary>
    public decimal MinPrice { get; set; }

    /// <summary>
    /// Gets or sets the maximum price among the found routes.
    /// </summary>
    public decimal MaxPrice { get; set; }

    /// <summary>
    /// Gets or sets the minimum travel time among the found routes (in minutes).
    /// </summary>
    public int MinTravelTime { get; set; }

    /// <summary>
    /// Gets or sets the maximum travel time among the found routes (in minutes).
    /// </summary>
    public int MaxTravelTime { get; set; }
}