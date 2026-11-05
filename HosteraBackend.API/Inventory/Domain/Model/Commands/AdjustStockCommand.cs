using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to record stock entering, leaving or moving between storage locations
/// </summary>
/// <param name="InventoryItemId">The identifier of the adjusted item</param>
/// <param name="Operation">The EStockOperation to record</param>
/// <param name="Quantity">The quantity moved, greater than zero</param>
/// <param name="StorageLocationId">The location where stock enters or leaves; the source location for a transfer</param>
/// <param name="DestinationStorageLocationId">The destination location, required only for a transfer</param>
/// <param name="Reason">The EStockAdjustmentReason that explains the movement</param>
/// <param name="Note">A note about the movement, required when the reason is Other</param>
/// <param name="Operator">The name of the staff member who records the movement</param>
public record AdjustStockCommand(
    int InventoryItemId,
    EStockOperation Operation,
    decimal Quantity,
    int StorageLocationId,
    int? DestinationStorageLocationId,
    EStockAdjustmentReason Reason,
    string? Note,
    string Operator);
