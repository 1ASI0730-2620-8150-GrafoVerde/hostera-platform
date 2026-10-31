namespace HosteraBackend.API.Rooms.Domain.Model.Queries;

/// <summary>
///     Query to get a room by ID
/// </summary>
/// <param name="RoomId">The room ID to search</param>
public record GetRoomByIdQuery(int RoomId);
