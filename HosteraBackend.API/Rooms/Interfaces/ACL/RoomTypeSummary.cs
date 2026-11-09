namespace HosteraBackend.API.Rooms.Interfaces.ACL;

/// <summary>
///     The data of a room type shared with other bounded contexts.
/// </summary>
/// <param name="Id">The identifier of the room type.</param>
/// <param name="PropertyId">The identifier of the property that owns the room type.</param>
/// <param name="Capacity">The number of guests a room of this type can host.</param>
/// <param name="IsActive">Whether new rooms and bookings can use the room type.</param>
public record RoomTypeSummary(int Id, int PropertyId, int Capacity, bool IsActive);
