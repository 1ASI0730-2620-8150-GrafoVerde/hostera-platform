using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Application.Internal.QueryServices;

/// <summary>
///     Represents the status period query service of the Rooms bounded context.
/// </summary>
/// <param name="statusPeriodRepository">
///     The <see cref="IStatusPeriodRepository" /> to use.
/// </param>
public class StatusPeriodQueryService(IStatusPeriodRepository statusPeriodRepository) : IStatusPeriodQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<StatusPeriod>> Handle(GetAllStatusPeriodsByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await statusPeriodRepository.FindByPropertyIdAsync(query.PropertyId, query.From, query.To,
            cancellationToken);
    }
}
