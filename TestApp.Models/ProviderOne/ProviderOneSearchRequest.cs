using TestApp.Models.Search;

namespace TestApp.Models.ProviderOne;

public class ProviderOneSearchRequest
{
    /// <summary>
    /// Mandatory
    /// Start point of route, e.g. Moscow
    /// </summary>
    public string From { get; set; } = null!;

    /// <summary>
    /// Mandatory
    /// End point of route, e.g. Sochi
    /// </summary>
    public string To { get; set; } = null!;

    /// <summary>
    /// Mandatory
    /// Start date of route
    /// </summary>
    public DateTime DateFrom { get; set; }

    /// <summary>
    /// Optional
    /// End date of route
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// Optional
    /// Maximum price of route
    /// </summary>
    public decimal? MaxPrice { get; set; }

    public static ProviderOneSearchRequest FromSearchRequest(SearchRequest request)
    {
        return new ProviderOneSearchRequest
        {
            From = request.Origin,
            To = request.Destination,
            DateFrom = request.OriginDateTime,
            DateTo = request.Filters?.DestinationDateTime,
            MaxPrice = request.Filters?.MaxPrice
        };
    }
}