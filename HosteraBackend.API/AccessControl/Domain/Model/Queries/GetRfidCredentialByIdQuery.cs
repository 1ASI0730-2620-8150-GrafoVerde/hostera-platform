namespace HosteraBackend.API.AccessControl.Domain.Model.Queries;

/// <summary>
///     Query to get an RFID credential by ID
/// </summary>
/// <param name="Id">The credential ID to search</param>
public record GetRfidCredentialByIdQuery(int Id);
