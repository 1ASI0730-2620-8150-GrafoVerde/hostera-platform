using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Inventory.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IStorageLocationRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class StorageLocationRepository(AppDbContext context)
    : BaseRepository<StorageLocation>(context), IStorageLocationRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<StorageLocation>> FindByPropertyIdAsync(int propertyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<StorageLocation>()
            .Where(location => location.PropertyId == propertyId)
            .OrderBy(location => location.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByPropertyIdAndCodeAsync(int propertyId, InventoryCode code,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<StorageLocation>()
            .AnyAsync(location => location.PropertyId == propertyId && location.Code == code, cancellationToken);
    }
}
