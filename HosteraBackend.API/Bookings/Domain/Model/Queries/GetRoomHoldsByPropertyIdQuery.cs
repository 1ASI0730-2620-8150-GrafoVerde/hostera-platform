namespace HosteraBackend.API.Bookings.Domain.Model.Queries;

/// <summary>
///     Query to get the Pending, Confirmed and Checked in bookings that hold a room of a property on a night of a
///     range
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
/// <param name="From">The first night of the range</param>
/// <param name="To">The last night of the range</param>
/// <param name="RoomId">Keep only the bookings of this room, or null for every room</param>
public record GetRoomHoldsByPropertyIdQuery(int PropertyId, DateOnly From, DateOnly To, int? RoomId = null);
