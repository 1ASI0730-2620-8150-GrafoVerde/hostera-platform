using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Rooms.Application.QueryServices;

/// <summary>
///     Represents the room query service of the Rooms bounded context.
/// </summary>
public interface IRoomQueryService
{
    /// <summary>
    ///     Handles the get all rooms by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllRoomsByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rooms of the property.</returns>
    Task<IEnumerable<Room>> Handle(GetAllRoomsByPropertyIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get room by id query.
    /// </summary>
    /// <param name="query">The <see cref="GetRoomByIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room, or null when it does not exist.</returns>
    Task<Room?> Handle(GetRoomByIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get room availability by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetRoomAvailabilityByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The day statuses of each matching room of the property, ordered by room number.</returns>
    Task<IEnumerable<RoomAvailability>> Handle(GetRoomAvailabilityByPropertyIdQuery query,
        CancellationToken cancellationToken);
}
