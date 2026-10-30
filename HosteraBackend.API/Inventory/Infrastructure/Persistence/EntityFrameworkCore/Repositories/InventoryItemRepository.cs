using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Inventory.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IInventoryItemRepository" />.
/// </summary>
/// <remarks>
///     The stock and the adjustments of an item are loaded automatically, as configured in the Inventory mapping.
/// </remarks>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class InventoryItemRepository(AppDbContext context)
    : BaseRepository<InventoryItem>(context), IInventoryItemRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<InventoryItem>> FindByPropertyIdAsync(int propertyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<InventoryItem>()
            .AsSplitQuery()
            .Where(item => item.PropertyId == propertyId)
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByPropertyIdAndCodeAsync(int propertyId, InventoryCode code,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<InventoryItem>()
            .AnyAsync(item => item.PropertyId == propertyId && item.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByStorageLocationIdAsync(int storageLocationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<InventoryItem>()
            .AnyAsync(item => item.PrimaryLocationId == storageLocationId
                              || item.Stocks.Any(stock => stock.StorageLocationId == storageLocationId),
                cancellationToken);
    }
}
