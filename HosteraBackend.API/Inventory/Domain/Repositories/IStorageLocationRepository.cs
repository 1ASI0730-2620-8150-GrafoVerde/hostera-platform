using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Inventory.Domain.Repositories;

/// <summary>
///     Defines the persistence operations for <see cref="StorageLocation" /> aggregates.
/// </summary>
public interface IStorageLocationRepository : IBaseRepository<StorageLocation>
{
    /// <summary>
    ///     Lists the storage locations of a property, ordered by name.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the storage locations of the property.</returns>
    Task<IEnumerable<StorageLocation>> FindByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Indicates whether a property already has a storage location with the given code.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="code">The code to search.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing true when the code is already used in the property.</returns>
    Task<bool> ExistsByPropertyIdAndCodeAsync(int propertyId, InventoryCode code,
        CancellationToken cancellationToken = default);
}
