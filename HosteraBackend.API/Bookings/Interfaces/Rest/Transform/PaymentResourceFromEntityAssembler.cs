using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="Payment" /> aggregate into a <see cref="PaymentResource" />
///     for REST representation.
/// </summary>
public static class PaymentResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="Payment" /> aggregate to its <see cref="PaymentResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="Payment" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="PaymentResource" /> representing the payment.</returns>
    public static PaymentResource ToResourceFromEntity(Payment entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new PaymentResource(
            entity.Id,
            entity.PropertyId,
            entity.BookingId,
            entity.Amount,
            BookingsEnumNames.ToName(entity.Method),
            entity.PaidAt,
            entity.Reference,
            entity.RecordedBy);
    }
}
