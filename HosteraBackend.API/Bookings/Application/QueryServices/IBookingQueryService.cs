using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.Queries;

namespace HosteraBackend.API.Bookings.Application.QueryServices;

/// <summary>
///     Represents the booking query service of the Bookings bounded context.
/// </summary>
public interface IBookingQueryService
{
    /// <summary>
    ///     Handles the get all bookings by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllBookingsByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching bookings and their total count.</returns>
    Task<BookingPage> Handle(GetAllBookingsByPropertyIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get booking by id query.
    /// </summary>
    /// <param name="query">The <see cref="GetBookingByIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The booking, or null when it does not exist.</returns>
    Task<Booking?> Handle(GetBookingByIdQuery query, CancellationToken cancellationToken);
}
