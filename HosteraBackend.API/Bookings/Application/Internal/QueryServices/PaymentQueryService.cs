using HosteraBackend.API.Bookings.Application.QueryServices;
using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Bookings.Domain.Repositories;

namespace HosteraBackend.API.Bookings.Application.Internal.QueryServices;

/// <summary>
///     Represents the payment query service of the Bookings bounded context.
/// </summary>
/// <param name="paymentRepository">
///     The <see cref="IPaymentRepository" /> to use.
/// </param>
public class PaymentQueryService(IPaymentRepository paymentRepository) : IPaymentQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Payment>> Handle(GetAllPaymentsByBookingIdQuery query,
        CancellationToken cancellationToken)
    {
        return await paymentRepository.FindByBookingIdAsync(query.BookingId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Payment>> Handle(GetAllPaymentsByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await paymentRepository.FindByPropertyIdAsync(query.PropertyId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<int, decimal>> Handle(GetPaidAmountsByBookingIdsQuery query,
        CancellationToken cancellationToken)
    {
        return await paymentRepository.SumAmountsByBookingIdsAsync(query.BookingIds, cancellationToken);
    }
}
