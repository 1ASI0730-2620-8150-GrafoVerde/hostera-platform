using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="StorageLocation" /> aggregate into a
///     <see cref="StorageLocationResource" /> for REST representation.
/// </summary>
public static class StorageLocationResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="StorageLocation" /> aggregate to its <see cref="StorageLocationResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="StorageLocation" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="StorageLocationResource" /> representing the location.</returns>
    public static StorageLocationResource ToResourceFromEntity(StorageLocation entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new StorageLocationResource(
            entity.Id,
            entity.PropertyId,
            entity.Code.Value,
            entity.Name,
            InventoryEnumNames.ToName(entity.Type),
            entity.Area,
            entity.ResponsibleTeam,
            entity.Description);
    }
}
