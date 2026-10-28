using HosteraBackend.API.Overview.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Overview.Domain.Model.ReadModels;

/// <summary>
///     Booking of a property, as the Overview bounded context reads it from the Bookings bounded context
/// </summary>
/// <param name="Id">The identifier of the booking</param>
/// <param name="Code">The code of the booking</param>
/// <param name="Status">The status of the booking</param>
/// <param name="GuestName">The name of the guest</param>
/// <param name="RoomId">The identifier of the assigned room, or null when no room was assigned yet</param>
/// <param name="CheckInDate">The day of arrival</param>
/// <param name="CheckOutDate">The day of departure</param>
/// <param name="TotalAmount">The total amount of the stay</param>
public record BookingReadModel(
    int Id,
    string Code,
    EBookingStatus Status,
    string GuestName,
    int? RoomId,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    decimal TotalAmount)
{
    /// <summary>
    ///     Gets the nights of the stay, from the day of arrival to the night before the departure.
    /// </summary>
    public IEnumerable<DateOnly> Nights
    {
        get
        {
            for (var night = CheckInDate; night < CheckOutDate; night = night.AddDays(1))
                yield return night;
        }
    }

    /// <summary>
    ///     Indicates whether the booking keeps its room for its nights.
    /// </summary>
    public bool HoldsRoom => Status is EBookingStatus.Pending or EBookingStatus.Confirmed or EBookingStatus.CheckedIn;

    /// <summary>
    ///     Indicates whether the nights of the booking count as sold and earn revenue.
    /// </summary>
    public bool EarnsRevenue =>
        Status is EBookingStatus.Confirmed or EBookingStatus.CheckedIn or EBookingStatus.CheckedOut;

    /// <summary>
    ///     Indicates whether the stay includes the night of a day.
    /// </summary>
    /// <param name="date">The day to evaluate.</param>
    /// <returns>True when the guest sleeps at the property the night of that day.</returns>
    public bool CoversNight(DateOnly date) => CheckInDate <= date && date < CheckOutDate;
}
