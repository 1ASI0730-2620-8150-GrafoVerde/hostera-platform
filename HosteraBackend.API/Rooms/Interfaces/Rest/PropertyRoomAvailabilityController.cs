using System.Net.Mime;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;
using HosteraBackend.API.Rooms.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HosteraBackend.API.Rooms.Interfaces.Rest;

/// <summary>
///     Room availability of a property controller.
/// </summary>
/// <param name="roomQueryService">The room query service</param>
[ApiController]
[Route("api/v1/properties/{propertyId:int}/room-availability")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Room Status Endpoints")]
public class PropertyRoomAvailabilityController(IRoomQueryService roomQueryService) : ControllerBase
{
    /// <summary>
    ///     Gets the status of each room of a property on the start date and the following six days.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="startDate">The first day of the week. Required.</param>
    /// <param name="status">An optional day status that the rooms must have on the start date.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the day statuses of each matching room, or bad request when the status is not valid.
    /// </returns>
    [HttpGet]
    [EndpointSummary("Get the room availability of a property")]
    [EndpointDescription("Get the status of each room of a property on the start date and the following six days. " +
                         "The optional status keeps only the rooms with that status on the start date")]
    [ProducesResponseType(typeof(IEnumerable<RoomAvailabilityResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRoomAvailabilityByPropertyId([FromRoute] int propertyId,
        [FromQuery] [BindRequired] DateOnly startDate, [FromQuery] string? status, CancellationToken cancellationToken)
    {
        try
        {
            EDayStatus? dayStatus = string.IsNullOrWhiteSpace(status)
                ? null
                : RoomsEnumNames.Parse<EDayStatus>(status, "day status");
            var query = new GetRoomAvailabilityByPropertyIdQuery(propertyId, startDate, dayStatus);
            var availability = await roomQueryService.Handle(query, cancellationToken);
            return Ok(availability.Select(RoomAvailabilityResourceFromValueObjectAssembler.ToResourceFromValueObject));
        }
        catch (ArgumentException exception)
        {
            return RoomsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
