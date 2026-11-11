using System.Net.Mime;
using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;
using HosteraBackend.API.Rooms.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Rooms.Interfaces.Rest;

/// <summary>
///     Rooms of a property controller.
/// </summary>
/// <param name="roomCommandService">The room command service</param>
/// <param name="roomQueryService">The room query service</param>
[ApiController]
[Route("api/v1/properties/{propertyId:int}/rooms")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Room Endpoints")]
public class PropertyRoomsController(
    IRoomCommandService roomCommandService,
    IRoomQueryService roomQueryService)
    : ControllerBase
{
    /// <summary>
    ///     Gets the rooms of a property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the rooms of the property, ordered by number.</returns>
    [HttpGet]
    [EndpointSummary("Get the rooms of a property")]
    [EndpointDescription("Get all rooms of a property, ordered by number")]
    [ProducesResponseType(typeof(IEnumerable<RoomResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomsByPropertyId([FromRoute] int propertyId,
        CancellationToken cancellationToken)
    {
        var query = new GetAllRoomsByPropertyIdQuery(propertyId);
        var rooms = await roomQueryService.Handle(query, cancellationToken);
        return Ok(rooms.Select(RoomResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>
    ///     Creates a room in a property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="resource">The CreateRoomResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the created room, or a problem details response when the property or room type does
    ///     not exist, the room type is inactive or the number is already used.
    /// </returns>
    [HttpPost]
    [EndpointSummary("Create a room")]
    [EndpointDescription("Create a room in a property with an active room type of the same property. " +
                         "The number must be unique within the property")]
    [ProducesResponseType(typeof(RoomResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateRoom([FromRoute] int propertyId, [FromBody] CreateRoomResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateRoomCommandFromResourceAssembler.ToCommandFromResource(resource, propertyId);
        var result = await roomCommandService.Handle(command, cancellationToken);

        return RoomsActionResultAssembler.ToActionResultFromResult(this, result,
            created => CreatedAtAction(
                nameof(RoomsController.GetRoomById),
                "Rooms",
                new { roomId = created.Id },
                RoomResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }
}
