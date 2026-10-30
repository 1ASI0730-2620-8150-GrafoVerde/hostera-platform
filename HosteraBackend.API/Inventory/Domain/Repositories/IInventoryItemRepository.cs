using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Inventory.Domain.Repositories;

/// <summary>
///     Defines the persistence operations for <see cref="InventoryItem" /> aggregates.
/// </summary>
/// <remarks>
///     Items are always loaded with their stock by location and their adjustment history.
/// </remarks>
public interface IInventoryItemRepository : IBaseRepository<InventoryItem>
{
    /// <summary>
    ///     Lists the inventory items of a property, ordered by name.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the inventory items of the property.</returns>
    Task<IEnumerable<InventoryItem>> FindByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Indicates whether a property already has an inventory item with the given code.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="code">The code to search.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing true when the code is already used in the property.</returns>
    Task<bool> ExistsByPropertyIdAndCodeAsync(int propertyId, InventoryCode code,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Indicates whether any inventory item is assigned to a storage location.
    /// </summary>
    /// <param name="storageLocationId">The identifier of the storage location.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing true when an item has the location as primary or keeps stock there.</returns>
    Task<bool> ExistsByStorageLocationIdAsync(int storageLocationId, CancellationToken cancellationToken = default);
}
