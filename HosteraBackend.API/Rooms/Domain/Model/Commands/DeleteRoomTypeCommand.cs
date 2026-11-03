namespace HosteraBackend.API.Rooms.Domain.Model.Commands;

/// <summary>
///     Command to delete a room type that no room or rate plan uses
/// </summary>
/// <param name="RoomTypeId">The identifier of the room type to delete</param>
public record DeleteRoomTypeCommand(int RoomTypeId);
