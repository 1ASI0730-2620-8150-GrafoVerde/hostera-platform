using System.Net.Mime;
using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;
using HosteraBackend.API.Rooms.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Rooms.Interfaces.Rest;

/// <summary>
///     Daily rates of a property controller.
/// </summary>
/// <param name="ratePlanQueryService">The rate plan query service</param>
[ApiController]
[Route("api/v1/properties/{propertyId:int}/daily-rates")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Rate Plan Endpoints")]
public class PropertyDailyRatesController(IRatePlanQueryService ratePlanQueryService) : ControllerBase
{
    /// <summary>
    ///     Gets the daily rates of a property, optionally between two nights.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="startDate">The first night of the range, or null for no lower limit.</param>
    /// <param name="endDate">The last night of the range, or null for no upper limit.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the daily rates of every rate plan and room type in the range, or bad request when the
    ///     range ends before it starts.
    /// </returns>
    [HttpGet]
    [EndpointSummary("Get the daily rates of a property")]
    [EndpointDescription("Get the daily rates of every rate plan and room type of a property, ordered by night. " +
                         "The optional startDate and endDate limit the nights. Nights without a daily rate use the " +
                         "base nightly rate of the room type")]
    [ProducesResponseType(typeof(IEnumerable<DailyRateResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDailyRatesByPropertyId([FromRoute] int propertyId,
        [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate, CancellationToken cancellationToken)
    {
        if (startDate > endDate)
            return RoomsActionResultAssembler.ToActionResultFromRateError(this, RateError.InvalidDateRange);

        var query = new GetDailyRatesByPropertyIdQuery(propertyId, startDate, endDate);
        var dailyRates = await ratePlanQueryService.Handle(query, cancellationToken);
        return Ok(dailyRates.Select(DailyRateResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
