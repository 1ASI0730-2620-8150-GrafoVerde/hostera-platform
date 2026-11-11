using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Application.Internal.QueryServices;

/// <summary>
///     Represents the room type query service of the Rooms bounded context.
/// </summary>
/// <param name="roomTypeRepository">
///     The <see cref="IRoomTypeRepository" /> to use.
/// </param>
public class RoomTypeQueryService(IRoomTypeRepository roomTypeRepository) : IRoomTypeQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<RoomType>> Handle(GetAllRoomTypesByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await roomTypeRepository.FindByPropertyIdAsync(query.PropertyId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RoomType?> Handle(GetRoomTypeByIdQuery query, CancellationToken cancellationToken)
    {
        return await roomTypeRepository.FindByIdAsync(query.RoomTypeId, cancellationToken);
    }
}
