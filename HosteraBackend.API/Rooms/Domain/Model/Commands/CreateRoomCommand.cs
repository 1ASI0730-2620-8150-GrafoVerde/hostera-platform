namespace HosteraBackend.API.Rooms.Domain.Model.Commands;

/// <summary>
///     Command to create a room in a property
/// </summary>
/// <param name="PropertyId">The identifier of the property that owns the room</param>
/// <param name="Number">The number of the room, unique within the property</param>
/// <param name="RoomTypeId">The identifier of the room type the room belongs to</param>
/// <param name="Floor">The floor of the room, or null when it is not recorded</param>
public record CreateRoomCommand(int PropertyId, string Number, int RoomTypeId, int? Floor);
