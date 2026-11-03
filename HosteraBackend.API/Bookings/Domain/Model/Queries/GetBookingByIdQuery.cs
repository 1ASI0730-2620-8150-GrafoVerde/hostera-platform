namespace HosteraBackend.API.Bookings.Domain.Model.Queries;

/// <summary>
///     Query to get a booking by ID
/// </summary>
/// <param name="BookingId">The booking ID to search</param>
public record GetBookingByIdQuery(int BookingId);
