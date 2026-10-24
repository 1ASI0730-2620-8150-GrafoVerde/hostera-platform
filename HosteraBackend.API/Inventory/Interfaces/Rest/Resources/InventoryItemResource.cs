namespace HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Inventory item resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the item</param>
/// <param name="PropertyId">The identifier of the property that owns the item</param>
/// <param name="Code">The code of the item</param>
/// <param name="Name">The name of the item</param>
/// <param name="Category">The category of the item</param>
/// <param name="Unit">The unit in which the item is counted</param>
/// <param name="PrimaryLocationId">The identifier of the primary storage location</param>
/// <param name="LowStockThreshold">The quantity at which the item needs replenishment</param>
/// <param name="TotalQuantity">The quantity on hand across all locations</param>
/// <param name="StockCondition">The stock condition: in-stock, low-stock or out-of-stock</param>
/// <param name="Stocks">The quantity on hand in each storage location</param>
/// <param name="Adjustments">The recorded stock adjustments, newest first</param>
public record InventoryItemResource(
    int Id,
    int PropertyId,
    string Code,
    string Name,
    string Category,
    string Unit,
    int PrimaryLocationId,
    decimal LowStockThreshold,
    decimal TotalQuantity,
    string StockCondition,
    IEnumerable<InventoryStockResource> Stocks,
    IEnumerable<StockAdjustmentResource> Adjustments);
