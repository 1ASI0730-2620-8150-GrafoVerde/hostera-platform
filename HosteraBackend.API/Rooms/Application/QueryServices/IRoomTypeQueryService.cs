using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;

namespace HosteraBackend.API.Rooms.Application.QueryServices;

/// <summary>
///     Represents the room type query service of the Rooms bounded context.
/// </summary>
public interface IRoomTypeQueryService
{
    /// <summary>
    ///     Handles the get all room types by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllRoomTypesByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room types of the property.</returns>
    Task<IEnumerable<RoomType>> Handle(GetAllRoomTypesByPropertyIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get room type by id query.
    /// </summary>
    /// <param name="query">The <see cref="GetRoomTypeByIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room type, or null when it does not exist.</returns>
    Task<RoomType?> Handle(GetRoomTypeByIdQuery query, CancellationToken cancellationToken);
}
