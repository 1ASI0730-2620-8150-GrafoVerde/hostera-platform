using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="Credential" /> aggregate into a
///     <see cref="CredentialResource" /> for REST representation.
/// </summary>
public static class CredentialResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="Credential" /> aggregate to its <see cref="CredentialResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="Credential" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="CredentialResource" /> representing the credential.</returns>
    public static CredentialResource ToResourceFromEntity(Credential entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new CredentialResource(
            entity.Id,
            entity.PropertyId,
            entity.CardId.Value,
            AccessControlEnumNames.ToName(entity.Type),
            AccessControlEnumNames.ToName(entity.StatusAt(DateTimeOffset.UtcNow)),
            entity.HolderName,
            entity.BookingId,
            entity.BookingCode,
            entity.RoomId,
            entity.StaffMemberId,
            entity.Scope is { } scope ? AccessControlEnumNames.ToName(scope) : null,
            entity.ValidityPeriod.ValidFrom,
            entity.ValidityPeriod.ValidUntil,
            entity.IssuedAt,
            entity.IssuedBy,
            entity.Revocation?.At,
            entity.Revocation?.OperatorId,
            entity.Revocation is { } revocation ? AccessControlEnumNames.ToName(revocation.Reason) : null,
            entity.Revocation?.Note);
    }
}
