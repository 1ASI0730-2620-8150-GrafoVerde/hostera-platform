namespace HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Stock adjustment resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the adjustment</param>
/// <param name="Operation">The operation: in, out or transfer</param>
/// <param name="QuantityChange">The signed change in the location: positive for stock in, negative for stock out</param>
/// <param name="StorageLocationId">The identifier of the storage location whose stock changed</param>
/// <param name="Reason">The reason of the movement</param>
/// <param name="Note">The note recorded with the movement</param>
/// <param name="Operator">The staff member who recorded the movement</param>
/// <param name="RecordedAt">The moment the movement was recorded</param>
/// <param name="TransferId">The identifier shared by the two adjustments of a transfer</param>
public record StockAdjustmentResource(
    int Id,
    string Operation,
    decimal QuantityChange,
    int StorageLocationId,
    string Reason,
    string? Note,
    string Operator,
    DateTimeOffset RecordedAt,
    Guid? TransferId);
