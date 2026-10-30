using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

namespace HosteraBackend.API.AccessControl.Domain.Model.Commands;

/// <summary>
///     Command to issue the key cards of a guest
/// </summary>
/// <param name="PropertyId">The identifier of the property</param>
/// <param name="BookingId">The identifier of the booking</param>
/// <param name="BookingCode">The code of the booking</param>
/// <param name="RoomId">The identifier of the room the cards open</param>
/// <param name="HolderName">The name of the guest who holds the cards</param>
/// <param name="ValidityPeriod">The ValidityPeriod of the stay</param>
/// <param name="CardIds">The CardId of each encoded card</param>
/// <param name="IssuedBy">The identifier of the operator who issues the cards</param>
public record IssueGuestKeyCardsCommand(
    int PropertyId,
    int BookingId,
    string BookingCode,
    int RoomId,
    string HolderName,
    ValidityPeriod ValidityPeriod,
    IReadOnlyList<CardId> CardIds,
    int IssuedBy);
