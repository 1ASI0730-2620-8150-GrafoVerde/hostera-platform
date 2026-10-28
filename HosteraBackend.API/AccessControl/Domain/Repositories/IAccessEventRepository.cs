using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.AccessControl.Domain.Repositories;

/// <summary>
///     The access event repository contract.
/// </summary>
public interface IAccessEventRepository : IBaseRepository<AccessEvent>
{
    /// <summary>
    ///     Find a page of the access events of a property that satisfy the criteria of a query
    /// </summary>
    /// <param name="query">The query with the property, the criteria and the page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the access events of the page, starting with the most recent, or empty otherwise.
    /// </returns>
    Task<IEnumerable<AccessEvent>> FindByPropertyIdAsync(GetAllAccessEventsByPropertyIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Count the access events of a property that satisfy the criteria of a query
    /// </summary>
    /// <param name="query">The query with the property and the criteria. Its page is ignored.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     The number of access events that satisfy the criteria.
    /// </returns>
    Task<int> CountByPropertyIdAsync(GetAllAccessEventsByPropertyIdQuery query,
        CancellationToken cancellationToken = default);
}
