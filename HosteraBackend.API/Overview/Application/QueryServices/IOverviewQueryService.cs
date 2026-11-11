using HosteraBackend.API.Overview.Application.Errors;
using HosteraBackend.API.Overview.Domain.Model.Queries;
using HosteraBackend.API.Overview.Domain.Model.ValueObjects;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Overview.Application.QueryServices;

/// <summary>
///     Represents the query service of the Overview bounded context.
/// </summary>
/// <remarks>
///     The Overview bounded context is read-only, so it has no command service.
/// </remarks>
public interface IOverviewQueryService
{
    /// <summary>
    ///     Handles the get dashboard by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetDashboardByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="OperationalDashboard" />, or the <see cref="OverviewError" /> that prevented it.</returns>
    Task<Result<OperationalDashboard, OverviewError>> Handle(GetDashboardByPropertyIdQuery query,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get all property overviews query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllPropertyOverviewsQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     The overview of each property whose sources are available, and the sources that could not provide data.
    /// </returns>
    Task<(IReadOnlyList<PropertyOverview> Overviews, IReadOnlyList<EDashboardSection> UnavailableSections)> Handle(
        GetAllPropertyOverviewsQuery query, CancellationToken cancellationToken);
}
