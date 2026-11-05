using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to update the information of an inventory item
/// </summary>
/// <remarks>
///     The code of an item cannot change, and its unit cannot change after its first stock adjustment.
/// </remarks>
/// <param name="InventoryItemId">The identifier of the item to update</param>
/// <param name="Name">The new name of the item</param>
/// <param name="Category">The new EInventoryCategory of the item</param>
/// <param name="Unit">The new EInventoryUnit of the item</param>
/// <param name="PrimaryLocationId">The identifier of the new primary storage location</param>
/// <param name="LowStockThreshold">The new quantity at which the item needs replenishment</param>
public record UpdateInventoryItemCommand(
    int InventoryItemId,
    string Name,
    EInventoryCategory Category,
    EInventoryUnit Unit,
    int PrimaryLocationId,
    decimal LowStockThreshold);
