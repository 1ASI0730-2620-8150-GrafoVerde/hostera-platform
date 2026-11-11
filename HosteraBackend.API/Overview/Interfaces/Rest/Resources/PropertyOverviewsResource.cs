namespace HosteraBackend.API.Overview.Interfaces.Rest.Resources;

/// <summary>
///     Property overviews resource for REST API, used to compare the properties
/// </summary>
/// <param name="Date">The day of the overviews</param>
/// <param name="Overviews">The overview of each property whose sources are available</param>
/// <param name="UnavailableSections">The sources that could not provide data: rooms or bookings</param>
public record PropertyOverviewsResource(
    DateOnly Date,
    IEnumerable<PropertyOverviewResource> Overviews,
    IEnumerable<string> UnavailableSections);
