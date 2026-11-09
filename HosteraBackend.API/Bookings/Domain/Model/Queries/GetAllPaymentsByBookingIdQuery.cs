namespace HosteraBackend.API.Bookings.Domain.Model.Queries;

/// <summary>
///     Query to get the payments of a booking
/// </summary>
/// <param name="BookingId">The booking ID to search</param>
public record GetAllPaymentsByBookingIdQuery(int BookingId);
