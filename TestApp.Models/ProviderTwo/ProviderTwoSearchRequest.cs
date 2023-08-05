using TestApp.Models.Search;

namespace TestApp.Models.ProviderTwo;

public class ProviderTwoSearchRequest
{
    /// <summary>
    /// Mandatory
    /// Start point of route, e.g. Moscow
    /// </summary>
    public string Departure { get; set; } = null!;

    /// <summary>
    /// Mandatory
    /// End point of route, e.g. Sochi
    /// </summary>
    public string Arrival { get; set; } = null!;

    /// <summary>
    /// Mandatory
    /// Start date of route
    /// </summary>
    public DateTime DepartureDate { get; set; }

    /// <summary>
    /// Optional
    /// Minimum value of time limit for route
    /// </summary>
    public DateTime? MinTimeLimit { get; set; }

    public static ProviderTwoSearchRequest FromSearchRequest(SearchRequest request)
    {
        return new ProviderTwoSearchRequest
        {
            Departure = request.Origin,
            Arrival = request.Destination,
            DepartureDate = request.OriginDateTime,
            MinTimeLimit = request.Filters?.MinTimeLimit
        };
    }
}