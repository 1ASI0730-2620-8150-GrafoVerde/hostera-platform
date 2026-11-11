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
///     Rate plans of a property controller.
/// </summary>
/// <param name="ratePlanCommandService">The rate plan command service</param>
/// <param name="ratePlanQueryService">The rate plan query service</param>
/// <param name="propertyQueryService">The property query service, which provides the currency of the plans</param>
[ApiController]
[Route("api/v1/properties/{propertyId:int}/rate-plans")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Rate Plan Endpoints")]
public class PropertyRatePlansController(
    IRatePlanCommandService ratePlanCommandService,
    IRatePlanQueryService ratePlanQueryService,
    IPropertyQueryService propertyQueryService)
    : ControllerBase
{
    /// <summary>
    ///     Gets the rate plans of a property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the rate plans of the property, or not found when the property does not exist.</returns>
    [HttpGet]
    [EndpointSummary("Get the rate plans of a property")]
    [EndpointDescription("Get all rate plans of a property, active and inactive, ordered by name")]
    [ProducesResponseType(typeof(IEnumerable<RatePlanResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRatePlansByPropertyId([FromRoute] int propertyId,
        CancellationToken cancellationToken)
    {
        var property = await propertyQueryService.Handle(new GetPropertyByIdQuery(propertyId), cancellationToken);
        if (property is null)
            return RoomsActionResultAssembler.ToActionResultFromRateError(this, RateError.PropertyNotFound);

        var ratePlans = await ratePlanQueryService.Handle(new GetAllRatePlansByPropertyIdQuery(propertyId),
            cancellationToken);
        return Ok(ratePlans.Select(ratePlan =>
            RatePlanResourceFromEntityAssembler.ToResourceFromEntity(ratePlan, property.Currency)));
    }

    /// <summary>
    ///     Creates a rate plan in a property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="resource">The CreateRatePlanResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the created rate plan, or a problem details response when the property or a room type
    ///     does not exist, a value is not valid or the name is already used.
    /// </returns>
    [HttpPost]
    [EndpointSummary("Create a rate plan")]
    [EndpointDescription("Create an active rate plan in a property. The name must be unique within the property, " +
                         "a refundable plan needs a cancellation policy, and an empty room type list sells every " +
                         "room type")]
    [ProducesResponseType(typeof(RatePlanResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateRatePlan([FromRoute] int propertyId,
        [FromBody] CreateRatePlanResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateRatePlanCommandFromResourceAssembler.ToCommandFromResource(resource, propertyId);
            var result = await ratePlanCommandService.Handle(command, cancellationToken);
            var property = await propertyQueryService.Handle(new GetPropertyByIdQuery(propertyId), cancellationToken);

            return RoomsActionResultAssembler.ToActionResultFromResult(this, result,
                created => CreatedAtAction(
                    nameof(RatePlansController.GetRatePlanById),
                    "RatePlans",
                    new { ratePlanId = created.Id },
                    RatePlanResourceFromEntityAssembler.ToResourceFromEntity(created, property!.Currency)));
        }
        catch (ArgumentException exception)
        {
            return RoomsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }
}
