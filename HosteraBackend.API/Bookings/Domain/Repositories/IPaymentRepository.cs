using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Bookings.Domain.Repositories;

/// <summary>
///     The payment repository contract.
/// </summary>
public interface IPaymentRepository : IBaseRepository<Payment>
{
    /// <summary>
    ///     Find the payments of a booking
    /// </summary>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An Enumerable containing the payments ordered by when they were received, or empty otherwise.</returns>
    Task<IEnumerable<Payment>> FindByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the payments received for the bookings of a property
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An Enumerable containing the payments ordered by when they were received, or empty otherwise.</returns>
    Task<IEnumerable<Payment>> FindByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the payment that a request with the same idempotency key recorded for a booking
    /// </summary>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="idempotencyKey">The key sent by the client.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the payment, or null when no request used the key.</returns>
    Task<Payment?> FindByBookingIdAndIdempotencyKeyAsync(int bookingId, string idempotencyKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sum the payments of each of several bookings
    /// </summary>
    /// <param name="bookingIds">The identifiers of the bookings.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the paid amount of each booking that has payments.</returns>
    Task<IReadOnlyDictionary<int, decimal>> SumAmountsByBookingIdsAsync(IReadOnlyCollection<int> bookingIds,
        CancellationToken cancellationToken = default);
}
