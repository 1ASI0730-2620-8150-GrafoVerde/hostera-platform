namespace HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

/// <summary>
///     Checked-out stay resource for REST API
/// </summary>
/// <param name="Booking">The checked-out booking</param>
/// <param name="EndedKeyCards">The number of key cards that stopped opening the room</param>
/// <param name="RoomMarkedForCleaning">Whether the room was marked as needs-cleaning on the departure day</param>
public record CheckedOutStayResource(BookingResource Booking, int EndedKeyCards, bool RoomMarkedForCleaning);
