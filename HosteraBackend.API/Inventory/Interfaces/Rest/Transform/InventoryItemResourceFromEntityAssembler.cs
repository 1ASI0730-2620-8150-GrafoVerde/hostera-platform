using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.Entities;
using HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming an <see cref="InventoryItem" /> aggregate into an
///     <see cref="InventoryItemResource" /> for REST representation.
/// </summary>
public static class InventoryItemResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts an <see cref="InventoryItem" /> aggregate to its <see cref="InventoryItemResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="InventoryItem" /> to convert. Must not be null.</param>
    /// <returns>An <see cref="InventoryItemResource" /> with the stock by location and the adjustments, newest first.</returns>
    public static InventoryItemResource ToResourceFromEntity(InventoryItem entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new InventoryItemResource(
            entity.Id,
            entity.PropertyId,
            entity.Code.Value,
            entity.Name,
            InventoryEnumNames.ToName(entity.Category),
            InventoryEnumNames.ToName(entity.Unit),
            entity.PrimaryLocationId,
            entity.LowStockThreshold,
            entity.TotalQuantity,
            InventoryEnumNames.ToName(entity.StockCondition),
            entity.Stocks
                .OrderBy(stock => stock.StorageLocationId)
                .Select(stock => new InventoryStockResource(stock.StorageLocationId, stock.Quantity)),
            entity.Adjustments
                .OrderByDescending(adjustment => adjustment.RecordedAt)
                .ThenByDescending(adjustment => adjustment.Id)
                .Select(ToResourceFromEntity));
    }

    private static StockAdjustmentResource ToResourceFromEntity(StockAdjustment adjustment) => new(
        adjustment.Id,
        InventoryEnumNames.ToName(adjustment.Operation),
        adjustment.QuantityChange,
        adjustment.StorageLocationId,
        InventoryEnumNames.ToName(adjustment.Reason),
        adjustment.Note,
        adjustment.Operator,
        adjustment.RecordedAt,
        adjustment.TransferId);
}
