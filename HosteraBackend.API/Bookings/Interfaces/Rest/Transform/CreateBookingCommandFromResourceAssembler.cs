using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="CreateBookingResource" /> into a
///     <see cref="CreateBookingCommand" />.
/// </summary>
public static class CreateBookingCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="CreateBookingResource" /> to a <see cref="CreateBookingCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the data of the booking. Must not be null.</param>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <returns>A new <see cref="CreateBookingCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the preferred language is not valid.</exception>
    public static CreateBookingCommand ToCommandFromResource(CreateBookingResource resource, int propertyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateBookingCommand(
            propertyId,
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
