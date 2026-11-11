using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.Queries;

namespace HosteraBackend.API.Bookings.Application.QueryServices;

/// <summary>
///     Represents the payment query service of the Bookings bounded context.
/// </summary>
public interface IPaymentQueryService
{
    /// <summary>
    ///     Handles the get all payments by booking id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllPaymentsByBookingIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The payments of the booking.</returns>
    Task<IEnumerable<Payment>> Handle(GetAllPaymentsByBookingIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get all payments by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllPaymentsByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The payments received for the bookings of the property.</returns>
    Task<IEnumerable<Payment>> Handle(GetAllPaymentsByPropertyIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get paid amounts by booking ids query.
    /// </summary>
    /// <param name="query">The <see cref="GetPaidAmountsByBookingIdsQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The sum of the payments of each booking that has payments.</returns>
    Task<IReadOnlyDictionary<int, decimal>> Handle(GetPaidAmountsByBookingIdsQuery query,
        CancellationToken cancellationToken);
}
