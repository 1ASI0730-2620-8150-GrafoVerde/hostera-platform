using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming an <see cref="AccessEvent" /> aggregate into an
///     <see cref="AccessEventResource" /> for REST representation.
/// </summary>
public static class AccessEventResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts an <see cref="AccessEvent" /> aggregate to its <see cref="AccessEventResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="AccessEvent" /> to convert. Must not be null.</param>
    /// <returns>An <see cref="AccessEventResource" /> representing the access event.</returns>
    public static AccessEventResource ToResourceFromEntity(AccessEvent entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new AccessEventResource(
            entity.Id,
            entity.PropertyId,
            entity.OccurredAt,
            entity.CredentialId,
            entity.CardId.Value,
            entity.HolderName,
            AccessControlEnumNames.ToName(entity.HolderType),
            AccessControlEnumNames.ToName(entity.AccessPoint),
            entity.RoomId,
            AccessControlEnumNames.ToName(entity.Result),
            entity.DenialReason is { } reason ? AccessControlEnumNames.ToName(reason) : null);
    }
}
