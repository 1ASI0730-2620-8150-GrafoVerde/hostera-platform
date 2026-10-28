using System.Net.Mime;
using HosteraBackend.API.AccessControl.Application.CommandServices;
using HosteraBackend.API.AccessControl.Application.QueryServices;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest;

[ApiController]
[Route("api/v1/access-events")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("RFID Access Event Endpoints")]
public class AccessEventsController(
    IAccessEventCommandService accessEventCommandService,
    IAccessEventQueryService accessEventQueryService)
    : ControllerBase
{
    [HttpGet("{accessEventId:int}")]
    [EndpointSummary("Get an access event by ID")]
    [EndpointDescription("Get an RFID access event by its unique identifier")]
    [ProducesResponseType(typeof(AccessEventResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccessEventById([FromRoute] int accessEventId,
        CancellationToken cancellationToken)
    {
        var query = new GetAccessEventByIdQuery(accessEventId);
        var accessEvent = await accessEventQueryService.Handle(query, cancellationToken);

        return accessEvent is null
            ? AccessEventActionResultAssembler.ToActionResultFromMissingAccessEvent(this)
            : Ok(AccessEventResourceFromEntityAssembler.ToResourceFromEntity(accessEvent));
    }

    [HttpPost]
    [EndpointSummary("Record an access event")]
    [EndpointDescription("Record a card presented at an RFID reader. The result is decided from the status and the scope of its credential")]
    [ProducesResponseType(typeof(AccessEventResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordAccessEvent([FromBody] RecordAccessEventResource resource,
        CancellationToken cancellationToken)
    {
        if (!AccessEventCommandFromResourceAssembler.IsRegisteredAccessPoint(resource.AccessPoint))
            return AccessEventActionResultAssembler.ToActionResultFromUnregisteredAccessPoint(this);

        try
        {
            var command = AccessEventCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await accessEventCommandService.Handle(command, cancellationToken);

            return AccessEventActionResultAssembler.ToActionResultFromResult(this, result,
                recorded => CreatedAtAction(
                    nameof(GetAccessEventById),
                    new { accessEventId = recorded.Id },
                    AccessEventResourceFromEntityAssembler.ToResourceFromEntity(recorded)));
        }
        catch (ArgumentException exception)
        {
            return AccessEventActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
