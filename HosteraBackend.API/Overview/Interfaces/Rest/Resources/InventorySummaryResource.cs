namespace HosteraBackend.API.Overview.Interfaces.Rest.Resources;

/// <summary>
///     Inventory summary resource for REST API
/// </summary>
/// <param name="ItemsCount">The inventory items of the property</param>
/// <param name="LowStockItems">The items that reached their low stock threshold</param>
/// <param name="OutOfStockItems">The items without quantity on hand</param>
/// <param name="ItemsToReplenish">The items that need replenishment</param>
public record InventorySummaryResource(int ItemsCount, int LowStockItems, int OutOfStockItems, int ItemsToReplenish);
