namespace HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

/// <summary>
///     Checked-in stay resource for REST API
/// </summary>
/// <param name="Booking">The checked-in booking</param>
/// <param name="KeyCardIds">The identifiers of the key cards registered in Access Control for the guest</param>
public record CheckedInStayResource(BookingResource Booking, IEnumerable<int> KeyCardIds);
