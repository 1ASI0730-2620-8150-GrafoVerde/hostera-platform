namespace HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

/// <summary>
///     The nights a booking holds a room, as the Rooms bounded context reads them from Bookings.
/// </summary>
/// <param name="RoomId">The identifier of the held room.</param>
/// <param name="BookingCode">The code of the booking that holds the room.</param>
/// <param name="CheckInDate">The first held night.</param>
/// <param name="CheckOutDate">The day the room is released; the last held night is the day before.</param>
/// <param name="Occupied">Whether the guest is staying, which makes the nights Occupied instead of Booked.</param>
public record RoomAssignment(int RoomId, string BookingCode, DateOnly CheckInDate, DateOnly CheckOutDate,
    bool Occupied)
{
    /// <summary>
    ///     Gets the status of the room on the held nights.
    /// </summary>
    public EDayStatus DayStatus => Occupied ? EDayStatus.Occupied : EDayStatus.Booked;

    /// <summary>
    ///     Indicates whether the booking holds the room on a night.
    /// </summary>
    /// <param name="date">The night to evaluate.</param>
    /// <returns>True when the night is between the check-in day and the day before check-out.</returns>
    public bool Covers(DateOnly date) => CheckInDate <= date && date < CheckOutDate;

    /// <summary>
    ///     Indicates whether the booking holds the room on at least one day of a range.
    /// </summary>
    /// <param name="startDate">The first day of the range.</param>
    /// <param name="endDate">The last day of the range.</param>
    /// <returns>True when a held night falls in the range.</returns>
    public bool Overlaps(DateOnly startDate, DateOnly endDate) => CheckInDate <= endDate && startDate < CheckOutDate;
}
