using HosteraBackend.API.Inventory.Application.Errors;
using HosteraBackend.API.Inventory.Application.QueryServices;
using HosteraBackend.API.Inventory.Domain.Model.Queries;
using HosteraBackend.API.Inventory.Interfaces.Rest.Transform;
using HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Authorization;

/// <summary>
///     Finds the property of the storage locations and inventory items addressed by route parameter.
/// </summary>
/// <param name="storageLocationQueryService">The storage location query service</param>
/// <param name="inventoryItemQueryService">The inventory item query service</param>
public class InventoryPropertyScopeResolver(
    IStorageLocationQueryService storageLocationQueryService,
    IInventoryItemQueryService inventoryItemQueryService)
    : IPropertyScopeResolver
{
    private const string StorageLocationIdKey = "storageLocationId";
    private const string InventoryItemIdKey = "inventoryItemId";

    /// <inheritdoc />
    public IReadOnlyCollection<string> RouteKeys { get; } = [StorageLocationIdKey, InventoryItemIdKey];

    /// <inheritdoc />
    public async Task<int?> FindPropertyIdAsync(string routeKey, int id, CancellationToken cancellationToken)
    {
        return routeKey == StorageLocationIdKey
            ? (await storageLocationQueryService.Handle(new GetStorageLocationByIdQuery(id), cancellationToken))
            ?.PropertyId
            : (await inventoryItemQueryService.Handle(new GetInventoryItemByIdQuery(id), cancellationToken))
            ?.PropertyId;
    }

    /// <inheritdoc />
    public IActionResult ToNotFoundResult(string routeKey, ControllerBase controller)
    {
        var error = routeKey == StorageLocationIdKey
            ? InventoryError.StorageLocationNotFound
            : InventoryError.InventoryItemNotFound;
        return InventoryActionResultAssembler.ToActionResultFromQueryResult<object>(controller, null, error,
            controller.Ok);
    }
}
