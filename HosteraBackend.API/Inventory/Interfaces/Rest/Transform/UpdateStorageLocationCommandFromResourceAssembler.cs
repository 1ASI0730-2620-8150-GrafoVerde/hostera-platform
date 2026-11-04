using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming an <see cref="UpdateStorageLocationResource" /> into an
///     <see cref="UpdateStorageLocationCommand" />.
/// </summary>
public static class UpdateStorageLocationCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an <see cref="UpdateStorageLocationResource" /> to an <see cref="UpdateStorageLocationCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the new data of the location. Must not be null.</param>
    /// <param name="storageLocationId">The identifier of the location to update.</param>
    /// <returns>A new <see cref="UpdateStorageLocationCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the type is not valid.</exception>
    public static UpdateStorageLocationCommand ToCommandFromResource(UpdateStorageLocationResource resource,
        int storageLocationId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateStorageLocationCommand(
            storageLocationId,
            resource.Name,
            InventoryEnumNames.Parse<EStorageLocationType>(resource.Type, "storage location type"),
            resource.Area,
            resource.ResponsibleTeam,
            resource.Description);
    }
}
