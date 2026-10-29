using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

namespace HosteraBackend.API.AccessControl.Domain.Model.Queries;

/// <summary>
///     Query to get an RFID credential by the number of its key card
/// </summary>
/// <param name="CardNumber">The CardNumber to search</param>
public record GetRfidCredentialByCardNumberQuery(CardNumber CardNumber);
