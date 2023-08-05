namespace TestApp.Models.ProviderTwo;

public class ProviderTwoPoint
{
    /// <summary>
    /// Mandatory
    /// Point of route
    /// </summary>
    public string Point { get; init; } = null!;

    /// <summary>
    /// Mandatory
    /// End date of route
    /// </summary>
    public DateTime Date { get; init; }
}