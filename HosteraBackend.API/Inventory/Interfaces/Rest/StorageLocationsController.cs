using System.Net.Mime;
using HosteraBackend.API.Inventory.Application.CommandServices;
using HosteraBackend.API.Inventory.Application.Errors;
using HosteraBackend.API.Inventory.Application.QueryServices;
using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Inventory.Domain.Model.Queries;
using HosteraBackend.API.Inventory.Interfaces.Rest.Resources;
using HosteraBackend.API.Inventory.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Inventory.Interfaces.Rest;

[ApiController]
[Route("api/v1/storage-locations")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Storage Location Endpoints")]
public class StorageLocationsController(
    IStorageLocationCommandService storageLocationCommandService,
    IStorageLocationQueryService storageLocationQueryService)
    : ControllerBase
{
    [HttpGet("{storageLocationId:int}")]
    [EndpointSummary("Get a storage location by ID")]
    [EndpointDescription("Get a storage location by its unique identifier")]
    [ProducesResponseType(typeof(StorageLocationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStorageLocationById([FromRoute] int storageLocationId,
        CancellationToken cancellationToken)
    {
        var query = new GetStorageLocationByIdQuery(storageLocationId);
        var storageLocation = await storageLocationQueryService.Handle(query, cancellationToken);

        return InventoryActionResultAssembler.ToActionResultFromQueryResult(this, storageLocation,
            InventoryError.StorageLocationNotFound,
            found => Ok(StorageLocationResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    [HttpPut("{storageLocationId:int}")]
    [EndpointSummary("Update a storage location")]
    [EndpointDescription("Update the information of a storage location. The code cannot change")]
    [ProducesResponseType(typeof(StorageLocationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStorageLocation([FromRoute] int storageLocationId,
        [FromBody] UpdateStorageLocationResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command =
                UpdateStorageLocationCommandFromResourceAssembler.ToCommandFromResource(resource, storageLocationId);
            var result = await storageLocationCommandService.Handle(command, cancellationToken);

            return InventoryActionResultAssembler.ToActionResultFromResult(this, result,
                updated => Ok(StorageLocationResourceFromEntityAssembler.ToResourceFromEntity(updated)));
        }
        catch (ArgumentException exception)
        {
            return InventoryActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }

    [HttpDelete("{storageLocationId:int}")]
    [EndpointSummary("Delete a storage location")]
    [EndpointDescription("Delete a storage location that no inventory item uses")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteStorageLocation([FromRoute] int storageLocationId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteStorageLocationCommand(storageLocationId);
        var result = await storageLocationCommandService.Handle(command, cancellationToken);

        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, _ => NoContent());
    }
}
