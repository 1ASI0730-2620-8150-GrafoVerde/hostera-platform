namespace HosteraBackend.API.Iam.Interfaces.Rest.Resources;

/// <summary>
///     User resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the user</param>
/// <param name="Username">The username of the user</param>
/// <param name="OrganizationId">The identifier of the organization whose properties the user operates</param>
public record UserResource(int Id, string Username, int OrganizationId);
