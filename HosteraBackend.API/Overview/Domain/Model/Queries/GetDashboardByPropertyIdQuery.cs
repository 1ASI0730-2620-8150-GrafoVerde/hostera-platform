namespace HosteraBackend.API.Overview.Domain.Model.Queries;

/// <summary>
///     Query to get the operational dashboard of a property
/// </summary>
/// <param name="PropertyId">The property ID to summarize</param>
/// <param name="Date">The day of the dashboard</param>
/// <param name="From">The first night of the daily performance</param>
/// <param name="To">The last night of the daily performance, included</param>
public record GetDashboardByPropertyIdQuery(int PropertyId, DateOnly Date, DateOnly From, DateOnly To);
