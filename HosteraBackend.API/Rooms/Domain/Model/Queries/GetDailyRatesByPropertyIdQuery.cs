namespace HosteraBackend.API.Rooms.Domain.Model.Queries;

/// <summary>
///     Query to get the daily rates of a property, optionally limited to a period
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
/// <param name="StartDate">The first night of the period, or null for no lower limit</param>
/// <param name="EndDate">The last night of the period, or null for no upper limit</param>
public record GetDailyRatesByPropertyIdQuery(int PropertyId, DateOnly? StartDate = null, DateOnly? EndDate = null);
