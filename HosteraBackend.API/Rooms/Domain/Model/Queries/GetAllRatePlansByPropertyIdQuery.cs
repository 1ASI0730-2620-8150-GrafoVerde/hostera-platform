namespace HosteraBackend.API.Rooms.Domain.Model.Queries;

/// <summary>
///     Query to get all rate plans of a property
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
public record GetAllRatePlansByPropertyIdQuery(int PropertyId);
