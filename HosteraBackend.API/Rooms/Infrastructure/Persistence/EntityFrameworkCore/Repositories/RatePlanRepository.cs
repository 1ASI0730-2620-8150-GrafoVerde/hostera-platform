using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Entities;
using HosteraBackend.API.Rooms.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Rooms.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IRatePlanRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class RatePlanRepository(AppDbContext context)
    : BaseRepository<RatePlan>(context), IRatePlanRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<RatePlan>> FindByPropertyIdAsync(int propertyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<RatePlan>()
            .Where(ratePlan => ratePlan.PropertyId == propertyId)
            .OrderBy(ratePlan => ratePlan.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByPropertyIdAndNameAsync(int propertyId, string name,
        int? excludedRatePlanId = null, CancellationToken cancellationToken = default)
    {
        return await Context.Set<RatePlan>()
            .AnyAsync(ratePlan => ratePlan.PropertyId == propertyId
                                  && ratePlan.Name == name
                                  && ratePlan.Id != excludedRatePlanId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByRoomTypeIdAsync(int roomTypeId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<RatePlanRoomType>()
            .AnyAsync(roomType => roomType.RoomTypeId == roomTypeId, cancellationToken);
    }
}
