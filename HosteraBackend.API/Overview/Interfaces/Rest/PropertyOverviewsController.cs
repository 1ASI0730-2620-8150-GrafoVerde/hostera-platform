using System.Net.Mime;
using HosteraBackend.API.Overview.Application.QueryServices;
using HosteraBackend.API.Overview.Domain.Model.Queries;
using HosteraBackend.API.Overview.Interfaces.Rest.Resources;
using HosteraBackend.API.Overview.Interfaces.Rest.Transform;
using HosteraBackend.API.Shared.Application.Internal.OutboundServices;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Overview.Interfaces.Rest;

[ApiController]
[Route("api/v1/property-overviews")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Operational Overview Endpoints")]
public class PropertyOverviewsController(IOverviewQueryService overviewQueryService, IClock clock) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Compare the properties")]
    [EndpointDescription("Get the overview of every property on a day to compare their occupancy and the rooms " +
                         "that need attention. The day defaults to today")]
    [ProducesResponseType(typeof(PropertyOverviewsResource), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPropertyOverviews([FromQuery] DateOnly? date,
        CancellationToken cancellationToken)
    {
        var day = date ?? clock.Today;
        var (overviews, unavailableSections) =
            await overviewQueryService.Handle(new GetAllPropertyOverviewsQuery(day), cancellationToken);

        return Ok(PropertyOverviewResourceFromEntityAssembler.ToResourceFromEntities(day, overviews,
            unavailableSections));
    }
}
