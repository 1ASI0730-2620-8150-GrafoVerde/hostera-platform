namespace HosteraBackend.API.Rooms.Domain.Model.Queries;

/// <summary>
///     Query to get the daily rates of a property in a period
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
/// <param name="StartDate">The first night of the period</param>
/// <param name="EndDate">The last night of the period</param>
public record GetDailyRatesByPropertyIdQuery(int PropertyId, DateOnly StartDate, DateOnly EndDate);
