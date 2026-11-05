using System.Net.Mime;
using HosteraBackend.API.Inventory.Application.CommandServices;
using HosteraBackend.API.Inventory.Application.Errors;
using HosteraBackend.API.Inventory.Application.QueryServices;
using HosteraBackend.API.Inventory.Domain.Model.Queries;
using HosteraBackend.API.Inventory.Interfaces.Rest.Resources;
using HosteraBackend.API.Inventory.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Inventory.Interfaces.Rest;

[ApiController]
[Route("api/v1/inventory-items")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Inventory Item Endpoints")]
public class InventoryItemsController(
    IInventoryItemCommandService inventoryItemCommandService,
    IInventoryItemQueryService inventoryItemQueryService)
    : ControllerBase
{
    [HttpGet("{inventoryItemId:int}")]
    [EndpointSummary("Get an inventory item by ID")]
    [EndpointDescription("Get an inventory item with its stock by location and its adjustment history")]
    [ProducesResponseType(typeof(InventoryItemResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInventoryItemById([FromRoute] int inventoryItemId,
        CancellationToken cancellationToken)
    {
        var query = new GetInventoryItemByIdQuery(inventoryItemId);
        var inventoryItem = await inventoryItemQueryService.Handle(query, cancellationToken);

        return InventoryActionResultAssembler.ToActionResultFromQueryResult(this, inventoryItem,
            InventoryError.InventoryItemNotFound,
            found => Ok(InventoryItemResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    [HttpPut("{inventoryItemId:int}")]
    [EndpointSummary("Update an inventory item")]
    [EndpointDescription(
        "Update the information of an inventory item. The code cannot change, and the unit cannot change after the first adjustment")]
    [ProducesResponseType(typeof(InventoryItemResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateInventoryItem([FromRoute] int inventoryItemId,
        [FromBody] UpdateInventoryItemResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = UpdateInventoryItemCommandFromResourceAssembler.ToCommandFromResource(resource, inventoryItemId);
            var result = await inventoryItemCommandService.Handle(command, cancellationToken);

            return InventoryActionResultAssembler.ToActionResultFromResult(this, result,
                updated => Ok(InventoryItemResourceFromEntityAssembler.ToResourceFromEntity(updated)));
        }
        catch (ArgumentException exception)
        {
            return InventoryActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }

    [HttpPost("{inventoryItemId:int}/stock-adjustments")]
    [EndpointSummary("Adjust the stock of an inventory item")]
    [EndpointDescription(
        "Record stock entering, leaving or moving between storage locations. The stock on hand can never become negative")]
    [ProducesResponseType(typeof(InventoryItemResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AdjustStock([FromRoute] int inventoryItemId,
        [FromBody] AdjustStockResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = AdjustStockCommandFromResourceAssembler.ToCommandFromResource(resource, inventoryItemId);
            var result = await inventoryItemCommandService.Handle(command, cancellationToken);

            return InventoryActionResultAssembler.ToActionResultFromResult(this, result,
                adjusted => CreatedAtAction(
                    nameof(GetInventoryItemById),
                    new { inventoryItemId = adjusted.Id },
                    InventoryItemResourceFromEntityAssembler.ToResourceFromEntity(adjusted)));
        }
        catch (ArgumentException exception)
        {
            return InventoryActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
