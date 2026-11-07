using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="CheckOutBookingResource" /> into a
///     <see cref="CheckOutBookingCommand" />.
/// </summary>
public static class CheckOutBookingCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="CheckOutBookingResource" /> to a <see cref="CheckOutBookingCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the room condition and departure note. Must not be null.</param>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <returns>A new <see cref="CheckOutBookingCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the room condition is not valid.</exception>
    public static CheckOutBookingCommand ToCommandFromResource(CheckOutBookingResource resource, int bookingId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CheckOutBookingCommand(
            bookingId,
            BookingsEnumNames.Parse<ERoomCondition>(resource.RoomCondition, "room condition"),
            resource.DepartureNote,
            resource.OperatorId);
    }
}
