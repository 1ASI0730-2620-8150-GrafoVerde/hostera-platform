namespace HosteraBackend.API.Rooms.Domain.Model.Commands;

/// <summary>
///     Command to update a room
/// </summary>
/// <param name="RoomId">The identifier of the room</param>
/// <param name="Number">The number of the room, unique within the property</param>
/// <param name="RoomTypeId">The identifier of the room type the room belongs to</param>
/// <param name="Floor">The floor of the room, or null when it is not recorded</param>
public record UpdateRoomCommand(int RoomId, string Number, int RoomTypeId, int? Floor);
