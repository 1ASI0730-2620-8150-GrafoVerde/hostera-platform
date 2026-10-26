using HosteraBackend.API.AccessControl.Domain.Model.Commands;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming the credential resources into their commands.
/// </summary>
public static class CredentialCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an <see cref="IssueGuestKeyCardsResource" /> to an <see cref="IssueGuestKeyCardsCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the data of the cards. Must not be null.</param>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <returns>A new <see cref="IssueGuestKeyCardsCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when a card identifier or the validity period is not valid.</exception>
    public static IssueGuestKeyCardsCommand ToCommandFromResource(IssueGuestKeyCardsResource resource, int propertyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new IssueGuestKeyCardsCommand(
            propertyId,
            resource.BookingId,
            resource.BookingCode,
            resource.RoomId,
            resource.HolderName,
            new ValidityPeriod(resource.ValidFrom!.Value, resource.ValidUntil),
            resource.CardIds.Select(cardId => new CardId(cardId)).ToList(),
            resource.IssuedBy);
    }

    /// <summary>
    ///     Converts an <see cref="IssueStaffCredentialResource" /> to an <see cref="IssueStaffCredentialCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the data of the credential. Must not be null.</param>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <returns>A new <see cref="IssueStaffCredentialCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the scope, card identifier or validity period is not valid.</exception>
    public static IssueStaffCredentialCommand ToCommandFromResource(IssueStaffCredentialResource resource,
        int propertyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new IssueStaffCredentialCommand(
            propertyId,
            resource.StaffMemberId,
            AccessControlEnumNames.Parse<EAccessScope>(resource.Scope, "access scope"),
            new ValidityPeriod(resource.ValidFrom!.Value, resource.ValidUntil),
            new CardId(resource.CardId),
            resource.IssuedBy);
    }

    /// <summary>
    ///     Converts a <see cref="ReplaceCredentialResource" /> to a <see cref="ReplaceCredentialCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the new card. Must not be null.</param>
    /// <param name="credentialId">The identifier of the credential to replace.</param>
    /// <returns>A new <see cref="ReplaceCredentialCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the card identifier is not valid.</exception>
    public static ReplaceCredentialCommand ToCommandFromResource(ReplaceCredentialResource resource, int credentialId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new ReplaceCredentialCommand(credentialId, new CardId(resource.CardId), resource.OperatorId);
    }

    /// <summary>
    ///     Converts a <see cref="RevokeCredentialResource" /> to a <see cref="RevokeCredentialCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the revocation. Must not be null.</param>
    /// <param name="credentialId">The identifier of the credential to revoke.</param>
    /// <returns>A new <see cref="RevokeCredentialCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the reason is not valid.</exception>
    public static RevokeCredentialCommand ToCommandFromResource(RevokeCredentialResource resource, int credentialId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new RevokeCredentialCommand(
            credentialId,
            AccessControlEnumNames.Parse<ERevocationReason>(resource.Reason, "revocation reason"),
            resource.Note,
            resource.OperatorId);
    }
}
