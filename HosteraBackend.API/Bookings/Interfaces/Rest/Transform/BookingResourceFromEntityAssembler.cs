using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="Booking" /> aggregate into a <see cref="BookingResource" />
///     for REST representation.
/// </summary>
public static class BookingResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="Booking" /> aggregate to its <see cref="BookingResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="Booking" /> to convert. Must not be null.</param>
    /// <param name="paidAmount">The sum of the payments of the booking.</param>
    /// <returns>A <see cref="BookingResource" /> representing the booking and its balance.</returns>
    public static BookingResource ToResourceFromEntity(Booking entity, decimal paidAmount)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new BookingResource(
            entity.Id,
            entity.PropertyId,
            entity.Code,
            BookingsEnumNames.ToName(entity.Status),
            entity.GuestName,
            entity.GuestEmail,
            entity.GuestPhone,
            BookingsEnumNames.ToName(entity.PreferredLanguage),
            entity.CheckInDate,
            entity.CheckOutDate,
            entity.Guests,
            entity.RoomTypeId,
            entity.RoomId,
            entity.RatePlanId,
            entity.TotalAmount,
            paidAmount,
            entity.BalanceDue(paidAmount),
            BookingsEnumNames.ToName(entity.PaymentStatus(paidAmount)),
            entity.GuestRequest,
            entity.CreatedAt,
            entity.ConfirmedAt,
            entity.ConfirmedBy,
            entity.CancelledAt,
            entity.CancelledBy,
            entity.CancellationReason is { } reason ? BookingsEnumNames.ToName(reason) : null,
            entity.CancellationNote,
            entity.NoShowAt,
            entity.NoShowBy);
    }
}
