using System.Net.Mime;
using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;
using HosteraBackend.API.Rooms.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Rooms.Interfaces.Rest;

/// <summary>
///     Rooms controller.
/// </summary>
/// <param name="roomCommandService">The room command service</param>
/// <param name="roomQueryService">The room query service</param>
[ApiController]
[Route("api/v1/rooms")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Room Endpoints")]
public class RoomsController(
    IRoomCommandService roomCommandService,
    IRoomQueryService roomQueryService)
    : ControllerBase
{
    /// <summary>
    ///     Gets a room by its identifier.
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the room, or not found when it does not exist.</returns>
    [HttpGet("{roomId:int}")]
    [EndpointSummary("Get a room by ID")]
    [EndpointDescription("Get a room by its unique identifier")]
    [ProducesResponseType(typeof(RoomResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoomById([FromRoute] int roomId, CancellationToken cancellationToken)
    {
        var query = new GetRoomByIdQuery(roomId);
        var room = await roomQueryService.Handle(query, cancellationToken);

        return RoomsActionResultAssembler.ToActionResultFromQueryResult(this, room, RoomError.RoomNotFound,
            found => Ok(RoomResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    /// <summary>
    ///     Updates the number, room type and floor of a room.
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="resource">The UpdateRoomResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the updated room, or a problem details response when the room or room type does not
    ///     exist, the new room type is inactive or the number is already used.
    /// </returns>
    [HttpPut("{roomId:int}")]
    [EndpointSummary("Update a room")]
    [EndpointDescription("Update the number, room type and floor of a room. A room can keep an inactive room type, " +
                         "but it cannot move to one")]
    [ProducesResponseType(typeof(RoomResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateRoom([FromRoute] int roomId, [FromBody] UpdateRoomResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateRoomCommandFromResourceAssembler.ToCommandFromResource(resource, roomId);
        var result = await roomCommandService.Handle(command, cancellationToken);

        return RoomsActionResultAssembler.ToActionResultFromResult(this, result,
            updated => Ok(RoomResourceFromEntityAssembler.ToResourceFromEntity(updated)));
    }
}
