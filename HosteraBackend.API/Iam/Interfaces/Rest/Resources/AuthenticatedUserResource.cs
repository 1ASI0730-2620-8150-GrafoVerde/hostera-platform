namespace HosteraBackend.API.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Authenticated user resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the user</param>
/// <param name="Username">The username of the user</param>
/// <param name="FullName">The full name of the person who uses the account</param>
/// <param name="Role">The role of the user in the organization: "OperationsManager" or "Operator"</param>
/// <param name="OrganizationId">The identifier of the organization whose properties the user operates</param>
/// <param name="Token">The bearer token to send in the Authorization header of the next requests</param>
public record AuthenticatedUserResource(
    int Id,
    string Username,
    string FullName,
    string Role,
    int OrganizationId,
    string Token);
