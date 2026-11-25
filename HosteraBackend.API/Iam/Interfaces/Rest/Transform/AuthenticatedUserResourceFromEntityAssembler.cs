using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Iam.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a signed-in <see cref="User" /> and its token into an
///     <see cref="AuthenticatedUserResource" />.
/// </summary>
public static class AuthenticatedUserResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="User" /> aggregate and its token to an <see cref="AuthenticatedUserResource" />.
    /// </summary>
    /// <param name="entity">The signed-in <see cref="User" />. Must not be null.</param>
    /// <param name="token">The token issued to the user. Must not be empty.</param>
    /// <returns>
    ///     A new <see cref="AuthenticatedUserResource" /> instance, with the role written by name, such as
    ///     <c>OperationsManager</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the token is empty.</exception>
    public static AuthenticatedUserResource ToResourceFromEntity(User entity, string token)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(token);
        return new AuthenticatedUserResource(entity.Id, entity.Username, entity.FullName, entity.Role.ToString(),
            entity.OrganizationId, token);
    }
}
