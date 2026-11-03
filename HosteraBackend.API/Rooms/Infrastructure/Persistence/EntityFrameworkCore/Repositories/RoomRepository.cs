using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Rooms.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IRoomRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class RoomRepository(AppDbContext context)
    : BaseRepository<Room>(context), IRoomRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Room>> FindByPropertyIdAsync(int propertyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Room>()
            .Where(room => room.PropertyId == propertyId)
            .OrderBy(room => room.Number)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByPropertyIdAndNumberAsync(int propertyId, string number,
        int? excludedRoomId = null, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Room>()
            .AnyAsync(room => room.PropertyId == propertyId
                              && room.Number == number
                              && room.Id != excludedRoomId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByRoomTypeIdAsync(int roomTypeId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Room>().AnyAsync(room => room.RoomTypeId == roomTypeId, cancellationToken);
    }
}
