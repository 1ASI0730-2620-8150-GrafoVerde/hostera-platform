using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Inventory.Domain.Model.Entities;

/// <summary>
///     Represents a recorded movement of stock of an inventory item.
/// </summary>
/// <remarks>
///     Adjustments are never edited or deleted: together they explain the quantity on hand.
///     A transfer is recorded as two adjustments, one per location, linked by the same TransferId.
/// </remarks>
public class StockAdjustment
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected StockAdjustment()
    {
        Operator = null!;
    }

    /// <summary>
    ///     Creates a stock adjustment.
    /// </summary>
    /// <param name="operation">The EStockOperation that produced the adjustment.</param>
    /// <param name="quantityChange">The signed change: positive when stock enters the location, negative when it leaves.</param>
    /// <param name="storageLocationId">The storage location whose stock changed.</param>
    /// <param name="reason">The EStockAdjustmentReason of the movement.</param>
    /// <param name="note">An optional note about the movement.</param>
    /// <param name="operator">The staff member who recorded the movement.</param>
    /// <param name="recordedAt">The moment the movement was recorded.</param>
    /// <param name="transferId">The identifier shared by the two adjustments of a transfer.</param>
    internal StockAdjustment(
        EStockOperation operation,
        decimal quantityChange,
        int storageLocationId,
        EStockAdjustmentReason reason,
        string? note,
        string @operator,
        DateTimeOffset recordedAt,
        Guid? transferId)
    {
        Operation = operation;
        QuantityChange = quantityChange;
        StorageLocationId = storageLocationId;
        Reason = reason;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        Operator = @operator.Trim();
        RecordedAt = recordedAt;
        TransferId = transferId;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this adjustment.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the adjusted inventory item.
    /// </summary>
    public int InventoryItemId { get; private set; }

    /// <summary>
    ///     Gets the operation that produced the adjustment.
    /// </summary>
    public EStockOperation Operation { get; private set; }

    /// <summary>
    ///     Gets the signed change in the location: positive for stock in, negative for stock out.
    /// </summary>
    public decimal QuantityChange { get; private set; }

    /// <summary>
    ///     Gets the identifier of the storage location whose stock changed.
    /// </summary>
    public int StorageLocationId { get; private set; }

    /// <summary>
    ///     Gets the reason of the movement.
    /// </summary>
    public EStockAdjustmentReason Reason { get; private set; }

    /// <summary>
    ///     Gets the note recorded with the movement.
    /// </summary>
    public string? Note { get; private set; }

    /// <summary>
    ///     Gets the name of the staff member who recorded the movement.
    /// </summary>
    public string Operator { get; private set; }

    /// <summary>
    ///     Gets the moment the movement was recorded.
    /// </summary>
    public DateTimeOffset RecordedAt { get; private set; }

    /// <summary>
    ///     Gets the identifier shared by the two adjustments of a transfer, or null for stock in and out.
    /// </summary>
    public Guid? TransferId { get; private set; }
}
