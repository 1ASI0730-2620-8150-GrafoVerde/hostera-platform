using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="CheckInBookingResource" /> into a
///     <see cref="CheckInBookingCommand" />.
/// </summary>
public static class CheckInBookingCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="CheckInBookingResource" /> to a <see cref="CheckInBookingCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the identity document and key cards. Must not be null.</param>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <returns>A new <see cref="CheckInBookingCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the document type is not valid.</exception>
    public static CheckInBookingCommand ToCommandFromResource(CheckInBookingResource resource, int bookingId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CheckInBookingCommand(
            bookingId,
            BookingsEnumNames.Parse<EDocumentType>(resource.DocumentType, "document type"),
            resource.DocumentNumber,
            resource.DocumentVerified,
            resource.KeyCardIds,
            resource.OperatorId);
    }
}
