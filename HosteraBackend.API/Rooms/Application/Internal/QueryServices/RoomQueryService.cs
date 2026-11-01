using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Application.Internal.QueryServices;

/// <summary>
///     Represents the room query service of the Rooms bounded context.
/// </summary>
/// <param name="roomRepository">
///     The <see cref="IRoomRepository" /> to use.
/// </param>
public class RoomQueryService(IRoomRepository roomRepository) : IRoomQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Room>> Handle(GetAllRoomsByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await roomRepository.FindByPropertyIdAsync(query.PropertyId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Room?> Handle(GetRoomByIdQuery query, CancellationToken cancellationToken)
    {
        return await roomRepository.FindByIdAsync(query.RoomId, cancellationToken);
    }
}
