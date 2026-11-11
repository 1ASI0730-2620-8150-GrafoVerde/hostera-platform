using System.Net.Mime;
using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;
using HosteraBackend.API.Rooms.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Rooms.Interfaces.Rest;

/// <summary>
///     Rate plans controller.
/// </summary>
/// <param name="ratePlanCommandService">The rate plan command service</param>
/// <param name="ratePlanQueryService">The rate plan query service</param>
/// <param name="propertyQueryService">The property query service, which provides the currency of the plans</param>
[ApiController]
[Route("api/v1/rate-plans")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Rate Plan Endpoints")]
public class RatePlansController(
    IRatePlanCommandService ratePlanCommandService,
    IRatePlanQueryService ratePlanQueryService,
    IPropertyQueryService propertyQueryService)
    : ControllerBase
{
    /// <summary>
    ///     Gets a rate plan by its identifier.
    /// </summary>
    /// <param name="ratePlanId">The identifier of the rate plan.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the rate plan, or not found when it does not exist.</returns>
    [HttpGet("{ratePlanId:int}")]
    [EndpointSummary("Get a rate plan by ID")]
    [EndpointDescription("Get a rate plan by its unique identifier")]
    [ProducesResponseType(typeof(RatePlanResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRatePlanById([FromRoute] int ratePlanId, CancellationToken cancellationToken)
    {
        var ratePlan = await ratePlanQueryService.Handle(new GetRatePlanByIdQuery(ratePlanId), cancellationToken);
        if (ratePlan is null)
            return RoomsActionResultAssembler.ToActionResultFromRateError(this, RateError.RatePlanNotFound);

        return Ok(await ToResourceAsync(ratePlan, cancellationToken));
    }

    /// <summary>
    ///     Updates the conditions and status of a rate plan.
    /// </summary>
    /// <param name="ratePlanId">The identifier of the rate plan.</param>
    /// <param name="resource">The UpdateRatePlanResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the updated rate plan, or a problem details response when it or a room type does not
    ///     exist, a value is not valid or the name is already used.
    /// </returns>
    [HttpPut("{ratePlanId:int}")]
    [EndpointSummary("Update a rate plan")]
    [EndpointDescription("Update the conditions, services, room types and status of a rate plan. " +
                         "Rate plans are made inactive instead of deleted")]
    [ProducesResponseType(typeof(RatePlanResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateRatePlan([FromRoute] int ratePlanId,
        [FromBody] UpdateRatePlanResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = UpdateRatePlanCommandFromResourceAssembler.ToCommandFromResource(resource, ratePlanId);
            var result = await ratePlanCommandService.Handle(command, cancellationToken);

            return await result.Fold<Task<IActionResult>>(
                async updated => Ok(await ToResourceAsync(updated, cancellationToken)),
                error => Task.FromResult(RoomsActionResultAssembler.ToActionResultFromRateError(this, error)));
        }
        catch (ArgumentException exception)
        {
            return RoomsActionResultAssembler.ToActionResultFromInvalidResource(this, exception);
        }
    }

    /// <summary>
    ///     Sets the price of the nights of a room type under the rate plan.
    /// </summary>
    /// <param name="ratePlanId">The identifier of the rate plan.</param>
    /// <param name="resource">The SetDailyRatesResource resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the daily rates of the range, or a problem details response when the plan or the room
    ///     type is not valid or the range is too long.
    /// </returns>
    [HttpPut("{ratePlanId:int}/daily-rates")]
    [EndpointSummary("Set the daily rates of a rate plan")]
    [EndpointDescription("Set the price of each night of a range for a room type sold under the plan. " +
                         "A null amount returns the nights to the base nightly rate of the room type")]
    [ProducesResponseType(typeof(IEnumerable<DailyRateResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SetDailyRates([FromRoute] int ratePlanId,
        [FromBody] SetDailyRatesResource resource, CancellationToken cancellationToken)
    {
        var command = SetDailyRatesCommandFromResourceAssembler.ToCommandFromResource(resource, ratePlanId);
        var result = await ratePlanCommandService.Handle(command, cancellationToken);

        return RoomsActionResultAssembler.ToActionResultFromResult(this, result,
            dailyRates => Ok(dailyRates.Select(DailyRateResourceFromEntityAssembler.ToResourceFromEntity)));
    }

    private async Task<RatePlanResource> ToResourceAsync(RatePlan ratePlan, CancellationToken cancellationToken)
    {
        var property = await propertyQueryService.Handle(new GetPropertyByIdQuery(ratePlan.PropertyId),
            cancellationToken);
        return RatePlanResourceFromEntityAssembler.ToResourceFromEntity(ratePlan, property?.Currency ?? string.Empty);
    }
}
