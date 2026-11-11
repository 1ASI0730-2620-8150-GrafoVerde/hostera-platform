using HosteraBackend.API.Overview.Domain.Model.ReadModels;

namespace HosteraBackend.API.Overview.Domain.Model.ValueObjects;

/// <summary>
///     Number of rooms of a property in each day status
/// </summary>
/// <param name="Counts">The number of rooms of each day status, including the statuses without rooms</param>
public record RoomStatusSummary(IReadOnlyDictionary<ERoomDayStatus, int> Counts)
{
    /// <summary>
    ///     Derives the status of a room on a day.
    /// </summary>
    /// <remarks>
    ///     A booking assigned to the room comes first: a checked-in guest occupies it and a pending or confirmed
    ///     booking holds it. Otherwise, a status period of the day applies; with none, the room is available.
    /// </remarks>
    /// <param name="room">The room to evaluate.</param>
    /// <param name="bookings">The bookings of the property.</param>
    /// <param name="statusPeriods">The status periods of the rooms of the property.</param>
    /// <param name="date">The day to evaluate.</param>
    /// <returns>The status of the room on that day.</returns>
    public static ERoomDayStatus DayStatusOf(
        RoomReadModel room,
        IEnumerable<BookingReadModel> bookings,
        IEnumerable<StatusPeriodReadModel> statusPeriods,
        DateOnly date)
    {
        var assignment = bookings.FirstOrDefault(booking =>
            booking.RoomId == room.Id && booking.HoldsRoom && booking.CoversNight(date));
        if (assignment is not null)
            return assignment.Status == EBookingStatus.CheckedIn ? ERoomDayStatus.Occupied : ERoomDayStatus.Booked;

        var period = statusPeriods.FirstOrDefault(statusPeriod =>
            statusPeriod.RoomId == room.Id && statusPeriod.Covers(date));
        return period?.Status switch
        {
            ERoomStatus.Blocked => ERoomDayStatus.Blocked,
            ERoomStatus.OutOfService => ERoomDayStatus.OutOfService,
            ERoomStatus.NeedsCleaning => ERoomDayStatus.NeedsCleaning,
            _ => ERoomDayStatus.Available
        };
    }

    /// <summary>
    ///     Counts the rooms of a property in each day status.
    /// </summary>
    /// <param name="rooms">The rooms of the property.</param>
    /// <param name="bookings">The bookings of the property.</param>
    /// <param name="statusPeriods">The status periods of the rooms of the property.</param>
    /// <param name="date">The day to evaluate.</param>
    /// <returns>The summary of the day statuses.</returns>
    public static RoomStatusSummary For(
        IEnumerable<RoomReadModel> rooms,
        IReadOnlyCollection<BookingReadModel> bookings,
        IReadOnlyCollection<StatusPeriodReadModel> statusPeriods,
        DateOnly date)
    {
        var counts = Enum.GetValues<ERoomDayStatus>().ToDictionary(status => status, _ => 0);
        foreach (var room in rooms)
            counts[DayStatusOf(room, bookings, statusPeriods, date)]++;

        return new RoomStatusSummary(counts);
    }
}
