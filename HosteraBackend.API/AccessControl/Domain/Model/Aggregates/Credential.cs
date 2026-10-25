using HosteraBackend.API.AccessControl.Domain.Model.Commands;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

namespace HosteraBackend.API.AccessControl.Domain.Model.Aggregates;

/// <summary>
///     Represents an RFID card that opens the room of a guest or the areas of a staff member.
/// </summary>
/// <remarks>
///     A guest key card belongs to a booking and a room and always has an end of validity.
///     A staff credential belongs to a staff member, has an access scope and can be permanent.
///     Its status at a given moment is derived from its validity period and its revocation.
/// </remarks>
public partial class Credential
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Credential()
    {
        CardId = null!;
        HolderName = null!;
        ValidityPeriod = null!;
    }

    /// <summary>
    ///     Creates a guest key card from an issue command.
    /// </summary>
    /// <remarks>
    ///     This constructor is called once for each card of the IssueGuestKeyCardsCommand.
    /// </remarks>
    /// <param name="command">The IssueGuestKeyCardsCommand command</param>
    /// <param name="cardId">The CardId of the encoded card</param>
    /// <exception cref="ArgumentNullException">Thrown when the command or the card identifier is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the holder name or booking code is empty, or the validity period has no end.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an identifier is not positive.</exception>
    public Credential(IssueGuestKeyCardsCommand command, CardId cardId)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(cardId);
        ArgumentNullException.ThrowIfNull(command.ValidityPeriod);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PropertyId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.BookingId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.RoomId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.IssuedBy);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.BookingCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.HolderName);

        if (command.ValidityPeriod.ValidUntil is null)
            throw new ArgumentException("A guest key card must have an end of validity.", nameof(command));

        PropertyId = command.PropertyId;
        CardId = cardId;
        Type = ECredentialType.GuestKeyCard;
        HolderName = command.HolderName.Trim();
        BookingId = command.BookingId;
        BookingCode = command.BookingCode.Trim();
        RoomId = command.RoomId;
        ValidityPeriod = command.ValidityPeriod;
        IssuedAt = DateTimeOffset.UtcNow;
        IssuedBy = command.IssuedBy;
    }

    /// <summary>
    ///     Creates a staff credential from an issue command.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for IssueStaffCredentialCommand.
    /// </remarks>
    /// <param name="command">The IssueStaffCredentialCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when the command, card identifier or validity period is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the holder name is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an identifier is not positive.</exception>
    public Credential(IssueStaffCredentialCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.CardId);
        ArgumentNullException.ThrowIfNull(command.ValidityPeriod);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PropertyId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.StaffMemberId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.IssuedBy);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.HolderName);

        PropertyId = command.PropertyId;
        CardId = command.CardId;
        Type = ECredentialType.StaffCredential;
        HolderName = command.HolderName.Trim();
        StaffMemberId = command.StaffMemberId;
        Scope = command.Scope;
        ValidityPeriod = command.ValidityPeriod;
        IssuedAt = DateTimeOffset.UtcNow;
        IssuedBy = command.IssuedBy;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this credential.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property that owns the credential.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the identifier written to the card.
    /// </summary>
    public CardId CardId { get; private set; }

    /// <summary>
    ///     Gets the kind of credential.
    /// </summary>
    public ECredentialType Type { get; private set; }

    /// <summary>
    ///     Gets the name of the person who holds the credential.
    /// </summary>
    public string HolderName { get; private set; }

    /// <summary>
    ///     Gets the identifier of the booking of a guest key card.
    /// </summary>
    public int? BookingId { get; private set; }

    /// <summary>
    ///     Gets the code of the booking of a guest key card.
    /// </summary>
    public string? BookingCode { get; private set; }

    /// <summary>
    ///     Gets the identifier of the room a guest key card opens.
    /// </summary>
    public int? RoomId { get; private set; }

    /// <summary>
    ///     Gets the identifier of the staff member who holds a staff credential.
    /// </summary>
    public int? StaffMemberId { get; private set; }

    /// <summary>
    ///     Gets the areas a staff credential opens.
    /// </summary>
    public EAccessScope? Scope { get; private set; }

    /// <summary>
    ///     Gets the period in which the credential grants access.
    /// </summary>
    public ValidityPeriod ValidityPeriod { get; private set; }

    /// <summary>
    ///     Gets the moment the credential was issued.
    /// </summary>
    public DateTimeOffset IssuedAt { get; private set; }

    /// <summary>
    ///     Gets the identifier of the operator who issued the credential.
    /// </summary>
    public int IssuedBy { get; private set; }

    /// <summary>
    ///     Gets the revocation of the credential, or null when it was not revoked.
    /// </summary>
    public Revocation? Revocation { get; private set; }

    /// <summary>
    ///     Derives the status of the credential at the given moment.
    /// </summary>
    /// <param name="moment">The moment to evaluate.</param>
    /// <returns>The status of the credential at that moment.</returns>
    public ECredentialStatus StatusAt(DateTimeOffset moment)
    {
        if (Revocation is not null) return ECredentialStatus.Revoked;
        if (moment < ValidityPeriod.ValidFrom) return ECredentialStatus.Scheduled;
        if (moment >= ValidityPeriod.ValidUntil) return ECredentialStatus.Expired;
        return ECredentialStatus.Active;
    }

    /// <summary>
    ///     Indicates whether the credential can still open doors at the given moment or later.
    /// </summary>
    /// <param name="moment">The moment to evaluate.</param>
    /// <returns>True when the credential is active or scheduled.</returns>
    public bool IsUsableAt(DateTimeOffset moment) =>
        StatusAt(moment) is ECredentialStatus.Active or ECredentialStatus.Scheduled;

    /// <summary>
    ///     Revokes the credential immediately.
    /// </summary>
    /// <param name="reason">The reason for the revocation.</param>
    /// <param name="note">The internal note, required for the Other reason.</param>
    /// <param name="operatorId">The identifier of the operator who revokes the credential.</param>
    /// <exception cref="InvalidOperationException">Thrown when the credential is expired or already revoked.</exception>
    public void Revoke(ERevocationReason reason, string? note, int operatorId)
    {
        var now = DateTimeOffset.UtcNow;

        if (!IsUsableAt(now))
            throw new InvalidOperationException("Only an active or scheduled credential can be revoked.");

        Revocation = new Revocation(reason, note, now, operatorId);
    }

    /// <summary>
    ///     Ends the access of a usable credential at the given moment, such as the check-out of a guest.
    /// </summary>
    /// <param name="moment">The moment the access ends.</param>
    public void EndAt(DateTimeOffset moment)
    {
        if (!IsUsableAt(moment)) return;

        ValidityPeriod = ValidityPeriod.EndingAt(moment);
    }
}
