using System.Data.Common;
using HosteraBackend.API.Inventory.Application.QueryServices;
using HosteraBackend.API.Inventory.Domain.Model.Queries;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Overview.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Overview.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Anti-corruption layer that reads the stock condition of the Inventory bounded context.
/// </summary>
/// <param name="inventoryItemQueryService">
///     The <see cref="IInventoryItemQueryService" /> of the Inventory bounded context.
/// </param>
/// <param name="logger">
///     The logger used to report a source that cannot provide its data.
/// </param>
public class ExternalInventoryService(
    IInventoryItemQueryService inventoryItemQueryService,
    ILogger<ExternalInventoryService> logger)
{
    /// <summary>
    ///     Fetches the stock condition of the inventory items of a property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The inventory summary, or null when the inventory cannot be read.</returns>
    public async Task<InventorySummary?> FetchInventorySummaryAsync(int propertyId,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = (await inventoryItemQueryService.Handle(
                new GetAllInventoryItemsByPropertyIdQuery(propertyId), cancellationToken)).ToList();

            return new InventorySummary(
                items.Count,
                items.Count(item => item.StockCondition == EStockCondition.LowStock),
                items.Count(item => item.StockCondition == EStockCondition.OutOfStock));
        }
        catch (DbException exception)
        {
            logger.LogWarning(exception, "The inventory of property {PropertyId} could not be read.", propertyId);
            return null;
        }
    }
}
