using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Rooms.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IDailyRateRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class DailyRateRepository(AppDbContext context)
    : BaseRepository<DailyRate>(context), IDailyRateRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<DailyRate>> FindByPropertyIdAndPeriodAsync(int propertyId, DateOnly startDate,
        DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<DailyRate>()
            .Where(rate => rate.PropertyId == propertyId && rate.Date >= startDate && rate.Date <= endDate)
            .OrderBy(rate => rate.Date)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DailyRate>> FindByRatePlanAndRoomTypeAsync(int ratePlanId, int roomTypeId,
        DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<DailyRate>()
            .Where(rate => rate.RatePlanId == ratePlanId
                           && rate.RoomTypeId == roomTypeId
                           && rate.Date >= startDate
                           && rate.Date <= endDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DailyRate>> FindByRoomTypeIdAsync(int roomTypeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<DailyRate>()
            .Where(rate => rate.RoomTypeId == roomTypeId)
            .ToListAsync(cancellationToken);
    }
}
