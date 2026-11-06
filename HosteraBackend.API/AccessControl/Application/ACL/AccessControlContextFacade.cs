using HosteraBackend.API.AccessControl.Application.CommandServices;
using HosteraBackend.API.AccessControl.Domain.Model.Commands;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;
using HosteraBackend.API.AccessControl.Interfaces.ACL;

namespace HosteraBackend.API.AccessControl.Application.ACL;

/// <summary>
///     Facade that other bounded contexts use to issue and end the key cards of guests.
/// </summary>
/// <param name="credentialCommandService">The credential command service</param>
public class AccessControlContextFacade(ICredentialCommandService credentialCommandService)
    : IAccessControlContextFacade
{
    private const string InvalidCardIdError = "InvalidCardId";

    /// <inheritdoc />
    public async Task<GuestKeyCardsIssue> IssueGuestKeyCardsAsync(int propertyId, int bookingId, string bookingCode,
        int roomId, string holderName, DateTimeOffset validFrom, DateTimeOffset validUntil,
        IReadOnlyList<string> cardIds, int issuedBy, CancellationToken cancellationToken)
    {
        IssueGuestKeyCardsCommand command;
        try
        {
            command = new IssueGuestKeyCardsCommand(propertyId, bookingId, bookingCode, roomId, holderName,
                new ValidityPeriod(validFrom, validUntil), cardIds.Select(cardId => new CardId(cardId)).ToList(),
                issuedBy);
        }
        catch (ArgumentException)
        {
            return new GuestKeyCardsIssue([], InvalidCardIdError);
        }

        var result = await credentialCommandService.Handle(command, cancellationToken);
        return result.Fold(
            keyCards => new GuestKeyCardsIssue(keyCards.Select(keyCard => keyCard.Id).ToList(), null),
            error => new GuestKeyCardsIssue([], error.ToString()));
    }

    /// <inheritdoc />
    public async Task<int> EndGuestKeyCardsAsync(int bookingId, DateTimeOffset endAt,
        CancellationToken cancellationToken)
    {
        var result = await credentialCommandService.Handle(new EndGuestKeyCardsCommand(bookingId, endAt),
            cancellationToken);
        return result.Fold(keyCards => keyCards.Count(), _ => 0);
    }
}
