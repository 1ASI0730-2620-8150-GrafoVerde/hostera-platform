namespace HosteraBackend.API.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get an inventory item by ID, with its stock by location and its adjustment history
/// </summary>
/// <param name="InventoryItemId">The inventory item ID to search</param>
public record GetInventoryItemByIdQuery(int InventoryItemId);
