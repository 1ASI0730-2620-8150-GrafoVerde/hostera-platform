using HosteraBackend.API.Inventory.Application.QueryServices;
using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.Queries;
using HosteraBackend.API.Inventory.Domain.Repositories;

namespace HosteraBackend.API.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Represents the storage location query service of the Inventory bounded context.
/// </summary>
/// <param name="storageLocationRepository">
///     The <see cref="IStorageLocationRepository" /> to use.
/// </param>
public class StorageLocationQueryService(IStorageLocationRepository storageLocationRepository)
    : IStorageLocationQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<StorageLocation>> Handle(GetAllStorageLocationsByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await storageLocationRepository.FindByPropertyIdAsync(query.PropertyId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<StorageLocation?> Handle(GetStorageLocationByIdQuery query, CancellationToken cancellationToken)
    {
        return await storageLocationRepository.FindByIdAsync(query.StorageLocationId, cancellationToken);
    }
}
