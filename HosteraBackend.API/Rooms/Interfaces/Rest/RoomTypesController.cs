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
///     Room types controller.
/// </summary>
/// <param name="roomTypeCommandService">The room type command service</param>
/// <param name="roomTypeQueryService">The room type query service</param>
[ApiController]
[Route("api/v1/room-types")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Room Type Endpoints")]
public class RoomTypesController(
    IRoomTypeCommandService roomTypeCommandService,
    IRoomTypeQueryService roomTypeQueryService)
    : ControllerBase
{
    /// <summary>
    ///     Gets a room type by its identifier.
    /// </summary>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the room type, or not found when it does not exist.</returns>
    [HttpGet("{roomTypeId:int}")]
    [EndpointSummary("Get a room type by ID")]
    [EndpointDescription("Get a room type by its unique identifier")]
    [ProducesResponseType(typeof(RoomTypeResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoomTypeById([FromRoute] int roomTypeId, CancellationToken cancellationToken)
    {
        var query = new GetRoomTypeByIdQuery(roomTypeId);
        var roomType = await roomTypeQueryService.Handle(query, cancellationToken);

        return RoomsActionResultAssembler.ToActionResultFromQueryResult(this, roomType, RoomError.RoomTypeNotFound,
            found => Ok(RoomTypeResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    /// <summary>
    ///     Updates the details and status of a room type.
    /// </summary>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <param name="resource">The UpdateRoomTypeResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the updated room type, or a problem details response when it does not exist, a value
    ///     is not valid or the name is already used.
    /// </returns>
    [HttpPut("{roomTypeId:int}")]
    [EndpointSummary("Update a room type")]
    [EndpointDescription("Update the details and status of a room type. Its rooms take the new capacity and beds")]
    [ProducesResponseType(typeof(RoomTypeResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateRoomType([FromRoute] int roomTypeId,
        [FromBody] UpdateRoomTypeResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = UpdateRoomTypeCommandFromResourceAssembler.ToCommandFromResource(resource, roomTypeId);
            var result = await roomTypeCommandService.Handle(command, cancellationToken);

            return RoomsActionResultAssembler.ToActionResultFromResult(this, result,
                updated => Ok(RoomTypeResourceFromEntityAssembler.ToResourceFromEntity(updated)));
        }
        catch (ArgumentException exception)
        {
            return RoomsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
