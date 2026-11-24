namespace HosteraBackend.API.Iam.Interfaces.ACL;

/// <summary>
///     The user who signed in to send the current request, as other bounded contexts see it.
/// </summary>
/// <param name="Id">The identifier of the user.</param>
/// <param name="Username">The username of the user.</param>
/// <param name="OrganizationId">The identifier of the organization whose properties the user operates.</param>
public record AuthenticatedUser(int Id, string Username, int OrganizationId);
