namespace HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Movement recorded by a stock adjustment.
/// </summary>
public enum EStockOperation
{
    /// <summary>
    ///     Stock entering a storage location.
    /// </summary>
    In,

    /// <summary>
    ///     Stock leaving a storage location.
    /// </summary>
    Out,

    /// <summary>
    ///     Stock moved from one storage location to another.
    /// </summary>
    Transfer
}
