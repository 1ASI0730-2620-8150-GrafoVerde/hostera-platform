using HosteraBackend.API.AccessControl.Domain.Model.Commands;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

namespace HosteraBackend.API.AccessControl.Domain.Model.Aggregates;

/// <summary>
///     Represents an RFID credential encoded on a key card.
/// </summary>
/// <remarks>
///     This aggregate root represents the access granted to a guest or staff member for a room.
///     A credential starts active and can only be revoked once; a revoked credential never grants access again.
///     Audit timestamps will be managed by the persistence layer once the Shared bounded context is available.
/// </remarks>
public partial class RfidCredential
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected RfidCredential()
    {
        CardNumber = null!;
        HolderName = null!;
        RoomNumber = null!;
        ValidityPeriod = null!;
    }

    /// <summary>
    ///     Creates a new RfidCredential from an issue command.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for IssueRfidCredentialCommand.
    /// </remarks>
    /// <param name="command">The IssueRfidCredentialCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the holder name or room number is empty.</exception>
    public RfidCredential(IssueRfidCredentialCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.HolderName);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.RoomNumber);

        CardNumber = command.CardNumber;
        HolderName = command.HolderName.Trim();
        RoomNumber = command.RoomNumber.Trim();
        ValidityPeriod = command.ValidityPeriod;
        IsRevoked = false;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this credential.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the number of the encoded key card.
    /// </summary>
    public CardNumber CardNumber { get; private set; }

    /// <summary>
    ///     Gets the name of the person who holds the card.
    /// </summary>
    public string HolderName { get; private set; }

    /// <summary>
    ///     Gets the room the card opens.
    /// </summary>
    public string RoomNumber { get; private set; }

    /// <summary>
    ///     Gets the period in which the card grants access.
    /// </summary>
    public ValidityPeriod ValidityPeriod { get; private set; }

    /// <summary>
    ///     Gets whether the credential was revoked.
    /// </summary>
    public bool IsRevoked { get; private set; }

    /// <summary>
    ///     Gets the reason recorded when the credential was revoked.
    /// </summary>
    public string? RevocationReason { get; private set; }

    /// <summary>
    ///     Revokes this credential so the card stops granting access.
    /// </summary>
    /// <param name="command">The RevokeRfidCredentialCommand command</param>
    /// <exception cref="InvalidOperationException">Thrown when the credential was already revoked.</exception>
    public void Revoke(RevokeRfidCredentialCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Reason);

        if (IsRevoked)
            throw new InvalidOperationException("The RFID credential is already revoked.");

        IsRevoked = true;
        RevocationReason = command.Reason.Trim();
    }

    /// <summary>
    ///     Indicates whether the card grants access at the given moment.
    /// </summary>
    /// <param name="moment">The moment of the access attempt.</param>
    /// <returns>True when the credential is not revoked and the moment is within its validity period.</returns>
    public bool GrantsAccessAt(DateTimeOffset moment) => !IsRevoked && ValidityPeriod.Includes(moment);
}
