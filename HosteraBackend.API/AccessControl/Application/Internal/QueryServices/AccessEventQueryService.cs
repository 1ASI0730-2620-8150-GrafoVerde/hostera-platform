using HosteraBackend.API.AccessControl.Application.QueryServices;
using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.AccessControl.Domain.Repositories;

namespace HosteraBackend.API.AccessControl.Application.Internal.QueryServices;

/// <summary>
///     Represents the access event query service of the Access Control bounded context.
/// </summary>
/// <param name="accessEventRepository">
///     The <see cref="IAccessEventRepository" /> to use.
/// </param>
public class AccessEventQueryService(IAccessEventRepository accessEventRepository) : IAccessEventQueryService
{
    /// <inheritdoc />
    public async Task<AccessEventPage> Handle(GetAllAccessEventsByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        var accessEvents = await accessEventRepository.FindByPropertyIdAsync(query, cancellationToken);
        var totalCount = await accessEventRepository.CountByPropertyIdAsync(query, cancellationToken);
        return new AccessEventPage(accessEvents, query.Page, query.PageSize, totalCount);
    }

    /// <inheritdoc />
    public async Task<AccessEvent?> Handle(GetAccessEventByIdQuery query, CancellationToken cancellationToken)
    {
        return await accessEventRepository.FindByIdAsync(query.Id, cancellationToken);
    }
}
