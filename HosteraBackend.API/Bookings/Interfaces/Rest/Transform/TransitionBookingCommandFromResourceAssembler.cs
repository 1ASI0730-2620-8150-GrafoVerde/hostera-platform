using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="TransitionBookingResource" /> into a
///     <see cref="TransitionBookingCommand" />.
/// </summary>
public static class TransitionBookingCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="TransitionBookingResource" /> to a <see cref="TransitionBookingCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the target status and its details. Must not be null.</param>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <returns>A new <see cref="TransitionBookingCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the status or the cancellation reason is not valid.</exception>
    public static TransitionBookingCommand ToCommandFromResource(TransitionBookingResource resource, int bookingId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new TransitionBookingCommand(
            bookingId,
            BookingsEnumNames.Parse<EBookingStatus>(resource.Status, "status"),
            resource.OperatorId,
            string.IsNullOrWhiteSpace(resource.CancellationReason)
                ? null
                : BookingsEnumNames.Parse<ECancellationReason>(resource.CancellationReason, "cancellation reason"),
            resource.CancellationNote);
    }
}
