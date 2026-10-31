namespace HosteraBackend.API.Rooms.Domain.Model.Queries;

/// <summary>
///     Query to get all room types of a property
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
public record GetAllRoomTypesByPropertyIdQuery(int PropertyId);
