using System.Net.Mime;
using HosteraBackend.API.Inventory.Application.CommandServices;
using HosteraBackend.API.Inventory.Application.QueryServices;
using HosteraBackend.API.Inventory.Domain.Model.Queries;
using HosteraBackend.API.Inventory.Interfaces.Rest.Resources;
using HosteraBackend.API.Inventory.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Inventory.Interfaces.Rest;

[ApiController]
[Route("api/v1/properties/{propertyId:int}/storage-locations")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Storage Location Endpoints")]
public class PropertyStorageLocationsController(
    IStorageLocationCommandService storageLocationCommandService,
    IStorageLocationQueryService storageLocationQueryService)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the storage locations of a property")]
    [EndpointDescription("Get all storage locations of a property, ordered by name")]
    [ProducesResponseType(typeof(IEnumerable<StorageLocationResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStorageLocationsByPropertyId([FromRoute] int propertyId,
        CancellationToken cancellationToken)
    {
        var query = new GetAllStorageLocationsByPropertyIdQuery(propertyId);
        var storageLocations = await storageLocationQueryService.Handle(query, cancellationToken);
        return Ok(storageLocations.Select(StorageLocationResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [EndpointSummary("Create a storage location")]
    [EndpointDescription("Create a storage location in a property. The code must be unique within the property")]
    [ProducesResponseType(typeof(StorageLocationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStorageLocation([FromRoute] int propertyId,
        [FromBody] CreateStorageLocationResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateStorageLocationCommandFromResourceAssembler.ToCommandFromResource(resource, propertyId);
            var result = await storageLocationCommandService.Handle(command, cancellationToken);

            return InventoryActionResultAssembler.ToActionResultFromResult(this, result,
                created => CreatedAtAction(
                    nameof(StorageLocationsController.GetStorageLocationById),
                    "StorageLocations",
                    new { storageLocationId = created.Id },
                    StorageLocationResourceFromEntityAssembler.ToResourceFromEntity(created)));
        }
        catch (ArgumentException exception)
        {
            return InventoryActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
