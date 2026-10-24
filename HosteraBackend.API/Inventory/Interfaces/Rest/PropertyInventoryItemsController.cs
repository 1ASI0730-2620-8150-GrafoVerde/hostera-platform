using System.Net.Mime;
using HosteraBackend.API.Inventory.Application.CommandServices;
using HosteraBackend.API.Inventory.Application.QueryServices;
using HosteraBackend.API.Inventory.Domain.Model.Queries;
using HosteraBackend.API.Inventory.Interfaces.Rest.Resources;
using HosteraBackend.API.Inventory.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Inventory.Interfaces.Rest;

[ApiController]
[Route("api/v1/properties/{propertyId:int}/inventory-items")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Inventory Item Endpoints")]
public class PropertyInventoryItemsController(
    IInventoryItemCommandService inventoryItemCommandService,
    IInventoryItemQueryService inventoryItemQueryService)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the inventory items of a property")]
    [EndpointDescription("Get all inventory items of a property with their stock by location and stock condition")]
    [ProducesResponseType(typeof(IEnumerable<InventoryItemResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInventoryItemsByPropertyId([FromRoute] int propertyId,
        CancellationToken cancellationToken)
    {
        var query = new GetAllInventoryItemsByPropertyIdQuery(propertyId);
        var inventoryItems = await inventoryItemQueryService.Handle(query, cancellationToken);
        return Ok(inventoryItems.Select(InventoryItemResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [EndpointSummary("Create an inventory item")]
    [EndpointDescription(
        "Create an inventory item in a property. The code must be unique within the property and the primary location must belong to it")]
    [ProducesResponseType(typeof(InventoryItemResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateInventoryItem([FromRoute] int propertyId,
        [FromBody] CreateInventoryItemResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateInventoryItemCommandFromResourceAssembler.ToCommandFromResource(resource, propertyId);
            var result = await inventoryItemCommandService.Handle(command, cancellationToken);

            return InventoryActionResultAssembler.ToActionResultFromResult(this, result,
                created => CreatedAtAction(
                    nameof(InventoryItemsController.GetInventoryItemById),
                    "InventoryItems",
                    new { inventoryItemId = created.Id },
                    InventoryItemResourceFromEntityAssembler.ToResourceFromEntity(created)));
        }
        catch (ArgumentException exception)
        {
            return InventoryActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
