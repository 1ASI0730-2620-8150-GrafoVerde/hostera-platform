namespace HosteraBackend.API.Overview.Interfaces.Rest.Resources;

/// <summary>
///     Arrival summary resource for REST API
/// </summary>
/// <param name="BookingId">The identifier of the booking</param>
/// <param name="BookingCode">The code of the booking</param>
/// <param name="GuestName">The name of the guest</param>
/// <param name="RoomNumber">The number of the assigned room, or null when no room was assigned yet</param>
/// <param name="CheckInDate">The day of arrival</param>
/// <param name="CheckOutDate">The day of departure</param>
/// <param name="Arrived">Whether the guest already checked in</param>
/// <param name="Status">The status of the booking: pending, confirmed or checked-in</param>
/// <param name="BalanceDue">What the booking still owes</param>
public record ArrivalSummaryResource(
    int BookingId,
    string BookingCode,
    string GuestName,
    string? RoomNumber,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    bool Arrived,
    string Status,
    decimal BalanceDue);
