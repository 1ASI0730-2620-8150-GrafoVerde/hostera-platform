namespace HosteraBackend.API.Communications.Interfaces.ACL;

/// <summary>
///     Facade that other bounded contexts use to email guests.
/// </summary>
public interface ICommunicationsContextFacade
{
    /// <summary>
    ///     Emails the guest of a new booking a confirmation with the details of the stay.
    /// </summary>
    /// <remarks>
    ///     The outcome is recorded as a booking confirmation and saved on its own, so call it after the booking is
    ///     saved. An email that the provider rejects, or that is skipped because sending is not configured, is not an
    ///     error.
    /// </remarks>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="bookingCode">The code of the booking, such as BKG-1048.</param>
    /// <param name="guestName">The full name of the guest.</param>
    /// <param name="guestEmail">The email address of the guest.</param>
    /// <param name="languageCode">The language of the guest, en or es; any other value is written in English.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="roomTypeName">The name of the booked room type.</param>
    /// <param name="checkInDate">The day the first night begins.</param>
    /// <param name="checkOutDate">The day the guest leaves after the last night.</param>
    /// <param name="guests">The number of people staying.</param>
    /// <param name="totalAmount">The price of all nights of the stay.</param>
    /// <param name="currency">The three-letter code of the currency of the property.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True when the email provider accepted the confirmation; false otherwise.</returns>
    Task<bool> SendBookingConfirmationAsync(int propertyId, int bookingId, string bookingCode, string guestName,
        string guestEmail, string languageCode, string propertyName, string roomTypeName, DateOnly checkInDate,
        DateOnly checkOutDate, int guests, decimal totalAmount, string currency, CancellationToken cancellationToken);
}
