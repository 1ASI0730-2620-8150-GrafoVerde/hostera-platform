using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Rooms.Application.CommandServices;

/// <summary>
///     Represents the room type command service of the Rooms bounded context.
/// </summary>
public interface IRoomTypeCommandService
{
    /// <summary>
    ///     Handles the create room type command.
    /// </summary>
    /// <param name="command">The <see cref="CreateRoomTypeCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created <see cref="RoomType" />, or the <see cref="RoomError" /> that prevented it.</returns>
    Task<Result<RoomType, RoomError>> Handle(CreateRoomTypeCommand command, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the update room type command.
    /// </summary>
    /// <param name="command">The <see cref="UpdateRoomTypeCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated <see cref="RoomType" />, or the <see cref="RoomError" /> that prevented it.</returns>
    Task<Result<RoomType, RoomError>> Handle(UpdateRoomTypeCommand command, CancellationToken cancellationToken);
}
