namespace HosteraBackend.API.Bookings.Domain.Model.ReadModels;

/// <summary>
///     A room type that a booking can use, as the Bookings context reads it from Rooms.
/// </summary>
/// <param name="Id">The identifier of the room type.</param>
/// <param name="PropertyId">The identifier of the property that owns the room type.</param>
/// <param name="Capacity">The largest number of guests a room of this type can host.</param>
/// <param name="IsActive">Whether new bookings can use the room type.</param>
public record BookableRoomType(int Id, int PropertyId, int Capacity, bool IsActive);
