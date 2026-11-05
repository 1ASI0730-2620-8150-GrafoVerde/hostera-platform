namespace HosteraBackend.API.AccessControl.Domain.Model.Queries;

/// <summary>
///     Query to get all credentials of a property
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
public record GetAllCredentialsByPropertyIdQuery(int PropertyId);
