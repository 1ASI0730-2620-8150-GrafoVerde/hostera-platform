using HosteraBackend.API.Communications.Application.CommandServices;
using HosteraBackend.API.Communications.Domain.Model.Commands;
using HosteraBackend.API.Communications.Domain.Model.ValueObjects;
using HosteraBackend.API.Communications.Interfaces.ACL;

namespace HosteraBackend.API.Communications.Application.ACL;

/// <summary>
///     Facade that other bounded contexts use to email guests.
/// </summary>
/// <param name="bookingConfirmationCommandService">The booking confirmation command service</param>
public class CommunicationsContextFacade(IBookingConfirmationCommandService bookingConfirmationCommandService)
    : ICommunicationsContextFacade
{
    /// <inheritdoc />
    public async Task<bool> SendBookingConfirmationAsync(int propertyId, int bookingId, string bookingCode,
        string guestName, string guestEmail, string languageCode, string propertyName, string roomTypeName,
        DateOnly checkInDate, DateOnly checkOutDate, int guests, decimal totalAmount, string currency,
        CancellationToken cancellationToken)
    {
        var command = new SendBookingConfirmationCommand(propertyId, bookingId, bookingCode, guestName, guestEmail,
            LanguageOf(languageCode), propertyName, roomTypeName, checkInDate, checkOutDate, guests, totalAmount,
            currency);

        var result = await bookingConfirmationCommandService.Handle(command, cancellationToken);
        return result.Fold(confirmation => confirmation.Status == EDeliveryStatus.Sent, _ => false);
    }

    private static EEmailLanguage LanguageOf(string languageCode) =>
        Enum.TryParse<EEmailLanguage>(languageCode?.Trim(), true, out var language) && Enum.IsDefined(language)
            ? language
            : EEmailLanguage.En;
}
