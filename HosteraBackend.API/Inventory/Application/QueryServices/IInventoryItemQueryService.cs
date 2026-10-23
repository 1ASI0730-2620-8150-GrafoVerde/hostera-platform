using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.Queries;

namespace HosteraBackend.API.Inventory.Application.QueryServices;

/// <summary>
///     Represents the inventory item query service of the Inventory bounded context.
/// </summary>
public interface IInventoryItemQueryService
{
    /// <summary>
    ///     Handles the get all inventory items by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllInventoryItemsByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The inventory items of the property.</returns>
    Task<IEnumerable<InventoryItem>> Handle(GetAllInventoryItemsByPropertyIdQuery query,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get inventory item by id query.
    /// </summary>
    /// <param name="query">The <see cref="GetInventoryItemByIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The inventory item, or null when it does not exist.</returns>
    Task<InventoryItem?> Handle(GetInventoryItemByIdQuery query, CancellationToken cancellationToken);
}
