using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

namespace HosteraBackend.API.AccessControl.Domain.Model.Commands;

/// <summary>
///     Command to issue an RFID credential for a key card
/// </summary>
/// <param name="CardNumber">The CardNumber of the encoded key card</param>
/// <param name="HolderName">The name of the guest or staff member who receives the card</param>
/// <param name="RoomNumber">The room the card opens</param>
/// <param name="ValidityPeriod">The ValidityPeriod in which the card grants access</param>
public record IssueRfidCredentialCommand(
    CardNumber CardNumber,
    string HolderName,
    string RoomNumber,
    ValidityPeriod ValidityPeriod);
