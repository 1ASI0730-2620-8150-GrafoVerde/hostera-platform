namespace HosteraBackend.API.Iam.Domain.Model.Queries;

/// <summary>
///     Query to get all users of an organization
/// </summary>
/// <param name="OrganizationId">The identifier of the organization of the users</param>
public record GetAllUsersByOrganizationIdQuery(int OrganizationId);
