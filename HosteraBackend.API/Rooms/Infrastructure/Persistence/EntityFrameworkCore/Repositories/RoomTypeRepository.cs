using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Rooms.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IRoomTypeRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class RoomTypeRepository(AppDbContext context)
    : BaseRepository<RoomType>(context), IRoomTypeRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<RoomType>> FindByPropertyIdAsync(int propertyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<RoomType>()
            .Where(roomType => roomType.PropertyId == propertyId)
            .OrderBy(roomType => roomType.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByPropertyIdAndNameAsync(int propertyId, string name,
        int? excludedRoomTypeId = null, CancellationToken cancellationToken = default)
    {
        return await Context.Set<RoomType>()
            .AnyAsync(roomType => roomType.PropertyId == propertyId
                                  && roomType.Name == name
                                  && roomType.Id != excludedRoomTypeId, cancellationToken);
    }
}
