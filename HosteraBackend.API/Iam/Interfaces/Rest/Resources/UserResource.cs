namespace HosteraBackend.API.Iam.Interfaces.Rest.Resources;

/// <summary>
///     User resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the user</param>
/// <param name="Username">The username of the user</param>
/// <param name="OrganizationId">The identifier of the organization whose properties the user operates</param>
/// <param name="FullName">The full name of the person who uses the account</param>
/// <param name="Role">The role of the user in the organization: "OperationsManager" or "Operator"</param>
public record UserResource(int Id, string Username, int OrganizationId, string FullName, string Role);
