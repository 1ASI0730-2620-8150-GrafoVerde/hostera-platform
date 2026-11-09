using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming an <see cref="UpdateBookingResource" /> into an
///     <see cref="UpdateBookingCommand" />.
/// </summary>
public static class UpdateBookingCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an <see cref="UpdateBookingResource" /> to an <see cref="UpdateBookingCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the new data of the booking. Must not be null.</param>
    /// <param name="bookingId">The identifier of the booking to update.</param>
    /// <returns>A new <see cref="UpdateBookingCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the preferred language is not valid.</exception>
    public static UpdateBookingCommand ToCommandFromResource(UpdateBookingResource resource, int bookingId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateBookingCommand(
            bookingId,
            resource.GuestName,
            resource.GuestEmail,
            resource.GuestPhone,
            BookingsEnumNames.Parse<EPreferredLanguage>(resource.PreferredLanguage, "preferred language"),
            resource.CheckInDate,
            resource.CheckOutDate,
            resource.Guests,
            resource.RoomTypeId,
            resource.RoomId,
            resource.RatePlanId,
            resource.GuestRequest);
    }
}
