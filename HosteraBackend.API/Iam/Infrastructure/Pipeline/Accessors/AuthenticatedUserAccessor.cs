using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Iam.Infrastructure.Pipeline.Middleware.Components;
using HosteraBackend.API.Iam.Interfaces.ACL;

namespace HosteraBackend.API.Iam.Infrastructure.Pipeline.Accessors;

/// <summary>
///     Reads the signed-in user that the <see cref="RequestAuthorizationMiddleware" /> stored in the current request.
/// </summary>
/// <param name="httpContextAccessor">The accessor of the current HTTP context.</param>
public class AuthenticatedUserAccessor(IHttpContextAccessor httpContextAccessor) : IAuthenticatedUserAccessor
{
    /// <inheritdoc />
    public AuthenticatedUser? AuthenticatedUser =>
        httpContextAccessor.HttpContext?.Items[RequestAuthorizationMiddleware.UserItemKey] is User user
            ? new AuthenticatedUser(user.Id, user.Username, user.OrganizationId)
            : null;
}
