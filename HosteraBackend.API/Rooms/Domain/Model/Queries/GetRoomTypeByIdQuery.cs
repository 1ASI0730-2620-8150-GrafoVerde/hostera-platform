namespace HosteraBackend.API.Rooms.Domain.Model.Queries;

/// <summary>
///     Query to get a room type by ID
/// </summary>
/// <param name="RoomTypeId">The room type ID to search</param>
public record GetRoomTypeByIdQuery(int RoomTypeId);
