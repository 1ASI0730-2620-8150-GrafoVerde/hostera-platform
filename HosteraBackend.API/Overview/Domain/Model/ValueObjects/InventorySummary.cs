namespace HosteraBackend.API.Overview.Domain.Model.ValueObjects;

/// <summary>
///     Stock condition of the inventory items of a property
/// </summary>
/// <param name="ItemsCount">The inventory items of the property</param>
/// <param name="LowStockItems">The items that reached their low stock threshold</param>
/// <param name="OutOfStockItems">The items without quantity on hand</param>
public record InventorySummary(int ItemsCount, int LowStockItems, int OutOfStockItems)
{
    /// <summary>
    ///     Gets the items that need replenishment.
    /// </summary>
    public int ItemsToReplenish => LowStockItems + OutOfStockItems;
}
