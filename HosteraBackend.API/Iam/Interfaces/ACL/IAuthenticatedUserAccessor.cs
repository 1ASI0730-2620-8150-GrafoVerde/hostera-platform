namespace HosteraBackend.API.Iam.Interfaces.ACL;

/// <summary>
///     Gives other bounded contexts the user who signed in to send the current request.
/// </summary>
public interface IAuthenticatedUserAccessor
{
    /// <summary>
    ///     Gets the signed-in user of the current request, or null when the request has no valid bearer token, such
    ///     as a sign-in request.
    /// </summary>
    AuthenticatedUser? AuthenticatedUser { get; }
}
