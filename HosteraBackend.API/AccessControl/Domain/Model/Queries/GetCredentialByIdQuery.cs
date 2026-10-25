namespace HosteraBackend.API.AccessControl.Domain.Model.Queries;

/// <summary>
///     Query to get a credential by ID
/// </summary>
/// <param name="Id">The credential ID to search</param>
public record GetCredentialByIdQuery(int Id);
