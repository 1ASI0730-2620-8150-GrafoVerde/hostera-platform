using HosteraBackend.API.Bookings.Application.QueryServices;
using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Bookings.Domain.Repositories;

namespace HosteraBackend.API.Bookings.Application.Internal.QueryServices;

/// <summary>
///     Represents the booking query service of the Bookings bounded context.
/// </summary>
/// <param name="bookingRepository">
///     The <see cref="IBookingRepository" /> to use.
/// </param>
public class BookingQueryService(IBookingRepository bookingRepository) : IBookingQueryService
{
    /// <inheritdoc />
    public async Task<BookingPage> Handle(GetAllBookingsByPropertyIdQuery query, CancellationToken cancellationToken)
    {
        var bookings = await bookingRepository.FindByPropertyIdAsync(query, cancellationToken);
        var totalCount = await bookingRepository.CountByPropertyIdAsync(query, cancellationToken);
        return new BookingPage(bookings, totalCount);
    }

    /// <inheritdoc />
    public async Task<Booking?> Handle(GetBookingByIdQuery query, CancellationToken cancellationToken)
    {
        return await bookingRepository.FindByIdAsync(query.BookingId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Booking>> Handle(GetRoomHoldsByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await bookingRepository.FindRoomHoldsAsync(query, cancellationToken);
    }
}
