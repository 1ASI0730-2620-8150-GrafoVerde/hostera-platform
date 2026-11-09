using System.Net.Mime;
using HosteraBackend.API.Overview.Application.QueryServices;
using HosteraBackend.API.Overview.Domain.Model.Queries;
using HosteraBackend.API.Overview.Interfaces.Rest.Resources;
using HosteraBackend.API.Overview.Interfaces.Rest.Transform;
using HosteraBackend.API.Shared.Application.Internal.OutboundServices;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Overview.Interfaces.Rest;

[ApiController]
[Route("api/v1/properties/{propertyId:int}/dashboard")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Operational Overview Endpoints")]
public class PropertyDashboardController(IOverviewQueryService overviewQueryService, IClock clock) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the operational dashboard of a property")]
    [EndpointDescription("Get the occupancy, room statuses, arrivals, revenue, inventory and access summaries of a " +
                         "property on a day. The day defaults to today and the performance range to its last 7 nights. " +
                         "A source that cannot provide data is listed in unavailableSections and its parts are null")]
    [ProducesResponseType(typeof(DashboardResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDashboardByPropertyId(
        [FromRoute] int propertyId,
        [FromQuery] DateOnly? date,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken cancellationToken)
    {
        var day = date ?? clock.Today;
        var lastNight = to ?? day;
        var firstNight = from ?? lastNight.AddDays(-6);

        var query = new GetDashboardByPropertyIdQuery(propertyId, day, firstNight, lastNight);
        var result = await overviewQueryService.Handle(query, cancellationToken);

        return OverviewActionResultAssembler.ToActionResultFromResult(this, result,
            dashboard => Ok(DashboardResourceFromEntityAssembler.ToResourceFromEntity(dashboard)));
    }
}
