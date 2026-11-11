using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming an <see cref="AdjustStockResource" /> into an
///     <see cref="AdjustStockCommand" />.
/// </summary>
public static class AdjustStockCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an <see cref="AdjustStockResource" /> to an <see cref="AdjustStockCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the data of the movement. Must not be null.</param>
    /// <param name="inventoryItemId">The identifier of the adjusted item.</param>
    /// <returns>A new <see cref="AdjustStockCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the operation or the reason is not valid.</exception>
    public static AdjustStockCommand ToCommandFromResource(AdjustStockResource resource, int inventoryItemId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new AdjustStockCommand(
            inventoryItemId,
            InventoryEnumNames.Parse<EStockOperation>(resource.Operation, "stock operation"),
            resource.Quantity,
            resource.StorageLocationId,
            resource.DestinationStorageLocationId,
            InventoryEnumNames.Parse<EStockAdjustmentReason>(resource.Reason, "stock adjustment reason"),
            resource.Note,
            resource.Operator);
    }
}
