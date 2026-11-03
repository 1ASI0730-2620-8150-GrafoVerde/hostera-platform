using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Rooms.Application.CommandServices;

/// <summary>
///     Represents the room command service of the Rooms bounded context.
/// </summary>
public interface IRoomCommandService
{
    /// <summary>
    ///     Handles the create room command.
    /// </summary>
    /// <param name="command">The <see cref="CreateRoomCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created <see cref="Room" />, or the <see cref="RoomError" /> that prevented it.</returns>
    Task<Result<Room, RoomError>> Handle(CreateRoomCommand command, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the update room command.
    /// </summary>
    /// <param name="command">The <see cref="UpdateRoomCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated <see cref="Room" />, or the <see cref="RoomError" /> that prevented it.</returns>
    Task<Result<Room, RoomError>> Handle(UpdateRoomCommand command, CancellationToken cancellationToken);
}
