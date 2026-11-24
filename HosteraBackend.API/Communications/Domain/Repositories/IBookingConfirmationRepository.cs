using HosteraBackend.API.Communications.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Communications.Domain.Repositories;

/// <summary>
///     The booking confirmation repository contract.
/// </summary>
public interface IBookingConfirmationRepository : IBaseRepository<BookingConfirmation>
{
    /// <summary>
    ///     Find the confirmations sent for a booking
    /// </summary>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the confirmations of the booking, starting with the most recent, or empty otherwise.
    /// </returns>
    Task<IEnumerable<BookingConfirmation>> FindByBookingIdAsync(int bookingId,
        CancellationToken cancellationToken = default);
}
