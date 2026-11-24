using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Communications.Interfaces.ACL;

namespace HosteraBackend.API.Bookings.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Anti-corruption service that emails guests through the Communications bounded context.
/// </summary>
/// <param name="communicationsContextFacade">The facade of the Communications bounded context.</param>
/// <param name="externalRoomService">The service that reads the property and room type of the booking.</param>
/// <param name="logger">The logger that reports a confirmation that could not be sent.</param>
public class ExternalGuestCommunicationService(
    ICommunicationsContextFacade communicationsContextFacade,
    ExternalRoomService externalRoomService,
    ILogger<ExternalGuestCommunicationService> logger)
{
    /// <summary>
    ///     Emails the guest of a saved booking a confirmation in the language the guest prefers.
    /// </summary>
    /// <remarks>
    ///     The booking is already saved, so this never fails: a problem with the email, the email provider or the
    ///     record of the confirmation is logged and the booking stays as it is. The confirmation is sent even when the
    ///     client stops waiting for the response.
    /// </remarks>
    /// <param name="booking">The saved booking.</param>
    /// <returns>True when the email provider accepted the confirmation; false otherwise.</returns>
    public async Task<bool> SendBookingConfirmationAsync(Booking booking)
    {
        var cancellationToken = CancellationToken.None;
        try
        {
            var property = await externalRoomService.FetchPropertyAsync(booking.PropertyId, cancellationToken);
            var roomType = await externalRoomService.FetchRoomTypeAsync(booking.RoomTypeId, cancellationToken);

            return await communicationsContextFacade.SendBookingConfirmationAsync(booking.PropertyId, booking.Id,
                booking.Code, booking.GuestName, booking.GuestEmail,
                booking.PreferredLanguage.ToString().ToLowerInvariant(), property?.Name ?? string.Empty,
                roomType?.Name ?? string.Empty, booking.CheckInDate, booking.CheckOutDate, booking.Guests,
                booking.TotalAmount, property?.Currency ?? string.Empty, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "The confirmation of booking {BookingCode} could not be sent", booking.Code);
            return false;
        }
    }
}
