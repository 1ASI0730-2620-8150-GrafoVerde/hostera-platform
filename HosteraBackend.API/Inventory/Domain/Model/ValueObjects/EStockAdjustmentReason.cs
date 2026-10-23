namespace HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Reason recorded for a stock adjustment.
/// </summary>
/// <remarks>
///     Each reason is valid only for some operations; see <see cref="StockAdjustmentReasonExtensions" />.
/// </remarks>
public enum EStockAdjustmentReason
{
    /// <summary>
    ///     First quantity recorded for an item. Valid for stock in.
    /// </summary>
    OpeningStock,

    /// <summary>
    ///     Delivery received from a supplier. Valid for stock in.
    /// </summary>
    SupplierDelivery,

    /// <summary>
    ///     Item returned to the storage location. Valid for stock in.
    /// </summary>
    ReturnToStorage,

    /// <summary>
    ///     Correction after a physical count. Valid for stock in and stock out.
    /// </summary>
    InventoryCorrection,

    /// <summary>
    ///     Item used in the daily operation. Valid for stock out.
    /// </summary>
    OperationalConsumption,

    /// <summary>
    ///     Item discarded because it was damaged or expired. Valid for stock out.
    /// </summary>
    DamagedOrExpired,

    /// <summary>
    ///     Item that went missing. Valid for stock out.
    /// </summary>
    LostItem,

    /// <summary>
    ///     Movement between two storage locations. Valid for transfers only.
    /// </summary>
    InternalTransfer,

    /// <summary>
    ///     Any other reason. Requires a note. Valid for stock in and stock out.
    /// </summary>
    Other
}

/// <summary>
///     Rules that relate each <see cref="EStockAdjustmentReason" /> to the operations it can explain.
/// </summary>
public static class StockAdjustmentReasonExtensions
{
    /// <summary>
    ///     Indicates whether the reason can explain the given operation.
    /// </summary>
    /// <param name="reason">The reason to evaluate.</param>
    /// <param name="operation">The operation being recorded.</param>
    /// <returns>True when the reason is valid for the operation.</returns>
    public static bool IsValidFor(this EStockAdjustmentReason reason, EStockOperation operation) => operation switch
    {
        EStockOperation.In => reason is EStockAdjustmentReason.OpeningStock
            or EStockAdjustmentReason.SupplierDelivery
            or EStockAdjustmentReason.ReturnToStorage
            or EStockAdjustmentReason.InventoryCorrection
            or EStockAdjustmentReason.Other,
        EStockOperation.Out => reason is EStockAdjustmentReason.OperationalConsumption
            or EStockAdjustmentReason.DamagedOrExpired
            or EStockAdjustmentReason.LostItem
            or EStockAdjustmentReason.InventoryCorrection
            or EStockAdjustmentReason.Other,
        EStockOperation.Transfer => reason is EStockAdjustmentReason.InternalTransfer,
        _ => false
    };
}
