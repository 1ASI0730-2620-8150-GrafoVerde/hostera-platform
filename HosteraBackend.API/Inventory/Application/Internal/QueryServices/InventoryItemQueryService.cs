using HosteraBackend.API.Inventory.Application.QueryServices;
using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.Queries;
using HosteraBackend.API.Inventory.Domain.Repositories;

namespace HosteraBackend.API.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Represents the inventory item query service of the Inventory bounded context.
/// </summary>
/// <param name="inventoryItemRepository">
///     The <see cref="IInventoryItemRepository" /> to use.
/// </param>
public class InventoryItemQueryService(IInventoryItemRepository inventoryItemRepository)
    : IInventoryItemQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<InventoryItem>> Handle(GetAllInventoryItemsByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await inventoryItemRepository.FindByPropertyIdAsync(query.PropertyId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InventoryItem?> Handle(GetInventoryItemByIdQuery query, CancellationToken cancellationToken)
    {
        return await inventoryItemRepository.FindByIdAsync(query.InventoryItemId, cancellationToken);
    }
}
