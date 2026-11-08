using HosteraBackend.API.Overview.Domain.Model.ReadModels;

namespace HosteraBackend.API.Overview.Domain.Model.ValueObjects;

/// <summary>
///     Booking that arrives at a property on a day
/// </summary>
/// <param name="BookingId">The identifier of the booking</param>
/// <param name="BookingCode">The code of the booking</param>
/// <param name="GuestName">The name of the guest</param>
/// <param name="RoomNumber">The number of the assigned room, or null when no room was assigned yet</param>
/// <param name="CheckInDate">The day of arrival</param>
/// <param name="CheckOutDate">The day of departure</param>
/// <param name="Arrived">Whether the guest already checked in</param>
/// <param name="Status">The status of the booking, such as Pending when it still awaits confirmation</param>
/// <param name="BalanceDue">What the booking still owes</param>
public record ArrivalSummary(
    int BookingId,
    string BookingCode,
    string GuestName,
    string? RoomNumber,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    bool Arrived,
    EBookingStatus Status,
    decimal BalanceDue)
{
    /// <summary>
    ///     Lists the arrivals of a day: the bookings due that day and those that already checked in that day.
    /// </summary>
    /// <param name="bookings">The bookings of the property.</param>
    /// <param name="rooms">The rooms of the property, used to show the number of the assigned room.</param>
    /// <param name="date">The day of the arrivals.</param>
    /// <returns>The arrivals, with the guests still expected first.</returns>
    public static IReadOnlyList<ArrivalSummary> For(
        IEnumerable<BookingReadModel> bookings,
        IEnumerable<RoomReadModel> rooms,
        DateOnly date)
    {
        var roomNumbers = rooms.ToDictionary(room => room.Id, room => room.Number);

        return bookings
            .Where(booking => booking.CheckInDate == date && booking.HoldsRoom)
            .Select(booking => new ArrivalSummary(
                booking.Id,
                booking.Code,
                booking.GuestName,
                booking.RoomId is { } roomId ? roomNumbers.GetValueOrDefault(roomId) : null,
                booking.CheckInDate,
                booking.CheckOutDate,
                booking.Status == EBookingStatus.CheckedIn,
                booking.Status,
                booking.BalanceDue))
            .OrderBy(arrival => arrival.Arrived)
            .ThenBy(arrival => arrival.GuestName)
            .ToList();
    }
}
