using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming an <see cref="UpdateInventoryItemResource" /> into an
///     <see cref="UpdateInventoryItemCommand" />.
/// </summary>
public static class UpdateInventoryItemCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an <see cref="UpdateInventoryItemResource" /> to an <see cref="UpdateInventoryItemCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the new data of the item. Must not be null.</param>
    /// <param name="inventoryItemId">The identifier of the item to update.</param>
    /// <returns>A new <see cref="UpdateInventoryItemCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the category or the unit is not valid.</exception>
    public static UpdateInventoryItemCommand ToCommandFromResource(UpdateInventoryItemResource resource,
        int inventoryItemId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateInventoryItemCommand(
            inventoryItemId,
            resource.Name,
            InventoryEnumNames.Parse<EInventoryCategory>(resource.Category, "inventory category"),
            InventoryEnumNames.Parse<EInventoryUnit>(resource.Unit, "inventory unit"),
            resource.PrimaryLocationId,
            resource.LowStockThreshold);
    }
}
