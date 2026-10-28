using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;

namespace HosteraBackend.API.AccessControl.Application.QueryServices;

/// <summary>
///     Represents the access event query service of the Access Control bounded context.
/// </summary>
public interface IAccessEventQueryService
{
    /// <summary>
    ///     Handles the get all access events by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllAccessEventsByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The requested <see cref="AccessEventPage" />.</returns>
    Task<AccessEventPage> Handle(GetAllAccessEventsByPropertyIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get access event by id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAccessEventByIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access event, or null when it does not exist.</returns>
    Task<AccessEvent?> Handle(GetAccessEventByIdQuery query, CancellationToken cancellationToken);
}
