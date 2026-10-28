using HosteraBackend.API.AccessControl.Domain.Model.Commands;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

namespace HosteraBackend.API.AccessControl.Domain.Model.Aggregates;

/// <summary>
///     Represents a credential presented at an RFID reader of a property and whether the door opened.
/// </summary>
/// <remarks>
///     An access event is immutable: once recorded, its moment, credential, access point and result never change.
///     The result is decided from the status of the credential at that moment and from the areas it opens.
/// </remarks>
public partial class AccessEvent
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected AccessEvent()
    {
        CardId = null!;
        HolderName = null!;
    }

    /// <summary>
    ///     Creates an access event from a record command and the credential written to the card.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for RecordAccessEventCommand.
    /// </remarks>
    /// <param name="command">The RecordAccessEventCommand command</param>
    /// <param name="credential">The Credential written to the presented card</param>
    /// <exception cref="ArgumentNullException">Thrown when the command, card identifier or credential is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the credential belongs to another property or card.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an identifier is not positive.</exception>
    public AccessEvent(RecordAccessEventCommand command, Credential credential)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.CardId);
        ArgumentNullException.ThrowIfNull(credential);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PropertyId);
        if (command.RoomId is { } roomId)
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomId, nameof(command.RoomId));

        if (credential.PropertyId != command.PropertyId || credential.CardId != command.CardId)
            throw new ArgumentException("The credential does not belong to the presented card.", nameof(credential));

        PropertyId = command.PropertyId;
        CredentialId = credential.Id;
        CardId = command.CardId;
        HolderType = credential.Type == ECredentialType.GuestKeyCard ? EHolderType.Guest : EHolderType.Staff;
        HolderName = credential.HolderName;
        // The room of a room door reader, or the room the guest key card opens for the other readers
        RoomId = command.RoomId ?? credential.RoomId;
        AccessPoint = command.AccessPoint;
        OccurredAt = command.OccurredAt;
        DenialReason = DenialReasonFor(credential, command.AccessPoint, command.RoomId, command.OccurredAt);
        Result = DenialReason is null ? EAccessResult.Granted : EAccessResult.Denied;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this access event.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property of the reader.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the identifier of the credential written to the presented card.
    /// </summary>
    public int? CredentialId { get; private set; }

    /// <summary>
    ///     Gets the identifier read from the card.
    /// </summary>
    public CardId CardId { get; private set; }

    /// <summary>
    ///     Gets whether the person who presented the card is a guest or a staff member.
    /// </summary>
    public EHolderType HolderType { get; private set; }

    /// <summary>
    ///     Gets the name of the person who holds the credential.
    /// </summary>
    public string HolderName { get; private set; }

    /// <summary>
    ///     Gets the identifier of the room of the door, or of the room the guest key card opens.
    /// </summary>
    public int? RoomId { get; private set; }

    /// <summary>
    ///     Gets the access point of the reader.
    /// </summary>
    public EAccessPoint AccessPoint { get; private set; }

    /// <summary>
    ///     Gets whether the door opened.
    /// </summary>
    public EAccessResult Result { get; private set; }

    /// <summary>
    ///     Gets the reason the door stayed closed, or null when access was granted.
    /// </summary>
    public EDenialReason? DenialReason { get; private set; }

    /// <summary>
    ///     Gets the moment the reader received the card.
    /// </summary>
    public DateTimeOffset OccurredAt { get; private set; }

    /// <summary>
    ///     Decides why a credential cannot open an access point at a given moment.
    /// </summary>
    /// <param name="credential">The credential presented at the reader.</param>
    /// <param name="accessPoint">The access point of the reader.</param>
    /// <param name="roomId">The room of a room door reader.</param>
    /// <param name="moment">The moment the reader received the card.</param>
    /// <returns>The reason for the denial, or null when the credential opens the access point.</returns>
    private static EDenialReason? DenialReasonFor(Credential credential, EAccessPoint accessPoint, int? roomId,
        DateTimeOffset moment)
    {
        switch (credential.StatusAt(moment))
        {
            case ECredentialStatus.Revoked: return EDenialReason.Revoked;
            case ECredentialStatus.Scheduled: return EDenialReason.NotActiveYet;
            case ECredentialStatus.Expired: return EDenialReason.Expired;
        }

        return OpensAccessPoint(credential, accessPoint, roomId) ? null : EDenialReason.OutsideScope;
    }

    /// <summary>
    ///     Indicates whether an active credential opens an access point.
    /// </summary>
    /// <remarks>
    ///     Every active credential opens the main entrance and the elevator. A guest key card also opens the door of
    ///     its own room. A staff credential opens the areas of its scope: service areas are the service corridors and
    ///     the storage rooms, all rooms are the room doors, and all areas are every access point.
    /// </remarks>
    private static bool OpensAccessPoint(Credential credential, EAccessPoint accessPoint, int? roomId)
    {
        if (accessPoint is EAccessPoint.MainEntrance or EAccessPoint.Elevator) return true;

        if (credential.Type == ECredentialType.GuestKeyCard)
            return accessPoint == EAccessPoint.RoomDoor && roomId is not null && roomId == credential.RoomId;

        return credential.Scope switch
        {
            EAccessScope.AllAreas => true,
            EAccessScope.AllRooms => accessPoint == EAccessPoint.RoomDoor,
            EAccessScope.ServiceAreas => accessPoint is EAccessPoint.ServiceCorridor or EAccessPoint.StorageRoom,
            _ => false
        };
    }
}
