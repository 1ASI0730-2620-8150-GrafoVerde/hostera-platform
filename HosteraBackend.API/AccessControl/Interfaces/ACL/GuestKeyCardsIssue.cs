namespace HosteraBackend.API.AccessControl.Interfaces.ACL;

/// <summary>
///     The outcome of issuing the key cards of a guest, shared with other bounded contexts.
/// </summary>
/// <param name="CredentialIds">The identifiers of the issued key cards; empty when the issue failed.</param>
/// <param name="ErrorCode">
///     The reason the key cards were not issued, such as DuplicateCardId or InvalidCardId, or null when they were.
/// </param>
public record GuestKeyCardsIssue(IReadOnlyList<int> CredentialIds, string? ErrorCode)
{
    /// <summary>
    ///     Gets whether every key card was issued.
    /// </summary>
    public bool Succeeded => ErrorCode is null;
}
