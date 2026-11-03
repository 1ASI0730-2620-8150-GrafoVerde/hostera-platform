using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;

namespace HosteraBackend.API.Rooms.Application.QueryServices;

/// <summary>
///     Represents the status period query service of the Rooms bounded context.
/// </summary>
public interface IStatusPeriodQueryService
{
    /// <summary>
    ///     Handles the get all status periods by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllStatusPeriodsByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The status periods of the rooms of the property that overlap the requested range.</returns>
    Task<IEnumerable<StatusPeriod>> Handle(GetAllStatusPeriodsByPropertyIdQuery query,
        CancellationToken cancellationToken);
}
