using System.Net.Mime;
using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;
using HosteraBackend.API.Rooms.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
    private const int MaxNightsPerRange = 366;

    /// <summary>
    ///     Gets the daily rates of a property between two nights.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="startDate">The first night of the range. Required.</param>
    /// <param name="endDate">The last night of the range. Required.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A response containing the daily rates of every rate plan and room type in the range, or bad request when the
    ///     range is not valid.
    /// </returns>
    [HttpGet]
    [EndpointSummary("Get the daily rates of a property")]
    [EndpointDescription("Get the daily rates of every rate plan and room type of a property between two nights, " +
                         "up to 366 nights. Nights without a daily rate use the base nightly rate of the room type")]
    [ProducesResponseType(typeof(IEnumerable<DailyRateResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDailyRatesByPropertyId([FromRoute] int propertyId,
        [FromQuery] [BindRequired] DateOnly startDate, [FromQuery] [BindRequired] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var nights = endDate.DayNumber - startDate.DayNumber + 1;
        if (nights < 1 || nights > MaxNightsPerRange)
            return RoomsActionResultAssembler.ToActionResultFromRateError(this, RateError.InvalidDateRange);

        var query = new GetDailyRatesByPropertyIdQuery(propertyId, startDate, endDate);
        var dailyRates = await ratePlanQueryService.Handle(query, cancellationToken);
        return Ok(dailyRates.Select(DailyRateResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
