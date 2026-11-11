using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Rooms.Application.CommandServices;

/// <summary>
///     Represents the status period command service of the Rooms bounded context.
/// </summary>
public interface IStatusPeriodCommandService
{
    /// <summary>
    ///     Handles the set room status command.
    /// </summary>
    /// <param name="command">The <see cref="SetRoomStatusCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     Every status period of the room after the change, or the <see cref="RoomError" /> that prevented it.
    /// </returns>
    Task<Result<IEnumerable<StatusPeriod>, RoomError>> Handle(SetRoomStatusCommand command,
        CancellationToken cancellationToken);
}
