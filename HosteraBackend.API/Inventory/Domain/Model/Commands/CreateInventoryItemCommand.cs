using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to create an inventory item in a property
/// </summary>
/// <param name="PropertyId">The identifier of the property that owns the item</param>
/// <param name="Code">The InventoryCode of the item, unique within the property</param>
/// <param name="Name">The name of the item</param>
/// <param name="Category">The EInventoryCategory of the item</param>
/// <param name="Unit">The EInventoryUnit in which the item is counted</param>
/// <param name="PrimaryLocationId">The identifier of the storage location where the item is usually kept</param>
/// <param name="LowStockThreshold">The quantity at which the item needs replenishment</param>
/// <param name="OpeningQuantity">The quantity already on hand in the primary location, or 0</param>
/// <param name="Operator">The name of the staff member who records the opening quantity; required when it is not 0</param>
public record CreateInventoryItemCommand(
    int PropertyId,
    InventoryCode Code,
    string Name,
    EInventoryCategory Category,
    EInventoryUnit Unit,
    int PrimaryLocationId,
    decimal LowStockThreshold,
    decimal OpeningQuantity = 0,
    string? Operator = null);
