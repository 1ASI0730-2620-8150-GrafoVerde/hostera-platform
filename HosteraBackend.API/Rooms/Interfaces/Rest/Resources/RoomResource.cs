namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Room resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the room</param>
/// <param name="PropertyId">The identifier of the property that owns the room</param>
/// <param name="Number">The number of the room</param>
/// <param name="RoomTypeId">The identifier of the room type the room belongs to</param>
/// <param name="Floor">The floor of the room, or null when it is not recorded</param>
public record RoomResource(int Id, int PropertyId, string Number, int RoomTypeId, int? Floor);
