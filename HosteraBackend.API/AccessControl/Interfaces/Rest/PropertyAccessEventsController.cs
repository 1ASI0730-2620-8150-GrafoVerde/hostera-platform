using System.Net.Mime;
using HosteraBackend.API.AccessControl.Application.QueryServices;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest;

[ApiController]
[Route("api/v1/properties/{propertyId:int}/access-events")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("RFID Access Event Endpoints")]
public class PropertyAccessEventsController(IAccessEventQueryService accessEventQueryService) : ControllerBase
{
    private const int MaxPageSize = 200;

    [HttpGet]
    [EndpointSummary("Get the access events of a property")]
    [EndpointDescription("Get a page of the RFID access events of a property, starting with the most recent. " +
                         "The total number of matching events is returned in the X-Total-Count header")]
    [ProducesResponseType(typeof(IEnumerable<AccessEventResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAccessEventsByPropertyId(
        [FromRoute] int propertyId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] string? result,
        [FromQuery] string? holderName,
        [FromQuery] int? credentialId,
        [FromQuery] string? cardId,
        [FromQuery] string? accessPoint,
        [FromQuery] int? roomId,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            if (page < 1)
                throw new ArgumentException("The page must be 1 or greater.", nameof(page));
            if (pageSize is < 1 or > MaxPageSize)
                throw new ArgumentException($"The page size must be between 1 and {MaxPageSize}.", nameof(pageSize));
            if (from is not null && to is not null && from > to)
                throw new ArgumentException("The start of the search must not be after its end.", nameof(from));

            var query = new GetAllAccessEventsByPropertyIdQuery(
                propertyId,
                from,
                to,
                result is null ? null : AccessControlEnumNames.Parse<EAccessResult>(result, "access result"),
                holderName,
                credentialId,
                cardId is null ? null : new CardId(cardId),
                accessPoint is null ? null : AccessControlEnumNames.Parse<EAccessPoint>(accessPoint, "access point"),
                roomId,
                page,
                pageSize);
            var accessEventPage = await accessEventQueryService.Handle(query, cancellationToken);

            Response.Headers["X-Total-Count"] = accessEventPage.TotalCount.ToString();
            return Ok(accessEventPage.AccessEvents.Select(AccessEventResourceFromEntityAssembler.ToResourceFromEntity));
        }
        catch (ArgumentException exception)
        {
            return AccessEventActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
