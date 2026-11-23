namespace HosteraBackend.API.Overview.Domain.Model.Queries;

/// <summary>
///     Query to get the overview of every property of an organization to compare them
/// </summary>
/// <param name="OrganizationId">The identifier of the organization that owns the properties</param>
/// <param name="Date">The day of the overviews</param>
public record GetAllPropertyOverviewsQuery(int OrganizationId, DateOnly Date);
