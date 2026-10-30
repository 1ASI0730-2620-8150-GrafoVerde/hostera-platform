namespace HosteraBackend.API.Overview.Domain.Model.Queries;

/// <summary>
///     Query to get the overview of every property to compare them
/// </summary>
/// <param name="Date">The day of the overviews</param>
public record GetAllPropertyOverviewsQuery(DateOnly Date);
