using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="CreateStorageLocationResource" /> into a
///     <see cref="CreateStorageLocationCommand" />.
/// </summary>
public static class CreateStorageLocationCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="CreateStorageLocationResource" /> to a <see cref="CreateStorageLocationCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the data of the location. Must not be null.</param>
    /// <param name="propertyId">The identifier of the property that owns the location.</param>
    /// <returns>A new <see cref="CreateStorageLocationCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the code or the type is not valid.</exception>
    public static CreateStorageLocationCommand ToCommandFromResource(CreateStorageLocationResource resource,
        int propertyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateStorageLocationCommand(
            propertyId,
            new InventoryCode(resource.Code),
            resource.Name,
            InventoryEnumNames.Parse<EStorageLocationType>(resource.Type, "storage location type"),
            resource.Area,
            resource.ResponsibleTeam,
            resource.Description);
    }
}
