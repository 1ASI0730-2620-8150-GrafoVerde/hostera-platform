using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;

namespace HosteraBackend.API.Rooms.Application.QueryServices;

/// <summary>
///     Represents the rate plan query service of the Rooms bounded context.
/// </summary>
public interface IRatePlanQueryService
{
    /// <summary>
    ///     Handles the get all rate plans by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllRatePlansByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rate plans of the property.</returns>
    Task<IEnumerable<RatePlan>> Handle(GetAllRatePlansByPropertyIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get rate plan by id query.
    /// </summary>
    /// <param name="query">The <see cref="GetRatePlanByIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rate plan, or null when it does not exist.</returns>
    Task<RatePlan?> Handle(GetRatePlanByIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get daily rates by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetDailyRatesByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The daily rates of the period.</returns>
    Task<IEnumerable<DailyRate>> Handle(GetDailyRatesByPropertyIdQuery query, CancellationToken cancellationToken);
}
