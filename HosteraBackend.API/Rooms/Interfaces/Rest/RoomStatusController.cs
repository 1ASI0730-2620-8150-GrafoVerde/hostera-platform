using System.Net.Mime;
using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;
using HosteraBackend.API.Rooms.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Rooms.Interfaces.Rest;

/// <summary>
///     Operational status of a room controller.
/// </summary>
/// <param name="statusPeriodCommandService">The status period command service</param>
[ApiController]
[Route("api/v1/rooms/{roomId:int}/status")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Room Status Endpoints")]
public class RoomStatusController(IStatusPeriodCommandService statusPeriodCommandService) : ControllerBase
{
    /// <summary>
    ///     Sets or releases the operational status of a room over a date range.
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="resource">The SetRoomStatusResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing every status period of the room after the change, or a problem details response when
    ///     the room does not exist, the range is not valid or the reason is missing.
    /// </returns>
    [HttpPut]
    [EndpointSummary("Set the status of a room")]
    [EndpointDescription("Set blocked, out-of-service or needs-cleaning over a date range, or available to release " +
                         "it. Existing periods in the range are trimmed, split or removed in the same operation")]
    [ProducesResponseType(typeof(IEnumerable<StatusPeriodResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetRoomStatus([FromRoute] int roomId, [FromBody] SetRoomStatusResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = SetRoomStatusCommandFromResourceAssembler.ToCommandFromResource(resource, roomId);
            var result = await statusPeriodCommandService.Handle(command, cancellationToken);

            return RoomsActionResultAssembler.ToActionResultFromResult(this, result,
                statusPeriods => Ok(statusPeriods.Select(StatusPeriodResourceFromEntityAssembler.ToResourceFromEntity)));
        }
        catch (ArgumentException exception)
        {
            return RoomsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
