namespace HosteraBackend.API.Rooms.Domain.Model.Queries;

/// <summary>
///     Query to get the status periods of the rooms of a property, optionally limited to a date range
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
/// <param name="From">The first day of the range, or null for no lower limit</param>
/// <param name="To">The last day of the range, or null for no upper limit</param>
public record GetAllStatusPeriodsByPropertyIdQuery(int PropertyId, DateOnly? From = null, DateOnly? To = null);
