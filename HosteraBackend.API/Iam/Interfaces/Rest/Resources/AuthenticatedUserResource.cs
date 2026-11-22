namespace HosteraBackend.API.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Authenticated user resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the user</param>
/// <param name="Username">The username of the user</param>
/// <param name="Token">The bearer token to send in the Authorization header of the next requests</param>
public record AuthenticatedUserResource(int Id, string Username, string Token);
