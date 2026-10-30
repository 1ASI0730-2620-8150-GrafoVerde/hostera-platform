namespace HosteraBackend.API.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to remove an empty secondary storage location from an inventory item
/// </summary>
/// <param name="InventoryItemId">The identifier of the inventory item</param>
/// <param name="StorageLocationId">The identifier of the storage location to remove from the item</param>
public record UnassignStorageLocationCommand(int InventoryItemId, int StorageLocationId);
