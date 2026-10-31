using System.Net.Mime;
using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;
using HosteraBackend.API.Rooms.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Rooms.Interfaces.Rest;

/// <summary>
///     Room types of a property controller.
/// </summary>
/// <param name="roomTypeCommandService">The room type command service</param>
/// <param name="roomTypeQueryService">The room type query service</param>
[ApiController]
[Route("api/v1/properties/{propertyId:int}/room-types")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Room Type Endpoints")]
public class PropertyRoomTypesController(
    IRoomTypeCommandService roomTypeCommandService,
    IRoomTypeQueryService roomTypeQueryService)
    : ControllerBase
{
    /// <summary>
    ///     Gets the room types of a property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the room types of the property, ordered by name.</returns>
    [HttpGet]
    [EndpointSummary("Get the room types of a property")]
    [EndpointDescription("Get all room types of a property, ordered by name")]
    [ProducesResponseType(typeof(IEnumerable<RoomTypeResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomTypesByPropertyId([FromRoute] int propertyId,
        CancellationToken cancellationToken)
    {
        var query = new GetAllRoomTypesByPropertyIdQuery(propertyId);
        var roomTypes = await roomTypeQueryService.Handle(query, cancellationToken);
        return Ok(roomTypes.Select(RoomTypeResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>
    ///     Creates a room type in a property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="resource">The CreateRoomTypeResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the created room type, or a problem details response when the property does not exist
    ///     or the name is already used.
    /// </returns>
    [HttpPost]
    [EndpointSummary("Create a room type")]
    [EndpointDescription("Create an active room type in a property. The name must be unique within the property")]
    [ProducesResponseType(typeof(RoomTypeResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateRoomType([FromRoute] int propertyId,
        [FromBody] CreateRoomTypeResource resource, CancellationToken cancellationToken)
    {
        var command = CreateRoomTypeCommandFromResourceAssembler.ToCommandFromResource(resource, propertyId);
        var result = await roomTypeCommandService.Handle(command, cancellationToken);

        return RoomsActionResultAssembler.ToActionResultFromResult(this, result,
            created => CreatedAtAction(
                nameof(RoomTypesController.GetRoomTypeById),
                "RoomTypes",
                new { roomTypeId = created.Id },
                RoomTypeResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }
}
