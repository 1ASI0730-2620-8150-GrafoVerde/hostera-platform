namespace HosteraBackend.API.Bookings.Interfaces.ACL;

/// <summary>
///     A booking that holds a room, shared with other bounded contexts.
/// </summary>
/// <param name="BookingId">The identifier of the booking.</param>
/// <param name="BookingCode">The code of the booking.</param>
/// <param name="RoomId">The identifier of the held room.</param>
/// <param name="CheckInDate">The first night the room is held.</param>
/// <param name="CheckOutDate">The day the room is released; the last held night is the day before.</param>
/// <param name="IsCheckedIn">Whether the guest is staying, which makes the nights Occupied instead of Booked.</param>
public record RoomHoldSummary(
    int BookingId,
    string BookingCode,
    int RoomId,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    bool IsCheckedIn);
