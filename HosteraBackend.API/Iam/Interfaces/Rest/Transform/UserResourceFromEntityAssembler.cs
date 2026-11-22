using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Iam.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="User" /> aggregate into a <see cref="UserResource" /> for
///     REST representation.
/// </summary>
public static class UserResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="User" /> aggregate to its <see cref="UserResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="User" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="UserResource" /> representing the user, without its password hash.</returns>
    public static UserResource ToResourceFromEntity(User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new UserResource(entity.Id, entity.Username);
    }
}
