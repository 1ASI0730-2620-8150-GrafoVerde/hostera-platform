namespace HosteraBackend.API.AccessControl.Interfaces.ACL;

/// <summary>
///     Facade that other bounded contexts use to issue and end the key cards of guests.
/// </summary>
public interface IAccessControlContextFacade
{
    /// <summary>
    ///     Issues the key cards encoded for the guest of a booking.
    /// </summary>
    /// <remarks>
    ///     The key cards are saved together with the other pending changes of the request, such as the check-in of the
    ///     booking. When they cannot be issued, nothing is saved.
    /// </remarks>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="bookingCode">The code of the booking, shown with the key cards.</param>
    /// <param name="roomId">The identifier of the room the key cards open.</param>
    /// <param name="holderName">The name of the guest who holds the key cards.</param>
    /// <param name="validFrom">When the key cards start opening the room.</param>
    /// <param name="validUntil">When the key cards stop opening the room.</param>
    /// <param name="cardIds">The four hexadecimal characters of each encoded card.</param>
    /// <param name="issuedBy">The identifier of the operator who issues the key cards.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identifiers of the issued key cards, or the reason they were not issued.</returns>
    Task<GuestKeyCardsIssue> IssueGuestKeyCardsAsync(int propertyId, int bookingId, string bookingCode, int roomId,
        string holderName, DateTimeOffset validFrom, DateTimeOffset validUntil, IReadOnlyList<string> cardIds,
        int issuedBy, CancellationToken cancellationToken);

    /// <summary>
    ///     Ends the key cards of a booking that still open its room.
    /// </summary>
    /// <remarks>
    ///     The change is saved together with the other pending changes of the request, such as the check-out of the
    ///     booking, even when no key card is still usable.
    /// </remarks>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="endAt">The moment the key cards stop opening the room.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of key cards that were ended.</returns>
    Task<int> EndGuestKeyCardsAsync(int bookingId, DateTimeOffset endAt, CancellationToken cancellationToken);
}
