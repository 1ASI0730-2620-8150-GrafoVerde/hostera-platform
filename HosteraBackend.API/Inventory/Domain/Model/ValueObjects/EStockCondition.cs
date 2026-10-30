namespace HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Condition of an inventory item compared with its low stock threshold.
/// </summary>
public enum EStockCondition
{
    /// <summary>
    ///     The quantity on hand is above the threshold.
    /// </summary>
    InStock,

    /// <summary>
    ///     The quantity on hand reached the threshold and needs replenishment.
    /// </summary>
    LowStock,

    /// <summary>
    ///     There is no quantity on hand.
    /// </summary>
    OutOfStock
}
