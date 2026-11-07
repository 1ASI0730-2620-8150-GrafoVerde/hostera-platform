using HosteraBackend.API.AccessControl.Interfaces.ACL;
using HosteraBackend.API.Bookings.Domain.Model.Aggregates;

namespace HosteraBackend.API.Bookings.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Anti-corruption service that issues and ends the key cards of guests in the Access Control bounded context.
/// </summary>
/// <param name="accessControlContextFacade">The facade of the Access Control bounded context.</param>
public class ExternalGuestKeyCardService(IAccessControlContextFacade accessControlContextFacade)
{
    /// <summary>
    ///     Issues the key cards of the guest of a booking, valid from now until the end of the stay.
    /// </summary>
    /// <remarks>
    ///     The key cards are saved together with the pending changes of the booking. When they cannot be issued,
    ///     nothing is saved.
    /// </remarks>
    /// <param name="booking">The booking whose guest receives the key cards.</param>
    /// <param name="validFrom">When the key cards start opening the room.</param>
    /// <param name="validUntil">When the key cards stop opening the room.</param>
    /// <param name="cardIds">The four hexadecimal characters of each encoded card.</param>
    /// <param name="issuedBy">The identifier of the operator who issues the key cards.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identifiers of the key cards, or the reason they were not issued.</returns>
    public async Task<(IReadOnlyList<int> CredentialIds, string? ErrorCode)> IssueGuestKeyCardsAsync(Booking booking,
        DateTimeOffset validFrom, DateTimeOffset validUntil, IReadOnlyList<string> cardIds, int issuedBy,
        CancellationToken cancellationToken)
    {
        var issue = await accessControlContextFacade.IssueGuestKeyCardsAsync(booking.PropertyId, booking.Id,
            booking.Code, booking.RoomId, booking.GuestName, validFrom, validUntil, cardIds, issuedBy,
            cancellationToken);
        return (issue.CredentialIds, issue.ErrorCode);
    }

    /// <summary>
    ///     Ends the key cards of a booking that still open its room.
    /// </summary>
    /// <remarks>
    ///     The change is saved together with the pending changes of the booking.
    /// </remarks>
    /// <param name="bookingId">The identifier of the booking.</param>
    /// <param name="endAt">The moment the key cards stop opening the room.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of key cards that were ended.</returns>
    public Task<int> EndGuestKeyCardsAsync(int bookingId, DateTimeOffset endAt, CancellationToken cancellationToken) =>
        accessControlContextFacade.EndGuestKeyCardsAsync(bookingId, endAt, cancellationToken);
}
