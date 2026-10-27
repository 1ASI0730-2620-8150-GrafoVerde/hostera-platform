using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Application.Internal.QueryServices;

/// <summary>
///     Represents the rate plan query service of the Rooms bounded context.
/// </summary>
/// <param name="ratePlanRepository">
///     The <see cref="IRatePlanRepository" /> to use.
/// </param>
/// <param name="dailyRateRepository">
///     The <see cref="IDailyRateRepository" /> to use.
/// </param>
public class RatePlanQueryService(IRatePlanRepository ratePlanRepository, IDailyRateRepository dailyRateRepository)
    : IRatePlanQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<RatePlan>> Handle(GetAllRatePlansByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await ratePlanRepository.FindByPropertyIdAsync(query.PropertyId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RatePlan?> Handle(GetRatePlanByIdQuery query, CancellationToken cancellationToken)
    {
        return await ratePlanRepository.FindByIdAsync(query.RatePlanId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DailyRate>> Handle(GetDailyRatesByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await dailyRateRepository.FindByPropertyIdAndPeriodAsync(query.PropertyId, query.StartDate,
            query.EndDate, cancellationToken);
    }
}
