namespace HosteraBackend.API.Rooms.Interfaces.ACL;

/// <summary>
///     The data of a room shared with other bounded contexts.
/// </summary>
/// <param name="Id">The identifier of the room.</param>
/// <param name="PropertyId">The identifier of the property that owns the room.</param>
/// <param name="Number">The number of the room.</param>
/// <param name="RoomTypeId">The identifier of the room type the room belongs to.</param>
public record RoomSummary(int Id, int PropertyId, string Number, int RoomTypeId);
