namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Room availability resource for REST API
/// </summary>
/// <param name="RoomId">The unique identifier of the room</param>
/// <param name="Number">The number of the room</param>
/// <param name="RoomTypeId">The identifier of the room type the room belongs to</param>
/// <param name="Floor">The floor of the room, or null when it is not recorded</param>
/// <param name="Days">The status of the room on each day of the week, in date order</param>
public record RoomAvailabilityResource(
    int RoomId,
    string Number,
    int RoomTypeId,
    int? Floor,
    IEnumerable<RoomDayStatusResource> Days);
