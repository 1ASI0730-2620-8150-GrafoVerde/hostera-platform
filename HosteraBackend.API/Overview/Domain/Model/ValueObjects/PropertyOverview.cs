using HosteraBackend.API.Overview.Domain.Model.ReadModels;

namespace HosteraBackend.API.Overview.Domain.Model.ValueObjects;

/// <summary>
///     Summary of how full a property is tonight and which of its rooms need attention today
/// </summary>
/// <param name="PropertyId">The identifier of the property</param>
/// <param name="Name">The name of the property</param>
/// <param name="RoomsCount">The rooms of the property</param>
/// <param name="HeldRooms">The rooms held by a booking tonight</param>
/// <param name="RoomsNeedingCleaning">The rooms that need cleaning today</param>
/// <param name="RoomsOutOfOrder">The rooms blocked or out of service today</param>
public record PropertyOverview(
    int PropertyId,
    string Name,
    int RoomsCount,
    int HeldRooms,
    int RoomsNeedingCleaning,
    int RoomsOutOfOrder)
{
    /// <summary>
    ///     Gets the share of the rooms held by a booking tonight, between 0 and 1.
    /// </summary>
    public decimal OccupancyRate => RoomsCount == 0 ? 0 : Math.Round((decimal)HeldRooms / RoomsCount, 4);

    /// <summary>
    ///     Gets the rooms that can still be sold tonight.
    /// </summary>
    public int AvailableRooms => Math.Max(RoomsCount - HeldRooms - RoomsOutOfOrder, 0);

    /// <summary>
    ///     Gets the rooms that need attention today: cleaning, a block or repairs.
    /// </summary>
    public int AlertsCount => RoomsNeedingCleaning + RoomsOutOfOrder;

    /// <summary>
    ///     Builds the overview of a property for a day.
    /// </summary>
    /// <param name="property">The property to summarize.</param>
    /// <param name="rooms">The rooms of the property.</param>
    /// <param name="bookings">The bookings of the property that include the day.</param>
    /// <param name="statusPeriods">The status periods of the rooms that include the day.</param>
    /// <param name="date">The day of the overview.</param>
    /// <returns>The overview of the property.</returns>
    public static PropertyOverview For(
        PropertyReadModel property,
        IReadOnlyCollection<RoomReadModel> rooms,
        IEnumerable<BookingReadModel> bookings,
        IEnumerable<StatusPeriodReadModel> statusPeriods,
        DateOnly date)
    {
        var roomIds = rooms.Select(room => room.Id).ToHashSet();
        var heldRooms = bookings
            .Where(booking => booking.RoomId is { } roomId && roomIds.Contains(roomId)
                                                         && booking.HoldsRoom
                                                         && booking.CoversNight(date))
            .Select(booking => booking.RoomId)
            .Distinct()
            .Count();
        var todayPeriods = statusPeriods
            .Where(period => roomIds.Contains(period.RoomId) && period.Covers(date))
            .ToList();

        return new PropertyOverview(
            property.Id,
            property.Name,
            rooms.Count,
            heldRooms,
            todayPeriods.Count(period => period.Status == ERoomStatus.NeedsCleaning),
            todayPeriods.Count(period => period.Status != ERoomStatus.NeedsCleaning));
    }
}
