using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="RecordPaymentResource" /> into a
///     <see cref="RecordPaymentCommand" />.
/// </summary>
public static class RecordPaymentCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="RecordPaymentResource" /> to a <see cref="RecordPaymentCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the data of the payment. Must not be null.</param>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="idempotencyKey">The value of the Idempotency-Key header, or null.</param>
    /// <returns>A new <see cref="RecordPaymentCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the payment method is not valid.</exception>
    public static RecordPaymentCommand ToCommandFromResource(RecordPaymentResource resource, int bookingId,
        string? idempotencyKey)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new RecordPaymentCommand(
            bookingId,
            resource.Amount,
            BookingsEnumNames.Parse<EPaymentMethod>(resource.Method, "payment method"),
            resource.PaidAt,
            resource.Reference,
            resource.RecordedBy,
            idempotencyKey);
    }
}
