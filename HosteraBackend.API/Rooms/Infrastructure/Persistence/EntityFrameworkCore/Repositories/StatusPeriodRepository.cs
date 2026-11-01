using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Rooms.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IStatusPeriodRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class StatusPeriodRepository(AppDbContext context)
    : BaseRepository<StatusPeriod>(context), IStatusPeriodRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<StatusPeriod>> FindByPropertyIdAsync(int propertyId, DateOnly? from, DateOnly? to,
        CancellationToken cancellationToken = default)
    {
        var statusPeriods = Context.Set<StatusPeriod>().Where(period => period.PropertyId == propertyId);
        if (from is { } firstDay)
            statusPeriods = statusPeriods.Where(period => period.EndDate >= firstDay);
        if (to is { } lastDay)
            statusPeriods = statusPeriods.Where(period => period.StartDate <= lastDay);

        return await statusPeriods
            .OrderBy(period => period.RoomId)
            .ThenBy(period => period.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StatusPeriod>> FindByRoomIdAndPeriodAsync(int roomId, DateOnly startDate,
        DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<StatusPeriod>()
            .Where(period => period.RoomId == roomId && period.StartDate <= endDate && period.EndDate >= startDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StatusPeriod>> FindByRoomIdAsync(int roomId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<StatusPeriod>()
            .Where(period => period.RoomId == roomId)
            .OrderBy(period => period.StartDate)
            .ToListAsync(cancellationToken);
    }
}
