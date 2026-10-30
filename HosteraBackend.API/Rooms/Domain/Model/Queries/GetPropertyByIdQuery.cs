namespace HosteraBackend.API.Rooms.Domain.Model.Queries;

/// <summary>
///     Query to get a property by ID
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
public record GetPropertyByIdQuery(int PropertyId);
