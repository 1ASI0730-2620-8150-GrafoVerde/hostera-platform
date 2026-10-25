using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.Queries;

namespace HosteraBackend.API.Inventory.Application.QueryServices;

/// <summary>
///     Represents the storage location query service of the Inventory bounded context.
/// </summary>
public interface IStorageLocationQueryService
{
    /// <summary>
    ///     Handles the get all storage locations by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllStorageLocationsByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The storage locations of the property.</returns>
    Task<IEnumerable<StorageLocation>> Handle(GetAllStorageLocationsByPropertyIdQuery query,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get storage location by id query.
    /// </summary>
    /// <param name="query">The <see cref="GetStorageLocationByIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The storage location, or null when it does not exist.</returns>
    Task<StorageLocation?> Handle(GetStorageLocationByIdQuery query, CancellationToken cancellationToken);
}
