namespace HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Inventory stock resource for REST API
/// </summary>
/// <param name="StorageLocationId">The identifier of the storage location</param>
/// <param name="Quantity">The quantity on hand in the location</param>
public record InventoryStockResource(int StorageLocationId, decimal Quantity);
