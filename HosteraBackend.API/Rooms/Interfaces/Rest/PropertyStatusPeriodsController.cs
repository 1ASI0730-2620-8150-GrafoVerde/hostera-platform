using System.Net.Mime;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;
using HosteraBackend.API.Rooms.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Rooms.Interfaces.Rest;

/// <summary>
///     Status periods of a property controller.
/// </summary>
/// <param name="statusPeriodQueryService">The status period query service</param>
[ApiController]
[Route("api/v1/properties/{propertyId:int}/status-periods")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Room Status Endpoints")]
public class PropertyStatusPeriodsController(IStatusPeriodQueryService statusPeriodQueryService) : ControllerBase
{
    /// <summary>
    ///     Gets the status periods of the rooms of a property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="from">The first day of the range, or null for no lower limit.</param>
    /// <param name="to">The last day of the range, or null for no upper limit.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the status periods that overlap the range, ordered by room and start date.</returns>
    [HttpGet]
    [EndpointSummary("Get the status periods of a property")]
    [EndpointDescription("Get the status periods of the rooms of a property. The optional from and to dates keep " +
                         "only the periods that overlap them")]
    [ProducesResponseType(typeof(IEnumerable<StatusPeriodResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatusPeriodsByPropertyId([FromRoute] int propertyId,
        [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken cancellationToken)
    {
        var query = new GetAllStatusPeriodsByPropertyIdQuery(propertyId, from, to);
        var statusPeriods = await statusPeriodQueryService.Handle(query, cancellationToken);
        return Ok(statusPeriods.Select(StatusPeriodResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
