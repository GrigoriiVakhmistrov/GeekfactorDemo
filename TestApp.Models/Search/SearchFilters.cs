namespace TestApp.Models.Search;

public class SearchFilters
{
    /// <summary>
    /// Optional
    /// End date of route
    /// </summary>
    public DateTime? DestinationDateTime { get; init; }

    /// <summary>
    /// Optional
    /// Maximum price of route
    /// </summary>
    public decimal? MaxPrice { get; init; }

    /// <summary>
    /// Optional
    /// Minimum value of time limit for route
    /// </summary>
    public DateTime? MinTimeLimit { get; init; }

    /// <summary>
    /// Optional
    /// Forcibly search in cached data
    /// </summary>
    public bool? OnlyCached { get; init; }
}