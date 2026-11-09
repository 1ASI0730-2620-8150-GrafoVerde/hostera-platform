namespace HosteraBackend.API.Bookings.Domain.Model.ReadModels;

/// <summary>
///     A room that a booking can hold, as the Bookings context reads it from Rooms.
/// </summary>
/// <param name="Id">The identifier of the room.</param>
/// <param name="PropertyId">The identifier of the property that owns the room.</param>
/// <param name="RoomTypeId">The identifier of the room type the room belongs to.</param>
public record BookableRoom(int Id, int PropertyId, int RoomTypeId);
