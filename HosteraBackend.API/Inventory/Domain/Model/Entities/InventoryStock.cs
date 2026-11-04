namespace HosteraBackend.API.Inventory.Domain.Model.Entities;

/// <summary>
///     Represents the quantity of an inventory item kept in one storage location.
/// </summary>
/// <remarks>
///     This entity belongs to the InventoryItem aggregate and only changes through it.
/// </remarks>
public class InventoryStock
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected InventoryStock()
    {
    }

    /// <summary>
    ///     Creates an empty stock record for a storage location.
    /// </summary>
    /// <param name="storageLocationId">The identifier of the storage location.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the identifier is not positive.</exception>
    public InventoryStock(int storageLocationId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(storageLocationId);
        StorageLocationId = storageLocationId;
        Quantity = 0;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this stock record.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the inventory item that owns this record.
    /// </summary>
    public int InventoryItemId { get; private set; }

    /// <summary>
    ///     Gets the identifier of the storage location where the stock is kept.
    /// </summary>
    public int StorageLocationId { get; private set; }

    /// <summary>
    ///     Gets the quantity on hand in the storage location.
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    ///     Adds stock to this location.
    /// </summary>
    /// <param name="quantity">The quantity to add.</param>
    internal void Increase(decimal quantity) => Quantity += quantity;

    /// <summary>
    ///     Removes stock from this location.
    /// </summary>
    /// <param name="quantity">The quantity to remove.</param>
    /// <exception cref="InvalidOperationException">Thrown when the quantity exceeds the stock on hand.</exception>
    internal void Decrease(decimal quantity)
    {
        if (quantity > Quantity)
            throw new InvalidOperationException("The stock on hand cannot become negative.");

        Quantity -= quantity;
    }
}
