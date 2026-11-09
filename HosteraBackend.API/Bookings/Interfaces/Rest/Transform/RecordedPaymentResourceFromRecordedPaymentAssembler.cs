using HosteraBackend.API.Bookings.Application.CommandServices;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="RecordedPayment" /> into a
///     <see cref="RecordedPaymentResource" /> for REST representation.
/// </summary>
public static class RecordedPaymentResourceFromRecordedPaymentAssembler
{
    /// <summary>
    ///     Converts a <see cref="RecordedPayment" /> to its <see cref="RecordedPaymentResource" />.
    /// </summary>
    /// <param name="recordedPayment">The <see cref="RecordedPayment" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="RecordedPaymentResource" /> with the payment and the new balance of its booking.</returns>
    public static RecordedPaymentResource ToResourceFromRecordedPayment(RecordedPayment recordedPayment)
    {
        ArgumentNullException.ThrowIfNull(recordedPayment);
        return new RecordedPaymentResource(
            PaymentResourceFromEntityAssembler.ToResourceFromEntity(recordedPayment.Payment),
            recordedPayment.BalanceDue,
            BookingsEnumNames.ToName(recordedPayment.PaymentStatus));
    }
}
